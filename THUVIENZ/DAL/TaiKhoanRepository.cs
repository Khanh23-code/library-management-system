using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THUVIENZ.DAL.Base;
using THUVIENZ.Models;

namespace THUVIENZ.DAL
{
    /// <summary>
    /// Repository cho Tài khoản.
    /// </summary>
    public class TaiKhoanRepository : BaseRepository<TaiKhoan>
    {
        public TaiKhoanRepository() : base(new LmsDbContext())
        {
        }

        public TaiKhoanRepository(LmsDbContext context) : base(context)
        {
        }

        public async Task<TaiKhoan?> GetAccountByUsernameAsync(string username)
        {
            return await GetByIdAsync(username);
        }

        /// <summary>
        /// Lấy danh sách tài khoản đang chờ duyệt.
        /// </summary>
        public async Task<IEnumerable<TaiKhoan>> GetPendingAccountsAsync()
        {
            return await _context.TaiKhoans
                .Include(t => t.DocGia)
                .Where(t => t.TrangThai == "Pending")
                .ToListAsync();
        }

        /// <summary>
        /// Xóa tài khoản và dữ liệu Độc giả liên quan (Dùng khi Reject).
        /// Sử dụng EF Core Transaction ngầm định.
        /// </summary>
        public async Task DeleteAccountAndReaderAsync(string username)
        {
            var reader = await _context.DocGias.FirstOrDefaultAsync(d => d.TenDangNhap == username);
            if (reader != null) _context.DocGias.Remove(reader);

            var account = await _context.TaiKhoans.FindAsync(username);
            if (account != null) _context.TaiKhoans.Remove(account);

            await _context.SaveChangesAsync();
        }
    }
}
