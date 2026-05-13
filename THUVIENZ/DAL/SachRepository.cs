using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THUVIENZ.DAL.Base;
using THUVIENZ.Models;

namespace THUVIENZ.DAL
{
    /// <summary>
    /// Repository cụ thể xử lý các nghiệp vụ truy xuất dữ liệu cho Sách.
    /// Đã viết lại cơ chế thêm sách hỗ trợ sinh tự động các bản sao vật lý trong cùng Transaction.
    /// Áp dụng Strict Null Safety tuyệt đối và chú thích Tiếng Việt.
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
        /// Ghi đè phương thức thêm mới sách.
        /// Khi giao diện UI gửi xuống 1 đối tượng Đầu Sách kèm thuộc tính SoLuong,
        /// hệ thống lưu Đầu Sách trước, sau đó dùng vòng lặp for tạo danh sách CuonSach vật lý
        /// và lưu toàn bộ trong cùng một Transaction để đảm bảo tính toàn vẹn dữ liệu.
        /// </summary>
        public override async Task AddAsync(Sach entity)
        {
            // Bắt đầu một Transaction để đảm bảo tính nguyên tử (Atomicity)
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Thêm Đầu Sách vào Database trước để EF Core tự động sinh MaSach
                await _context.Sachs.AddAsync(entity);
                await _context.SaveChangesAsync();

                // 2. Lấy số lượng bản sao cần tạo từ thuộc tính UI binding (tối thiểu là 1)
                int copiesCount = entity.SoLuong > 0 ? entity.SoLuong : 1;

                // 3. Dùng vòng lặp for sinh danh sách các đối tượng CuonSach tương ứng
                for (int i = 0; i < copiesCount; i++)
                {
                    var cuonSach = new CuonSach
                    {
                        MaSach = entity.MaSach,
                        TinhTrang = "Sẵn sàng",
                        NgayNhap = DateTime.Now
                    };
                    await _context.CuonSachs.AddAsync(cuonSach);
                }

                // 4. Lưu tiếp danh sách các bản sao vật lý vào Database
                await _context.SaveChangesAsync();

                // 5. Xác nhận hoàn tất toàn bộ chuỗi thao tác thành công
                await transaction.CommitAsync();
            }
            catch
            {
                // Hoàn tác (Rollback) toàn bộ dữ liệu nếu xảy ra bất kỳ lỗi nào
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra xem một đầu sách có bất kỳ bản sao vật lý nào đang được mượn hay không.
        /// Dựa trên bảng mới CUONSACH.
        /// </summary>
        public bool IsBookCurrentlyBorrowed(int maSach)
        {
            return _context.CuonSachs.Any(cs => cs.MaSach == maSach && cs.TinhTrang == "Đang mượn");
        }

        public override async Task<Sach?> GetByIdAsync(object id)
        {
            if (id is int maSach)
            {
                return await _context.Sachs
                    .Include(s => s.CuonSachs)
                    .Include(s => s.TheLoaiSach)
                    .FirstOrDefaultAsync(s => s.MaSach == maSach);
            }
            return await base.GetByIdAsync(id);
        }

        public override async Task<IEnumerable<Sach>> GetAllAsync()
        {
            return await _context.Sachs
                .Include(s => s.CuonSachs)
                .Include(s => s.TheLoaiSach)
                .ToListAsync();
        }

        /// <summary>
        /// Tìm kiếm sách theo Tên sách, Mã ISBN, Tác giả hoặc Mã sách.
        /// </summary>
        public async Task<IEnumerable<Sach>> SearchBooksAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllAsync();

            string trimmedKeyword = keyword.Trim();
            bool isId = int.TryParse(trimmedKeyword, out int id);

            return await _context.Sachs
                .Include(s => s.CuonSachs)
                .Include(s => s.TheLoaiSach)
                .Where(s => s.TenSach.Contains(trimmedKeyword) || 
                            (s.MaISBN != null && s.MaISBN.Contains(trimmedKeyword)) ||
                            (s.TacGia != null && s.TacGia.Contains(trimmedKeyword)) || 
                            (isId && s.MaSach == id) ||
                            s.MaSach.ToString().Contains(trimmedKeyword))
                .ToListAsync();
        }
    }
}
