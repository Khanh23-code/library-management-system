using System.Windows;
using System.Windows.Controls;

namespace THUVIENZ.Views.Popups
{
    public partial class AddBookPopup : UserControl
    {
        public AddBookPopup()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            // Ẩn chính popup này đi
            this.Visibility = Visibility.Collapsed;
        }

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            // Logic hoàn tác sẽ xử lý sau, tạm thời ta có thể làm trống các TextBox ở đây
        }
    }
}