using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FontAwesome.Sharp;
using THUVIENZ.BLL;
using THUVIENZ.Core;
using THUVIENZ.Models;
using THUVIENZ.Views.Components;

namespace THUVIENZ.Views
{
    /// <summary>
    /// Logic xử lý hiển thị và tương tác của màn hình thông báo Độc giả.
    /// Áp dụng Strict Null Safety tuyệt đối và chú thích Tiếng Việt chi tiết.
    /// </summary>
    public partial class Notifications : UserControl
    {
        // Sự kiện bắn tín hiệu lên MainWindow để clear dấu chấm đỏ ở sidebar
        public event Action? OnNotificationsViewed;

        private readonly NotificationService _notificationService;

        public Notifications()
        {
            InitializeComponent();
            _notificationService = new NotificationService();
        }

        // Kích hoạt ngay khi màn hình thông báo vừa hiển thị
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // 1. Tải danh sách thông báo từ Database
            await LoadNotificationsAsync();

            // 2. Đánh dấu tất cả thông báo của Độc giả hiện tại là đã đọc
            if (!string.IsNullOrEmpty(UserSession.UserID))
            {
                try
                {
                    await _notificationService.MarkAsReadAsync(UserSession.UserID);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Lỗi khi đánh dấu đã đọc: " + ex.Message);
                }
            }

            // 3. Kích hoạt sự kiện báo hiệu lên MainWindow để dọn dẹp chấm đỏ ở menu
            OnNotificationsViewed?.Invoke();
        }

        /// <summary>
        /// Tải danh sách thông báo từ DB và tạo các Card giao diện tương ứng.
        /// </summary>
        private async Task LoadNotificationsAsync()
        {
            NotificationContainer.Children.Clear();

            if (string.IsNullOrEmpty(UserSession.UserID)) return;

            try
            {
                var list = await _notificationService.GetNotificationsAsync(UserSession.UserID);
                
                foreach (var item in list)
                {
                    // Khởi tạo Card giao diện tùy biến
                    var card = new NotificationCard
                    {
                        Title = item.TieuDe,
                        Message = item.NoiDung,
                        Timestamp = GetRelativeTimeString(item.NgayTao),
                        NotiType = item.LoaiThongBao,
                        Tag = item.MaThongBao // Lưu lại mã để xóa trong DB khi người dùng bấm đóng
                    };

                    // Hook các sự kiện tương tác
                    card.OnCloseRequested += NotificationCard_OnCloseRequested;
                    card.OnCardClicked += NotificationCard_OnCardClicked;

                    // Đưa vào StackPanel
                    NotificationContainer.Children.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể nạp dữ liệu thông báo: " + ex.Message, "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Hàm xử lý nút X: Xóa Card thông báo ra khỏi StackPanel và xóa trong DB
        private async void NotificationCard_OnCloseRequested(object sender, RoutedEventArgs e)
        {
            if (sender is NotificationCard card && card.Tag is int notiId)
            {
                // Xóa khỏi giao diện lập tức để tối ưu trải nghiệm (Optimistic UI)
                NotificationContainer.Children.Remove(card);

                try
                {
                    // Xóa vĩnh viễn trong Database
                    await _notificationService.DeleteNotificationAsync(notiId);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Lỗi xóa thông báo: " + ex.Message);
                }
            }
        }

        // Hàm xử lý khi Click vào Card: Nạp dữ liệu và mở Popup chi tiết
        private void NotificationCard_OnCardClicked(object sender, RoutedEventArgs e)
        {
            if (sender is NotificationCard card)
            {
                // Đổ text từ Card sang Popup
                DetailPopup.PopupTitle = card.Title;
                DetailPopup.PopupMessage = card.Message;
                DetailPopup.PopupTimestamp = card.Timestamp;

                // Đồng bộ hóa màu sắc và Icon của Popup khớp với loại thông báo trên Card
                switch (card.NotiType)
                {
                    case NotificationType.Success:
                        DetailPopup.PopupIcon = IconChar.CheckCircle;
                        DetailPopup.IconColor = (Brush)new BrushConverter().ConvertFrom("#137333");
                        DetailPopup.IconBgColor = (Brush)new BrushConverter().ConvertFrom("#E6F4EA");
                        break;
                    case NotificationType.Failure:
                        DetailPopup.PopupIcon = IconChar.TimesCircle;
                        DetailPopup.IconColor = (Brush)new BrushConverter().ConvertFrom("#C5221F");
                        DetailPopup.IconBgColor = (Brush)new BrushConverter().ConvertFrom("#FCE8E6");
                        break;
                    case NotificationType.Warning:
                        DetailPopup.PopupIcon = IconChar.ExclamationTriangle;
                        DetailPopup.IconColor = (Brush)new BrushConverter().ConvertFrom("#B06000");
                        DetailPopup.IconBgColor = (Brush)new BrushConverter().ConvertFrom("#FEF7E0");
                        break;
                    case NotificationType.Info:
                    default:
                        DetailPopup.PopupIcon = IconChar.InfoCircle;
                        DetailPopup.IconColor = (Brush)new BrushConverter().ConvertFrom("#1A73E8");
                        DetailPopup.IconBgColor = (Brush)new BrushConverter().ConvertFrom("#E8F0FE");
                        break;
                }

                // Hiển thị lớp Popup mặt nạ lên màn hình
                DetailPopup.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Tính toán thời gian tương đối thân thiện bằng Tiếng Việt.
        /// </summary>
        private string GetRelativeTimeString(DateTime dt)
        {
            var span = DateTime.Now - dt;
            if (span.TotalDays >= 365)
                return $"{(int)(span.TotalDays / 365)} năm trước";
            if (span.TotalDays >= 30)
                return $"{(int)(span.TotalDays / 30)} tháng trước";
            if (span.TotalDays >= 7)
                return $"{(int)(span.TotalDays / 7)} tuần trước";
            if (span.TotalDays >= 1)
                return $"{(int)span.TotalDays} ngày trước";
            if (span.TotalHours >= 1)
                return $"{(int)span.TotalHours} giờ trước";
            if (span.TotalMinutes >= 1)
                return $"{(int)span.TotalMinutes} phút trước";
            return "Vừa xong";
        }
    }
}