using System.Windows.Input;
using THUVIENZ.BLL;
using THUVIENZ.Core;

namespace THUVIENZ.ViewModels
{
    public class RegisterViewModel : ObservableObject
    {
        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        // Lưu ý: Password và ConfirmPassword thường xử lý trực tiếp ở View vì lý do bảo mật trong WPF, 
        // nhưng ta vẫn có thể để đây nếu team muốn dùng cho các mục đích khác.

        public RegisterViewModel()
        {
            // Khởi tạo các command nếu sau này team muốn chuyển hẳn sang MVVM thuần
        }
    }
}