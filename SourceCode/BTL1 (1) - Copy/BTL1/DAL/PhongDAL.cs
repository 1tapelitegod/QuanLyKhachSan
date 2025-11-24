using System;
using System.Data;
using System.Data.SqlClient;

namespace BTL1.DAL
{
    public class PhongDAL
    {
        private static PhongDAL instance;

        public static PhongDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new PhongDAL();
                return instance;
            }
        }

        private PhongDAL() { }

        public DataTable GetAllPhong()
        {
            string query = "SELECT * FROM Phong ORDER BY MaPhong";
            return DataProvider.Instance.ExecuteQuery(query);
        }

        public DataTable SearchPhong(string keyword)
        {
            string query = @"SELECT * FROM Phong 
                WHERE MaPhong LIKE '%' + @keyword + '%' 
                OR TenPhong LIKE N'%' + @keyword + '%'
                OR LoaiPhong LIKE N'%' + @keyword + '%'
                ORDER BY MaPhong";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@keyword", keyword)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public DataTable GetPhongByTrangThai(string trangThai)
        {
            if (trangThai == "Tất cả")
            {
                return GetAllPhong();
            }

            string query = "SELECT * FROM Phong WHERE TrangThai = @trangThai ORDER BY MaPhong";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@trangThai", trangThai)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public DataTable GetPhongByMa(string maPhong)
        {
            string query = "SELECT * FROM Phong WHERE MaPhong = @maPhong";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maPhong", maPhong)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        private string GenerateMaPhong()
        {
            string query = "SELECT TOP 1 MaPhong FROM Phong ORDER BY MaPhong DESC";
            object result = DataProvider.Instance.ExecuteScalar(query);
            
            if (result == null || string.IsNullOrEmpty(result.ToString()))
            {
                return "P001";
            }
            
            string lastMa = result.ToString();
            int number = int.Parse(lastMa.Substring(1));
            return "P" + (number + 1).ToString("D3");
        }

        public bool InsertPhong(string tenPhong, string loaiPhong, int soNguoiToiDa, decimal giaPhong, string moTa, string trangThai)
        {
            string maPhong = GenerateMaPhong();
            
            string query = @"INSERT INTO Phong(MaPhong, TenPhong, LoaiPhong, SoNguoiToiDa, GiaPhong, MoTa, TrangThai)
                VALUES (@maPhong, @tenPhong, @loaiPhong, @soNguoiToiDa, @giaPhong, @moTa, @trangThai)";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maPhong", maPhong),
                new SqlParameter("@tenPhong", tenPhong),
                new SqlParameter("@loaiPhong", loaiPhong),
                new SqlParameter("@soNguoiToiDa", soNguoiToiDa),
                new SqlParameter("@giaPhong", giaPhong),
                new SqlParameter("@moTa", moTa ?? (object)DBNull.Value),
                new SqlParameter("@trangThai", trangThai)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool UpdatePhong(string maPhong, string tenPhong, string loaiPhong, int soNguoiToiDa, decimal giaPhong, string moTa, string trangThai)
        {
            string query = @"UPDATE Phong SET 
                TenPhong = @tenPhong, 
                LoaiPhong = @loaiPhong,
                SoNguoiToiDa = @soNguoiToiDa, 
                GiaPhong = @giaPhong,
                MoTa = @moTa, 
                TrangThai = @trangThai 
                WHERE MaPhong = @maPhong";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tenPhong", tenPhong),
                new SqlParameter("@loaiPhong", loaiPhong),
                new SqlParameter("@soNguoiToiDa", soNguoiToiDa),
                new SqlParameter("@giaPhong", giaPhong),
                new SqlParameter("@moTa", moTa ?? (object)DBNull.Value),
                new SqlParameter("@trangThai", trangThai),
                new SqlParameter("@maPhong", maPhong)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool DeletePhong(string maPhong)
        {
            string checkQuery = @"SELECT COUNT(*) FROM ChiTietHoaDon WHERE MaDV = @maPhong";
            
            SqlParameter[] checkParams = new SqlParameter[]
            {
                new SqlParameter("@maPhong", maPhong)
            };
            
            object count = DataProvider.Instance.ExecuteScalar(checkQuery, checkParams);
            
            if (Convert.ToInt32(count) > 0)
            {
                throw new Exception("Không thể xóa phòng đang được sử dụng trong hóa đơn!");
            }

            string query = "DELETE FROM Phong WHERE MaPhong = @maPhong";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maPhong", maPhong)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }
        
        public bool UpdateTrangThaiPhong(string maPhong, string trangThai)
        {
            string query = "UPDATE Phong SET TrangThai = @trangThai WHERE MaPhong = @maPhong";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@trangThai", trangThai),
                new SqlParameter("@maPhong", maPhong)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // Thêm phương thức mới:
        public DataTable SearchPhongByMa(string maPhong)
        {
            try
            {
                string query = @"
                    SELECT MaPhong, TenPhong, LoaiPhong, SoNguoiToiDa, GiaPhong, TrangThai, MoTa
                    FROM Phong 
                    WHERE MaPhong LIKE @MaPhong
                    ORDER BY MaPhong";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaPhong", "%" + maPhong + "%")
                };
                
                return DataProvider.Instance.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm phòng: " + ex.Message);
            }
        }
    }
}