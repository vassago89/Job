using FIAT_Project.Core;
using FIAT_Project.Core.Enums;
using FIAT_Project.Core.Service;
using Net.Framework.Algorithm.Enums;
using Net.Framework.Data.ImageDatas;
using Net.Framework.Device.Matrox;
using Net.Framework.Helper.Patterns;
using Net.Framework.Helper.Wpf.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Point = System.Windows.Point;

namespace FIAT_Project.Wpf.ViewModels
{
    public class ImageControlViewModel : BindableBase
    {
        private bool _isPolygon;
        public bool IsPolygon
        {
            get => _isPolygon;
            set
            {
                SetProperty(ref _isPolygon, value);

                if (_isPolygon)
                {
                    SystemConfig.ROIShapeDictionary[Lazer] = EShape.Polygon;
                    SystemConfig.OnROIChangedDictionary[Lazer] = true;
                }
            }
        }

        private bool _isRectangle;
        public bool IsRectangle
        {
            get => _isRectangle;
            set
            {
                SetProperty(ref _isRectangle, value);
                if (_isRectangle)
                {
                    SystemConfig.ROIShapeDictionary[Lazer] = EShape.Rectangle;
                    SystemConfig.OnROIChangedDictionary[Lazer] = true;
                }
            }
        }

        private bool _isEllipse;
        public bool IsEllipse
        {
            get => _isEllipse;
            set
            {
                SetProperty(ref _isEllipse, value);

                if (_isEllipse)
                {
                    SystemConfig.ROIShapeDictionary[Lazer] = EShape.Ellipse;
                    SystemConfig.OnROIChangedDictionary[Lazer] = true;
                }
            }
        }

        public int ImageIndex { get; set; }

        private ELazer lazer;
        public ELazer Lazer
        {
            get => lazer;
            set
            {
                lazer = value;
                switch (SystemConfig.ROIShapeDictionary[Lazer])
                {
                    case EShape.Rectangle:
                        IsRectangle = true;
                        break;
                    case EShape.Ellipse:
                        IsEllipse = true;
                        break;
                    case EShape.Polygon:
                        IsPolygon = true;
                        break;
                }

                OnROI = SystemConfig.OnROIDictionary[Lazer];

                RectangleROI = SystemConfig.ROIRectangleDictionary[Lazer];
                EllipseROI = SystemConfig.ROIEllipseDictionary[Lazer];
                PolygonROI = new PointCollection(SystemConfig.ROIPointDictionary[Lazer].ToList().ConvertAll(p => new Point(p.X, p.Y)));
            }
        }

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
            get => _onROI;
            set
            {
                SystemConfig.OnROIDictionary[Lazer] = value;
                SetProperty(ref _onROI, value);                
            }
        }

        private Rectangle _rectangleROI;
        public Rectangle RectangleROI
        {
            get => _rectangleROI;
            set
            {
                SetProperty(ref _rectangleROI, value);
                SystemConfig.ROIRectangleDictionary[Lazer] = _rectangleROI;
            }
        }

        private Rectangle _ellipseROI;
        public Rectangle EllipseROI
        {
            get => _ellipseROI;
            set
            {
                SetProperty(ref _ellipseROI, value);
                SystemConfig.ROIEllipseDictionary[Lazer] = _ellipseROI;
            }
        }

        private PointCollection _polygonROI;
        public PointCollection PolygonROI
        {
            get => _polygonROI;
            set
            {
                SetProperty(ref _polygonROI, value);
                SystemConfig.ROIPointDictionary[Lazer] = _polygonROI.ToList().ConvertAll(p => new System.Drawing.Point((int)p.X, (int)p.Y)).ToArray();
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

        private PointCollection _setPolygonROI;
        public PointCollection SetPolygonROI
        {
            get => _setPolygonROI;
            set => SetProperty(ref _setPolygonROI, value);
        }

        private PointCollection _tempPolygonROI;

        public SystemConfig SystemConfig { get; }

        public ImageControlViewModel(ProcessService processService, SystemConfig systemConfig)
        {
            try
            {
                SetPolygonROI = new PointCollection();
                _tempPolygonROI = new PointCollection();

                SystemConfig = systemConfig;
                ZoomService = new ZoomService();

                processService.Processed += Processed;

                _pipeLine = new PipeLine<(int, int, byte[][])>(true);
                _pipeLine.Run(new CancellationToken());

                _pipeLine.Job = new Action<(int, int, byte[][])>(tuple =>
                {
                    var width = tuple.Item1;
                    var height = tuple.Item2;
                    var datas = tuple.Item3;

                    if (datas.Length < ImageIndex)
                        return;

                    //var size = width * height;

                    //var total = size * 3;

                    //var sourceData = new byte[total];

                    //for (int i = 0, j = 0; i < total; i += 3, j++)
                    //{
                    //    sourceData[i] = datas[ImageIndex][j];
                    //    sourceData[i + 1] = datas[ImageIndex][j + size];
                    //    sourceData[i + 2] = datas[ImageIndex][j + size + size];
                    //}

                    //var temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null, sourceData, width * 3);
                    //temp.Freeze();
                    //Source = temp;
                });

                ZoomFitCommand = new DelegateCommand(ZoomFit);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show(e.StackTrace);
            }
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
            if (OnROI == false)
                return;

            var curPos = e.GetPosition(sender as IInputElement);
                
            switch (SystemConfig.ROIShapeDictionary[Lazer])
            {
                case EShape.Rectangle:
                case EShape.Ellipse:
                    SetRectROI = Rectangle.Empty;
                    _setROIStartPos = curPos;
                    break;
                case EShape.Polygon:
                    //if (Keyboard.GetKeyStates(Key.LeftShift) == KeyStates.Down)
                    //    SetPolygonROI = new PointCollection();
                    break;
            }

            IsSetROI = true;
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

        public void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OnROI == false)
                return;

            switch (SystemConfig.ROIShapeDictionary[Lazer])
            {
                case EShape.Rectangle:
                    IsSetROI = false;
                    RectangleROI = SetRectROI;
                    break;
                case EShape.Ellipse:
                    IsSetROI = false;
                    EllipseROI = SetRectROI;
                    break;
                case EShape.Polygon:
                    var a = Keyboard.GetKeyStates(Key.LeftShift);
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        var curPos = e.GetPosition(sender as IInputElement);
                        _tempPolygonROI.Add(curPos);
                    }
                    else
                    {
                        if (SetPolygonROI.Count > 2)
                            PolygonROI = SetPolygonROI;

                        IsSetROI = false;
                        _tempPolygonROI = new PointCollection();
                        SetPolygonROI = new PointCollection();
                    }
                    break;
            }

            if (IsSetROI == false)
                SystemConfig.OnROIChangedDictionary[Lazer] = true;
        }

        public void OnMouseLeave()
        {
            _isPanning = false;
            IsSetROI = false;

            SetRectROI = Rectangle.Empty;
            _tempPolygonROI = new PointCollection();
            SetPolygonROI = new PointCollection();
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            var curPos = e.GetPosition(sender as IInputElement);

            if (_isPanning)
            {
                ZoomService.TranslateX += curPos.X - _panningStartPos.X;
                ZoomService.TranslateY += curPos.Y - _panningStartPos.Y;
            }
            else if (_isSetROI)
            {
                switch (SystemConfig.ROIShapeDictionary[Lazer])
                {
                    case EShape.Rectangle:
                    case EShape.Ellipse:
                        SetRectROI = new Rectangle(
                            (int)Math.Min(_setROIStartPos.X, curPos.X),
                            (int)Math.Min(_setROIStartPos.Y, curPos.Y),
                            (int)Math.Abs(_setROIStartPos.X - curPos.X),
                            (int)Math.Abs(_setROIStartPos.Y - curPos.Y));
                        break;
                    case EShape.Polygon:
                        var collection = new PointCollection();
                        foreach (var point in _tempPolygonROI)
                            collection.Add(point);

                        collection.Add(curPos);

                        SetPolygonROI = collection;
                        break;
                }
               
            }
        }
    }
}