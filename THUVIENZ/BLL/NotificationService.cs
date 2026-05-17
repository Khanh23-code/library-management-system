using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using THUVIENZ.DAL;
using THUVIENZ.Models;

namespace THUVIENZ.BLL
{
    /// <summary>
    /// Service xử lý nghiệp vụ liên quan đến Thông báo của độc giả.
    /// Đã tích hợp bộ quét tự động dịch các bản ghi mượn sách thực tế thành thông báo tiếng Việt cá nhân hóa.
    /// </summary>
    public class NotificationService
    {
        private readonly LmsDbContext _context;
        private readonly ThongBaoRepository _thongBaoRepository;
        private readonly DocGiaRepository _docGiaRepository;

        public NotificationService()
        {
            _context = new LmsDbContext();
            _thongBaoRepository = new ThongBaoRepository(_context);
            _docGiaRepository = new DocGiaRepository(_context);
        }

        public NotificationService(ThongBaoRepository repo, DocGiaRepository docGiaRepo)
        {
            _context = new LmsDbContext();
            _thongBaoRepository = repo;
            _docGiaRepository = docGiaRepo;
        }

        /// <summary>
        /// Lấy toàn bộ thông báo của độc giả theo tên đăng nhập.
        /// Tự động quét CSDL mượn trả thực tế để tạo các thông báo quá hạn / hết hạn trước khi nạp.
        /// </summary>
        public async Task<IEnumerable<ThongBao>> GetNotificationsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return Enumerable.Empty<ThongBao>();

            // 1. Chạy quét ngầm tự sinh thông báo thật dựa trên lịch sử mượn thực tế
            await AutoGenerateActiveBorrowingNotificationsAsync(username);

            // 2. Tải danh sách thông báo từ Database lên giao diện
            var reader = await _docGiaRepository.GetReaderProfileAsync(username);
            if (reader == null) return Enumerable.Empty<ThongBao>();

            return await _thongBaoRepository.GetByReaderIdAsync(reader.MaDocGia);
        }

        /// <summary>
        /// Kiểm tra xem độc giả có thông báo chưa đọc hay không để bật chấm đỏ.
        /// Tự động quét CSDL mượn trả thực tế để bật chấm đỏ thời gian thực nếu có sách quá hạn mới phát sinh.
        /// </summary>
        public async Task<bool> HasUnreadNotificationsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            // 1. Chạy quét ngầm tự sinh thông báo thật dựa trên lịch sử mượn thực tế
            await AutoGenerateActiveBorrowingNotificationsAsync(username);

            // 2. Kiểm tra trạng thái chấm đỏ
            var reader = await _docGiaRepository.GetReaderProfileAsync(username);
            if (reader == null) return false;

            return await _thongBaoRepository.HasUnreadAsync(reader.MaDocGia);
        }

        /// <summary>
        /// Đánh dấu toàn bộ thông báo của độc giả là đã đọc (được gọi khi vào màn hình Thông báo).
        /// </summary>
        public async Task MarkAsReadAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return;
            var reader = await _docGiaRepository.GetReaderProfileAsync(username);
            if (reader == null) return;

            await _thongBaoRepository.MarkAllAsReadAsync(reader.MaDocGia);
        }

        /// <summary>
        /// Xóa vĩnh viễn một thông báo khi người dùng click nút đóng (X).
        /// </summary>
        public async Task DeleteNotificationAsync(int notificationId)
        {
            if (notificationId <= 0) return;
            var noti = await _thongBaoRepository.GetByIdAsync(notificationId);
            if (noti != null)
            {
                _thongBaoRepository.Delete(noti);
                await _thongBaoRepository.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Quét CSDL mượn trả thực tế của độc giả để tự sinh thông báo thật (quá hạn, sắp hết hạn).
        /// </summary>
        private async Task AutoGenerateActiveBorrowingNotificationsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return;
            var reader = await _docGiaRepository.GetReaderProfileAsync(username);
            if (reader == null) return;

            string friendlyName = GetFirstName(reader.HoTen);

            try
            {
                // Lấy toàn bộ chi tiết giao dịch mượn chưa trả (NgayTraThucTe == null) của độc giả này
                var activeBorrowings = await _context.ChiTietMuonTras
                    .Include(c => c.CuonSach)
                    .ThenInclude(cs => cs!.Sach)
                    .Where(c => c.PhieuMuon != null && c.PhieuMuon.MaDocGia == reader.MaDocGia && c.NgayTraThucTe == null)
                    .ToListAsync();

                // Lấy toàn bộ thông báo đang có của độc giả này để tránh tạo trùng lặp
                var existingNotis = await _thongBaoRepository.GetByReaderIdAsync(reader.MaDocGia);

                foreach (var b in activeBorrowings)
                {
                    var bookName = b.CuonSach?.Sach?.TenSach ?? "Sách";
                    var daysRemaining = (b.HanTra.Date - DateTime.Today).Days;

                    if (daysRemaining < 0)
                    {
                        // 1. Xử lý thông báo Quá hạn (Failure - Màu Đỏ)
                        var title = "Sách đã quá hạn";
                        
                        // Kiểm tra xem hôm nay đã cảnh báo quá hạn cho cuốn sách này chưa
                        var hasAlreadyNotifiedOverdue = existingNotis.Any(n => 
                            n.TieuDe == title && 
                            n.NoiDung.Contains(bookName) && 
                            n.NgayTao.Date == DateTime.Today);

                        if (!hasAlreadyNotifiedOverdue)
                        {
                            var msg = $"{friendlyName} ơi, quyển sách '{bookName}' của bạn đã bị quá hạn trả rồi đấy!! Bạn hãy thu xếp hoàn trả sớm để tránh tích lũy thêm phí phạt trễ hạn nhé!";
                            await CreateNotificationAsync(reader.MaDocGia, title, msg, NotificationType.Failure);
                        }
                    }
                    else if (daysRemaining <= 3)
                    {
                        // 2. Xử lý thông báo Sắp hết hạn (Warning - Màu Vàng)
                        var title = "Sách sắp hết hạn";

                        // Kiểm tra xem trong vòng 3 ngày qua đã tạo thông báo nhắc nhở sắp hết hạn cho quyển này chưa
                        var hasAlreadyNotifiedExpiry = existingNotis.Any(n => 
                            n.TieuDe == title && 
                            n.NoiDung.Contains(bookName) && 
                            (DateTime.Today - n.NgayTao.Date).TotalDays <= 3);

                        if (!hasAlreadyNotifiedExpiry)
                        {
                            var msg = $"{friendlyName} ơi, quyển '{bookName}' của bạn chỉ còn thời hạn mượn là {daysRemaining} ngày thôi, bạn đừng quên nhé!!";
                            await CreateNotificationAsync(reader.MaDocGia, title, msg, NotificationType.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi quét tự động sinh thông báo thực: " + ex.Message);
            }
        }

        /// <summary>
        /// Tách lấy tên gọi (tên riêng) từ Họ và Tên đầy đủ của người Việt.
        /// </summary>
        public static string GetFirstName(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "Bạn";
            var parts = fullName.Trim().Split(' ');
            return parts[parts.Length - 1];
        }

        /// <summary>
        /// Phương thức tiện ích để tạo thông báo mới cho độc giả (ví dụ: mượn/trả sách thành công).
        /// </summary>
        public async Task CreateNotificationAsync(int maDocGia, string title, string message, NotificationType type)
        {
            var noti = new ThongBao
            {
                MaDocGia = maDocGia,
                TieuDe = title,
                NoiDung = message,
                LoaiThongBao = type,
                NgayTao = DateTime.Now,
                DaDoc = false
            };
            await _thongBaoRepository.AddAsync(noti);
            await _thongBaoRepository.SaveChangesAsync();
        }
    }
}
