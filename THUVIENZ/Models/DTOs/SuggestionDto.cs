namespace THUVIENZ.Models.DTOs
{
    /// <summary>
    /// Đối tượng vận chuyển dữ liệu (DTO) cho chức năng gợi ý tìm kiếm.
    /// Giúp chuẩn hóa dữ liệu trả về từ Backend cho Frontend dễ dàng hiển thị.
    /// </summary>
    public class SuggestionDto
    {
        /// <summary>
        /// Mã định danh của cuốn sách.
        /// </summary>
        public int MaSach { get; set; }

        /// <summary>
        /// Văn bản chính hiển thị (ví dụ: Tên sách).
        /// </summary>
        public string DisplayText { get; set; } = string.Empty;

        /// <summary>
        /// Văn bản phụ hiển thị (ví dụ: Tên tác giả).
        /// </summary>
        public string SecondaryText { get; set; } = string.Empty;

        /// <summary>
        /// Phân loại gợi ý: "Title" (theo tên) hoặc "Author" (theo tác giả).
        /// </summary>
        public string Loai { get; set; } = string.Empty;
    }
}
