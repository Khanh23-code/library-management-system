using System.Windows;
using System.Windows.Controls;
using THUVIENZ.Views;
using THUVIENZ.Views.Components;
using THUVIENZ.Core;

namespace THUVIENZ
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ApplyRouting();
        }

        private void ApplyRouting()
        {
            // Bỏ qua Login để test Admin (Auth đang ở nhánh khác)
            UserSession.UserID = "AD_DEV_TEST";
            UserSession.Role = "Admin";

            string userId = UserSession.UserID ?? "";

            if (userId.StartsWith("AD_"))
            {
                // 1. Khởi tạo thanh Nav cho Admin
                var adminNav = new AdminNavigationBar();
                adminNav.OnNavigate += (page, name) => HandleNavigation(adminNav, page, name);

                // 2. Lắp vào cột trái
                NavContainer.Content = adminNav;

                // 3. Trang mặc định khi Admin vào app
                HandleNavigation(adminNav, new AdminBooks(), "Books");
            }
            else if (userId.StartsWith("RD_"))
            {
                // 1. Khởi tạo thanh Nav cho Reader
                var readerNav = new NavigationBar();
                readerNav.OnNavigate += (page, name) => HandleNavigation(readerNav, page, name);

                // 2. Lắp vào cột trái
                NavContainer.Content = readerNav;

                // 3. Trang mặc định cho Reader
                HandleNavigation(readerNav, new Profile(), "Profile");
            }
        }

        // Hàm xử lý điều hướng dùng chung cho cả Admin và Reader
        private void HandleNavigation(UserControl navBar, UserControl newPage, string pageName)
        {
            // Hiển thị nội dung trang mới vào cột phải
            MainContent.Content = newPage;

            // Đồng bộ trạng thái Highlight trên thanh Nav đang hiển thị
            if (navBar is AdminNavigationBar adminNav)
                adminNav.ActivePage = pageName;
            else if (navBar is NavigationBar readerNav)
                readerNav.ActivePage = pageName;

            // Xử lý đặc biệt cho AdminReaders để hỗ trợ trang "Yêu cầu đăng ký"
            if (newPage is AdminReaders readersPage)
            {
                readersPage.OnSubNavigate += (subPage) => {
                    MainContent.Content = subPage;
                };
            }
        }
    }
}