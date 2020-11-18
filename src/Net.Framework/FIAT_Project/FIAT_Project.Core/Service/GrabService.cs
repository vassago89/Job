using Net.Framework.Data.ImageDatas;
using Net.Framework.Device.Matrox;
using Net.Framework.Helper;
using Net.Framework.Matrox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Core.Service
{
    public class GrabService
    {
        public event Action<int, int, byte[][]> Grabbed;
        private MatroxSystemGateway _gateway;

        private Queue<ImageData<byte>> _queue;
        private PipeLine<ImageData<byte>> _pipeLine;

        public GrabService()
        {
            MatroxApplicationHelper.Initilize();
            _queue = new Queue<ImageData<byte>>();
            _pipeLine = new PipeLine<ImageData<byte>>(true);

            _pipeLine.Job = new Action<ImageData<byte>>((imageData) =>
            {
                var datas = new byte[imageData.Channels][];

                for (int i = 0; i < imageData.Channels; i++)
                {
                    datas[i] = new byte[imageData.Width * imageData.Height];
                    Buffer.BlockCopy(imageData.Data, i * imageData.Width * imageData.Height, datas[i], 0, imageData.Width * imageData.Height);
                }

                Grabbed?.Invoke(imageData.Width, imageData.Height, datas);
            });

            _pipeLine.Run( new System.Threading.CancellationToken());

            _gateway = new MatroxSystemGateway();
            _gateway.ImageGrabberInfos.Add(new MatroxImageGrabberInfo()
            {
                SystemNo = 0,
                Type = MatroxImageGrabberType.SOLIOS
            });

            _gateway.ImageDeviceInfos.Add(new MatroxImageDeviceInfo()
            {
                BufferSize = 5,
                DcfPath = "MIL10_SOL_BV-C8300NV_re2.dcf",
                DigitizerNo = 0
            });

            _gateway.Initialize();

            foreach (var imageDevice in _gateway.ImageDevices)
                imageDevice.Grabbed += ImageGrabbed;

            
        }

        private void ImageGrabbed(IImageData obj)
        {
            var imageData = obj as ImageData<byte>;
            _pipeLine.Enqueue(imageData);
            //var datas = new byte[imageData.Channels][];
            
            //for (int i = 0; i < imageData.Channels; i++)
            //{
            //    datas[i] = new byte[imageData.Width * imageData.Height];
            //    Buffer.BlockCopy(imageData.Data, i * imageData.Width * imageData.Height, datas[i], 0, imageData.Width * imageData.Height);
            //}

            //Grabbed?.Invoke(imageData.Width, imageData.Height, datas);
        }



        public void Start()
        {
            foreach (var imageDevice in _gateway.ImageDevices)
                imageDevice.ContinuousGrab();
        }

        public void Stop()
        {
            foreach (var imageDevice in _gateway.ImageDevices)
                imageDevice.Stop();
        }
    }
}
