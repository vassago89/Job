using Matrox.MatroxImagingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Matrox
{
    public class MatroxMultiplyProcesser : IDisposable
    {
        private MIL_ID _color;
        private MIL_ID _src;
        
        private byte[] _buffer;
        private int _width;
        private int _height;

        public MatroxMultiplyProcesser(int width, int height)
        {
            _width = width;
            _height = height;

            MIL.MbufAllocColor(MIL.M_DEFAULT_HOST,
                        3,
                        width,
                        height,
                        8 + MIL.M_UNSIGNED,
                        MIL.M_IMAGE + MIL.M_PROC,
                        ref _color);
            
            MIL.MbufAllocColor(MIL.M_DEFAULT_HOST,
                        1,
                        width,
                        height,
                        8 + MIL.M_UNSIGNED,
                        MIL.M_IMAGE + MIL.M_PROC,
                        ref _src);
            
            _buffer = new byte[width * height * 3];
            
            MatroxObjectPool.Add(this);
        }   

        public byte[] Multiply(
            byte[] color, byte[] src1, byte[] src2, 
            double colorRatio, double ratio1, double ratio2,
            int chennel1, int chennel2)
        {
            MIL.MbufPut(_color, color);

            MIL.MbufPut(_src, src1);
            var child = MIL.MbufChildColor2d(_color, chennel1, 0, 0, _width, _height, MIL.M_NULL);
            MIL.MimArithMultiple(child, colorRatio, _src, ratio1, 1, child, MIL.M_MULTIPLY_ACCUMULATE_2, MIL.M_DEFAULT);
            MIL.MbufFree(child);

            MIL.MbufPut(_src, src2);
            child = MIL.MbufChildColor2d(_color, chennel2, 0, 0, _width, _height, MIL.M_NULL);
            MIL.MimArithMultiple(child, colorRatio, _src, ratio2, 1, child, MIL.M_MULTIPLY_ACCUMULATE_2, MIL.M_DEFAULT);
            MIL.MbufFree(child);

            MIL.MbufGet(_color, _buffer);

            return _buffer;
        }

        public byte[] Multiply(byte[] color, byte[] src, double colorRatio, double ratio, int chennel)
        {
            MIL.MbufPut(_color, color);
            MIL.MbufPut(_src, src);

            var child = MIL.MbufChildColor2d(_color, chennel, 0, 0, _width, _height, MIL.M_NULL);
            MIL.MimArithMultiple(child, colorRatio, _src, ratio, 1, child, MIL.M_MULTIPLY_ACCUMULATE_2, MIL.M_DEFAULT);
            MIL.MbufFree(child);

            MIL.MbufGet(_color, _buffer);

            return _buffer;
        }

        public void Dispose()
        {
            MIL.MbufFree(_color);
            MIL.MbufFree(_src);
        }
    }
}
