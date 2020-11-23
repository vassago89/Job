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
        public ImageSource _original;
        public ImageSource Original
        {
            get => _original;
            set => SetProperty(ref _original, value);
        }

        private ImageSource _lazer660;
        public ImageSource Lazer660
        {
            get => _lazer660;
            set => SetProperty(ref _lazer660, value);
        }

        private ImageSource _lazer760;
        public ImageSource Lazer760
        {
            get => _lazer760;
            set => SetProperty(ref _lazer760, value);
        }

        public ImageControlViewModel(GrabService grabService)
        {
            grabService.ServiceGrabbed += ServiceGrabbed;
        }

        private void ServiceGrabbed(int width, int height, byte[][] datas)
        {
            var temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null, datas[0], width);
            temp.Freeze();
            Original = temp;

            temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null, datas[1], width);
            temp.Freeze();
            Lazer660 = temp;


            temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null, datas[2], width);
            temp.Freeze();
            Lazer760 = temp;
        }
    }
}