using System;
using System.Collections.Generic;
using THUVIENZ.Core;

namespace THUVIENZ.Models
{
    /// <summary>
    /// Thực thể đại diện cho bảng CUONSACH trong Database.
    /// Quản lý từng bản sao vật lý đơn lẻ của một đầu sách (dùng cho Kiosk/máy quét RFID).
    /// Áp dụng Strict Null Safety tuyệt đối và chú thích Tiếng Việt.
    /// </summary>
    public class CuonSach : ObservableObject
    {
        private int _maCuonSach;
        /// <summary>
        /// Mã cuốn sách vật lý (Khóa chính tự tăng, dán mã vạch/RFID trên gáy sách).
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

        private int _maSach;
        /// <summary>
        /// Mã đầu sách gốc (Khóa ngoại).
        /// </summary>
        public int MaSach
        {
            get => _maSach;
            set
            {
                _maSach = value;
                OnPropertyChanged();
            }
        }

        private string _tinhTrang = "Sẵn sàng";
        /// <summary>
        /// Tình trạng vật lý: 'Sẵn sàng', 'Đang mượn', 'Bị mất', hoặc 'Bảo trì'.
        /// </summary>
        public string TinhTrang
        {
            get => _tinhTrang;
            set
            {
                _tinhTrang = value;
                OnPropertyChanged();
            }
        }

        private DateTime _ngayNhap = DateTime.Now;
        /// <summary>
        /// Ngày nhập cuốn sách vật lý này vào kho.
        /// </summary>
        public DateTime NgayNhap
        {
            get => _ngayNhap;
            set
            {
                _ngayNhap = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Đối tượng Đầu Sách gốc liên kết (Quan hệ N-1).
        /// </summary>
        public virtual Sach? Sach { get; set; }

        /// <summary>
        /// Danh sách lịch sử giao dịch mượn/trả của cuốn sách này (Quan hệ 1-N).
        /// </summary>
        public virtual ICollection<ChiTietMuonTra> ChiTietMuonTras { get; set; } = new List<ChiTietMuonTra>();
    }
}
