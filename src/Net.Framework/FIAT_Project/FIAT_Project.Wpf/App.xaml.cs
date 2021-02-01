using FIAT_Project.Core;
using FIAT_Project.Core.Enums;
using FIAT_Project.Core.Service;
using FIAT_Project.Wpf.Datas;
using FIAT_Project.Wpf.Views;
using Net.Framework.Device.Matrox;
using Net.Framework.Matrox;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace FIAT_Project.Wpf
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnInitialized()
        {
            RenderOptions.ProcessRenderMode = RenderMode.Default;

            base.OnInitialized();

            var systemConfig = Container.Resolve<SystemConfig>();
            var protocolService = Container.Resolve<ProtocolService>();

            protocolService.SetLed(systemConfig.ValueLed * 1000.0);
            protocolService.Set660(systemConfig.ValueDictionary[ELazer.L660] * 1000.0);
            protocolService.Set760(systemConfig.ValueDictionary[ELazer.L760] * 1000.0);

            protocolService.SetGain(systemConfig.GainLed, ELazer.L660, true);
            protocolService.SetGain(systemConfig.GainDictionary[ELazer.L660], ELazer.L660);
            protocolService.SetGain(systemConfig.GainDictionary[ELazer.L760], ELazer.L760);

            var maxExp = Math.Max(systemConfig.ExposureLed,
                Math.Max(systemConfig.ExposureDictionary[ELazer.L660],
                systemConfig.ExposureDictionary[ELazer.L760]));

            protocolService.SetFrameRate(1000.0 / maxExp);

            protocolService.SetExposure(systemConfig.ExposureLed, ELazer.L660, true);
            protocolService.SetExposure(systemConfig.ExposureDictionary[ELazer.L660], ELazer.L660);
            protocolService.SetExposure(systemConfig.ExposureDictionary[ELazer.L760], ELazer.L760);
        }
        
        protected override Window CreateShell()
        {
            return new ShellWindow();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry
                .RegisterSingleton<GrabService>()
                .RegisterSingleton<ProcessService>()
                .RegisterSingleton<RecordService>()
                .RegisterSingleton<ProtocolService>()
                .RegisterSingleton<SettingStore>()
                .RegisterInstance(SystemConfig.Load(Environment.CurrentDirectory));
            //throw new NotImplementedException();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var protocolService = Container.Resolve<ProtocolService>();
            protocolService.OffLed();
            protocolService.Off660();
            protocolService.Off760();

            MatroxObjectPool.Dispose();
            MatroxApplicationHelper.Dispose();

            if (e.ApplicationExitCode == 0)
            {
               
                //MatroxHelper.FreeApplication();
                //try
                //{
                //    c
                //    Container.Resolve<IoService>().Stop();
                //    Container.Resolve<IoService>().Cancle();
                //}
                //catch
                //{
                //    var pm = new PatternMatching();
                //    pm.AddPattern
                //}
            }

            //MatroxHelper.FreeApplication();
            //CudaMethods.CUDA_RELEASE();

            base.OnExit(e);
        }

        private void PrismApplication_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Exception.Message);
            e.Handled = true;
            return;
        }
    }
}
