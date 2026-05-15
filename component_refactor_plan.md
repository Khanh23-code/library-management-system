# Mosco "Quiet Luxury" Design System - Refactoring Plan

Bản kế hoạch này được đệ trình để Lead xem xét và duyệt trước khi tiến hành tái cấu trúc (Refactor) hệ thống UI của dự án Mosco (quy mô 20.000+ thẻ bài).

Mục tiêu chính: **Loại bỏ sự trùng lặp (DRY), đồng bộ hóa trải nghiệm người dùng (UX) và tối ưu hóa khả năng mở rộng (Scalability).**

---

## 🛑 User Review Required (Cần Lead duyệt)

> [!IMPORTANT]
> Việc áp dụng Base Component (đặc biệt là BaseActivity) sẽ yêu cầu cập nhật toàn bộ 16 màn hình hiện tại. Chúng ta cần đảm bảo logic Edge-to-edge (tràn viền) không làm vỡ các màn hình cũ.
> **Câu hỏi cho Lead:** Chúng ta nên refactor theo kiểu "Cuốn chiếu" (làm màn hình nào, refactor màn hình đó) hay thực hiện "Big Bang" (đổi toàn bộ project sang Base mới trong 1 nhánh)?

> [!WARNING]
> Nút bấm ở màn hình **Upgrade** hiện tại đang sai chuẩn nghiêm trọng (dùng `layout_height_percent="0.07"`, tạo ra một hình khối to, vuông vức, màu xám xịt). Nó hoàn toàn "lạc quẻ" so với các nút bo góc 56dp cực kỳ tinh tế ở màn **Spin** và **Stage (K-Tour)**. Tôi sẽ đưa việc fix nút Upgrade này lên độ ưu tiên cao nhất trong phân hệ Buttons.

---

## 🏗️ Proposed Changes (Các thay đổi đề xuất)

### 1. Phân hệ Nền tảng (The Skeleton)
Chuẩn hóa cấu trúc xương sống cho toàn bộ dự án để tránh lặp code WindowInsets và Loading.

#### [NEW] `com.vn.jet.mosco.base.BaseMoscoActivity`
- Quản lý cấu hình Edge-to-edge (tràn viền) cho mọi màn hình.
- Cung cấp hàm `showLoading()` và `hideLoading()` dùng chung (Shimmer/Overlay).

#### [NEW] `com.vn.jet.mosco.base.BaseMoscoFragment`
- Kế thừa các đặc tính tương tự Activity, chuyên biệt cho Fragment.

---

### 2. Phân hệ Nguyên tử (Atoms: Buttons & Inputs)
Đồng bộ hóa 100% các nút bấm và ô tìm kiếm.

#### [MODIFY] Hệ thống `MoscoButton`
- Xóa bỏ việc fix cứng chiều cao bằng `percent` (như lỗi ở màn Upgrade).
- Chuẩn hóa toàn bộ Primary Button về chiều cao `56dp`, `cornerRadius="28dp"`, với màu Gradient/Solid thống nhất (lấy chuẩn từ màn Spin/Stage).
- Xây dựng Custom View `MoscoButton` (kế thừa AppCompatButton) để tự động xử lý logic *Click Debouncing* (chống spam click) và trạng thái *Disabled* (màu xám tối).

#### [NEW] Component `MoscoSearchBar`
- Nâng cấp từ `InventoryFilterBar` hiện tại.
- Bổ sung ô nhập liệu `EditText` (với icon kính lúp) thiết kế theo phong cách Glassmorphism (Kính mờ) để tái sử dụng ở màn Friend, Inventory, và Shop.

---

### 3. Phân hệ Hiển thị Lõi (The Core Molecule: Objet Card)
Giải quyết triệt để 80% code XML đang bị lặp lại ở các màn hình chứa thẻ bài.

#### [NEW] Component `ObjetCardView`
- Đóng gói toàn bộ XML lõi (Ảnh 1:1.54, Name Tag, OVR, Badge, Lock Overlay) vào một Custom View.
- Cung cấp API đơn giản cho Adapter: `cardView.bind(CollectionEntry)` hoặc `cardView.setMissingState()`.
- **Loại bỏ hoàn toàn** các file XML trùng lặp như `item_collection_book_card.xml`, `item_inventory_card.xml`, `item_spin_card.xml`.

---

### 4. Phân hệ Tương tác (Dialogs & Messages)
Loại bỏ việc sử dụng giao diện Dialog/Toast mặc định của Android.

#### [NEW] Component `MoscoDialogManager`
- Khai tử việc `new android.app.Dialog()` rải rác khắp nơi.
- Tạo một khuôn mẫu `MoscoDialog` (hoặc `GalacticBottomSheet`) duy nhất quy định chặt chẽ:
  - **Header**: Font Pretendard, in đậm, căn giữa.
  - **Body**: Text xám sáng (OnSurfaceVariant).
  - **Action**: Dùng chuẩn `MoscoButton` đã quy định ở trên (1 nút Primary, 1 nút Ghost/TextOnly).
- Tích hợp hiệu ứng mờ nền (Blur) khi Dialog xuất hiện.

#### [NEW] Component `MoscoNotification` (Thay thế Toast)
- Xây dựng Notification Banner hiển thị từ trên xuống (hoặc dưới lên) với viền Neon và background Dark. Tránh dùng Toast hệ thống gây mất chất "Quiet Luxury".

---

## 🧪 Verification Plan (Kế hoạch Kiểm duyệt)

### Automated Tests
- Chạy linter để đảm bảo không còn file XML nào sử dụng `android:layout_height_percent` cho Button.
- Build và cài đặt trên Giả lập Android 9 để kiểm tra hiệu năng của Custom Views.

### Manual Verification
1. Review màn hình **Upgrade**: Nút bấm đã được bo tròn, thu gọn và có màu sắc đồng bộ với Spin chưa?
2. Bật một Dialog bất kỳ (vd: Exit Confirm): Giao diện có chuẩn chỉ với cấu trúc Header/Body/Button mới không?
3. Review Code: Cấu trúc Adapter có giảm thiểu được hàng chục dòng code thừa nhờ `ObjetCardView` hay không.
