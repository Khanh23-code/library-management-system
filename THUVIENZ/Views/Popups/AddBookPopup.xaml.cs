using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using THUVIENZ.Models;

namespace THUVIENZ.Views.Popups
{
    public partial class AddBookPopup : UserControl
    {
        public event EventHandler<BookModel>? OnBookAdded;
        public event EventHandler<Sach>? OnBookUpdated;

        private string? _selectedImagePath;
        private bool _isEditMode = false;
        private int _editBookId = 0;
        private Sach? _editTargetBook;

        public AddBookPopup()
        {
            InitializeComponent();
        }

        public void OpenForAdd()
        {
            _isEditMode = false;
            _editBookId = 0;
            _editTargetBook = null;

            lblPopupTitle.Text = "Thêm Sách Mới";
            btnSaveContent.Content = "Thêm sách";

            txtBookID.Text = "";
            txtTitle.Text = "";
            txtAuthor.Text = "";
            txtCategory.Text = "";
            txtLang.Text = "";
            txtPageNumber.Text = "";
            txtQuantity.Text = "1";
            txtStatus.Text = "Còn sách";
            txtDescription.Text = "";

            _selectedImagePath = null;
            imgPreview.Source = null;
            spPlaceholder.Visibility = Visibility.Visible;
            tbImageName.Text = "Chọn ảnh...";
        }

        public void LoadBookForEdit(Sach book)
        {
            _isEditMode = true;
            _editBookId = book.MaSach;
            _editTargetBook = book;

            lblPopupTitle.Text = "Cập nhật Sách";
            btnSaveContent.Content = "Lưu thay đổi";

            txtBookID.Text = book.MaSach.ToString();
            txtTitle.Text = book.TenSach ?? "";
            txtAuthor.Text = book.TacGia ?? "";
            
            txtCategory.Text = book.MaTheLoai.ToString();
            txtLang.Text = book.NhaXuatBan ?? "";
            txtPageNumber.Text = book.NamXuatBan.ToString();
            txtQuantity.Text = book.SoLuong.ToString();
            txtStatus.Text = book.TinhTrang ?? "Còn sách";
            txtDescription.Text = book.MoTa ?? "";

            _selectedImagePath = null;
            imgPreview.Source = null;
            spPlaceholder.Visibility = Visibility.Visible;
            tbImageName.Text = "Chọn ảnh mới (Tùy chọn)...";

            // Nạp lại ảnh bìa đã thiết lập từ trước (nếu có)
            if (!string.IsNullOrEmpty(book.HinhAnh))
            {
                // Có thể là đường dẫn tuyệt đối hoặc tên file trong thư mục Assets/Images
                string fullPath = book.HinhAnh;
                if (!Path.IsPathRooted(fullPath))
                {
                    fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Images", book.HinhAnh);
                }

                if (File.Exists(fullPath))
                {
                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(fullPath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();

                        imgPreview.Source = bitmap;
                        spPlaceholder.Visibility = Visibility.Collapsed;
                        tbImageName.Text = Path.GetFileName(fullPath);
                        _selectedImagePath = fullPath;
                    }
                    catch { }
                }
            }
        }

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Chọn ảnh bìa sách",
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImagePath = openFileDialog.FileName;
                
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(_selectedImagePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    imgPreview.Source = bitmap;
                    spPlaceholder.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể hiển thị demo ảnh: {ex.Message}", "Lỗi tải ảnh", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                tbImageName.Text = Path.GetFileName(_selectedImagePath);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode && _editTargetBook != null)
            {
                LoadBookForEdit(_editTargetBook);
            }
            else
            {
                OpenForAdd();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên sách.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTitle.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên tác giả.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtAuthor.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Vui lòng nhập Thể loại sách.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCategory.Focus();
                return;
            }

            if (_isEditMode && _editTargetBook != null)
            {
                _editTargetBook.TenSach = txtTitle.Text.Trim();
                _editTargetBook.TacGia = txtAuthor.Text.Trim();
                
                if (int.TryParse(txtCategory.Text.Trim(), out int catId))
                    _editTargetBook.MaTheLoai = catId;
                else
                    _editTargetBook.MaTheLoai = 1;

                _editTargetBook.NhaXuatBan = txtLang.Text.Trim();
                
                if (int.TryParse(txtPageNumber.Text.Trim(), out int yearOrPage))
                    _editTargetBook.NamXuatBan = yearOrPage;

                if (int.TryParse(txtQuantity.Text.Trim(), out int qty))
                    _editTargetBook.SoLuong = qty;

                _editTargetBook.TinhTrang = string.IsNullOrWhiteSpace(txtStatus.Text) ? "Còn sách" : txtStatus.Text.Trim();
                _editTargetBook.MoTa = txtDescription.Text.Trim();

                OnBookUpdated?.Invoke(this, _editTargetBook);
                
                // Nếu có file ảnh bìa được chọn/đổi
                if (!string.IsNullOrEmpty(_selectedImagePath) && File.Exists(_selectedImagePath))
                {
                    // Nếu đường dẫn file khác với hình ảnh hiện tại
                    if (_editTargetBook.HinhAnh != _selectedImagePath)
                    {
                        try
                        {
                            var service = new THUVIENZ.BLL.BookManagementService();
                            using (var stream = new FileStream(_selectedImagePath, FileMode.Open, FileAccess.Read))
                            {
                                string ext = Path.GetExtension(_selectedImagePath);
                                _ = service.UploadBookCoverAsync(_editTargetBook.MaSach, stream, ext);
                            }
                        }
                        catch { }
                    }
                }
            }
            else
            {
                int.TryParse(txtQuantity.Text.Trim(), out int qty);
                var newBook = new BookModel
                {
                    BookID = "NEW",
                    Title = txtTitle.Text.Trim(),
                    Author = txtAuthor.Text.Trim(),
                    Category = txtCategory.Text.Trim(),
                    Language = string.IsNullOrWhiteSpace(txtLang.Text) ? "Tiếng Việt" : txtLang.Text.Trim(),
                    PageNumber = string.IsNullOrWhiteSpace(txtPageNumber.Text) ? "2026" : txtPageNumber.Text.Trim(),
                    Quantity = qty > 0 ? qty : 1,
                    Status = string.IsNullOrWhiteSpace(txtStatus.Text) ? "Còn sách" : txtStatus.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    ImagePath = _selectedImagePath
                };

                OnBookAdded?.Invoke(this, newBook);
            }

            this.Visibility = Visibility.Collapsed;
        }
    }

    public class BookModel
    {
        public string BookID { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string PageNumber { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
    }
}