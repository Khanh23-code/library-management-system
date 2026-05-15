using System;
using THUVIENZ.Core;

namespace THUVIENZ.Models
{
    /// <summary>
    /// Thực thể đại diện cho bảng CHITIETMUONTRA trong Database.
    /// Gộp chung toàn bộ logic theo dõi giao dịch Mượn và Trả sách vật lý.
    /// Áp dụng Strict Null Safety tuyệt đối và chú thích Tiếng Việt.
    /// </summary>
    public class ChiTietMuonTra : ObservableObject
    {
        private int _maPhieuMuon;
        /// <summary>
        /// Mã phiếu mượn (Một phần của Khóa chính phức hợp, Khóa ngoại).
        /// </summary>
        public int MaPhieuMuon
        {
            get => _maPhieuMuon;
            set
            {
                _maPhieuMuon = value;
                OnPropertyChanged();
            }
        }

        private int _maCuonSach;
        /// <summary>
        /// Mã cuốn sách vật lý (Một phần của Khóa chính phức hợp, Khóa ngoại).
        /// </summary>
        public int MaCuonSach
        {
            get => _maCuonSach;
            set
            {
                _maCuonSach = value;
                OnPropertyChanged();
            }
        }

        private DateTime _hanTra;
        /// <summary>
        /// Hạn trả sách dự kiến (được tính trước bằng C# khi tạo phiếu).
        /// </summary>
        public DateTime HanTra
        {
            get => _hanTra;
            set
            {
                _hanTra = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _ngayTraThucTe;
        /// <summary>
        /// Ngày trả thực tế (NULL nếu sách đang được mượn và chưa trả).
        /// </summary>
        public DateTime? NgayTraThucTe
        {
            get => _ngayTraThucTe;
            set
            {
                _ngayTraThucTe = value;
                OnPropertyChanged();
            }
        }

        private decimal _tienPhat;
        /// <summary>
        /// Số tiền phạt phát sinh nếu trả trễ hạn hoặc làm hỏng sách.
        /// </summary>
        public decimal TienPhat
        {
            get => _tienPhat;
            set
            {
                _tienPhat = value;
                OnPropertyChanged();
            }
        }

        private string? _tinhTrangCuonSachKhiTra;
        /// <summary>
        /// Tình trạng vật lý của cuốn sách khi độc giả mang trả.
        /// </summary>
        public string? TinhTrangCuonSachKhiTra
        {
            get => _tinhTrangCuonSachKhiTra;
            set
            {
                _tinhTrangCuonSachKhiTra = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Đối tượng Phiếu Mượn gốc liên kết (Quan hệ N-1).
        /// </summary>
        public virtual PhieuMuon? PhieuMuon { get; set; }

        /// <summary>
        /// Đối tượng Cuốn Sách vật lý liên kết (Quan hệ N-1).
        /// </summary>
        public virtual CuonSach? CuonSach { get; set; }
    }
}
