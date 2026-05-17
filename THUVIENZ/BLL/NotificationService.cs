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
    /// Thiết kế theo dạng Transient Thread-safe để ngăn chặn triệt để hiện tượng đụng độ Transaction trong WPF đa luồng.
    /// </summary>
    public class NotificationService
    {
        public NotificationService()
        {
        }

        public NotificationService(ThongBaoRepository repo, DocGiaRepository docGiaRepo)
        {
            // Hỗ trợ tương thích ngược cho unit test nếu có
        }

        /// <summary>
        /// Lấy toàn bộ thông báo của độc giả theo tên đăng nhập.
        /// Sử dụng LmsDbContext ngắn hạn để bảo đảm an toàn đa luồng.
        /// </summary>
        public async Task<IEnumerable<ThongBao>> GetNotificationsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return Enumerable.Empty<ThongBao>();

            using (var context = new LmsDbContext())
            {
                var thongBaoRepo = new ThongBaoRepository(context);
                var docGiaRepo = new DocGiaRepository(context);

                // 1. Chạy quét ngầm tự sinh thông báo thật dựa trên lịch sử mượn thực tế
                await AutoGenerateActiveBorrowingNotificationsAsync(username, context, thongBaoRepo, docGiaRepo);

                // 2. Tải danh sách thông báo từ Database lên giao diện
                var reader = await docGiaRepo.GetReaderProfileAsync(username);
                if (reader == null) return Enumerable.Empty<ThongBao>();

                return await thongBaoRepo.GetByReaderIdAsync(reader.MaDocGia);
            }
        }

        /// <summary>
        /// Kiểm tra xem độc giả có thông báo chưa đọc hay không để bật chấm đỏ.
        /// Sử dụng LmsDbContext ngắn hạn để tránh đụng độ với luồng chính của Thủ thư.
        /// </summary>
        public async Task<bool> HasUnreadNotificationsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            using (var context = new LmsDbContext())
            {
                var thongBaoRepo = new ThongBaoRepository(context);
                var docGiaRepo = new DocGiaRepository(context);

                // 1. Chạy quét ngầm tự sinh thông báo thật dựa trên lịch sử mượn thực tế
                await AutoGenerateActiveBorrowingNotificationsAsync(username, context, thongBaoRepo, docGiaRepo);

                // 2. Kiểm tra trạng thái chấm đỏ
                var reader = await docGiaRepo.GetReaderProfileAsync(username);
                if (reader == null) return false;

                return await thongBaoRepo.HasUnreadAsync(reader.MaDocGia);
            }
        }

        /// <summary>
        /// Đánh dấu toàn bộ thông báo của độc giả là đã đọc.
        /// </summary>
        public async Task MarkAsReadAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return;

            using (var context = new LmsDbContext())
            {
                var thongBaoRepo = new ThongBaoRepository(context);
                var docGiaRepo = new DocGiaRepository(context);

                var reader = await docGiaRepo.GetReaderProfileAsync(username);
                if (reader == null) return;

                await thongBaoRepo.MarkAllAsReadAsync(reader.MaDocGia);
            }
        }

        /// <summary>
        /// Xóa vĩnh viễn một thông báo khi người dùng click nút đóng (X).
        /// </summary>
        public async Task DeleteNotificationAsync(int notificationId)
        {
            if (notificationId <= 0) return;

            using (var context = new LmsDbContext())
            {
                var thongBaoRepo = new ThongBaoRepository(context);
                var noti = await thongBaoRepo.GetByIdAsync(notificationId);
                if (noti != null)
                {
                    thongBaoRepo.Delete(noti);
                    await thongBaoRepo.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Phương thức nội bộ thực hiện quét và tự sinh thông báo thật cho độc giả.
        /// </summary>
        private async Task AutoGenerateActiveBorrowingNotificationsAsync(
            string username, 
            LmsDbContext context, 
            ThongBaoRepository thongBaoRepository, 
            DocGiaRepository docGiaRepository)
        {
            if (string.IsNullOrWhiteSpace(username)) return;
            var reader = await docGiaRepository.GetReaderProfileAsync(username);
            if (reader == null) return;

            string friendlyName = GetFirstName(reader.HoTen);

            try
            {
                // Lấy toàn bộ chi tiết giao dịch mượn chưa trả (NgayTraThucTe == null) của độc giả này
                var activeBorrowings = await context.ChiTietMuonTras
                    .Include(c => c.CuonSach)
                    .ThenInclude(cs => cs!.Sach)
                    .Where(c => c.PhieuMuon != null && c.PhieuMuon.MaDocGia == reader.MaDocGia && c.NgayTraThucTe == null)
                    .ToListAsync();

                // Lấy toàn bộ thông báo đang có của độc giả này để tránh tạo trùng lặp
                var existingNotis = await thongBaoRepository.GetByReaderIdAsync(reader.MaDocGia);

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
                            await CreateNotificationInternalAsync(context, thongBaoRepository, reader.MaDocGia, title, msg, NotificationType.Failure);
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
                            await CreateNotificationInternalAsync(context, thongBaoRepository, reader.MaDocGia, title, msg, NotificationType.Warning);
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
        /// Tạo thông báo mới cho độc giả sử dụng DbContext độc lập.
        /// </summary>
        public async Task CreateNotificationAsync(int maDocGia, string title, string message, NotificationType type)
        {
            using (var context = new LmsDbContext())
            {
                var thongBaoRepo = new ThongBaoRepository(context);
                await CreateNotificationInternalAsync(context, thongBaoRepo, maDocGia, title, message, type);
            }
        }

        /// <summary>
        /// Ghi trực tiếp thông báo vào DbContext đang được sử dụng.
        /// </summary>
        private async Task CreateNotificationInternalAsync(LmsDbContext context, ThongBaoRepository repo, int maDocGia, string title, string message, NotificationType type)
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
            await repo.AddAsync(noti);
            await repo.SaveChangesAsync();
        }
    }
}
