using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using THUVIENZ.BLL.Base;
using THUVIENZ.DAL;
using THUVIENZ.Models;

namespace THUVIENZ.BLL
{
    /// <summary>
    /// Service quản lý các hoạt động cho sách.
    /// Kế thừa BaseService và tuân thủ các quy tắc của Tech Lead.
    /// </summary>
    public class BookManagementService : BaseService<Sach>
    {
        private readonly SachRepository _sachRepository;

        public BookManagementService() : this(new SachRepository(new LmsDbContext()))
        {
        }

        public BookManagementService(SachRepository repository) : base(repository)
        {
            _sachRepository = repository;
        }

        /// <summary>
        /// Thêm sách mới kèm theo kiểm tra tính hợp lệ.
        /// </summary>
        public async Task AddBookAsync(Sach book)
        {
            if (string.IsNullOrWhiteSpace(book.TenSach))
                throw new ArgumentException("Tên sách không được để trống.");

            await AddAsync(book);
        }

        /// <summary>
        /// Xóa sách nếu sách không đang trong trạng thái được mượn.
        /// </summary>
        public async Task DeleteBookAsync(int maSach)
        {
            // Quy tắc nghiệp vụ: Không thể xóa sách đang được mượn
            if (_sachRepository.IsBookCurrentlyBorrowed(maSach))
            {
                throw new InvalidOperationException("Quy tắc nghiệp vụ: Sách đang ở trạng thái 'Đang mượn', không thể xóa khỏi hệ thống.");
            }

            await DeleteAsync(maSach);
        }

        /// <summary>
        /// Xử lý tải lên ảnh bìa sách với 3 lớp bảo mật: Giới hạn dung lượng, Whitelist đuôi file, và kiểm tra Magic Numbers.
        /// </summary>
        /// <param name="maSach">Mã cuốn sách cần cập nhật ảnh.</param>
        /// <param name="imageStream">Luồng dữ liệu ảnh từ Frontend.</param>
        /// <param name="extension">Đuôi file (ví dụ: .jpg, .png).</param>
        public async Task<string> UploadBookCoverAsync(int maSach, Stream imageStream, string extension, bool updateDatabase = true)
        {
            // --- LỚP BẢO MẬT 1: KIỂM TRA TỒN TẠI VÀ DUNG LƯỢNG ---
            using var context = new DAL.LmsDbContext();
            var book = await context.Sachs.FindAsync(maSach);
            if (book == null) throw new KeyNotFoundException("Không tìm thấy sách để cập nhật ảnh.");

            if (imageStream == null || imageStream.Length == 0)
                throw new ArgumentException("Dữ liệu hình ảnh không hợp lệ.");

            // Chặn file > 5MB để tránh tấn công DoS
            const long MaxFileSize = 5 * 1024 * 1024; 
            if (imageStream.Length > MaxFileSize)
                throw new ArgumentException("Dung lượng ảnh vượt quá giới hạn cho phép (Tối đa 5MB).");

            // --- LỚP BẢO MẬT 2: WHITELIST ĐUÔI FILE ---
            string ext = extension.ToLower();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            if (Array.IndexOf(allowedExtensions, ext) == -1)
                throw new SecurityException("Định dạng file không được phép. Chỉ chấp nhận .jpg, .jpeg, .png.");

            // --- LỚP BẢO MẬT 3: KIỂM TRA CHỮ KÝ FILE (MAGIC NUMBERS) ---
            // Đọc 8 byte đầu tiên để xác thực nội dung thực sự của file
            byte[] header = new byte[8];
            long originalPosition = imageStream.Position;
            await imageStream.ReadAsync(header, 0, 8);
            
            // Trả lại vị trí ban đầu của stream để sau đó copy file không bị mất dữ liệu
            if (imageStream.CanSeek) imageStream.Position = originalPosition;

            bool isJpeg = header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
            bool isPng = header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47;

            if (!isJpeg && !isPng)
                throw new SecurityException("Nội dung file không phải là hình ảnh hợp lệ (Fake extension).");

            // --- TIẾN HÀNH LƯU FILE KHI ĐÃ AN TOÀN ---
            string fileName = $"{Guid.NewGuid()}{ext}";
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Images");
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await imageStream.CopyToAsync(fileStream);
            }

            if (updateDatabase)
            {
                // Cập nhật trực tiếp cột HinhAnh bằng SQL thuần để tránh lỗi xung đột phiên bản (Concurrency OCC)
                await context.Database.ExecuteSqlRawAsync("UPDATE SACH SET HinhAnh = {0} WHERE MaSach = {1}", fileName, maSach);
            }

            return fileName;
        }
    }
}
