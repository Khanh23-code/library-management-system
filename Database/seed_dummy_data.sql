-- ======================================================================
-- SCRIPT SINH DỮ LIỆU MẪU (DUMMY DATA) CHO HỆ THỐNG MỚI
-- Minh họa mô hình tách biệt SACH và CUONSACH kèm luồng Mượn/Trả gộp
-- ======================================================================
USE QL_ThuVien;
GO

-- 1. THỂ LOẠI SÁCH
SET IDENTITY_INSERT THELOAISACH ON;
INSERT INTO THELOAISACH (MaTheLoai, TenTheLoai) VALUES
(1, N'Khoa học Công nghệ'),
(2, N'Văn học Nghệ thuật'),
(3, N'Kinh tế & Quản trị');
SET IDENTITY_INSERT THELOAISACH OFF;
GO

-- 2. LOẠI ĐỘC GIẢ
SET IDENTITY_INSERT LOAIDOCGIA ON;
INSERT INTO LOAIDOCGIA (MaLoaiDocGia, TenLoaiDocGia) VALUES
(1, N'Sinh viên'),
(2, N'Giảng viên');
SET IDENTITY_INSERT LOAIDOCGIA OFF;
GO

-- 2B. THAM SỐ HỆ THỐNG
INSERT INTO THAMSO (TenThamSo, GiaTri) VALUES
('SoSachMuonToiDa', 5),
('SoNgayMuonToiDa', 14),
('TienPhatMoiNgay', 2000);
GO

-- 3. TÀI KHOẢN
INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES
('reader_vana', '123456', 'Reader', 'Active'),
('reader_thib', '123456', 'Reader', 'Active');
GO

-- 4. ĐỘC GIẢ (Có đầy đủ Giới tính và SĐT)
SET IDENTITY_INSERT DOCGIA ON;
INSERT INTO DOCGIA (MaDocGia, TenDangNhap, HoTen, MaLoaiDocGia, GioiTinh, SoDienThoai, Email, DiaChi, NgaySinh) VALUES
(1, 'reader_vana', N'Nguyễn Văn A', 1, N'Nam', '0901234567', 'vana@gmail.com', N'Quận 1, TP.HCM', '2003-05-15'),
(2, 'reader_thib', N'Trần Thị B', 2, N'Nữ', '0919876543', 'thib@gmail.com', N'Quận Bình Thạnh, TP.HCM', '1995-08-22');
SET IDENTITY_INSERT DOCGIA OFF;
GO

-- 5. ĐẦU SÁCH (SACH)
SET IDENTITY_INSERT SACH ON;
INSERT INTO SACH (MaSach, MaISBN, TenSach, MaTheLoai, TacGia, NhaXuatBan, NamXuatBan, TriGia, MoTa) VALUES
(101, 'ISBN-9780132350884', N'Clean Code: A Handbook of Agile Software Craftsmanship', 1, N'Robert C. Martin', N'Prentice Hall', 2008, 500000, N'Sách hướng dẫn viết mã sạch'),
(102, 'ISBN-9780201633610', N'Design Patterns: Elements of Reusable Object-Oriented Software', 1, N'Erich Gamma', N'Addison-Wesley', 1994, 450000, N'Sách kinh điển về mẫu thiết kế');
SET IDENTITY_INSERT SACH OFF;
GO

-- 6. TẠO CÁC BẢN SAO VẬT LÝ (CUONSACH) BẰNG VÒNG LẶP
-- A. Tạo 3 cuốn vật lý cho đầu sách Clean Code (MaSach = 101)
DECLARE @i INT = 1;
WHILE @i <= 3
BEGIN
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (101, N'Sẵn sàng');
    SET @i = @i + 1;
END;

-- B. Tạo 2 cuốn vật lý cho đầu sách Design Patterns (MaSach = 102)
SET @i = 1;
WHILE @i <= 2
BEGIN
    INSERT INTO CUONSACH (MaSach, TinhTrang) VALUES (102, N'Sẵn sàng');
    SET @i = @i + 1;
END;
GO

-- 7. PHIẾU MƯỢN & CHI TIẾT MƯỢN TRẢ MẪU
-- Độc giả Nguyễn Văn A mượn 2 cuốn sách (1 cuốn đã trả, 1 cuốn đang mượn)
SET IDENTITY_INSERT PHIEUMUON ON;
INSERT INTO PHIEUMUON (MaPhieuMuon, MaDocGia, NgayMuon) VALUES (1, 1, '2026-05-01');
SET IDENTITY_INSERT PHIEUMUON OFF;
GO

-- Giả sử MaCuonSach 1 và 2 thuộc đầu sách 101
-- Cuốn 1: Đã trả đúng hạn
INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat, TinhTrangCuonSachKhiTra)
VALUES (1, 1, '2026-05-15', '2026-05-10', 0, N'Sẵn sàng');

-- Cuốn 2: Đang mượn (chưa trả)
INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat, TinhTrangCuonSachKhiTra)
VALUES (1, 2, '2026-05-15', NULL, 0, NULL);
GO

PRINT N'---> Khởi tạo toàn bộ dữ liệu mẫu thành công!';
