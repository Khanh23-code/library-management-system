using System;
using System.Windows;
using System.Windows.Controls;
using THUVIENZ.Views;

namespace THUVIENZ.Views.Components
{
    public partial class AdminNavigationBar : UserControl
    {
        // Biến đánh dấu trang đang active để XAML đổi màu
        public static readonly DependencyProperty ActivePageProperty =
            DependencyProperty.Register("ActivePage", typeof(string), typeof(AdminNavigationBar), new PropertyMetadata(string.Empty));

        public string ActivePage
        {
            get { return (string)GetValue(ActivePageProperty); }
            set { SetValue(ActivePageProperty, value); }
        }

        // Sự kiện báo ra ngoài MainWindow để chuyển trang
        public event Action<UserControl, string> OnNavigate;

        public AdminNavigationBar()
        {
            InitializeComponent();
        }

        private void BtnBooks_Click(object sender, RoutedEventArgs e)
        {
            // Tạm thời gọi đến class AdminBooks (ta sẽ tạo nó sau)
            // OnNavigate?.Invoke(new AdminBooks(), "Books"); 
        }

        private void BtnReaders_Click(object sender, RoutedEventArgs e)
        {
            // OnNavigate?.Invoke(new AdminReaders(), "Readers"); 
        }

        private void BtnBorrowing_Click(object sender, RoutedEventArgs e)
        {
            // Có thể tái sử dụng view Borrowing cũ hoặc tạo AdminBorrowing
            // OnNavigate?.Invoke(new Borrowing(), "Borrowing"); 
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            // OnNavigate?.Invoke(new AdminReport(), "Report"); 
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new Login().Show();
            Window.GetWindow(this)?.Close();
        }
    }
}