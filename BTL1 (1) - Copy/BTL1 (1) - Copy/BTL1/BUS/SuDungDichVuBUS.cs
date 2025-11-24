using System;
using System.Data;
using BTL1.DAL;

namespace BTL1.BUS
{
    public class SuDungDichVuBUS
    {
        private static SuDungDichVuBUS instance;

        public static SuDungDichVuBUS Instance
        {
            get
            {
                if (instance == null)
                    instance = new SuDungDichVuBUS();
                return instance;
            }
        }

        private SuDungDichVuBUS() { }

        // Lấy tất cả sử dụng dịch vụ
        public DataTable GetAllSuDungDichVu()
        {
            try
            {
                return SuDungDichVuDAL.Instance.GetAllSuDungDichVu();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách sử dụng dịch vụ: " + ex.Message);
            }
        }

        // Lấy sử dụng dịch vụ theo mã
        public DataTable GetSuDungDichVuByMa(string maSuDung)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maSuDung))
                    throw new Exception("Mã sử dụng không hợp lệ!");

                return SuDungDichVuDAL.Instance.GetSuDungDichVuByMa(maSuDung);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy thông tin sử dụng dịch vụ: " + ex.Message);
            }
        }

        // Tìm kiếm sử dụng dịch vụ
        public DataTable SearchSuDungDichVu(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return SuDungDichVuDAL.Instance.GetAllSuDungDichVu();

                return SuDungDichVuDAL.Instance.SearchSuDungDichVu(keyword);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        // Thêm sử dụng dịch vụ
        public bool InsertSuDungDichVu(string maPhong, string maDV, string maKH, 
            int soLuong, DateTime ngaySuDung, string ghiChu, string trangThai)
        {
            try
            {
                // Validate
                ValidateSuDungDichVu(maPhong, maDV, maKH, soLuong, trangThai);

                // Tạo mã tự động
                string ngayHienTai = DateTime.Now.ToString("ddMMyyyy");
                string maSuDung = SuDungDichVuDAL.Instance.GenerateMaSuDungDichVu(ngayHienTai);

                // Kiểm tra mã đã tồn tại (đề phòng)
                if (SuDungDichVuDAL.Instance.CheckMaSuDungExists(maSuDung))
                    throw new Exception("Mã sử dụng đã tồn tại! Vui lòng thử lại.");

                // Thêm mới
                return SuDungDichVuDAL.Instance.InsertSuDungDichVu(
                    maSuDung, maPhong, maDV, maKH, soLuong, ngaySuDung, ghiChu, trangThai);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thêm sử dụng dịch vụ: " + ex.Message);
            }
        }

        // Cập nhật sử dụng dịch vụ
        public bool UpdateSuDungDichVu(string maSuDung, string maPhong, string maDV, string maKH, 
            int soLuong, DateTime ngaySuDung, string ghiChu, string trangThai)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maSuDung))
                    throw new Exception("Mã sử dụng không hợp lệ!");

                // Validate
                ValidateSuDungDichVu(maPhong, maDV, maKH, soLuong, trangThai);

                return SuDungDichVuDAL.Instance.UpdateSuDungDichVu(
                    maSuDung, maPhong, maDV, maKH, soLuong, ngaySuDung, ghiChu, trangThai);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật sử dụng dịch vụ: " + ex.Message);
            }
        }

        // Xóa sử dụng dịch vụ
        public bool DeleteSuDungDichVu(string maSuDung)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maSuDung))
                    throw new Exception("Mã sử dụng không hợp lệ!");

                return SuDungDichVuDAL.Instance.DeleteSuDungDichVu(maSuDung);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa sử dụng dịch vụ: " + ex.Message);
            }
        }

        // Lấy sử dụng dịch vụ theo phòng
        public DataTable GetSuDungDichVuByPhong(string maPhong)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maPhong))
                    throw new Exception("Mã phòng không hợp lệ!");

                return SuDungDichVuDAL.Instance.GetSuDungDichVuByPhong(maPhong);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy sử dụng dịch vụ theo phòng: " + ex.Message);
            }
        }

        // Thống kê sử dụng dịch vụ theo ngày
        public DataTable ThongKeSuDungDichVuTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            try
            {
                if (tuNgay > denNgay)
                    throw new Exception("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc!");

                return SuDungDichVuDAL.Instance.ThongKeSuDungDichVuTheoNgay(tuNgay, denNgay);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thống kê sử dụng dịch vụ: " + ex.Message);
            }
        }

        // Validate sử dụng dịch vụ
        private void ValidateSuDungDichVu(string maPhong, string maDV, string maKH, int soLuong, string trangThai)
        {
            if (string.IsNullOrWhiteSpace(maPhong))
                throw new Exception("Mã phòng không được để trống!");

            if (string.IsNullOrWhiteSpace(maDV))
                throw new Exception("Mã dịch vụ không được để trống!");

            if (string.IsNullOrWhiteSpace(maKH))
                throw new Exception("Mã khách hàng không được để trống!");

            if (soLuong <= 0)
                throw new Exception("Số lượng phải lớn hơn 0!");

            if (soLuong > 1000)
                throw new Exception("Số lượng không được vượt quá 1000!");

            if (string.IsNullOrWhiteSpace(trangThai))
                throw new Exception("Trạng thái không được để trống!");

            if (trangThai != "Chưa thanh toán" && trangThai != "Đã thanh toán" && trangThai != "Đã hủy")
                throw new Exception("Trạng thái không hợp lệ!");
        }

        // Thêm phương thức mới vào cuối class:
        public DataTable SearchByMaSuDung(string maSuDung)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maSuDung))
                    return GetAllSuDungDichVu();

                return SuDungDichVuDAL.Instance.SearchByMaSuDung(maSuDung);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm: " + ex.Message);
            }
        }
    }
}