using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THUVIENZ.DAL;
using THUVIENZ.Models;

namespace THUVIENZ.BLL
{
    /// <summary>
    /// Service xử lý nghiệp vụ liên quan đến Thông báo của độc giả.
    /// Áp dụng Strict Null Safety tuyệt đối và chú thích Tiếng Việt chi tiết.
    /// </summary>
    public class NotificationService
    {
        private readonly ThongBaoRepository _thongBaoRepository;
        private readonly DocGiaRepository _docGiaRepository;

        public NotificationService()
        {
            var context = new LmsDbContext();
            _thongBaoRepository = new ThongBaoRepository(context);
            _docGiaRepository = new DocGiaRepository(context);
        }

        public NotificationService(ThongBaoRepository repo, DocGiaRepository docGiaRepo)
        {
            _thongBaoRepository = repo;
            _docGiaRepository = docGiaRepo;
        }

        /// <summary>
        /// Lấy toàn bộ thông báo của độc giả theo tên đăng nhập.
        /// </summary>
        public async Task<IEnumerable<ThongBao>> GetNotificationsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return Enumerable.Empty<ThongBao>();
            var reader = await _docGiaRepository.GetReaderProfileAsync(username);
            if (reader == null) return Enumerable.Empty<ThongBao>();

            return await _thongBaoRepository.GetByReaderIdAsync(reader.MaDocGia);
        }

        /// <summary>
        /// Kiểm tra xem độc giả có thông báo chưa đọc hay không để bật chấm đỏ.
        /// </summary>
        public async Task<bool> HasUnreadNotificationsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
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
