using System;
using System.Threading.Tasks;

namespace Net.Core.Device
{
    public interface IDevice
    {

    }

    public interface IDevice<T> : IDevice where T : IDeviceInfo
    {
        bool Initialize(T deviceInfo);
    }
}
