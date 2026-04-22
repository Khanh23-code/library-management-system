using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace THUVIENZ.Views
{
    /// <summary>
    /// Interaction logic for AdminBooks.xaml
    /// </summary>
    public partial class AdminBooks : UserControl
    {
        public AdminBooks()
        {
            InitializeComponent();
        }

        private void BtnShowAddPopup_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị overlay popup
            AddPopup.Visibility = Visibility.Visible;
        }
    }
}
