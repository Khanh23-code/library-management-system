using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using THUVIENZ.BLL;
using THUVIENZ.Core;
using THUVIENZ.Models;
using THUVIENZ.DAL;
using Microsoft.EntityFrameworkCore;

namespace THUVIENZ.ViewModels
{
    public class ReaderManagementViewModel : ObservableObject
    {
        private readonly LmsDbContext _context;
        private readonly ReaderManagementService _readerService;

        private ObservableCollection<DocGia> _readers = new ObservableCollection<DocGia>();
        public ObservableCollection<DocGia> Readers
        {
            get => _readers;
            set { _readers = value; OnPropertyChanged(); }
        }

        private int _pendingRequestCount;
        public int PendingRequestCount
        {
            get => _pendingRequestCount;
            set { _pendingRequestCount = value; OnPropertyChanged(); }
        }

        private string _searchKeyword = string.Empty;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                _ = ExecuteSearchAsync();
            }
        }

        public ICommand LoadReadersCommand { get; }
        public ICommand DeleteReaderCommand { get; }

        public ReaderManagementViewModel()
        {
            _context = new LmsDbContext();
            _readerService = new ReaderManagementService();

            LoadReadersCommand = new RelayCommand(async _ => await LoadDataAsync());
            DeleteReaderCommand = new RelayCommand(async param => await ExecuteDeleteAsync(param));

            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            try
            {
                var readers = await _context.DocGias.ToListAsync();
                Readers = new ObservableCollection<DocGia>(readers);

                PendingRequestCount = await _context.TaiKhoans.CountAsync(t => t.TrangThai == "Pending");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách độc giả: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ExecuteSearchAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchKeyword))
                {
                    await LoadDataAsync();
                }
                else
                {
                    var keyword = SearchKeyword.ToLower();
                    // SoDienThoai không tồn tại trong DB model của ứng dụng, ta tìm theo HoTen và Email
                    var filtered = await _context.DocGias
                        .Where(d => d.HoTen.ToLower().Contains(keyword) || 
                                    (d.Email != null && d.Email.ToLower().Contains(keyword)))
                        .ToListAsync();
                    Readers = new ObservableCollection<DocGia>(filtered);
                }
            }
            catch (Exception)
            {
                // Bỏ qua lỗi nhỏ khi gõ nhanh
            }
        }

        private async Task ExecuteDeleteAsync(object? param)
        {
            if (param is DocGia reader)
            {
                var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa tài khoản độc giả '{reader.HoTen}' không?", 
                                              "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm == MessageBoxResult.No) return;

                try
                {
                    // Kiểm tra xem độc giả có phiếu mượn chưa trả không
                    bool hasActiveLoans = await _context.PhieuMuons
                        .Join(_context.ChiTietPhieuMuons, pm => pm.MaPhieuMuon, ct => ct.MaPhieuMuon, (pm, ct) => new { pm, ct })
                        .AnyAsync(x => x.pm.MaDocGia == reader.MaDocGia && x.ct.TrangThai == "Đang mượn");

                    if (hasActiveLoans)
                    {
                        MessageBox.Show("Không thể xóa: Độc giả này vẫn còn sách đang mượn chưa hoàn trả.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Nếu độc giả có tài khoản đăng nhập liên kết, gỡ bỏ tài khoản trước để tránh lỗi Foreign Key
                    if (!string.IsNullOrEmpty(reader.TenDangNhap))
                    {
                        var taiKhoan = await _context.TaiKhoans.FindAsync(reader.TenDangNhap);
                        if (taiKhoan != null)
                        {
                            _context.TaiKhoans.Remove(taiKhoan);
                            await _context.SaveChangesAsync();
                        }
                    }

                    await _readerService.DeleteReaderAsync(reader.MaDocGia);
                    MessageBox.Show("Xóa độc giả thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể xóa độc giả do dữ liệu ràng buộc lịch sử: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
