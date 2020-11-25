using FIAT_Project.Wpf.Views;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIAT_Project.Wpf.ViewModels
{
    public class WorkListDialogViewModel : BindableBase
    {
        //private FIATDbContext _local = new FIATDbContext();
        //public FIATDbContext Local => _local;
        
        public WorkListDialogViewModel()
        {

        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("You can intercept the closing event, and cancel here.");
        }
    }
}
