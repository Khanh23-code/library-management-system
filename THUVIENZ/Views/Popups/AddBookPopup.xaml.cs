using System;
using System.Windows;
using System.Windows.Controls;

namespace THUVIENZ.Views.Popups
{
    public partial class AddBookPopup : UserControl
    {
        // Sự kiện báo ra ngoài kèm theo dữ liệu cuốn sách mới
        public event EventHandler<BookModel> OnBookAdded;

        public AddBookPopup()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            // Làm trống các ô nhập liệu
            txtTitle.Text = "";
            txtBookID.Text = "";
            txtAuthor.Text = "";
            txtCategory.Text = "";
            txtLang.Text = "";
            txtPageNumber.Text = "";
            txtDescription.Text = "";
        }

        // Sự kiện khi bấm nút "Thêm sách" (Màu xanh)
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. Gom dữ liệu từ các TextBox
            var newBook = new BookModel
            {
                BookID = string.IsNullOrEmpty(txtBookID.Text) ? "NEW-000" : txtBookID.Text,
                Title = string.IsNullOrEmpty(txtTitle.Text) ? "Sách chưa đặt tên" : txtTitle.Text,
                Author = string.IsNullOrEmpty(txtAuthor.Text) ? "Đang cập nhật" : txtAuthor.Text,
                Category = string.IsNullOrEmpty(txtCategory.Text) ? "Khác" : txtCategory.Text,
                Language = string.IsNullOrEmpty(txtLang.Text) ? "Đang cập nhật" : txtLang.Text,
                PageNumber = string.IsNullOrEmpty(txtPageNumber.Text) ? "0" : txtPageNumber.Text,
                Description = string.IsNullOrEmpty(txtDescription.Text) ? "Không có mô tả" : txtDescription.Text,
                Status = "Sẵn sàng" // Sách mới auto sẵn sàng
            };

            // 2. Bắn sự kiện ra ngoài cho AdminBooks
            OnBookAdded?.Invoke(this, newBook);

            // 3. Đóng popup và xóa form
            BtnUndo_Click(null, null);
            this.Visibility = Visibility.Collapsed;
        }
    }

    // Model dữ liệu chung cho Sách
    public class BookModel
    {
        public string BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string PageNumber { get; set; }
        public string Language { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}