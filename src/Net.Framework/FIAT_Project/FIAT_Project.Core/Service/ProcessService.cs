using FIAT_Project.Core.Enums;
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
    public class ProcessService
    {
        public Action<int, int, byte[][]> Processed;
        public Action<ELazer, long[]> HistoProcessed;

        private MatroxBayerProcessor _bayerProcessor;
        private AutoThresholder _autoThresholder;
        private ShapeDrawer _shapeDrawer;
        private SystemConfig _systemConfig;

        private Dictionary<ELazer, byte[]> _maskDictionary;
        private Dictionary<ELazer, byte[]> _bufferDictionary;

        private Dictionary<ELazer, MatroxMultiplyProcesser> _multiplyProcesserDictionary;
        private MatroxMultiplyProcesser _mergeMultiplyProcesser;
        private byte[] _mergedBuffer;

        public ProcessService(GrabService grabService, SystemConfig systemConfig)
        {
            _systemConfig = systemConfig;

            grabService.Grabbed += ServiceGrabbed;

            _bayerProcessor = new MatroxBayerProcessor(grabService.Width, grabService.Height);
            _autoThresholder = new AutoThresholder();
            _shapeDrawer = new ShapeDrawer(grabService.Width, grabService.Height, 1);

            _maskDictionary = new Dictionary<ELazer, byte[]>();
            _maskDictionary[ELazer.L660] = new byte[grabService.Width * grabService.Height];
            _maskDictionary[ELazer.L760] = new byte[grabService.Width * grabService.Height];

            _bufferDictionary = new Dictionary<ELazer, byte[]>();
            _bufferDictionary[ELazer.L660] = new byte[grabService.Width * grabService.Height];
            _bufferDictionary[ELazer.L760] = new byte[grabService.Width * grabService.Height];

            _mergedBuffer = new byte[grabService.Width * grabService.Height * 3];

            //_multiplyProcesserDictionary = new Dictionary<ELazer, MatroxMultiplyProcesser>();
            //_multiplyProcesserDictionary[ELazer.L660] = new MatroxMultiplyProcesser(grabService.Width, grabService.Height, 3, 1, 1, 2, 2, 2);
            //_multiplyProcesserDictionary[ELazer.L760] = new MatroxMultiplyProcesser(grabService.Width, grabService.Height, 3, 1, 1, 2, 2, 2);

            //_mergeMultiplyProcesser = new MatroxMultiplyProcesser(grabService.Width, grabService.Height, 3, )
        }

        public void SetCoefficient(float red, float green, float blue)
        {
            _bayerProcessor.SetCoefficient(new float[] { red, green, blue });
        }

        private void ServiceGrabbed(int width, int height, byte[][] datas)
        {
            if (_systemConfig.OnAutoBayer)
                _systemConfig.CoefficientValues = _bayerProcessor.GenerateCoefficient(datas[0]);
            else
                _bayerProcessor.SetCoefficient(_systemConfig.CoefficientValues);

            datas[0] = _bayerProcessor.Process(datas[0]);

            Array.Clear(_mergedBuffer, 0, _mergedBuffer.Length);

            if (_systemConfig.UseDictionary[ELazer.L660] == true)
                datas[1] = Process(ELazer.L660, datas[1], width, height);

            if (_systemConfig.UseDictionary[ELazer.L760] == true)
                datas[2] = Process(ELazer.L760, datas[2], width, height);

            var mergeDatas = new byte[][] { datas[0], datas[1], datas[2], _mergedBuffer };

            if (_systemConfig.ThresholdMode == EThresholdMode.BinaryMode)
            {
                var lengthOfOneChannel = width * height;
                for (int redIndex = 0, greenIndex = lengthOfOneChannel, blueIndex = lengthOfOneChannel * 2; redIndex < lengthOfOneChannel; redIndex++, greenIndex++, blueIndex++)
                {
                    if (mergeDatas[3][redIndex] == 0 && mergeDatas[3][greenIndex] == 0)
                    {
                        mergeDatas[3][redIndex] = datas[0][redIndex];
                        mergeDatas[3][greenIndex] = datas[0][greenIndex];
                        mergeDatas[3][blueIndex] = datas[0][blueIndex];
                    }
                }
            }

            Processed?.Invoke(width, height, mergeDatas);
        }

        private byte[] Process(ELazer lazer, byte[] data, int width, int height)
        {
            var histo = _autoThresholder.GetHistogram(data);

            switch (_systemConfig.ThresholdMode)
            {
                case EThresholdMode.GrayMode:
                    break;
                case EThresholdMode.BinaryMode:
                    data = Threshold(lazer, data, width, height, histo);
                    Merge(lazer, data, _mergedBuffer, width * height, lazer == ELazer.L660 ? 0 : 1);
                    break;
            }

            return data;
        }

        private void Merge(ELazer lazer, byte[] dataOfSource, byte[] dataOfMerged, int lengthOfOneChannel, int channelIndex)
        {
            for (int indexOfSource = 0, indexOfDestination = channelIndex * lengthOfOneChannel; indexOfSource < lengthOfOneChannel; indexOfSource++, indexOfDestination++)
            {
                if (dataOfSource[indexOfSource] == byte.MaxValue)
                    dataOfMerged[indexOfDestination] = byte.MaxValue;
            }
        }
        
        private byte[] AutoThreshold(ELazer lazer, byte[] data, long[] histo, EThresholdMethod method = EThresholdMethod.Li)
        {
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

            return Threshold(lazer, data, value);
        }

        private byte[] Threshold(ELazer lazer, byte[] data, double value)
        {
            var binaryData = _bufferDictionary[lazer];
            Array.Clear(binaryData, 0, binaryData.Length);
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] >= value)
                    binaryData[i] = byte.MaxValue;
            }

            return binaryData;
        }

        private byte[] Threshold(ELazer lazer, byte[] data, int width, int height, long[] histo)
        {
            var dataOfSource = data;
            var srcRect = new Rectangle(0, 0, width, height);

            if (_systemConfig.AutoDictionary[lazer] == true)
                dataOfSource = AutoThreshold(lazer, dataOfSource, histo, _systemConfig.MethodDictionary[lazer]);
            else if (_systemConfig.ManualDictionary[lazer] == true)
                dataOfSource = Threshold(lazer,dataOfSource, _systemConfig.ThresholdDictionary[lazer]);

            if (_systemConfig.OnROIDictionary[lazer])
            {
                if (_systemConfig.OnROIChangedDictionary[lazer])
                {
                    _systemConfig.OnROIChangedDictionary[lazer] = false;

                    Array.Clear(_maskDictionary[lazer], 0, _maskDictionary[lazer].Length);
                    switch (_systemConfig.ROIShapeDictionary[lazer])
                    {
                        case EShape.Rectangle:
                            _maskDictionary[lazer] = _shapeDrawer.DrawRect(_maskDictionary[lazer], _systemConfig.ROIRectangleDictionary[lazer], true);
                            break;
                        case EShape.Ellipse:
                            _maskDictionary[lazer] = _shapeDrawer.DrawEllipse(_maskDictionary[lazer], _systemConfig.ROIEllipseDictionary[lazer], true);
                            break;
                        case EShape.Polygon:
                            _maskDictionary[lazer] = _shapeDrawer.DrawPolygon(_maskDictionary[lazer], _systemConfig.ROIPointDictionary[lazer], true);
                            break;
                    }
                }

                histo = _autoThresholder.GetHistogram(data, _maskDictionary[lazer]);
            }

            HistoProcessed?.Invoke(lazer, histo);

            return dataOfSource;
        }
    }
}