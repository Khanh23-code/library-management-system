using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace THUVIENZ.Views.Components
{
    public partial class RequestCard : UserControl
    {
        public static readonly DependencyProperty ReaderNameProperty =
            DependencyProperty.Register("ReaderName", typeof(string), typeof(RequestCard), new PropertyMetadata("Tên độc giả"));
        public string ReaderName { get { return (string)GetValue(ReaderNameProperty); } set { SetValue(ReaderNameProperty, value); } }

        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(string), typeof(RequestCard), new PropertyMetadata("Địa chỉ"));
        public string Address { get { return (string)GetValue(AddressProperty); } set { SetValue(AddressProperty, value); } }

        public static readonly DependencyProperty GenderProperty =
            DependencyProperty.Register("Gender", typeof(string), typeof(RequestCard), new PropertyMetadata("Nam/Nữ"));
        public string Gender { get { return (string)GetValue(GenderProperty); } set { SetValue(GenderProperty, value); } }

        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register("Email", typeof(string), typeof(RequestCard), new PropertyMetadata("email@domain.com"));
        public string Email { get { return (string)GetValue(EmailProperty); } set { SetValue(EmailProperty, value); } }

        public static readonly DependencyProperty PhoneProperty =
            DependencyProperty.Register("Phone", typeof(string), typeof(RequestCard), new PropertyMetadata("0000000000"));
        public string Phone { get { return (string)GetValue(PhoneProperty); } set { SetValue(PhoneProperty, value); } }

        public static readonly DependencyProperty AvatarSourceProperty =
            DependencyProperty.Register("AvatarSource", typeof(string), typeof(RequestCard), new PropertyMetadata("/Assets/phai.png"));
        public string AvatarSource { get { return (string)GetValue(AvatarSourceProperty); } set { SetValue(AvatarSourceProperty, value); } }

        // MVVM Command support
        public static readonly DependencyProperty AcceptCommandProperty =
            DependencyProperty.Register("AcceptCommand", typeof(ICommand), typeof(RequestCard), new PropertyMetadata(null));
        public ICommand AcceptCommand { get { return (ICommand)GetValue(AcceptCommandProperty); } set { SetValue(AcceptCommandProperty, value); } }

        public static readonly DependencyProperty AcceptCommandParameterProperty =
            DependencyProperty.Register("AcceptCommandParameter", typeof(object), typeof(RequestCard), new PropertyMetadata(null));
        public object AcceptCommandParameter { get { return GetValue(AcceptCommandParameterProperty); } set { SetValue(AcceptCommandParameterProperty, value); } }

        public static readonly DependencyProperty RejectCommandProperty =
            DependencyProperty.Register("RejectCommand", typeof(ICommand), typeof(RequestCard), new PropertyMetadata(null));
        public ICommand RejectCommand { get { return (ICommand)GetValue(RejectCommandProperty); } set { SetValue(RejectCommandProperty, value); } }

        public static readonly DependencyProperty RejectCommandParameterProperty =
            DependencyProperty.Register("RejectCommandParameter", typeof(object), typeof(RequestCard), new PropertyMetadata(null));
        public object RejectCommandParameter { get { return GetValue(RejectCommandParameterProperty); } set { SetValue(RejectCommandParameterProperty, value); } }

        public RequestCard()
        {
            InitializeComponent();
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (AcceptCommand != null && AcceptCommand.CanExecute(AcceptCommandParameter))
            {
                AcceptCommand.Execute(AcceptCommandParameter);
            }
        }

        private void BtnReject_Click(object sender, RoutedEventArgs e)
        {
            if (RejectCommand != null && RejectCommand.CanExecute(RejectCommandParameter))
            {
                RejectCommand.Execute(RejectCommandParameter);
            }
        }
    }
}