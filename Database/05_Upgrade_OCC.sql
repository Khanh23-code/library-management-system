-- =========================================================
-- DATABASE UPGRADE: OPTIMISTIC CONCURRENCY CONTROL (OCC)
-- =========================================================
USE QL_ThuVien;
GO

-- Thêm cột RowVersion (Timestamp) vào bảng SACH để xử lý Race Condition
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('SACH') AND name = 'RowVersion'
)
BEGIN
    ALTER TABLE SACH ADD RowVersion ROWVERSION;
END
GO

PRINT 'Da them cot RowVersion vao bang SACH thanh cong.';
