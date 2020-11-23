using FIAT_Project.Core.Service;
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

namespace FIAT_Project.Wpf
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
        
        protected override Window CreateShell()
        {
            return new ShellWindow();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry
                .RegisterSingleton<GrabService>()
                .RegisterSingleton<ProtocolService>();
            //throw new NotImplementedException();
        }
    }
}
