using System;
using System.Data;
using System.Data.SqlClient;

namespace BTL1.DAL
{
    public class DatPhongDAL
    {
        private static DatPhongDAL instance;

        public static DatPhongDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new DatPhongDAL();
                return instance;
            }
        }

        private DatPhongDAL() { }

        public DataTable GetAllDatPhong()
        {
            string query = @"SELECT dp.*, 
                kh.TenKH, kh.SoDienThoai,
                p.TenPhong, p.LoaiPhong, p.GiaPhong,
                nv.TenNV
                FROM DatPhong dp
                INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
                INNER JOIN Phong p ON dp.MaPhong = p.MaPhong
                INNER JOIN NhanVien nv ON dp.MaNV = nv.MaNV
                ORDER BY dp.NgayDat DESC";
            
            return DataProvider.Instance.ExecuteQuery(query);
        }

        public DataTable GetDatPhongByMa(string maDatPhong)
        {
            string query = @"SELECT dp.*, 
                kh.TenKH, kh.SoDienThoai, kh.CMND,
                p.TenPhong, p.LoaiPhong, p.GiaPhong,
                nv.TenNV
                FROM DatPhong dp
                INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
                INNER JOIN Phong p ON dp.MaPhong = p.MaPhong
                INNER JOIN NhanVien nv ON dp.MaNV = nv.MaNV
                WHERE dp.MaDatPhong = @maDatPhong";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maDatPhong", maDatPhong)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public DataTable SearchDatPhong(string keyword)
        {
            string query = @"SELECT dp.*, 
                kh.TenKH, kh.SoDienThoai,
                p.TenPhong, p.LoaiPhong,
                nv.TenNV
                FROM DatPhong dp
                INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
                INNER JOIN Phong p ON dp.MaPhong = p.MaPhong
                INNER JOIN NhanVien nv ON dp.MaNV = nv.MaNV
                WHERE dp.MaDatPhong LIKE '%' + @keyword + '%'
                OR kh.TenKH LIKE N'%' + @keyword + '%'
                OR kh.SoDienThoai LIKE '%' + @keyword + '%'
                OR p.TenPhong LIKE N'%' + @keyword + '%'
                ORDER BY dp.NgayDat DESC";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@keyword", keyword)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public string GenerateMaDatPhong()
        {
            string query = "EXEC sp_TaoMaDatPhong";
            object result = DataProvider.Instance.ExecuteScalar(query);
            return result?.ToString() ?? $"DP_{DateTime.Now:ddMMyyyy}001";
        }

        public bool KiemTraPhongTrong(string maPhong, DateTime ngayNhanPhong, DateTime ngayTraPhong)
        {
            string query = "EXEC sp_KiemTraPhongTrong @maPhong, @ngayNhanPhong, @ngayTraPhong";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maPhong", maPhong),
                new SqlParameter("@ngayNhanPhong", ngayNhanPhong),
                new SqlParameter("@ngayTraPhong", ngayTraPhong)
            };
            
            object result = DataProvider.Instance.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) == 0;
        }

        public bool InsertDatPhong(string maDatPhong, string maKH, string maPhong, string maNV,
            DateTime ngayDat, DateTime ngayNhanPhong, DateTime ngayTraPhong, int soNguoi, 
            decimal tienCoc, string ghiChu)
        {
            string query = @"INSERT INTO DatPhong(MaDatPhong, MaKH, MaPhong, MaNV, NgayDat, 
                NgayNhanPhong, NgayTraPhong, SoNguoi, TienCoc, TrangThai, GhiChu)
                VALUES (@maDatPhong, @maKH, @maPhong, @maNV, @ngayDat, @ngayNhanPhong, 
                @ngayTraPhong, @soNguoi, @tienCoc, N'Đã đặt', @ghiChu)";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maDatPhong", maDatPhong),
                new SqlParameter("@maKH", maKH),
                new SqlParameter("@maPhong", maPhong),
                new SqlParameter("@maNV", maNV),
                new SqlParameter("@ngayDat", ngayDat),
                new SqlParameter("@ngayNhanPhong", ngayNhanPhong),
                new SqlParameter("@ngayTraPhong", ngayTraPhong),
                new SqlParameter("@soNguoi", soNguoi),
                new SqlParameter("@tienCoc", tienCoc),
                new SqlParameter("@ghiChu", ghiChu ?? (object)DBNull.Value)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool UpdateDatPhong(string maDatPhong, DateTime ngayNhanPhong, DateTime ngayTraPhong, 
            int soNguoi, decimal tienCoc, string ghiChu)
        {
            string query = @"UPDATE DatPhong SET 
                NgayNhanPhong = @ngayNhanPhong,
                NgayTraPhong = @ngayTraPhong,
                SoNguoi = @soNguoi,
                TienCoc = @tienCoc,
                GhiChu = @ghiChu
                WHERE MaDatPhong = @maDatPhong";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ngayNhanPhong", ngayNhanPhong),
                new SqlParameter("@ngayTraPhong", ngayTraPhong),
                new SqlParameter("@soNguoi", soNguoi),
                new SqlParameter("@tienCoc", tienCoc),
                new SqlParameter("@ghiChu", ghiChu ?? (object)DBNull.Value),
                new SqlParameter("@maDatPhong", maDatPhong)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool UpdateTrangThaiDatPhong(string maDatPhong, string trangThai)
        {
            string query = "UPDATE DatPhong SET TrangThai = @trangThai WHERE MaDatPhong = @maDatPhong";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@trangThai", trangThai),
                new SqlParameter("@maDatPhong", maDatPhong)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool DeleteDatPhong(string maDatPhong)
        {
            string query = "DELETE FROM DatPhong WHERE MaDatPhong = @maDatPhong";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maDatPhong", maDatPhong)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public DataTable GetDatPhongByTrangThai(string trangThai)
        {
            if (trangThai == "Tất cả")
                return GetAllDatPhong();

            string query = @"SELECT dp.*, 
                kh.TenKH, kh.SoDienThoai,
                p.TenPhong, p.LoaiPhong, p.GiaPhong,
                nv.TenNV
                FROM DatPhong dp
                INNER JOIN KhachHang kh ON dp.MaKH = kh.MaKH
                INNER JOIN Phong p ON dp.MaPhong = p.MaPhong
                INNER JOIN NhanVien nv ON dp.MaNV = nv.MaNV
                WHERE dp.TrangThai = @trangThai
                ORDER BY dp.NgayDat DESC";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@trangThai", trangThai)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public bool UpdateDatPhongFull(string maDatPhong, string maKH, string maPhong, 
            DateTime ngayNhanPhong, DateTime ngayTraPhong, int soNguoi, decimal tienCoc, string ghiChu)
        {
            string query = @"UPDATE DatPhong SET 
                MaKH = @maKH,
                MaPhong = @maPhong,
                NgayNhanPhong = @ngayNhanPhong,
                NgayTraPhong = @ngayTraPhong,
                SoNguoi = @soNguoi,
                TienCoc = @tienCoc,
                GhiChu = @ghiChu
                WHERE MaDatPhong = @maDatPhong";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maKH", maKH),
                new SqlParameter("@maPhong", maPhong),
                new SqlParameter("@ngayNhanPhong", ngayNhanPhong),
                new SqlParameter("@ngayTraPhong", ngayTraPhong),
                new SqlParameter("@soNguoi", soNguoi),
                new SqlParameter("@tienCoc", tienCoc),
                new SqlParameter("@ghiChu", ghiChu ?? (object)DBNull.Value),
                new SqlParameter("@maDatPhong", maDatPhong)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public DataTable GetPhongTrong()
        {
            string query = @"SELECT p.* FROM Phong p 
                WHERE p.TrangThai = N'Trống' 
                AND p.MaPhong NOT IN (
                    SELECT MaPhong FROM DatPhong 
                    WHERE TrangThai IN (N'Đã đặt', N'Đang ở')
                    AND GETDATE() BETWEEN NgayNhanPhong AND NgayTraPhong
                )
                ORDER BY p.TenPhong";
            
            return DataProvider.Instance.ExecuteQuery(query);
        }

        // ✅ THÊM METHOD MỚI: Cập nhật trạng thái theo MaPhong và MaKH
        public bool UpdateTrangThaiByMaPhong(string maPhong, string maKH, string trangThai)
        {
            try
            {
                string query = @"UPDATE DatPhong 
                    SET TrangThai = @TrangThai 
                    WHERE MaPhong = @MaPhong AND MaKH = @MaKH AND TrangThai = N'Đã đặt'";
                
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@TrangThai", trangThai),
                    new SqlParameter("@MaPhong", maPhong),
                    new SqlParameter("@MaKH", maKH)
                };
                
                int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật trạng thái đặt phòng: " + ex.Message);
            }
        }
    }
}