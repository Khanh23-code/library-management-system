using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using THUVIENZ.BLL;
using THUVIENZ.Core;
using THUVIENZ.Models;
using THUVIENZ.DAL;
using Microsoft.EntityFrameworkCore;

namespace THUVIENZ.ViewModels
{
    public class AdminCirculationViewModel : ObservableObject
    {
        private readonly LmsDbContext _context;
        private readonly CirculationService _circulationService;

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
            _circulationService = new CirculationService();

            LoadDataCommand = new RelayCommand(async _ => await LoadReadersAsync());
            ReturnBookCommand = new RelayCommand(async param => await ExecuteReturnAsync(param));

            _ = LoadReadersAsync();
        }

        public async Task LoadReadersAsync()
        {
            string statusFilter = IsHistoryTab ? "Đã trả" : "Đang mượn";

            var readers = await _context.DocGias
                .Select(d => new DocGiaWithBorrowCount
                {
                    MaDocGia = d.MaDocGia,
                    HoTen = d.HoTen,
                    BorrowCount = _context.ChiTietPhieuMuons
                        .Join(_context.PhieuMuons, ct => ct.MaPhieuMuon, pm => pm.MaPhieuMuon, (ct, pm) => new { ct, pm })
                        .Count(x => x.pm.MaDocGia == d.MaDocGia && x.ct.TrangThai == statusFilter)
                })
                .Where(x => x.BorrowCount > 0)
                .ToListAsync();

            ReadersList = new ObservableCollection<DocGiaWithBorrowCount>(readers);
        }

        public async Task LoadBorrowedBooksAsync(int readerId)
        {
            string statusFilter = IsHistoryTab ? "Đã trả" : "Đang mượn";

            var books = await _context.ChiTietPhieuMuons
                .Join(_context.PhieuMuons, ct => ct.MaPhieuMuon, pm => pm.MaPhieuMuon, (ct, pm) => new { ct, pm })
                .Join(_context.Sachs, x => x.ct.MaSach, s => s.MaSach, (x, s) => new { x.ct, x.pm, s })
                .Where(x => x.pm.MaDocGia == readerId && x.ct.TrangThai == statusFilter)
                .Select(x => new BorrowedBookInfo
                {
                    MaSach = x.s.MaSach,
                    TenSach = x.s.TenSach,
                    NgayMuon = x.pm.NgayMuon,
                    HanTra = x.pm.NgayMuon.AddDays(30)
                })
                .ToListAsync();

            BorrowedBooks = new ObservableCollection<BorrowedBookInfo>(books);
        }

        private async Task ExecuteReturnAsync(object? param)
        {
            if (IsHistoryTab)
            {
                System.Windows.MessageBox.Show("Sách này đã được hoàn trả xong.", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                return;
            }

            if (param is BorrowedBookInfo book && SelectedReader != null)
            {
                try
                {
                    bool success = await _circulationService.ProcessReturnAsync(SelectedReader.MaDocGia, book.MaSach, DateTime.Now);
                    if (success)
                    {
                        await LoadBorrowedBooksAsync(SelectedReader.MaDocGia);
                        await LoadReadersAsync();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Lỗi trả sách: {ex.Message}", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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
        public string TenSach { get; set; } = string.Empty;
        public DateTime NgayMuon { get; set; }
        public DateTime HanTra { get; set; }
    }
}
