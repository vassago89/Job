using Net.Framework.Device.ImageDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Device.Matrox
{
    public class MatroxSystemGateway
    {
        public List<MatroxImageGrabberInfo> ImageGrabberInfos { get; }
        public List<MatroxImageGrabber> ImageGrabbers { get; }

        public List<MatroxImageDeviceInfo> ImageDeviceInfos { get; }
        public List<MatroxImageDeviceOnGrabber> ImageDevices { get; }

        public MatroxSystemGateway()
        {
            ImageGrabberInfos = new List<MatroxImageGrabberInfo>();
            ImageGrabbers = new List<MatroxImageGrabber>();

            ImageDeviceInfos = new List<MatroxImageDeviceInfo>();
            ImageDevices = new List<MatroxImageDeviceOnGrabber>();
        }

        public void Initialize()
        {
            foreach (var grabberInfo in ImageGrabberInfos)
            {
                var grabber = new MatroxImageGrabber();
                grabber.Initialize(grabberInfo);
                ImageGrabbers.Add(grabber);

                foreach (var deviceInfo in ImageDeviceInfos)
                {
                    var imageDevice = new MatroxImageDeviceOnGrabber();
                    imageDevice.Initialize(deviceInfo, grabber);
                    ImageDevices.Add(imageDevice);
                }
            }
        }
    }
}
