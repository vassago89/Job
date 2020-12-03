using FIAT_Project.Core.Enums;
using FIAT_Project.Core.Service;
using FIAT_Project.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace FIAT_Project.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ImageControl
    /// </summary>
    public partial class ImageControl : UserControl
    {
        private ImageControlViewModel _viewModel => DataContext as ImageControlViewModel;

        public int ImageIndex
        {
            get => _viewModel.ImageIndex;
            set => _viewModel.ImageIndex = value;
        }

        public string Header
        {
            get => _viewModel.Header;
            set => _viewModel.Header = value;
        }

        public bool OnLazer
        {
            get => _viewModel.OnLazer;
            set => _viewModel.OnLazer = value;
        }

        public ELazer Lazer
        {
            get => _viewModel.Lazer;
            set => _viewModel.Lazer = value;
        }

        public ImageControl()
        {
            InitializeComponent();
        }
    }
}
