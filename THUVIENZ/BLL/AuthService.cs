using System;
using System.Threading.Tasks;
using THUVIENZ.DAL;
using THUVIENZ.Models;
using BCrypt.Net;

namespace THUVIENZ.BLL
{
    /// <summary>
    /// Service xử lý các nghiệp vụ liên quan đến xác thực và phân quyền.
    /// Đã nâng cấp bảo mật bằng BCrypt kèm cơ chế tương thích ngược (Fallback).
    /// </summary>
    public class AuthService
    {
        private readonly TaiKhoanRepository _taiKhoanRepository;

        public AuthService() : this(new TaiKhoanRepository())
        {
        }

        public AuthService(TaiKhoanRepository repository)
        {
            _taiKhoanRepository = repository;
        }

        /// <summary>
        /// Xử lý logic đăng nhập với cơ chế kiểm tra BCrypt Hash và Plaintext.
        /// </summary>
        public async Task<string?> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            try
            {
                TaiKhoan? account = await _taiKhoanRepository.GetAccountByUsernameAsync(username);

                if (account == null) return null;

                if (account.TrangThai != "Active")
                {
                    return "PENDING_OR_LOCKED";
                }

                bool isPasswordValid = false;

                try
                {
                    isPasswordValid = BCrypt.Net.BCrypt.Verify(password, account.MatKhau);
                }
                catch (Exception)
                {
                    if (password == account.MatKhau)
                    {
                        isPasswordValid = true;
                    }
                }

                if (isPasswordValid)
                {
                    return account.Quyen;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi Auth: " + ex.Message);
                throw;
            }

            return null;
        }
    }
}
