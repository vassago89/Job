using System;
using System.Threading.Tasks;

using Net.Framework.Data.ImageDatas;

namespace Net.Framework.Device.ImageDevices
{
    public interface IImageDevice : IDevice<IImageDeviceInfo>
    {
        event Action<byte> Grabbed;

        Task<IImageData> Grab();
        bool ContinuousGrab();
        bool Stop();
    }
}
