-- =========================================================
-- DATABASE: QL_THU_VIEN (FINAL MASTER VERSION)
-- BỘ KHỞI TẠO CẤU TRÚC HOÀN CHỈNH (SCHEMA + LOGIC + INDEXES)
-- =========================================================
CREATE DATABASE QL_ThuVien;
GO
USE QL_ThuVien;
GO

-- 1. TAIKHOAN: Tăng độ dài MatKhau để lưu Hash kèm trạng thái kích hoạt
CREATE TABLE TAIKHOAN (
    TenDangNhap VARCHAR(50) PRIMARY KEY,
    MatKhau VARCHAR(255) NOT NULL, 
    Quyen NVARCHAR(20) NOT NULL, -- 'Admin' hoặc 'Reader'
    TrangThai NVARCHAR(20) DEFAULT 'Active'
);

-- 2. THAMSO: Lưu các quy định của thư viện
CREATE TABLE THAMSO (
    TenThamSo VARCHAR(50) PRIMARY KEY,
    GiaTri FLOAT NOT NULL
);

-- 3. LOAIDOCGIA
CREATE TABLE LOAIDOCGIA (
    MaLoaiDocGia INT PRIMARY KEY IDENTITY(1,1),
    TenLoaiDocGia NVARCHAR(50) NOT NULL
);

-- 4. DOCGIA: Ràng buộc UNIQUE cho tài khoản đảm bảo 1-1
CREATE TABLE DOCGIA (
    MaDocGia INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    MaLoaiDocGia INT,
    NgaySinh DATE,
    DiaChi NVARCHAR(200),
    Email NVARCHAR(100),
    NgayLapThe DATE,
    TongNo MONEY DEFAULT 0,
    TenDangNhap VARCHAR(50) UNIQUE,
    FOREIGN KEY (MaLoaiDocGia) REFERENCES LOAIDOCGIA(MaLoaiDocGia),
    FOREIGN KEY (TenDangNhap) REFERENCES TAIKHOAN(TenDangNhap)
);

-- 5. THELOAISACH
CREATE TABLE THELOAISACH (
    MaTheLoai INT PRIMARY KEY IDENTITY(1,1),
    TenTheLoai NVARCHAR(50) NOT NULL
);

-- 6. SACH: Đồng bộ toàn bộ 12 trường UI/Model và giải pháp xử lý tranh chấp (OCC)
CREATE TABLE SACH (
    MaSach INT PRIMARY KEY IDENTITY(1,1),
    TenSach NVARCHAR(100) NOT NULL,
    MaTheLoai INT,
    TacGia NVARCHAR(100),
    NamXuatBan INT,
    NhaXuatBan NVARCHAR(100),
    NgayNhap DATE,
    TriGia MONEY,
    TinhTrang NVARCHAR(50) DEFAULT N'Sẵn sàng', 
    MoTa NVARCHAR(500),
    SoLuong INT DEFAULT 1,
    HinhAnh NVARCHAR(255),
    RowVersion ROWVERSION,
    FOREIGN KEY (MaTheLoai) REFERENCES THELOAISACH(MaTheLoai)
);

-- 7. PHIEUMUON
CREATE TABLE PHIEUMUON (
    MaPhieuMuon INT PRIMARY KEY IDENTITY(1,1),
    MaDocGia INT,
    NgayMuon DATE NOT NULL,
    FOREIGN KEY (MaDocGia) REFERENCES DOCGIA(MaDocGia)
);

-- 8. CHITIETPHIEUMUON: Theo dõi trạng thái từng cuốn sách
CREATE TABLE CHITIETPHIEUMUON (
    MaPhieuMuon INT,
    MaSach INT,
    TrangThai NVARCHAR(50) DEFAULT N'Đang mượn', -- 'Đã trả', 'Mất'
    PRIMARY KEY (MaPhieuMuon, MaSach),
    FOREIGN KEY (MaPhieuMuon) REFERENCES PHIEUMUON(MaPhieuMuon),
    FOREIGN KEY (MaSach) REFERENCES SACH(MaSach)
);

-- 9. PHIEUTRA
CREATE TABLE PHIEUTRA (
    MaPhieuTra INT PRIMARY KEY IDENTITY(1,1),
    MaDocGia INT,
    NgayTra DATE NOT NULL,
    TienPhatKyNay MONEY DEFAULT 0,
    FOREIGN KEY (MaDocGia) REFERENCES DOCGIA(MaDocGia)
);

-- 10. CHITIETPHIEUTRA: Liên kết trực tiếp tới Phiếu Mượn để tính ngày trễ
CREATE TABLE CHITIETPHIEUTRA (
    MaPhieuTra INT,
    MaPhieuMuon INT,
    MaSach INT,
    SoNgayMuon INT,
    TienPhat MONEY DEFAULT 0,
    PRIMARY KEY (MaPhieuTra, MaPhieuMuon, MaSach),
    FOREIGN KEY (MaPhieuTra) REFERENCES PHIEUTRA(MaPhieuTra),
    FOREIGN KEY (MaPhieuMuon, MaSach) REFERENCES CHITIETPHIEUMUON(MaPhieuMuon, MaSach)
);

-- 11. PHIEUTHUTIENPHAT
CREATE TABLE PHIEUTHUTIENPHAT (
    MaPhieuThu INT PRIMARY KEY IDENTITY(1,1),
    MaDocGia INT,
    SoTienThu MONEY NOT NULL,
    ConLai MONEY,
    NgayThu DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaDocGia) REFERENCES DOCGIA(MaDocGia)
);
GO

-- =========================================================
-- PHẦN CÀI ĐẶT TÀI KHOẢN ADMIN MẶC ĐỊNH
-- =========================================================
INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, Quyen, TrangThai)
VALUES ('admin', 'admin', 'Admin', 'Active');
GO

-- =========================================================
-- PHẦN NÂNG CẤP LOGIC NÂNG CAO (AUTOMATION & REPORTING)
-- =========================================================

-- 1. Trigger tự động cập nhật trạng thái sách khi mượn (Automation)
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'trg_AutoUpdateBookStatus')
    DROP TRIGGER trg_AutoUpdateBookStatus;
GO

CREATE TRIGGER trg_AutoUpdateBookStatus
ON CHITIETPHIEUMUON
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE SACH
    SET TinhTrang = N'Đang mượn'
    FROM SACH
    INNER JOIN inserted i ON SACH.MaSach = i.MaSach;
END;
GO

-- 2. Stored Procedure xuất báo cáo trễ hạn (Reporting)
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetOverdueReport')
    DROP PROCEDURE sp_GetOverdueReport;
GO

CREATE PROCEDURE sp_GetOverdueReport
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @maxDays INT;
    DECLARE @fineRate MONEY;

    SELECT @maxDays = CAST(GiaTri AS INT) FROM THAMSO WHERE TenThamSo = 'SoNgayMuonToiDa';
    SELECT @fineRate = CAST(GiaTri AS MONEY) FROM THAMSO WHERE TenThamSo = 'TienPhatMoiNgay';

    SET @maxDays = ISNULL(@maxDays, 7);
    SET @fineRate = ISNULL(@fineRate, 1000);

    SELECT 
        DG.HoTen AS [TenDocGia],
        S.TenSach AS [TenSach],
        PM.NgayMuon AS [NgayMuon],
        DATEADD(day, @maxDays, PM.NgayMuon) AS [HanTra],
        DATEDIFF(day, DATEADD(day, @maxDays, PM.NgayMuon), GETDATE()) AS [SoNgayTre],
        (DATEDIFF(day, DATEADD(day, @maxDays, PM.NgayMuon), GETDATE()) * @fineRate) AS [TienPhatDuKien]
    FROM DOCGIA DG
    JOIN PHIEUMUON PM ON DG.MaDocGia = PM.MaDocGia
    JOIN CHITIETPHIEUMUON CT ON PM.MaPhieuMuon = CT.MaPhieuMuon
    JOIN SACH S ON CT.MaSach = S.MaSach
    WHERE CT.TrangThai = N'Đang mượn'
      AND DATEADD(day, @maxDays, PM.NgayMuon) < GETDATE();
END;
GO

-- 3. Tối ưu hóa hiệu năng tìm kiếm (Performance Indexing)
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'idx_Sach_TenSach')
    CREATE NONCLUSTERED INDEX idx_Sach_TenSach ON SACH (TenSach);

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'idx_DocGia_HoTen')
    CREATE NONCLUSTERED INDEX idx_DocGia_HoTen ON DOCGIA (HoTen);
GO

PRINT N'---> Hoàn tất khởi tạo toàn bộ CSDL QL_ThuVien thành công!';