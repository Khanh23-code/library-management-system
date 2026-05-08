USE QL_ThuVien;
GO

-- Thêm cột HinhAnh vào bảng SACH để lưu tên file ảnh bìa
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('SACH') AND name = 'HinhAnh'
)
BEGIN
    ALTER TABLE SACH ADD HinhAnh NVARCHAR(255) NULL;
END
GO

PRINT 'Da them cot HinhAnh vao bang SACH thanh cong.';
