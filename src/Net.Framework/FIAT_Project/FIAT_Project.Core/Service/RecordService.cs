using Net.Framework.Data.ImageDatas;
using Net.Framework.Data.Recorder;
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
        private bool _onRecord = false;

        private byte[] _buffer;

        private string _directoryName = "Temp";
        private string _fileName = "Temp.avi";

        public Action<bool> RecordingStarted;
        private DrawingService _drawingService;

        public RecordService(GrabService grabService, ProcessService processService, DrawingService drawingService)
        {
            _drawingService = drawingService;
            _grabService = grabService;
            _processService = processService;
            _recorder = new MatroxRecoreder<byte>();
            _recorder.Intialize(grabService.Width * 2, grabService.Height * 2, 3);

            _buffer = new byte[grabService.Width * grabService.Height * 12];
        }

        public void Start()
        {
            lock (this)
            {
                var directory = Path.Combine(Environment.CurrentDirectory, _directoryName);
                if (Directory.Exists(directory) == false)
                    Directory.CreateDirectory(directory);

                Array.Clear(_buffer, 0, _buffer.Length);
                _recorder.Start(Path.Combine(directory, _fileName));
                _processService.Processed += Processed;
                _onRecord = true;

                RecordingStarted?.Invoke(true);
            }
        }

        private void Processed(int width, int height, byte[][] datas)
        {
            lock (this)
            {
                _drawingService.Drawing(width, height, datas, _buffer);
                _recorder.Enqueue(_buffer);
            }
        }

        public void Stop()
        {
            lock (this)
            {
                _onRecord = false;

                _processService.Processed -= Processed;

                var frameRate = _grabService.FrameRate;
                _recorder.Stop(frameRate);

                RecordingStarted?.Invoke(false);
            }
        }

        public void CopyTo(string path)
        {
            try
            {
                File.Copy(Path.Combine(_directoryName, _fileName), path, true);
            }
            catch(Exception e)
            {

            }
        }
    }
}
