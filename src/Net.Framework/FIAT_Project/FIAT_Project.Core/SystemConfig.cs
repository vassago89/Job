﻿using FIAT_Project.Core.Enums;
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
        
        public string ProtocolPort { get; set; }
        public string DcfPath { get; set; }

        public double MaxLed { get; set; }
        public uint CaptureCount { get; set; }
        
        public Dictionary<ELazer, bool> OnROIDictionary { get; set; }
        public Dictionary<ELazer, EShape> ROIShapeDictionary { get; set; }
        public Dictionary<ELazer, Point[]> ROIPointDictionary { get; set; }
        public Dictionary<ELazer, Rectangle> ROIRectangleDictionary { get; set; }
        public Dictionary<ELazer, Rectangle> ROIEllipseDictionary { get; set; }

        public Dictionary<ELazer, double> ValueDictionary { get; set; }
        public Dictionary<ELazer, double> MaxValueDictionary { get; set; }

        public Dictionary<ELazer, bool> UseDictionary { get; set; }

        public Dictionary<ELazer, bool> AutoDictionary { get; set; }
        public Dictionary<ELazer, EThresholdMethod> MethodDictionary { get; set; }

        public Dictionary<ELazer, bool> ManualDictionary { get; set; }
        public Dictionary<ELazer, byte> ThresholdDictionary { get; set; }

        public float[] CoefficientValues { get; set; }

        public Dictionary<ELazer, byte[]> ColorDictionary { get; set; }
        
        private double _valueLed;
        public double ValueLed
        {
            get => _valueLed;
            set
            {
                if (value < 0 || value > MaxLed)
                    return;

                _valueLed = value;
            }
        }

        public double Value660
        {
            get => ValueDictionary[ELazer.L660];
            set
            {
                if (value < 0 || value > MaxValueDictionary[ELazer.L660])
                    return;

                ValueDictionary[ELazer.L660] = value;
            }
        }

        public double Value760
        {
            get => ValueDictionary[ELazer.L760];
            set
            {
                if (value < 0 || value > MaxValueDictionary[ELazer.L760])
                    return;

                ValueDictionary[ELazer.L760] = value;
            }
        }

        public byte Threshold660
        {
            get => ThresholdDictionary[ELazer.L660];
            set
            {
                if (value < 0 || value > 255)
                    return;

                ThresholdDictionary[ELazer.L660] = value;
            }
        }
        
        public byte Threshold760
        {
            get => ThresholdDictionary[ELazer.L760];
            set
            {
                if (value < 0 || value > 255)
                    return;

                ThresholdDictionary[ELazer.L760] = value;
            }
        }

        public SystemConfig()
        {
            ProtocolPort = "COM6";
            DcfPath = "MIL10_SOL_BV-C8300NV_re2.dcf";
            
            ValueLed = MaxLed = 1.000;
            CaptureCount = 5;

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
        }
    }
}
