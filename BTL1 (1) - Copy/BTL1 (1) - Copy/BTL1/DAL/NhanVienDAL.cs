using System;
using System.Data;
using System.Data.SqlClient;

namespace BTL1.DAL
{
    public class NhanVienDAL
    {
        private static NhanVienDAL instance;

        public static NhanVienDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new NhanVienDAL();
                return instance;
            }
        }

        private NhanVienDAL() { }

        public DataTable GetAllNhanVien()
        {
            string query = "SELECT * FROM NhanVien ORDER BY MaNV DESC";
            return DataProvider.Instance.ExecuteQuery(query);
        }

        public DataTable SearchNhanVien(string keyword)
        {
            string query = @"SELECT * FROM NhanVien 
                WHERE TenNV LIKE N'%' + @keyword + '%' 
                OR SoDienThoai LIKE '%' + @keyword + '%'
                OR CMND LIKE '%' + @keyword + '%'
                OR MaNV LIKE '%' + @keyword + '%'
                OR ChucVu LIKE N'%' + @keyword + '%'
                ORDER BY MaNV DESC";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@keyword", keyword)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        private string GenerateMaNV()
        {
            string query = "SELECT TOP 1 MaNV FROM NhanVien WHERE MaNV LIKE 'NV%' ORDER BY MaNV DESC";
            object result = DataProvider.Instance.ExecuteScalar(query);
            
            if (result == null || string.IsNullOrEmpty(result.ToString()))
            {
                return "NV001";
            }
            
            string lastMa = result.ToString();
            if (lastMa.StartsWith("NV") && lastMa.Length > 2)
            {
                int number = int.Parse(lastMa.Substring(2));
                return "NV" + (number + 1).ToString("D3");
            }
            
            return "NV001";
        }

        public bool InsertNhanVien(string tenNV, string cmnd, string sdt, string diaChi, DateTime ngaySinh, string gioiTinh, string chucVu, decimal luong)
        {
            string maNV = GenerateMaNV();
            
            string query = @"INSERT INTO NhanVien(MaNV, TenNV, CMND, SoDienThoai, DiaChi, NgaySinh, GioiTinh, ChucVu, Luong, MatKhau)
                VALUES (@maNV, @tenNV, @cmnd, @sdt, @diaChi, @ngaySinh, @gioiTinh, @chucVu, @luong, N'123456')";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maNV", maNV),
                new SqlParameter("@tenNV", tenNV),
                new SqlParameter("@cmnd", cmnd),
                new SqlParameter("@sdt", sdt),
                new SqlParameter("@diaChi", diaChi),
                new SqlParameter("@ngaySinh", ngaySinh),
                new SqlParameter("@gioiTinh", gioiTinh),
                new SqlParameter("@chucVu", chucVu),
                new SqlParameter("@luong", luong)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool UpdateNhanVien(string maNV, string tenNV, string cmnd, string sdt, string diaChi, DateTime ngaySinh, string gioiTinh, string chucVu, decimal luong)
        {
            string query = @"UPDATE NhanVien SET 
                TenNV = @tenNV, 
                CMND = @cmnd, 
                SoDienThoai = @sdt, 
                DiaChi = @diaChi, 
                NgaySinh = @ngaySinh, 
                GioiTinh = @gioiTinh, 
                ChucVu = @chucVu, 
                Luong = @luong 
                WHERE MaNV = @maNV";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tenNV", tenNV),
                new SqlParameter("@cmnd", cmnd),
                new SqlParameter("@sdt", sdt),
                new SqlParameter("@diaChi", diaChi),
                new SqlParameter("@ngaySinh", ngaySinh),
                new SqlParameter("@gioiTinh", gioiTinh),
                new SqlParameter("@chucVu", chucVu),
                new SqlParameter("@luong", luong),
                new SqlParameter("@maNV", maNV)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool DeleteNhanVien(string maNV)
        {
            string checkQuery = @"SELECT COUNT(*) FROM HoaDon WHERE MaNV = @maNV";
            
            SqlParameter[] checkParams = new SqlParameter[]
            {
                new SqlParameter("@maNV", maNV)
            };
            
            object count = DataProvider.Instance.ExecuteScalar(checkQuery, checkParams);
            
            if (Convert.ToInt32(count) > 0)
            {
                throw new Exception("Không thể xóa nhân viên đã lập hóa đơn!");
            }

            string query = "DELETE FROM NhanVien WHERE MaNV = @maNV";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maNV", maNV)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public DataTable CheckLogin(string username, string password)
        {
            string query = @"SELECT MaNV, TenNV, ChucVu, SoDienThoai 
                FROM NhanVien 
                WHERE MaNV = @username AND MatKhau = @password";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@username", username),
                new SqlParameter("@password", password)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public DataTable GetNhanVienByMa(string maNV)
        {
            string query = "SELECT * FROM NhanVien WHERE MaNV = @maNV";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maNV", maNV)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        // Thêm phương thức mới:
        public DataTable SearchNhanVienByMa(string maNV)
        {
            try
            {
                string query = @"
                    SELECT MaNV, TenNV, CMND, SoDienThoai, DiaChi, NgaySinh, GioiTinh, ChucVu, Luong
                    FROM NhanVien 
                    WHERE MaNV LIKE @MaNV
                    ORDER BY MaNV DESC";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaNV", "%" + maNV + "%")
                };
                
                return DataProvider.Instance.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm nhân viên: " + ex.Message);
            }
        }
    }
}