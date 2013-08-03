﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Phone.Media.Capture;
using FlashLightApi;

namespace TorchLight.IoC
{
    internal class FlashLightServiceImpl:IFlashLightService
    {
        private AudioVideoCaptureDevice _avDevice;
        public bool IsFlashOn { get; set; }
        public bool IsInitialized { get; set; }

        public event EventHandler<bool> FinishedInitialization;

        const CameraSensorLocation SensorLocation = CameraSensorLocation.Back;

        public async void Init()
        {
            _avDevice = await GetCameraDevice();
            IsInitialized = true;

            if (FinishedInitialization != null)
            {
                FinishedInitialization.Invoke(this, IsInitialized);
            }
        }

        public async Task TurnFlashOn()
        {
            try
            {
                if (IsFlashSupported())
                {
                    ChangeTorchMode(VideoTorchMode.On);
                    SetFlashLightToMaxIntensity();

                    IsFlashOn = true;
                }
                else
                {
                    //ShowWhiteScreenInsteadOfCameraTorch();
                }

            }
            catch (Exception ex)
            {
                // Flashlight isn't supported on this device, instead show a White Screen as the flash light
                //ShowWhiteScreenInsteadOfCameraTorch();
            }
        }

private void SetFlashLightToMaxIntensity()
        {
            _avDevice.SetProperty(KnownCameraAudioVideoProperties.VideoTorchPower,
                AudioVideoCaptureDevice.GetSupportedPropertyRange(SensorLocation,
                    KnownCameraAudioVideoProperties.VideoTorchPower).Max);
        }
        
        public async Task TurnFlashOff()
        {
            try
            {
                if (IsFlashSupported())
                {
                    ChangeTorchMode(VideoTorchMode.Off);
                    SetFlashLightToMaxIntensity();

                    IsFlashOn = true;
                    IsFlashOn = false;
                }
                else
                {
                    //ShowWhiteScreenInsteadOfCameraTorch();
                }

            }
            catch (Exception ex)
            {
                // Flashlight isn't supported on this device, instead show a White Screen as the flash light
                //ShowWhiteScreenInsteadOfCameraTorch();
            }
        }
        
        private void ChangeTorchMode(VideoTorchMode newTorchMode)
        {
            _avDevice.SetProperty(KnownCameraAudioVideoProperties.VideoTorchMode, newTorchMode);

            // set flash power to maxinum
                //_avDevice.SetProperty(KnownCameraAudioVideoProperties.VideoTorchPower,
                //    AudioVideoCaptureDevice.GetSupportedPropertyRange(SensorLocation,
                //        KnownCameraAudioVideoProperties.VideoTorchPower).Min);
 
        }

        private static bool IsFlashSupported()
        {
            var supportedCameraModes = AudioVideoCaptureDevice
                .GetSupportedPropertyValues(SensorLocation, KnownCameraAudioVideoProperties.VideoTorchMode);
            var flashSupported = supportedCameraModes.ToList().Contains((UInt32) VideoTorchMode.On);
            return flashSupported;
        }

        private static async Task<AudioVideoCaptureDevice> GetCameraDevice()
        {
            var avDevice = await AudioVideoCaptureDevice.OpenAsync(SensorLocation,
                AudioVideoCaptureDevice.GetAvailableCaptureResolutions(SensorLocation).First());
            return avDevice;
        }

    }
}