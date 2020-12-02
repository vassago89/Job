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

        public RecordService(GrabService grabService, ProcessService processService)
        {
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
            }
        }

        private void Processed(int width, int height, byte[][] datas)
        {
            lock (this)
            {
                if (_onRecord == false)
                    return;
                
                for (int i = 0, j = 0; i < height; i++, j += 2)
                {
                    Buffer.BlockCopy(datas[0], i * width, _buffer, j * width, width);
                    Buffer.BlockCopy(datas[3], i * width, _buffer, j * width + width, width);
                }

                var doubleHeight = height * 2;
                
                for (int i = height, j = height * 4; i < height * 2; i++, j += 2)
                {
                    Buffer.BlockCopy(datas[0], i * width, _buffer, j * width, width);
                    Buffer.BlockCopy(datas[3], i * width, _buffer, j * width + width, width);
                }

                for (int i = height * 2, j = height * 8; i < height * 3; i++, j += 2)
                {
                    Buffer.BlockCopy(datas[0], i * width, _buffer, j * width, width);
                    Buffer.BlockCopy(datas[3], i * width, _buffer, j * width + width, width);
                }

                for (int i = 0, j = height * 2; i < height; i++, j += 2)
                    Buffer.BlockCopy(datas[1], i * width, _buffer, j * width, width);

                for (int i = height, j = height * 6; i < height * 2; i++, j += 2)
                    Buffer.BlockCopy(datas[2], i * width, _buffer, j * width + width, width);

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
