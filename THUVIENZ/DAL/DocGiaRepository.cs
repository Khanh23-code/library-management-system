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
    /// Cập nhật truy vấn dựa trên cấu trúc bảng mới CHITIETMUONTRA gộp chung.
    /// Áp dụng Strict Null Safety tuyệt đối và chú thích Tiếng Việt.
    /// </summary>
    public class DocGiaRepository : BaseRepository<DocGia>
    {
        public DocGiaRepository(LmsDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy danh sách độc giả kèm theo số lượng sách đang mượn.
        /// </summary>
        public async Task<IEnumerable<object>> GetReadersWithBorrowedCountAsync()
        {
            return await _context.DocGias
                .Select(d => new
                {
                    ReaderID = d.MaDocGia,
                    ReaderName = d.HoTen,
                    TotalBorrowed = _context.ChiTietMuonTras
                        .Include(c => c.PhieuMuon)
                        .Count(c => c.PhieuMuon!.MaDocGia == d.MaDocGia && c.NgayTraThucTe == null)
                })
                .ToListAsync();
        }

        public async Task<DocGia?> GetReaderProfileAsync(string username)
        {
            return await _context.DocGias.FirstOrDefaultAsync(d => d.TenDangNhap == username);
        }

        /// <summary>
        /// Lấy danh sách các Đầu Sách mà độc giả đang mượn chưa trả.
        /// </summary>
        public async Task<IEnumerable<Sach>> GetBorrowedBooksAsync(int maDocGia)
        {
            var items = await _context.ChiTietMuonTras
                .Include(c => c.PhieuMuon)
                .Include(c => c.CuonSach)
                .ThenInclude(cs => cs!.Sach)
                .Where(c => c.PhieuMuon!.MaDocGia == maDocGia && c.NgayTraThucTe == null)
                .ToListAsync();

            return items
                .Where(c => c.CuonSach?.Sach != null)
                .Select(c => c.CuonSach!.Sach!)
                .Distinct()
                .ToList();
        }

        public async Task<decimal> GetReaderDebtAsync(int maDocGia)
        {
            var reader = await _context.DocGias.FindAsync(maDocGia);
            return reader?.TongNo ?? 0;
        }
    }
}
