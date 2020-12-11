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

        public ProtocolService(SystemConfig systemConfig)
        {
            var serialDeviceInfo = new SerialDeviceInfo()
            {
                BaudRate = 9600,
                DataBits = 8,
                PortName = systemConfig.ProtocolPort,
                StopBits = StopBits.One,
                Parity = Parity.None
            };

            _device = new SerialDevice();
            _device.Initialize(serialDeviceInfo);
            _device.SerialPort.DataReceived += SerialPort_DataReceived;
            _device.Open();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void SetLed(double value)
        {
            var buffer = new byte[8];

            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x02;
            buffer[4] = (byte)(value / 256);
            buffer[5] = (byte)(value % 256);
            buffer[6] = (byte)((buffer[2] + buffer[3] + buffer[4] + buffer[5]) & 0xFF);

            _device?.Write(buffer, 8);
        }

        public void OnLed()
        {
            var buffer = new byte[8];
            
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x06;
            buffer[4] = 0x00;
            buffer[5] = 0x01;
            buffer[6] = 0x0C;
            
            _device?.Write(buffer, 8);
        }

        public void OffLed()
        {
            var buffer = new byte[8];
            
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x06;
            buffer[4] = 0x00;
            buffer[5] = 0x00;
            buffer[6] = 0x0B;

            _device?.Write(buffer, 8);
        }

        public void Set660(double value)
        {
            var buffer = new byte[8];
            
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x00;
            buffer[4] = (byte)(value / 256);
            buffer[5] = (byte)(value % 256);
            buffer[6] = (byte)((buffer[2] + buffer[3] + buffer[4] + buffer[5]) & 0xFF);

            _device?.Write(buffer, 8);
        }

        public void On660()
        {
            var buffer = new byte[8];
            
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x04;
            buffer[4] = 0x00;
            buffer[5] = 0x01;
            buffer[6] = 0x0A;

            _device?.Write(buffer, 8);
        }

        public void Off660()
        {
            var buffer = new byte[8];
            
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x04;
            buffer[4] = 0x00;
            buffer[5] = 0x00;
            buffer[6] = 0x09;

            _device?.Write(buffer, 8);
        }

        public void Set760(double value)
        {
            var buffer = new byte[8];

            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x01;
            buffer[4] = (byte)(value / 256);
            buffer[5] = (byte)(value % 256);
            buffer[6] = (byte)((buffer[2] + buffer[3] + buffer[4] + buffer[5]) & 0xFF);

            _device?.Write(buffer, 8);
        }

        public void On760()
        {
            var buffer = new byte[8];
            
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x05;
            buffer[4] = 0x00;
            buffer[5] = 0x01;
            buffer[6] = 0x0B;

            _device?.Write(buffer, 8);
        }

        public void Off760()
        {
            var buffer = new byte[8];
            
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x05;
            buffer[4] = 0x00;
            buffer[5] = 0x00;
            buffer[6] = 0x0A;

            _device?.Write(buffer, 8);
        }
    }
}
