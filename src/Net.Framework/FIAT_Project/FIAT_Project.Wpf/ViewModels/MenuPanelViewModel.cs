using FIAT_Project.Wpf.Views;
using MaterialDesignThemes.Wpf;
using Net.Framework.Data.ImageDatas;
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
        public DelegateCommand SettingCommand { get; }
        public DelegateCommand ExitCommand { get; }

        public MenuPanelViewModel()
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
