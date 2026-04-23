using System.Windows;
using System.Windows.Controls;
using THUVIENZ.Views.Components;

namespace THUVIENZ.Views
{
    public partial class AdminReaders : UserControl
    {
        // Biến tạm để lưu ID của độc giả đang chuẩn bị xóa
        private string _readerIdToDelete;

        public event Action<UserControl> OnSubNavigate;

        public AdminReaders()
        {
            InitializeComponent();
        }

        // Hàm này chạy khi bấm nút "DELETE" trên bất kỳ ReaderCard nào
        private void ReaderCard_OnDeleteClick(object sender, ReaderCard e)
        {
            // Lưu lại ID để lát nữa xóa
            _readerIdToDelete = e.ReaderId;

            // Cập nhật thông báo cho Popup
            DeletePopup.PopupTitle = "Xóa Độc giả";
            DeletePopup.PopupMessage = $"Bạn có chắc chắn muốn xóa tài khoản của độc giả '{e.ReaderName}' (ID: {e.ReaderId}) không? Dữ liệu mượn trả liên quan cũng có thể bị ảnh hưởng.";

            // Hiện popup lên
            DeletePopup.Visibility = Visibility.Visible;
        }

        // Hàm này chạy khi bấm "Xóa vĩnh viễn" trong cái Popup
        private void DeletePopup_OnConfirm(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_readerIdToDelete))
            {
                // TODO: Gọi logic xuống Database để xóa độc giả có ID là _readerIdToDelete
                MessageBox.Show($"Đã xóa độc giả: {_readerIdToDelete}");

                // Reset biến
                _readerIdToDelete = null;
            }
        }

        private void BtnViewRequests_Click(object sender, RoutedEventArgs e)
        {
            var requestPage = new AdminReaderRequests();

            // Thiết lập logic quay lại: Khi trang Request báo "GoBack", 
            // ta yêu cầu MainWindow hiện lại chính trang AdminReaders này.
            requestPage.GoBackRequested += () =>
            {
                OnSubNavigate?.Invoke(this);
            };

            // Báo cho MainWindow nạp trang AdminReaderRequests vào khung hình
            OnSubNavigate?.Invoke(requestPage);
        }
    }
}