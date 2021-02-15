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

        public bool IsColor
        {
            get => _viewModel.IsColor;
            set => _viewModel.IsColor = value;
        }

        public bool IsMerged
        {
            get => _viewModel.IsMerged;
            set => _viewModel.IsMerged = value;
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
