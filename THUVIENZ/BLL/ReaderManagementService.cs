using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THUVIENZ.BLL.Base;
using THUVIENZ.DAL;
using THUVIENZ.Models;

namespace THUVIENZ.BLL
{
    /// <summary>
    /// Service quản lý độc giả cho Admin.
    /// Kế thừa BaseService để đảm bảo nguyên tắc DRY.
    /// </summary>
    public class ReaderManagementService : BaseService<DocGia>
    {
        private readonly DocGiaRepository _docGiaRepository;

        public ReaderManagementService() : this(new DocGiaRepository(new LmsDbContext()))
        {
        }

        public ReaderManagementService(DocGiaRepository repository) : base(repository)
        {
            _docGiaRepository = repository;
        }

        /// <summary>
        /// Lấy danh sách tổng hợp độc giả và số sách đang mượn để hiển thị trên UI.
        /// </summary>
        public async Task<IEnumerable<object>> GetAdminReaderListAsync()
        {
            return await _docGiaRepository.GetReadersWithBorrowedCountAsync();
        }

        /// <summary>
        /// Logic thay đổi loại độc giả kèm theo kiểm tra tính hợp lệ.
        /// </summary>
        public async Task ChangeReaderTypeAsync(int readerId, int newTypeId)
        {
            var reader = await _repository.GetByIdAsync(readerId);
            if (reader == null) throw new KeyNotFoundException("Không tìm thấy độc giả.");

            reader.MaLoaiDocGia = newTypeId;
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteReaderAsync(int readerId)
        {
            await DeleteAsync(readerId);
        }
    }
}
