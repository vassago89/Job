using FIAT_Project.Core;
using FIAT_Project.Core.Enums;
using FIAT_Project.Core.Service;
using FIAT_Project.Wpf.Datas;
using FIAT_Project.Wpf.Views;
using MaterialDesignThemes.Wpf;
using Net.Framework.Algorithm.Enums;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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

        public SettingDialogViewModel(SettingStore settingStore, SystemConfig systemConfig)
        {
            try
            {
                SettingStore = settingStore;
                SystemConfig = systemConfig;
                Ports = SerialPort.GetPortNames();

                Methods = typeof(EThresholdMethod).GetEnumValues().Cast<EThresholdMethod>();

                RestartCommand = new DelegateCommand(() =>
                {
                    systemConfig.Save(Environment.CurrentDirectory);
                    Application.Current.Shutdown(1000);
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
