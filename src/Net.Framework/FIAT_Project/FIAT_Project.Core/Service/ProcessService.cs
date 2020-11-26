using Net.Framework.Algorithm;
using Net.Framework.Algorithm.Enums;
using Net.Framework.Matrox;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        public Action<int, int, byte[][]> Processed;

        private MatroxBayerProcessor _bayerProcessor;
        private AutoThresholder _autoThresholder;

        private SystemConfig _systemConfig;

        public ProcessService(GrabService grabService, SystemConfig systemConfig)
        {
            _systemConfig = systemConfig;

            grabService.Grabbed += ServiceGrabbed;

            _bayerProcessor = new MatroxBayerProcessor(grabService.Width, grabService.Height);
            _autoThresholder = new AutoThresholder();
        }

        private void ServiceGrabbed(int width, int height, byte[][] datas)
        {
            if (_systemConfig.OnBayer)
                _bayerProcessor.GetCoefficient(datas[0]);

            datas[0] = _bayerProcessor.Process(datas[0]);

            if (_systemConfig.UseDictionary[ELazer.L660] == true)
                datas[1] = Threshold(ELazer.L660, datas[1], width, height);

            if (_systemConfig.UseDictionary[ELazer.L760] == true)
                datas[2] = Threshold(ELazer.L760, datas[2], width, height);

            var lengthOfOneChannel = width * height;

            var mergeDatas = new byte[datas.Length + 1][];
            mergeDatas[0] = datas[0];
            for (int i = 1; i < 4; i++)
                mergeDatas[i] = new byte[lengthOfOneChannel * 3];

            if (_systemConfig.UseDictionary[ELazer.L660] == true)
                Merge(ELazer.L660, datas[1], mergeDatas[1], mergeDatas[3], lengthOfOneChannel, 0);

            if (_systemConfig.UseDictionary[ELazer.L760] == true)
                Merge(ELazer.L760, datas[2], mergeDatas[2], mergeDatas[3], lengthOfOneChannel, 1);

            for (int redIndex = 0, greenIndex = lengthOfOneChannel, blueIndex = lengthOfOneChannel * 2; redIndex < lengthOfOneChannel; redIndex++, greenIndex++, blueIndex++)
            {
                if (mergeDatas[3][redIndex] == 0 && mergeDatas[3][greenIndex] == 0)
                {
                    mergeDatas[3][redIndex] = datas[0][redIndex];
                    mergeDatas[3][greenIndex] = datas[0][greenIndex];
                    mergeDatas[3][blueIndex] = datas[0][blueIndex];
                }
            }

            Processed?.Invoke(width, height, mergeDatas);
        }

        private void Merge(ELazer lazer, byte[] dataOfSource, byte[] dataOfDestination, byte[] dataOfMerged, int lengthOfOneChannel, int channelIndex)
        {
            if (_systemConfig.AutoDictionary[lazer] || _systemConfig.ManualDictionary[lazer])
            {
                for (int indexOfSource = 0, indexOfDestination = channelIndex * lengthOfOneChannel; indexOfSource < lengthOfOneChannel; indexOfSource++, indexOfDestination++)
                {
                    if (dataOfSource[indexOfSource] == byte.MaxValue)
                    {
                        dataOfDestination[indexOfDestination] = byte.MaxValue;
                        dataOfMerged[indexOfDestination] = byte.MaxValue;
                    }
                }
            }
        }

        private byte[] AutoThreshold(byte[] data, EThresholdMethod method)
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
            
            return Threshold(data, value); ;
        }

        private byte[] Threshold(byte[] data, double value)
        {
            var binaryData = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] >= value)
                    binaryData[i] = byte.MaxValue;
            }

            return binaryData;
        }

        private byte[] Threshold(ELazer eLazer, byte[] data, int width, int height)
        {
            var dataOfSource = data;
            var srcRect = new Rectangle(0, 0, width, height);
            var destRect = _systemConfig.RectROI;

            destRect.Intersect(srcRect);

            if (_systemConfig.OnROI && destRect.Width > 0 && destRect.Height > 0)
                dataOfSource = ImageHelper.GetDataOfROI(dataOfSource, width, _systemConfig.RectROI.X, _systemConfig.RectROI.Y, _systemConfig.RectROI.Width, _systemConfig.RectROI.Height);

            if (_systemConfig.AutoDictionary[eLazer] == true)
                dataOfSource = AutoThreshold(dataOfSource, _systemConfig.MethodDictionary[eLazer]);
            else if (_systemConfig.ManualDictionary[eLazer] == true)
                dataOfSource = Threshold(dataOfSource, _systemConfig.ThresholdDictionary[eLazer]);

            if (_systemConfig.OnROI && destRect.Width > 0 && destRect.Height > 0)
                ImageHelper.SetROIToSource(data, dataOfSource, width, _systemConfig.RectROI.X, _systemConfig.RectROI.Y, _systemConfig.RectROI.Width, _systemConfig.RectROI.Height);
            else
                data = dataOfSource;

            return data;
        }
    }
}
