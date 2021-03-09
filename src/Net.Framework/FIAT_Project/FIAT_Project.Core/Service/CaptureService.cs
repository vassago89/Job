using FIAT_Project.Core.Enums;
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

        private ICapturer<byte> _ledCapturer;
        private ICapturer<byte> _mergedCapturer;

        private Dictionary<ELazer, ICapturer<byte>> _capturerDictionary;

        public Action<bool> CapturingStarted;
        private SystemConfig _systemConfig;

        private int _current;
        private int _count;
        private string _directory;

        public CaptureService(GrabService grabService, ProcessService processService, DrawingService drawingService, SystemConfig systemConfig)
        {
            try
            {
                var captureDirectory = Path.Combine(Environment.CurrentDirectory, systemConfig.CapturePath);
                if (Directory.Exists(captureDirectory) == false)
                    Directory.CreateDirectory(captureDirectory);

                _systemConfig = systemConfig;

                _grabService = grabService;
                _processService = processService;

                _ledCapturer = new MatroxCapturer<byte>();
                _ledCapturer.Intialize(grabService.Width, grabService.Height, 3);

                _mergedCapturer = new MatroxCapturer<byte>();
                _mergedCapturer.Intialize(grabService.Width, grabService.Height, 3);

                _capturerDictionary = new Dictionary<ELazer, ICapturer<byte>>();

                foreach (var pair in _systemConfig.UseDictionary)
                {
                    if (pair.Value)
                    {
                        _capturerDictionary[pair.Key] = new MatroxCapturer<byte>();
                        _capturerDictionary[pair.Key].Intialize(grabService.Width, grabService.Height, 1);
                    }
                }
            }
            catch (Exception e)
            {
                //throw e;
            }
        }

        public void Start(int count)
        {
            lock (this)
            {
                _current = 1;
                _count = count;
                
                var captureDirectory = Path.Combine(Environment.CurrentDirectory, _systemConfig.CapturePath);
                if (Directory.Exists(captureDirectory) == false)
                    Directory.CreateDirectory(captureDirectory);

                _directory = Path.Combine(captureDirectory, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                if (Directory.Exists(_directory) == false)
                    Directory.CreateDirectory(_directory);

                _processService.Processed += Processed;
                
                CapturingStarted?.Invoke(true);
            }
        }

        private void Processed(int width, int height, byte[] ledData, byte[] mergedData, Dictionary<ELazer, byte[]> dataDictionary)
        {
            lock (this)
            {
                _ledCapturer.Capture(ledData, Path.Combine(_directory, $"Color_{_current}.bmp"));
                _mergedCapturer.Capture(mergedData, Path.Combine(_directory, $"Merged_{_current}.bmp"));

                foreach (var pair in dataDictionary)
                {
                    switch (pair.Key)
                    {
                        case ELazer.L660:
                            _capturerDictionary[pair.Key].Capture(pair.Value, Path.Combine(_directory, $"Ch1_{_current}.bmp"));
                            break;
                        case ELazer.L760:
                            _capturerDictionary[pair.Key].Capture(pair.Value, Path.Combine(_directory, $"Ch2_{_current}.bmp"));
                            break;
                    }
                }

                _current++;

                if (_current > _count)
                {
                    _processService.Processed -= Processed;
                    return;
                }
            }
        }
    }
}
