﻿using FIAT_Project.Core.Service;
using Net.Framework.Data.ImageDatas;
using Net.Framework.Device.Matrox;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FIAT_Project.Wpf.ViewModels
{
    public class ImageControlViewModel : BindableBase
    {
        private ImageSource[] _channels;
        public ImageSource Channel1
        {
            get => _channels[0];
            set => SetProperty(ref _channels[0], value);
        }
        
        public ImageSource Channel2
        {
            get => _channels[1];
            set => SetProperty(ref _channels[1], value);
        }

        
        public ImageSource Channel3
        {
            get => _channels[2];
            set => SetProperty(ref _channels[2], value);
        }

        public ImageControlViewModel(GrabService grabService)
        {
            _channels = new ImageSource[3];

            grabService.Grabbed += GrabService_Grabbed;
        }

        private void GrabService_Grabbed(int arg1, int arg2, byte[][] arg3)
        {
            //Channel1 = obj[2];
            //Channel2 = obj[1];
            //Channel3 = obj[0];
        }
    }
}