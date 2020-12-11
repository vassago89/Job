using FIAT_Project.Core;
using FIAT_Project.Core.Enums;
using FIAT_Project.Core.Service;
using LiveCharts;
using LiveCharts.Wpf;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FIAT_Project.Wpf.ViewModels
{
    public class StatisticsPanelViewModel : BindableBase
    {
        private ELazer _lazer;
        public ELazer Lazer
        {
            get => _lazer;
            set
            {
                SetProperty(ref _lazer, value);
                IsLogScale = _systemConfig.IsLogScaleDictionary[value];
                IsAutoScale = _systemConfig.IsAutoScaleDictionary[value];
            }
        }
        public int ImageIndex { get; set; }
        
        private ChartValues<double> _chartValues;
        public ChartValues<double> ChartValues
        {
            get => _chartValues;
            set => SetProperty(ref _chartValues, value);
        }

        private byte _minValue;
        public byte MinValue
        {
            get => _minValue;
            set => SetProperty(ref _minValue, value);
        }

        private byte _maxValue;
        public byte MaxValue
        {
            get => _maxValue;
            set => SetProperty(ref _maxValue, value);
        }

        private double _avgValue;
        public double AvgValue
        {
            get => _avgValue;
            set => SetProperty(ref _avgValue, value);
        }

        private double _stdDevValue;
        public double StdDevValue
        {
            get => _stdDevValue;
            set => SetProperty(ref _stdDevValue, value);
        }

        private bool _isAutoScale;
        public bool IsAutoScale
        {
            get => _isAutoScale;
            set
            {
                SetProperty(ref _isAutoScale, value);
                _systemConfig.IsAutoScaleDictionary[Lazer] = value;
            }
        }

        private bool _isLogScale;
        public bool IsLogScale
        {
            get => _isLogScale;
            set
            {
                SetProperty(ref _isLogScale, value);
                _systemConfig.IsLogScaleDictionary[Lazer] = value;
            }
        }

        private int _axisMinValue;
        public int AxisMinValue
        {
            get => _axisMinValue;
            set => SetProperty(ref _axisMinValue, value);
        }

        private int _axisMaxValue;
        public int AxisMaxValue
        {
            get => _axisMaxValue;
            set => SetProperty(ref _axisMaxValue, value);
        }

        private int _step;
        public int Step
        {
            get => _step;
            set => SetProperty(ref _step, value);
        }

        private SystemConfig _systemConfig;

        public StatisticsPanelViewModel(ProcessService processService, SystemConfig systemConfig)
        {
            _systemConfig = systemConfig;

            IsAutoScale = true;
            AxisMinValue = 0;
            AxisMaxValue = 260;
            Step = 15;

            processService.HistoProcessed += Processed;

            ChartValues = new ChartValues<double>();
        }

        private void Processed(ELazer lazer, long[] histo)
        {
            if (Lazer != lazer)
                return;
            
            MinValue = 0;
            for (int i = 0; i < histo.Length; i++)
            {
                if (histo[i] > 0)
                {
                    MinValue = (byte)i;
                    break;
                }
            }

            MaxValue = 0;
            for (int i = histo.Length - 1; i >= 0; i--)
            {
                if (histo[i] > 0)
                {
                    MaxValue = (byte)i;
                    break;
                }
            }

            var logHisto = Array.ConvertAll(histo, (h) => (double)h);//.ToList().ConvertAll(h => (double)h).ToArray();
            if (_isLogScale)
            {
                logHisto = new double[histo.Length];
                for (int i = 0; i < logHisto.Length; i++)
                    logHisto[i] = Math.Log(1 + histo[i]);
            }

            ChartValues.Clear();
            ChartValues.AddRange(logHisto);

            if (IsAutoScale)
            {
                Step = 15;
                AxisMinValue = MinValue;
                AxisMaxValue = MaxValue;
            }
            else
            {
                Step = 15;
                AxisMinValue = 0;
                AxisMaxValue = 260;
            }

            double sum = 0;
            double count = 0;
            double sumOfDev = 0;
            for (int i = 0; i < histo.Length; i++)
            {
                var temp = histo[i] * i;

                sum += temp;
                count += histo[i];
                sumOfDev += i * temp;
            }
            
            AvgValue = sum / count;
            
            StdDevValue = Math.Sqrt((sumOfDev / count) - (AvgValue * AvgValue));
        }
    }
}
