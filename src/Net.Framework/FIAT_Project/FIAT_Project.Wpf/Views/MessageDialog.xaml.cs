using System.Windows.Controls;

namespace FIAT_Project.Wpf.Views
{
    /// <summary>
    /// Interaction logic for WorkListDialog
    /// </summary>
    public partial class MessageDialog : UserControl
    {
        public string Message { get; }
        public bool CancleAvailable { get; }

        public MessageDialog(string message, bool cancleAvailable)
        {
            Message = message;
            CancleAvailable = cancleAvailable;

            InitializeComponent();
        }
    }
}
