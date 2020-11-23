using System;
using System.Threading.Tasks;

using Net.Framework.Data.ImageDatas;

namespace Net.Framework.Device.ImageDevices
{
    public interface IImageDevice
    {
        int Width { get; }
        int Height { get; }
        int Channels { get; }
        double FrameRate { get; }

        event Action<IImageData> Grabbed;

        Task<IImageData> Grab();
        bool ContinuousGrab();
        bool Stop();
    }

    public interface IImageDeviceNonGrabber<TDeviceInfo> : IImageDevice, IDevice where TDeviceInfo : ImageDeviceInfo
    {
        TDeviceInfo Info { get; }
    }

    public interface IImageDeviceOnGrabber<TDeviceInfo, TGrabber> : IImageDeviceNonGrabber<TDeviceInfo> where TDeviceInfo : ImageDeviceInfo
    {
        TGrabber Grabber { get; }

        bool Initialize(TDeviceInfo deviceInfo, TGrabber grabber);
    }
}
