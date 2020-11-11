using FIAT_Project.Core.Models;
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

        public DelegateCommand SearchCommand => new DelegateCommand(Search);

        public WorkListDialogViewModel()
        {

        }
        
        private async void Search()
        {
           

            using (var context = new FIATDbContext())
            {
                var patient = new Patient();
                var s = context.Patients;
                var l = s.Local;
                context.Patients.Add(patient);
            }
            

            var view = new LoadingDialog();

            //show the dialog
            var result = await DialogHost.Show(view, "WorkListDialog", ClosingEventHandler);

            await Task.Delay(3000);
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("You can intercept the closing event, and cancel here.");
        }
    }
}
