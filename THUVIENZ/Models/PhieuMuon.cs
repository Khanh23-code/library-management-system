using System;
using System.Collections.Generic;
using THUVIENZ.Core;

namespace THUVIENZ.Models
{
    /// <summary>
    /// Thực thể đại diện cho bảng PHIEUMUON trong Database.
    /// Lưu vết các lần thực hiện giao dịch mượn sách của độc giả.
    /// Áp dụng Strict Null Safety tuyệt đối và chú thích Tiếng Việt.
    /// </summary>
    public class PhieuMuon : ObservableObject
    {
        private int _maPhieuMuon;
        /// <summary>
        /// Mã phiếu mượn (Khóa chính tự tăng).
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

        private int _maDocGia;
        /// <summary>
        /// Mã độc giả thực hiện giao dịch mượn (Khóa ngoại).
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

        private DateTime _ngayMuon = DateTime.Now;
        /// <summary>
        /// Ngày giờ lập phiếu mượn.
        /// </summary>
        public DateTime NgayMuon
        {
            get => _ngayMuon;
            set
            {
                _ngayMuon = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Đối tượng Độc giả liên kết (Quan hệ N-1).
        /// </summary>
        public virtual DocGia? DocGia { get; set; }

        /// <summary>
        /// Danh sách các chi tiết mượn/trả thuộc phiếu này (Quan hệ 1-N).
        /// </summary>
        public virtual ICollection<ChiTietMuonTra> ChiTietMuonTras { get; set; } = new List<ChiTietMuonTra>();
    }
}
