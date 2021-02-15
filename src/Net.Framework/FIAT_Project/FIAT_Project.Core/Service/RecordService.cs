using FIAT_Project.Core.Enums;
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
using System.Threading;

namespace FIAT_Project.Core.Service
{
    public class RecordService
    {
        private ProcessService _processService;
        private GrabService _grabService;

        private IRecorder<byte> _ledRecorder;
        private IRecorder<byte> _mergedRecorder;
        private Dictionary<ELazer, IRecorder<byte>> _recoderDictionary;

        private byte[] _ledBuffer;
        private byte[] _mergedBuffer;
        private Dictionary<ELazer, byte[]> _bufferDictionary;

        public Action<bool> RecordingStarted;

        private SystemConfig _systemConfig;

        private double _frameRate;

        private Timer _timer;
        
        public RecordService(GrabService grabService, ProcessService processService, SystemConfig systemConfig)
        {
            _grabService = grabService;
            _processService = processService;
            
            _systemConfig = systemConfig;

            _ledRecorder = new MatroxRecoreder<byte>();
            _ledRecorder.Intialize(grabService.Width, grabService.Height, 3);

            _mergedRecorder = new MatroxRecoreder<byte>();
            _mergedRecorder.Intialize(grabService.Width, grabService.Height, 3);

            _recoderDictionary = new Dictionary<ELazer, IRecorder<byte>>();
            foreach (var pair in _systemConfig.UseDictionary)
            {
                if (pair.Value)
                {
                    _recoderDictionary[pair.Key] = new MatroxRecoreder<byte>();
                    _recoderDictionary[pair.Key].Intialize(grabService.Width, grabService.Height, 1);
                }
            }

            _bufferDictionary = new Dictionary<ELazer, byte[]>();
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

                _ledRecorder.Start(Path.Combine(directory, "Color.avi"));
                _mergedRecorder.Start(Path.Combine(directory, "Merged.avi"));

                foreach (var pair in _recoderDictionary)
                {
                    switch (pair.Key)
                    {
                        case ELazer.L660:
                            pair.Value.Start(Path.Combine(directory, "Ch1.avi"));
                            break;
                        case ELazer.L760:
                            pair.Value.Start(Path.Combine(directory, "Ch2.avi"));
                            break;
                    }
                }

                _timer = new Timer(new TimerCallback((obj) =>
                {
                    lock (this)
                    {
                        if (_ledBuffer == null || _mergedBuffer == null)
                            return;

                        _ledRecorder.Enqueue(_ledBuffer);
                        _mergedRecorder.Enqueue(_mergedBuffer);
                        foreach (var pair in _recoderDictionary)
                            pair.Value.Enqueue(_bufferDictionary[pair.Key]);

                        _ledBuffer = null;
                        _mergedBuffer = null;

                        _bufferDictionary.Clear();
                    }
                }), null, (int)Math.Round(1000.0 / _frameRate), (int)Math.Round(1000.0 / _frameRate));

                _processService.Processed += Processed;
                RecordingStarted?.Invoke(true);
            }
        }

        private void Processed(int width, int height, byte[] ledData, byte[] mergedData, Dictionary<ELazer, byte[]> dataDictionary)
        {
            lock (this)
            {
                _ledBuffer = ledData;
                _mergedBuffer = mergedData;
                
                foreach (var pair in dataDictionary)
                    _bufferDictionary[pair.Key] = pair.Value;

                //_ledRecorder.Enqueue(ledData);
                //_mergedRecorder.Enqueue(mergedData);
                //foreach (var pair in _recoderDictionary)
                //    pair.Value.Enqueue(dataDictionary[pair.Key]);
            }
        }

        public void Stop()
        {
            lock (this)
            {
                _timer.Dispose();

                _processService.Processed -= Processed;

                _ledRecorder.Stop(_frameRate);
                _mergedRecorder.Stop(_frameRate);

                foreach (var recorder in _recoderDictionary.Values)
                    recorder.Stop(_frameRate);

                RecordingStarted?.Invoke(false);
            }
        }
    }
}