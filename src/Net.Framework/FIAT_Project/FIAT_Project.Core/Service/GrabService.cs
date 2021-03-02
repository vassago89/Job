using FIAT_Project.Core.Enums;
using Net.Framework.Data.ImageDatas;
using Net.Framework.Device.ImageDevices;
using Net.Framework.Device.Matrox;
using Net.Framework.Helper;
using Net.Framework.Helper.Patterns;
using Net.Framework.Matrox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FIAT_Project.Core.Service
{
    public class GrabService
    {
        public event Action<int, int, byte[], Dictionary<ELazer, byte[]>> Grabbed;
        private MatroxSystemGateway _gateway;

        //private Queue<ImageData<byte>> _queue;
        //private PipeLine<ImageData<byte>> _pipeLine;

        private IImageDevice _imageDevice;

        public double FrameRate => _imageDevice.FrameRate;
        public int Width => _imageDevice.Width;
        public int Height => _imageDevice.Height;
        public int Channels => _imageDevice.Channels;

        public byte[] _ledData;
        public Dictionary<ELazer, byte[]> _dataDictionary { get; set; }

        public Action<bool> GrabbingStarted;
        
        private SystemConfig _systemConfig;
        public GrabService(SystemConfig systemConfig)
        {
            try
            {
                _dataDictionary = new Dictionary<ELazer, byte[]>();

                _systemConfig = systemConfig;

                MatroxApplicationHelper.Initilize();
                _gateway = new MatroxSystemGateway();
                _gateway.ImageGrabberInfos.Add(new MatroxImageGrabberInfo()
                {
                    SystemNo = 0,
                    Type = MatroxImageGrabberType.DEFAULT
                });

                _gateway.ImageDeviceInfos.Add(new MatroxImageDeviceInfo()
                {
                    BufferSize = 5,
                    DcfPath = systemConfig.DcfPath,
                    DigitizerNo = 0
                });

                _gateway.Initialize();

                _imageDevice = _gateway.ImageDevices.First();
                _imageDevice.DeviceGrabbed += DeviceImageGrabbed;


                _ledData = new byte[_imageDevice.Width * _imageDevice.Height];
                foreach (var pair in _systemConfig.UseDictionary)
                {
                    if (pair.Value)
                        _dataDictionary[pair.Key] = new byte[_imageDevice.Width * _imageDevice.Height];
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Release()
        {
            foreach (var device in _gateway.ImageDevices)
                device.Dispose();

            foreach (var grabber in _gateway.ImageGrabbers)
                grabber.Dispose();
        }

        private void DeviceImageGrabbed(IImageData obj)
        {
            var imageData = obj as ImageData<byte>;

            int size = imageData.Width * imageData.Height;

            int index = 2;
            Buffer.BlockCopy(imageData.Data, index * size, _ledData, 0, size);
            index--;

            if (_dataDictionary.ContainsKey(ELazer.L660))
                Buffer.BlockCopy(imageData.Data, index * size, _dataDictionary[ELazer.L660], 0, size);

            index--;

            if (_dataDictionary.ContainsKey(ELazer.L760))
                Buffer.BlockCopy(imageData.Data, index * size, _dataDictionary[ELazer.L760], 0, size);

            index--;

            Grabbed?.Invoke(imageData.Width, imageData.Height, _ledData, _dataDictionary);
        }
        
        public void Start()
        {
            foreach (var imageDevice in _gateway.ImageDevices)
                imageDevice.ContinuousGrab();

            GrabbingStarted?.Invoke(true);
        }

        public void Stop()
        {
            foreach (var imageDevice in _gateway.ImageDevices)
                imageDevice.Stop();

            GrabbingStarted?.Invoke(false);
        }
    }
}
