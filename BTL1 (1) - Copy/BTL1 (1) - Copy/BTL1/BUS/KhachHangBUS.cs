using System;
using System.Data;
using BTL1.DAL;

namespace BTL1.BUS
{
    public class KhachHangBUS
    {
        private static KhachHangBUS instance;

        public static KhachHangBUS Instance
        {
            get
            {
                if (instance == null)
                    instance = new KhachHangBUS();
                return instance;
            }
        }

        private KhachHangBUS() { }

        public DataTable GetAllKhachHang()
        {
            try
            {
                return KhachHangDAL.Instance.GetAllKhachHang();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách khách hàng: " + ex.Message);
            }
        }

        public DataTable SearchKhachHang(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return GetAllKhachHang();

                return KhachHangDAL.Instance.SearchKhachHang(keyword.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm khách hàng: " + ex.Message);
            }
        }

        public bool InsertKhachHang(string tenKH, string cmnd, string sdt, string diaChi, DateTime ngaySinh, string gioiTinh, string quocTich)
        {
            try
            {
                ValidateKhachHang(tenKH, sdt, cmnd, ngaySinh);

                if (KhachHangDAL.Instance.CheckKhachHangExists(sdt))
                    throw new Exception($"Số điện thoại '{sdt}' đã tồn tại trong hệ thống!");

                return KhachHangDAL.Instance.InsertKhachHang(tenKH, cmnd, sdt, diaChi, ngaySinh, gioiTinh, quocTich);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thêm khách hàng: " + ex.Message);
            }
        }

        public bool UpdateKhachHang(string maKH, string tenKH, string cmnd, string sdt, string diaChi, DateTime ngaySinh, string gioiTinh, string quocTich)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maKH))
                    throw new Exception("Mã khách hàng không hợp lệ!");

                ValidateKhachHang(tenKH, sdt, cmnd, ngaySinh);

                return KhachHangDAL.Instance.UpdateKhachHang(maKH, tenKH, cmnd, sdt, diaChi, ngaySinh, gioiTinh, quocTich);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật khách hàng: " + ex.Message);
            }
        }

        public bool DeleteKhachHang(string maKH)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maKH))
                    throw new Exception("Mã khách hàng không hợp lệ!");

                return KhachHangDAL.Instance.DeleteKhachHang(maKH);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa khách hàng: " + ex.Message);
            }
        }

        public DataTable GetKhachHangByMa(string maKH)
        {
            try
            {
                return KhachHangDAL.Instance.GetKhachHangByMa(maKH);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy thông tin khách hàng: " + ex.Message);
            }
        }

        public DataTable GetKhachHangBySDT(string sdt)
        {
            try
            {
                return KhachHangDAL.Instance.GetKhachHangBySDT(sdt);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy thông tin khách hàng: " + ex.Message);
            }
        }

        public DataTable SearchKhachHangByMa(string maKH)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maKH))
                    return GetAllKhachHang();

                return KhachHangDAL.Instance.SearchKhachHangByMa(maKH);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm khách hàng: " + ex.Message);
            }
        }

        private void ValidateKhachHang(string tenKH, string sdt, string cmnd, DateTime ngaySinh)
        {
            if (string.IsNullOrWhiteSpace(tenKH))
                throw new Exception("Tên khách hàng không được để trống!");

            if (tenKH.Length > 200)
                throw new Exception("Tên khách hàng không được quá 200 ký tự!");
            
            if (string.IsNullOrWhiteSpace(sdt))
                throw new Exception("Số điện thoại không được để trống!");

            if (sdt.Length < 10 || sdt.Length > 11)
                throw new Exception("Số điện thoại phải có 10-11 số!");

            if (!string.IsNullOrWhiteSpace(cmnd) && (cmnd.Length < 9 || cmnd.Length > 12))
                throw new Exception("CMND/CCCD phải có 9-12 số!");

            if (ngaySinh > DateTime.Now.AddYears(-16))
                throw new Exception("Khách hàng phải trên 16 tuổi!");
        }
    }
}