using Net.Framework.Data.ImageDatas;
using Net.Framework.Data.Recorder;
using Net.Framework.Helper.Patterns;
using Net.Framework.Matrox;
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
        private ProcessService _processService;
        private GrabService _grabService;

        private IRecorder<byte> _recorder;
        private List<IRecorder<byte>> _recorders;

        private byte[] _buffer;
        
        public Action<bool> RecordingStarted;
        private DrawingService _drawingService;

        private SystemConfig _systemConfig;

        private double _frameRate;
        
        public RecordService(GrabService grabService, ProcessService processService, DrawingService drawingService, SystemConfig systemConfig)
        {
            _drawingService = drawingService;
            _grabService = grabService;
            _processService = processService;
        
            _recorder = new MatroxRecoreder<byte>();
            _recorder.Intialize(grabService.Width * 2, grabService.Height * 2, 3);

            _recorders = new List<IRecorder<byte>>();
            for (int i =0; i < 4; i++)
            {
                var recorder = new MatroxRecoreder<byte>();

                if (i == 1 || i == 2)
                    recorder.Intialize(grabService.Width, grabService.Height, 1);
                else
                    recorder.Intialize(grabService.Width, grabService.Height, 3);

                _recorders.Add(recorder);
            }

            _systemConfig = systemConfig;

            _buffer = new byte[grabService.Width * grabService.Height * 12];
        }

        public void Start(double frameRate)
        {
            lock (this)
            {
                _frameRate = frameRate;

                var recordDirectory = Path.Combine(Environment.CurrentDirectory, _systemConfig.RecordPath);
                if (Directory.Exists(recordDirectory) == false)
                    Directory.CreateDirectory(recordDirectory);

                var directory = Path.Combine(recordDirectory, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                if (Directory.Exists(directory) == false)
                    Directory.CreateDirectory(directory);

                Array.Clear(_buffer, 0, _buffer.Length);
                _recorder.Start(Path.Combine(directory, "Total.avi"));
                for (int i = 0; i < 4;  i++)
                {
                    switch (i)
                    {
                        case 0:
                            _recorders[i].Start(Path.Combine(directory, "Color.avi"));
                            break;
                        case 1:
                            _recorders[i].Start(Path.Combine(directory, "Ch1.avi"));
                            break;
                        case 2:
                            _recorders[i].Start(Path.Combine(directory, "Ch2.avi"));
                            break;
                        case 3:
                            _recorders[i].Start(Path.Combine(directory, "Merged.avi"));
                            break;
                    }   
                }
                    

                _processService.Processed += Processed;

                RecordingStarted?.Invoke(true);
            }
        }

        private void Processed(int width, int height, byte[][] datas)
        {
            lock (this)
            {
                _drawingService.Drawing(width, height, datas, _buffer);
                _recorder.Enqueue(_buffer);

                for (int i = 0; i < 4; i++)
                    _recorders[i].Enqueue(datas[i]);
            }
        }

        public void Stop()
        {
            lock (this)
            {
                _processService.Processed -= Processed;
                
                _recorder.Stop(_frameRate);

                foreach (var recorder in _recorders)
                    recorder.Stop(_frameRate);

                RecordingStarted?.Invoke(false);
            }
        }
    }
}
