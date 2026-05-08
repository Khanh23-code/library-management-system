using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THUVIENZ.DAL.Base;
using THUVIENZ.Models;

namespace THUVIENZ.DAL
{
    /// <summary>
    /// Repository cụ thể cho Độc giả. Kế thừa từ BaseRepository để tận dụng CRUD Generic.
    /// </summary>
    public class DocGiaRepository : BaseRepository<DocGia>
    {
        public DocGiaRepository(LmsDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy danh sách độc giả kèm theo số lượng sách đang mượn.
        /// Sử dụng Compiled Queries hoặc LINQ tối ưu của EF Core 8.
        /// </summary>
        public async Task<IEnumerable<object>> GetReadersWithBorrowedCountAsync()
        {
            return await _context.DocGias
                .Select(d => new
                {
                    ReaderID = d.MaDocGia,
                    ReaderName = d.HoTen,
                    TotalBorrowed = _context.ChiTietPhieuMuons
                        .Count(ct => ct.MaPhieuMuon != null && 
                                     _context.PhieuMuons.Any(pm => pm.MaPhieuMuon == ct.MaPhieuMuon && pm.MaDocGia == d.MaDocGia) &&
                                     ct.TrangThai == "Đang mượn")
                })
                .ToListAsync();
        }

        public async Task<DocGia?> GetReaderProfileAsync(string username)
        {
            return await _context.DocGias.FirstOrDefaultAsync(d => d.TenDangNhap == username);
        }

        public async Task<IEnumerable<Sach>> GetBorrowedBooksAsync(int maDocGia)
        {
            return await _context.ChiTietPhieuMuons
                .Where(ct => ct.MaPhieuMuon != null && 
                             _context.PhieuMuons.Any(pm => pm.MaPhieuMuon == ct.MaPhieuMuon && pm.MaDocGia == maDocGia) &&
                             ct.TrangThai == "Đang mượn")
                .Join(_context.Sachs, ct => ct.MaSach, s => s.MaSach, (ct, s) => s)
                .ToListAsync();
        }

        public async Task<decimal> GetReaderDebtAsync(int maDocGia)
        {
            var reader = await _context.DocGias.FindAsync(maDocGia);
            return reader?.TongNo ?? 0;
        }
    }
}
