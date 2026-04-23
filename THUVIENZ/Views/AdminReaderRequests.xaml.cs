using System;
using System.Windows;
using System.Windows.Controls;
using THUVIENZ.Views.Components;

namespace THUVIENZ.Views
{
    public partial class AdminReaderRequests : UserControl
    {
        // Sự kiện báo cho MainWindow biết để quay về AdminReaders
        public event Action GoBackRequested;

        public AdminReaderRequests()
        {
            InitializeComponent();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            GoBackRequested?.Invoke();
        }

        private void RequestCard_OnAcceptClick(object sender, RequestCard e)
        {
            MessageBox.Show($"Đã cấp tài khoản cho: {e.ReaderName}. Hệ thống sẽ tự động tạo ID và gửi email thông báo.");
            // Ẩn/Xóa thẻ sau khi xử lý
            e.Visibility = Visibility.Collapsed;
        }

        private void RequestCard_OnRejectClick(object sender, RequestCard e)
        {
            // Có thể tái sử dụng ConfirmDeletePopup ở đây
            MessageBox.Show($"Đã từ chối yêu cầu của: {e.ReaderName}");
            e.Visibility = Visibility.Collapsed;
        }
    }
}