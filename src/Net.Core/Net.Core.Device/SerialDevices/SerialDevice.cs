using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace Net.Core.Device.SerialDevices
{
    public interface ISerialDevice : IDevice<ISerialDeviceInfo>
    {
        SerialPort SerialPort { get; }

        bool Open();
        bool Close();
    }

    public class SerialDevice : ISerialDevice
    {
        SerialPort _serialPort;

        public SerialPort SerialPort => _serialPort;


        private Exception lastException;
        public Exception LastException => lastException;

        public bool Initialize(ISerialDeviceInfo serialDeviceInfo)
        {
            try
            {
                _serialPort = new SerialPort(
                    serialDeviceInfo.PortName,
                    serialDeviceInfo.BaudRate,
                    serialDeviceInfo.Parity,
                    serialDeviceInfo.DataBits,
                    serialDeviceInfo.StopBits);

                return true;
            }
            catch (Exception e)
            {

            }

            return false;
        }

        public bool Open()
        {
            try
            {
                _serialPort.Open();
                return true;
            }
            catch (Exception e)
            {

            }

            return false;
        }

        public bool Close()
        {
            try
            {
                _serialPort.Close();
                return true;
            }
            catch (Exception e)
            {

            }

            return false;
        }
    }
}
