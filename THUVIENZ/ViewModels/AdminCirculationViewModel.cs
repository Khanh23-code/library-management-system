using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using THUVIENZ.BLL;
using THUVIENZ.Core;
using THUVIENZ.DAL;

namespace THUVIENZ.ViewModels
{
    /// <summary>
    /// ViewModel quản lý giao diện Circulation (Mượn/Trả) dành cho Admin.
    /// Kết nối trực tiếp với MuonTraService theo đúng cấu trúc DB chuẩn hóa mới.
    /// Áp dụng Strict Null Safety tuyệt đối và chú thích Tiếng Việt.
    /// </summary>
    public class AdminCirculationViewModel : ObservableObject
    {
        private readonly LmsDbContext _context;
        private readonly MuonTraService _muonTraService;

        private bool _isHistoryTab;
        public bool IsHistoryTab
        {
            get => _isHistoryTab;
            set
            {
                _isHistoryTab = value;
                OnPropertyChanged();
                _ = LoadReadersAsync();
                BorrowedBooks.Clear();
            }
        }

        private ObservableCollection<DocGiaWithBorrowCount> _readersList = new ObservableCollection<DocGiaWithBorrowCount>();
        public ObservableCollection<DocGiaWithBorrowCount> ReadersList
        {
            get => _readersList;
            set { _readersList = value; OnPropertyChanged(); }
        }

        private DocGiaWithBorrowCount? _selectedReader;
        public DocGiaWithBorrowCount? SelectedReader
        {
            get => _selectedReader;
            set 
            { 
                _selectedReader = value; 
                OnPropertyChanged();
                if (_selectedReader != null) _ = LoadBorrowedBooksAsync(_selectedReader.MaDocGia);
            }
        }

        private ObservableCollection<BorrowedBookInfo> _borrowedBooks = new ObservableCollection<BorrowedBookInfo>();
        public ObservableCollection<BorrowedBookInfo> BorrowedBooks
        {
            get => _borrowedBooks;
            set { _borrowedBooks = value; OnPropertyChanged(); }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand ReturnBookCommand { get; }

        public AdminCirculationViewModel()
        {
            _context = new LmsDbContext();
            _muonTraService = new MuonTraService();

            LoadDataCommand = new RelayCommand(async _ => await LoadReadersAsync());
            ReturnBookCommand = new RelayCommand(async param => await ExecuteReturnAsync(param));

            _ = LoadReadersAsync();
        }

        /// <summary>
        /// Nạp danh sách các độc giả đang có giao dịch tương ứng với Tab hiện tại.
        /// </summary>
        public async Task LoadReadersAsync()
        {
            bool isReturnedFilter = IsHistoryTab;

            var readers = await _context.DocGias
                .Select(d => new DocGiaWithBorrowCount
                {
                    MaDocGia = d.MaDocGia,
                    HoTen = d.HoTen,
                    BorrowCount = _context.ChiTietMuonTras
                        .Include(c => c.PhieuMuon)
                        .Count(c => c.PhieuMuon!.MaDocGia == d.MaDocGia && (isReturnedFilter ? c.NgayTraThucTe != null : c.NgayTraThucTe == null))
                })
                .Where(x => x.BorrowCount > 0)
                .ToListAsync();

            ReadersList = new ObservableCollection<DocGiaWithBorrowCount>(readers);
        }

        /// <summary>
        /// Nạp danh sách chi tiết các cuốn sách vật lý mượn/trả của độc giả được chọn.
        /// </summary>
        public async Task LoadBorrowedBooksAsync(int readerId)
        {
            bool isReturnedFilter = IsHistoryTab;

            var books = await _context.ChiTietMuonTras
                .Include(c => c.PhieuMuon)
                .Include(c => c.CuonSach)
                .ThenInclude(cs => cs!.Sach)
                .Where(c => c.PhieuMuon!.MaDocGia == readerId && (isReturnedFilter ? c.NgayTraThucTe != null : c.NgayTraThucTe == null))
                .Select(c => new BorrowedBookInfo
                {
                    MaSach = c.MaCuonSach, // Ánh xạ sang MaCuonSach để hiển thị đúng ID vật lý trên cột DataGrid XAML
                    MaCuonSach = c.MaCuonSach,
                    TenSach = c.CuonSach!.Sach != null ? c.CuonSach.Sach.TenSach : "Không xác định",
                    NgayMuon = c.PhieuMuon!.NgayMuon,
                    HanTra = c.HanTra
                })
                .ToListAsync();

            BorrowedBooks = new ObservableCollection<BorrowedBookInfo>(books);
        }

        /// <summary>
        /// Thực hiện thủ tục nhận trả cuốn sách vật lý được chọn.
        /// </summary>
        private async Task ExecuteReturnAsync(object? param)
        {
            if (IsHistoryTab)
            {
                MessageBox.Show("Sách này đã được hoàn trả xong.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (param is BorrowedBookInfo book && SelectedReader != null)
            {
                try
                {
                    var ketQua = await _muonTraService.ThucHienTraSachAsync(book.MaCuonSach);
                    if (ketQua.ThanhCong)
                    {
                        MessageBox.Show(ketQua.ThongBao, "Hoàn tất trả sách", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadBorrowedBooksAsync(SelectedReader.MaDocGia);
                        await LoadReadersAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xử lý trả sách: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    public class DocGiaWithBorrowCount
    {
        public int MaDocGia { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public int BorrowCount { get; set; }
    }

    public class BorrowedBookInfo
    {
        public int MaSach { get; set; }
        public int MaCuonSach { get; set; }
        public string TenSach { get; set; } = string.Empty;
        public DateTime NgayMuon { get; set; }
        public DateTime HanTra { get; set; }
    }
}
