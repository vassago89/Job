using FIAT_Project.Core.Service;
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


        public MenuPanelViewModel(StateService stateService)
        {
            SettingCommand = new DelegateCommand(async () =>
            {
                var view = new SettingDialog();

                //show the dialog
                var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);

                //check the result...
                Console.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
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
                    MemoryUsage = stateService.MemoryUsage;

                    foreach (var info in stateService.DriveInfos)
                    {
                        var founded = DriveInfos.First(d => d.Name == info.Name);
                        founded.AvailableFreeSpace = info.AvailableFreeSpace / 1024 / 1024 / 1024;
                    }

                    await Task.Delay(1000);
                }
            }, TaskCreationOptions.LongRunning);
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
