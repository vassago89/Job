using FIAT_Project.Wpf.Views;
using MaterialDesignThemes.Wpf;
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

        public MenuPanelViewModel()
        {

        }

        private async void ShowWorkListDialog()
        {
            var view = new WorkListDialog();

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);

            //check the result...
            Console.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("You can intercept the closing event, and cancel here.");
        }
    }
}
