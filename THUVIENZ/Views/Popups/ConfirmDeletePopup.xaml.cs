using System;
using System.Windows;
using System.Windows.Controls;

namespace THUVIENZ.Views.Popups
{
    public partial class ConfirmDeletePopup : UserControl
    {
        // 1. Tiêu đề Popup
        public static readonly DependencyProperty PopupTitleProperty =
            DependencyProperty.Register("PopupTitle", typeof(string), typeof(ConfirmDeletePopup), new PropertyMetadata("Xác nhận xóa"));

        public string PopupTitle
        {
            get { return (string)GetValue(PopupTitleProperty); }
            set { SetValue(PopupTitleProperty, value); }
        }

        // 2. Nội dung lời nhắn
        public static readonly DependencyProperty PopupMessageProperty =
            DependencyProperty.Register("PopupMessage", typeof(string), typeof(ConfirmDeletePopup), new PropertyMetadata("Bạn có chắc chắn muốn xóa mục này không? Hành động này không thể hoàn tác."));

        public string PopupMessage
        {
            get { return (string)GetValue(PopupMessageProperty); }
            set { SetValue(PopupMessageProperty, value); }
        }

        // 3. Sự kiện báo ra ngoài khi user bấm nút "Xóa vĩnh viễn"
        public event EventHandler? OnConfirm;

        public ConfirmDeletePopup()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Đóng popup
            this.Visibility = Visibility.Collapsed;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            // Báo cho Form cha biết để chạy code xóa trong CSDL
            OnConfirm?.Invoke(this, EventArgs.Empty);

            // Xóa xong thì đóng popup
            this.Visibility = Visibility.Collapsed;
        }
    }
}