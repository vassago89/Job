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
        public ELazer Lazer { get; set; }
        public int ImageIndex { get; set; }
        
        private ChartValues<long> _chartValues;
        public ChartValues<long> ChartValues
        {
            get => _chartValues;
            set => SetProperty(ref _chartValues, value);
        }

        public StatisticsPanelViewModel(ProcessService processService, SystemConfig systemConfig)
        {
            processService.HistoProcessed += Processed;

            ChartValues = new ChartValues<long>();
        }

        private void Processed(ELazer lazer, long[] histo)
        {
            if (Lazer != lazer)
                return;

            ChartValues.Clear();
            ChartValues.AddRange(histo);
        }
    }
}
