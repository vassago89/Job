using FIAT_Project.Core;
using FIAT_Project.Core.Enums;
using FIAT_Project.Core.Service;
using FIAT_Project.Wpf.Datas;
using FIAT_Project.Wpf.Views;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Net.Framework.Algorithm.Enums;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FIAT_Project.Wpf.ViewModels
{
    public class SettingDialogViewModel : BindableBase
    {
        //private FIATDbContext _local = new FIATDbContext();
        //public FIATDbContext Local => _local;

        //public DelegateCommand SearchCommand => new DelegateCommand(Search);

        public IEnumerable<string> Ports { get; }
        public IEnumerable<EThresholdMethod> Methods { get; }

        public SystemConfig SystemConfig { get; }

        public EThresholdMethod Method660
        {
            get => SystemConfig.MethodDictionary[ELazer.L660];
            set => SystemConfig.MethodDictionary[ELazer.L660] = value;
        }

        public EThresholdMethod Method760
        {
            get => SystemConfig.MethodDictionary[ELazer.L760];
            set => SystemConfig.MethodDictionary[ELazer.L760] = value;
        }

        public SettingStore SettingStore { get; }
        public DelegateCommand RestartCommand { get; }
        public DelegateCommand DcfCommand { get; }
        public DelegateCommand CaptureCommand { get; }
        public DelegateCommand RecordCommand { get; }

        public IEnumerable<EProtocolType> ProtocolTypes => Enum.GetValues(typeof(EProtocolType)) as IEnumerable<EProtocolType>;
        public IEnumerable<EChannel> Channels => Enum.GetValues(typeof(EChannel)) as IEnumerable<EChannel>;
        
        public EChannel Channel660
        {
            get => SystemConfig.ChennelDictionary[ELazer.L660];
            set => SystemConfig.ChennelDictionary[ELazer.L660] = value;
        }

        public EChannel Channel760
        {
            get => SystemConfig.ChennelDictionary[ELazer.L760];
            set => SystemConfig.ChennelDictionary[ELazer.L760] = value;
        }

        private string _dcfPath;
        public string DcfPath
        {
            get => _dcfPath;
            set
            {
                SetProperty(ref _dcfPath, value);
                SystemConfig.DcfPath = _dcfPath;
            }
        }

        private string _capturePath;
        public string CapturePath
        {
            get => _capturePath;
            set
            {
                SetProperty(ref _capturePath, value);
                SystemConfig.CapturePath = _capturePath;
            }
        }

        private string _recordPath;
        public string RecordPath
        {
            get => _recordPath;
            set
            {
                SetProperty(ref _recordPath, value);
                SystemConfig.RecordPath = _recordPath;
            }
        }

        public SettingDialogViewModel(SettingStore settingStore, SystemConfig systemConfig)
        {
            try
            {
                SettingStore = settingStore;
                SystemConfig = systemConfig;

                DcfPath = systemConfig.DcfPath;
                RecordPath = systemConfig.RecordPath;
                CapturePath = systemConfig.CapturePath;

                Ports = SerialPort.GetPortNames();

                Methods = typeof(EThresholdMethod).GetEnumValues().Cast<EThresholdMethod>();

                RestartCommand = new DelegateCommand(() =>
                {
                    systemConfig.Save(Environment.CurrentDirectory);
                    Application.Current.Shutdown(1000);
                });

                DcfCommand = new DelegateCommand(() =>
                {
                    var dialog = new OpenFileDialog();

                    dialog.InitialDirectory = Path.Combine(Environment.CurrentDirectory, "DCF123");
                    if (dialog.ShowDialog() == true)
                        DcfPath = dialog.FileName;
                });

                CaptureCommand = new DelegateCommand(() =>
                {
                    var dialog = new CommonOpenFileDialog();
                    dialog.IsFolderPicker = true;
                    dialog.DefaultDirectory = systemConfig.CapturePath;// Path.Combine(Environment.CurrentDirectory, "../Capture");
                    dialog.InitialDirectory = systemConfig.CapturePath;
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                        CapturePath = dialog.FileName;
                });

                RecordCommand = new DelegateCommand(() =>
                {
                    var dialog = new CommonOpenFileDialog();
                    dialog.IsFolderPicker = true;
                    dialog.DefaultDirectory = systemConfig.RecordPath;//Path.Combine(Environment.CurrentDirectory, );
                    dialog.InitialDirectory = systemConfig.RecordPath;
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                        RecordPath = dialog.FileName;
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show(e.StackTrace);
            }
}

        //private async void Search()
        //{


        //    using (var context = new FIATDbContext())
        //    {
        //        var patient = new Patient();
        //        var s = context.Patients;
        //        var l = s.Local;
        //        context.Patients.Add(patient);
        //    }


        //    var view = new LoadingDialog();

        //    //show the dialog
        //    var result = await DialogHost.Show(view, "WorkListDialog", ClosingEventHandler);

        //    await Task.Delay(3000);
        //}

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("You can intercept the closing event, and cancel here.");
        }
    }
}
