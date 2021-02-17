using FIAT_Project.Core.Enums;
using FIAT_Project.Core.Service;
using Net.Framework.Algorithm.Enums;
using Net.Framework.Helper.Patterns;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Core
{
    public class SystemConfig : Writable<SystemConfig>
    {
        [JsonIgnore]
        public Dictionary<ELazer, bool> OnROIChangedDictionary { get; set; }

        public bool OnAutoBayer { get; set; }

        public EProtocolType ProtocolType { get; set; }

        public string LazerProtocolPort { get; set; }
        public string GrabberProtocolPort { get; set; }
        public string DcfPath { get; set; }
        public string CapturePath { get; set; }
        public string RecordPath { get; set; }

        public double MaxLed { get; set; }
        public int CaptureCount { get; set; }
        
        public Dictionary<ELazer, bool> OnROIDictionary { get; set; }
        public Dictionary<ELazer, EShape> ROIShapeDictionary { get; set; }
        public Dictionary<ELazer, Point[]> ROIPointDictionary { get; set; }
        public Dictionary<ELazer, Rectangle> ROIRectangleDictionary { get; set; }
        public Dictionary<ELazer, Rectangle> ROIEllipseDictionary { get; set; }

        public Dictionary<ELazer, double> ValueDictionary { get; set; }
        public Dictionary<ELazer, double> MaxValueDictionary { get; set; }

        public EThresholdMode ThresholdMode { get; set; }
        public Dictionary<ELazer, bool> UseDictionary { get; set; }

        public Dictionary<ELazer, bool> AutoDictionary { get; set; }
        public Dictionary<ELazer, EThresholdMethod> MethodDictionary { get; set; }

        public Dictionary<ELazer, bool> ManualDictionary { get; set; }
        public Dictionary<ELazer, byte> ThresholdDictionary { get; set; }

        public float[] CoefficientValues { get; set; }

        public Dictionary<ELazer, byte[]> ColorDictionary { get; set; }

        public int ExposureLed { get; set; }
        public Dictionary<ELazer, int> ExposureDictionary { get; set; }

        public int GainLed { get; set; }
        public Dictionary<ELazer, int> GainDictionary { get; set; }

        public Dictionary<ELazer, bool> IsAutoScaleDictionary { get; set; }
        public Dictionary<ELazer, bool> IsLogScaleDictionary { get; set; }
        public Dictionary<ELazer, EChannel> ChennelDictionary { get; set; }

        public Dictionary<ESensitivity, int> LedPresetDictionary { get; set; }
        public Dictionary<ELazer, Dictionary<ESensitivity, int>> LazerPresetDictionary { get; set; }

        public double ValueLed { get; set; }
        public int RecodingFrame { get; set; }
        
        public double RatioColor { get; set; }
        public double Ratio660 { get; set; }
        public double Ratio760 { get; set; }

        public ELazerMachineType LazerMachineType { get; set; }

        public SystemConfig()
        {
            LazerProtocolPort = "COM6";
            GrabberProtocolPort = "COM3";
            DcfPath = "MIL10_SOL_BV-C8300NV_re2.dcf";
            CapturePath = "..\\Capture";
            RecordPath = "..\\Record";
            ValueLed = MaxLed = 1.000;
            CaptureCount = 5;

            ProtocolType = EProtocolType.Channel2;

            OnAutoBayer = false;
            CoefficientValues = new float[3];
            for (int i = 0; i < 3; i++)
                CoefficientValues[i] = 1;

            OnROIChangedDictionary = new Dictionary<ELazer, bool>();
            OnROIChangedDictionary[ELazer.L660] = true;
            OnROIChangedDictionary[ELazer.L760] = true;

            OnROIDictionary = new Dictionary<ELazer, bool>();
            OnROIDictionary[ELazer.L660] = false;
            OnROIDictionary[ELazer.L760] = false;

            ROIShapeDictionary = new Dictionary<ELazer, EShape>();
            ROIShapeDictionary[ELazer.L660] = EShape.Rectangle;
            ROIShapeDictionary[ELazer.L760] = EShape.Rectangle;

            ROIRectangleDictionary = new Dictionary<ELazer, Rectangle>();
            ROIRectangleDictionary[ELazer.L660] = Rectangle.Empty;
            ROIRectangleDictionary[ELazer.L760] = Rectangle.Empty;

            ROIEllipseDictionary = new Dictionary<ELazer, Rectangle>();
            ROIEllipseDictionary[ELazer.L660] = Rectangle.Empty;
            ROIEllipseDictionary[ELazer.L760] = Rectangle.Empty;

            ROIPointDictionary = new Dictionary<ELazer, Point[]>();
            ROIPointDictionary[ELazer.L660] = new Point[0];
            ROIPointDictionary[ELazer.L760] = new Point[0];

            ValueDictionary = new Dictionary<ELazer, double>();
            MaxValueDictionary = new Dictionary<ELazer, double>();
            ValueDictionary[ELazer.L660] = MaxValueDictionary[ELazer.L660] = 1.5;
            ValueDictionary[ELazer.L760] = MaxValueDictionary[ELazer.L760] = 2.5;

            ThresholdMode = EThresholdMode.BinaryMode;

            UseDictionary = new Dictionary<ELazer, bool>();
            UseDictionary[ELazer.L660] = true;
            UseDictionary[ELazer.L760] = true;

            AutoDictionary = new Dictionary<ELazer, bool>();
            AutoDictionary[ELazer.L660] = true;
            AutoDictionary[ELazer.L760] = true;

            MethodDictionary = new Dictionary<ELazer, EThresholdMethod>();

            MethodDictionary[ELazer.L660] = EThresholdMethod.Li;
            MethodDictionary[ELazer.L760] = EThresholdMethod.Li;

            ManualDictionary = new Dictionary<ELazer, bool>();
            ManualDictionary[ELazer.L660] = false;
            ManualDictionary[ELazer.L760] = false;

            ThresholdDictionary = new Dictionary<ELazer, byte>();
            ThresholdDictionary[ELazer.L660] = 127;
            ThresholdDictionary[ELazer.L760] = 127;

            ColorDictionary = new Dictionary<ELazer, byte[]>();
            ColorDictionary[ELazer.L660] = new byte[3] { 255, 0, 0 };
            ColorDictionary[ELazer.L760] = new byte[3] { 0, 255, 0 };

            ExposureLed = 1;
            ExposureDictionary = new Dictionary<ELazer, int>();
            ExposureDictionary[ELazer.L660] = 1;
            ExposureDictionary[ELazer.L760] = 1;

            GainLed = 0;
            GainDictionary = new Dictionary<ELazer, int>();
            GainDictionary[ELazer.L660] = 0;
            GainDictionary[ELazer.L760] = 0;

            IsAutoScaleDictionary = new Dictionary<ELazer, bool>();
            IsAutoScaleDictionary[ELazer.L660] = true;
            IsAutoScaleDictionary[ELazer.L760] = true;

            IsLogScaleDictionary = new Dictionary<ELazer, bool>();
            IsLogScaleDictionary[ELazer.L660] = false;
            IsLogScaleDictionary[ELazer.L760] = false;

            LedPresetDictionary = new Dictionary<ESensitivity, int>();
            LedPresetDictionary[ESensitivity.Low] = 5;
            LedPresetDictionary[ESensitivity.Medium] = 10;
            LedPresetDictionary[ESensitivity.High] = 15;

            LazerPresetDictionary = new Dictionary<ELazer, Dictionary<ESensitivity, int>>();
            LazerPresetDictionary[ELazer.L660] = new Dictionary<ESensitivity, int>();
            LazerPresetDictionary[ELazer.L660][ESensitivity.Low] = 5;
            LazerPresetDictionary[ELazer.L660][ESensitivity.Medium] = 10;
            LazerPresetDictionary[ELazer.L660][ESensitivity.High] = 15;

            LazerPresetDictionary[ELazer.L760] = new Dictionary<ESensitivity, int>();
            LazerPresetDictionary[ELazer.L760][ESensitivity.Low] = 5;
            LazerPresetDictionary[ELazer.L760][ESensitivity.Medium] = 10;
            LazerPresetDictionary[ELazer.L760][ESensitivity.High] = 15;

            RecodingFrame = 15;

            RatioColor = 1;
            Ratio660 = 1;
            Ratio760 = 1;

            ChennelDictionary = new Dictionary<ELazer, EChannel>();
            ChennelDictionary[ELazer.L660] = EChannel.Red;
            ChennelDictionary[ELazer.L760] = EChannel.Green;
        }
    }
}
