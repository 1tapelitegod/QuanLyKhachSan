using System;
using System.Data;
using BTL1.DAL;

namespace BTL1.BUS
{
    public class PhongBUS
    {
        private static PhongBUS instance;

        public static PhongBUS Instance
        {
            get
            {
                if (instance == null)
                    instance = new PhongBUS();
                return instance;
            }
        }

        private PhongBUS() { }

        public DataTable GetAllPhong()
        {
            try
            {
                return PhongDAL.Instance.GetAllPhong();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách phòng: " + ex.Message);
            }
        }

        public DataTable GetPhongTrong()
        {
            try
            {
                // Gọi từ DatPhongDAL vì phương thức đã được định nghĩa ở đó
                return DatPhongDAL.Instance.GetPhongTrong();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách phòng trống: " + ex.Message);
            }
        }

        public DataTable SearchPhong(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return GetAllPhong();

                return PhongDAL.Instance.SearchPhong(keyword.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm phòng: " + ex.Message);
            }
        }

        public DataTable GetPhongByTrangThai(string trangThai)
        {
            try
            {
                return PhongDAL.Instance.GetPhongByTrangThai(trangThai);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy phòng theo trạng thái: " + ex.Message);
            }
        }

        public DataTable GetPhongByMa(string maPhong)
        {
            try
            {
                return PhongDAL.Instance.GetPhongByMa(maPhong);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy thông tin phòng: " + ex.Message);
            }
        }

        public bool InsertPhong(string tenPhong, string loaiPhong, int soNguoiToiDa, decimal giaPhong, string moTa, string trangThai)
        {
            try
            {
                ValidatePhong(tenPhong, loaiPhong, soNguoiToiDa, giaPhong, trangThai);

                return PhongDAL.Instance.InsertPhong(tenPhong, loaiPhong, soNguoiToiDa, giaPhong, moTa, trangThai);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thêm phòng: " + ex.Message);
            }
        }

        public bool UpdatePhong(string maPhong, string tenPhong, string loaiPhong, int soNguoiToiDa, decimal giaPhong, string moTa, string trangThai)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maPhong))
                    throw new Exception("Mã phòng không hợp lệ!");

                ValidatePhong(tenPhong, loaiPhong, soNguoiToiDa, giaPhong, trangThai);

                return PhongDAL.Instance.UpdatePhong(maPhong, tenPhong, loaiPhong, soNguoiToiDa, giaPhong, moTa, trangThai);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật phòng: " + ex.Message);
            }
        }

        public bool DeletePhong(string maPhong)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maPhong))
                    throw new Exception("Mã phòng không hợp lệ!");

                return PhongDAL.Instance.DeletePhong(maPhong);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa phòng: " + ex.Message);
            }
        }

        public bool UpdateTrangThaiPhong(string maPhong, string trangThai)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maPhong))
                    throw new Exception("Mã phòng không hợp lệ!");

                if (string.IsNullOrWhiteSpace(trangThai))
                    throw new Exception("Trạng thái không hợp lệ!");

                return PhongDAL.Instance.UpdateTrangThaiPhong(maPhong, trangThai);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật trạng thái phòng: " + ex.Message);
            }
        }

        public DataTable SearchPhongByMa(string maPhong)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maPhong))
                    return GetAllPhong();

                return PhongDAL.Instance.SearchPhongByMa(maPhong);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm phòng: " + ex.Message);
            }
        }

        private void ValidatePhong(string tenPhong, string loaiPhong, int soNguoiToiDa, decimal giaPhong, string trangThai)
        {
            if (string.IsNullOrWhiteSpace(tenPhong))
                throw new Exception("Tên phòng không được để trống!");

            if (tenPhong.Length > 100)
                throw new Exception("Tên phòng không được quá 100 ký tự!");
            
            if (string.IsNullOrWhiteSpace(loaiPhong))
                throw new Exception("Loại phòng không được để trống!");

            if (soNguoiToiDa < 1 || soNguoiToiDa > 20)
                throw new Exception("Số người tối đa phải từ 1 đến 20!");

            if (giaPhong <= 0)
                throw new Exception("Giá phòng phải lớn hơn 0!");

            if (giaPhong > 999999999)
                throw new Exception("Giá phòng không được vượt quá 999,999,999 VNĐ!");

            if (string.IsNullOrWhiteSpace(trangThai))
                throw new Exception("Trạng thái không được để trống!");
        }
    }
}