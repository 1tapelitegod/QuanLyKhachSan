using System;
using System.Data;
using BTL1.DAL;

namespace BTL1.BUS
{
    public class ThongKeBUS
    {
        private static ThongKeBUS instance;

        public static ThongKeBUS Instance
        {
            get
            {
                if (instance == null)
                    instance = new ThongKeBUS();
                return instance;
            }
        }

        private ThongKeBUS() { }

        public decimal GetTongDoanhThu(DateTime tuNgay, DateTime denNgay)
        {
            try
            {
                return ThongKeDAL.Instance.GetTongDoanhThu(tuNgay, denNgay);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy tổng doanh thu: " + ex.Message);
            }
        }

        public int GetTongHoaDon(DateTime tuNgay, DateTime denNgay)
        {
            try
            {
                return ThongKeDAL.Instance.GetTongHoaDon(tuNgay, denNgay);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy tổng hóa đơn: " + ex.Message);
            }
        }

        public int GetTongKhachHang()
        {
            try
            {
                return ThongKeDAL.Instance.GetTongKhachHang();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy tổng khách hàng: " + ex.Message);
            }
        }

        public int GetSoPhongTrong()
        {
            try
            {
                return ThongKeDAL.Instance.GetSoPhongTrong();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy số phòng trống: " + ex.Message);
            }
        }

        public DataTable ThongKeDoanhThuTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            try
            {
                if (tuNgay > denNgay)
                    throw new Exception("Ngày bắt đầu phải nhỏ hơn ngày kết thúc!");

                return ThongKeDAL.Instance.ThongKeDoanhThuTheoNgay(tuNgay, denNgay);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thống kê doanh thu theo ngày: " + ex.Message);
            }
        }

        public DataTable ThongKeDoanhThuTheoThang(DateTime tuNgay, DateTime denNgay)
        {
            try
            {
                if (tuNgay > denNgay)
                    throw new Exception("Ngày bắt đầu phải nhỏ hơn ngày kết thúc!");

                return ThongKeDAL.Instance.ThongKeDoanhThuTheoThang(tuNgay, denNgay);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thống kê doanh thu theo tháng: " + ex.Message);
            }
        }

        public DataTable ThongKePhongTheoTrangThai()
        {
            try
            {
                return ThongKeDAL.Instance.ThongKePhongTheoTrangThai();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thống kê phòng theo trạng thái: " + ex.Message);
            }
        }

        public DataTable ThongKeTopKhachHang(DateTime tuNgay, DateTime denNgay, int top)
        {
            try
            {
                if (tuNgay > denNgay)
                    throw new Exception("Ngày bắt đầu phải nhỏ hơn ngày kết thúc!");

                if (top <= 0)
                    throw new Exception("Số lượng top phải lớn hơn 0!");

                return ThongKeDAL.Instance.ThongKeTopKhachHang(tuNgay, denNgay, top);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thống kê top khách hàng: " + ex.Message);
            }
        }

        public DataTable ThongKeNhanVien(DateTime tuNgay, DateTime denNgay)
        {
            try
            {
                if (tuNgay > denNgay)
                    throw new Exception("Ngày bắt đầu phải nhỏ hơn ngày kết thúc!");

                return ThongKeDAL.Instance.ThongKeNhanVien(tuNgay, denNgay);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thống kê nhân viên: " + ex.Message);
            }
        }

        public string FormatTienVND(decimal amount)
        {
            return amount.ToString("#,##0") + " VNĐ";
        }
    }
}