using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using FontAwesome.Sharp;

namespace THUVIENZ.UserControls
{
    public partial class PasswordInput : UserControl
    {
        // 1. Giữ nguyên thuộc tính Placeholder
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(PasswordInput), new PropertyMetadata("Enter password"));

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        // 2. Chuyển Password thành DependencyProperty có hỗ trợ TwoWay Binding
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                "Password",
                typeof(string),
                typeof(PasswordInput),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordPropertyChanged));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Cờ kiểm soát để tránh vòng lặp vô hạn khi UI cập nhật Data và Data cập nhật lại UI
        private bool _isUpdating = false;

        // Xử lý khi ViewModel ra lệnh xóa trắng (hoặc đổi) mật khẩu
        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PasswordInput)d;
            var newValue = (string)e.NewValue ?? string.Empty;

            if (control._isUpdating) return;

            control._isUpdating = true;
            if (control.txtPassword.Password != newValue)
                control.txtPassword.Password = newValue;
            if (control.txtPasswordVisible.Text != newValue)
                control.txtPasswordVisible.Text = newValue;
            control._isUpdating = false;
        }

        public PasswordInput()
        {
            InitializeComponent();
        }

        // 3. Cập nhật biến Password mỗi khi người dùng gõ phím
        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isUpdating) return;

            _isUpdating = true;
            if (txtPasswordVisible.Text != txtPassword.Password)
                txtPasswordVisible.Text = txtPassword.Password;

            // Bắn dữ liệu ra DependencyProperty để ViewModel nhận được
            Password = txtPassword.Password;
            _isUpdating = false;

            // Ẩn/hiện placeholder
            lblPlaceholder.Visibility = string.IsNullOrEmpty(txtPassword.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating) return;

            _isUpdating = true;
            if (txtPassword.Password != txtPasswordVisible.Text)
                txtPassword.Password = txtPasswordVisible.Text;

            // Bắn dữ liệu ra DependencyProperty để ViewModel nhận được
            Password = txtPasswordVisible.Text;
            _isUpdating = false;

            lblPlaceholder.Visibility = string.IsNullOrEmpty(txtPasswordVisible.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            // Lấy reference của IconImage từ bên trong ControlTemplate
            var iconPassword = (IconImage)btnShowPassword.Template.FindName("iconPassword", btnShowPassword);

            if (txtPasswordVisible.Visibility == Visibility.Collapsed)
            {
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;

                // ĐỔI SANG ICON CON MẮT BỊ GẠCH CHÉO
                if (iconPassword != null) iconPassword.Icon = IconChar.EyeSlash;

                txtPasswordVisible.Focus();
                txtPasswordVisible.CaretIndex = txtPasswordVisible.Text.Length;
            }
            else
            {
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                txtPassword.Visibility = Visibility.Visible;

                // ĐỔI LẠI ICON CON MẮT MỞ
                if (iconPassword != null) iconPassword.Icon = IconChar.Eye;

                txtPassword.Focus();

                var selectMethod = typeof(PasswordBox).GetMethod("Select", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (selectMethod != null)
                {
                    selectMethod.Invoke(txtPassword, new object[] { txtPassword.Password.Length, 0 });
                }
            }
        }
    }
}