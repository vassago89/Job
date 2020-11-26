using FIAT_Project.Core.Service;
using Net.Framework.Algorithm.Enums;
using Net.Framework.Helper.Patterns;
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
        public bool OnBayer { get; set; }

        public bool OnROI { get; set; }
        public Rectangle RectROI { get; set; }

        public string ProtocolPort { get; set; }
        public string DcfPath { get; set; }

        public double MaxLed { get; set; }
        
        public Dictionary<ELazer, double> ValueDictionary { get; set; }
        public Dictionary<ELazer, double> MaxValueDictionary { get; set; }

        public Dictionary<ELazer, bool> UseDictionary { get; set; }

        public Dictionary<ELazer, bool> AutoDictionary { get; set; }
        public Dictionary<ELazer, EThresholdMethod> MethodDictionary { get; set; }

        public Dictionary<ELazer, bool> ManualDictionary { get; set; }
        public Dictionary<ELazer, byte> ThresholdDictionary { get; set; }

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

            OnBayer = true;

            ValueLed = MaxLed = 1.000;
            
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
        }
    }
}
