using System;
using System.Windows;
using System.Windows.Controls;
using THUVIENZ.ViewModels;

namespace THUVIENZ.Views
{
    public partial class AdminReaders : UserControl
    {
        private readonly ReaderManagementViewModel _viewModel;
        public event Action<UserControl>? OnSubNavigate;

        public AdminReaders()
        {
            InitializeComponent();
            _viewModel = new ReaderManagementViewModel();
            this.DataContext = _viewModel;
        }

        private void BtnViewRequests_Click(object sender, RoutedEventArgs e)
        {
            var requestPage = new AdminReaderRequests();
            // Điều hướng sang trang yêu cầu (nếu MainWindow hỗ trợ OnSubNavigate)
            OnSubNavigate?.Invoke(requestPage);
        }

        // Các hàm cũ về Popup có thể được refactor vào Command trong ViewModel sau
        private void DeletePopup_OnConfirm(object sender, EventArgs e)
        {
            DeletePopup.Visibility = Visibility.Collapsed;
        }
    }
}