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
        public event Action<int, int, byte[][]> Grabbed;
        private MatroxSystemGateway _gateway;

        //private Queue<ImageData<byte>> _queue;
        //private PipeLine<ImageData<byte>> _pipeLine;

        private IImageDevice _imageDevice;

        public double FrameRate => _imageDevice.FrameRate;
        public int Width => _imageDevice.Width;
        public int Height => _imageDevice.Height;
        public int Channels => _imageDevice.Channels;

        public Action<bool> GrabbingStarted;

        private byte[][] _buffers;
        private SystemConfig _systemConfig;
        public GrabService(SystemConfig systemConfig)
        {
            _systemConfig = systemConfig;

            MatroxApplicationHelper.Initilize();
            _gateway = new MatroxSystemGateway();
            _gateway.ImageGrabberInfos.Add(new MatroxImageGrabberInfo()
            {
                SystemNo = 0,
                Type = MatroxImageGrabberType.SOLIOS
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

            _buffers = new byte[_imageDevice.Channels][];
            for (int i = 0; i < _imageDevice.Channels; i++)
            {
                _buffers[i] = new byte[_imageDevice.Width * _imageDevice.Height];
            }
        }

        private void DeviceImageGrabbed(IImageData obj)
        {
            var imageData = obj as ImageData<byte>;

            int size = imageData.Width * imageData.Height;
            for (int i = 0; i < _imageDevice.Channels; i++)
                Buffer.BlockCopy(imageData.Data, i * size, _buffers[i], 0, size);

            Grabbed?.Invoke(imageData.Width, imageData.Height, _buffers.Reverse().ToArray());
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
