using FIAT_Project.Core;
using FIAT_Project.Core.Service;
using Net.Framework.Data.ImageDatas;
using Net.Framework.Device.Matrox;
using Net.Framework.Helper.Patterns;
using Net.Framework.Helper.Wpf.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Point = System.Windows.Point;

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
        
        public DelegateCommand ZoomFitCommand { get; }

        private PipeLine<(int, int, byte[][])> _pipeLine;

        public ZoomService OriginalZoomService { get; }
        public ZoomService MergedZoomService { get; }

        FrameworkElement _originalPresentor;
        FrameworkElement _mergedPresentor;

        private double _scale;
        public double Scale
        {
            get => _scale;
            set => SetProperty(ref _scale, value);
        }

        private int _width;
        private int _height;
        
        bool _isPanning;

        Point _panningTranslatePos;
        Point _panningStartPos;

        Point _setROIStartPos;

        bool _isSetROI;
        public bool IsSetROI
        {
            get => _isSetROI;
            set => SetProperty(ref _isSetROI, value);
        }

        bool _onROI;
        public bool OnROI
        {
            get => _onROI;
            set
            {
                SetProperty(ref _onROI, value);
                SystemConfig.OnROI = value;
            }
        }

        private Rectangle _setRectROI;
        public Rectangle SetRectROI
        {
            get => _setRectROI;
            set => SetProperty(ref _setRectROI, value);
        }

        private Rectangle _rectROI;
        public Rectangle RectROI
        {
            get => _rectROI;
            set
            {
                SetProperty(ref _rectROI, value);
                SystemConfig.RectROI = value;
            }
        }

        public SystemConfig SystemConfig { get; }

        public ImageControlViewModel(ProcessService processService, SystemConfig systemConfig)
        {
            SystemConfig = systemConfig;

            _onROI = systemConfig.OnROI;
            _rectROI = systemConfig.RectROI;

            OriginalZoomService = new ZoomService();
            MergedZoomService = new ZoomService();

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

            ZoomFitCommand = new DelegateCommand(ZoomFit);
        }

        private void Processed(int width, int height, byte[][] datas)
        {
            if (Merged == null)
            {
                _width = width;
                _height = height;

                App.Current.Dispatcher.Invoke(() =>
                {
                    ZoomFit();
                });
            }

            _pipeLine.Enqueue((width, height, datas));
        }

        private void ZoomFit()
        {
            MergedZoomService.ZoomFit(_mergedPresentor.ActualWidth, _mergedPresentor.ActualHeight, _width, _height);
            OriginalZoomService.ZoomFit(_originalPresentor.ActualWidth, _originalPresentor.ActualHeight, _width, _height);
        }

        public void OnOriginalLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _originalPresentor = sender as FrameworkElement;
            if (_originalPresentor != null && _mergedPresentor != null)
                Scale = Math.Min(_originalPresentor.ActualWidth / _mergedPresentor.ActualWidth, _originalPresentor.ActualHeight / _mergedPresentor.ActualHeight);
        }

        public void OnMergedLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _mergedPresentor = sender as FrameworkElement;
            if (_originalPresentor != null && _mergedPresentor != null)
                Scale = Math.Min(_originalPresentor.ActualWidth / _mergedPresentor.ActualWidth, _originalPresentor.ActualHeight / _mergedPresentor.ActualHeight);
        }

        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var pos = e.GetPosition(sender as IInputElement);
            if (e.Delta > 0)
                MergedZoomService.ExecuteZoom(pos.X, pos.Y, 1.1f);
            else
                MergedZoomService.ExecuteZoom(pos.X, pos.Y, 0.9f);
        }
        

        public void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (OnROI)
            {
                var curPos = e.GetPosition(sender as IInputElement);
                IsSetROI = true;

                _setROIStartPos = curPos;
            }
        }

        public void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var curPos = e.GetPosition(sender as IInputElement);
            _isPanning = true;

            _panningTranslatePos = new Point(MergedZoomService.TranslateX, MergedZoomService.TranslateY);
            _panningStartPos = curPos;
        }

        public void OnMouseRightButtonUp()
        {
            _isPanning = false;
        }

        public void OnMouseLeftButtonUp()
        {
            IsSetROI = false;

            if (OnROI)
                RectROI = SetRectROI;
        }

        public void OnMouseLeave()
        {
            _isPanning = false;
            IsSetROI = false;
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            var curPos = e.GetPosition(sender as IInputElement);

            if (_isPanning)
            {
                MergedZoomService.TranslateX = _panningTranslatePos.X + curPos.X - _panningStartPos.X;
                MergedZoomService.TranslateY = _panningTranslatePos.Y + curPos.Y - _panningStartPos.Y;
            }

            if (_isSetROI)
            {
                SetRectROI = new Rectangle(
                    (int)Math.Min(_setROIStartPos.X, curPos.X),
                    (int)Math.Min(_setROIStartPos.Y, curPos.Y),
                    (int)Math.Abs(_setROIStartPos.X - curPos.X),
                    (int)Math.Abs(_setROIStartPos.Y - curPos.Y));
            }
        }
    }
}