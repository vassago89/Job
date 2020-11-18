using System;
using System.IO.Ports;

namespace Net.Framework.Device.SerialDevices
{
    public interface ISerialDeviceInfo : IDeviceInfo
    {
        string PortName { get; }
        int BaudRate { get; }
        Parity Parity { get; }
        int DataBits { get; }
        StopBits StopBits { get; }
    }

    [Serializable]
    public class SerialDeviceInfo : ISerialDeviceInfo
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public Parity Parity { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }

        public SerialDeviceInfo()
        {

        }
    }
}
