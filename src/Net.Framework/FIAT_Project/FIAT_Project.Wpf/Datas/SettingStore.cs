using FIAT_Project.Core;
using FIAT_Project.Core.Enums;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Wpf.Datas
{
    public class SettingStore : BindableBase
    {
        private SystemConfig _systemConfig;

        bool _temp;
        public bool Use660
        {
            get => _systemConfig.UseDictionary[ELazer.L660];
            set
            {
                _systemConfig.UseDictionary[ELazer.L660] = value;
                SetProperty(ref _temp, value);
            }
        }

        public bool Use760
        {
            get => _systemConfig.UseDictionary[ELazer.L760];
            set
            {
                _systemConfig.UseDictionary[ELazer.L760] = value;
                SetProperty(ref _temp, value);
            }
        }

        private bool _onGrab;
        public bool OnGrab
        {
            get => _onGrab;
            set => SetProperty(ref _onGrab, value);
        }

        private EProtocolType _protocolType;
        public EProtocolType ProtocolType
        {
            get => _systemConfig.ProtocolType;
            set
            {
                _systemConfig.ProtocolType = value;
                SetProperty(ref _protocolType, value);
                switch (_protocolType)
                {
                    case EProtocolType.Channel2:
                        _systemConfig.UseDictionary[ELazer.L660] = false;
                        _systemConfig.UseDictionary[ELazer.L760] = true;
                        break;
                    case EProtocolType.Channel3:
                        _systemConfig.UseDictionary[ELazer.L660] = true;
                        _systemConfig.UseDictionary[ELazer.L760] = true;
                        break;
                }
            }
        }

        public SettingStore(SystemConfig systemConfig)
        {
            _systemConfig = systemConfig;
        }
    }
}
