using Matrox.MatroxImagingLibrary;
using Net.Framework.Data.ImageDatas;
using Net.Framework.Data.Recorder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Matrox
{
    public class MatroxCapturer<TData> : ICapturer<TData>, IDisposable
    {
        public MIL_ID _source;

        private int _width;
        public int Width => _width;

        private int _height;
        public int Height => _height;

        private int _channels;
        public int Channels => _channels;

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

            MatroxObjectPool.Add(this);
        }

        public void Capture(TData[] data, string path)
        {
            lock (this)
            {
                MIL.MbufPut(_source, data.Cast<byte>().ToArray());
                MIL.MbufExport(path, MIL.M_BMP, _source);
            }
        }
        
        public void Dispose()
        {
            MIL.MbufFree(_source);
        }
    }
}
