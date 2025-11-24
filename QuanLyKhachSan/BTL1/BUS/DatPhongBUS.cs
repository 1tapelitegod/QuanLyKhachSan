using System;
using System.Data;
using BTL1.DAL;

namespace BTL1.BUS
{
    public class DatPhongBUS
    {
        private static DatPhongBUS instance;

        public static DatPhongBUS Instance
        {
            get
            {
                if (instance == null)
                    instance = new DatPhongBUS();
                return instance;
            }
        }

        private DatPhongBUS() { }

        public DataTable GetAllDatPhong()
        {
            try
            {
                return DatPhongDAL.Instance.GetAllDatPhong();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách đặt phòng: " + ex.Message);
            }
        }

        public DataTable GetDatPhongByMa(string maDatPhong)
        {
            try
            {
                return DatPhongDAL.Instance.GetDatPhongByMa(maDatPhong);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy thông tin đặt phòng: " + ex.Message);
            }
        }

        public DataTable SearchDatPhong(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return GetAllDatPhong();

                return DatPhongDAL.Instance.SearchDatPhong(keyword.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm đặt phòng: " + ex.Message);
            }
        }

        public string GenerateMaDatPhong()
        {
            try
            {
                return DatPhongDAL.Instance.GenerateMaDatPhong();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tạo mã đặt phòng: " + ex.Message);
            }
        }

        public bool KiemTraPhongTrong(string maPhong, DateTime ngayNhanPhong, DateTime ngayTraPhong)
        {
            try
            {
                return DatPhongDAL.Instance.KiemTraPhongTrong(maPhong, ngayNhanPhong, ngayTraPhong);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi kiểm tra phòng trống: " + ex.Message);
            }
        }

        public bool DatPhong(string maKH, string maPhong, string maNV, DateTime ngayNhanPhong, 
            DateTime ngayTraPhong, int soNguoi, decimal tienCoc, string ghiChu)
        {
            try
            {
                // Validate
                ValidateDatPhong(ngayNhanPhong, ngayTraPhong, soNguoi, tienCoc);

                // Kiểm tra phòng trống
                if (!KiemTraPhongTrong(maPhong, ngayNhanPhong, ngayTraPhong))
                {
                    throw new Exception("Phòng đã được đặt trong khoảng thời gian này!");
                }

                // Tạo mã đặt phòng
                string maDatPhong = GenerateMaDatPhong();

                // Insert đặt phòng
                bool result = DatPhongDAL.Instance.InsertDatPhong(
                    maDatPhong, maKH, maPhong, maNV, DateTime.Now,
                    ngayNhanPhong, ngayTraPhong, soNguoi, tienCoc, ghiChu);

                if (result)
                {
                    // Cập nhật trạng thái phòng thành "Đã đặt"
                    PhongDAL.Instance.UpdateTrangThaiPhong(maPhong, "Đã đặt");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi đặt phòng: {ex.Message}");
            }
        }

        public bool UpdateDatPhong(string maDatPhong, string maKH, string maPhong, 
            DateTime ngayNhanPhong, DateTime ngayTraPhong, int soNguoi, decimal tienCoc, string ghiChu)
        {
            try
            {
                ValidateDatPhong(ngayNhanPhong, ngayTraPhong, soNguoi, tienCoc);

                // Lấy thông tin đặt phòng cũ để kiểm tra phòng
                DataTable dtOld = DatPhongDAL.Instance.GetDatPhongByMa(maDatPhong);
                if (dtOld.Rows.Count == 0)
                    throw new Exception("Không tìm thấy thông tin đặt phòng!");

                string maPhongCu = dtOld.Rows[0]["MaPhong"].ToString();

                // Nếu đổi phòng, kiểm tra phòng mới có trống không
                if (maPhong != maPhongCu)
                {
                    if (!KiemTraPhongTrong(maPhong, ngayNhanPhong, ngayTraPhong))
                    {
                        throw new Exception("Phòng mới đã được đặt trong khoảng thời gian này!");
                    }
                }

                // Cập nhật đặt phòng
                bool result = DatPhongDAL.Instance.UpdateDatPhongFull(
                    maDatPhong, maKH, maPhong, ngayNhanPhong, ngayTraPhong, soNguoi, tienCoc, ghiChu);

                if (result && maPhong != maPhongCu)
                {
                    // Nếu đổi phòng, cập nhật trạng thái
                    PhongDAL.Instance.UpdateTrangThaiPhong(maPhongCu, "Trống");
                    PhongDAL.Instance.UpdateTrangThaiPhong(maPhong, "Đã đặt");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi cập nhật đặt phòng: {ex.Message}");
            }
        }

        public bool HuyDatPhong(string maDatPhong, string maPhong)
        {
            try
            {
                // Xóa đặt phòng
                bool result = DatPhongDAL.Instance.DeleteDatPhong(maDatPhong);

                if (result)
                {
                    // Cập nhật trạng thái phòng về "Trống"
                    PhongDAL.Instance.UpdateTrangThaiPhong(maPhong, "Trống");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi hủy đặt phòng: {ex.Message}");
            }
        }

        public bool NhanPhong(string maDatPhong, string maPhong)
        {
            try
            {
                // Cập nhật trạng thái đặt phòng thành "Đang ở"
                bool resultDP = DatPhongDAL.Instance.UpdateTrangThaiDatPhong(maDatPhong, "Đang ở");

                if (resultDP)
                {
                    // Cập nhật trạng thái phòng thành "Đang sử dụng"
                    PhongDAL.Instance.UpdateTrangThaiPhong(maPhong, "Đang sử dụng");
                }

                return resultDP;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi nhận phòng: {ex.Message}");
            }
        }

        public bool TraPhong(string maDatPhong, string maPhong)
        {
            try
            {
                // Cập nhật trạng thái đặt phòng thành "Đã trả"
                bool resultDP = DatPhongDAL.Instance.UpdateTrangThaiDatPhong(maDatPhong, "Đã trả");

                if (resultDP)
                {
                    // Cập nhật trạng thái phòng về "Trống"
                    PhongDAL.Instance.UpdateTrangThaiPhong(maPhong, "Trống");
                }

                return resultDP;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trả phòng: {ex.Message}");
            }
        }

        public DataTable GetDatPhongByTrangThai(string trangThai)
        {
            try
            {
                return DatPhongDAL.Instance.GetDatPhongByTrangThai(trangThai);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy đặt phòng theo trạng thái: " + ex.Message);
            }
        }

        /// <summary>
        /// Tính tổng tiền phòng = (Giá phòng × Số ngày) - Tiền đặt cọc
        /// </summary>
        /// <param name="giaPhong">Giá phòng mỗi đêm</param>
        /// <param name="ngayNhanPhong">Ngày nhận phòng</param>
        /// <param name="ngayTraPhong">Ngày trả phòng</param>
        /// <param name="tienCoc">Tiền đặt cọc (mặc định = 0)</param>
        /// <returns>Tổng tiền sau khi trừ đặt cọc</returns>
        public decimal TinhTongTien(decimal giaPhong, DateTime ngayNhanPhong, DateTime ngayTraPhong, decimal tienCoc = 0)
        {
            // Tính số ngày ở
            int soNgay = (ngayTraPhong - ngayNhanPhong).Days;
            if (soNgay <= 0) soNgay = 1;
            
            // Tổng tiền = Giá phòng × Số ngày
            decimal tongTien = giaPhong * soNgay;
            
            // Trừ tiền đặt cọc
            decimal tongTienSauCoc = tongTien - tienCoc;
            
            // Đảm bảo không âm (nếu cọc lớn hơn tổng tiền thì trả về 0)
            return tongTienSauCoc < 0 ? 0 : tongTienSauCoc;
        }

        /// <summary>
        /// Overload: Tính tổng tiền không trừ cọc (giữ tương thích ngược)
        /// </summary>
        public decimal TinhTongTienKhongTruCoc(decimal giaPhong, DateTime ngayNhanPhong, DateTime ngayTraPhong)
        {
            int soNgay = (ngayTraPhong - ngayNhanPhong).Days;
            if (soNgay <= 0) soNgay = 1;
            return giaPhong * soNgay;
        }

        private void ValidateDatPhong(DateTime ngayNhanPhong, DateTime ngayTraPhong, int soNguoi, decimal tienCoc)
        {
            if (ngayTraPhong <= ngayNhanPhong)
                throw new Exception("Ngày trả phòng phải sau ngày nhận phòng!");

            if (soNguoi <= 0)
                throw new Exception("Số người phải lớn hơn 0!");

            if (tienCoc < 0)
                throw new Exception("Tiền cọc không hợp lệ!");
        }
    }
}