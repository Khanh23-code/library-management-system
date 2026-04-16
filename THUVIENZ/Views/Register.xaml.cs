using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using THUVIENZ.BLL;

namespace THUVIENZ.Views
{
    public partial class Register : Window
    {
        private readonly AuthService _authService;

        public Register()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fullName = txtFullName.Text;
                string id = txtId.Text;
                string password = txtPassword.Password;
                string confirmPassword = txtConfirmPassword.Password;

                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (password != confirmPassword)
                {
                    MessageBox.Show("Mật khẩu xác nhận không khớp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Giả định AuthService của bạn có hàm Register(username, password, fullName)
                // bool isSuccess = _authService.Register(username, password, fullName);

                // Tạm thời hiển thị thành công
                MessageBox.Show("Đăng ký thành công! Vui lòng chờ Admin duyệt.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Trở về trang đăng nhập
                new Login().Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBackToLogin_Click(object sender, MouseButtonEventArgs e)
        {
            new Login().Show();
            this.Close();
        }

    }
}