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

            var buffer = Encoding.UTF8.GetBytes($"trgmode 1\r");
            _grabberDevice?.Write(buffer, buffer.Length);

            Thread.Sleep(10);
        }

        public void Release()
        {
            _lazerDevice.Close();
            _grabberDevice.Close();
        }

        private byte[] GetBuffer()
        {
            var buffer = new byte[8];
            buffer[0] = 0x55;
            buffer[1] = 0xAA;
            buffer[2] = 0x05;

            return buffer;
        }


        private void Send(byte[] buffer)
        {
            buffer[6] = (byte)((buffer[2] + buffer[3] + buffer[4] + buffer[5]) & 0xFF);
            _lazerDevice?.Write(buffer, 8);
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var port = sender as SerialPort;
            var message = port.ReadExisting();
            //throw new NotImplementedException();
        }

        public void SetLed(double value)
        {
            var buffer = GetBuffer();

            switch (_systemConfig.ProtocolType)
            {
                case EProtocolType.Channel2:
                    buffer[3] = 0x01;
                    break;
                case EProtocolType.Channel3:
                    buffer[3] = 0x02;
                    break;
            }

            buffer[4] = (byte)(value / 256);
            buffer[5] = (byte)(value % 256);

            Send(buffer);
        }

        public void OnLed()
        {
            var buffer = GetBuffer();
            
            switch (_systemConfig.ProtocolType)
            {
                case EProtocolType.Channel2:
                    buffer[3] = 0x05;
                    break;
                case EProtocolType.Channel3:
                    buffer[3] = 0x06;
                    break;
            }

            buffer[5] = 0x01;

            Send(buffer);
        }

        public void OffLed()
        {
            var buffer = GetBuffer();
            
            switch (_systemConfig.ProtocolType)
            {
                case EProtocolType.Channel2:
                    buffer[3] = 0x05;
                    break;
                case EProtocolType.Channel3:
                    buffer[3] = 0x06;
                    break;
            }

            Send(buffer);
        }

        public void Set660(double value)
        {
            if (_systemConfig.UseDictionary[ELazer.L660] == false)
                return;

            var buffer = GetBuffer();

            switch (_systemConfig.ProtocolType)
            {
                case EProtocolType.Channel2:
                    return;
                case EProtocolType.Channel3:
                    buffer[3] = 0x00;
                    break;
            }

            buffer[4] = (byte)(value / 256);
            buffer[5] = (byte)(value % 256);

            Send(buffer);
        }

        public void On660()
        {
            if (_systemConfig.UseDictionary[ELazer.L660] == false)
                return;

            var buffer = GetBuffer();
            
            switch (_systemConfig.ProtocolType)
            {
                case EProtocolType.Channel2:
                    return;
                case EProtocolType.Channel3:
                    buffer[3] = 0x04;
                    break;
            }

            buffer[5] = 0x01;

            Send(buffer);
        }

        public void Off660()
        {
            if (_systemConfig.UseDictionary[ELazer.L660] == false)
                return;

            var buffer = GetBuffer();

            switch (_systemConfig.ProtocolType)
            {
                case EProtocolType.Channel2:
                    return;
                case EProtocolType.Channel3:
                    buffer[3] = 0x04;
                    break;
            }

            Send(buffer);
        }

        public void Set760(double value)
        {
            if (_systemConfig.UseDictionary[ELazer.L760] == false)
                return;

            var buffer = GetBuffer();

            switch (_systemConfig.ProtocolType)
            {
                case EProtocolType.Channel2:
                    buffer[3] = 0x00;
                    break;
                case EProtocolType.Channel3:
                    buffer[3] = 0x01;
                    break;
            }
            
            buffer[4] = (byte)(value / 256);
            buffer[5] = (byte)(value % 256);

            Send(buffer);
        }

        public void On760()
        {
            if (_systemConfig.UseDictionary[ELazer.L760] == false)
                return;
            
            var buffer = GetBuffer();

            switch (_systemConfig.ProtocolType)
            {
                case EProtocolType.Channel2:
                    buffer[3] = 0x04;
                    break;
                case EProtocolType.Channel3:
                    buffer[3] = 0x05;
                    break;
            }

            buffer[5] = 0x01;

            Send(buffer);
        }

        public void Off760()
        {
            if (_systemConfig.UseDictionary[ELazer.L760] == false)
                return;

            var buffer = GetBuffer();

            switch (_systemConfig.ProtocolType)
            {
                case EProtocolType.Channel2:
                    buffer[3] = 0x04;
                    break;
                case EProtocolType.Channel3:
                    buffer[3] = 0x05;
                    break;
            }

            Send(buffer);
        }
        
        public void SetExposure(int value, ELazer lazer, bool isLed = false)
        {
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
