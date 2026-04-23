using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using THUVIENZ.Views.Components;
using THUVIENZ.Views.Popups;

namespace THUVIENZ.Views
{
    public partial class AdminBooks : UserControl
    {
        // Sử dụng ObservableCollection để UI tự động cập nhật khi có thêm/xóa sách
        private ObservableCollection<BookModel> _mockBooks;

        public AdminBooks()
        {
            InitializeComponent();
            LoadMockData();

            // Đăng ký lắng nghe sự kiện từ AddPopup
            AddPopup.OnBookAdded += AddPopup_OnBookAdded;
        }

        private void LoadMockData()
        {
            _mockBooks = new ObservableCollection<BookModel>
            {
                new BookModel { BookID = "DNT-101", Title = "Đắc Nhân Tâm", Author = "Dale Carnegie", Category = "Tâm lý kỹ năng", Status = "Sẵn sàng" },
                new BookModel { BookID = "IT-404", Title = "Clean Code", Author = "Robert C. Martin", Category = "Công nghệ", Status = "Đang mượn" },
                new BookModel { BookID = "NGK-999", Title = "Nhà Giả Kim", Author = "Paulo Coelho", Category = "Tiểu thuyết", Status = "Sẵn sàng" },
                new BookModel { BookID = "ECO-200", Title = "Tâm lý học tội phạm", Author = "Stanton E. Samenow", Category = "Tâm lý học", Status = "Sẵn sàng" }
            };

            // Nạp dữ liệu vào bảng DataGrid
            dgBooks.ItemsSource = _mockBooks;
        }

        // Sự kiện hiển thị Popup
        private void BtnShowAddPopup_Click(object sender, RoutedEventArgs e)
        {
            AddPopup.Visibility = Visibility.Visible;
        }

        // Bắt lấy dữ liệu sách mới từ Popup ném ra
        private void AddPopup_OnBookAdded(object sender, BookModel newBook)
        {
            // Thêm sách vào danh sách. ObservableCollection sẽ tự động vẽ thêm 1 dòng trên DataGrid!
            _mockBooks.Add(newBook);

            MessageBox.Show($"Đã thêm sách '{newBook.Title}' thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}