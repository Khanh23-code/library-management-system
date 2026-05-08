using System.Collections.Generic;
using System.Threading.Tasks;
using THUVIENZ.BLL.Base;
using THUVIENZ.DAL;
using THUVIENZ.Models;

namespace THUVIENZ.BLL
{
    /// <summary>
    /// Service xử lý nghiệp vụ duyệt và từ chối tài khoản độc giả mới.
    /// Kế thừa BaseService và tuân thủ các quy tắc của Tech Lead.
    /// </summary>
    public class AccountApprovalService : BaseService<TaiKhoan>
    {
        private readonly TaiKhoanRepository _taiKhoanRepository;

        public AccountApprovalService() : this(new TaiKhoanRepository(new LmsDbContext()))
        {
        }

        public AccountApprovalService(TaiKhoanRepository repository) : base(repository)
        {
            _taiKhoanRepository = repository;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách tài khoản đang chờ phê duyệt.
        /// Chú thích bằng tiếng Việt.
        /// </summary>
        public async Task<IEnumerable<TaiKhoan>> GetPendingAccountsAsync()
        {
            return await _taiKhoanRepository.GetPendingAccountsAsync();
        }

        /// <summary>
        /// Phê duyệt tài khoản và chuyển sang trạng thái Active.
        /// </summary>
        public async Task ApproveAccountAsync(string username)
        {
            var account = await _repository.GetByIdAsync(username);
            if (account != null)
            {
                account.TrangThai = "Active";
                await _repository.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Từ chối tài khoản và xóa bỏ thông tin liên quan trong hệ thống.
        /// </summary>
        public async Task RejectAccountAsync(string username)
        {
            // Xóa bỏ thông tin nếu bị từ chối thông qua Repository chuyên biệt
            await _taiKhoanRepository.DeleteAccountAndReaderAsync(username);
        }
    }
}
