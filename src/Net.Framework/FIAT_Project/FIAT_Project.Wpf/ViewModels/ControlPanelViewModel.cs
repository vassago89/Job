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

        private int _lightValue;
        public int LightValue
        {
            get => _lightValue;
            set => SetProperty(ref _lightValue, value);
        }

        public DelegateCommand GrabCommand { get; }
        public DelegateCommand StopCommand { get; }

        public DelegateCommand RecordStartCommand { get; }
        public DelegateCommand RecordStopCommand { get; }

        public DelegateCommand OnLedCommand { get; }
        public DelegateCommand OffLedCommand { get; }

        public DelegateCommand On660Command { get; }
        public DelegateCommand Off660Command { get; }

        public DelegateCommand On760Command { get; }
        public DelegateCommand Off760Command { get; }

        public ControlPanelViewModel(GrabService grabService, ProtocolService protocolService, RecordService recordService)
        {
            GrabCommand = new DelegateCommand(() =>
            {
                grabService.Start();
                OnGrab = true;
            });

            StopCommand = new DelegateCommand(() =>
            {
                grabService.Stop();
                OnGrab = false;

                recordService.Stop();
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

            RecordStartCommand = new DelegateCommand(() =>
            {
                recordService.Start();

            });
        }
    }
}
