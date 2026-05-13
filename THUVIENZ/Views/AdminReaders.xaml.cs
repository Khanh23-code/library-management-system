using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using THUVIENZ.Core;
using THUVIENZ.DAL;
using THUVIENZ.Models;
using THUVIENZ.ViewModels;

namespace THUVIENZ.Views
{
    public partial class AdminReaders : UserControl
    {
        private readonly ReaderManagementViewModel _viewModel;
        public event Action<UserControl>? OnSubNavigate;

        public ICommand OpenSuspendPopupCommand { get; }
        public ICommand OpenDeletePopupCommand { get; }

        public AdminReaders()
        {
            InitializeComponent();
            _viewModel = new ReaderManagementViewModel();
            this.DataContext = _viewModel;

            OpenSuspendPopupCommand = new RelayCommand(param => OpenActionPopup(param as DocGia, "Suspend"));
            OpenDeletePopupCommand = new RelayCommand(param =>
            {
                // Nếu đang ở tab Active thì mở popup xử lý có nhập lý do
                if (_viewModel.CurrentTabStatus == "Active")
                {
                    OpenActionPopup(param as DocGia, "Delete");
                }
                else
                {
                    // Nếu ở tab khác (đã khóa/vô hiệu hóa) thì dùng luôn lệnh xóa cũ của ViewModel
                    if (_viewModel.DeleteReaderCommand.CanExecute(param))
                    {
                        _viewModel.DeleteReaderCommand.Execute(param);
                    }
                }
            });

            ActionPopup.OnActionSubmitted += ActionPopup_OnActionSubmitted;
        }

        private void OpenActionPopup(DocGia? reader, string mode)
        {
            if (reader == null) return;
            ActionPopup.TargetReader = reader;
            ActionPopup.ActionMode = mode;
            ActionPopup.ReasonText = string.Empty;
            ActionPopup.Visibility = Visibility.Visible;
        }

        private async void ActionPopup_OnActionSubmitted(string reason, string duration)
        {
            var targetReader = ActionPopup.TargetReader;
            if (targetReader == null) return;

            try
            {
                using var context = new LmsDbContext();
                // Đảm bảo không bị lỗi reference rỗng bằng cách tự cấp tài khoản nếu chưa có
                if (string.IsNullOrEmpty(targetReader.TenDangNhap))
                {
                    string targetStatus = ActionPopup.ActionMode == "Suspend" ? "Locked" : "DisActive";
                    var newTaiKhoan = new TaiKhoan
                    {
                        TenDangNhap = $"acc_{targetReader.MaDocGia}_{DateTime.Now:yyMMddHHmmss}",
                        MatKhau = "manual_locked",
                        Quyen = "Reader",
                        TrangThai = targetStatus
                    };
                    context.TaiKhoans.Add(newTaiKhoan);
                    var dbReader = await context.DocGias.FindAsync(targetReader.MaDocGia);
                    if (dbReader != null) dbReader.TenDangNhap = newTaiKhoan.TenDangNhap;
                    await context.SaveChangesAsync();
                }
                else
                {
                    var account = await context.TaiKhoans.FindAsync(targetReader.TenDangNhap);
                    if (account != null)
                    {
                        string targetStatus = ActionPopup.ActionMode == "Suspend" ? "Locked" : "DisActive";
                        account.TrangThai = targetStatus;
                        await context.SaveChangesAsync();
                    }
                }

                string summary = ActionPopup.ActionMode == "Suspend" 
                    ? $"Đã đình chỉ tài khoản độc giả thành công!\n- Thời hạn: {duration}\n- Lý do chi tiết: {reason}"
                    : $"Đã vô hiệu hóa tài khoản độc giả thành công!\n- Lý do chi tiết: {reason}";

                MessageBox.Show(summary, "Hoàn tất xử lý", MessageBoxButton.OK, MessageBoxImage.Information);
                await _viewModel.LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xử lý từ CSDL: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnViewRequests_Click(object sender, RoutedEventArgs e)
        {
            var requestPage = new AdminReaderRequests();
            requestPage.GoBackRequested += () =>
            {
                // Khởi tạo trang mới để làm mới UI và Data
                OnSubNavigate?.Invoke(new AdminReaders());
            };
            OnSubNavigate?.Invoke(requestPage);
        }

        private void DeletePopup_OnConfirm(object sender, EventArgs e)
        {
            DeletePopup.Visibility = Visibility.Collapsed;
        }

        private void TabActive_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.SetTabStatus("Active");
        }

        private void TabLocked_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.SetTabStatus("Locked");
        }

        private void TabDisActive_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.SetTabStatus("DisActive");
        }
    }
}