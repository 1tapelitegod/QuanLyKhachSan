using System;
using System.Data;
using System.Data.SqlClient;

namespace BTL1.DAL
{
    public class KhachHangDAL
    {
        private static KhachHangDAL instance;

        public static KhachHangDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new KhachHangDAL();
                return instance;
            }
        }

        private KhachHangDAL() { }

        public DataTable GetAllKhachHang()
        {
            string query = "SELECT * FROM KhachHang ORDER BY MaKH DESC";
            return DataProvider.Instance.ExecuteQuery(query);
        }

        public DataTable SearchKhachHang(string keyword)
        {
            string query = @"SELECT * FROM KhachHang 
                WHERE TenKH LIKE N'%' + @keyword + '%' 
                OR SoDienThoai LIKE '%' + @keyword + '%'
                OR CMND LIKE '%' + @keyword + '%'
                OR MaKH LIKE '%' + @keyword + '%'
                ORDER BY MaKH DESC";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@keyword", keyword)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        private string GenerateMaKH()
        {
            string query = "SELECT TOP 1 MaKH FROM KhachHang ORDER BY MaKH DESC";
            object result = DataProvider.Instance.ExecuteScalar(query);
            
            if (result == null || string.IsNullOrEmpty(result.ToString()))
            {
                return "KH001";
            }
            
            string lastMa = result.ToString();
            int number = int.Parse(lastMa.Substring(2));
            return "KH" + (number + 1).ToString("D3");
        }

        public bool InsertKhachHang(string tenKH, string cmnd, string sdt, string diaChi, DateTime ngaySinh, string gioiTinh, string quocTich)
        {
            string maKH = GenerateMaKH();
            
            string query = @"INSERT INTO KhachHang(MaKH, TenKH, CMND, SoDienThoai, DiaChi, NgaySinh, GioiTinh, QuocTich)
                VALUES (@maKH, @tenKH, @cmnd, @sdt, @diaChi, @ngaySinh, @gioiTinh, @quocTich)";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maKH", maKH),
                new SqlParameter("@tenKH", tenKH),
                new SqlParameter("@cmnd", cmnd),
                new SqlParameter("@sdt", sdt),
                new SqlParameter("@diaChi", diaChi),
                new SqlParameter("@ngaySinh", ngaySinh),
                new SqlParameter("@gioiTinh", gioiTinh),
                new SqlParameter("@quocTich", quocTich)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool UpdateKhachHang(string maKH, string tenKH, string cmnd, string sdt, string diaChi, DateTime ngaySinh, string gioiTinh, string quocTich)
        {
            string query = @"UPDATE KhachHang SET 
                TenKH = @tenKH, 
                CMND = @cmnd, 
                SoDienThoai = @sdt, 
                DiaChi = @diaChi, 
                NgaySinh = @ngaySinh, 
                GioiTinh = @gioiTinh, 
                QuocTich = @quocTich 
                WHERE MaKH = @maKH";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tenKH", tenKH),
                new SqlParameter("@cmnd", cmnd),
                new SqlParameter("@sdt", sdt),
                new SqlParameter("@diaChi", diaChi),
                new SqlParameter("@ngaySinh", ngaySinh),
                new SqlParameter("@gioiTinh", gioiTinh),
                new SqlParameter("@quocTich", quocTich),
                new SqlParameter("@maKH", maKH)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool DeleteKhachHang(string maKH)
        {
            string checkQuery = @"SELECT COUNT(*) FROM HoaDon WHERE MaKH = @maKH";
            
            SqlParameter[] checkParams = new SqlParameter[]
            {
                new SqlParameter("@maKH", maKH)
            };
            
            object count = DataProvider.Instance.ExecuteScalar(checkQuery, checkParams);
            
            if (Convert.ToInt32(count) > 0)
            {
                throw new Exception("Không thể xóa khách hàng đã có hóa đơn!");
            }

            string query = "DELETE FROM KhachHang WHERE MaKH = @maKH";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maKH", maKH)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool CheckKhachHangExists(string sdt)
        {
            string query = "SELECT COUNT(*) FROM KhachHang WHERE SoDienThoai = @sdt";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@sdt", sdt)
            };
            
            object result = DataProvider.Instance.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        public DataTable GetKhachHangByMa(string maKH)
        {
            string query = "SELECT * FROM KhachHang WHERE MaKH = @maKH";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maKH", maKH)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public DataTable GetKhachHangBySDT(string sdt)
        {
            string query = "SELECT * FROM KhachHang WHERE SoDienThoai = @sdt";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@sdt", sdt)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        // Thêm phương thức mới:
        public DataTable SearchKhachHangByMa(string maKH)
        {
            try
            {
                string query = @"
                    SELECT MaKH, TenKH, CMND, SoDienThoai, DiaChi, NgaySinh, GioiTinh, QuocTich
                    FROM KhachHang 
                    WHERE MaKH LIKE @MaKH
                    ORDER BY MaKH DESC";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaKH", "%" + maKH + "%")
                };
                
                return DataProvider.Instance.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm khách hàng: " + ex.Message);
            }
        }
    }
}