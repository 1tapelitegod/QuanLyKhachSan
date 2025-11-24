using System;
using System.Data;
using System.Data.SqlClient;

namespace BTL1.DAL
{
    public class ThongKeDAL
    {
        private static ThongKeDAL instance;

        public static ThongKeDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new ThongKeDAL();
                return instance;
            }
        }

        private ThongKeDAL() { }

        public decimal GetTongDoanhThu(DateTime tuNgay, DateTime denNgay)
        {
            string query = @"SELECT ISNULL(SUM(TongTien), 0) 
                FROM HoaDon 
                WHERE NgayLap BETWEEN @tuNgay AND @denNgay";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tuNgay", tuNgay),
                new SqlParameter("@denNgay", denNgay)
            };
            
            object result = DataProvider.Instance.ExecuteScalar(query, parameters);
            return result != null ? Convert.ToDecimal(result) : 0;
        }

        public int GetTongHoaDon(DateTime tuNgay, DateTime denNgay)
        {
            string query = @"SELECT COUNT(*) 
                FROM HoaDon 
                WHERE NgayLap BETWEEN @tuNgay AND @denNgay";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tuNgay", tuNgay),
                new SqlParameter("@denNgay", denNgay)
            };
            
            object result = DataProvider.Instance.ExecuteScalar(query, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int GetTongKhachHang()
        {
            string query = "SELECT COUNT(*) FROM KhachHang";
            object result = DataProvider.Instance.ExecuteScalar(query);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int GetSoPhongTrong()
        {
            string query = "SELECT COUNT(*) FROM Phong WHERE TrangThai = N'Trống'";
            object result = DataProvider.Instance.ExecuteScalar(query);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public DataTable ThongKeDoanhThuTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            string query = @"
                SELECT 
                    CAST(NgayLap AS DATE) AS Ngay,
                    COUNT(*) AS SoHoaDon,
                    SUM(TongTien) AS DoanhThu
                FROM HoaDon
                WHERE NgayLap BETWEEN @tuNgay AND @denNgay
                GROUP BY CAST(NgayLap AS DATE)
                ORDER BY Ngay DESC";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tuNgay", tuNgay),
                new SqlParameter("@denNgay", denNgay)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public DataTable ThongKeDoanhThuTheoThang(DateTime tuNgay, DateTime denNgay)
        {
            string query = @"
                SELECT 
                    YEAR(NgayLap) AS Nam,
                    MONTH(NgayLap) AS Thang,
                    COUNT(*) AS SoHoaDon,
                    SUM(TongTien) AS DoanhThu
                FROM HoaDon
                WHERE NgayLap BETWEEN @tuNgay AND @denNgay
                GROUP BY YEAR(NgayLap), MONTH(NgayLap)
                ORDER BY Nam DESC, Thang DESC";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tuNgay", tuNgay),
                new SqlParameter("@denNgay", denNgay)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public DataTable ThongKePhongTheoTrangThai()
        {
            string query = @"
                SELECT 
                    TrangThai,
                    COUNT(*) AS SoLuong,
                    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Phong) AS DECIMAL(5,2)) AS TyLe
                FROM Phong
                GROUP BY TrangThai
                ORDER BY SoLuong DESC";
            
            return DataProvider.Instance.ExecuteQuery(query);
        }

        public DataTable ThongKeTopKhachHang(DateTime tuNgay, DateTime denNgay, int top)
        {
            string query = @"
                SELECT TOP (@top)
                    kh.MaKH,
                    kh.TenKH,
                    kh.SoDienThoai,
                    COUNT(hd.MaHD) AS SoLanDat,
                    SUM(hd.TongTien) AS TongChiTieu
                FROM KhachHang kh
                INNER JOIN HoaDon hd ON kh.MaKH = hd.MaKH
                WHERE hd.NgayLap BETWEEN @tuNgay AND @denNgay
                GROUP BY kh.MaKH, kh.TenKH, kh.SoDienThoai
                ORDER BY TongChiTieu DESC";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@top", top),
                new SqlParameter("@tuNgay", tuNgay),
                new SqlParameter("@denNgay", denNgay)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }

        public DataTable ThongKeNhanVien(DateTime tuNgay, DateTime denNgay)
        {
            string query = @"
                SELECT 
                    nv.MaNV,
                    nv.TenNV,
                    nv.ChucVu,
                    COUNT(hd.MaHD) AS SoHoaDon,
                    ISNULL(SUM(hd.TongTien), 0) AS DoanhThu
                FROM NhanVien nv
                LEFT JOIN HoaDon hd ON nv.MaNV = hd.MaNV 
                    AND hd.NgayLap BETWEEN @tuNgay AND @denNgay
                GROUP BY nv.MaNV, nv.TenNV, nv.ChucVu
                ORDER BY DoanhThu DESC";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tuNgay", tuNgay),
                new SqlParameter("@denNgay", denNgay)
            };
            
            return DataProvider.Instance.ExecuteQuery(query, parameters);
        }
    }
}