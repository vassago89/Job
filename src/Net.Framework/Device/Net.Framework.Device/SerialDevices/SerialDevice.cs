using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace Net.Framework.Device.SerialDevices
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
        
        private ISerialDeviceInfo _info;
        public ISerialDeviceInfo Info => _info;

        public bool Initialize(ISerialDeviceInfo info)
        {
            try
            {
                _info = info;
                _serialPort = new SerialPort(
                    _info.PortName,
                    _info.BaudRate,
                    _info.Parity,
                    _info.DataBits,
                    _info.StopBits);

                _serialPort.DtrEnable = true;
                _serialPort.RtsEnable = true;

                return true;
            }
            catch (Exception e)
            {
                throw e;
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

        public bool Write(byte[] buffer, int count)
        {
            try
            {
                _serialPort.Write(buffer, 0, count);
                return true;
            }
            catch (Exception e)
            {

            }

            return false;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
