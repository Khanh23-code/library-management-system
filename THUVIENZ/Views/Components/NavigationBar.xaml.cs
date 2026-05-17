using System;
using System.Windows;
using System.Windows.Controls;
using THUVIENZ.BLL;
using THUVIENZ.Core;
using THUVIENZ.Views;
using THUVIENZ.Views.Components;

namespace THUVIENZ
{
    public partial class NavigationBar : UserControl
    {
        public static readonly DependencyProperty ActivePageProperty =
            DependencyProperty.Register("ActivePage", typeof(string), typeof(NavigationBar), new PropertyMetadata(string.Empty));

        public string ActivePage
        {
            get { return (string)GetValue(ActivePageProperty); }
            set { SetValue(ActivePageProperty, value); }
        }

        public event Action<UserControl, string>? OnNavigate;

        private readonly NotificationService _notificationService = new NotificationService();

        public NavigationBar()
        {
            InitializeComponent();
        }

        private System.Windows.Threading.DispatcherTimer? _unreadPollTimer;

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CheckUnreadNotificationsAsync();

            // Khởi tạo bộ đếm thời gian polling kiểm tra thông báo mới (Real-time polling)
            if (_unreadPollTimer == null)
            {
                _unreadPollTimer = new System.Windows.Threading.DispatcherTimer();
                _unreadPollTimer.Interval = TimeSpan.FromSeconds(10); // Cứ mỗi 10 giây quét DB một lần
                _unreadPollTimer.Tick += async (s, args) => await CheckUnreadNotificationsAsync();
                _unreadPollTimer.Start();
            }
        }

        private async Task CheckUnreadNotificationsAsync()
        {
            if (!string.IsNullOrEmpty(UserSession.UserID))
            {
                try
                {
                    bool hasUnread = await _notificationService.HasUnreadNotificationsAsync(UserSession.UserID);
                    RedDotBadge.Visibility = hasUnread ? Visibility.Visible : Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Lỗi check thông báo chưa đọc: " + ex.Message);
                    RedDotBadge.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                RedDotBadge.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            // "Báo cáo" ra bên ngoài là tôi muốn chuyển sang trang Profile
            OnNavigate?.Invoke(new Profile(), "Profile");
        }

        private void BtnFavorite_Click(object sender, RoutedEventArgs e)
        {
            OnNavigate?.Invoke(new Favorite(), "Favorite");
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            OnNavigate?.Invoke(new Search(), "Search");
        }

        private void BtnBorrowing_Click(object sender, RoutedEventArgs e)
        {
            OnNavigate?.Invoke(new Borrowing(), "Borrowing");
        }

        private void BtnNotifications_Click(object sender, RoutedEventArgs e)
        {
            OnNavigate?.Invoke(new Notifications(), "Notifications");
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Riêng Logout thì vẫn đóng MainWindow và mở Login
            new Login().Show();
            Window.GetWindow(this)?.Close();
        }

        public void ClearRedDot()
        {
            RedDotBadge.Visibility = Visibility.Collapsed;
        }
    }
}
