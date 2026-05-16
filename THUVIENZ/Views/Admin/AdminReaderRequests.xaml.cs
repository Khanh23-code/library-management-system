using System.Windows;
using System.Windows.Controls;
using THUVIENZ.ViewModels;

namespace THUVIENZ.Views
{
    public partial class AdminReaderRequests : UserControl
    {
        private readonly AccountApprovalViewModel _viewModel;
        public event System.Action? GoBackRequested;

        public AdminReaderRequests()
        {
            InitializeComponent();
            _viewModel = new AccountApprovalViewModel();
            this.DataContext = _viewModel;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            GoBackRequested?.Invoke();
            // Logic quay lại dashboard (nếu dùng Navigation)
            this.Visibility = Visibility.Collapsed;
        }
    }
}