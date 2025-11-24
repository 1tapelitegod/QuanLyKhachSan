using System;
using System.Data;
using BTL1.DAL;
using System.Windows.Forms;

namespace BTL1.BUS
{
    public class HoaDonBUS
    {
        private static HoaDonBUS instance;

        public static HoaDonBUS Instance
        {
            get
            {
                if (instance == null)
                    instance = new HoaDonBUS();
                return instance;
            }
        }

        private HoaDonBUS() { }

        // YC2: Lấy tất cả mã hóa đơn
        public DataTable GetAllMaHoaDon()
        {
            return HoaDonDAL.Instance.GetAllMaHoaDon();
        }

        // YC3: Lấy hóa đơn theo mã
        public DataTable GetHoaDonByMa(string maHD)
        {
            return HoaDonDAL.Instance.GetHoaDonByMa(maHD);
        }

        // YC3: Lấy chi tiết hóa đơn
        public DataTable GetChiTietHoaDon(string maHD)
        {
            return HoaDonDAL.Instance.GetChiTietHoaDon(maHD);
        }

        // YC6: Tạo mã hóa đơn tự động (HĐB_ddMMyyyy0xxx)
        public string GenerateMaHoaDon(string ngayHienTai)
        {
            try
            {
                string maHD = "";
                int attempt = 0;
                
                do
                {
                    maHD = HoaDonDAL.Instance.GenerateMaHoaDon();
                    attempt++;
                    
                    if (attempt > 100)
                    {
                        throw new Exception("Không thể tạo mã hóa đơn sau 100 lần thử!");
                    }
                    
                    if (HoaDonDAL.Instance.CheckMaHDExists(maHD))
                    {
                        System.Threading.Thread.Sleep(50);
                    }
                    
                } while (HoaDonDAL.Instance.CheckMaHDExists(maHD));
                
                return maHD;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tạo mã hóa đơn: " + ex.Message);
            }
        }

        // YC7: Tính thành tiền
        public decimal CalculateThanhTien(decimal donGia, int soLuong, decimal giamGia)
        {
            decimal tienHang = donGia * soLuong;
            decimal tienGiam = tienHang * giamGia / 100;
            return tienHang - tienGiam;
        }

        // YC8: Lưu hóa đơn và chi tiết
        public bool SaveHoaDon(string maHD, string maKH, string maNV, DateTime ngayLap, 
            decimal tongTien, string ghiChu, DataTable dtChiTiet)
        {
            try
            {
                // Lưu hóa đơn
                bool resultHD = HoaDonDAL.Instance.InsertHoaDon(maHD, maKH, maNV, ngayLap, tongTien, ghiChu);
                
                if (!resultHD)
                    throw new Exception("Lỗi lưu hóa đơn!");

                // Lưu chi tiết
                foreach (DataRow row in dtChiTiet.Rows)
                {
                    string maDV = row["MaDV"].ToString();
                    string loaiDV = row["LoaiDichVu"].ToString();
                    int soLuong = Convert.ToInt32(row["SoLuong"]);
                    decimal donGia = Convert.ToDecimal(row["DonGia"]);
                    decimal giamGia = Convert.ToDecimal(row["GiamGia"]);
                    decimal thanhTien = Convert.ToDecimal(row["ThanhTien"]);

                    // Lưu chi tiết hóa đơn
                    bool resultCT = HoaDonDAL.Instance.InsertChiTietHoaDon(
                        maHD, maDV, soLuong, donGia, giamGia, thanhTien);

                    if (!resultCT)
                        throw new Exception("Lỗi lưu chi tiết hóa đơn!");

                    // ✅ CẬP NHẬT TRẠNG THÁI
                    if (loaiDV == "DV")
                    {
                        // Cập nhật trạng thái SuDungDichVu thành "Đã thanh toán"
                        SuDungDichVuDAL.Instance.UpdateTrangThaiByMaDV(maDV, maKH, "Đã thanh toán");
                    }
                    else if (loaiDV == "PHONG")
                    {
                        // Cập nhật trạng thái DatPhong thành "Đã thanh toán"
                        DatPhongDAL.Instance.UpdateTrangThaiByMaPhong(maDV, maKH, "Đã thanh toán");
                        
                        // Cập nhật trạng thái phòng thành "Trống"
                        PhongDAL.Instance.UpdateTrangThaiPhong(maDV, "Trống");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lưu hóa đơn: {ex.Message}");
            }
        }

        // YC10: Hủy hóa đơn và hoàn lại số lượng
        public bool CancelHoaDon(string maHD)
        {
            try
            {
                // Lấy chi tiết để hoàn lại số lượng
                DataTable dtChiTiet = GetChiTietHoaDon(maHD);

                foreach (DataRow row in dtChiTiet.Rows)
                {
                    string maDV = row["MaDV"].ToString();
                    int soLuong = Convert.ToInt32(row["SoLuong"]);

                    // Hoàn lại số lượng (chỉ với dịch vụ)
                    DichVuDAL.Instance.UpdateSoLuongTon(maDV, soLuong);
                }

                // Xóa chi tiết
                HoaDonDAL.Instance.DeleteChiTietHoaDon(maHD);

                // Xóa hóa đơn
                bool result = HoaDonDAL.Instance.DeleteHoaDon(maHD);
                
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi hủy hóa đơn: {ex.Message}");
            }
        }

        // YC11: Xuất Excel
        public void ExportToExcel(string maHD, DataTable dtChiTiet, string tenKH, string sdt, decimal tongTien)
        {
            try
            {
                // ✅ HIỂN THỊ DIALOG CHỌN FILE TRƯỚC
                string fileName = $"HoaDon_{maHD}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = fileName;
                sfd.Filter = "Excel Files|*.xlsx";
                sfd.Title = "💾 Lưu hóa đơn Excel";
                sfd.DefaultExt = "xlsx";

                // ✅ Nếu user Cancel thì return NGAY (không hiện MessageBox)
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return; // ✅ Thoát im lặng
                }

                // ✅ TẠO FILE EXCEL SAU KHI ĐÃ CHỌN ĐƯỜNG DẪN
                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("HoaDon");

                    // Header
                    ws.Cells["A1"].Value = "HÓA ĐƠN KHÁCH SẠN";
                    ws.Cells["A1:H1"].Merge = true;
                    ws.Cells["A1"].Style.Font.Size = 18;
                    ws.Cells["A1"].Style.Font.Bold = true;
                    ws.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    // Thông tin
                    ws.Cells["A3"].Value = "Mã HĐ:";
                    ws.Cells["B3"].Value = maHD;
                    ws.Cells["A4"].Value = "Khách hàng:";
                    ws.Cells["B4"].Value = tenKH;
                    ws.Cells["A5"].Value = "SĐT:";
                    ws.Cells["B5"].Value = sdt;
                    ws.Cells["A6"].Value = "Ngày:";
                    ws.Cells["B6"].Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                    // Chi tiết
                    ws.Cells["A8"].Value = "STT";
                    ws.Cells["B8"].Value = "Mã";
                    ws.Cells["C8"].Value = "Tên dịch vụ";
                    ws.Cells["D8"].Value = "Loại";
                    ws.Cells["E8"].Value = "SL";
                    ws.Cells["F8"].Value = "Đơn giá";
                    ws.Cells["G8"].Value = "Giảm giá %";
                    ws.Cells["H8"].Value = "Thành tiền";

                    int row = 9;
                    int stt = 1;
                    foreach (DataRow dr in dtChiTiet.Rows)
                    {
                        ws.Cells[$"A{row}"].Value = stt++;
                        ws.Cells[$"B{row}"].Value = dr["MaDV"];
                        ws.Cells[$"C{row}"].Value = dr["TenDichVu"];
                        ws.Cells[$"D{row}"].Value = dr["LoaiDichVu"];
                        ws.Cells[$"E{row}"].Value = dr["SoLuong"];
                        ws.Cells[$"F{row}"].Value = dr["DonGia"];
                        ws.Cells[$"G{row}"].Value = dr["GiamGia"];
                        ws.Cells[$"H{row}"].Value = dr["ThanhTien"];
                        row++;
                    }

                    ws.Cells[$"G{row}"].Value = "TỔNG TIỀN:";
                    ws.Cells[$"H{row}"].Value = tongTien;
                    ws.Cells[$"H{row}"].Style.Font.Bold = true;

                    // ✅ LƯU FILE VÀO ĐƯỜNG DẪN ĐÃ CHỌN
                    System.IO.File.WriteAllBytes(sfd.FileName, package.GetAsByteArray());
                }

                // ✅ CHỈ HIỂN THỊ MESSAGEBOX KHI LƯU THÀNH CÔNG
                MessageBox.Show(
                    $"✅ Xuất Excel thành công!\n\nĐã lưu tại:\n{sfd.FileName}", 
                    "Thành công", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // ✅ Chỉ throw exception khi có lỗi thực sự
                throw new Exception($"Lỗi xuất Excel: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách dịch vụ & phòng chưa thanh toán của khách hàng
        /// ✅ CẬP NHẬT: Tính toán lại thành tiền phòng sau khi trừ tiền cọc
        /// </summary>
        public DataTable GetDichVuChuaThanhToanByKH(string maKH)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maKH))
                    throw new Exception("Mã khách hàng không hợp lệ!");

                DataTable dt = HoaDonDAL.Instance.GetDichVuChuaThanhToanByKH(maKH);

                // ✅ XỬ LÝ TÍNH LẠI THÀNH TIỀN CHO PHÒNG (TRỪ TIỀN CỌC)
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string loaiDV = row["LoaiDichVu"].ToString();
                        
                        // Chỉ xử lý cho PHÒNG
                        if (loaiDV == "PHONG")
                        {
                            decimal donGia = Convert.ToDecimal(row["DonGia"]);
                            int soLuong = Convert.ToInt32(row["SoLuong"]); // Số ngày
                            decimal tienCoc = row.Table.Columns.Contains("TienCoc") && row["TienCoc"] != DBNull.Value 
                                ? Convert.ToDecimal(row["TienCoc"]) 
                                : 0;

                            // Tính lại thành tiền = (Đơn giá × Số ngày) - Tiền cọc
                            decimal thanhTienGoc = donGia * soLuong;
                            decimal thanhTienSauCoc = thanhTienGoc - tienCoc;
                            
                            // Đảm bảo không âm
                            if (thanhTienSauCoc < 0) thanhTienSauCoc = 0;

                            // Cập nhật lại ThanhTien trong DataTable
                            row["ThanhTien"] = thanhTienSauCoc;
                        }
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy dịch vụ chưa thanh toán: " + ex.Message);
            }
        }

        /// <summary>
        /// ✅ PHƯƠNG THỨC MỚI: Tính thành tiền cho phòng (có xử lý tiền cọc)
        /// </summary>
        /// <param name="donGia">Giá phòng/đêm</param>
        /// <param name="soNgay">Số đêm</param>
        /// <param name="tienCoc">Tiền đặt cọc</param>
        /// <returns>Thành tiền sau khi trừ cọc</returns>
        public decimal TinhThanhTienPhong(decimal donGia, int soNgay, decimal tienCoc)
        {
            if (soNgay <= 0) soNgay = 1;
            
            decimal tongTien = donGia * soNgay;
            decimal thanhTien = tongTien - tienCoc;
            
            return thanhTien < 0 ? 0 : thanhTien;
        }
    }
}