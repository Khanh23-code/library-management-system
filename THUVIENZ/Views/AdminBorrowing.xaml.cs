using System.Collections.Generic;
using System.Windows.Controls;

namespace THUVIENZ.Views
{
    public partial class AdminBorrowing : UserControl
    {
        // Danh sách lưu trữ dữ liệu giả lập
        private List<BorrowingReader> _mockReaders;

        public AdminBorrowing()
        {
            InitializeComponent();
            LoadMockData();
        }

        private void LoadMockData()
        {
            // 1. Tạo dữ liệu giả
            _mockReaders = new List<BorrowingReader>
            {
                new BorrowingReader
                {
                    ReaderID = "RD-20511", ReaderName = "Nguyễn Tân Binh", TotalBorrowed = 2,
                    Books = new List<BorrowedBook>
                    {
                        new BorrowedBook { BookID = "IT-404", Title = "Clean Code", BorrowDate = "15/04/2026", DueDate = "22/04/2026" },
                        new BorrowedBook { BookID = "IT-102", Title = "Design Patterns", BorrowDate = "15/04/2026", DueDate = "22/04/2026" }
                    }
                },
                new BorrowingReader
                {
                    ReaderID = "RD-20512", ReaderName = "Trần Thị B", TotalBorrowed = 1,
                    Books = new List<BorrowedBook>
                    {
                        new BorrowedBook { BookID = "NGK-999", Title = "Nhà Giả Kim", BorrowDate = "20/04/2026", DueDate = "27/04/2026" }
                    }
                },
                new BorrowingReader
                {
                    ReaderID = "RD-20513", ReaderName = "Lê Văn C", TotalBorrowed = 3,
                    Books = new List<BorrowedBook>
                    {
                        new BorrowedBook { BookID = "DNT-101", Title = "Đắc Nhân Tâm", BorrowDate = "10/04/2026", DueDate = "17/04/2026" },
                        new BorrowedBook { BookID = "ECO-200", Title = "Tâm lý học tội phạm", BorrowDate = "12/04/2026", DueDate = "19/04/2026" },
                        new BorrowedBook { BookID = "NOV-333", Title = "Hai vạn dặm dưới đáy biển", BorrowDate = "12/04/2026", DueDate = "19/04/2026" }
                    }
                }
            };

            // 2. Nạp danh sách Độc giả vào bảng bên trái
            dgReaders.ItemsSource = _mockReaders;

            // 3. Mặc định chọn dòng đầu tiên khi mới mở trang
            if (_mockReaders.Count > 0)
            {
                dgReaders.SelectedIndex = 0;
            }
        }

        // Sự kiện khi Admin click chọn 1 độc giả ở bảng bên trái
        private void dgReaders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lấy dòng dữ liệu đang được chọn
            if (dgReaders.SelectedItem is BorrowingReader selectedReader)
            {
                // Cập nhật tiêu đề bên phải
                txtSelectedReader.Text = $"Độc giả: {selectedReader.ReaderName} ({selectedReader.ReaderID})";

                // Đổ danh sách sách mượn của người này vào bảng bên phải
                dgBooks.ItemsSource = selectedReader.Books;
            }
        }
    }

    // --- CÁC CLASS ĐẠI DIỆN DATA (MODELS) ---
    public class BorrowingReader
    {
        public string ReaderID { get; set; }
        public string ReaderName { get; set; }
        public int TotalBorrowed { get; set; }

        // Chứa danh sách các cuốn sách mà người này mượn
        public List<BorrowedBook> Books { get; set; }
    }

    public class BorrowedBook
    {
        public string BookID { get; set; }
        public string Title { get; set; }
        public string BorrowDate { get; set; }
        public string DueDate { get; set; }
    }
}