using System;
using System.Data;
using System.Text.RegularExpressions;
using BTL1.DAL;

namespace BTL1.BUS
{
    public class DichVuBUS
    {
        private static DichVuBUS instance;

        public static DichVuBUS Instance
        {
            get
            {
                if (instance == null)
                    instance = new DichVuBUS();
                return instance;
            }
        }

        private DichVuBUS() { }

        public DataTable GetAllDichVu()
        {
            try
            {
                return DichVuDAL.Instance.GetAllDichVu();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách dịch vụ: " + ex.Message);
            }
        }

        public DataTable SearchDichVu(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return DichVuDAL.Instance.GetAllDichVu();

                return DichVuDAL.Instance.SearchDichVu(keyword);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        public bool InsertDichVu(string tenDV, decimal giaDV, string donVi, string moTa)
        {
            try
            {
                ValidateDichVu(tenDV, giaDV, donVi);

                if (DichVuDAL.Instance.CheckDichVuExists(tenDV))
                    throw new Exception($"Dịch vụ '{tenDV}' đã tồn tại!");

                return DichVuDAL.Instance.InsertDichVu(tenDV, giaDV, donVi, moTa);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thêm dịch vụ: " + ex.Message);
            }
        }

        public bool UpdateDichVu(string maDV, string tenDV, decimal giaDV, string donVi, string moTa)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maDV))
                    throw new Exception("Mã dịch vụ không hợp lệ!");

                ValidateDichVu(tenDV, giaDV, donVi);

                return DichVuDAL.Instance.UpdateDichVu(maDV, tenDV, giaDV, donVi, moTa);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật dịch vụ: " + ex.Message);
            }
        }

        public bool DeleteDichVu(string maDV)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maDV))
                    throw new Exception("Mã dịch vụ không hợp lệ!");

                return DichVuDAL.Instance.DeleteDichVu(maDV);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa dịch vụ: " + ex.Message);
            }
        }

        public DataTable GetDichVuByMa(string maDV)
        {
            try
            {
                return DichVuDAL.Instance.GetDichVuByMa(maDV);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy thông tin dịch vụ: " + ex.Message);
            }
        }

        private void ValidateDichVu(string tenDV, decimal giaDV, string donVi)
        {
            if (string.IsNullOrWhiteSpace(tenDV))
                throw new Exception("Tên dịch vụ không được để trống!");

            if (tenDV.Length > 200)
                throw new Exception("Tên dịch vụ không được quá 200 ký tự!");
            
            if (giaDV <= 0)
                throw new Exception("Giá dịch vụ phải lớn hơn 0!");

            if (giaDV > 999999999)
                throw new Exception("Giá dịch vụ không được vượt quá 999,999,999 VNĐ!");

            if (string.IsNullOrWhiteSpace(donVi))
                throw new Exception("Đơn vị không được để trống!");

            if (donVi.Length > 50)
                throw new Exception("Đơn vị không được quá 50 ký tự!");
        }

        public string FormatTienVND(decimal amount)
        {
            return amount.ToString("#,##0") + " VNĐ";
        }

        public bool ValidateGiaDV(string giaDVText, out decimal giaDV)
        {
            string cleanText = Regex.Replace(giaDVText, @"[^\d,.]", "");
            return decimal.TryParse(cleanText, out giaDV);
        }

        public DataTable SearchDichVuByMa(string maDV)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maDV))
                    return GetAllDichVu();

                return DichVuDAL.Instance.SearchDichVuByMa(maDV);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm dịch vụ: " + ex.Message);
            }
        }
    }
}