using FIAT_Project.Core.Enums;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Wpf.Datas
{
    public class Preset : BindableBase
    {
        private int _low;
        public int Low
        {
            get => _low;
            set
            {
                SetProperty(ref _low, value);
                _presetDicionary[ESensitivity.Low] = value;
            }
        }

        private int _medium;
        public int Medium
        {
            get => _medium;
            set
            {
                SetProperty(ref _medium, value);
                _presetDicionary[ESensitivity.Medium] = value;
            }
        }

        private int _high;
        public int High
        {
            get => _high;
            set
            {
                SetProperty(ref _high, value);
                _presetDicionary[ESensitivity.High] = value;
            }
        }

        IDictionary<ESensitivity, int> _presetDicionary;

        public Preset(Dictionary<ESensitivity, int> presetDicionary)
        {
            _presetDicionary = presetDicionary;
            _low = presetDicionary[ESensitivity.Low];
            _medium = presetDicionary[ESensitivity.Medium];
            _high = presetDicionary[ESensitivity.High];
        }
    }
}
