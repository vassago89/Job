using Net.Framework.Data.ImageDatas;
using Net.Framework.Data.Recorder;
using Net.Framework.Matrox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Core.Service
{
    public class CaptureService
    {
        private ProcessService _processService;
        private GrabService _grabService;

        private ICapturer<byte> _capturer;
        private byte[] _buffer;
        
        public Action<bool> CapturingStarted;
        private DrawingService _drawingService;
        private SystemConfig _systemConfig;

        private int _current;
        private int _count;
        private string _directory;

        public CaptureService(GrabService grabService, ProcessService processService, DrawingService drawingService, SystemConfig systemConfig)
        {
            _systemConfig = systemConfig;

            _drawingService = drawingService;
            _grabService = grabService;
            _processService = processService;
            _capturer = new MatroxCapturer<byte>();
            _capturer.Intialize(grabService.Width * 2, grabService.Height * 2, 3);

            _buffer = new byte[grabService.Width * grabService.Height * 12];
        }

        public void Start(int count)
        {
            lock (this)
            {
                _current = 0;
                _count = count;

                _directory = Path.Combine(Environment.CurrentDirectory, _systemConfig.CapturePath);
                if (Directory.Exists(_directory) == false)
                    Directory.CreateDirectory(_directory);

                Array.Clear(_buffer, 0, _buffer.Length);
                _processService.Processed += Processed;
                
                CapturingStarted?.Invoke(true);
            }
        }

        private void Processed(int width, int height, byte[][] datas)
        {
            lock (this)
            {
                if (_current >= _count)
                {
                    _processService.Processed -= Processed;
                    
                    //Process.Start(_directory);

                    return;
                }

                _drawingService.Drawing(width, height, datas, _buffer);
                _capturer.Capture(_buffer, Path.Combine(_directory, $"{DateTime.Now.ToString("yyyyMMdd_HHmmssfff")}.bmp"));
                _current++;
            }
        }
    }
}
