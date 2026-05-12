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
    /// <summary>
    /// ViewModel quản lý danh sách Độc giả trên giao diện Admin.
    /// Áp dụng Strict Null Safety tuyệt đối và chú thích Tiếng Việt.
    /// </summary>
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

        /// <summary>
        /// Nạp toàn bộ danh sách độc giả và đếm số lượng yêu cầu tài khoản mới.
        /// </summary>
        public async Task LoadDataAsync()
        {
            try
            {
                using var context = new LmsDbContext();
                var readers = await context.DocGias.ToListAsync();
                Readers = new ObservableCollection<DocGia>(readers);

                PendingRequestCount = await context.TaiKhoans.CountAsync(t => t.TrangThai == "Pending");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách độc giả: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Tìm kiếm độc giả theo Họ tên hoặc Email.
        /// </summary>
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
                    using var context = new LmsDbContext();
                    var keyword = SearchKeyword.ToLower();
                    var filtered = await context.DocGias
                        .Where(d => d.HoTen.ToLower().Contains(keyword) || 
                                    (d.Email != null && d.Email.ToLower().Contains(keyword)) ||
                                    (d.SoDienThoai != null && d.SoDienThoai.Contains(keyword))) // Tìm thêm theo SĐT mới
                        .ToListAsync();
                    Readers = new ObservableCollection<DocGia>(filtered);
                }
            }
            catch (Exception)
            {
                // Bỏ qua lỗi nhỏ khi gõ nhanh
            }
        }

        /// <summary>
        /// Thực hiện thủ tục xóa tài khoản độc giả và kiểm tra các ràng buộc liên quan.
        /// </summary>
        private async Task ExecuteDeleteAsync(object? param)
        {
            if (param is DocGia reader)
            {
                var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa tài khoản độc giả '{reader.HoTen}' không?", 
                                              "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm == MessageBoxResult.No) return;

                try
                {
                    using var context = new LmsDbContext();
                    // Kiểm tra xem độc giả có cuốn sách vật lý nào đang mượn chưa trả không (dựa trên DB mới)
                    bool hasActiveLoans = await context.ChiTietMuonTras
                        .Include(c => c.PhieuMuon)
                        .AnyAsync(c => c.PhieuMuon!.MaDocGia == reader.MaDocGia && c.NgayTraThucTe == null);

                    if (hasActiveLoans)
                    {
                        MessageBox.Show("Không thể xóa: Độc giả này vẫn còn sách đang mượn chưa hoàn trả.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Nếu độc giả có tài khoản đăng nhập liên kết, gỡ bỏ tài khoản trước để tránh lỗi Foreign Key
                    if (!string.IsNullOrEmpty(reader.TenDangNhap))
                    {
                        var taiKhoan = await context.TaiKhoans.FindAsync(reader.TenDangNhap);
                        if (taiKhoan != null)
                        {
                            context.TaiKhoans.Remove(taiKhoan);
                            await context.SaveChangesAsync();
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
