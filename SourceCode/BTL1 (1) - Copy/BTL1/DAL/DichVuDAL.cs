using System;
using System.Data;
using System.Data.SqlClient;

namespace BTL1.DAL
{
    public class DichVuDAL
    {
        private static DichVuDAL instance;

        public static DichVuDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new DichVuDAL();
                return instance;
            }
        }

        private DichVuDAL() { }

        public DataTable GetAllDichVu()
        {
            string query = "SELECT * FROM DichVu ORDER BY MaDV DESC";
            return DataProvider.Instance.ExecuteQuery(query);
        }

        public DataTable SearchDichVu(string keyword)
        {
            string query = @"SELECT * FROM DichVu 
                WHERE TenDV LIKE N'%' + @keyword + '%' 
                OR MaDV LIKE '%' + @keyword + '%'
                OR DonVi LIKE N'%' + @keyword + '%'
                OR MoTa LIKE N'%' + @keyword + '%'
                ORDER BY MaDV DESC";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@keyword", keyword)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public DataTable SearchDichVuByMa(string maDV)
        {
            try
            {
                string query = @"
                    SELECT MaDV, TenDV, GiaDV, DonVi, MoTa
                    FROM DichVu 
                    WHERE MaDV LIKE @MaDV
                    ORDER BY MaDV";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaDV", "%" + maDV + "%")
                };
                
                return DataProvider.Instance.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm dịch vụ: " + ex.Message);
            }
        }

        private string GenerateMaDV()
        {
            string query = "SELECT TOP 1 MaDV FROM DichVu ORDER BY MaDV DESC";
            object result = DataProvider.Instance.ExecuteScalar(query);
            
            if (result == null || string.IsNullOrEmpty(result.ToString()))
            {
                return "DV001";
            }
            
            string lastMa = result.ToString();
            int number = int.Parse(lastMa.Substring(2));
            return "DV" + (number + 1).ToString("D3");
        }

        public bool InsertDichVu(string tenDV, decimal giaDV, string donVi, string moTa)
        {
            string maDV = GenerateMaDV();
            
            string query = @"INSERT INTO DichVu(MaDV, TenDV, GiaDV, DonVi, MoTa)
                VALUES (@maDV, @tenDV, @giaDV, @donVi, @moTa)";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maDV", maDV),
                new SqlParameter("@tenDV", tenDV),
                new SqlParameter("@giaDV", giaDV),
                new SqlParameter("@donVi", donVi),
                new SqlParameter("@moTa", moTa)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool UpdateDichVu(string maDV, string tenDV, decimal giaDV, string donVi, string moTa)
        {
            string query = @"UPDATE DichVu SET 
                TenDV = @tenDV, 
                GiaDV = @giaDV, 
                DonVi = @donVi, 
                MoTa = @moTa 
                WHERE MaDV = @maDV";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tenDV", tenDV),
                new SqlParameter("@giaDV", giaDV),
                new SqlParameter("@donVi", donVi),
                new SqlParameter("@moTa", moTa),
                new SqlParameter("@maDV", maDV)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool DeleteDichVu(string maDV)
        {
            string checkQuery = @"SELECT COUNT(*) FROM ChiTietHoaDon WHERE MaDV = @maDV";
            
            SqlParameter[] checkParams = new SqlParameter[]
            {
                new SqlParameter("@maDV", maDV)
            };
            
            object count = DataProvider.Instance.ExecuteScalar(checkQuery, checkParams);
            
            if (Convert.ToInt32(count) > 0)
            {
                throw new Exception("Không thể xóa dịch vụ đang được sử dụng trong hóa đơn!");
            }

            string query = "DELETE FROM DichVu WHERE MaDV = @maDV";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maDV", maDV)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public DataTable GetDichVuByMa(string maDV)
        {
            string query = "SELECT * FROM DichVu WHERE MaDV = @maDV";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maDV", maDV)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public int GetSoLuongTon(string maDV)
        {
            string query = "SELECT ISNULL(SoLuongTon, 0) FROM DichVu WHERE MaDV = @maDV";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maDV", maDV)
            };
            
            object result = DataProvider.Instance.ExecuteScalar(query, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public bool UpdateSoLuongTon(string maDV, int soLuong)
        {
            string query = "UPDATE DichVu SET SoLuongTon = SoLuongTon + @soLuong WHERE MaDV = @maDV";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@soLuong", soLuong),
                new SqlParameter("@maDV", maDV)
            };
            
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public bool CheckDichVuExists(string tenDV)
        {
            string query = "SELECT COUNT(*) FROM DichVu WHERE TenDV = @tenDV";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tenDV", tenDV)
            };
            
            object result = DataProvider.Instance.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) > 0;
        }
    }
}