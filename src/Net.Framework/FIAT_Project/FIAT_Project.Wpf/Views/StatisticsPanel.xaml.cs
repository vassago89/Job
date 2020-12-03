using FIAT_Project.Core.Enums;
using FIAT_Project.Wpf.ViewModels;
using System.Windows.Controls;
using System.Windows.Media;

namespace FIAT_Project.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ControlPanel
    /// </summary>
    public partial class StatisticsPanel : UserControl
    {
        private StatisticsPanelViewModel _viewModel => DataContext as StatisticsPanelViewModel;
        
        public ELazer Lazer
        {
            get => _viewModel.Lazer;
            set => _viewModel.Lazer = value;
        }

        public int ImageIndex
        {
            get => _viewModel.ImageIndex;
            set => _viewModel.ImageIndex = value;
        }

        public StatisticsPanel()
        {
            InitializeComponent();  
        }
    }
}
