using FIAT_Project.Core.Enums;
using Net.Framework.Device.SerialDevices;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FIAT_Project.Core.Service
{
    public class ProtocolService
    {
        private SerialDevice _lazerDevice;
        private SerialDevice _grabberDevice;

        private SystemConfig _systemConfig;

        public ProtocolService(SystemConfig systemConfig)
        {
            _systemConfig = systemConfig;

            var lazerDeviceInfo = new SerialDeviceInfo()
            {
                BaudRate = 9600,
                DataBits = 8,
                PortName = systemConfig.LazerProtocolPort,
                StopBits = StopBits.One,
                Parity = Parity.None
            };

            _lazerDevice = new SerialDevice();
            _lazerDevice.Initialize(lazerDeviceInfo);
            _lazerDevice.SerialPort.DataReceived += SerialPort_DataReceived;
            _lazerDevice.Open();

            var grabberDeviceInfo = new SerialDeviceInfo()
            {
                BaudRate = 115200,
                DataBits = 8,
                PortName = systemConfig.GrabberProtocolPort,
                StopBits = StopBits.One,
                Parity = Parity.None
            };

            _grabberDevice = new SerialDevice();
            _grabberDevice.Initialize(grabberDeviceInfo);
            _grabberDevice.SerialPort.DataReceived += SerialPort_DataReceived;
            _grabberDevice.Open();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var port = sender as SerialPort;
            var message = port.ReadExisting();
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

            _lazerDevice?.Write(buffer, 8);
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
            
            _lazerDevice?.Write(buffer, 8);
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

            _lazerDevice?.Write(buffer, 8);
        }

        public void Set660(double value)
        {
            if (_systemConfig.UseDictionary[ELazer.L660] == false)
                return;

            var buffer = new byte[8];
            
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x00;
            buffer[4] = (byte)(value / 256);
            buffer[5] = (byte)(value % 256);
            buffer[6] = (byte)((buffer[2] + buffer[3] + buffer[4] + buffer[5]) & 0xFF);

            _lazerDevice?.Write(buffer, 8);
        }

        public void On660()
        {
            if (_systemConfig.UseDictionary[ELazer.L660] == false)
                return;

            var buffer = new byte[8];
            
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x04;
            buffer[4] = 0x00;
            buffer[5] = 0x01;
            buffer[6] = 0x0A;

            _lazerDevice?.Write(buffer, 8);
        }

        public void Off660()
        {
            if (_systemConfig.UseDictionary[ELazer.L660] == false)
                return;

            var buffer = new byte[8];
            
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x04;
            buffer[4] = 0x00;
            buffer[5] = 0x00;
            buffer[6] = 0x09;

            _lazerDevice?.Write(buffer, 8);
        }

        public void Set760(double value)
        {
            if (_systemConfig.UseDictionary[ELazer.L760] == false)
                return;

            var buffer = new byte[8];

            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x01;
            buffer[4] = (byte)(value / 256);
            buffer[5] = (byte)(value % 256);
            buffer[6] = (byte)((buffer[2] + buffer[3] + buffer[4] + buffer[5]) & 0xFF);

            _lazerDevice?.Write(buffer, 8);
        }

        public void On760()
        {
            if (_systemConfig.UseDictionary[ELazer.L760] == false)
                return;

            var buffer = new byte[8];
            
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x05;
            buffer[4] = 0x00;
            buffer[5] = 0x01;
            buffer[6] = 0x0B;

            _lazerDevice?.Write(buffer, 8);
        }

        public void Off760()
        {
            if (_systemConfig.UseDictionary[ELazer.L760] == false)
                return;

            var buffer = new byte[8];
            
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;
            buffer[3] = 0x05;
            buffer[4] = 0x00;
            buffer[5] = 0x00;
            buffer[6] = 0x0A;

            _lazerDevice?.Write(buffer, 8);
        }

        public void SetExposure(int value, ELazer lazer, bool isLed = false)
        {
            //SetFrameRate(1000.0 / value);

            //Thread.Sleep(10);

            var chennel = isLed ? 'b' : lazer == ELazer.L660 ? 'g' : 'r';

            var buffer = Encoding.UTF8.GetBytes($"exp {chennel} {value * 1000}\r");
            _grabberDevice?.Write(buffer, buffer.Length);

            Thread.Sleep(10);
        }

        public void SetGain(int value, ELazer lazer, bool isLed = false)
        {
            var chennel = isLed ? 'b' : lazer == ELazer.L660 ? 'g' : 'r';

            var buffer = Encoding.UTF8.GetBytes($"gain {chennel} {(int)(value * 3.36)}\r");
            _grabberDevice?.Write(buffer, buffer.Length);
        }

        public void SetFrameRate(double frameRate)
        {
            var buffer = Encoding.UTF8.GetBytes($"frame {1000000 / frameRate}\r");
            _grabberDevice?.Write(buffer, buffer.Length);

            Thread.Sleep(10);
        }
    }
}
