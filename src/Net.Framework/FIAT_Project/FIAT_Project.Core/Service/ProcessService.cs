﻿using FIAT_Project.Core.Enums;
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
        
        private MatroxMultiplyProcesser _mergeMultiplyProcesser;
        private byte[] _mergedBuffer;

        private GrabService _grabService;

        public ProcessService(GrabService grabService, SystemConfig systemConfig)
        {
            _systemConfig = systemConfig;
            _grabService = grabService;

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

            //_mergedBuffer = new byte[grabService.Width * grabService.Height * 3];

            //_multiplyProcesserDictionary = new Dictionary<ELazer, MatroxMultiplyProcesser>();
            //_multiplyProcesserDictionary[ELazer.L660] = new MatroxMultiplyProcesser(grabService.Width, grabService.Height, 3, 1, 1, 2, 2, 2);
            //_multiplyProcesserDictionary[ELazer.L760] = new MatroxMultiplyProcesser(grabService.Width, grabService.Height, 3, 1, 1, 2, 2, 2);

            //_mergeMultiplyProcesser = new MatroxMultiplyProcesser(grabService.Width, grabService.Height, 3, )

            _mergeMultiplyProcesser =
                new MatroxMultiplyProcesser(
                    grabService.Width,
                    grabService.Height, 0, 1);
        }

        public void SetCoefficient(float red, float green, float blue)
        {
            _bayerProcessor.SetCoefficient(new float[] { red, green, blue });
        }
        
        private void ServiceGrabbed(int width, int height, byte[][] datas)
        {
            var tasks = new List<Task>();

            tasks.Add(Task.Run(() =>
            {
                if (_systemConfig.OnAutoBayer)
                    _systemConfig.CoefficientValues = _bayerProcessor.GenerateCoefficient(datas[0]);
                else
                    _bayerProcessor.SetCoefficient(_systemConfig.CoefficientValues);

                datas[0] = _bayerProcessor.Process(datas[0]);
            }));
            
            if (_systemConfig.UseDictionary[ELazer.L660] == true)
                tasks.Add(Task.Run(() => StatisticsProcess(ELazer.L660, datas[1])));

            if (_systemConfig.UseDictionary[ELazer.L760] == true)
                tasks.Add(Task.Run(() => StatisticsProcess(ELazer.L760, datas[2])));

            _mergedBuffer = new byte[_grabService.Width * _grabService.Height * 3];

            switch (_systemConfig.ThresholdMode)
            {
                case EThresholdMode.GrayMode:
                    var buffer = _mergeMultiplyProcesser.Multiply(datas[0], datas[1], datas[2], _systemConfig.RatioColor, _systemConfig.Ratio660, _systemConfig.Ratio760);
                    Array.Copy(buffer, _mergedBuffer, _mergedBuffer.Length);
                    break;
                case EThresholdMode.BinaryMode:
                    if (_systemConfig.UseDictionary[ELazer.L660] == true)
                        tasks.Add(Task.Run(() => datas[1] = BinaryProcess(ELazer.L660, datas[1], width, height)));

                    if (_systemConfig.UseDictionary[ELazer.L760] == true)
                        tasks.Add(Task.Run(() => datas[2] = BinaryProcess(ELazer.L760, datas[2], width, height)));

                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();

                    var lengthOfOneChannel = width * height;
                    for (int redIndex = 0, greenIndex = lengthOfOneChannel, blueIndex = lengthOfOneChannel * 2; redIndex < lengthOfOneChannel; redIndex++, greenIndex++, blueIndex++)
                    {
                        if (_mergedBuffer[redIndex] == 0 && _mergedBuffer[greenIndex] == 0)
                        {
                            _mergedBuffer[redIndex] = datas[0][redIndex];
                            _mergedBuffer[greenIndex] = datas[0][greenIndex];
                            _mergedBuffer[blueIndex] = datas[0][blueIndex];
                        }
                    }
                    break;
            }

            Task.WaitAll(tasks.ToArray());

            var mergeDatas = new byte[][] { datas[0], datas[1], datas[2], _mergedBuffer };

            //Task.Run(() => Processed?.Invoke(width, height, mergeDatas));
            Processed?.Invoke(width, height, mergeDatas);
        }

        private byte[] BinaryProcess(ELazer lazer, byte[] data, int width, int height)
        {
            var histo = _autoThresholder.GetHistogram(data);

            data = Threshold(lazer, data, width, height, histo);
            Merge(lazer, data, _mergedBuffer, width * height, lazer == ELazer.L660 ? 0 : 1);

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

        private void StatisticsProcess(ELazer lazer, byte[] data)
        {
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

                HistoProcessed?.Invoke(lazer, _autoThresholder.GetHistogram(data, _maskDictionary[lazer]));
            }
            else
            {
                HistoProcessed?.Invoke(lazer, _autoThresholder.GetHistogram(data));
            }
        }

        private byte[] Threshold(ELazer lazer, byte[] data, int width, int height, long[] histo)
        {
            var dataOfSource = data;
            var srcRect = new Rectangle(0, 0, width, height);

            if (_systemConfig.AutoDictionary[lazer] == true)
                dataOfSource = AutoThreshold(lazer, dataOfSource, histo, _systemConfig.MethodDictionary[lazer]);
            else if (_systemConfig.ManualDictionary[lazer] == true)
                dataOfSource = Threshold(lazer,dataOfSource, _systemConfig.ThresholdDictionary[lazer]);
            
            return dataOfSource;
        }
    }
}