using Net.Framework.Data.ImageDatas;
using Net.Framework.Data.Recorder;
using Net.Framework.Matrox.Recorder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Core.Service
{
    public class RecordService
    {
        private GrabService _grabService;
        
        private IRecorder<byte>[] recorders;

        public RecordService(GrabService grabService)
        {
            _grabService = grabService;
            recorders = new IRecorder<byte>[3];
            for (int i = 0; i < 3; i++)
            {
                var recorder = new MatroxRecoreder<byte>();
                recorder.Intialize(grabService.Width, grabService.Height, 1);

                recorders[i] = recorder;
            }
        }

        public void Start(string[] paths)
        {
            for (int i = 0; i < 3; i++)
                recorders[i].Start(paths[i]);

            _grabService.ServiceGrabbed += ServiceGrabbed;
        }

        private void ServiceGrabbed(int width, int height, byte[][] datas)
        {
            for (int i = 0; i < 3; i++)
                recorders[i].Enqueue(datas[i]);
        }

        public void Stop()
        {
            var frameRate = _grabService.FrameRate;
            foreach (var recorder in recorders)
                recorder.Stop(frameRate);
        }
    }
}
