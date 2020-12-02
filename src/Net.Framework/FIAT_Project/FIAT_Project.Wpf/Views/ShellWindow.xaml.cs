using FIAT_Project.Core.Enums;
using System.Windows;

namespace FIAT_Project.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellWindow : Window
    {
        public static ELazer Lazer660 => ELazer.L660;
        public static ELazer Lazer760 => ELazer.L760;

        public ShellWindow()
        {
            InitializeComponent();
        }
    }
}
