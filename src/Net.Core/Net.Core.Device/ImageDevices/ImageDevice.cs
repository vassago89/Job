using System;
using System.Threading.Tasks;

using Net.Core.Data.ImageDatas;

namespace Net.Core.Device.ImageDevices
{
    public interface IImageDevice : IDevice<IImageDeviceInfo>
    {
        event Action<byte> Grabbed;

        Task<IImageData> Grab();
        bool ContinuousGrab();
        bool Stop();
    }
}
