using FIAT_Project.Core;
using FIAT_Project.Core.Service;
using Microsoft.Win32;
using Net.Framework.Device.Matrox;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FIAT_Project.Wpf.ViewModels
{
    public class ControlPanelViewModel : BindableBase
    {
        private bool _onGrab;
        public bool OnGrab
        {
            get => _onGrab;
            set => SetProperty(ref _onGrab, value);
        }

        private bool _offGrab;
        public bool OffGrab
        {
            get => _offGrab;
            set => SetProperty(ref _offGrab, value);
        }

        private bool _onRecord;
        public bool OnRecord
        {
            get => _onRecord;
            set => SetProperty(ref _onRecord, value);
        }

        private bool _offRecord;
        public bool OffRecord
        {
            get => _offRecord;
            set => SetProperty(ref _offRecord, value);
        }


        private bool _onLed;
        public bool OnLed
        {
            get => _onLed;
            set => SetProperty(ref _onLed, value);
        }

        private bool _on660;
        public bool On660
        {
            get => _on660;
            set => SetProperty(ref _on660, value);
        }

        private bool _on760;
        public bool On760
        {
            get => _on760;
            set => SetProperty(ref _on760, value);
        }
        
        public DelegateCommand GrabCommand { get; }
        public DelegateCommand StopCommand { get; }

        public DelegateCommand RecordStartCommand { get; }
        public DelegateCommand RecordStopCommand { get; }

        public DelegateCommand OnLedCommand { get; }
        public DelegateCommand OffLedCommand { get; }
        public DelegateCommand SetLedCommand { get; }

        public DelegateCommand On660Command { get; }
        public DelegateCommand Off660Command { get; }
        public DelegateCommand Set660Command { get; }

        public DelegateCommand On760Command { get; }
        public DelegateCommand Off760Command { get; }
        public DelegateCommand Set760Command { get; }

        private bool _on660Auto;
        public bool On660Auto
        {
            get => _on660Auto;
            set
            {
                SetProperty(ref _on660Auto, value);
                SystemConfig.AutoDictionary[ELazer.L660] = value;
            }
        }

        private bool _on760Auto;
        public bool On760Auto
        {
            get => _on760Auto;
            set
            {
                SetProperty(ref _on760Auto, value);
                SystemConfig.AutoDictionary[ELazer.L760] = value;
            }
        }

        private bool _on660Manual;
        public bool On660Manual
        {
            get => _on660Manual;
            set
            {
                SetProperty(ref _on660Manual, value);
                SystemConfig.ManualDictionary[ELazer.L660] = value;
            }
        }

        private bool _on760Manual;
        public bool On760Manual
        {
            get => _on760Manual;
            set
            {
                SetProperty(ref _on760Manual, value);
                SystemConfig.ManualDictionary[ELazer.L760] = value;
            }
        }

        public DelegateCommand SaveCommand { get; }

        public SystemConfig SystemConfig { get; }
        
        private ProcessService _processService;

        public ControlPanelViewModel(
            GrabService grabService,
            ProcessService processService,
            ProtocolService protocolService, 
            RecordService recordService, 
            SystemConfig systemConfig)
        {
            _processService = processService;

            SystemConfig = systemConfig;

            _on660Auto = SystemConfig.AutoDictionary[ELazer.L660];
            _on760Auto = SystemConfig.AutoDictionary[ELazer.L760];

            _offGrab = true;
            _offRecord = true;

            SaveCommand = new DelegateCommand(() =>
            {
                systemConfig.Save(Environment.CurrentDirectory);
            });

            GrabCommand = new DelegateCommand(() =>
            {
                grabService.Start();
                OnGrab = true;
                OffGrab = false;
            });

            StopCommand = new DelegateCommand(() =>
            {
                grabService.Stop();
                OnGrab = false;
                OffGrab = true;
            });

            RecordStartCommand = new DelegateCommand(() =>
            {
                recordService.Start();
                OnRecord = true;
                OffRecord = false;
            });

            RecordStopCommand = new DelegateCommand(() =>
            {
                recordService.Stop();
                OnRecord = false;
                OffRecord = true;

                var dialog = new SaveFileDialog()
                {
                    Filter = "AVI (*.avi)|*.avi"
                };

                if (dialog.ShowDialog(App.Current.MainWindow) == true)
                {
                    recordService.CopyTo(dialog.FileName);
                }
            });

            OnLedCommand = new DelegateCommand(() =>
            {
                protocolService.OnLed();
                OnLed = true;
            });

            OffLedCommand = new DelegateCommand(() =>
            {
                protocolService.OffLed();
                OnLed = false;
            });

            On660Command = new DelegateCommand(() =>
            {
                protocolService.On660();
                On660 = true;
            });

            Off660Command = new DelegateCommand(() =>
            {
                protocolService.Off660();
                On660 = false;
            });

            On760Command = new DelegateCommand(() =>
            {
                protocolService.On760();
                On760 = true;
            });

            Off760Command = new DelegateCommand(() =>
            {
                protocolService.Off760();
                On760 = false;
            });

            SetLedCommand = new DelegateCommand(() =>
            {
                protocolService.SetLed(SystemConfig.ValueLed * 1000);
            });

            Set660Command = new DelegateCommand(() =>
            {
                protocolService.Set660(SystemConfig.ValueDictionary[ELazer.L660] * 1000);
            });

            Set660Command = new DelegateCommand(() =>
            {
                protocolService.Set760(SystemConfig.ValueDictionary[ELazer.L760] * 1000);
            });
        }
    }
}
