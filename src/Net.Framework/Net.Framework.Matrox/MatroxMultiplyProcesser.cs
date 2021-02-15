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
        private MIL_ID _src1;
        private MIL_ID _src2;
        
        private MIL_ID _child1;
        private MIL_ID _child2;
        
        private byte[] _buffer;
        private int _width;
        private int _height;

        public MatroxMultiplyProcesser(int width, int height, int channel1, int channel2)
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

            MIL.MbufChildColor(_color, channel1 == 0 ? MIL.M_RED : channel1 == 1 ? MIL.M_GREEN : MIL.M_BLUE, ref _child1);
            MIL.MbufChildColor(_color, channel2 == 0 ? MIL.M_RED : channel2 == 1 ? MIL.M_GREEN : MIL.M_BLUE, ref _child2);

            MIL.MbufAllocColor(MIL.M_DEFAULT_HOST,
                        1,
                        width,
                        height,
                        8 + MIL.M_UNSIGNED,
                        MIL.M_IMAGE + MIL.M_PROC,
                        ref _src1);

            MIL.MbufAllocColor(MIL.M_DEFAULT_HOST,
                        1,
                        width,
                        height,
                        8 + MIL.M_UNSIGNED,
                        MIL.M_IMAGE + MIL.M_PROC,
                        ref _src2);
            
            _buffer = new byte[width * height * 3];
            
            MatroxObjectPool.Add(this);
        }   

        public byte[] Multiply(byte[] color, byte[] src1, byte[] src2, double colorRatio, double ratio1, double ratio2)
        {
            MIL.MbufPut(_color, color);
            MIL.MbufPut(_src1, src1);
            MIL.MbufPut(_src2, src2);
            
            MIL.MimArithMultiple(_child1, colorRatio, _src1, ratio1, colorRatio + ratio1, _child1, MIL.M_MULTIPLY_ACCUMULATE_2, MIL.M_DEFAULT);
            MIL.MimArithMultiple(_child2, colorRatio, _src2, ratio2, colorRatio + ratio2, _child2, MIL.M_MULTIPLY_ACCUMULATE_2, MIL.M_DEFAULT);
            
            MIL.MbufGet(_color, _buffer);

            return _buffer;
        }

        public byte[] Multiply(byte[] color, byte[] src, double colorRatio, double ratio)
        {
            MIL.MbufPut(_color, color);
            MIL.MbufPut(_src1, src);

            MIL.MimArithMultiple(_child1, colorRatio, _src1, ratio, colorRatio + ratio, _child1, MIL.M_MULTIPLY_ACCUMULATE_2, MIL.M_DEFAULT);

            MIL.MbufGet(_color, _buffer);

            return _buffer;
        }

        public void Dispose()
        {
            MIL.MbufFree(_child1);
            MIL.MbufFree(_child2);

            MIL.MbufFree(_color);
            MIL.MbufFree(_src1);
            MIL.MbufFree(_src2);
        }
    }
}
