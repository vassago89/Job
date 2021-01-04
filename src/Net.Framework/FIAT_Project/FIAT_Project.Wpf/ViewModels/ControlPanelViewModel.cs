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

        private double _valueLed;
        public double ValueLed
        {
            get => _valueLed;
            set
            {
                if (value < 0 || value > SystemConfig.MaxLed)
                    return;

                SetProperty(ref _valueLed, value);
                SystemConfig.ValueLed = value;
            }
        }

        private double _value660;
        public double Value660
        {
            get => _value660;
            set
            {
                if (value < 0 || value > SystemConfig.MaxValueDictionary[ELazer.L660])
                    return;

                SetProperty(ref _value660, value);
                SystemConfig.ValueDictionary[ELazer.L660] = value;
            }
        }

        private double _value760;
        public double Value760
        {
            get => _value760;
            set
            {
                if (value < 0 || value > SystemConfig.MaxValueDictionary[ELazer.L760])
                    return;

                SetProperty(ref _value760, value);
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
                if (value < 1 || value > 1000)
                    return;

                SetProperty(ref _exposureLed, value);
            }
        }
        
        private int _exposure660;
        public int Exposure660
        {
            get => _exposure660;
            set
            {
                if (value < 1 || value > 1000)
                    return;

                SetProperty(ref _exposure660, value);
            }
        }

        private int _exposure760;
        public int Exposure760
        {
            get => _exposure760;
            set
            {
                if (value < 1 || value > 1000)
                    return;

                SetProperty(ref _exposure760, value);
            }
        }

        private int _gainLed;
        public int GainLed
        {
            get => _gainLed;
            set => SetProperty(ref _gainLed, value);
        }


        private int _gain660;
        public int Gain660
        {
            get => _gain660;
            set => SetProperty(ref _gain660, value);
        }

        private int _gain760;
        public int Gain760
        {
            get => _gain760;
            set => SetProperty(ref _gain760, value);
        }

        public DelegateCommand SaveCommand { get; }

        public SystemConfig SystemConfig { get; }
        
        private ProcessService _processService;
        private ProtocolService _protocolService;

        private CaptureService _captureService;

        public DelegateCommand<string> SetCommand { get; }
        public DelegateCommand<int?> PresetLedCommand { get; }
        public DelegateCommand<int?> Preset660Command { get; }
        public DelegateCommand<int?> Preset760Command { get; }
        
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
                    var maxExposure = Math.Max(SystemConfig.ExposureLed, Math.Max(SystemConfig.ExposureDictionary[ELazer.L660], SystemConfig.ExposureDictionary[ELazer.L760]));
                    var frameRate = 1000.0 / maxExposure;
                    frameRate = Math.Min(SystemConfig.RecodingFrame, frameRate);
                    _protocolService.SetFrameRate(SystemConfig.RecodingFrame);

                    recordService.Start(frameRate);
                    OnRecord = true;
                    OffRecord = false;
                });

                RecordStopCommand = new DelegateCommand(() =>
                {
                    recordService.Stop();
                    _protocolService.SetFrameRate(60);
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
                
                SetCommand = new DelegateCommand<string>((str) =>
                {
                    switch (str)
                    {
                        case "ValueLed":
                            protocolService.SetLed(SystemConfig.ValueLed * 1000);
                            break;
                        case "Value660":
                            protocolService.Set660(SystemConfig.ValueDictionary[ELazer.L660] * 1000);
                            break;
                        case "Value760":
                            protocolService.Set760(SystemConfig.ValueDictionary[ELazer.L760] * 1000);
                            break;
                        case "Threshold660":
                            SetManual(ELazer.L660);
                            break;
                        case "Threshold760":
                            SetManual(ELazer.L760);
                            break;
                        case "ExposureLed":
                            SetExposure(ELazer.L660, true);
                            break;
                        case "Exposure660":
                            SetExposure(ELazer.L660);
                            break;
                        case "Exposure760":
                            SetExposure(ELazer.L760);
                            break;
                        case "GainLed":
                            SetGain(ELazer.L660, true);
                            break;
                        case "Gain660":
                            SetGain(ELazer.L660);
                            break;
                        case "Gain760":
                            SetGain(ELazer.L760);
                            break;
                    }
                });

                CaptureCommand = new DelegateCommand(() =>
                {
                    _captureService.Start(SystemConfig.CaptureCount);
                });

                PresetLedCommand = new DelegateCommand<int?>((value) =>
                {
                    ExposureLed = value.Value;
                    SetExposure(ELazer.L660, true);
                });

                Preset660Command = new DelegateCommand<int?>((value) =>
                {
                    Exposure660 = value.Value;
                    SetExposure(ELazer.L660);
                });

                Preset760Command = new DelegateCommand<int?>((value) =>
                {
                    Exposure760 = value.Value;
                    SetExposure(ELazer.L760);
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

        private void SetExposure(ELazer lazer, bool isColor = false)
        {
            if (isColor)
            {
                _protocolService.SetExposure(ExposureLed, ELazer.L660, true);
                SystemConfig.ExposureLed = ExposureLed;
                return;
            }
            
            switch (lazer)
            {
                case ELazer.L660:
                    _protocolService.SetExposure(Exposure660, ELazer.L660);
                    SystemConfig.ExposureDictionary[ELazer.L660] = Exposure660;
                    break;
                case ELazer.L760:
                    _protocolService.SetExposure(Exposure760, ELazer.L760);
                    SystemConfig.ExposureDictionary[ELazer.L760] = Exposure760;
                    break;
            }
        }

        private void SetGain(ELazer lazer, bool isColor = false)
        {
            if (isColor)
            {
                _protocolService.SetGain(GainLed, ELazer.L660, true);
                SystemConfig.GainLed = GainLed;
                return;
            }

            switch (lazer)
            {
                case ELazer.L660:
                    _protocolService.SetGain(Gain660, ELazer.L660);
                    SystemConfig.GainDictionary[ELazer.L660] = Gain660;
                    break;
                case ELazer.L760:
                    _protocolService.SetGain(Gain760, ELazer.L760);
                    SystemConfig.GainDictionary[ELazer.L760] = Gain760;
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

        public void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Return)
                return;

            var tb = sender as System.Windows.Controls.TextBox;
            if (byte.TryParse(tb.Text, out byte value))
            {
                switch (tb.Tag)
                {
                    case "ValueLed":
                        ValueLed = value;
                        _protocolService.SetLed(SystemConfig.ValueLed * 1000);
                        break;
                    case "Value660":
                        Value660 = value;
                        _protocolService.Set660(SystemConfig.ValueDictionary[ELazer.L660] * 1000);
                        break;
                    case "Value760":
                        Value760 = value;
                        _protocolService.Set760(SystemConfig.ValueDictionary[ELazer.L760] * 1000);
                        break;
                    case "Threshold660":
                        Threshold660 = value;
                        SetManual(ELazer.L660);
                        break;
                    case "Threshold760":
                        Threshold760 = value;
                        SetManual(ELazer.L760);
                        break;
                    case "ExposureLed":
                        ExposureLed = value;
                        SetExposure(ELazer.L660, true);
                        break;
                    case "Exposure660":
                        Exposure660 = value;
                        SetExposure(ELazer.L660);
                        break;
                    case "Exposure760":
                        Exposure760 = value;
                        SetExposure(ELazer.L760);
                        break;
                    case "GainLed":
                        GainLed = value;
                        SetGain(ELazer.L660, true);
                        break;
                    case "Gain660":
                        Gain660 = value;
                        SetGain(ELazer.L660);
                        break;
                    case "Gain760":
                        Gain760 = value;
                        SetGain(ELazer.L760);
                        break;
                }
            }
        }
    }
}
