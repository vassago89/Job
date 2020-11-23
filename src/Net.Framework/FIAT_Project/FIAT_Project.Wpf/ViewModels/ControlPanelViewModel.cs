using FIAT_Project.Core;
using FIAT_Project.Core.Service;
using Net.Framework.Device.Matrox;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
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

        private double _ledValue;
        public double LedValue
        {
            get => _ledValue;
            set
            {
                if (value < 0)
                    value = 0;

                if (value > _systemConfig.LedMax)
                    value = _systemConfig.LedMax;

                SetProperty(ref _ledValue, value);
            }
        }

        private double _lazer660Value;
        public double Lazer660Value
        {
            get => _lazer660Value;
            set
            {
                if (value < 0)
                    value = 0;

                if (value > _systemConfig.Lazer660Max)
                    value = _systemConfig.Lazer660Max;

                SetProperty(ref _lazer660Value, value);
            }
        }

        private double _lazer760Value;
        public double Lazer760Value
        {
            get => _lazer760Value;
            set
            {
                if (value < 0)
                    value = 0;

                if (value > _systemConfig.Lazer760Max)
                    value = _systemConfig.Lazer760Max;

                SetProperty(ref _lazer760Value, value);
            }
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

        SystemConfig _systemConfig;

        public ControlPanelViewModel(GrabService grabService, ProtocolService protocolService, RecordService recordService, SystemConfig systemConfig)
        {
            _systemConfig = systemConfig;

            _ledValue = _systemConfig.LedMax;
            _lazer660Value = _systemConfig.Lazer660Max;
            _lazer760Value = _systemConfig.Lazer760Max;

            _offGrab = true;
            _offRecord = true;

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
                recordService.Start(new string[] { "d:\\1.avi", "d:\\2.avi", "d:\\3.avi" });
                OnRecord = true;
                OffRecord = false;
            });

            RecordStopCommand = new DelegateCommand(() =>
            {
                recordService.Stop();
                OnRecord = false;
                OffRecord = true;
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
                protocolService.SetLed(LedValue * 1000);
            });

            Set660Command = new DelegateCommand(() =>
            {
                protocolService.Set660(Lazer660Value * 1000);
            });

            Set660Command = new DelegateCommand(() =>
            {
                protocolService.Set760(Lazer760Value * 1000);
            });
        }
    }
}
