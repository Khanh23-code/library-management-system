using THUVIENZ.Core;

namespace THUVIENZ.Models
{
    /// <summary>
    /// Thực thể đại diện cho bảng THAMSO trong Database.
    /// Lưu trữ các quy định, tham số cấu hình linh hoạt của thư viện.
    /// Toàn bộ code áp dụng Strict Null Safety và chú thích Tiếng Việt.
    /// </summary>
    public class ThamSo : ObservableObject
    {
        private string _tenThamSo = string.Empty;
        /// <summary>
        /// Tên tham số (Khóa chính).
        /// </summary>
        public string TenThamSo
        {
            get => _tenThamSo;
            set
            {
                _tenThamSo = value;
                OnPropertyChanged();
            }
        }

        private double _giaTri;
        /// <summary>
        /// Giá trị của tham số (dạng số thực).
        /// </summary>
        public double GiaTri
        {
            get => _giaTri;
            set
            {
                _giaTri = value;
                OnPropertyChanged();
            }
        }
    }
}
