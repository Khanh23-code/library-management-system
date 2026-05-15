using System.Windows;
using System.Windows.Controls;
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

        public NavigationBar()
        {
            InitializeComponent();
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
    }
}
