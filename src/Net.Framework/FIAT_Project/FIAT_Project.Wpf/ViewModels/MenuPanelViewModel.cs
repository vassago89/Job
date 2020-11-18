using FIAT_Project.Wpf.Views;
using MaterialDesignThemes.Wpf;
using Net.Framework.Device.Matrox;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FIAT_Project.Wpf.ViewModels
{
    public class MenuPanelViewModel : BindableBase
    {
        public DelegateCommand WorkListCommand => new DelegateCommand(ShowWorkListDialog);

        public MatroxSystemGateway _gateWay;

        public MenuPanelViewModel(MatroxSystemGateway gateway)
        {
            _gateWay = gateway;
        }

        private async void ShowWorkListDialog()
        {
            foreach (var imageDevice in _gateWay.ImageDevices)
            {
                imageDevice.Grabbed += ImageDevice_Grabbed;
                imageDevice.ContinuousGrab();
            }
                



            return;
            var view = new WorkListDialog();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);

            //check the result...
            Console.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
        }

        private void ImageDevice_Grabbed(Net.Framework.Data.ImageDatas.IImageData obj)
        {
            //throw new NotImplementedException();
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("You can intercept the closing event, and cancel here.");
        }
    }
}
