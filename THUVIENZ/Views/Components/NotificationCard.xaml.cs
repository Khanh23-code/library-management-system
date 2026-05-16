using System;
using System.Windows;
using System.Windows.Controls;
using THUVIENZ.Models;

namespace THUVIENZ.Views.Components
{
    public partial class NotificationCard : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(NotificationCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(NotificationCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty TimestampProperty =
            DependencyProperty.Register("Timestamp", typeof(string), typeof(NotificationCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty NotiTypeProperty =
            DependencyProperty.Register("NotiType", typeof(NotificationType), typeof(NotificationCard), new PropertyMetadata(NotificationType.Info));

        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        public string Message { get => (string)GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
        public string Timestamp { get => (string)GetValue(TimestampProperty); set => SetValue(TimestampProperty, value); }
        public NotificationType NotiType { get => (NotificationType)GetValue(NotiTypeProperty); set => SetValue(NotiTypeProperty, value); }

        // Đăng ký RoutedEvent cho nút Xóa
        public static readonly RoutedEvent CloseRequestedEvent =
            EventManager.RegisterRoutedEvent("OnCloseRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotificationCard));

        public event RoutedEventHandler OnCloseRequested
        {
            add { AddHandler(CloseRequestedEvent, value); }
            remove { RemoveHandler(CloseRequestedEvent, value); }
        }

        // Đăng ký RoutedEvent cho việc Click vào Card để xem chi tiết
        public static readonly RoutedEvent CardClickedEvent =
            EventManager.RegisterRoutedEvent("OnCardClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotificationCard));

        public event RoutedEventHandler OnCardClicked
        {
            add { AddHandler(CardClickedEvent, value); }
            remove { RemoveHandler(CardClickedEvent, value); }
        }

        public NotificationCard()
        {
            InitializeComponent();
        }

        // Xử lý khi click vào nút X
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true; // Ngăn chặn sự kiện click lan xuống Card_MouseLeftButtonUp
            RaiseEvent(new RoutedEventArgs(CloseRequestedEvent));
        }

        // Xử lý khi click vào phần nền của Card
        private void Card_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CardClickedEvent));
        }
    }
}