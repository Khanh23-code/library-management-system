using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THUVIENZ.BLL;
using THUVIENZ.DAL;
using THUVIENZ.DAL.Base;
using THUVIENZ.Models;
using Xunit;

namespace THUVIENZ.Tests
{
    public class AcceptanceTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<LmsDbContext> _options;

        public AcceptanceTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<LmsDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new LmsDbContext(_options);
            context.Database.EnsureCreated();
            SeedDummyData(context);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        private void SeedDummyData(LmsDbContext context)
        {
            // 1. Thể loại sách
            context.TheLoaiSachs.AddRange(
                new TheLoaiSach { TenTheLoai = "Khoa học Công nghệ" },
                new TheLoaiSach { TenTheLoai = "Văn học Nghệ thuật" },
                new TheLoaiSach { TenTheLoai = "Kinh tế & Quản trị" }
            );
            context.SaveChanges();

            // 2. Loại độc giả
            context.LoaiDocGias.AddRange(
                new LoaiDocGia { TenLoaiDocGia = "Sinh viên" },
                new LoaiDocGia { TenLoaiDocGia = "Giảng viên" }
            );
            context.SaveChanges();

            // 3. Tham số hệ thống
            context.ThamSos.AddRange(
                new ThamSo { TenThamSo = "SoSachMuonToiDa", GiaTri = 5 },
                new ThamSo { TenThamSo = "SoNgayMuonToiDa", GiaTri = 14 },
                new ThamSo { TenThamSo = "TienPhatMoiNgay", GiaTri = 2000 }
            );
            context.SaveChanges();

            // 4. Tài khoản
            context.TaiKhoans.AddRange(
                new TaiKhoan { TenDangNhap = "reader_vana", MatKhau = "123456", Quyen = "Reader", TrangThai = "Active" },
                new TaiKhoan { TenDangNhap = "reader_thib", MatKhau = "123456", Quyen = "Reader", TrangThai = "Active" }
            );
            context.SaveChanges();

            // 5. Độc giả
            context.DocGias.AddRange(
                new DocGia { TenDangNhap = "reader_vana", HoTen = "Nguyễn Văn A", MaLoaiDocGia = 1, GioiTinh = "Nam", SoDienThoai = "0901234567", Email = "vana@gmail.com", DiaChi = "Quận 1, TP.HCM", NgaySinh = new DateTime(2003, 5, 15) },
                new DocGia { TenDangNhap = "reader_thib", HoTen = "Trần Thị B", MaLoaiDocGia = 2, GioiTinh = "Nữ", SoDienThoai = "0919876543", Email = "thib@gmail.com", DiaChi = "Quận Bình Thạnh, TP.HCM", NgaySinh = new DateTime(1995, 8, 22) }
            );
            context.SaveChanges();

            // 6. Sách gốc
            var sach1 = new Sach { MaISBN = "ISBN-9780132350884", TenSach = "Clean Code: A Handbook of Agile Software Craftsmanship", MaTheLoai = 1, TacGia = "Robert C. Martin", NhaXuatBan = "Prentice Hall", NamXuatBan = 2008, TriGia = 500000, MoTa = "Sách hướng dẫn viết mã sạch" };
            var sach2 = new Sach { MaISBN = "ISBN-9780201633610", TenSach = "Design Patterns: Elements of Reusable Object-Oriented Software", MaTheLoai = 1, TacGia = "Erich Gamma", NhaXuatBan = "Addison-Wesley", NamXuatBan = 1994, TriGia = 450000, MoTa = "Sách kinh điển về mẫu thiết kế" };
            context.Sachs.AddRange(sach1, sach2);
            context.SaveChanges();

            // 7. Bản sao vật lý
            context.CuonSachs.AddRange(
                new CuonSach { MaSach = sach1.MaSach, TinhTrang = "Sẵn sàng", NgayNhap = DateTime.Now },
                new CuonSach { MaSach = sach1.MaSach, TinhTrang = "Đang mượn", NgayNhap = DateTime.Now },
                new CuonSach { MaSach = sach1.MaSach, TinhTrang = "Sẵn sàng", NgayNhap = DateTime.Now },
                new CuonSach { MaSach = sach2.MaSach, TinhTrang = "Sẵn sàng", NgayNhap = DateTime.Now },
                new CuonSach { MaSach = sach2.MaSach, TinhTrang = "Sẵn sàng", NgayNhap = DateTime.Now }
            );
            context.SaveChanges();

            // 8. Phiếu mượn và chi tiết
            var phieuMuon = new PhieuMuon { MaDocGia = 1, NgayMuon = new DateTime(2026, 5, 1) };
            context.PhieuMuons.Add(phieuMuon);
            context.SaveChanges();

            var cuonSachs = context.CuonSachs.ToList();
            context.ChiTietMuonTras.AddRange(
                new ChiTietMuonTra { MaPhieuMuon = phieuMuon.MaPhieuMuon, MaCuonSach = cuonSachs[0].MaCuonSach, HanTra = new DateTime(2026, 5, 15), NgayTraThucTe = new DateTime(2026, 5, 10), TienPhat = 0, TinhTrangCuonSachKhiTra = "Sẵn sàng" },
                new ChiTietMuonTra { MaPhieuMuon = phieuMuon.MaPhieuMuon, MaCuonSach = cuonSachs[1].MaCuonSach, HanTra = new DateTime(2026, 5, 15), NgayTraThucTe = null, TienPhat = 0 }
            );
            context.SaveChanges();
        }

        [Fact]
        public async Task TC_INV_01_AddBook_GeneratesPhysicalCopies()
        {
            using var context = new LmsDbContext(_options);
            var repo = new SachRepository(context);
            var sach = new Sach
            {
                TenSach = "C# Nâng Cao",
                MaTheLoai = 1,
                SoLuong = 5
            };

            await repo.AddAsync(sach);

            var savedSach = context.Sachs.FirstOrDefault(s => s.TenSach == "C# Nâng Cao");
            Assert.NotNull(savedSach);
            var copies = context.CuonSachs.Where(c => c.MaSach == savedSach.MaSach).ToList();
            Assert.Equal(5, copies.Count);
            Assert.All(copies, c => Assert.Equal("Sẵn sàng", c.TinhTrang));
        }

        [Fact]
        public async Task TC_INV_02_CascadeDelete_RemovesPhysicalCopies()
        {
            using var context = new LmsDbContext(_options);
            var service = new BookManagementService(new SachRepository(context));
            var book102 = context.Sachs.FirstOrDefault(s => s.TenSach.Contains("Design Patterns"));
            Assert.NotNull(book102);

            await service.DeleteBookAsync(book102.MaSach);

            var deletedBook = context.Sachs.FirstOrDefault(s => s.MaSach == book102.MaSach);
            Assert.Null(deletedBook);
            var copies = context.CuonSachs.Where(c => c.MaSach == book102.MaSach).ToList();
            Assert.Empty(copies);
        }

        [Fact]
        public async Task TC_INV_03_PreventDeleteBorrowedBook()
        {
            using var context = new LmsDbContext(_options);
            var service = new BookManagementService(new SachRepository(context));
            var book101 = context.Sachs.FirstOrDefault(s => s.TenSach.Contains("Clean Code"));
            Assert.NotNull(book101);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteBookAsync(book101.MaSach));
        }

        [Fact]
        public async Task TC_BOR_01_BorrowBookSuccess()
        {
            using var context = new LmsDbContext(_options);
            var settingsService = new LibrarySettingsService(new BaseRepository<ThamSo>(context));
            await settingsService.WarmUpCacheAsync();
            var service = new MuonTraService(context, settingsService);

            var docGia2 = context.DocGias.FirstOrDefault(d => d.HoTen == "Trần Thị B");
            Assert.NotNull(docGia2);
            
            var book101 = context.Sachs.FirstOrDefault(s => s.TenSach.Contains("Clean Code"));
            Assert.NotNull(book101);
            var copyReady = context.CuonSachs.FirstOrDefault(c => c.MaSach == book101.MaSach && c.TinhTrang == "Sẵn sàng");
            Assert.NotNull(copyReady);

            bool result = await service.ThucHienMuonSachAsync(docGia2.MaDocGia, new List<int> { copyReady.MaCuonSach });

            Assert.True(result);
            var updatedCopy = context.CuonSachs.FirstOrDefault(c => c.MaCuonSach == copyReady.MaCuonSach);
            Assert.Equal("Đang mượn", updatedCopy?.TinhTrang);
        }

        [Fact]
        public async Task TC_BOR_02_ExceedMaxLimit()
        {
            using var context = new LmsDbContext(_options);
            var settingsService = new LibrarySettingsService(new BaseRepository<ThamSo>(context));
            await settingsService.WarmUpCacheAsync();
            await settingsService.UpdateParamAsync("SoSachMuonToiDa", 3);

            var service = new MuonTraService(context, settingsService);
            var docGia1 = context.DocGias.FirstOrDefault(d => d.HoTen == "Nguyễn Văn A");
            Assert.NotNull(docGia1);

            var readyCopies = context.CuonSachs.Where(c => c.TinhTrang == "Sẵn sàng").Take(3).Select(c => c.MaCuonSach).ToList();
            Assert.Equal(3, readyCopies.Count);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.ThucHienMuonSachAsync(docGia1.MaDocGia, readyCopies));
        }

        [Fact]
        public async Task TC_BOR_03_BorrowNotReadyBook()
        {
            using var context = new LmsDbContext(_options);
            var settingsService = new LibrarySettingsService(new BaseRepository<ThamSo>(context));
            await settingsService.WarmUpCacheAsync();
            var service = new MuonTraService(context, settingsService);
            var docGia2 = context.DocGias.FirstOrDefault(d => d.HoTen == "Trần Thị B");
            Assert.NotNull(docGia2);

            var borrowedCopy = context.CuonSachs.FirstOrDefault(c => c.TinhTrang == "Đang mượn");
            Assert.NotNull(borrowedCopy);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.ThucHienMuonSachAsync(docGia2.MaDocGia, new List<int> { borrowedCopy.MaCuonSach }));
        }

        [Fact]
        public async Task TC_RET_01_ReturnOnTime()
        {
            using var context = new LmsDbContext(_options);
            var settingsService = new LibrarySettingsService(new BaseRepository<ThamSo>(context));
            await settingsService.WarmUpCacheAsync();
            var service = new MuonTraService(context, settingsService);

            var borrowedCopy = context.CuonSachs.FirstOrDefault(c => c.TinhTrang == "Đang mượn");
            Assert.NotNull(borrowedCopy);

            var chiTiet = context.ChiTietMuonTras.FirstOrDefault(c => c.MaCuonSach == borrowedCopy.MaCuonSach && c.NgayTraThucTe == null);
            Assert.NotNull(chiTiet);
            chiTiet.HanTra = DateTime.Now.AddDays(1);
            context.SaveChanges();

            var ketQua = await service.ThucHienTraSachAsync(borrowedCopy.MaCuonSach);

            Assert.True(ketQua.ThanhCong);
            Assert.Equal(0, ketQua.TienPhat);
            var updatedCopy = context.CuonSachs.FirstOrDefault(c => c.MaCuonSach == borrowedCopy.MaCuonSach);
            Assert.Equal("Sẵn sàng", updatedCopy?.TinhTrang);
        }

        [Fact]
        public async Task TC_RET_02_ReturnOverdue()
        {
            using var context = new LmsDbContext(_options);
            var settingsService = new LibrarySettingsService(new BaseRepository<ThamSo>(context));
            await settingsService.WarmUpCacheAsync();
            var service = new MuonTraService(context, settingsService);

            var borrowedCopy = context.CuonSachs.FirstOrDefault(c => c.TinhTrang == "Đang mượn");
            Assert.NotNull(borrowedCopy);

            var chiTiet = context.ChiTietMuonTras.FirstOrDefault(c => c.MaCuonSach == borrowedCopy.MaCuonSach && c.NgayTraThucTe == null);
            Assert.NotNull(chiTiet);
            chiTiet.HanTra = DateTime.Now.AddDays(-5);
            context.SaveChanges();

            var ketQua = await service.ThucHienTraSachAsync(borrowedCopy.MaCuonSach);

            Assert.True(ketQua.ThanhCong);
            Assert.True(ketQua.TienPhat > 0);
            Assert.Equal(10000, ketQua.TienPhat);
            
            var docGia = context.DocGias.FirstOrDefault(d => d.MaDocGia == 1);
            Assert.Equal(10000, docGia?.TongNo);
        }

        [Fact]
        public async Task TC_RET_03_ReturnNotBorrowed()
        {
            using var context = new LmsDbContext(_options);
            var settingsService = new LibrarySettingsService(new BaseRepository<ThamSo>(context));
            await settingsService.WarmUpCacheAsync();
            var service = new MuonTraService(context, settingsService);

            var readyCopy = context.CuonSachs.FirstOrDefault(c => c.TinhTrang == "Sẵn sàng");
            Assert.NotNull(readyCopy);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.ThucHienTraSachAsync(readyCopy.MaCuonSach));
        }
    }
}
