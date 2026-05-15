using System;
using THUVIENZ.Core;

namespace THUVIENZ.Models
{
    /// <summary>
    /// Thực thể đại diện cho bảng PHIEUTHUTIENPHAT trong Database.
    /// Lưu trữ thông tin thu tiền phạt trễ hạn của độc giả.
    /// Áp dụng Strict Null Safety tuyệt đối và chú thích Tiếng Việt.
    /// </summary>
    public class PhieuThuTienPhat : ObservableObject
    {
        private int _maPhieuThu;
        /// <summary>
        /// Mã phiếu thu (Khóa chính tự tăng).
        /// </summary>
        public int MaPhieuThu
        {
            get => _maPhieuThu;
            set
            {
                _maPhieuThu = value;
                OnPropertyChanged();
            }
        }

        private int _maDocGia;
        /// <summary>
        /// Mã độc giả nộp phạt.
        /// </summary>
        public int MaDocGia
        {
            get => _maDocGia;
            set
            {
                _maDocGia = value;
                OnPropertyChanged();
            }
        }

        private decimal _soTienThu;
        /// <summary>
        /// Số tiền thực thu.
        /// </summary>
        public decimal SoTienThu
        {
            get => _soTienThu;
            set
            {
                _soTienThu = value;
                OnPropertyChanged();
            }
        }

        private DateTime _ngayThu = DateTime.Now;
        /// <summary>
        /// Ngày thực hiện thu tiền phạt.
        /// </summary>
        public DateTime NgayThu
        {
            get => _ngayThu;
            set
            {
                _ngayThu = value;
                OnPropertyChanged();
            }
        }

        private string? _ghiChu;
        /// <summary>
        /// Ghi chú thêm về giao dịch thu tiền (nếu có).
        /// </summary>
        public string? GhiChu
        {
            get => _ghiChu;
            set
            {
                _ghiChu = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Đối tượng Độc giả liên kết (Quan hệ N-1).
        /// </summary>
        public virtual DocGia? DocGia { get; set; }
    }
}
