using Net.Framework.Device.SerialDevices;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Core.Service
{
    public class ProtocolService
    {
        public SerialDevice _device;

        public ProtocolService()
        {
            var serialDeviceInfo = new SerialDeviceInfo()
            {
                BaudRate = 9600,
                DataBits = 8,
                PortName = "COM6",
                StopBits = StopBits.One,
                Parity = Parity.None
            };

            _device = new SerialDevice();
            _device.Initialize(serialDeviceInfo);
            _device.Open();
        }

        public void LightOn(int value)
        {
            var buffer = new byte[7];
            
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x02;
            buffer[4] = (byte)(value / 256);
            buffer[5] = (byte)(value % 256);
            buffer[6] = (byte)((buffer[2] + buffer[3] + buffer[4] + buffer[5]) & 0xFF);

            
        }

        public void LightOff()
        {
            var buffer = new byte[7];
            
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x06;
            buffer[4] = 0x00;
            buffer[5] = 0x00;
            buffer[6] = 0x0B;

            _device.Write(buffer, 7);
        }
    }
}
