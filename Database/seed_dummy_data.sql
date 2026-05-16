-- ======================================================================
-- SCRIPT SINH DỮ LIỆU MẪU (DUMMY DATA) SIÊU CẤP CHO HỆ THỐNG
-- Cung cấp hàng trăm bản ghi để biểu đồ thống kê trông chuyên nghiệp hơn.
-- ======================================================================
USE QL_ThuVien;
GO

-- XÓA DỮ LIỆU CŨ ĐỂ SEED MỚI (CHỈ DÙNG TRONG MÔI TRƯỜNG DEV)
DELETE FROM CHITIETMUONTRA;
DELETE FROM PHIEUMUON;
DELETE FROM CUONSACH;
DELETE FROM SACH;
DELETE FROM DOCGIA;
DELETE FROM TAIKHOAN WHERE TenDangNhap <> 'admin'; -- Giữ lại admin
DELETE FROM THELOAISACH;
DELETE FROM LOAIDOCGIA;
GO

-- RESET IDENTITY SEEDS
DBCC CHECKIDENT ('LOAIDOCGIA', RESEED, 0);
DBCC CHECKIDENT ('DOCGIA', RESEED, 0);
DBCC CHECKIDENT ('THELOAISACH', RESEED, 0);
DBCC CHECKIDENT ('SACH', RESEED, 100); -- Start books from 101
DBCC CHECKIDENT ('CUONSACH', RESEED, 0);
DBCC CHECKIDENT ('PHIEUMUON', RESEED, 0);
GO

-- 1. THỂ LOẠI SÁCH
SET IDENTITY_INSERT THELOAISACH ON;
INSERT INTO THELOAISACH (MaTheLoai, TenTheLoai) VALUES 
(1, N'Khoa học Công nghệ'),
(2, N'Văn học Nghệ thuật'),
(3, N'Kinh tế & Quản trị'),
(4, N'Ngoại ngữ'),
(5, N'Kỹ năng sống'),
(6, N'Lịch sử & Địa lý'),
(7, N'Tâm lý học'),
(8, N'Triết học'),
(9, N'Y học & Sức khỏe'),
(10, N'Chính trị - Pháp luật');
SET IDENTITY_INSERT THELOAISACH OFF;
GO

-- 2. LOẠI ĐỘC GIẢ
SET IDENTITY_INSERT LOAIDOCGIA ON;
INSERT INTO LOAIDOCGIA (MaLoaiDocGia, TenLoaiDocGia) VALUES 
(1, N'Sinh viên'),
(2, N'Giảng viên'),
(3, N'Nghiên cứu sinh');
SET IDENTITY_INSERT LOAIDOCGIA OFF;
GO

-- 3. TÀI KHOẢN & ĐỘC GIẢ (Sinh tự động 30 độc giả)
SET IDENTITY_INSERT DOCGIA ON;
DECLARE @i INT = 1;
WHILE @i <= 30
BEGIN
    DECLARE @user NVARCHAR(50) = 'reader' + CAST(@i AS NVARCHAR);
    IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE TenDangNhap = @user)
        INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES (@user, '123456', 'Reader', 'Active');
    
    INSERT INTO DOCGIA (MaDocGia, TenDangNhap, HoTen, MaLoaiDocGia, GioiTinh, SoDienThoai, Email, DiaChi, NgaySinh, TongNo) 
    VALUES (@i, @user, N'Độc giả ' + CAST(@i AS NVARCHAR), (@i % 3) + 1, 
            CASE WHEN @i % 2 = 0 THEN N'Nam' ELSE N'Nữ' END, 
            '09' + CAST(10000000 + @i AS NVARCHAR), 
            @user + '@gmail.com', N'Địa chỉ ' + CAST(@i AS NVARCHAR), 
            DATEADD(year, -20 - (@i % 10), GETDATE()), 0);
    SET @i = @i + 1;
END;
SET IDENTITY_INSERT DOCGIA OFF;
GO

-- 4. ĐẦU SÁCH (Sinh tự động 50 đầu sách)
SET IDENTITY_INSERT SACH ON;
DECLARE @j INT = 101;
WHILE @j <= 150
BEGIN
    INSERT INTO SACH (MaSach, MaISBN, TenSach, MaTheLoai, TacGia, NhaXuatBan, NamXuatBan, TriGia, MoTa) 
    VALUES (@j, 'ISBN-' + CAST(9780000000000 + @j AS NVARCHAR), 
            N'Sách ' + CAST(@j AS NVARCHAR), 
            (@j % 10) + 1, 
            N'Tác giả ' + CAST(@j AS NVARCHAR), 
            N'NXB ' + CASE WHEN @j % 2 = 0 THEN N'Giáo dục' ELSE N'Trẻ' END, 
            2010 + (@j % 14), 100000 + (@j * 1000), 
            N'Mô tả cho cuốn sách ' + CAST(@j AS NVARCHAR));
    SET @j = @j + 1;
END;
SET IDENTITY_INSERT SACH OFF;
GO

-- 4B. CUỐN SÁCH (Bản sao vật lý)
SET IDENTITY_INSERT CUONSACH ON;
DECLARE @cs_id INT = 1;
DECLARE @book_id INT = 101;
WHILE @book_id <= 150
BEGIN
    -- Mỗi đầu sách tạo 2-3 cuốn vật lý
    INSERT INTO CUONSACH (MaCuonSach, MaSach, TinhTrang) VALUES (@cs_id, @book_id, N'Sẵn sàng'); SET @cs_id = @cs_id + 1;
    INSERT INTO CUONSACH (MaCuonSach, MaSach, TinhTrang) VALUES (@cs_id, @book_id, N'Sẵn sàng'); SET @cs_id = @cs_id + 1;
    IF @book_id % 3 = 0 
    BEGIN 
        INSERT INTO CUONSACH (MaCuonSach, MaSach, TinhTrang) VALUES (@cs_id, @book_id, N'Sẵn sàng'); 
        SET @cs_id = @cs_id + 1; 
    END;
    SET @book_id = @book_id + 1;
END;
SET IDENTITY_INSERT CUONSACH OFF;
GO

-- 5. GIAO DỊCH MƯỢN TRẢ (Sinh tự động 150 giao dịch trong 30 ngày qua)
DECLARE @k INT = 1;
WHILE @k <= 150
BEGIN
    DECLARE @MaDG INT = (@k % 30) + 1;
    DECLARE @NgayMuon DATETIME = DATEADD(day, -(@k % 30), GETDATE());
    
    INSERT INTO PHIEUMUON (MaDocGia, NgayMuon) VALUES (@MaDG, @NgayMuon);
    DECLARE @MaPhieu INT = SCOPE_IDENTITY();
    
    -- Chọn sách ngẫu nhiên (MaSach từ 101-150)
    DECLARE @MaSach1 INT = 101 + (@k % 50);
    DECLARE @MaCuon1 INT = (SELECT TOP 1 MaCuonSach FROM CUONSACH WHERE MaSach = @MaSach1);
    
    -- Pattern cho Apriori: Độc giả mượn cặp sách 101 & 102 thường xuyên
    IF @k % 5 = 0
    BEGIN
        SET @MaCuon1 = (SELECT TOP 1 MaCuonSach FROM CUONSACH WHERE MaSach = 101);
        DECLARE @MaCuon2 INT = (SELECT TOP 1 MaCuonSach FROM CUONSACH WHERE MaSach = 102);
        
        IF @MaCuon1 IS NOT NULL
            INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat) 
            VALUES (@MaPhieu, @MaCuon1, DATEADD(day, 14, @NgayMuon), CASE WHEN @k % 2 = 0 THEN DATEADD(day, 7, @NgayMuon) ELSE NULL END, 0);
        
        IF @MaCuon2 IS NOT NULL
            INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat) 
            VALUES (@MaPhieu, @MaCuon2, DATEADD(day, 14, @NgayMuon), CASE WHEN @k % 2 = 0 THEN DATEADD(day, 7, @NgayMuon) ELSE NULL END, 0);
    END
    ELSE
    BEGIN
        -- Mượn 1 cuốn ngẫu nhiên
        IF @MaCuon1 IS NOT NULL
            INSERT INTO CHITIETMUONTRA (MaPhieuMuon, MaCuonSach, HanTra, NgayTraThucTe, TienPhat) 
            VALUES (@MaPhieu, @MaCuon1, DATEADD(day, 14, @NgayMuon), 
                   CASE WHEN @k % 3 != 0 THEN DATEADD(day, 10, @NgayMuon) ELSE NULL END, -- 1/3 là chưa trả
                   0);
    END
    
    SET @k = @k + 1;
END;
GO

-- 6. TẠO DỮ LIỆU QUÁ HẠN ĐỂ BIỂU ĐỒ TRÔNG "REAL"
-- Cập nhật một số phiếu mượn cũ thành quá hạn (HanTra < hiện tại và chưa trả)
UPDATE CHITIETMUONTRA 
SET HanTra = DATEADD(day, -5, GETDATE())
WHERE MaPhieuMuon IN (SELECT TOP 20 MaPhieuMuon FROM PHIEUMUON ORDER BY NEWID())
AND NgayTraThucTe IS NULL;

-- Cập nhật trạng thái sách vật lý
UPDATE CS SET TinhTrang = N'Đang mượn' FROM CUONSACH CS 
WHERE MaCuonSach IN (SELECT MaCuonSach FROM CHITIETMUONTRA WHERE NgayTraThucTe IS NULL);
UPDATE CS SET TinhTrang = N'Sẵn sàng' FROM CUONSACH CS 
WHERE MaCuonSach NOT IN (SELECT MaCuonSach FROM CHITIETMUONTRA WHERE NgayTraThucTe IS NULL);
GO

PRINT N'---> Đã khởi tạo 150 giao dịch, 50 đầu sách và 30 độc giả. Sẵn sàng cho báo cáo siêu cấp!';
