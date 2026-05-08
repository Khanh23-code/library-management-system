using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace THUVIENZ.Views.Components
{
    public partial class ReaderCard : UserControl
    {
        public static readonly DependencyProperty ReaderNameProperty =
            DependencyProperty.Register("ReaderName", typeof(string), typeof(ReaderCard), new PropertyMetadata("Tên độc giả"));
        public string ReaderName { get { return (string)GetValue(ReaderNameProperty); } set { SetValue(ReaderNameProperty, value); } }

        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(string), typeof(ReaderCard), new PropertyMetadata("Địa chỉ"));
        public string Address { get { return (string)GetValue(AddressProperty); } set { SetValue(AddressProperty, value); } }

        public static readonly DependencyProperty ReaderIdProperty =
            DependencyProperty.Register("ReaderId", typeof(string), typeof(ReaderCard), new PropertyMetadata("RD-0000"));
        public string ReaderId { get { return (string)GetValue(ReaderIdProperty); } set { SetValue(ReaderIdProperty, value); } }

        public static readonly DependencyProperty GenderProperty =
            DependencyProperty.Register("Gender", typeof(string), typeof(ReaderCard), new PropertyMetadata("Nam/Nữ"));
        public string Gender { get { return (string)GetValue(GenderProperty); } set { SetValue(GenderProperty, value); } }

        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register("Email", typeof(string), typeof(ReaderCard), new PropertyMetadata("email@domain.com"));
        public string Email { get { return (string)GetValue(EmailProperty); } set { SetValue(EmailProperty, value); } }

        public static readonly DependencyProperty PhoneProperty =
            DependencyProperty.Register("Phone", typeof(string), typeof(ReaderCard), new PropertyMetadata("0000000000"));
        public string Phone { get { return (string)GetValue(PhoneProperty); } set { SetValue(PhoneProperty, value); } }

        public static readonly DependencyProperty AvatarSourceProperty =
            DependencyProperty.Register("AvatarSource", typeof(string), typeof(ReaderCard), new PropertyMetadata("/Assets/phai.png"));
        public string AvatarSource { get { return (string)GetValue(AvatarSourceProperty); } set { SetValue(AvatarSourceProperty, value); } }

        // MVVM Command support
        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(ReaderCard), new PropertyMetadata(null));
        public ICommand DeleteCommand { get { return (ICommand)GetValue(DeleteCommandProperty); } set { SetValue(DeleteCommandProperty, value); } }

        public static readonly DependencyProperty DeleteCommandParameterProperty =
            DependencyProperty.Register("DeleteCommandParameter", typeof(object), typeof(ReaderCard), new PropertyMetadata(null));
        public object DeleteCommandParameter { get { return GetValue(DeleteCommandParameterProperty); } set { SetValue(DeleteCommandParameterProperty, value); } }

        public ReaderCard()
        {
            InitializeComponent();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteCommand != null && DeleteCommand.CanExecute(DeleteCommandParameter))
            {
                DeleteCommand.Execute(DeleteCommandParameter);
            }
        }
    }
}