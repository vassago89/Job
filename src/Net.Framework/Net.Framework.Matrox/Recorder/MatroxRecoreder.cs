using Matrox.MatroxImagingLibrary;
using Net.Framework.Data.ImageDatas;
using Net.Framework.Data.Recorder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Matrox.Recorder
{
    public class MatroxRecoreder<TData> : IRecorder<TData>, IDisposable
    {
        private bool _onRecord;

        public MIL_ID _source;

        private int _width;
        public int Width => _width;

        private int _height;
        public int Height => _height;

        private int _channels;
        public int Channels => _channels;
        
        private string _path;

        public void Intialize(int width, int height, int channels)
        {
            _width = width;
            _height = height;
            _channels = channels;

            
            MIL.MbufAllocColor(MIL.M_DEFAULT_HOST,
                        _channels,
                        _width,
                        _height,
                        8 + MIL.M_UNSIGNED,
                        MIL.M_IMAGE,
                        ref _source);
        }

        public void Enqueue(TData[] data)
        {
            lock (this)
            {
                if (_onRecord == false)
                    return;
                
                MIL.MbufPut(_source, data.Cast<byte>().ToArray());
                MIL.MbufExportSequence(_path, MIL.M_AVI_MJPG, ref _source, 1, MIL.M_DEFAULT, MIL.M_WRITE);
            }
        }

        public void Start(string path)
        {
            lock (this)
            {
                _path = path;

                MIL.MbufExportSequence(_path, MIL.M_AVI_MJPG, MIL.M_NULL, MIL.M_NULL, MIL.M_DEFAULT, MIL.M_OPEN);
                _onRecord = true;
            }
        }

        public void Stop(double frameRate)
        {
            lock (this)
            {
                MIL.MbufExportSequence(_path, MIL.M_AVI_MJPG, MIL.M_NULL, MIL.M_NULL, frameRate, MIL.M_CLOSE);
                _onRecord = false;
            }
        }

        public void Dispose()
        {
            MIL.MbufFree(_source);
        }
    }
}
