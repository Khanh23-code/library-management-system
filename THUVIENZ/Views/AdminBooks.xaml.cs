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
            
            _viewModel = new BookManagementViewModel();
            this.DataContext = _viewModel;

            AddPopup.OnBookAdded += AddPopup_OnBookAdded;
            AddPopup.OnBookUpdated += AddPopup_OnBookUpdated;
        }

        private void BtnShowAddPopup_Click(object sender, RoutedEventArgs e)
        {
            AddPopup.OpenForAdd();
            AddPopup.Visibility = Visibility.Visible;
        }

        private async void BtnEditRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is THUVIENZ.Models.Sach bookRow)
            {
                try
                {
                    // Load data mới nhất từ database theo yêu cầu
                    var service = new THUVIENZ.BLL.BookManagementService();
                    var latestBook = await service.GetByIdAsync(bookRow.MaSach);
                    if (latestBook != null)
                    {
                        AddPopup.LoadBookForEdit(latestBook);
                        AddPopup.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin sách trong cơ sở dữ liệu.", "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Lỗi tải thông tin sách: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void AddPopup_OnBookAdded(object? sender, BookModel newBook)
        {
            try
            {
                var service = new THUVIENZ.BLL.BookManagementService();
                int.TryParse(newBook.PageNumber, out int year);
                var sach = new THUVIENZ.Models.Sach
                {
                    TenSach = newBook.Title,
                    TacGia = newBook.Author,
                    NhaXuatBan = newBook.Language,
                    NamXuatBan = year > 0 ? year : System.DateTime.Now.Year,
                    SoLuong = newBook.Quantity > 0 ? newBook.Quantity : 1,
                    TinhTrang = string.IsNullOrWhiteSpace(newBook.Status) ? "Còn sách" : newBook.Status,
                    MoTa = newBook.Description,
                    TriGia = 100000,
                    NgayNhap = System.DateTime.Now,
                    MaTheLoai = 1
                };

                // Nếu nhập mã thể loại là số
                if (int.TryParse(newBook.Category, out int catId))
                    sach.MaTheLoai = catId;

                await service.AddBookAsync(sach);

                if (!string.IsNullOrEmpty(newBook.ImagePath) && System.IO.File.Exists(newBook.ImagePath))
                {
                    using (var stream = new System.IO.FileStream(newBook.ImagePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        string ext = System.IO.Path.GetExtension(newBook.ImagePath);
                        await service.UploadBookCoverAsync(sach.MaSach, stream, ext);
                    }
                }

                MessageBox.Show("Thêm sách và tải ảnh bìa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                _viewModel.LoadBooksCommand.Execute(null);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi xử lý thêm sách: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddPopup_OnBookUpdated(object? sender, THUVIENZ.Models.Sach updatedBook)
        {
            try
            {
                var service = new THUVIENZ.BLL.BookManagementService();
                await service.UpdateAsync(updatedBook);

                MessageBox.Show("Cập nhật thông tin sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                _viewModel.LoadBooksCommand.Execute(null);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi xử lý cập nhật sách: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}