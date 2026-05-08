using System.Windows;
using System.Windows.Controls;
using THUVIENZ.ViewModels;
using THUVIENZ.Views.Popups;

namespace THUVIENZ.Views
{
    public partial class AdminBooks : UserControl
    {
        private readonly BookManagementViewModel _viewModel;

        public AdminBooks()
        {
            InitializeComponent();
            
            // Khởi tạo ViewModel và gán DataContext để Binding hoạt động
            _viewModel = new BookManagementViewModel();
            this.DataContext = _viewModel;

            // Đăng ký lắng nghe sự kiện từ AddPopup (Nếu cần tích hợp logic thêm sách)
            AddPopup.OnBookAdded += AddPopup_OnBookAdded;
        }

        private void BtnShowAddPopup_Click(object sender, RoutedEventArgs e)
        {
            AddPopup.Visibility = Visibility.Visible;
        }

        private void AddPopup_OnBookAdded(object sender, BookModel newBook)
        {
            // Tạm thời giữ lại để không lỗi UI, nhưng thực tế nên gọi ViewModel.AddCommand
            // Trong bản nâng cấp tiếp theo, AddPopup sẽ binding trực tiếp vào ViewModel
            AddPopup.Visibility = Visibility.Collapsed;
            _viewModel.LoadBooksCommand.Execute(null);
        }
    }
}