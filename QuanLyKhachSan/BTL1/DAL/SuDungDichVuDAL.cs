using System;
using System.Data;
using System.Data.SqlClient;

namespace BTL1.DAL
{
    public class SuDungDichVuDAL
    {
        private static SuDungDichVuDAL instance;

        public static SuDungDichVuDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new SuDungDichVuDAL();
                return instance;
            }
        }

        private SuDungDichVuDAL() { }

        // Lấy tất cả sử dụng dịch vụ
        public DataTable GetAllSuDungDichVu()
        {
            string query = "EXEC sp_GetAllSuDungDichVu";
            return DataProvider.Instance.ExecuteQuery(query);
        }

        // Lấy sử dụng dịch vụ theo mã
        public DataTable GetSuDungDichVuByMa(string maSuDung)
        {
            string query = "EXEC sp_GetSuDungDichVuByMa @MaSuDung";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaSuDung", maSuDung)
            };
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        // Tìm kiếm sử dụng dịch vụ
        public DataTable SearchSuDungDichVu(string keyword)
        {
            string query = "EXEC sp_SearchSuDungDichVu @Keyword";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", keyword)
            };
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        // Tạo mã sử dụng dịch vụ tự động
        public string GenerateMaSuDungDichVu(string ngayHienTai)
        {
            string query = "EXEC sp_TaoMaSuDungDichVu @NgayHienTai, @MaSuDung OUTPUT";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@NgayHienTai", ngayHienTai),
                new SqlParameter("@MaSuDung", SqlDbType.NVarChar, 50) { Direction = ParameterDirection.Output }
            };

            DataProvider.Instance.ExecuteQuery(query, parameters);
            return parameters[1].Value.ToString();
        }

        // Thêm sử dụng dịch vụ
        public bool InsertSuDungDichVu(string maSuDung, string maPhong, string maDV, string maKH, 
            int soLuong, DateTime ngaySuDung, string ghiChu, string trangThai)
        {
            string query = "EXEC sp_InsertSuDungDichVu @MaSuDung, @MaPhong, @MaDV, @MaKH, @SoLuong, @NgaySuDung, @GhiChu, @TrangThai";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaSuDung", maSuDung),
                new SqlParameter("@MaPhong", maPhong),
                new SqlParameter("@MaDV", maDV),
                new SqlParameter("@MaKH", maKH),
                new SqlParameter("@SoLuong", soLuong),
                new SqlParameter("@NgaySuDung", ngaySuDung),
                new SqlParameter("@GhiChu", (object)ghiChu ?? DBNull.Value),
                new SqlParameter("@TrangThai", trangThai)
            };

            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // Cập nhật sử dụng dịch vụ
        public bool UpdateSuDungDichVu(string maSuDung, string maPhong, string maDV, string maKH, 
            int soLuong, DateTime ngaySuDung, string ghiChu, string trangThai)
        {
            string query = "EXEC sp_UpdateSuDungDichVu @MaSuDung, @MaPhong, @MaDV, @MaKH, @SoLuong, @NgaySuDung, @GhiChu, @TrangThai";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaSuDung", maSuDung),
                new SqlParameter("@MaPhong", maPhong),
                new SqlParameter("@MaDV", maDV),
                new SqlParameter("@MaKH", maKH),
                new SqlParameter("@SoLuong", soLuong),
                new SqlParameter("@NgaySuDung", ngaySuDung),
                new SqlParameter("@GhiChu", (object)ghiChu ?? DBNull.Value),
                new SqlParameter("@TrangThai", trangThai)
            };

            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // Xóa sử dụng dịch vụ
        public bool DeleteSuDungDichVu(string maSuDung)
        {
            string query = "EXEC sp_DeleteSuDungDichVu @MaSuDung";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaSuDung", maSuDung)
            };

            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // Lấy sử dụng dịch vụ theo phòng
        public DataTable GetSuDungDichVuByPhong(string maPhong)
        {
            string query = "EXEC sp_GetSuDungDichVuByPhong @MaPhong";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaPhong", maPhong)
            };

            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        // Thống kê sử dụng dịch vụ theo ngày
        public DataTable ThongKeSuDungDichVuTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            string query = "EXEC sp_ThongKeSuDungDichVuTheoNgay @TuNgay, @DenNgay";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TuNgay", tuNgay),
                new SqlParameter("@DenNgay", denNgay)
            };

            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        // Kiểm tra mã sử dụng dịch vụ đã tồn tại
        public bool CheckMaSuDungExists(string maSuDung)
        {
            string query = "SELECT COUNT(*) FROM SuDungDichVu WHERE MaSuDung = @MaSuDung";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaSuDung", maSuDung)
            };

            int count = (int)DataProvider.Instance.ExecuteScalar(query, parameters);
            return count > 0;
        }

        // Tìm kiếm theo mã sử dụng
        public DataTable SearchByMaSuDung(string maSuDung)
        {
            try
            {
                string query = @"
                    SELECT 
                        sd.MaSuDung,
                        sd.MaPhong,
                        p.TenPhong,
                        dv.TenDV,
                        kh.TenKH,
                        sd.SoLuong,
                        dv.GiaDV as DonGia,
                        (sd.SoLuong * dv.GiaDV) as ThanhTien,
                        sd.NgaySuDung,
                        sd.TrangThai,
                        sd.MaDV,
                        sd.MaKH,
                        kh.SoDienThoai,
                        sd.GhiChu
                    FROM SuDungDichVu sd
                    JOIN Phong p ON sd.MaPhong = p.MaPhong
                    JOIN DichVu dv ON sd.MaDV = dv.MaDV
                    JOIN KhachHang kh ON sd.MaKH = kh.MaKH
                    WHERE sd.MaSuDung LIKE @MaSuDung
                    ORDER BY sd.NgaySuDung DESC";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaSuDung", "%" + maSuDung + "%")
                };
                
                return DataProvider.Instance.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        // ✅ THÊM METHOD MỚI: Cập nhật trạng thái theo MaDV và MaKH
        public bool UpdateTrangThaiByMaDV(string maDV, string maKH, string trangThai)
        {
            try
            {
                string query = @"UPDATE SuDungDichVu 
                    SET TrangThai = @TrangThai 
                    WHERE MaDV = @MaDV AND MaKH = @MaKH AND TrangThai = N'Chưa thanh toán'";
                
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@TrangThai", trangThai),
                    new SqlParameter("@MaDV", maDV),
                    new SqlParameter("@MaKH", maKH)
                };
                
                int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật trạng thái sử dụng dịch vụ: " + ex.Message);
            }
        }
    }
}