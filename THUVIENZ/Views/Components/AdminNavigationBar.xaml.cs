using System;
using System.Windows;
using System.Windows.Controls;
using THUVIENZ.Views; // Chứa AdminBooks, AdminReaders, v.v.

namespace THUVIENZ.Views.Components
{
    public partial class AdminNavigationBar : UserControl
    {
        public static readonly DependencyProperty ActivePageProperty =
            DependencyProperty.Register("ActivePage", typeof(string), typeof(AdminNavigationBar), new PropertyMetadata(string.Empty));

        public string ActivePage
        {
            get { return (string)GetValue(ActivePageProperty); }
            set { SetValue(ActivePageProperty, value); }
        }

        // Sự kiện dùng chung để MainWindow bắt được
        public event Action<UserControl, string>? OnNavigate;

        public AdminNavigationBar()
        {
            InitializeComponent();
        }

        private void BtnBooks_Click(object sender, RoutedEventArgs e)
        {
            OnNavigate?.Invoke(new AdminBooks(), "Books");
        }

        private void BtnReaders_Click(object sender, RoutedEventArgs e)
        {
            NavigateToReadersPage(new AdminReaders());
        }

        private void NavigateToReadersPage(AdminReaders readersPage)
        {
            readersPage.OnSubNavigate += (subPage) => {
                if (subPage is AdminReaders newReadersPage)
                {
                    // Khi trang con (AdminReaderRequests) yêu cầu quay lại AdminReaders mới,
                    // ta cần đăng ký lại sự kiện OnSubNavigate cho trang mới này.
                    NavigateToReadersPage(newReadersPage);
                }
                else
                {
                    OnNavigate?.Invoke(subPage, "Readers");
                }
            };
            OnNavigate?.Invoke(readersPage, "Readers");
        }

        private void BtnBorrowing_Click(object sender, RoutedEventArgs e)
        {
            OnNavigate?.Invoke(new AdminBorrowing(), "Borrowing");
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            OnNavigate?.Invoke(new AdminReport(), "Report");
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new Login().Show();
            Window.GetWindow(this)?.Close();
        }
    }
}