using FIAT_Project.Core;
using FIAT_Project.Core.Enums;
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
        public int ImageIndex { get; set; }
        public ELazer Lazer { get; set; }

        private string _header; 
        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }

        private bool _onLazer;
        public bool OnLazer
        {
            get => _onLazer;
            set => SetProperty(ref _onLazer, value);
        }

        private ImageSource _source;
        public ImageSource Source
        {
            get => _source;
            set => SetProperty(ref _source, value);
        }
        
        public DelegateCommand ZoomFitCommand { get; }

        private PipeLine<(int, int, byte[][])> _pipeLine;

        public ZoomService ZoomService { get; }

        FrameworkElement _presentor;
        
        private int _width;
        private int _height;
        
        bool _isPanning;

        Point _panningTranslatePos;
        Point _panningStartPos;

        Point _setROIStartPos;
        
        bool _onROI;
        public bool OnROI
        {
            get
            {
                _onROI = SystemConfig.OnROIDictionary[Lazer];
                return _onROI;
            }
            set
            {
                SystemConfig.OnROIDictionary[Lazer] = value;
                SetProperty(ref _onROI, value);                
            }
        }

        private Rectangle _rectROI;
        public Rectangle RectROI
        {
            get
            {
                _rectROI = SystemConfig.ROIDictionary[Lazer];
                return _rectROI;
            }
            set
            {
                SystemConfig.ROIDictionary[Lazer] = value;
                SetProperty(ref _rectROI, value);
            }
        }

        bool _isSetROI;
        public bool IsSetROI
        {
            get => _isSetROI;
            set => SetProperty(ref _isSetROI, value);
        }

        private Rectangle _setRectROI;
        public Rectangle SetRectROI
        {
            get => _setRectROI;
            set => SetProperty(ref _setRectROI, value);
        }

        public SystemConfig SystemConfig { get; }

        public ImageControlViewModel(ProcessService processService, SystemConfig systemConfig)
        {
            SystemConfig = systemConfig;
            
            ZoomService = new ZoomService();

            processService.Processed += Processed;

            _pipeLine = new PipeLine<(int, int, byte [][])>(true);
            _pipeLine.Run(new CancellationToken());

            _pipeLine.Job = new Action<(int, int, byte[][])>(tuple =>
            {
                var width = tuple.Item1;
                var height = tuple.Item2;
                var datas = tuple.Item3;

                if (datas.Length < ImageIndex)
                    return;

                var size = width * height;
                var total = size * 3;

                var sourceData = new byte[total];

                for (int i = 0, j = 0; i < total; i += 3, j++)
                {
                    sourceData[i] = datas[ImageIndex][j];
                    sourceData[i + 1] = datas[ImageIndex][j + size];
                    sourceData[i + 2] = datas[ImageIndex][j + size + size];
                }

                var temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null, sourceData, width * 3);
                temp.Freeze();
                Source = temp;
            });

            ZoomFitCommand = new DelegateCommand(ZoomFit);
        }

        private void Processed(int width, int height, byte[][] datas)
        {
            if (Source == null)
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
            ZoomService.ZoomFit(_presentor.ActualWidth, _presentor.ActualHeight, _width, _height);
        }

        public void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _presentor = sender as FrameworkElement;
        }

        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var pos = e.GetPosition(sender as IInputElement);
            if (e.Delta > 0)
                ZoomService.ExecuteZoom(pos.X, pos.Y, 1.1f);
            else
                ZoomService.ExecuteZoom(pos.X, pos.Y, 0.9f);
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

            _panningTranslatePos = new Point(ZoomService.TranslateX, ZoomService.TranslateY);
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
                ZoomService.TranslateX = _panningTranslatePos.X + curPos.X - _panningStartPos.X;
                ZoomService.TranslateY = _panningTranslatePos.Y + curPos.Y - _panningStartPos.Y;
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