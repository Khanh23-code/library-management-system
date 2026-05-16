using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FontAwesome.Sharp;

namespace THUVIENZ.Views.Popups
{
    public partial class NotificationDetailPopup : UserControl
    {
        public static readonly DependencyProperty PopupTitleProperty = DependencyProperty.Register("PopupTitle", typeof(string), typeof(NotificationDetailPopup));
        public static readonly DependencyProperty PopupMessageProperty = DependencyProperty.Register("PopupMessage", typeof(string), typeof(NotificationDetailPopup));
        public static readonly DependencyProperty PopupTimestampProperty = DependencyProperty.Register("PopupTimestamp", typeof(string), typeof(NotificationDetailPopup));
        public static readonly DependencyProperty PopupIconProperty = DependencyProperty.Register("PopupIcon", typeof(IconChar), typeof(NotificationDetailPopup));
        public static readonly DependencyProperty IconColorProperty = DependencyProperty.Register("IconColor", typeof(Brush), typeof(NotificationDetailPopup));
        public static readonly DependencyProperty IconBgColorProperty = DependencyProperty.Register("IconBgColor", typeof(Brush), typeof(NotificationDetailPopup));

        public string PopupTitle { get => (string)GetValue(PopupTitleProperty); set => SetValue(PopupTitleProperty, value); }
        public string PopupMessage { get => (string)GetValue(PopupMessageProperty); set => SetValue(PopupMessageProperty, value); }
        public string PopupTimestamp { get => (string)GetValue(PopupTimestampProperty); set => SetValue(PopupTimestampProperty, value); }
        public IconChar PopupIcon { get => (IconChar)GetValue(PopupIconProperty); set => SetValue(PopupIconProperty, value); }
        public Brush IconColor { get => (Brush)GetValue(IconColorProperty); set => SetValue(IconColorProperty, value); }
        public Brush IconBgColor { get => (Brush)GetValue(IconBgColorProperty); set => SetValue(IconBgColorProperty, value); }

        public NotificationDetailPopup() { InitializeComponent(); }

        private void BtnClose_Click(object sender, RoutedEventArgs e) { this.Visibility = Visibility.Collapsed; }
        private void Background_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) { this.Visibility = Visibility.Collapsed; }
        private void Border_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) { e.Handled = true; } // Click vào thẻ trắng ko bị tắt
    }
}