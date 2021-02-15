using FIAT_Project.Core.Service;
using FIAT_Project.Wpf.Datas;
using FIAT_Project.Wpf.Views;
using MaterialDesignThemes.Wpf;
using Net.Framework.Data.ImageDatas;
using Net.Framework.Device.Matrox;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FIAT_Project.Wpf.ViewModels
{
    public class BindableDriveInfo : BindableBase
    {
        public string Name { get; }
        public long TotalSize { get; }

        private long _availableFreeSpace;
        public long AvailableFreeSpace
        {
            get => _availableFreeSpace;
            set => SetProperty(ref _availableFreeSpace, value);
        }

        public BindableDriveInfo(string name, long totalSize)
        {
            Name = name;
            TotalSize = totalSize;
        }
    }

    public class MenuPanelViewModel : BindableBase
    {
        public DelegateCommand WorkListCommand => new DelegateCommand(ShowWorkListDialog);
        public DelegateCommand SettingCommand { get; }
        public DelegateCommand ExitCommand { get; }

        public ObservableCollection<BindableDriveInfo> DriveInfos { get; }

        private double _cpuUsage;
        public double CpuUsage
        {
            get => _cpuUsage;
            set => SetProperty(ref _cpuUsage, value);
        }

        private double _memoryUsage;
        public double MemoryUsage
        {
            get => _memoryUsage;
            set => SetProperty(ref _memoryUsage, value);
        }

        private double _frameRate;
        public double FrameRate
        {
            get => _frameRate;
            set => SetProperty(ref _frameRate, value);
        }

        private string _state;
        public string State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        private Brush _stateBrush; 
        public Brush StateBrush
        {
            get => _stateBrush;
            set => SetProperty(ref _stateBrush, value);
        }

        public SettingStore SettingStore { get; }

        public MenuPanelViewModel(StateService stateService, GrabService grabService, RecordService recordService, SettingStore settingStore)
        {
            try
            {
                SettingStore = settingStore;

                State = "Idle";
                var brush = new SolidColorBrush(Colors.LightGray);
                brush.Freeze();
                StateBrush = brush;

                grabService.GrabbingStarted += GrabbingStarted;
                recordService.RecordingStarted += RecordingStarted;

                SettingCommand = new DelegateCommand(async () =>
                {
                    var message = "When you change the settings, restart the program. Do you agree ?";

                    var result = await DialogHost.Show(new MessageDialog(message, true), "RootDialog", ClosingEventHandler);

                    
                    if (bool.Parse((string)result))
                        await DialogHost.Show(new SettingDialog(), "RootDialog", ClosingEventHandler);
                });

                ExitCommand = new DelegateCommand(App.Current.Shutdown);

                DriveInfos = new ObservableCollection<BindableDriveInfo>();

                foreach (var info in stateService.DriveInfos)
                    DriveInfos.Add(new BindableDriveInfo(info.Name, info.TotalSize));

                Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        CpuUsage = stateService.CpuUsage;
                        MemoryUsage = stateService.MemoryUsage / 1024;
                        
                        foreach (var info in stateService.DriveInfos)
                        {
                            var founded = DriveInfos.First(d => d.Name == info.Name);
                            founded.AvailableFreeSpace = info.AvailableFreeSpace / 1024 / 1024 / 1024;
                        }

                        FrameRate = grabService.FrameRate;

                        await Task.Delay(1000);
                    }
                }, TaskCreationOptions.LongRunning);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show(e.StackTrace);
            }
        }

        private void GrabbingStarted(bool state)
        {
            State = state ? "Run" : "Idle";
            var brush = new SolidColorBrush(state ? Colors.LimeGreen : Colors.LightGray);
            brush.Freeze();
            StateBrush = brush;
        }

        private void RecordingStarted(bool state)
        {
            State = state ? "Recoding" : "Run";
            var brush = new SolidColorBrush(state ? Colors.Yellow : Colors.LimeGreen);
            brush.Freeze();
            StateBrush = brush;
        }

        private async void ShowWorkListDialog()
        {

            return;
        }

        private void ImageDevice_Grabbed(IImageData obj)
        {
            var data = obj as ImageData<byte>;

            //throw new NotImplementedException();
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("You can intercept the closing event, and cancel here.");
        }
    }
}
