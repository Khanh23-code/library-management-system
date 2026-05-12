using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using THUVIENZ.BLL;
using THUVIENZ.Core;
using THUVIENZ.Models;

namespace THUVIENZ.ViewModels
{
    public class BookManagementViewModel : ObservableObject
    {
        private ObservableCollection<Sach> _booksList = new ObservableCollection<Sach>();
        public ObservableCollection<Sach> BooksList
        {
            get => _booksList;
            set
            {
                _booksList = value;
                OnPropertyChanged();
            }
        }

        private Sach? _selectedBook;
        public Sach? SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged();
            }
        }

        private string _searchKeyword = string.Empty;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                ExecuteSearch();
            }
        }

        public ICommand LoadBooksCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        private readonly BookManagementService _bookService;

        public BookManagementViewModel()
        {
            _bookService = new BookManagementService();

            LoadBooksCommand = new RelayCommand(_ => LoadAllBooks());
            AddCommand = new RelayCommand(_ => ExecuteAdd());
            UpdateCommand = new RelayCommand(param => ExecuteUpdate(param));
            DeleteCommand = new RelayCommand(param => ExecuteDelete(param));

            LoadAllBooks();
        }

        public async void LoadAllBooks()
        {
            try
            {
                var books = await _bookService.GetAllAsync();
                BooksList = new ObservableCollection<Sach>(books);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách sách: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ExecuteSearch()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchKeyword))
                {
                    var books = await _bookService.GetAllAsync();
                    BooksList = new ObservableCollection<Sach>(books);
                }
                else
                {
                    var repo = new THUVIENZ.DAL.SachRepository();
                    var books = await repo.SearchBooksAsync(SearchKeyword);
                    BooksList = new ObservableCollection<Sach>(books);
                }
            }
            catch (Exception)
            {
                // Bỏ qua lỗi nhỏ khi gõ nhanh
            }
        }

        private async void ExecuteAdd()
        {
            if (SelectedBook == null || string.IsNullOrWhiteSpace(SelectedBook.TenSach))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sách mới.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _bookService.AddBookAsync(SelectedBook);
                MessageBox.Show("Thêm sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadAllBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi nghiệp vụ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ExecuteUpdate(object? param)
        {
            var targetBook = param as Sach ?? SelectedBook;
            if (targetBook == null)
            {
                MessageBox.Show("Vui lòng chọn một cuốn sách để cập nhật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _bookService.UpdateAsync(targetBook);
                MessageBox.Show("Cập nhật thông tin sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadAllBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi nghiệp vụ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ExecuteDelete(object? param)
        {
            var targetBook = param as Sach ?? SelectedBook;
            if (targetBook == null)
            {
                MessageBox.Show("Vui lòng chọn một cuốn sách để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa sách '{targetBook.TenSach}' (Mã: {targetBook.MaSach}) không?", 
                                        "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (confirm == MessageBoxResult.No) return;

            try
            {
                await _bookService.DeleteBookAsync(targetBook.MaSach);
                MessageBox.Show("Xóa sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadAllBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Không thể xóa", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }
}
