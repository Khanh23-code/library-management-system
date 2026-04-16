using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace THUVIENZ.UserControls
{
    public partial class PasswordInput : UserControl
    {
        // Tạo thuộc tính Placeholder để gán chữ bên ngoài XAML
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(PasswordInput), new PropertyMetadata("Enter password"));

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        // Tạo thuộc tính Password để lấy giá trị mật khẩu dễ dàng
        public string Password
        {
            get
            {
                return txtPasswordVisible.Visibility == Visibility.Visible
                    ? txtPasswordVisible.Text
                    : txtPassword.Password;
            }
        }

        public PasswordInput()
        {
            InitializeComponent();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (txtPasswordVisible.Text != txtPassword.Password)
                txtPasswordVisible.Text = txtPassword.Password;

            // Ẩn/hiện placeholder
            lblPlaceholder.Visibility = string.IsNullOrEmpty(txtPassword.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPassword.Password != txtPasswordVisible.Text)
                txtPassword.Password = txtPasswordVisible.Text;

            lblPlaceholder.Visibility = string.IsNullOrEmpty(txtPasswordVisible.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (txtPasswordVisible.Visibility == Visibility.Collapsed)
            {
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;
                btnShowPassword.Content = " ";
                txtPasswordVisible.Focus();
                txtPasswordVisible.CaretIndex = txtPasswordVisible.Text.Length;
            }
            else
            {
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                txtPassword.Visibility = Visibility.Visible;
                btnShowPassword.Content = "👁";
                txtPassword.Focus();

                var selectMethod = typeof(PasswordBox).GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic);
                if (selectMethod != null)
                {
                    selectMethod.Invoke(txtPassword, new object[] { txtPassword.Password.Length, 0 });
                }
            }
        }
    }
}