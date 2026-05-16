using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FontAwesome.Sharp;

namespace THUVIENZ.Views
{
    public partial class Notifications : UserControl
    {
        // Sự kiện bắn tín hiệu lên MainWindow để clear dấu chấm đỏ ở sidebar
        public event Action? OnNotificationsViewed;

        public Notifications()
        {
            InitializeComponent();
        }

        // Kích hoạt ngay khi màn hình thông báo vừa hiển thị
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            OnNotificationsViewed?.Invoke();
        }

        // Hàm xử lý nút X: Xóa Card thông báo ra khỏi StackPanel
        private void NotificationCard_OnCloseRequested(object sender, RoutedEventArgs e)
        {
            if (sender is UIElement card)
            {
                NotificationContainer.Children.Remove(card);
            }
        }

        // Hàm xử lý khi Click vào Card: Nạp dữ liệu và mở Popup chi tiết
        private void NotificationCard_OnCardClicked(object sender, RoutedEventArgs e)
        {
            if (sender is THUVIENZ.Views.Components.NotificationCard card)
            {
                // Đổ text từ Card sang Popup
                DetailPopup.PopupTitle = card.Title;
                DetailPopup.PopupMessage = card.Message;
                DetailPopup.PopupTimestamp = card.Timestamp;

                // Đồng bộ hóa màu sắc và Icon của Popup khớp với loại thông báo trên Card
                switch (card.NotiType)
                {
                    case Models.NotificationType.Success:
                        DetailPopup.PopupIcon = IconChar.CheckCircle;
                        DetailPopup.IconColor = (Brush)new BrushConverter().ConvertFrom("#137333");
                        DetailPopup.IconBgColor = (Brush)new BrushConverter().ConvertFrom("#E6F4EA");
                        break;
                    case Models.NotificationType.Failure:
                        DetailPopup.PopupIcon = IconChar.TimesCircle;
                        DetailPopup.IconColor = (Brush)new BrushConverter().ConvertFrom("#C5221F");
                        DetailPopup.IconBgColor = (Brush)new BrushConverter().ConvertFrom("#FCE8E6");
                        break;
                    case Models.NotificationType.Warning:
                        DetailPopup.PopupIcon = IconChar.ExclamationTriangle;
                        DetailPopup.IconColor = (Brush)new BrushConverter().ConvertFrom("#B06000");
                        DetailPopup.IconBgColor = (Brush)new BrushConverter().ConvertFrom("#FEF7E0");
                        break;
                    case Models.NotificationType.Info:
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
    }
}