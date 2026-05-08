using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THUVIENZ.DAL;
using THUVIENZ.DAL.Base;
using THUVIENZ.Models;

namespace THUVIENZ.BLL
{
    /// <summary>
    /// Service xử lý nghiệp vụ Mượn/Trả sách.
    /// Toàn bộ logic Transaction và tính toán được thực hiện tại đây bằng EF Core 8.
    /// </summary>
    public class CirculationService
    {
        private readonly LmsDbContext _context;
        private readonly LibrarySettingsService _settingsService;

        public CirculationService() : this(new LmsDbContext(), new LibrarySettingsService())
        {
        }

        public CirculationService(LmsDbContext context, LibrarySettingsService settingsService)
        {
            _context = context;
            _settingsService = settingsService;
        }

        /// <summary>
        /// Xử lý trả sách. Sử dụng Transaction để đảm bảo tính nguyên tử.
        /// Chú thích bằng tiếng Việt theo yêu cầu của Tech Lead.
        /// </summary>
        public async Task<bool> ProcessReturnAsync(int readerId, int bookId, DateTime returnDate)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Tìm thông tin mượn gần nhất của cuốn sách này cho độc giả này
                var borrowDetail = await _context.ChiTietPhieuMuons
                    .Join(_context.PhieuMuons, 
                          ct => ct.MaPhieuMuon, 
                          pm => pm.MaPhieuMuon, 
                          (ct, pm) => new { ct, pm })
                    .Where(x => x.pm.MaDocGia == readerId && x.ct.MaSach == bookId && x.ct.TrangThai == "Đang mượn")
                    .OrderByDescending(x => x.pm.NgayMuon)
                    .Select(x => new { x.ct, x.pm.NgayMuon })
                    .FirstOrDefaultAsync();

                if (borrowDetail == null)
                    throw new InvalidOperationException("Không tìm thấy thông tin mượn của cuốn sách này.");

                // 2. Lấy các tham số quy định
                int maxDays = (int)await _settingsService.GetValueAsync("SoNgayMuonToiDa");
                decimal fineRate = (decimal)await _settingsService.GetValueAsync("TienPhatMoiNgay");

                // 3. Tính toán ngày trễ và tiền phạt
                DateTime dueDate = borrowDetail.NgayMuon.AddDays(maxDays);
                int overdueDays = (returnDate.Date - dueDate.Date).Days;
                decimal fineAmount = overdueDays > 0 ? overdueDays * fineRate : 0;

                // 4. Tạo Phiếu Trả
                var phieuTra = new PhieuTra
                {
                    MaDocGia = readerId,
                    NgayTra = returnDate,
                    TienPhatKyNay = fineAmount
                };
                _context.PhieuTras.Add(phieuTra);
                await _context.SaveChangesAsync();

                // 5. Tạo Chi Tiết Phiếu Trả
                var chiTietTra = new ChiTietPhieuTra
                {
                    MaPhieuTra = phieuTra.MaPhieuTra,
                    MaPhieuMuon = borrowDetail.ct.MaPhieuMuon,
                    MaSach = bookId,
                    SoNgayMuon = (returnDate.Date - borrowDetail.NgayMuon.Date).Days,
                    TienPhat = fineAmount
                };
                _context.ChiTietPhieuTras.Add(chiTietTra);

                // 6. Cập nhật trạng thái Chi Tiết Phiếu Mượn
                borrowDetail.ct.TrangThai = "Đã trả";
                _context.ChiTietPhieuMuons.Update(borrowDetail.ct);

                // 7. Cập nhật trạng thái Sách và kiểm tra OCC (RowVersion)
                var book = await _context.Sachs.FindAsync(bookId);
                if (book != null)
                {
                    book.TinhTrang = "Sẵn sàng";
                    _context.Sachs.Update(book);
                }

                // 8. Cập nhật tổng nợ của Độc giả
                var reader = await _context.DocGias.FindAsync(readerId);
                if (reader != null && fineAmount > 0)
                {
                    reader.TongNo = (reader.TongNo ?? 0) + fineAmount;
                    _context.DocGias.Update(reader);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                throw new Exception("Dữ liệu sách đã được cập nhật bởi một Admin khác. Vui lòng thử lại.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Xử lý mượn sách. Kiểm tra các quy tắc nghiệp vụ trước khi lưu.
        /// </summary>
        public async Task<bool> BorrowBooksAsync(int readerId, List<int> bookIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                int maxBooks = (int)await _settingsService.GetValueAsync("SoSachMuonToiDa");
                var currentBorrowedCount = await _context.ChiTietPhieuMuons
                    .Join(_context.PhieuMuons, ct => ct.MaPhieuMuon, pm => pm.MaPhieuMuon, (ct, pm) => new { ct, pm })
                    .CountAsync(x => x.pm.MaDocGia == readerId && x.ct.TrangThai == "Đang mượn");

                if (currentBorrowedCount + bookIds.Count > maxBooks)
                    throw new InvalidOperationException($"Vượt quá giới hạn mượn sách ({maxBooks} cuốn).");

                var phieuMuon = new PhieuMuon
                {
                    MaDocGia = readerId,
                    NgayMuon = DateTime.Now
                };
                _context.PhieuMuons.Add(phieuMuon);
                await _context.SaveChangesAsync();

                foreach (var bookId in bookIds)
                {
                    var book = await _context.Sachs.FindAsync(bookId);
                    if (book == null || book.TinhTrang != "Sẵn sàng")
                        throw new InvalidOperationException($"Sách mã {bookId} không sẵn sàng.");

                    var chiTiet = new ChiTietPhieuMuon
                    {
                        MaPhieuMuon = phieuMuon.MaPhieuMuon,
                        MaSach = bookId,
                        TrangThai = "Đang mượn"
                    };
                    _context.ChiTietPhieuMuons.Add(chiTiet);

                    book.TinhTrang = "Đang mượn";
                    _context.Sachs.Update(book);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
