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
                // Lọc bỏ những độc giả đã chuyển sang trạng thái Soft Delete (DisActive)
                var readers = await context.DocGias
                    .Include(d => d.TaiKhoan)
                    .Where(d => d.TaiKhoan == null || d.TaiKhoan.TrangThai != "DisActive")
                    .ToListAsync();

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
                        .Include(d => d.TaiKhoan)
                        .Where(d => (d.TaiKhoan == null || d.TaiKhoan.TrangThai != "DisActive") &&
                                    (d.HoTen.ToLower().Contains(keyword) || 
                                     (d.Email != null && d.Email.ToLower().Contains(keyword)) ||
                                     (d.SoDienThoai != null && d.SoDienThoai.Contains(keyword)))) // Tìm thêm theo SĐT mới
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
        /// Thực hiện thủ tục vô hiệu hóa tài khoản độc giả (Soft Delete) để bảo toàn lịch sử mượn trả.
        /// </summary>
        private async Task ExecuteDeleteAsync(object? param)
        {
            if (param is DocGia reader)
            {
                var confirm = MessageBox.Show($"Bạn có chắc chắn muốn vô hiệu hóa tài khoản độc giả '{reader.HoTen}'?", 
                                              "Xác nhận vô hiệu hóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm == MessageBoxResult.No) return;

                try
                {
                    using var context = new LmsDbContext();
                    // Kiểm tra xem độc giả có cuốn sách vật lý nào đang mượn chưa trả không
                    bool hasActiveLoans = await context.ChiTietMuonTras
                        .Include(c => c.PhieuMuon)
                        .AnyAsync(c => c.PhieuMuon!.MaDocGia == reader.MaDocGia && c.NgayTraThucTe == null);

                    if (hasActiveLoans)
                    {
                        MessageBox.Show("Không thể vô hiệu hóa: Độc giả này vẫn còn sách đang mượn chưa hoàn trả.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Thực hiện Soft Delete: Không xóa vật lý để bảo toàn dữ liệu, chỉ đổi trạng thái tài khoản sang "DisActive"
                    if (!string.IsNullOrEmpty(reader.TenDangNhap))
                    {
                        var taiKhoan = await context.TaiKhoans.FindAsync(reader.TenDangNhap);
                        if (taiKhoan != null)
                        {
                            taiKhoan.TrangThai = "DisActive";
                        }
                    }
                    else
                    {
                        // Nếu độc giả chưa có tài khoản, tự sinh một tài khoản ảo trạng thái DisActive để lưu vết
                        var newTaiKhoan = new TaiKhoan
                        {
                            TenDangNhap = $"dis_{reader.MaDocGia}_{System.DateTime.Now:yyMMddHHmmss}",
                            MatKhau = "disactive",
                            Quyen = "Reader",
                            TrangThai = "DisActive"
                        };
                        context.TaiKhoans.Add(newTaiKhoan);
                        var targetReader = await context.DocGias.FindAsync(reader.MaDocGia);
                        if (targetReader != null) targetReader.TenDangNhap = newTaiKhoan.TenDangNhap;
                    }

                    await context.SaveChangesAsync();

                    MessageBox.Show("Đã vô hiệu hóa độc giả thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    string errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    MessageBox.Show($"Lỗi xử lý vô hiệu hóa từ CSDL: {errorMsg}\n\n(Gợi ý: Vui lòng kiểm tra lại ràng buộc CHECK constraint của bảng TAIKHOAN trong Database vật lý)", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
