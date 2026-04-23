using System.Windows;
using System.Windows.Controls;
using THUVIENZ.Views; 

namespace THUVIENZ
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            NavBar.OnNavigate += NavBar_OnNavigate;
            NavBar_OnNavigate(new Profile(), "Profile");
        }

        private void NavBar_OnNavigate(UserControl newPage, string pageName)
        {
            MainContent.Content = newPage;

            NavBar.ActivePage = pageName;

            if (newPage is AdminReaders readersPage)
            {
                readersPage.OnSubNavigate += (subPage) => {
                    MainContent.Content = subPage;
                };
            }
        }
    }
}