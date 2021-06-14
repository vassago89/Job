using FIAT_Project.Core;
using FIAT_Project.Core.Enums;
using FIAT_Project.Core.Service;
using FIAT_Project.Wpf.Stores;
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

        public bool IsColor { get; set; }
        public bool IsMerged { get; set; }

        private ELazer lazer;
        public ELazer Lazer
        {
            get => lazer;
            set
            {
                lazer = value;
                switch (SystemConfig.ROIShapeDictionary[lazer])
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

                RectangleROI = SystemConfig.ROIRectangleDictionary[lazer];
                EllipseROI = SystemConfig.ROIEllipseDictionary[lazer];
                PolygonROI = new PointCollection(SystemConfig.ROIPointDictionary[lazer].ToList().ConvertAll(p => new Point(p.X, p.Y)));

                OnROI = SystemConfig.OnROIDictionary[lazer];

                if (OnROI)
                {
                    switch (SystemConfig.ROIShapeDictionary[lazer])
                    {
                        case EShape.Rectangle:
                            RectangleROI = SystemConfig.ROIRectangleDictionary[lazer];
                            break;
                        case EShape.Ellipse:
                            EllipseROI = SystemConfig.ROIEllipseDictionary[lazer];
                            break;
                        case EShape.Polygon:
                            PolygonROI = new PointCollection(SystemConfig.ROIPointDictionary[lazer].ToList().ConvertAll(p => new Point(p.X, p.Y)));
                            break;
                    }
                }
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

        public DelegateCommand ZoomInCommand { get; }
        public DelegateCommand ZoomOutCommand { get; }
        public DelegateCommand ZoomFitCommand { get; }
        
        public ZoomService ZoomService { get; }

        FrameworkElement _presentor;
        
        private int _width;
        private int _height;
        
        bool _isPanning;
        
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

                if (_onROI == false)
                    _roiStore.SetROI(Lazer, Rectangle.Empty);
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
                _roiStore.SetROI(Lazer, _rectangleROI);
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
                _roiStore.SetROI(Lazer, _ellipseROI);
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

                if (_polygonROI.Count == 0)
                    return;

                var xOrdered = _polygonROI.OrderBy(p => p.X);
                var yOrdered = _polygonROI.OrderBy(p => p.Y);
                var rect = new Rectangle(
                    (int)Math.Round(xOrdered.First().X),
                    (int)Math.Round(yOrdered.First().Y),
                    (int)Math.Round(xOrdered.Last().X - xOrdered.First().X),
                    (int)Math.Round(yOrdered.Last().Y - yOrdered.First().Y));


                _roiStore.SetROI(Lazer, rect);
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

        private byte[] _buffer;
        private PipeLine<(int width, int height, byte[] data)> _pipeLine;

        private ROIStore _roiStore;

        public ImageControlViewModel(ProcessService processService, ROIStore roiStore, SystemConfig systemConfig)
        {
            try
            {
                SetPolygonROI = new PointCollection();
                _tempPolygonROI = new PointCollection();

                _roiStore = roiStore;
                SystemConfig = systemConfig;
                ZoomService = new ZoomService();

                processService.Processed += Processed;

                ZoomInCommand = new DelegateCommand(() =>
                {
                    ZoomService.ZoomIn(_presentor.ActualWidth, _presentor.ActualHeight);
                });

                ZoomOutCommand = new DelegateCommand(() =>
                {
                    ZoomService.ZoomOut(_presentor.ActualWidth, _presentor.ActualHeight);
                });

                ZoomFitCommand = new DelegateCommand(ZoomFit);

                _pipeLine = new PipeLine<(int width, int height, byte[] data)>(true);
                _pipeLine.Job = new Action<(int width, int height, byte[] data)>((tuple) =>
                {
                    DrawImage(tuple.width, tuple.height, tuple.data);
                });

                _pipeLine.Run(new CancellationToken());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show(e.StackTrace);
            }
        }

        private void DrawImage(int width, int height, byte[] data)
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
            
            var size = width * height;
            if (IsColor == false && IsMerged == false)
            {
                var temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null, data, width);
                temp.Freeze();
                Source = temp;
            }
            else
            {
                var total = size * 3;
                if (_buffer == null)
                    _buffer = new byte[total];

                for (int i = 0, red = 0, green = size, blue = size * 2; i < total; red++, green++, blue++)
                {
                    _buffer[i++] = data[red];
                    _buffer[i++] = data[green];
                    _buffer[i++] = data[blue];
                }

                var temp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null, _buffer, width * 3);
                temp.Freeze();
                Source = temp;
            }
        }

        private void Processed(int width, int height, byte[] ledData, byte[] mergedData, Dictionary<ELazer, byte[]> dataDictionary)
        {
            if (IsColor)
            {
                _pipeLine.Enqueue((width, height, ledData));
                return;
            }
                
            if (IsMerged)
            {
                _pipeLine.Enqueue((width, height, mergedData));
                return;
            }
            
            if (dataDictionary.ContainsKey(Lazer))
            {
                _pipeLine.Enqueue((width, height, dataDictionary[Lazer]));
                _roiStore.SetImage(Lazer, Source);
            }
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
            if (OnROI == false || ZoomService.Scale == 0)
                return;

            var curPos = e.GetPosition(_presentor);
                
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
            
            _panningStartPos = new Point(curPos.X, curPos.Y);
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
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        var curPos = e.GetPosition(_presentor);
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
            if (_isPanning)
            {
                var curPos = e.GetPosition(sender as IInputElement);
                ZoomService.TranslateX += curPos.X - _panningStartPos.X;
                ZoomService.TranslateY += curPos.Y - _panningStartPos.Y;

                _panningStartPos.X = curPos.X;
                _panningStartPos.Y = curPos.Y;
            }
            else if (_isSetROI)
            {
                var curPos = e.GetPosition(_presentor);

                if (ZoomService.Scale == 0)
                    return;

                //curPos = new Point(curPos.X / ZoomService.Scale, curPos.Y / ZoomService.Scale);
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