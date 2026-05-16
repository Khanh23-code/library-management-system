using System;
using System.Windows;
using System.Windows.Controls;

namespace THUVIENZ.Views
{
    public partial class Notifications : UserControl
    {
        // Sự kiện báo lên MainWindow khi người dùng đã mở trang thông báo ra xem
        public event Action? OnNotificationsViewed;

        public Notifications()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Ngay khi trang được load lên màn hình, bắn event báo đã xem tất cả
            OnNotificationsViewed?.Invoke();
        }

        private void NotificationCard_OnCloseRequested(object sender, RoutedEventArgs e)
        {
            // Xóa card khỏi giao diện khi bấm nút X
            if (sender is UIElement card)
            {
                NotificationContainer.Children.Remove(card);
            }
        }
    }
}