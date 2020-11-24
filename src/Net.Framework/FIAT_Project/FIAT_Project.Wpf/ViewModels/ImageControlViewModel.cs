using FIAT_Project.Core;
using FIAT_Project.Core.Service;
using Net.Framework.Data.ImageDatas;
using Net.Framework.Device.Matrox;
using Net.Framework.Helper.Patterns;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        private ImageSource _merged;
        public ImageSource Merged
        {
            get => _merged;
            set => SetProperty(ref _merged, value);
        }

        private PipeLine<(int, int, byte[][])> _pipeLine;

        public ImageControlViewModel(ProcessService processService, SystemConfig systemConfig)
        {
            processService.Processed += Processed;

            _pipeLine = new PipeLine<(int, int, byte [][])>(true);
            _pipeLine.Run(new CancellationToken());

            _pipeLine.Job = new Action<(int, int, byte[][])>(tuple =>
            {
                var width = tuple.Item1;
                var height = tuple.Item2;
                var datas = tuple.Item3;

                var size = width * height;
                var total = size * 3;

                var sourceDatas = new byte[4][];
                for (int i = 0; i < 4; i++)
                    sourceDatas[i] = new byte[total];

                for (int k = 0; k < 4; k++)
                {
                    for (int i = 0, j = 0; i < total; i += 3, j++)
                    {
                        sourceDatas[k][i] = datas[k][j];
                        sourceDatas[k][i + 1] = datas[k][j + size];
                        sourceDatas[k][i + 2] = datas[k][j + size + size];
                    }
                }

                var temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null, sourceDatas[0], width * 3);
                temp.Freeze();
                Original = temp;

                temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null, sourceDatas[1], width * 3);
                temp.Freeze();
                Lazer660 = temp;

                temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null, sourceDatas[2], width * 3);
                temp.Freeze();
                Lazer760 = temp;

                temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null, sourceDatas[3], width * 3);
                temp.Freeze();
                Merged = temp;
            });
        }

        private void Processed(int width, int height, byte[][] datas)
        {
            _pipeLine.Enqueue((width, height, datas));
        }
    }
}