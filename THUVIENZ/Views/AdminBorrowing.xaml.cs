using System.Windows.Controls;
using THUVIENZ.ViewModels;

namespace THUVIENZ.Views
{
    public partial class AdminBorrowing : UserControl
    {
        private readonly AdminCirculationViewModel _viewModel;

        public AdminBorrowing()
        {
            InitializeComponent();
            _viewModel = new AdminCirculationViewModel();
            this.DataContext = _viewModel;
        }
    }
}