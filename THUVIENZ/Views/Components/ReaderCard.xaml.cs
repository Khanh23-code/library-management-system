using System.Windows;
using System.Windows.Controls;

namespace THUVIENZ.Views.Components
{
    public partial class ReaderCard : UserControl
    {
        public ReaderCard()
        {
            InitializeComponent();
        }

        // 1. Tên
        public static readonly DependencyProperty ReaderNameProperty =
            DependencyProperty.Register("ReaderName", typeof(string), typeof(ReaderCard), new PropertyMetadata("Tên độc giả"));
        public string ReaderName { get { return (string)GetValue(ReaderNameProperty); } set { SetValue(ReaderNameProperty, value); } }

        // 2. Địa chỉ
        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(string), typeof(ReaderCard), new PropertyMetadata("Địa chỉ"));
        public string Address { get { return (string)GetValue(AddressProperty); } set { SetValue(AddressProperty, value); } }

        // 3. Mã Độc giả
        public static readonly DependencyProperty ReaderIdProperty =
            DependencyProperty.Register("ReaderId", typeof(string), typeof(ReaderCard), new PropertyMetadata("RD-0000"));
        public string ReaderId { get { return (string)GetValue(ReaderIdProperty); } set { SetValue(ReaderIdProperty, value); } }

        // 4. Giới tính
        public static readonly DependencyProperty GenderProperty =
            DependencyProperty.Register("Gender", typeof(string), typeof(ReaderCard), new PropertyMetadata("Nam/Nữ"));
        public string Gender { get { return (string)GetValue(GenderProperty); } set { SetValue(GenderProperty, value); } }

        // 5. Email
        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register("Email", typeof(string), typeof(ReaderCard), new PropertyMetadata("email@domain.com"));
        public string Email { get { return (string)GetValue(EmailProperty); } set { SetValue(EmailProperty, value); } }

        // 6. Số điện thoại
        public static readonly DependencyProperty PhoneProperty =
            DependencyProperty.Register("Phone", typeof(string), typeof(ReaderCard), new PropertyMetadata("0000000000"));
        public string Phone { get { return (string)GetValue(PhoneProperty); } set { SetValue(PhoneProperty, value); } }

        // 7. Đường dẫn ảnh đại diện
        public static readonly DependencyProperty AvatarSourceProperty =
            DependencyProperty.Register("AvatarSource", typeof(string), typeof(ReaderCard), new PropertyMetadata("/Assets/phai.png"));

        public string AvatarSource
        {
            get { return (string)GetValue(AvatarSourceProperty); }
            set { SetValue(AvatarSourceProperty, value); }
        }
        public event EventHandler<ReaderCard> OnDeleteClick;

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Báo hiệu ra bên ngoài và truyền chính UserControl này ra
            OnDeleteClick?.Invoke(this, this);
        }
    }
}