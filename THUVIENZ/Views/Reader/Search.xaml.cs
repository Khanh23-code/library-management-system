using System.Windows.Controls;
using THUVIENZ.ViewModels;

namespace THUVIENZ.Views
{
    public partial class Search : UserControl
    {
        private readonly SearchViewModel _viewModel;

        public Search()
        {
            InitializeComponent();

            _viewModel = new SearchViewModel();
            this.DataContext = _viewModel;

            _viewModel.ExecuteSearch();
        }
    }
}
