using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THUVIENZ.DAL.Base;
using THUVIENZ.Models;

namespace THUVIENZ.DAL
{
    /// <summary>
    /// Repository cụ thể cho Sách.
    /// </summary>
    public class SachRepository : BaseRepository<Sach>
    {
        public SachRepository() : base(new LmsDbContext())
        {
        }

        public SachRepository(LmsDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Kiểm tra xem cuốn sách có đang được mượn hay không.
        /// Chú thích bằng tiếng Việt.
        /// </summary>
        public bool IsBookCurrentlyBorrowed(int maSach)
        {
            return _context.ChiTietPhieuMuons.Any(ct => ct.MaSach == maSach && ct.TrangThai == "Đang mượn");
        }

        /// <summary>
        /// Tìm kiếm sách theo tên hoặc tác giả.
        /// </summary>
        public async Task<IEnumerable<Sach>> SearchBooksAsync(string keyword)
        {
            return await _context.Sachs
                .Where(s => s.TenSach.Contains(keyword) || s.TacGia.Contains(keyword))
                .ToListAsync();
        }
    }
}
