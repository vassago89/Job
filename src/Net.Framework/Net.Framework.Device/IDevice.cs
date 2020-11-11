using System;
using System.Threading.Tasks;

namespace Net.Framework.Device
{
    public interface IDevice
    {

    }

    public interface IDevice<T> : IDevice where T : IDeviceInfo
    {
        bool Initialize(T deviceInfo);
    }
}
