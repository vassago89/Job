using FIAT_Project.Core;
using FIAT_Project.Core.Enums;
using FIAT_Project.Core.Service;
using Microsoft.Win32;
using Net.Framework.Device.Matrox;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace FIAT_Project.Wpf.ViewModels
{
    public class ControlPanelViewModel : BindableBase
    {
        private Color _color660;
        public Color Color660
        {
            get => _color660;
            set
            {
                SetProperty(ref _color660, value);
                SystemConfig.ColorDictionary[ELazer.L660][0] = _color660.R;
                SystemConfig.ColorDictionary[ELazer.L660][1] = _color660.G;
                SystemConfig.ColorDictionary[ELazer.L660][2] = _color660.B;
            }
        }

        private Color _color760;
        public Color Color760
        {
            get => _color760;
            set
            {
                SetProperty(ref _color760, value);
                SystemConfig.ColorDictionary[ELazer.L760][0] = _color760.R;
                SystemConfig.ColorDictionary[ELazer.L760][1] = _color760.G;
                SystemConfig.ColorDictionary[ELazer.L760][2] = _color760.B;
            }
        }

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
        
        public double ValueLed
        {
            get => SystemConfig.ValueLed;
            set
            {
                if (value < 0 || value > SystemConfig.MaxLed)
                    return;

                SystemConfig.ValueLed = value;
            }
        }

        public double Value660
        {
            get => SystemConfig.ValueDictionary[ELazer.L660];
            set
            {
                if (value < 0 || value > SystemConfig.MaxValueDictionary[ELazer.L660])
                    return;

                SystemConfig.ValueDictionary[ELazer.L660] = value;
            }
        }

        public double Value760
        {
            get => SystemConfig.ValueDictionary[ELazer.L760];
            set
            {
                if (value < 0 || value > SystemConfig.MaxValueDictionary[ELazer.L760])
                    return;

                SystemConfig.ValueDictionary[ELazer.L760] = value;
            }
        }


        public DelegateCommand GrabCommand { get; }
        public DelegateCommand StopCommand { get; }
        
        public DelegateCommand RecordStartCommand { get; }
        public DelegateCommand RecordStopCommand { get; }

        public DelegateCommand CaptureCommand { get; }

        public DelegateCommand OnLedCommand { get; }
        public DelegateCommand OffLedCommand { get; }
        public DelegateCommand SetLedCommand { get; }

        public DelegateCommand On660Command { get; }
        public DelegateCommand Off660Command { get; }
        public DelegateCommand Set660Command { get; }

        public DelegateCommand On760Command { get; }
        public DelegateCommand Off760Command { get; }
        public DelegateCommand Set760Command { get; }

        public DelegateCommand Set660ManualCommand { get; }
        public DelegateCommand Set760ManualCommand { get; }

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

        private byte _threshold660;
        public byte Threshold660
        {
            get => _threshold660;
            set => SetProperty(ref _threshold660, value);
        }

        private byte _threshold760;
        public byte Threshold760
        {
            get => _threshold760;
            set => SetProperty(ref _threshold760, value);
        }

        private float _coefficientRed;
        public float CoefficientRed
        {
            get => _coefficientRed;
            set
            {
                if (value < 0 || value > 5.5)
                    return;

                SetProperty(ref _coefficientRed, value);
                SystemConfig.CoefficientValues[0] = value;
            }
        }

        private float _coefficientGreen;
        public float CoefficientGreen
        {
            get => _coefficientGreen;
            set
            {
                if (value < 0 || value > 5.5)
                    return;

                SetProperty(ref _coefficientGreen, value);
                SystemConfig.CoefficientValues[1] = value;
            }
        }

        private float _coefficientBlue;
        public float CoefficientBlue
        {
            get => _coefficientBlue;
            set
            {
                if (value < 0 || value > 5.5)
                    return;

                SetProperty(ref _coefficientBlue, value);
                SystemConfig.CoefficientValues[2] = value;
            }
        }

        private bool _autoWhiteBalance;
        public bool AutoWhiteBalance
        {
            get => _autoWhiteBalance;
            set
            {
                SetProperty(ref _autoWhiteBalance, value);
                SystemConfig.OnAutoBayer = value;
            }
        }

        private int _exposureLed;
        public int ExposureLed
        {
            get => _exposureLed;
            set
            {
                SetProperty(ref _exposureLed, value);
                _protocolService.SetExposure(value, ELazer.L660, true);
                SystemConfig.ExposureLed = value;
            }
        }


        private int _exposure660;
        public int Exposure660
        {
            get => _exposure660;
            set
            {
                SetProperty(ref _exposure660, value);
                _protocolService.SetExposure(value, ELazer.L660);
                SystemConfig.ExposureDictionary[ELazer.L660] = value;
            }
        }

        private int _exposure760;
        public int Exposure760
        {
            get => _exposure760;
            set
            {
                SetProperty(ref _exposure760, value);
                _protocolService.SetExposure(value, ELazer.L760);
                SystemConfig.ExposureDictionary[ELazer.L760] = value;
            }
        }

        private int _gainLed;
        public int GainLed
        {
            get => _gainLed;
            set
            {
                SetProperty(ref _gainLed, value);
                _protocolService.SetGain(value, ELazer.L660, true);
                SystemConfig.GainLed = value;
            }
        }


        private int _gain660;
        public int Gain660
        {
            get => _gain660;
            set
            {
                SetProperty(ref _gain660, value);
                _protocolService.SetGain(value, ELazer.L660);
                SystemConfig.GainDictionary[ELazer.L660] = value;
            }
        }

        private int _gain760;
        public int Gain760
        {
            get => _gain760;
            set
            {
                SetProperty(ref _gain760, value);
                _protocolService.SetGain(value, ELazer.L760);
                SystemConfig.GainDictionary[ELazer.L760] = value;
            }
        }

        public DelegateCommand SaveCommand { get; }

        public SystemConfig SystemConfig { get; }
        
        private ProcessService _processService;
        private ProtocolService _protocolService;

        private CaptureService _captureService;

        public ControlPanelViewModel(
            GrabService grabService,
            ProcessService processService,
            ProtocolService protocolService, 
            RecordService recordService,
            CaptureService captureService,
            SystemConfig systemConfig)
        {
            try
            {
                _captureService = captureService;
                _processService = processService;
                _protocolService = protocolService;

                SystemConfig = systemConfig;

                _color660 = Color.FromRgb(SystemConfig.ColorDictionary[ELazer.L660][0], SystemConfig.ColorDictionary[ELazer.L660][1], SystemConfig.ColorDictionary[ELazer.L660][2]);
                _color760 = Color.FromRgb(SystemConfig.ColorDictionary[ELazer.L760][0], SystemConfig.ColorDictionary[ELazer.L760][1], SystemConfig.ColorDictionary[ELazer.L760][2]);

                ValueLed = SystemConfig.ValueLed;
                Value660 = SystemConfig.ValueDictionary[ELazer.L660];
                Value760 = SystemConfig.ValueDictionary[ELazer.L760];

                Threshold660 = SystemConfig.ThresholdDictionary[ELazer.L660];
                Threshold760 = SystemConfig.ThresholdDictionary[ELazer.L760];

                ExposureLed = systemConfig.ExposureLed;
                Exposure660 = systemConfig.ExposureDictionary[ELazer.L660];
                Exposure760 = systemConfig.ExposureDictionary[ELazer.L760];

                GainLed = systemConfig.GainLed;
                Gain660 = systemConfig.GainDictionary[ELazer.L660];
                Gain760 = systemConfig.GainDictionary[ELazer.L760];

                CoefficientRed = SystemConfig.CoefficientValues[0];
                CoefficientGreen = SystemConfig.CoefficientValues[1];
                CoefficientBlue = SystemConfig.CoefficientValues[2];

                AutoWhiteBalance = SystemConfig.OnAutoBayer;

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

                Set660ManualCommand = new DelegateCommand(() =>
                {
                    SetManual(ELazer.L660);
                });

                Set760ManualCommand = new DelegateCommand(() =>
                {
                    SetManual(ELazer.L760);
                });

                CaptureCommand = new DelegateCommand(() =>
                {
                    _captureService.Start(SystemConfig.CaptureCount);
                });

                processService.Processed += Processed;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show(e.StackTrace);
            }
        }

        private void SetManual(ELazer lazer)
        {
            switch (lazer)
            {
                case ELazer.L660:
                    SystemConfig.ThresholdDictionary[ELazer.L660] = Threshold660;
                    break;
                case ELazer.L760:
                    SystemConfig.ThresholdDictionary[ELazer.L760] = Threshold760;
                    break;
            }
        }

        private void Processed(int width, int height, byte[][] datas)
        {
            if (SystemConfig.OnAutoBayer)
            {
                CoefficientRed = SystemConfig.CoefficientValues[0];
                CoefficientGreen = SystemConfig.CoefficientValues[1];
                CoefficientBlue = SystemConfig.CoefficientValues[2];
            }
        }

        public void On660KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Return)
                return;

            var tb = sender as System.Windows.Controls.TextBox;
            if (byte.TryParse(tb.Text, out byte value))
            {
                Threshold660 = value;
                SetManual(ELazer.L660);
            }
        }

        public void On760KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Return)
                return;

            var tb = sender as System.Windows.Controls.TextBox;
            if (byte.TryParse(tb.Text, out byte value))
            {
                Threshold760 = value;
                SetManual(ELazer.L760);
            }
        }
    }
}
