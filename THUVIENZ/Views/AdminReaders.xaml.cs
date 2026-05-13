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
            requestPage.GoBackRequested += () =>
            {
                // Khởi tạo trang mới để làm mới UI và Data
                OnSubNavigate?.Invoke(new AdminReaders());
            };
            OnSubNavigate?.Invoke(requestPage);
        }

        // Các hàm cũ về Popup có thể được refactor vào Command trong ViewModel sau
        private void DeletePopup_OnConfirm(object sender, EventArgs e)
        {
            DeletePopup.Visibility = Visibility.Collapsed;
        }

        private void TabActive_Click(object sender, RoutedEventArgs e)
        {
            // Frontend UI branch: stub for Active tab switching
        }

        private void TabLocked_Click(object sender, RoutedEventArgs e)
        {
            // Frontend UI branch: stub for Locked tab switching
        }

        private void TabDisActive_Click(object sender, RoutedEventArgs e)
        {
            // Frontend UI branch: stub for DisActive tab switching
        }
    }
}