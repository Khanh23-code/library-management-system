using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Reflection;
using THUVIENZ.BLL;
using THUVIENZ.Core;
using THUVIENZ.ViewModels;
using System;

namespace THUVIENZ.Views
{
    public partial class Login : Window
    {
        private readonly AuthService _authService;
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        private const uint EM_SETSEL = 0x00B1;

        public Login()
        {
            InitializeComponent();
            _authService = new AuthService();
            this.DataContext = new LoginViewModel();
        }

        private void LnkRegister_Click(object sender, RoutedEventArgs e)
        {
            Register registerWindow = new Register();
            registerWindow.Show();
            this.Close();
        }

        private void LnkForgot_Click(object sender, RoutedEventArgs e)
        {
            ForgotPassword forgotWindow = new ForgotPassword();
            forgotWindow.Show();
            this.Close();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // 1. Lấy ID người dùng nhập vào
            string id = txtUserID.Text.Trim();

            // 2. Kiểm tra xác thực (giả sử bạn gọi AuthService để check pass)
            // if (_authService.Login(id, txtPassword.Password)) 

            if (!string.IsNullOrEmpty(id))
            {
                // 3. LƯU QUAN TRỌNG: Gán ID vào Session trước khi mở MainWindow
                THUVIENZ.Core.UserSession.UserID = id;

                // 4. Mở MainWindow và đóng cửa sổ Login hiện tại
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Vui lòng nhập ID đăng nhập!");
            }
        }
    }
}