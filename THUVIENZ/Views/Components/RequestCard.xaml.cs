using System;
using System.Windows;
using System.Windows.Controls;

namespace THUVIENZ.Views.Components
{
    public partial class RequestCard : UserControl
    {
        // 1. Tên
        public static readonly DependencyProperty ReaderNameProperty =
            DependencyProperty.Register("ReaderName", typeof(string), typeof(RequestCard), new PropertyMetadata("Tên độc giả"));
        public string ReaderName { get { return (string)GetValue(ReaderNameProperty); } set { SetValue(ReaderNameProperty, value); } }

        // 2. Địa chỉ
        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(string), typeof(RequestCard), new PropertyMetadata("Địa chỉ"));
        public string Address { get { return (string)GetValue(AddressProperty); } set { SetValue(AddressProperty, value); } }

        // 3. Giới tính
        public static readonly DependencyProperty GenderProperty =
            DependencyProperty.Register("Gender", typeof(string), typeof(RequestCard), new PropertyMetadata("Nam/Nữ"));
        public string Gender { get { return (string)GetValue(GenderProperty); } set { SetValue(GenderProperty, value); } }

        // 4. Email
        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register("Email", typeof(string), typeof(RequestCard), new PropertyMetadata("email@domain.com"));
        public string Email { get { return (string)GetValue(EmailProperty); } set { SetValue(EmailProperty, value); } }

        // 5. Số điện thoại
        public static readonly DependencyProperty PhoneProperty =
            DependencyProperty.Register("Phone", typeof(string), typeof(RequestCard), new PropertyMetadata("0000000000"));
        public string Phone { get { return (string)GetValue(PhoneProperty); } set { SetValue(PhoneProperty, value); } }

        // 6. Đường dẫn ảnh đại diện (ĐÂY LÀ BIẾN SẼ FIX LỖI CỦA BẠN)
        public static readonly DependencyProperty AvatarSourceProperty =
            DependencyProperty.Register("AvatarSource", typeof(string), typeof(RequestCard), new PropertyMetadata("/Assets/phai.png"));
        public string AvatarSource { get { return (string)GetValue(AvatarSourceProperty); } set { SetValue(AvatarSourceProperty, value); } }


        // Sự kiện báo ra ngoài
        public event EventHandler<RequestCard> OnAcceptClick;
        public event EventHandler<RequestCard> OnRejectClick;

        public RequestCard()
        {
            InitializeComponent();
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            OnAcceptClick?.Invoke(this, this);
        }

        private void BtnReject_Click(object sender, RoutedEventArgs e)
        {
            OnRejectClick?.Invoke(this, this);
        }
    }
}