-- ======================================================================
-- SCRIPT SINH DỮ LIỆU MẪU (DUMMY DATA) PHONG PHÚ CHO HỆ THỐNG
-- Cung cấp đầy đủ dữ liệu test cho: Độc giả, Yêu cầu đăng ký (Pending),
-- Kho sách vật lý, Giao dịch đang mượn, Lịch sử trả sách và Sách quá hạn.
-- ======================================================================
USE QL_ThuVien;
GO

-- 1. THỂ LOẠI SÁCH
SET IDENTITY_INSERT THELOAISACH ON;
IF NOT EXISTS (SELECT 1 FROM THELOAISACH WHERE MaTheLoai = 1)
    INSERT INTO THELOAISACH (MaTheLoai, TenTheLoai) VALUES (1, N'Khoa học Công nghệ');
IF NOT EXISTS (SELECT 1 FROM THELOAISACH WHERE MaTheLoai = 2)
    INSERT INTO THELOAISACH (MaTheLoai, TenTheLoai) VALUES (2, N'Văn học Nghệ thuật');
IF NOT EXISTS (SELECT 1 FROM THELOAISACH WHERE MaTheLoai = 3)
    INSERT INTO THELOAISACH (MaTheLoai, TenTheLoai) VALUES (3, N'Kinh tế & Quản trị');
SET IDENTITY_INSERT THELOAISACH OFF;
GO

-- 2. LOẠI ĐỘC GIẢ
SET IDENTITY_INSERT LOAIDOCGIA ON;
IF NOT EXISTS (SELECT 1 FROM LOAIDOCGIA WHERE MaLoaiDocGia = 1)
    INSERT INTO LOAIDOCGIA (MaLoaiDocGia, TenLoaiDocGia) VALUES (1, N'Sinh viên');
IF NOT EXISTS (SELECT 1 FROM LOAIDOCGIA WHERE MaLoaiDocGia = 2)
    INSERT INTO LOAIDOCGIA (MaLoaiDocGia, TenLoaiDocGia) VALUES (2, N'Giảng viên');
SET IDENTITY_INSERT LOAIDOCGIA OFF;
GO

-- 2B. THAM SỐ HỆ THỐNG
IF NOT EXISTS (SELECT 1 FROM THAMSO WHERE TenThamSo = 'SoSachMuonToiDa')
    INSERT INTO THAMSO (TenThamSo, GiaTri) VALUES ('SoSachMuonToiDa', 5);
IF NOT EXISTS (SELECT 1 FROM THAMSO WHERE TenThamSo = 'SoNgayMuonToiDa')
    INSERT INTO THAMSO (TenThamSo, GiaTri) VALUES ('SoNgayMuonToiDa', 14);
IF NOT EXISTS (SELECT 1 FROM THAMSO WHERE TenThamSo = 'TienPhatMoiNgay')
    INSERT INTO THAMSO (TenThamSo, GiaTri) VALUES ('TienPhatMoiNgay', 2000);
GO

-- 3. TÀI KHOẢN (Bao gồm Active và các tài khoản đang yêu cầu đăng ký - Pending)
IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TenDangNhap = 'reader_vana')
    INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES ('reader_vana', '123456', 'Reader', 'Active');
IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TenDangNhap = 'reader_thib')
    INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES ('reader_thib', '123456', 'Reader', 'Active');
IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TenDangNhap = 'reader_leminh')
    INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES ('reader_leminh', '123456', 'Reader', 'Active');

-- Thêm các tài khoản Pending để test chức năng Duyệt/Xóa yêu cầu đăng ký của Admin
IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TenDangNhap = 'pending_user1')
    INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES ('pending_user1', 'password123', 'Reader', 'Pending');
IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TenDangNhap = 'pending_user2')
    INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES ('pending_user2', 'securepass', 'Reader', 'Pending');
GO

-- 4. ĐỘC GIẢ (Có đầy đủ Giới tính và SĐT để test giao diện và tìm kiếm)
SET IDENTITY_INSERT DOCGIA ON;
IF NOT EXISTS (SELECT 1 FROM DOCGIA WHERE MaDocGia = 1)
    INSERT INTO DOCGIA (MaDocGia, TenDangNhap, HoTen, MaLoaiDocGia, GioiTinh, SoDienThoai, Email, DiaChi, NgaySinh, TongNo) 
    VALUES (1, 'reader_vana', N'Nguyễn Văn A', 1, N'Nam', '0901234567', 'vana@gmail.com', N'Quận 1, TP.HCM', '2003-05-15', 0);

IF NOT EXISTS (SELECT 1 FROM DOCGIA WHERE MaDocGia = 2)
    INSERT INTO DOCGIA (MaDocGia, TenDangNhap, HoTen, MaLoaiDocGia, GioiTinh, SoDienThoai, Email, DiaChi, NgaySinh, TongNo) 
    VALUES (2, 'reader_thib', N'Trần Thị B', 2, N'Nữ', '0919876543', 'thib@gmail.com', N'Quận Bình Thạnh, TP.HCM', '1995-08-22', 0);

IF NOT EXISTS (SELECT 1 FROM DOCGIA WHERE MaDocGia = 3)
    INSERT INTO DOCGIA (MaDocGia, TenDangNhap, HoTen, MaLoaiDocGia, GioiTinh, SoDienThoai, Email, DiaChi, NgaySinh, TongNo) 
    VALUES (3, 'reader_leminh', N'Lê Minh C', 1, N'Nam', '0988112233', 'leminh@gmail.com', N'Quận 7, TP.HCM', '2002-12-01', 15000);

IF NOT EXISTS (SELECT 1 FROM DOCGIA WHERE MaDocGia = 4)
    INSERT INTO DOCGIA (MaDocGia, TenDangNhap, HoTen, MaLoaiDocGia, GioiTinh, SoDienThoai, Email, DiaChi, NgaySinh, TongNo) 
    VALUES (4, 'pending_user1', N'Nguyễn Chờ Duyệt 1', 1, N'Nam', '0911223344', 'pending1@gmail.com', N'Quận 3, TP.HCM', '2004-01-10', 0);

IF NOT EXISTS (SELECT 1 FROM DOCGIA WHERE MaDocGia = 5)
    INSERT INTO DOCGIA (MaDocGia, TenDangNhap, HoTen, MaLoaiDocGia, GioiTinh, SoDienThoai, Email, DiaChi, NgaySinh, TongNo) 
    VALUES (5, 'pending_user2', N'Trần Chờ Duyệt 2', 2, N'Nữ', '0955667788', 'pending2@gmail.com', N'Quận 5, TP.HCM', '1998-06-15', 0);
SET IDENTITY_INSERT DOCGIA OFF;
GO

-- 5. ĐẦU SÁCH (SACH)
SET IDENTITY_INSERT SACH ON;
IF NOT EXISTS (SELECT 1 FROM SACH WHERE MaSach = 101)
    INSERT INTO SACH (MaSach, MaISBN, TenSach, MaTheLoai, TacGia, NhaXuatBan, NamXuatBan, TriGia, MoTa) 
    VALUES (101, 'ISBN-9780132350884', N'Clean Code: A Handbook of Agile Software Craftsmanship', 1, N'Robert C. Martin', N'Prentice Hall', 2008, 500000, N'Sách hướng dẫn viết mã sạch và tối ưu hóa phần mềm');

IF NOT EXISTS (SELECT 1 FROM SACH WHERE MaSach = 102)
    INSERT INTO SACH (MaSach, MaISBN, TenSach, MaTheLoai, TacGia, NhaXuatBan, NamXuatBan, TriGia, MoTa) 
    VALUES (102, 'ISBN-9780201633610', N'Design Patterns: Elements of Reusable Object-Oriented Software', 1, N'Erich Gamma', N'Addison-Wesley', 1994, 450000, N'Sách kinh điển về các mẫu thiết kế hướng đối tượng');

IF NOT EXISTS (SELECT 1 FROM SACH WHERE MaSach = 103)
    INSERT INTO SACH (MaSach, MaISBN, TenSach, MaTheLoai, TacGia, NhaXuatBan, NamXuatBan, TriGia, MoTa) 
    VALUES (103, 'ISBN-9781491950296', N'Programming C# 8.0: Build Cloud, Web, and Desktop Applications', 1, N'Ian Griffiths', N'O''Reilly Media', 2019, 600000, N'Cẩm nang toàn diện về lập trình C# và nền tảng .NET');
SET IDENTITY_INSERT SACH OFF;
GO

-- 6. TẠO CÁC BẢN SAO VẬT LÝ (CUONSACH)
-- Kiểm tra và chèn các bản sao vật lý cho các đầu sách trên nếu kho trống
IF NOT EXISTS (SELECT 1 FROM CUONSACH WHERE MaSach = 101)
BEGIN
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (101, N'Sẵn sàng');
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (101, N'Đang mượn');
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (101, N'Sẵn sàng');
END;

IF NOT EXISTS (SELECT 1 FROM CUONSACH WHERE MaSach = 102)
BEGIN
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (102, N'Sẵn sàng');
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (102, N'Đang mượn');
END;

IF NOT EXISTS (SELECT 1 FROM CUONSACH WHERE MaSach = 103)
BEGIN
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (103, N'Đang mượn'); -- Cuốn này cho mượn quá hạn
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (103, N'Sẵn sàng');
END;
GO

-- 7. PHIẾU MƯỢN & CHI TIẾT MƯỢN TRẢ MẪU (LƯU VẾT GIAO DỊCH PHONG PHÚ)
-- Xóa tạm các chi tiết mượn trả cũ để chèn bộ test đầy đủ
DELETE FROM CHITIETMUONTRA;
DELETE FROM PHIEUMUON;
GO

SET IDENTITY_INSERT PHIEUMUON ON;
-- Phiếu 1: Nguyễn Văn A mượn (Có 1 cuốn đã trả, 1 cuốn đang mượn bình thường)
INSERT INTO PHIEUMUON (MaPhieuMuon, MaDocGia, NgayMuon) VALUES (1, 1, DATEADD(day, -10, GETDATE()));

-- Phiếu 2: Trần Thị B mượn (Đang mượn bình thường chưa tới hạn)
INSERT INTO PHIEUMUON (MaPhieuMuon, MaDocGia, NgayMuon) VALUES (2, 2, DATEADD(day, -3, GETDATE()));

-- Phiếu 3: Lê Minh C mượn (Bị trễ hạn / Quá hạn để test báo cáo Overdue Report)
INSERT INTO PHIEUMUON (MaPhieuMuon, MaDocGia, NgayMuon) VALUES (3, 3, DATEADD(day, -25, GETDATE()));
SET IDENTITY_INSERT PHIEUMUON OFF;
GO

-- Lấy ra các ID CuonSach ngẫu nhiên tương ứng để map vào chi tiết
DECLARE @cs101_1 INT, @cs101_2 INT, @cs102_1 INT, @cs103_1 INT;
SELECT TOP 1 @cs101_1 = MaCuonSach FROM CUONSACH WHERE MaSach = 101 ORDER BY MaCuonSach ASC;
SELECT TOP 1 @cs101_2 = MaCuonSach FROM CUONSACH WHERE MaSach = 101 AND MaCuonSach > @cs101_1 ORDER BY MaCuonSach ASC;
SELECT TOP 1 @cs102_1 = MaCuonSach FROM CUONSACH WHERE MaSach = 102 ORDER BY MaCuonSach ASC;
SELECT TOP 1 @cs103_1 = MaCuonSach FROM CUONSACH WHERE MaSach = 103 ORDER BY MaCuonSach ASC;

-- Chi tiết Phiếu 1:
-- Cuốn 1 (Đã trả xong):
IF @cs101_1 IS NOT NULL
    INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat, TinhTrangCuonSachKhiTra)
    VALUES (1, @cs101_1, DATEADD(day, 4, GETDATE()), DATEADD(day, -5, GETDATE()), 0, N'Sẵn sàng');

-- Cuốn 2 (Đang mượn):
IF @cs101_2 IS NOT NULL
    INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat, TinhTrangCuonSachKhiTra)
    VALUES (1, @cs101_2, DATEADD(day, 4, GETDATE()), NULL, 0, NULL);

-- Chi tiết Phiếu 2 (Đang mượn):
IF @cs102_1 IS NOT NULL
    INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat, TinhTrangCuonSachKhiTra)
    VALUES (2, @cs102_1, DATEADD(day, 11, GETDATE()), NULL, 0, NULL);

-- Chi tiết Phiếu 3 (Quá hạn trả sách, HanTra trong quá khứ):
IF @cs103_1 IS NOT NULL
    INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat, TinhTrangCuonSachKhiTra)
    VALUES (3, @cs103_1, DATEADD(day, -11, GETDATE()), NULL, 0, NULL);
GO

-- Đồng bộ lại trạng thái TinhTrang trong bảng CUONSACH cho khớp thực tế luồng giao dịch
UPDATE CS
SET TinhTrang = N'Đang mượn'
FROM CUONSACH CS
WHERE MaCuonSach IN (SELECT MaCuonSach FROM CHITIETMUONTRA WHERE NgayTraThucTe IS NULL);

UPDATE CS
SET TinhTrang = N'Sẵn sàng'
FROM CUONSACH CS
WHERE MaCuonSach NOT IN (SELECT MaCuonSach FROM CHITIETMUONTRA WHERE NgayTraThucTe IS NULL);
GO

PRINT N'---> Khởi tạo và làm phong phú toàn bộ dữ liệu mẫu (Dummy Data) thành công!';
