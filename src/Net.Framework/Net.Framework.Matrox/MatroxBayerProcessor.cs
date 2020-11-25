﻿using Matrox.MatroxImagingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Matrox
{
    public class MatroxBayerProcessor : IDisposable
    {
        private MIL_ID _source;
        private MIL_ID _destination;
        private MIL_ID _coef;

        private int _width;
        public int Width => _width;

        private int _height;
        public int Height => _height;

        public MatroxBayerProcessor(int width, int height)
        {
            _width = width;
            _height = height;

            MIL.MbufAlloc1d(MIL.M_DEFAULT_HOST, 3, 32 + MIL.M_FLOAT, MIL.M_ARRAY, ref _coef);

            float[] initValues = { 1.0f, };
            MIL.MbufPut(_coef, initValues);

            MIL.MbufAllocColor(MIL.M_DEFAULT_HOST,
                        3,
                        _width,
                        _height,
                        8 + MIL.M_UNSIGNED,
                        MIL.M_IMAGE + MIL.M_PROC,
                        ref _source);

            MIL.MbufAllocColor(MIL.M_DEFAULT_HOST,
                        3,
                        _width,
                        _height,
                        8 + MIL.M_UNSIGNED,
                        MIL.M_IMAGE + MIL.M_PROC,
                        ref _destination);

           
        }

        public void GetCoefficient(byte[] data)
        {
            MIL.MbufPut(_source, data);
            MIL.MbufBayer(_source, MIL.M_NULL, _coef, MIL.M_BAYER_RG + MIL.M_WHITE_BALANCE_CALCULATE);
        }

        public byte[] Process(byte[] data)
        {
            MIL.MbufPut(_source, data);
            MIL.MbufBayer(_source, _destination, _coef, MIL.M_BAYER_RG);

            var buffer = new byte[_width * _height * 3];
            MIL.MbufGet(_destination, buffer);
            return buffer;
        }

        public void Dispose()
        {
            MIL.MbufFree(_source);
            MIL.MbufFree(_destination);
            MIL.MbufFree(_coef);
        }
    }
}