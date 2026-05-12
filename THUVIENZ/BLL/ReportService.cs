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
    /// Áp dụng Strict Null Safety và cấu trúc DB chuẩn hóa mới gộp chung.
    /// Toàn bộ chú thích viết bằng Tiếng Việt.
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
        /// Tìm các cặp đầu sách thường xuyên được mượn cùng nhau.
        /// </summary>
        /// <param name="minSupport">Độ hỗ trợ tối thiểu (số lần xuất hiện)</param>
        public async Task<List<Tuple<int, int, int>>> GetFrequentBookPairsAsync(int minSupport = 2)
        {
            // 1. Lấy dữ liệu mượn sách theo từng giao dịch (Phiếu mượn)
            var items = await _context.ChiTietMuonTras
                .Include(c => c.CuonSach)
                .Select(c => new { c.MaPhieuMuon, MaSach = c.CuonSach!.MaSach })
                .ToListAsync();

            var transactions = items
                .GroupBy(x => x.MaPhieuMuon)
                .Select(g => g.Select(x => x.MaSach).ToList())
                .ToList();

            var pairCounts = new Dictionary<Tuple<int, int>, int>();

            // 2. Thuật toán Apriori: Tạo ứng viên bộ 2 (2-itemsets)
            foreach (var transaction in transactions)
            {
                if (transaction.Count < 2) continue;

                var sortedItems = transaction.Distinct().OrderBy(i => i).ToList();
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
            var rawData = await _context.ChiTietMuonTras
                .Include(c => c.PhieuMuon)
                .Include(c => c.CuonSach)
                .ThenInclude(cs => cs!.Sach)
                .Where(c => c.PhieuMuon!.NgayMuon >= from && c.PhieuMuon!.NgayMuon <= to)
                .ToListAsync();

            return rawData
                .Where(c => c.CuonSach != null)
                .GroupBy(c => c.CuonSach!.MaSach)
                .Select(g => new BookStatDTO
                {
                    MaSach = g.Key,
                    TenSach = g.FirstOrDefault()?.CuonSach?.Sach?.TenSach ?? "Không xác định",
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .Take(top)
                .ToList();
        }

        /// <summary>
        /// Báo cáo độc giả quá hạn hoặc nợ tiền.
        /// Tính toán theo thời gian thực dựa trên HanTra.
        /// </summary>
        public async Task<List<ReaderStatDTO>> GetReaderOverdueReportsAsync()
        {
            return await _context.DocGias
                .Select(d => new ReaderStatDTO
                {
                    MaDocGia = d.MaDocGia,
                    HoTen = d.HoTen,
                    TongNo = d.TongNo,
                    OverdueCount = _context.ChiTietMuonTras
                        .Include(c => c.PhieuMuon)
                        .Count(c => c.PhieuMuon!.MaDocGia == d.MaDocGia && 
                                    c.NgayTraThucTe == null && 
                                    c.HanTra < DateTime.Now)
                })
                .Where(x => x.TongNo > 0 || x.OverdueCount > 0)
                .ToListAsync();
        }

        /// <summary>
        /// Báo cáo thống kê mượn sách theo thể loại.
        /// </summary>
        public async Task<IEnumerable<object>> GetBorrowingStatsByCategoryAsync(DateTime from, DateTime to)
        {
            var list = await _context.ChiTietMuonTras
                .Include(c => c.PhieuMuon)
                .Include(c => c.CuonSach)
                .ThenInclude(cs => cs!.Sach)
                .ThenInclude(s => s!.TheLoaiSach)
                .Where(c => c.PhieuMuon!.NgayMuon >= from && c.PhieuMuon!.NgayMuon <= to)
                .ToListAsync();

            return list
                .Where(c => c.CuonSach?.Sach?.TheLoaiSach != null)
                .GroupBy(c => c.CuonSach!.Sach!.TheLoaiSach!.TenTheLoai)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    BorrowCount = g.Count(),
                    Percentage = 0 // Sẽ tính toán tỷ lệ phần trăm ở tầng hiển thị
                })
                .ToList();
        }
    }
}
