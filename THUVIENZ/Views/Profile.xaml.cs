using System.Windows.Controls;
using THUVIENZ.Core;
using THUVIENZ.ViewModels;

namespace THUVIENZ.Views
{
    public partial class Profile : UserControl
    {
        private readonly ProfileViewModel _viewModel;

        public Profile()
        {
            InitializeComponent();
            
            // Khá»Ÿi táº¡o ViewModel vÃ  thiáº¿t láº­p DataContext cho Binding
            _viewModel = new ProfileViewModel();
            this.DataContext = _viewModel;

            // Tải dữ liệu hồ sơ của độc giả đang đăng nhập
            if (!string.IsNullOrEmpty(UserSession.UserID))
            {
                _ = _viewModel.LoadProfileDataAsync(UserSession.UserID);
            }
        }
    }
}
