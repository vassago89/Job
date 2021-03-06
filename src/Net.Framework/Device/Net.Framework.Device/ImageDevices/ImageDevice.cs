﻿using System;
using System.Threading.Tasks;

using Net.Framework.Data.ImageDatas;

namespace Net.Framework.Device.ImageDevices
{
    public interface IImageDevice : IDevice
    {
        int Width { get; }
        int Height { get; }
        int Channels { get; }
        double FrameRate { get; }

        event Action<IImageData> DeviceGrabbed;

        Task<IImageData> Grab();
        bool ContinuousGrab();
        bool Stop();
    }

    public interface IImageDeviceNonGrabber<TDeviceInfo> : IImageDevice where TDeviceInfo : ImageDeviceInfo
    {
        TDeviceInfo Info { get; }
    }

    public interface IImageDeviceOnGrabber<TDeviceInfo, TGrabber> : IImageDeviceNonGrabber<TDeviceInfo> where TDeviceInfo : ImageDeviceInfo
    {
        TGrabber Grabber { get; }

        bool Initialize(TDeviceInfo deviceInfo, TGrabber grabber);
    }
}
