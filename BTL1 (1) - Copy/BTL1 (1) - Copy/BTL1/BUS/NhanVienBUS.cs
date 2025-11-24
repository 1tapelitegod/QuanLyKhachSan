using System;
using System.Data;
using BTL1.DAL;

namespace BTL1.BUS
{
    public class NhanVienBUS
    {
        private static NhanVienBUS instance;

        public static NhanVienBUS Instance
        {
            get
            {
                if (instance == null)
                    instance = new NhanVienBUS();
                return instance;
            }
        }

        private NhanVienBUS() { }

        public DataTable GetAllNhanVien()
        {
            try
            {
                return NhanVienDAL.Instance.GetAllNhanVien();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách nhân viên: " + ex.Message);
            }
        }

        public DataTable SearchNhanVien(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return GetAllNhanVien();

                return NhanVienDAL.Instance.SearchNhanVien(keyword.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm nhân viên: " + ex.Message);
            }
        }

        public bool InsertNhanVien(string tenNV, string cmnd, string sdt, string diaChi, DateTime ngaySinh, string gioiTinh, string chucVu, decimal luong)
        {
            try
            {
                ValidateNhanVien(tenNV, sdt, cmnd, ngaySinh, luong, chucVu);

                return NhanVienDAL.Instance.InsertNhanVien(tenNV, cmnd, sdt, diaChi, ngaySinh, gioiTinh, chucVu, luong);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thêm nhân viên: " + ex.Message);
            }
        }

        public bool UpdateNhanVien(string maNV, string tenNV, string cmnd, string sdt, string diaChi, DateTime ngaySinh, string gioiTinh, string chucVu, decimal luong)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maNV))
                    throw new Exception("Mã nhân viên không hợp lệ!");

                ValidateNhanVien(tenNV, sdt, cmnd, ngaySinh, luong, chucVu);

                return NhanVienDAL.Instance.UpdateNhanVien(maNV, tenNV, cmnd, sdt, diaChi, ngaySinh, gioiTinh, chucVu, luong);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật nhân viên: " + ex.Message);
            }
        }

        public bool DeleteNhanVien(string maNV)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maNV))
                    throw new Exception("Mã nhân viên không hợp lệ!");

                return NhanVienDAL.Instance.DeleteNhanVien(maNV);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa nhân viên: " + ex.Message);
            }
        }

        public DataTable DangNhap(string username, string password)
        {
            try
            {
                return NhanVienDAL.Instance.CheckLogin(username, password);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi đăng nhập: " + ex.Message);
            }
        }

        private void ValidateNhanVien(string tenNV, string sdt, string cmnd, DateTime ngaySinh, decimal luong, string chucVu)
        {
            if (string.IsNullOrWhiteSpace(tenNV))
                throw new Exception("Tên nhân viên không được để trống!");

            if (tenNV.Length > 200)
                throw new Exception("Tên nhân viên không được quá 200 ký tự!");
            
            if (string.IsNullOrWhiteSpace(sdt))
                throw new Exception("Số điện thoại không được để trống!");

            if (sdt.Length < 10 || sdt.Length > 11)
                throw new Exception("Số điện thoại phải có 10-11 số!");

            if (!string.IsNullOrWhiteSpace(cmnd) && (cmnd.Length < 9 || cmnd.Length > 12))
                throw new Exception("CMND/CCCD phải có 9-12 số!");

            int tuoi = DateTime.Now.Year - ngaySinh.Year;
            if (tuoi < 18)
                throw new Exception("Nhân viên phải từ 18 tuổi trở lên!");

            if (luong <= 0)
                throw new Exception("Lương phải lớn hơn 0!");

            if (luong > 999999999)
                throw new Exception("Lương không được vượt quá 999,999,999 VNĐ!");

            if (string.IsNullOrWhiteSpace(chucVu))
                throw new Exception("Chức vụ không được để trống!");
        }

        public DataTable SearchNhanVienByMa(string maNV)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maNV))
                    return GetAllNhanVien();

                return NhanVienDAL.Instance.SearchNhanVienByMa(maNV);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm nhân viên: " + ex.Message);
            }
        }
    }
}