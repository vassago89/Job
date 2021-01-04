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
        private MIL_ID _first;
        private MIL_ID _second;
        private MIL_ID _destination;

        private byte[] _buffer;

        private int _const1;
        private int _const2;
        private int _const3;

        public MatroxMultiplyProcesser(int width, int height, int firstChannel, int secondChannel, int destinationChannel, int const1, int const2, int const3)
        {
            MIL.MbufAllocColor(MIL.M_DEFAULT_HOST,
                        firstChannel,
                        width,
                        height,
                        8 + MIL.M_UNSIGNED,
                        MIL.M_IMAGE + MIL.M_PROC,
                        ref _first);

            MIL.MbufAllocColor(MIL.M_DEFAULT_HOST,
                        secondChannel,
                        width,
                        height,
                        8 + MIL.M_UNSIGNED,
                        MIL.M_IMAGE + MIL.M_PROC,
                        ref _second);

            MIL.MbufAllocColor(MIL.M_DEFAULT_HOST,
                        destinationChannel,
                        width,
                        height,
                        8 + MIL.M_UNSIGNED,
                        MIL.M_IMAGE + MIL.M_PROC,
                        ref _destination);

            _buffer = new byte[width * height * destinationChannel];

            _const1 = const1;
            _const2 = const2;
            _const3 = const3;

            MatroxObjectPool.Add(this);
        }

        public byte[] Multiply(byte[] first, byte[] second)
        {
            MIL.MbufPut(_first, first);
            MIL.MbufPut(_second, second);

            MIL.MimArithMultiple(_first, _const1, _second, _const2, _const3, _destination, MIL.M_MULTIPLY_ACCUMULATE_2, MIL.M_DEFAULT);

            MIL.MbufGet(_destination, _buffer);

            return _buffer;
        }

        public void Dispose()
        {
            MIL.MbufFree(_first);
            MIL.MbufFree(_second);
            MIL.MbufFree(_destination);
        }
    }
}
