using System;
using THUVIENZ.Core;

namespace THUVIENZ.Models
{
    /// <summary>
    /// Thực thể đại diện cho bảng THONGBAO trong Database.
    /// Áp dụng Strict Null Safety tuyệt đối và chú thích Tiếng Việt.
    /// </summary>
    public class ThongBao : ObservableObject
    {
        private int _maThongBao;
        /// <summary>
        /// Mã thông báo (Khóa chính tự tăng).
        /// </summary>
        public int MaThongBao
        {
            get => _maThongBao;
            set
            {
                _maThongBao = value;
                OnPropertyChanged();
            }
        }

        private int _maDocGia;
        /// <summary>
        /// Mã độc giả liên kết (Khóa ngoại).
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

        private string _tieuDe = string.Empty;
        /// <summary>
        /// Tiêu đề của thông báo.
        /// </summary>
        public string TieuDe
        {
            get => _tieuDe;
            set
            {
                _tieuDe = value;
                OnPropertyChanged();
            }
        }

        private string _noiDung = string.Empty;
        /// <summary>
        /// Nội dung chi tiết của thông báo.
        /// </summary>
        public string NoiDung
        {
            get => _noiDung;
            set
            {
                _noiDung = value;
                OnPropertyChanged();
            }
        }

        private NotificationType _loaiThongBao = NotificationType.Info;
        /// <summary>
        /// Loại thông báo (Success, Failure, Warning, Info).
        /// </summary>
        public NotificationType LoaiThongBao
        {
            get => _loaiThongBao;
            set
            {
                _loaiThongBao = value;
                OnPropertyChanged();
            }
        }

        private DateTime _ngayTao = DateTime.Now;
        /// <summary>
        /// Thời điểm tạo thông báo.
        /// </summary>
        public DateTime NgayTao
        {
            get => _ngayTao;
            set
            {
                _ngayTao = value;
                OnPropertyChanged();
            }
        }

        private bool _daDoc;
        /// <summary>
        /// Trạng thái đã đọc thông báo (0: Chưa đọc, 1: Đã đọc).
        /// </summary>
        public bool DaDoc
        {
            get => _daDoc;
            set
            {
                _daDoc = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Thực thể Độc giả liên kết (Quan hệ N-1).
        /// </summary>
        public virtual DocGia? DocGia { get; set; }
    }
}
