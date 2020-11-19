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


        private bool _onLight;
        public bool OnLight
        {
            get => _onLight;
            set => SetProperty(ref _onLight, value);
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

        public DelegateCommand LightOnCommand { get; }
        public DelegateCommand LightOffCommand { get; }

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

            LightOnCommand = new DelegateCommand(() =>
            {
                protocolService.LightOn(LightValue * 10);
                OnLight = true;
            });

            LightOffCommand = new DelegateCommand(() =>
            {
                protocolService.LightOff();
                OnLight = false;
            });

            RecordStartCommand = new DelegateCommand(() =>
            {
                recordService.Start();

            });
        }
    }
}
