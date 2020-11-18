using System;
using System.Threading.Tasks;

using Net.Framework.Data.ImageDatas;

namespace Net.Framework.Device.ImageDevices
{
    public interface IImageDevice
    {
        event Action<IImageData> Grabbed;

        Task<IImageData> Grab();
        bool ContinuousGrab();
        bool Stop();
    }

    public interface IImageDeviceNonGrabber<TDeviceInfo> : IImageDevice, IDevice
    {
        TDeviceInfo Info { get; }
    }

    public interface IImageDeviceOnGrabber<TDeviceInfo, TGrabber> : IImageDeviceNonGrabber<TDeviceInfo>
    {
        TGrabber Grabber { get; }

        bool Initialize(TDeviceInfo deviceInfo, TGrabber grabber);
    }
}
