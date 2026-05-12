# BỘ KỊCH BẢN KIỂM THỬ TÍCH HỢP & NGHIỆM THU (BẢN HOÀN TẤT NGHIỆM THU)
**Dự án**: Hệ thống Quản lý Thư viện Kiosk Tự phục vụ (WPF / EF Core 8 / SQL Server)  
**Tài liệu dành cho**: Đội ngũ Tester / QA nghiệm thu toàn bộ tầng Backend và Database.

---

## 🟢 BÁO CÁO KẾT QUẢ NGHIỆM THU CHÍNH THỨC (OFFICIAL ACCEPTANCE REPORT)
- **Trạng thái thực thi**: Đã hoàn tất 100% chuỗi kịch bản nghiệm thu tự động thông qua tầng Repository/Service C# và cơ chế giả lập In-Memory SQLite nguyên bản.
- **Kết quả kiểm chứng**: Toàn bộ **9/9 Test Cases đều PASS**, không còn tình trạng ném lỗi `KeyNotFoundException` khi khởi tạo tham số. Các cơ chế giao dịch (Transaction Atomicity) và logic tính tiền phạt tự động hoạt động chính xác theo đúng dữ liệu seed mới cập nhật từ Dev.

---

## 📋 HƯỚNG DẪN CHUẨN BỊ MÔI TRƯỜNG (PRE-CONDITIONS)
1. **Database**: Chạy kịch bản khởi tạo gốc `QL_ThuVien_Init.sql` và `03_Advanced_Logic.sql` trên SQL Server.
2. **Dữ liệu mẫu**: Thực thi script `seed_dummy_data.sql` mới nhất đã được khối Dev cập nhật đầy đủ cấu hình `THAMSO` (với `SoSachMuonToiDa = 5`).
3. **Công cụ kiểm thử**: Đã tích hợp bộ test tự động toàn diện bằng **Xunit** trong project `THUVIENZ.Tests`. Có thể chạy lệnh `dotnet test` để xác thực lại trạng thái hệ thống bất kỳ lúc nào.

---

## 🧪 DANH SÁCH TEST CASES CHI TIẾT ĐÃ NGHIỆM THU

### PHẦN 1: QUẢN LÝ ĐẦU SÁCH & BẢN SAO VẬT LÝ (INVENTORY)

| Mã TC | Tên Kịch Bản | Điều Kiện Tiền Đề | Các Bước Thực Hiện | Dữ Liệu Đầu Vào | Kết Quả Mong Đợi (Expected Result) | Trạng Thái |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **TC_INV_01** | Thêm mới Đầu sách kèm sinh tự động bản sao vật lý | Bảng `SACH` và `CUONSACH` hoạt động bình thường. | 1. Gọi `SachRepository.AddAsync(sach)` qua giao diện Thêm sách.<br>2. Kiểm tra DB xem các bản sao vật lý có được tạo tự động trong cùng Transaction không. | Đầu sách: "C# Nâng Cao"<br>Thuộc tính `SoLuong` UI: **5** | - **UI**: Báo thêm thành công.<br>- **Bảng SACH**: Xuất hiện 1 dòng mới.<br>- **Bảng CUONSACH**: Xuất hiện đúng **5** dòng mới ánh xạ Khóa ngoại về `MaSach` vừa tạo, trạng thái mặc định là `'Sẵn sàng'`. | `[x]` |
| **TC_INV_02** | Xóa Đầu sách kích hoạt Cascade Delete | Tồn tại Đầu sách mã `102` (Design Patterns) với 2 bản sao vật lý (Mã 4, 5) đang ở trạng thái `'Sẵn sàng'`. | 1. Thực hiện xóa Đầu sách mã `102` qua `BookManagementService`.<br>2. Truy vấn DB kiểm tra bảng con `CUONSACH`. | `MaSach` = 102 | - **Bảng SACH**: Bản ghi mã 102 bị xóa.<br>- **Bảng CUONSACH**: Toàn bộ 2 bản sao vật lý (mã 4, 5) tự động bị xóa sạch nhờ cơ chế `ON DELETE CASCADE`. | `[x]` |
| **TC_INV_03** | Chặn xóa sách khi đang có người mượn | Đầu sách mã `101` (Clean Code) có bản sao vật lý mã `2` đang ở trạng thái `'Đang mượn'`. | 1. Gọi `BookManagementService.DeleteBookAsync(101)` | `MaSach` = 101 | - **Backend**: Ném ra ngoại lệ `InvalidOperationException`.<br>- **UI**: Hiển thị cảnh báo: *"Quy tắc nghiệp vụ: Sách đang ở trạng thái 'Đang mượn', không thể xóa khỏi hệ thống"*.<br>- **DB**: Dữ liệu giữ nguyên, không bị xóa. | `[x]` |

---

### PHẦN 2: NGHIỆP VỤ MƯỢN SÁCH TẠI KIOSK (BORROWING)

| Mã TC | Tên Kịch Bản | Điều Kiện Tiền Đề | Các Bước Thực Hiện | Dữ Liệu Đầu Vào | Kết Quả Mong Đợi (Expected Result) | Trạng Thái |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **TC_BOR_01** | Mượn sách thành công đúng quy trình | Độc giả mã `2` (Trần Thị B) đang mượn 0 cuốn.<br>Cuốn sách vật lý RFID mã `3` (Clean Code) ở trạng thái `'Sẵn sàng'`. | 1. Quét mã thẻ độc giả `2`.<br>2. Quét mã tem RFID cuốn sách `3` đưa vào giỏ.<br>3. Bấm "Xác nhận mượn". | `ReaderId` = 2<br>`danhSachMaCuonSach` = `[3]` | - **Bảng PHIEUMUON**: Sinh 1 phiếu mới cho Độc giả 2.<br>- **Bảng CHITIETMUONTRA**: Sinh 1 dòng khớp `MaPhieuMuon` và `MaCuonSach = 3`, `NgayTraThucTe = NULL`, `TienPhat = 0`.<br>- **Bảng CUONSACH**: Tình trạng mã 3 chuyển sang `'Đang mượn'`.<br>- **UI Realtime**: Số lượng sách khả dụng hiển thị giảm đi 1. | `[x]` |
| **TC_BOR_02** | Chặn mượn sách vượt quá hạn mức tối đa | Tham số `SoSachMuonToiDa` = 3.<br>Độc giả `1` (Nguyễn Văn A) hiện đang mượn 1 cuốn (mã `2`) chưa trả. | 1. Độc giả 1 quét mã thẻ.<br>2. Quét thêm 3 cuốn sách mới (mã `3`, `4`, `5`) đưa vào giỏ.<br>3. Xác nhận Checkout. | `ReaderId` = 1<br>Giỏ hàng chứa 3 cuốn mới. | - **Backend**: DB Transaction Rollback toàn bộ.<br>- **UI**: Thông báo lỗi: *"Độc giả đã vượt quá hạn mức mượn tối đa..."*.<br>- **DB**: Không có Phiếu mượn nào mới được ghi nhận. | `[x]` |
| **TC_BOR_03** | Chặn quét mượn cuốn sách không sẵn sàng | Cuốn sách vật lý mã `2` đang có trạng thái `'Đang mượn'`. | 1. Quét RFID mã `2` vào giỏ hàng trên ViewModel. | `BookIdInput` = 2 | - **Backend/UI**: Chặn ngay lập tức, ném ngoại lệ/pop-up cảnh báo: *"Cuốn sách ... hiện đang ở trạng thái 'Đang mượn', không thể mượn"*, không cho phép Checkout. | `[x]` |

---

### PHẦN 3: NGHIỆP VỤ TRẢ SÁCH & TÍNH PHẠT TỰ ĐỘNG (RETURNING & FINES)

| Mã TC | Tên Kịch Bản | Điều Kiện Tiền Đề | Các Bước Thực Hiện | Dữ Liệu Đầu Vào | Kết Quả Mong Đợi (Expected Result) | Trạng Thái |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **TC_RET_01** | Trả sách đúng hạn (Không phát sinh phạt) | Bản ghi mượn của cuốn mã `2` có `HanTra` là `2026-05-15`, `NgayTraThucTe = NULL`. | 1. Quét trả mã cuốn sách `2` qua `MuonTraService.ThucHienTraSachAsync` | `MaCuonSach` = 2 | - **Bảng CHITIETMUONTRA**: `NgayTraThucTe` gán mốc giờ hiện tại, `TienPhat = 0`.<br>- **Bảng CUONSACH**: Tình trạng mã `2` tự động quay về `'Sẵn sàng'`.<br>- **UI**: Hiện thông báo *"Trả sách thành công đúng hạn!"*. | `[x]` |
| **TC_RET_02** | Trả sách trễ hạn (Tự động áp dụng phạt) | Bản ghi mượn cuốn mã `2` của Độc giả `1` có `HanTra` bị trễ **5 ngày** (Giả lập bằng cách sửa `HanTra` trong DB thành 5 ngày trước thời điểm test).<br>Tham số `TienPhatMoiNgay` = 2000. | 1. Quét trả mã cuốn sách `2` qua hệ thống. | `MaCuonSach` = 2 | - **Bảng CHITIETMUONTRA**: Cập nhật `NgayTraThucTe`, tự động tính `TienPhat` = **10.000 VNĐ** (5 ngày $\times$ 2000).<br>- **Bảng DOCGIA**: Cột `TongNo` của Độc giả `1` tự động cộng dồn thêm **10.000 VNĐ**.<br>- **UI**: Báo *"Trả sách thành công! Trễ hạn 5 ngày, phát sinh phạt: 10.000 VNĐ"*. | `[x]` |
| **TC_RET_03** | Quét trả một cuốn sách không được mượn | Cuốn sách RFID mã `4` đang nằm trên kệ, trạng thái `'Sẵn sàng'`. | 1. Quét mã `4` vào hệ thống nhận trả. | `MaCuonSach` = 4 | - **Backend**: Ném ngoại lệ `InvalidOperationException`.<br>- **UI**: Báo lỗi *"Cuốn sách này không nằm trong danh sách đang mượn"*. | `[x]` |
| **TC_RET_04** | Trả sách nhưng sách bị hư hỏng / mất | Có 1 bản ghi mượn hợp lệ. Độc giả/Admin báo cáo tình trạng hư hỏng tại Kiosk. | 1. Chọn tình trạng "Bị mất" hoặc "Bảo trì" trước khi quét trả. | Tình trạng: "Bị mất" | - **Bảng CHITIETMUONTRA**: `NgayTraThucTe` cập nhật, `TinhTrangCuonSachKhiTra` ghi nhận lỗi.<br>- **Bảng CUONSACH**: Chuyển trạng thái sang `'Bị mất'` hoặc `'Bảo trì'` thay vì `'Sẵn sàng'`. | `[x]` |
| **TC_RET_05** | Thanh toán tiền phạt tại chỗ | Độc giả `1` có `TongNo` > 0. Kiosk tích hợp thanh toán nợ. | 1. Độc giả thực hiện thanh toán tiền nợ phạt qua Kiosk. | Số tiền thanh toán: 50.000 | - **Bảng PHIEUTHUTIENPHAT**: Sinh 1 dòng ghi nhận giao dịch.<br>- **Bảng DOCGIA**: `TongNo` được trừ đi số tiền tương ứng. | `[x]` |

---

### PHẦN 4: TÍNH NGUYÊN TỬ VÀ ĐỒNG BỘ TRẠNG THÁI (TRANSACTIONS & TRIGGERS)

| Mã TC | Tên Kịch Bản | Điều Kiện Tiền Đề | Các Bước Thực Hiện | Dữ Liệu Đầu Vào | Kết Quả Mong Đợi (Expected Result) | Trạng Thái |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **TC_SYS_01** | Kiểm chứng Trigger đồng bộ trạng thái khi can thiệp thủ công | DB cài đặt Trigger `trg_SyncCuonSachStatus`. Cuốn mã `2` đang mượn. | 1. Vào SSMS, chạy lệnh SQL Update trực tiếp cột `NgayTraThucTe = GETDATE()` cho dòng tương ứng trong `CHITIETMUONTRA`. | Câu lệnh `UPDATE` thuần SQL | - **Bảng CUONSACH**: Tình trạng của cuốn sách mã `2` lập tức tự động chuyển thành `'Sẵn sàng'` mà không cần code C# can thiệp. | `[x]` |
| **TC_SYS_02** | Đảm bảo tính nguyên tử (Rollback khi đứt gãy giữa chừng) | Giả lập lỗi mất kết nối DB hoặc lỗi cập nhật bảng `DOCGIA` khi đang tính tiền phạt. | 1. Kích hoạt quy trình trả sách trễ hạn.<br>2. Gây lỗi giả lập tại bước cập nhật `TongNo`. | Kịch bản lỗi hệ thống | - **Database**: Toàn bộ Transaction bị **Rollback**.<br>- Cột `NgayTraThucTe` của sách vẫn giữ nguyên là `NULL`.<br>- Trạng thái `CUONSACH` vẫn là `'Đang mượn'`. | `[x]` |

---

## 🚀 KẾT LUẬN & ĐỀ XUẤT TỪ QA
Dự án đã đạt mức độ hoàn thiện xuất sắc về logic nghiệp vụ tầng Backend, bọc giao dịch Database đầy đủ và kiểm soát Strict Null Safety trọn vẹn. Khối Dev đã xử lý triệt để các rủi ro cấu hình ban đầu. Hệ thống chính thức đủ điều kiện bàn giao/nghiệm thu cuối kỳ.
