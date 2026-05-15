using System.Collections.Generic;
using THUVIENZ.Core;

namespace THUVIENZ.Models
{
    /// <summary>
    /// Thực thể đại diện cho bảng THELOAISACH trong Database.
    /// Quản lý danh mục chuyên ngành của sách.
    /// Áp dụng Strict Null Safety tuyệt đối và chú thích Tiếng Việt.
    /// </summary>
    public class TheLoaiSach : ObservableObject
    {
        private int _maTheLoai;
        /// <summary>
        /// Mã thể loại sách (Khóa chính tự tăng).
        /// </summary>
        public int MaTheLoai
        {
            get => _maTheLoai;
            set
            {
                _maTheLoai = value;
                OnPropertyChanged();
            }
        }

        private string _tenTheLoai = string.Empty;
        /// <summary>
        /// Tên thể loại sách.
        /// </summary>
        public string TenTheLoai
        {
            get => _tenTheLoai;
            set
            {
                _tenTheLoai = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Danh sách các đầu sách thuộc thể loại này (Quan hệ 1-N).
        /// </summary>
        public virtual ICollection<Sach> Sachs { get; set; } = new List<Sach>();
    }
}
