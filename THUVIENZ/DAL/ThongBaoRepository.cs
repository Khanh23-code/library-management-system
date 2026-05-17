using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THUVIENZ.DAL.Base;
using THUVIENZ.Models;

namespace THUVIENZ.DAL
{
    /// <summary>
    /// Repository quản lý truy xuất dữ liệu cho thực thể Thông báo.
    /// Kế thừa từ BaseRepository để tận dụng CRUD Generic.
    /// </summary>
    public class ThongBaoRepository : BaseRepository<ThongBao>
    {
        public ThongBaoRepository() : base(new LmsDbContext())
        {
        }

        public ThongBaoRepository(LmsDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy toàn bộ danh sách thông báo của độc giả xếp theo thứ tự mới nhất trước.
        /// </summary>
        public async Task<IEnumerable<ThongBao>> GetByReaderIdAsync(int maDocGia)
        {
            return await _context.ThongBaos
                .Where(t => t.MaDocGia == maDocGia)
                .OrderByDescending(t => t.NgayTao)
                .ToListAsync();
        }

        /// <summary>
        /// Kiểm tra độc giả có thông báo chưa đọc hay không.
        /// </summary>
        public async Task<bool> HasUnreadAsync(int maDocGia)
        {
            return await _context.ThongBaos
                .AnyAsync(t => t.MaDocGia == maDocGia && !t.DaDoc);
        }

        /// <summary>
        /// Đánh dấu tất cả thông báo của độc giả là đã đọc.
        /// </summary>
        public async Task MarkAllAsReadAsync(int maDocGia)
        {
            var unread = await _context.ThongBaos
                .Where(t => t.MaDocGia == maDocGia && !t.DaDoc)
                .ToListAsync();

            if (unread.Any())
            {
                foreach (var item in unread)
                {
                    item.DaDoc = true;
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
