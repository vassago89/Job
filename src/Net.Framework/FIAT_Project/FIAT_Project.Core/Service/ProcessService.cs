using Net.Framework.Algorithm;
using Net.Framework.Algorithm.Enums;
using Net.Framework.Matrox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Core.Service
{
    public enum ELazer
    {
        L660, L760
    }

    public class ProcessService
    {
        private bool _onBayer;

        public Action<int, int, byte[][]> Processed;

        private MatroxBayerProcessor _bayerProcessor;
        private AutoThresholder _autoThresholder;

        private SystemConfig _systemConfig;

        public ProcessService(GrabService grabService, SystemConfig systemConfig)
        {
            _systemConfig = systemConfig;

            _onBayer = true;

            grabService.Grabbed += ServiceGrabbed;

            _bayerProcessor = new MatroxBayerProcessor(grabService.Width, grabService.Height);
            _autoThresholder = new AutoThresholder();
        }

        private void ServiceGrabbed(int width, int height, byte[][] datas)
        {
            if (_onBayer)
                _bayerProcessor.GetCoefficient(datas[0]);

            datas[0] = _bayerProcessor.Process(datas[0]);
            
            var mergeDatas = new byte[4][];
            for (int i = 0; i < 3; i++)
                mergeDatas[i] = datas[i];

            mergeDatas[3] = new byte[datas[0].Length];
            Merge(mergeDatas);

            Processed?.Invoke(width, height, mergeDatas);
        }

        private void Merge(byte[][] datas)
        {
            if (_systemConfig.AutoDictionary[ELazer.L660] == true)
                datas[1] = AutoThreshold(datas[1], 0, _systemConfig.MethodDictionary[ELazer.L660]);
            if (_systemConfig.AutoDictionary[ELazer.L760] == true)
                datas[2] = AutoThreshold(datas[2], 1, _systemConfig.MethodDictionary[ELazer.L760]);

            if (_systemConfig.ManualDictionary[ELazer.L660] == true)
                datas[1] = Threshold(datas[1], 0, _systemConfig.ThresholdDictionary[ELazer.L660]);
            if (_systemConfig.ManualDictionary[ELazer.L760] == true)
                datas[2] = Threshold(datas[2], 1, _systemConfig.ThresholdDictionary[ELazer.L760]);

            var length = datas[0].Length / 3;

            if (_systemConfig.AutoDictionary[ELazer.L660] || _systemConfig.ManualDictionary[ELazer.L660])
            {
                for (int i = 0; i < length; i++)
                {
                    if (datas[1][i] == byte.MaxValue)
                        datas[3][i] = byte.MaxValue;
                }
            }

            var doubleLength = length * 2;
            if (_systemConfig.AutoDictionary[ELazer.L760] || _systemConfig.ManualDictionary[ELazer.L760])
            {
                for (int i = length; i < doubleLength; i ++)
                {
                    if (datas[2][i] == byte.MaxValue)
                        datas[3][i] = byte.MaxValue;
                }
            }

            for (int ri = 0, gi = length, bi = doubleLength; ri < length; ri++, gi++, bi++)
            {
                if (datas[3][ri] == 0 && datas[3][gi] == 0)
                {
                    datas[3][ri] = datas[0][ri];
                    datas[3][gi] = datas[0][gi];
                    datas[3][bi] = datas[0][bi];
                }
            }
        }

        private byte[] AutoThreshold(byte[] data, int channel, EThresholdMethod method)
        {
            var histo = _autoThresholder.GetHistogram(data);
            var value = -1.0;
            switch (method)
            {
                case EThresholdMethod.Otsu:
                    value = _autoThresholder.Otsu(histo);
                    break;
                case EThresholdMethod.Li:
                    value = _autoThresholder.Li(histo);
                    break;
                case EThresholdMethod.Triangle:
                    value = _autoThresholder.Triangle(histo);
                    break;
            }
            
            return Threshold(data, channel, value);
        }

        private byte[] Threshold(byte[] data, int channel, double value)
        {
            var binaryData = new byte[data.Length];
            var startIndex = channel * data.Length / 3;
            var endIndex = startIndex + data.Length / 3;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (data[i] >= value)
                    binaryData[i] = 255;
            }

            return binaryData;
        }

        public void BayerStart()
        {
            _onBayer = true;
        }

        public void BayerStop()
        {
            _onBayer = false;
        }
    }
}
