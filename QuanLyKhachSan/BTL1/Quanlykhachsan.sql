

-- Xóa database cũ nếu tồn tại
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'QuanLyKhachSan')
BEGIN
    ALTER DATABASE QuanLyKhachSan SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE QuanLyKhachSan
END
GO

-- Tạo database mới
CREATE DATABASE QuanLyKhachSan
GO

USE QuanLyKhachSan
GO

-- =============================================
-- 1. TẠO CÁC BẢNG
-- =============================================

-- Bảng Nhân Viên (User = MaNV, Pass = MatKhau)
CREATE TABLE NhanVien (
    MaNV NVARCHAR(20) PRIMARY KEY,  -- Dùng làm username
    TenNV NVARCHAR(100) NOT NULL,
    CMND NVARCHAR(20),
    SoDienThoai NVARCHAR(15),
    DiaChi NVARCHAR(200),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    ChucVu NVARCHAR(50),
    Luong DECIMAL(18,2),
    MatKhau NVARCHAR(50) NOT NULL   -- Password để đăng nhập
)
GO

-- Bảng Khách Hàng
CREATE TABLE KhachHang (
    MaKH NVARCHAR(20) PRIMARY KEY,
    TenKH NVARCHAR(100) NOT NULL,
    CMND NVARCHAR(20),
    SoDienThoai NVARCHAR(15) NOT NULL,
    DiaChi NVARCHAR(200),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    QuocTich NVARCHAR(50)
)
GO

-- Bảng Phòng
CREATE TABLE Phong (
    MaPhong NVARCHAR(20) PRIMARY KEY,
    TenPhong NVARCHAR(100) NOT NULL,
    LoaiPhong NVARCHAR(50),
    GiaPhong DECIMAL(18,2) NOT NULL,
    TrangThai NVARCHAR(50) DEFAULT N'Trống',
    MoTa NVARCHAR(500),
    SoNguoiToiDa INT
)
GO

-- Bảng Dịch Vụ (Hàng Hóa)
CREATE TABLE DichVu (
    MaDV NVARCHAR(20) PRIMARY KEY,
    TenDV NVARCHAR(100) NOT NULL,
    GiaDV DECIMAL(18,2) NOT NULL,
    DonVi NVARCHAR(20),
    MoTa NVARCHAR(500),
    SoLuongTon INT DEFAULT 100  -- Số lượng tồn kho
)
GO

-- Bảng Hóa Đơn
CREATE TABLE HoaDon (
    MaHD NVARCHAR(50) PRIMARY KEY,
    MaKH NVARCHAR(20) NOT NULL,
    MaNV NVARCHAR(20) NOT NULL,  -- Nhân viên lập hóa đơn (từ đăng nhập)
    NgayLap DATETIME DEFAULT GETDATE(),
    TongTien DECIMAL(18,2) DEFAULT 0,
    TrangThai NVARCHAR(50) DEFAULT N'Chưa thanh toán',
    GhiChu NVARCHAR(500),
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH),
    FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
)
GO

-- Bảng Chi Tiết Hóa Đơn
CREATE TABLE ChiTietHoaDon (
    MaHD NVARCHAR(50),
    MaDV NVARCHAR(20),
    SoLuong INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    GiamGia DECIMAL(5,2) DEFAULT 0,  -- % giảm giá
    ThanhTien DECIMAL(18,2),
    PRIMARY KEY (MaHD, MaDV),
    FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD) ON DELETE CASCADE,
    FOREIGN KEY (MaDV) REFERENCES DichVu(MaDV)
)
GO

-- =============================================
-- 2. THÊM DỮ LIỆU MẪU
-- =============================================

-- Nhân Viên (User = MaNV, Password = MatKhau)
INSERT INTO NhanVien (MaNV, TenNV, CMND, SoDienThoai, DiaChi, NgaySinh, GioiTinh, ChucVu, Luong, MatKhau)
VALUES 
(N'NV001', N'Nguyễn Văn An', N'001234567890', N'0901234567', N'123 Lê Lợi, Q1, TP.HCM', '1990-05-15', N'Nam', N'Quản lý', 15000000, N'123456'),
(N'NV002', N'Trần Thị Bình', N'001234567891', N'0901234568', N'456 Nguyễn Huệ, Q1, TP.HCM', '1992-08-20', N'Nữ', N'Lễ tân', 8000000, N'123456'),
(N'NV003', N'Lê Văn Cường', N'001234567892', N'0901234569', N'789 Trần Hưng Đạo, Q5, TP.HCM', '1988-03-10', N'Nam', N'Kế toán', 10000000, N'123456'),
(N'NV004', N'Phạm Thị Dung', N'001234567893', N'0901234570', N'321 Hai Bà Trưng, Q3, TP.HCM', '1995-11-25', N'Nữ', N'Nhân viên phục vụ', 7000000, N'123456'),
(N'NV005', N'Hoàng Văn Em', N'001234567894', N'0901234571', N'654 Võ Văn Tần, Q3, TP.HCM', '1993-07-30', N'Nam', N'Bảo vệ', 6000000, N'123456'),
(N'admin', N'Administrator', N'000000000000', N'0900000000', N'Khách sạn ABC', '1985-01-01', N'Nam', N'Quản trị viên', 20000000, N'admin123')
GO

-- Khách Hàng
INSERT INTO KhachHang (MaKH, TenKH, CMND, SoDienThoai, DiaChi, NgaySinh, GioiTinh, QuocTich)
VALUES 
(N'KH001', N'Nguyễn Minh Anh', N'002345678901', N'0912345678', N'100 Nguyễn Thị Minh Khai, Q1, TP.HCM', '1985-04-12', N'Nam', N'Việt Nam'),
(N'KH002', N'Lê Thu Hương', N'002345678902', N'0912345679', N'200 Điện Biên Phủ, Q3, TP.HCM', '1990-09-18', N'Nữ', N'Việt Nam'),
(N'KH003', N'Trần Đức Huy', N'002345678903', N'0912345680', N'300 Cách Mạng Tháng 8, Q10, TP.HCM', '1988-12-25', N'Nam', N'Việt Nam'),
(N'KH004', N'Phạm Thị Lan', N'002345678904', N'0912345681', N'400 Lê Văn Sỹ, Q3, TP.HCM', '1992-06-08', N'Nữ', N'Việt Nam'),
(N'KH005', N'John Smith', N'PASS123456', N'0912345682', N'Hotel Guest', '1980-01-15', N'Nam', N'USA'),
(N'KH006', N'Kim Min-ji', N'PASS789012', N'0912345683', N'Tourist', '1995-03-22', N'Nữ', N'Hàn Quốc'),
(N'KH007', N'Nguyễn Văn Bình', N'002345678905', N'0912345684', N'500 Phan Xích Long, Phú Nhuận, TP.HCM', '1987-08-14', N'Nam', N'Việt Nam'),
(N'KH008', N'Trương Thị Cẩm', N'002345678906', N'0912345685', N'600 Hoàng Sa, Q3, TP.HCM', '1993-11-30', N'Nữ', N'Việt Nam')
GO

-- Phòng
INSERT INTO Phong (MaPhong, TenPhong, LoaiPhong, GiaPhong, TrangThai, MoTa, SoNguoiToiDa)
VALUES 
(N'P101', N'Phòng 101', N'Phòng Đơn', 500000, N'Trống', N'Phòng đơn tiêu chuẩn, giường đơn, view thành phố', 1),
(N'P102', N'Phòng 102', N'Phòng Đơn', 500000, N'Trống', N'Phòng đơn tiêu chuẩn, giường đơn, view thành phố', 1),
(N'P103', N'Phòng 103', N'Phòng Đôi', 800000, N'Trống', N'Phòng đôi tiêu chuẩn, giường đôi, view thành phố', 2),
(N'P104', N'Phòng 104', N'Phòng Đôi', 800000, N'Trống', N'Phòng đôi tiêu chuẩn, giường đôi, view thành phố', 2),
(N'P201', N'Phòng 201', N'Phòng VIP', 1500000, N'Trống', N'Phòng VIP, giường king size, ban công view biển', 2),
(N'P202', N'Phòng 202', N'Phòng VIP', 1500000, N'Trống', N'Phòng VIP, giường king size, ban công view biển', 2),
(N'P203', N'Phòng 203', N'Phòng Suite', 2500000, N'Trống', N'Phòng Suite cao cấp, phòng khách riêng, view panorama', 4),
(N'P204', N'Phòng 204', N'Phòng Suite', 2500000, N'Trống', N'Phòng Suite cao cấp, phòng khách riêng, view panorama', 4),
(N'P301', N'Phòng 301', N'Phòng Đơn', 500000, N'Trống', N'Phòng đơn tiêu chuẩn, giường đơn, view thành phố', 1),
(N'P302', N'Phòng 302', N'Phòng Đôi', 800000, N'Trống', N'Phòng đôi tiêu chuẩn, giường đôi, view thành phố', 2)
GO

-- Dịch Vụ (Hàng Hóa)
INSERT INTO DichVu (MaDV, TenDV, GiaDV, DonVi, MoTa, SoLuongTon)
VALUES 
-- Đồ uống
(N'DV001', N'Coca Cola', 15000, N'Chai', N'Nước ngọt có gas', 200),
(N'DV002', N'Pepsi', 15000, N'Chai', N'Nước ngọt có gas', 200),
(N'DV003', N'Sting', 12000, N'Chai', N'Nước tăng lực', 150),
(N'DV004', N'Redbull', 25000, N'Lon', N'Nước tăng lực cao cấp', 100),
(N'DV005', N'Nước suối Lavie', 8000, N'Chai', N'Nước suối tinh khiết 500ml', 500),
(N'DV006', N'Trà xanh 0 độ', 10000, N'Chai', N'Trà xanh không độ', 150),

-- Đồ ăn
(N'DV007', N'Snack Ostar', 20000, N'Gói', N'Snack khoai tây', 100),
(N'DV008', N'Kẹo Alpenliebe', 30000, N'Túi', N'Kẹo sữa', 80),
(N'DV009', N'Mì Hảo Hảo', 5000, N'Gói', N'Mì ăn liền', 200),
(N'DV010', N'Bánh Oreo', 25000, N'Gói', N'Bánh quy socola', 100),

-- Dịch vụ
(N'DV011', N'Ăn sáng Buffet', 100000, N'Suất', N'Buffet sáng tự chọn', 999),
(N'DV012', N'Giặt ủi', 50000, N'Kg', N'Dịch vụ giặt ủi quần áo', 999),
(N'DV013', N'Massage', 300000, N'Giờ', N'Dịch vụ massage thư giãn', 10),
(N'DV014', N'Karaoke', 200000, N'Giờ', N'Phòng karaoke VIP', 5),
(N'DV015', N'Đưa đón sân bay', 500000, N'Lượt', N'Dịch vụ đưa đón sân bay', 3)
GO

-- Hóa Đơn Mẫu
INSERT INTO HoaDon (MaHD, MaKH, MaNV, NgayLap, TongTien, TrangThai, GhiChu)
VALUES 
(N'HDB_18112024001', N'KH001', N'NV002', '2024-11-18 14:30:00', 235000, N'Đã thanh toán', N'Khách hàng mua đồ uống và snack'),
(N'HDB_18112024002', N'KH002', N'NV002', '2024-11-18 15:45:00', 180000, N'Đã thanh toán', N'Khách đặt ăn sáng'),
(N'HDB_17112024001', N'KH003', N'NV001', '2024-11-17 10:20:00', 540000, N'Đã thanh toán', N'Khách sử dụng dịch vụ massage'),
(N'HDB_16112024001', N'KH004', N'NV002', '2024-11-16 16:00:00', 125000, N'Đã thanh toán', N'Mua đồ ăn vặt'),
(N'HDB_15112024001', N'KH005', N'NV004', '2024-11-15 09:30:00', 650000, N'Đã thanh toán', N'Khách nước ngoài đặt dịch vụ')
GO

-- Chi Tiết Hóa Đơn
INSERT INTO ChiTietHoaDon (MaHD, MaDV, SoLuong, DonGia, GiamGia, ThanhTien)
VALUES 
-- HDB_18112024001
(N'HDB_18112024001', N'DV001', 4, 15000, 0, 60000),
(N'HDB_18112024001', N'DV003', 3, 12000, 0, 36000),
(N'HDB_18112024001', N'DV007', 5, 20000, 10, 90000),
(N'HDB_18112024001', N'DV010', 2, 25000, 2, 49000),

-- HDB_18112024002
(N'HDB_18112024002', N'DV011', 2, 100000, 10, 180000),

-- HDB_17112024001
(N'HDB_17112024001', N'DV013', 2, 300000, 10, 540000),

-- HDB_16112024001
(N'HDB_16112024001', N'DV002', 3, 15000, 0, 45000),
(N'HDB_16112024001', N'DV008', 2, 30000, 0, 60000),
(N'HDB_16112024001', N'DV009', 4, 5000, 0, 20000),

-- HDB_15112024001
(N'HDB_15112024001', N'DV015', 1, 500000, 0, 500000),
(N'HDB_15112024001', N'DV011', 1, 100000, 0, 100000),
(N'HDB_15112024001', N'DV004', 2, 25000, 0, 50000)
GO

-- =============================================
-- 3. STORED PROCEDURES
-- =============================================

-- Đăng nhập (User = MaNV, Pass = MatKhau)
CREATE PROCEDURE sp_DangNhap
    @MaNV NVARCHAR(20),
    @MatKhau NVARCHAR(50)
AS
BEGIN
    SELECT MaNV, TenNV, ChucVu, SoDienThoai
    FROM NhanVien
    WHERE MaNV = @MaNV AND MatKhau = @MatKhau
END
GO

-- Lấy khách hàng theo SĐT
CREATE PROCEDURE sp_GetKhachHangBySDT
    @SoDienThoai NVARCHAR(15)
AS
BEGIN
    SELECT * FROM KhachHang WHERE SoDienThoai = @SoDienThoai
END
GO

-- Tạo mã hóa đơn tự động (HDB_ddMMyyyy0xxx)
CREATE PROCEDURE sp_TaoMaHoaDon
    @NgayHienTai NVARCHAR(20),
    @MaHD NVARCHAR(50) OUTPUT
AS
BEGIN
    DECLARE @SoThuTu INT
    
    SELECT @SoThuTu = COUNT(*) + 1
    FROM HoaDon
    WHERE MaHD LIKE 'HDB_' + @NgayHienTai + '%'
    
    SET @MaHD = 'HDB_' + @NgayHienTai + '0' + RIGHT('00' + CAST(@SoThuTu AS NVARCHAR(3)), 3)
END
GO

-- Kiểm tra số lượng tồn
CREATE PROCEDURE sp_GetSoLuongTon
    @MaDV NVARCHAR(20)
AS
BEGIN
    SELECT SoLuongTon FROM DichVu WHERE MaDV = @MaDV
END
GO

-- Lưu hóa đơn (Thêm hóa đơn + Chi tiết + Trừ tồn kho)
CREATE PROCEDURE sp_LuuHoaDon
    @MaHD NVARCHAR(50),
    @MaKH NVARCHAR(20),
    @MaNV NVARCHAR(20),
    @NgayLap DATETIME,
    @TongTien DECIMAL(18,2),
    @TrangThai NVARCHAR(50),
    @GhiChu NVARCHAR(500)
AS
BEGIN
    BEGIN TRANSACTION
    
    BEGIN TRY
        -- Thêm hóa đơn
        INSERT INTO HoaDon (MaHD, MaKH, MaNV, NgayLap, TongTien, TrangThai, GhiChu)
        VALUES (@MaHD, @MaKH, @MaNV, @NgayLap, @TongTien, @TrangThai, @GhiChu)
        
        COMMIT TRANSACTION
        SELECT 1 AS Result
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0 AS Result
    END CATCH
END
GO

-- Thêm chi tiết hóa đơn và trừ tồn kho
CREATE PROCEDURE sp_ThemChiTietHoaDon
    @MaHD NVARCHAR(50),
    @MaDV NVARCHAR(20),
    @SoLuong INT,
    @DonGia DECIMAL(18,2),
    @GiamGia DECIMAL(5,2),
    @ThanhTien DECIMAL(18,2)
AS
BEGIN
    BEGIN TRANSACTION
    
    BEGIN TRY
        -- Kiểm tra tồn kho
        DECLARE @SoLuongTon INT
        SELECT @SoLuongTon = SoLuongTon FROM DichVu WHERE MaDV = @MaDV
        
        IF @SoLuongTon < @SoLuong
        BEGIN
            ROLLBACK TRANSACTION
            SELECT 0 AS Result, N'Không đủ số lượng' AS Message
            RETURN
        END
        
        -- Thêm chi tiết
        INSERT INTO ChiTietHoaDon (MaHD, MaDV, SoLuong, DonGia, GiamGia, ThanhTien)
        VALUES (@MaHD, @MaDV, @SoLuong, @DonGia, @GiamGia, @ThanhTien)
        
        -- Trừ tồn kho
        UPDATE DichVu SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaDV = @MaDV
        
        COMMIT TRANSACTION
        SELECT 1 AS Result, N'Thành công' AS Message
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0 AS Result, ERROR_MESSAGE() AS Message
    END CATCH
END
GO

-- Hủy hóa đơn và hoàn lại số lượng
CREATE PROCEDURE sp_HuyHoaDon
    @MaHD NVARCHAR(50)
AS
BEGIN
    BEGIN TRANSACTION
    
    BEGIN TRY
        -- Hoàn lại số lượng
        UPDATE DichVu
        SET SoLuongTon = SoLuongTon + ct.SoLuong
        FROM DichVu dv
        INNER JOIN ChiTietHoaDon ct ON dv.MaDV = ct.MaDV
        WHERE ct.MaHD = @MaHD
        
        -- Xóa hóa đơn (Chi tiết sẽ tự động xóa do ON DELETE CASCADE)
        DELETE FROM HoaDon WHERE MaHD = @MaHD
        
        COMMIT TRANSACTION
        SELECT 1 AS Result
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0 AS Result
    END CATCH
END
GO

-- Thống kê doanh thu theo ngày
CREATE PROCEDURE sp_ThongKeDoanhThuNgay
    @TuNgay DATE,
    @DenNgay DATE
AS
BEGIN
    SELECT 
        CONVERT(DATE, NgayLap) AS Ngay,
        COUNT(MaHD) AS SoHoaDon,
        SUM(TongTien) AS TongDoanhThu
    FROM HoaDon
    WHERE CONVERT(DATE, NgayLap) BETWEEN @TuNgay AND @DenNgay
        AND TrangThai = N'Đã thanh toán'
    GROUP BY CONVERT(DATE, NgayLap)
    ORDER BY Ngay DESC
END
GO

-- =============================================
-- 4. VIEWS
-- =============================================

-- View chi tiết hóa đơn đầy đủ
CREATE VIEW vw_ChiTietHoaDonDayDu
AS
SELECT 
    hd.MaHD,
    hd.NgayLap,
    kh.MaKH,
    kh.TenKH,
    kh.SoDienThoai,
    nv.MaNV,
    nv.TenNV,
    ct.MaDV,
    dv.TenDV,
    ct.SoLuong,
    ct.DonGia,
    ct.GiamGia,
    ct.ThanhTien,
    hd.TongTien,
    hd.TrangThai
FROM HoaDon hd
INNER JOIN KhachHang kh ON hd.MaKH = kh.MaKH
INNER JOIN NhanVien nv ON hd.MaNV = nv.MaNV
INNER JOIN ChiTietHoaDon ct ON hd.MaHD = ct.MaHD
INNER JOIN DichVu dv ON ct.MaDV = dv.MaDV
GO

-- =============================================
-- KIỂM TRA DỮ LIỆU
-- =============================================

DECLARE @SoNhanVien INT, @SoKhachHang INT, @SoPhong INT, @SoDichVu INT, @SoHoaDon INT, @TongDoanhThu DECIMAL(18,2)

SELECT @SoNhanVien = COUNT(*) FROM NhanVien
SELECT @SoKhachHang = COUNT(*) FROM KhachHang
SELECT @SoPhong = COUNT(*) FROM Phong
SELECT @SoDichVu = COUNT(*) FROM DichVu
SELECT @SoHoaDon = COUNT(*) FROM HoaDon
SELECT @TongDoanhThu = ISNULL(SUM(TongTien), 0) FROM HoaDon WHERE TrangThai = N'Đã thanh toán'

PRINT '========== THÔNG TIN HỆ THỐNG =========='
PRINT 'Số nhân viên: ' + CAST(@SoNhanVien AS NVARCHAR(10))
PRINT 'Số khách hàng: ' + CAST(@SoKhachHang AS NVARCHAR(10))
PRINT 'Số phòng: ' + CAST(@SoPhong AS NVARCHAR(10))
PRINT 'Số dịch vụ: ' + CAST(@SoDichVu AS NVARCHAR(10))
PRINT 'Số hóa đơn: ' + CAST(@SoHoaDon AS NVARCHAR(10))
PRINT 'Tổng doanh thu: ' + FORMAT(@TongDoanhThu, 'N0') + ' VNĐ'
PRINT '======================================='
GO



/*
THÔNG TIN ĐĂNG NHẬP:
------------------------
User: NV001 | Pass: 123456 (Quản lý - Nguyễn Văn An)
User: NV002 | Pass: 123456 (Lễ tân - Trần Thị Bình)
User: NV003 | Pass: 123456 (Kế toán - Lê Văn Cường)
User: admin | Pass: admin123 (Administrator)

