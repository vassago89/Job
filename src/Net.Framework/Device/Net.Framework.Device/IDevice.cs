using System;
using System.Threading.Tasks;

namespace Net.Framework.Device
{
    public interface IDevice : IDisposable
    {
        
    }

    public interface IDevice<T> : IDevice where T : IDeviceInfo
    {
        T Info { get; }

        bool Initialize(T deviceInfo);
    }
}
