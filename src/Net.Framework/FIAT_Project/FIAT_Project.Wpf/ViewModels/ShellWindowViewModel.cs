using FIAT_Project.Core.Enums;
using FIAT_Project.Core.Service;
using FIAT_Project.Wpf.Datas;
using FIAT_Project.Wpf.Views;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FIAT_Project.Wpf.ViewModels
{
    public class ShellWindowViewModel : BindableBase
    {
        private bool _recordingStarted;
        public bool RecordingStarted
        {
            get => _recordingStarted;
            set => SetProperty(ref _recordingStarted, value);
        }

        public SettingStore SettingStore { get; }

        public ShellWindowViewModel(RecordService recordService, SettingStore settingStore)
        {
            recordService.RecordingStarted += Started;
            SettingStore = settingStore;
        }

        private void Started(bool state)
        {
            RecordingStarted = state;
        }
    }
}
