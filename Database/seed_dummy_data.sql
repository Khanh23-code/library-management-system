USE [QL_ThuVien];
GO

-- =====================================================================
-- SCRIPT SINH DỮ LIỆU MẪU (DUMMY DATA) CHO CỤM ADMIN
-- Hỗ trợ test hiển thị trực quan các màn hình: Quản lý Sách, Độc giả, Mượn Trả
-- =====================================================================

-- 1. BẢNG THỂ LOẠI SÁCH
SET IDENTITY_INSERT [THELOAISACH] ON;
INSERT INTO [THELOAISACH] (MaTheLoai, TenTheLoai) VALUES
(1, N'Khoa học Công nghệ'),
(2, N'Văn học Nghệ thuật'),
(3, N'Kinh tế & Kinh doanh'),
(4, N'Tâm lý & Kỹ năng sống');
SET IDENTITY_INSERT [THELOAISACH] OFF;
GO

-- 2. BẢNG LOẠI ĐỘC GIẢ
SET IDENTITY_INSERT [LOAIDOCGIA] ON;
INSERT INTO [LOAIDOCGIA] (MaLoaiDocGia, TenLoaiDocGia) VALUES
(1, N'Sinh viên'),
(2, N'Giảng viên'),
(3, N'Nghiên cứu sinh');
SET IDENTITY_INSERT [LOAIDOCGIA] OFF;
GO

-- 3. BẢNG TÀI KHOẢN (Cho các độc giả mẫu)
INSERT INTO [TAIKHOAN] (TenDangNhap, MatKhau, Quyen, TrangThai) VALUES
(N'RD_NGUYENVANA', N'123456', N'Reader', N'Active'),
(N'RD_TRANTHIB', N'123456', N'Reader', N'Active'),
(N'RD_LEVANHUNG', N'123456', N'Reader', N'Pending');
GO

-- 4. BẢNG ĐỘC GIẢ
SET IDENTITY_INSERT [DOCGIA] ON;
INSERT INTO [DOCGIA] (MaDocGia, HoTen, MaLoaiDocGia, NgaySinh, DiaChi, Email, NgayLapThe, TongNo, TenDangNhap) VALUES
(1, N'Nguyễn Văn A', 1, '2003-05-15', N'Quận 1, TP.HCM', N'nva@gmail.com', '2025-01-10', 0, N'RD_NGUYENVANA'),
(2, N'Trần Thị B', 2, '1995-08-22', N'Quận Bình Thạnh, TP.HCM', N'ttb@gmail.com', '2024-11-05', 0, N'RD_TRANTHIB'),
(3, N'Lê Văn Hùng', 1, '2004-02-18', N'Quận 7, TP.HCM', N'lvhung@gmail.com', '2026-03-12', 0, N'RD_LEVANHUNG');
SET IDENTITY_INSERT [DOCGIA] OFF;
GO

-- 5. BẢNG SÁCH (Lưu ý: Không insert cột RowVersion vì SQL Server tự động tạo)
SET IDENTITY_INSERT [SACH] ON;
INSERT INTO [SACH] (MaSach, TenSach, MaTheLoai, TacGia, NamXuatBan, NhaXuatBan, NgayNhap, TriGia, TinhTrang, MoTa, SoLuong, HinhAnh) VALUES
(101, N'Clean Code: A Handbook of Agile Software Craftsmanship', 1, N'Robert C. Martin', 2008, N'Prentice Hall', '2024-01-15', 500000, N'Còn sách', N'Sách hướng dẫn viết mã sạch', 5, NULL),
(102, N'Design Patterns: Elements of Reusable Object-Oriented Software', 1, N'Erich Gamma', 1994, N'Addison-Wesley', '2024-02-20', 450000, N'Đang mượn', N'Sách kinh điển về mẫu thiết kế', 3, NULL),
(103, N'Nhà Giả Kim (The Alchemist)', 2, N'Paulo Coelho', 2020, N'NXB Nhã Nam', '2025-05-10', 120000, N'Còn sách', N'Tiểu thuyết truyền cảm hứng', 10, NULL),
(104, N'Đắc Nhân Tâm', 4, N'Dale Carnegie', 2019, N'NXB Tổng Hợp', '2023-08-14', 90000, N'Đang mượn', N'Nghệ thuật ứng xử đỉnh cao', 8, NULL),
(105, N'Nghệ Thuật Lãnh Đạo Đỉnh Cao', 3, N'John C. Maxwell', 2021, N'NXB Trẻ', '2025-11-01', 150000, N'Đang mượn', N'Phát triển tư duy lãnh đạo', 4, NULL);
SET IDENTITY_INSERT [SACH] OFF;
GO

-- 6. BẢNG PHIẾU MƯỢN
SET IDENTITY_INSERT [PHIEUMUON] ON;
INSERT INTO [PHIEUMUON] (MaPhieuMuon, MaDocGia, NgayMuon) VALUES
(1, 1, '2026-05-01'),
(2, 2, '2026-05-05');
SET IDENTITY_INSERT [PHIEUMUON] OFF;
GO

-- 7. BẢNG CHI TIẾT PHIẾU MƯỢN
INSERT INTO [CHITIETPHIEUMUON] (MaPhieuMuon, MaSach, TrangThai) VALUES
(1, 102, N'Đang mượn'),
(1, 104, N'Đang mượn'),
(2, 105, N'Đang mượn');
GO

PRINT N'---> Khởi tạo dữ liệu mẫu thành công!';
