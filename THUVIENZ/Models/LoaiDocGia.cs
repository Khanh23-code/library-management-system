using System.Collections.Generic;
using THUVIENZ.Core;

namespace THUVIENZ.Models
{
    /// <summary>
    /// Thực thể đại diện cho bảng LOAIDOCGIA trong Database.
    /// Quản lý phân loại đối tượng độc giả (Sinh viên, Giảng viên...).
    /// Áp dụng Strict Null Safety tuyệt đối và chú thích Tiếng Việt.
    /// </summary>
    public class LoaiDocGia : ObservableObject
    {
        private int _maLoaiDocGia;
        /// <summary>
        /// Mã loại độc giả (Khóa chính tự tăng).
        /// </summary>
        public int MaLoaiDocGia
        {
            get => _maLoaiDocGia;
            set
            {
                _maLoaiDocGia = value;
                OnPropertyChanged();
            }
        }

        private string _tenLoaiDocGia = string.Empty;
        /// <summary>
        /// Tên loại độc giả.
        /// </summary>
        public string TenLoaiDocGia
        {
            get => _tenLoaiDocGia;
            set
            {
                _tenLoaiDocGia = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Danh sách các độc giả thuộc loại này (Quan hệ 1-N).
        /// </summary>
        public virtual ICollection<DocGia> DocGias { get; set; } = new List<DocGia>();
    }
}
