using FIAT_Project.Core.Service;
using Net.Framework.Device.Matrox;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FIAT_Project.Wpf.ViewModels
{
    public class ControlPanelViewModel : BindableBase
    {
        public DelegateCommand GrabCommand { get; }
        public DelegateCommand StopCommand { get; }

        public ControlPanelViewModel(GrabService grabService)
        {
            GrabCommand = new DelegateCommand(() =>
            {
                grabService.Start();
            });

            StopCommand = new DelegateCommand(() =>
            {
                grabService.Stop();
            });
        }
    }
}
