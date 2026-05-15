-- ======================================================================
-- SCRIPT SINH DỮ LIỆU MẪU (DUMMY DATA) PHONG PHÚ CHO HỆ THỐNG
-- Cung cấp đầy đủ dữ liệu test cho: Độc giả, Yêu cầu đăng ký (Pending),
-- Kho sách vật lý, Giao dịch đang mượn, Lịch sử trả sách và Sách quá hạn.
-- Tích hợp bộ dữ liệu trễ hạn chuyên sâu theo từng ngưỡng cài đặt xử phạt.
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
IF NOT EXISTS (SELECT 1 FROM THELOAISACH WHERE MaTheLoai = 4)
    INSERT INTO THELOAISACH (MaTheLoai, TenTheLoai) VALUES (4, N'Ngoại ngữ');
IF NOT EXISTS (SELECT 1 FROM THELOAISACH WHERE MaTheLoai = 5)
    INSERT INTO THELOAISACH (MaTheLoai, TenTheLoai) VALUES (5, N'Kỹ năng sống');
IF NOT EXISTS (SELECT 1 FROM THELOAISACH WHERE MaTheLoai = 6)
    INSERT INTO THELOAISACH (MaTheLoai, TenTheLoai) VALUES (6, N'Lịch sử & Địa lý');
IF NOT EXISTS (SELECT 1 FROM THELOAISACH WHERE MaTheLoai = 7)
    INSERT INTO THELOAISACH (MaTheLoai, TenTheLoai) VALUES (7, N'Tâm lý học');
IF NOT EXISTS (SELECT 1 FROM THELOAISACH WHERE MaTheLoai = 8)
    INSERT INTO THELOAISACH (MaTheLoai, TenTheLoai) VALUES (8, N'Triết học');
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
IF NOT EXISTS (SELECT 1 FROM THAMSO WHERE TenThamSo = 'TongNoToiDa')
    INSERT INTO THAMSO (TenThamSo, GiaTri) VALUES ('TongNoToiDa', 50000);
GO

-- 3. TÀI KHOẢN (Bao gồm Active gốc và các tài khoản chuyên dụng cho test trễ hạn)
IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TenDangNhap = 'reader_vana')
    INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES ('reader_vana', '123456', 'Reader', 'Active');
IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TenDangNhap = 'reader_thib')
    INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES ('reader_thib', '123456', 'Reader', 'Active');
IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TenDangNhap = 'reader_leminh')
    INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES ('reader_leminh', '123456', 'Reader', 'Active');

-- Các tài khoản trễ hạn chuyên sâu
IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TenDangNhap = 'reader_trehan1')
    INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES ('reader_trehan1', '123456', 'Reader', 'Active');
IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TenDangNhap = 'reader_trehan2')
    INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES ('reader_trehan2', '123456', 'Reader', 'Active');
IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TenDangNhap = 'reader_trehan3')
    INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES ('reader_trehan3', '123456', 'Reader', 'Active');

-- Tài khoản Pending
IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TenDangNhap = 'pending_user1')
    INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES ('pending_user1', 'password123', 'Reader', 'Pending');
IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TenDangNhap = 'pending_user2')
    INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES ('pending_user2', 'securepass', 'Reader', 'Pending');
GO

-- 4. ĐỘC GIẢ
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

-- Các độc giả trễ hạn minh họa cụ thể cho từng ngưỡng cài đặt
IF NOT EXISTS (SELECT 1 FROM DOCGIA WHERE MaDocGia = 6)
    INSERT INTO DOCGIA (MaDocGia, TenDangNhap, HoTen, MaLoaiDocGia, GioiTinh, SoDienThoai, Email, DiaChi, NgaySinh, TongNo) 
    VALUES (6, 'reader_trehan1', N'Trần Trễ Hạn Nhẹ', 1, N'Nữ', '0933112233', 'trehan1@gmail.com', N'Quận 10, TP.HCM', '2001-04-12', 0);

IF NOT EXISTS (SELECT 1 FROM DOCGIA WHERE MaDocGia = 7)
    INSERT INTO DOCGIA (MaDocGia, TenDangNhap, HoTen, MaLoaiDocGia, GioiTinh, SoDienThoai, Email, DiaChi, NgaySinh, TongNo) 
    VALUES (7, 'reader_trehan2', N'Phạm Nợ Ngập Đầu', 1, N'Nam', '0944112233', 'trehan2@gmail.com', N'Quận Gò Vấp, TP.HCM', '2000-09-09', 60000); -- Nợ gốc vượt ngưỡng 50k

IF NOT EXISTS (SELECT 1 FROM DOCGIA WHERE MaDocGia = 8)
    INSERT INTO DOCGIA (MaDocGia, TenDangNhap, HoTen, MaLoaiDocGia, GioiTinh, SoDienThoai, Email, DiaChi, NgaySinh, TongNo) 
    VALUES (8, 'reader_trehan3', N'Hoàng Mượn Kịch Trần', 2, N'Nam', '0966112233', 'trehan3@gmail.com', N'TP. Thủ Đức, TP.HCM', '1992-02-02', 0);
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

IF NOT EXISTS (SELECT 1 FROM SACH WHERE MaSach = 104)
    INSERT INTO SACH (MaSach, MaISBN, TenSach, MaTheLoai, TacGia, NhaXuatBan, NamXuatBan, TriGia, MoTa) 
    VALUES (104, 'ISBN-9780321125217', N'Domain-Driven Design: Tackling Complexity in the Heart of Software', 1, N'Eric Evans', N'Addison-Wesley', 2003, 550000, N'Sách chuyên sâu về kiến trúc và thiết kế hướng tên miền');

IF NOT EXISTS (SELECT 1 FROM SACH WHERE MaSach = 105)
    INSERT INTO SACH (MaSach, MaISBN, TenSach, MaTheLoai, TacGia, NhaXuatBan, NamXuatBan, TriGia, MoTa) 
    VALUES (105, 'ISBN-9780134685991', N'Effective Java (3rd Edition)', 1, N'Joshua Bloch', N'Addison-Wesley', 2017, 480000, N'Sách cẩm nang các quy tắc tối ưu cho lập trình viên Java');
SET IDENTITY_INSERT SACH OFF;
GO

-- 6. TẠO CÁC BẢN SAO VẬT LÝ (CUONSACH)
-- Bổ sung đầy đủ sách vật lý cho các giao dịch
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
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (102, N'Đang mượn');
END;

IF NOT EXISTS (SELECT 1 FROM CUONSACH WHERE MaSach = 103)
BEGIN
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (103, N'Đang mượn');
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (103, N'Sẵn sàng');
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (103, N'Đang mượn');
END;

IF NOT EXISTS (SELECT 1 FROM CUONSACH WHERE MaSach = 104)
BEGIN
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (104, N'Đang mượn');
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (104, N'Đang mượn');
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (104, N'Đang mượn');
END;

IF NOT EXISTS (SELECT 1 FROM CUONSACH WHERE MaSach = 105)
BEGIN
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (105, N'Đang mượn');
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (105, N'Đang mượn');
END;
GO

-- 7. PHIẾU MƯỢN & CHI TIẾT MƯỢN TRẢ MẪU
DELETE FROM CHITIETMUONTRA;
DELETE FROM PHIEUMUON;
GO

SET IDENTITY_INSERT PHIEUMUON ON;
-- Phiếu 1: Nguyễn Văn A mượn
INSERT INTO PHIEUMUON (MaPhieuMuon, MaDocGia, NgayMuon) VALUES (1, 1, DATEADD(day, -10, GETDATE()));
-- Phiếu 2: Trần Thị B mượn
INSERT INTO PHIEUMUON (MaPhieuMuon, MaDocGia, NgayMuon) VALUES (2, 2, DATEADD(day, -3, GETDATE()));
-- Phiếu 3: Lê Minh C mượn
INSERT INTO PHIEUMUON (MaPhieuMuon, MaDocGia, NgayMuon) VALUES (3, 3, DATEADD(day, -25, GETDATE()));

-- Phiếu 4: Trần Trễ Hạn Nhẹ (Trường hợp 1: Trễ 5 ngày để test phạt tiền)
INSERT INTO PHIEUMUON (MaPhieuMuon, MaDocGia, NgayMuon) VALUES (4, 6, DATEADD(day, -19, GETDATE()));

-- Phiếu 5: Phạm Nợ Ngập Đầu (Trường hợp 2: Trễ 30 ngày, cộng nợ cũ vượt 50k)
INSERT INTO PHIEUMUON (MaPhieuMuon, MaDocGia, NgayMuon) VALUES (5, 7, DATEADD(day, -44, GETDATE()));

-- Phiếu 6: Hoàng Mượn Kịch Trần (Trường hợp 3: Mượn tối đa 5 cuốn sách)
INSERT INTO PHIEUMUON (MaPhieuMuon, MaDocGia, NgayMuon) VALUES (6, 8, DATEADD(day, -5, GETDATE()));
SET IDENTITY_INSERT PHIEUMUON OFF;
GO

-- Lấy ra danh sách các mã cuốn sách vật lý ngẫu nhiên để map vào phiếu mượn
DECLARE @cs1 INT, @cs2 INT, @cs3 INT, @cs4 INT, @cs5 INT, @cs6 INT, @cs7 INT, @cs8 INT, @cs9 INT, @cs10 INT, @cs11 INT;

-- Gán mã cuốn sách tuần tự từ các bảng
SELECT TOP 1 @cs1 = MaCuonSach FROM CUONSACH WHERE MaSach = 101 ORDER BY MaCuonSach ASC;
SELECT TOP 1 @cs2 = MaCuonSach FROM CUONSACH WHERE MaSach = 101 AND MaCuonSach > @cs1 ORDER BY MaCuonSach ASC;
SELECT TOP 1 @cs3 = MaCuonSach FROM CUONSACH WHERE MaSach = 102 ORDER BY MaCuonSach ASC;
SELECT TOP 1 @cs4 = MaCuonSach FROM CUONSACH WHERE MaSach = 103 ORDER BY MaCuonSach ASC;
SELECT TOP 1 @cs5 = MaCuonSach FROM CUONSACH WHERE MaSach = 102 AND MaCuonSach > @cs3 ORDER BY MaCuonSach ASC;
SELECT TOP 1 @cs6 = MaCuonSach FROM CUONSACH WHERE MaSach = 103 AND MaCuonSach > @cs4 ORDER BY MaCuonSach ASC;
SELECT TOP 1 @cs7 = MaCuonSach FROM CUONSACH WHERE MaSach = 104 ORDER BY MaCuonSach ASC;
SELECT TOP 1 @cs8 = MaCuonSach FROM CUONSACH WHERE MaSach = 104 AND MaCuonSach > @cs7 ORDER BY MaCuonSach ASC;
SELECT TOP 1 @cs9 = MaCuonSach FROM CUONSACH WHERE MaSach = 104 AND MaCuonSach > @cs8 ORDER BY MaCuonSach ASC;
SELECT TOP 1 @cs10 = MaCuonSach FROM CUONSACH WHERE MaSach = 105 ORDER BY MaCuonSach ASC;
SELECT TOP 1 @cs11 = MaCuonSach FROM CUONSACH WHERE MaSach = 105 AND MaCuonSach > @cs10 ORDER BY MaCuonSach ASC;

-- Chi tiết Phiếu 1:
IF @cs1 IS NOT NULL INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat) VALUES (1, @cs1, DATEADD(day, 4, GETDATE()), DATEADD(day, -5, GETDATE()), 0);
IF @cs2 IS NOT NULL INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat) VALUES (1, @cs2, DATEADD(day, 4, GETDATE()), NULL, 0);

-- Chi tiết Phiếu 2:
IF @cs3 IS NOT NULL INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat) VALUES (2, @cs3, DATEADD(day, 11, GETDATE()), NULL, 0);

-- Chi tiết Phiếu 3:
IF @cs4 IS NOT NULL INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat) VALUES (3, @cs4, DATEADD(day, -11, GETDATE()), NULL, 0);

-- Chi tiết Phiếu 4 (Trần Trễ Hạn Nhẹ - Quá hạn 5 ngày):
IF @cs5 IS NOT NULL INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat) VALUES (4, @cs5, DATEADD(day, -5, GETDATE()), NULL, 0);

-- Chi tiết Phiếu 5 (Phạm Nợ Ngập Đầu - Quá hạn 30 ngày):
IF @cs6 IS NOT NULL INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat) VALUES (5, @cs6, DATEADD(day, -30, GETDATE()), NULL, 0);

-- Chi tiết Phiếu 6 (Hoàng Mượn Kịch Trần - 5 cuốn cùng lúc đang giữ bình thường):
IF @cs7 IS NOT NULL INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat) VALUES (6, @cs7, DATEADD(day, 9, GETDATE()), NULL, 0);
IF @cs8 IS NOT NULL INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat) VALUES (6, @cs8, DATEADD(day, 9, GETDATE()), NULL, 0);
IF @cs9 IS NOT NULL INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat) VALUES (6, @cs9, DATEADD(day, 9, GETDATE()), NULL, 0);
IF @cs10 IS NOT NULL INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat) VALUES (6, @cs10, DATEADD(day, 9, GETDATE()), NULL, 0);
IF @cs11 IS NOT NULL INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat) VALUES (6, @cs11, DATEADD(day, 9, GETDATE()), NULL, 0);
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

PRINT N'---> Khởi tạo và làm phong phú toàn bộ dữ liệu mẫu (Dummy Data) kèm kịch bản trễ hạn thành công!';
