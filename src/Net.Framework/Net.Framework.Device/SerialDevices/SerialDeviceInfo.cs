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
        public string PortName { get; }
        public int BaudRate { get; }
        public Parity Parity { get; }
        public int DataBits { get; }
        public StopBits StopBits { get; }

        public SerialDeviceInfo(
            string portName,
            int baudRate,
            Parity parity,
            int dataBits,
            StopBits stopBits)
        {
            PortName = portName;
            BaudRate = baudRate;
            Parity = parity;
            DataBits = dataBits;
            StopBits = StopBits;
        }
    }
}
