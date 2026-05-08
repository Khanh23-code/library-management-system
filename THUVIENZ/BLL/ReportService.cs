using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THUVIENZ.DAL;
using THUVIENZ.Models;

namespace THUVIENZ.BLL
{
    /// <summary>
    /// Service xử lý báo cáo và khai phá dữ liệu (Data Mining).
    /// </summary>
    public class ReportService
    {
        private readonly LmsDbContext _context;

        public ReportService() : this(new LmsDbContext())
        {
        }

        public ReportService(LmsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Khai phá mẫu mượn sách bằng thuật toán Apriori đơn giản.
        /// Tìm các cặp sách thường xuyên được mượn cùng nhau.
        /// </summary>
        /// <param name="minSupport">Độ hỗ trợ tối thiểu (số lần xuất hiện)</param>
        public async Task<List<Tuple<int, int, int>>> GetFrequentBookPairsAsync(int minSupport = 2)
        {
            // 1. Lấy dữ liệu mượn sách theo từng giao dịch (Phiếu mượn)
            var transactions = await _context.ChiTietPhieuMuons
                .GroupBy(ct => ct.MaPhieuMuon)
                .Select(g => g.Select(ct => ct.MaSach).ToList())
                .ToListAsync();

            var pairCounts = new Dictionary<Tuple<int, int>, int>();

            // 2. Thuật toán Apriori: Tạo ứng viên bộ 2 (2-itemsets)
            foreach (var transaction in transactions)
            {
                if (transaction.Count < 2) continue;

                var sortedItems = transaction.OrderBy(i => i).ToList();
                for (int i = 0; i < sortedItems.Count; i++)
                {
                    for (int j = i + 1; j < sortedItems.Count; j++)
                    {
                        var pair = new Tuple<int, int>(sortedItems[i], sortedItems[j]);
                        if (pairCounts.ContainsKey(pair))
                            pairCounts[pair]++;
                        else
                            pairCounts[pair] = 1;
                    }
                }
            }

            // 3. Lọc theo độ hỗ trợ tối thiểu (Support)
            return pairCounts
                .Where(p => p.Value >= minSupport)
                .Select(p => new Tuple<int, int, int>(p.Key.Item1, p.Key.Item2, p.Value))
                .OrderByDescending(p => p.Item3)
                .ToList();
        }

        /// <summary>
        /// Lấy danh sách các đầu sách được mượn nhiều nhất (Top Borrowed).
        /// Chú thích bằng tiếng Việt.
        /// </summary>
        public async Task<List<BookStatDTO>> GetTopBorrowedBooksAsync(DateTime from, DateTime to, int top = 10)
        {
            return await _context.ChiTietPhieuMuons
                .Join(_context.PhieuMuons, ct => ct.MaPhieuMuon, pm => pm.MaPhieuMuon, (ct, pm) => new { ct, pm })
                .Where(x => x.pm.NgayMuon >= from && x.pm.NgayMuon <= to)
                .GroupBy(x => x.ct.MaSach)
                .Select(g => new BookStatDTO
                {
                    MaSach = g.Key,
                    TenSach = _context.Sachs.Where(s => s.MaSach == g.Key).Select(s => s.TenSach).FirstOrDefault() ?? "N/A",
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .Take(top)
                .ToListAsync();
        }

        /// <summary>
        /// Báo cáo độc giả quá hạn hoặc nợ tiền.
        /// Tính toán theo thời gian thực dựa trên THAMSO.
        /// </summary>
        public async Task<List<ReaderStatDTO>> GetReaderOverdueReportsAsync()
        {
            var maxDaysParam = await _context.ThamSos.FindAsync("SoNgayMuonToiDa");
            int maxDays = (int)(maxDaysParam?.GiaTri ?? 7);

            return await _context.DocGias
                .Select(d => new ReaderStatDTO
                {
                    MaDocGia = d.MaDocGia,
                    HoTen = d.HoTen,
                    TongNo = d.TongNo ?? 0,
                    OverdueCount = _context.ChiTietPhieuMuons
                        .Join(_context.PhieuMuons, ct => ct.MaPhieuMuon, pm => pm.MaPhieuMuon, (ct, pm) => new { ct, pm })
                        .Count(x => x.pm.MaDocGia == d.MaDocGia && 
                                    x.ct.TrangThai == "Đang mượn" && 
                                    EF.Functions.DateDiffDay(x.pm.NgayMuon, DateTime.Now) > maxDays)
                })
                .Where(x => x.TongNo > 0 || x.OverdueCount > 0)
                .ToListAsync();
        }

        /// <summary>
        /// Báo cáo thống kê mượn sách theo thể loại.
        /// </summary>
        public async Task<IEnumerable<object>> GetBorrowingStatsByCategoryAsync(DateTime from, DateTime to)
        {
            return await _context.ChiTietPhieuMuons
                .Join(_context.PhieuMuons, ct => ct.MaPhieuMuon, pm => pm.MaPhieuMuon, (ct, pm) => new { ct, pm })
                .Where(x => x.pm.NgayMuon >= from && x.pm.NgayMuon <= to)
                .Join(_context.Sachs, x => x.ct.MaSach, s => s.MaSach, (x, s) => new { x, s })
                .Join(_context.TheLoaiSachs, x => x.s.MaTheLoai, tl => tl.MaTheLoai, (x, tl) => new { tl.TenTheLoai })
                .GroupBy(x => x.TenTheLoai)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    BorrowCount = g.Count(),
                    Percentage = 0 // Sẽ tính toán ở tầng ViewModel hoặc bên dưới
                })
                .ToListAsync();
        }
    }
}
