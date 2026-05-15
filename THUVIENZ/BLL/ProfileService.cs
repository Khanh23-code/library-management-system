using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THUVIENZ.DAL;
using THUVIENZ.Models;
using THUVIENZ.DAL.Base;

namespace THUVIENZ.BLL
{
    /// <summary>
    /// Service xử lý logic liên quan đến hồ sơ Độc giả.
    /// Tuân thủ kiến trúc mới của Tech Lead.
    /// </summary>
    public class ProfileService
    {
        private readonly DocGiaRepository _docGiaRepository;

        public ProfileService() : this(new DocGiaRepository(new LmsDbContext()))
        {
        }

        public ProfileService(DocGiaRepository repository)
        {
            _docGiaRepository = repository;
        }

        /// <summary>
        /// Lấy thông tin cá nhân của Độc giả.
        /// </summary>
        public async Task<DocGia?> GetReaderInfoAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            return await _docGiaRepository.GetReaderProfileAsync(username);
        }

        /// <summary>
        /// Lấy danh sách sách đang mượn của Độc giả.
        /// </summary>
        public async Task<IEnumerable<Sach>> GetActiveBorrowedBooksAsync(int maDocGia)
        {
            if (maDocGia <= 0) return Enumerable.Empty<Sach>();
            return await _docGiaRepository.GetBorrowedBooksAsync(maDocGia);
        }
    }
}
