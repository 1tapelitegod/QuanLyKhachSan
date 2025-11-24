using System;
using System.Data;
using System.Data.SqlClient;

namespace BTL1.DAL
{
    public class HoaDonDAL
    {
        private static HoaDonDAL instance;

        public static HoaDonDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new HoaDonDAL();
                return instance;
            }
        }

        private HoaDonDAL() { }

        public DataTable GetAllMaHoaDon()
        {
            string query = "SELECT MaHD FROM HoaDon ORDER BY NgayLap DESC";
            return DataProvider.Instance.ExecuteQuery(query);
        }

        public DataTable GetHoaDonByMa(string maHD)
        {
            string query = "SELECT * FROM HoaDon WHERE MaHD = @maHD";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maHD", maHD)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public DataTable GetChiTietHoaDon(string maHD)
        {
            string query = @"SELECT ct.*, 
                CASE 
                    WHEN p.MaPhong IS NOT NULL THEN p.TenPhong
                    WHEN dv.MaDV IS NOT NULL THEN dv.TenDV
                END AS TenHang,
                CASE 
                    WHEN p.MaPhong IS NOT NULL THEN 'PHÒNG'
                    WHEN dv.MaDV IS NOT NULL THEN 'DỊCH VỤ'
                END AS LoaiHang
                FROM ChiTietHoaDon ct
                LEFT JOIN Phong p ON ct.MaDV = p.MaPhong
                LEFT JOIN DichVu dv ON ct.MaDV = dv.MaDV
                WHERE ct.MaHD = @maHD";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maHD", maHD)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public string GenerateMaHoaDon()
        {
            string ngayHienTai = DateTime.Now.ToString("ddMMyyyy");
            
            // ✅ FIX: Tìm số lớn nhất trong ngày hiện tại
            string query = @"SELECT ISNULL(MAX(CAST(RIGHT(MaHD, 3) AS INT)), 0) AS MaxNumber
                FROM HoaDon 
                WHERE MaHD LIKE 'HDB_' + @ngayHienTai + '%'";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ngayHienTai", ngayHienTai)
            };
            
            object result = DataProvider.Instance.ExecuteScalar(query, parameters);
            
            int number = Convert.ToInt32(result);
            
            // Tăng số lên 1
            return $"HDB_{ngayHienTai}{(number + 1):D3}";
        }

        public bool InsertHoaDon(string maHD, string maKH, string maNV, DateTime ngayLap, decimal tongTien, string ghiChu)
        {
            string query = @"INSERT INTO HoaDon(MaHD, MaKH, MaNV, NgayLap, TongTien, TrangThai, GhiChu)
                VALUES (@maHD, @maKH, @maNV, @ngayLap, @tongTien, N'Đã thanh toán', @ghiChu)";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maHD", maHD),
                new SqlParameter("@maKH", maKH),
                new SqlParameter("@maNV", maNV),
                new SqlParameter("@ngayLap", ngayLap),
                new SqlParameter("@tongTien", tongTien),
                new SqlParameter("@ghiChu", ghiChu ?? (object)DBNull.Value)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool InsertChiTietHoaDon(string maHD, string maDV, int soLuong, decimal donGia, decimal giamGia, decimal thanhTien)
        {
            string query = @"INSERT INTO ChiTietHoaDon(MaHD, MaDV, SoLuong, DonGia, GiamGia, ThanhTien)
                VALUES (@maHD, @maDV, @soLuong, @donGia, @giamGia, @thanhTien)";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maHD", maHD),
                new SqlParameter("@maDV", maDV),
                new SqlParameter("@soLuong", soLuong),
                new SqlParameter("@donGia", donGia),
                new SqlParameter("@giamGia", giamGia),
                new SqlParameter("@thanhTien", thanhTien)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool DeleteHoaDon(string maHD)
        {
            string query = "DELETE FROM HoaDon WHERE MaHD = @maHD";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maHD", maHD)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool DeleteChiTietHoaDon(string maHD)
        {
            string query = "DELETE FROM ChiTietHoaDon WHERE MaHD = @maHD";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maHD", maHD)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool UpdateTrangThaiHoaDon(string maHD, string trangThai)
        {
            string query = "UPDATE HoaDon SET TrangThai = @trangThai WHERE MaHD = @maHD";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@trangThai", trangThai),
                new SqlParameter("@maHD", maHD)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public DataTable SearchHoaDon(string keyword)
        {
            string query = @"SELECT hd.*, kh.TenKH, nv.TenNV
                FROM HoaDon hd
                INNER JOIN KhachHang kh ON hd.MaKH = kh.MaKH
                INNER JOIN NhanVien nv ON hd.MaNV = nv.MaNV
                WHERE hd.MaHD LIKE '%' + @keyword + '%'
                OR kh.TenKH LIKE N'%' + @keyword + '%'
                OR kh.SoDienThoai LIKE '%' + @keyword + '%'
                ORDER BY hd.NgayLap DESC";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@keyword", keyword)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public bool CheckMaHDExists(string maHD)
        {
            string query = "SELECT COUNT(*) FROM HoaDon WHERE MaHD = @maHD";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maHD", maHD)
            };
            
            object result = DataProvider.Instance.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Lấy tất cả dịch vụ và phòng chưa thanh toán của khách hàng
        /// </summary>
        public DataTable GetDichVuChuaThanhToanByKH(string maKH)
        {
            try
            {
                string query = "EXEC sp_GetDichVuChuaThanhToanByKH @MaKH";
                
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaKH", maKH)
                };

                return DataProvider.Instance.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy dịch vụ chưa thanh toán: " + ex.Message);
            }
        }
    }
}