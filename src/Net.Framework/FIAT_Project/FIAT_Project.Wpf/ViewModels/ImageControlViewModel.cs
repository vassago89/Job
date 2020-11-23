using FIAT_Project.Core.Service;
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

            grabService.ServiceGrabbed += ServiceGrabbed;
        }

        private void ServiceGrabbed(int width, int height, byte[][] datas)
        {
            var temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null, datas[0], width);
            temp.Freeze();
            Channel1 = temp;

            temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null, datas[1], width);
            temp.Freeze();
            Channel2 = temp;


            temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null, datas[2], width);
            temp.Freeze();
            Channel3 = temp;
            //Channel1 = obj[2];
            //Channel2 = obj[1];
            //Channel3 = obj[0];
        }
    }
}