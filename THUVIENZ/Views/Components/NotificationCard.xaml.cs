using System;
using System.Windows;
using System.Windows.Controls;
using THUVIENZ.Models;

namespace THUVIENZ.Views.Components
{
    public partial class NotificationCard : UserControl
    {
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(NotificationCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty TimestampProperty =
            DependencyProperty.Register("Timestamp", typeof(string), typeof(NotificationCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty NotiTypeProperty =
            DependencyProperty.Register("NotiType", typeof(NotificationType), typeof(NotificationCard), new PropertyMetadata(NotificationType.Info));

        public string Message { get => (string)GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
        public string Timestamp { get => (string)GetValue(TimestampProperty); set => SetValue(TimestampProperty, value); }
        public NotificationType NotiType { get => (NotificationType)GetValue(NotiTypeProperty); set => SetValue(NotiTypeProperty, value); }

        // SỬA TẠI ĐÂY: Đăng ký RoutedEvent để XAML có thể nhận diện được thuộc tính OnCloseRequested
        public static readonly RoutedEvent CloseRequestedEvent =
            EventManager.RegisterRoutedEvent("OnCloseRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotificationCard));

        public event RoutedEventHandler OnCloseRequested
        {
            add { AddHandler(CloseRequestedEvent, value); }
            remove { RemoveHandler(CloseRequestedEvent, value); }
        }

        public NotificationCard()
        {
            InitializeComponent();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Kích hoạt RoutedEvent lên UI cha
            RaiseEvent(new RoutedEventArgs(CloseRequestedEvent));
        }
    }
}