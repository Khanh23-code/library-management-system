using System.Collections.Generic;
using THUVIENZ.Core;

namespace THUVIENZ.Models
{
    /// <summary>
    /// Thực thể đại diện cho bảng TAIKHOAN trong Database.
    /// Đã khóa cứng quyền (Admin, Reader) và trạng thái kích hoạt.
    /// Áp dụng Strict Null Safety tuyệt đối theo tiêu chuẩn dự án.
    /// </summary>
    public class TaiKhoan : ObservableObject
    {
        private string _tenDangNhap = string.Empty;
        /// <summary>
        /// Tên đăng nhập (Khóa chính).
        /// </summary>
        public string TenDangNhap
        {
            get => _tenDangNhap;
            set
            {
                _tenDangNhap = value;
                OnPropertyChanged();
            }
        }

        private string _matKhau = string.Empty;
        /// <summary>
        /// Mật khẩu đăng nhập.
        /// </summary>
        public string MatKhau
        {
            get => _matKhau;
            set
            {
                _matKhau = value;
                OnPropertyChanged();
            }
        }

        private string _quyen = string.Empty;
        /// <summary>
        /// Quyền hạn: 'Admin' hoặc 'Reader'.
        /// </summary>
        public string Quyen
        {
            get => _quyen;
            set
            {
                _quyen = value;
                OnPropertyChanged();
            }
        }

        private string _trangThai = "Pending";
        /// <summary>
        /// Trạng thái tài khoản: 'Pending', 'Active', 'Locked' hoặc 'DisActive'.
        /// </summary>
        public string TrangThai
        {
            get => _trangThai;
            set
            {
                _trangThai = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Đối tượng Độc giả ánh xạ 1-1 với tài khoản này (nếu có).
        /// </summary>
        public virtual DocGia? DocGia { get; set; }
    }
}
