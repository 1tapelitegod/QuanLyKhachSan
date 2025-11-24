using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BTL1.BUS;
using BTL1.DAL;
using BTL1.GUI.Forms;

namespace BTL1.GUI.UserControls
{
    public partial class UC_HoaDon : UserControl
    {
        private string maNVDangNhap;
        private string tenNVDangNhap;
        private DataTable dtChiTiet;
        private decimal tongTien = 0;

        public UC_HoaDon(string maNV, string tenNV)
        {
            InitializeComponent();
            this.maNVDangNhap = maNV ?? "";
            this.tenNVDangNhap = tenNV ?? "";
            InitializeChiTiet();
            InitializeEvents();
            this.Load += (s, e) => LoadInitialData();
        }

        public UC_HoaDon()
        {
            InitializeComponent();
            this.maNVDangNhap = "";
            this.tenNVDangNhap = "";
            InitializeChiTiet();
            InitializeEvents();
        }

        private void InitializeChiTiet()
        {
            try
            {
                dtChiTiet = new DataTable();
                dtChiTiet.Columns.Add("MaDV", typeof(string));
                dtChiTiet.Columns.Add("TenDichVu", typeof(string));
                dtChiTiet.Columns.Add("LoaiDichVu", typeof(string));
                dtChiTiet.Columns.Add("SoLuong", typeof(int));
                dtChiTiet.Columns.Add("DonGia", typeof(decimal));
                dtChiTiet.Columns.Add("GiamGia", typeof(decimal));
                dtChiTiet.Columns.Add("ThanhTien", typeof(decimal));
                
                dgvChiTiet.DataSource = dtChiTiet;
                dgvChiTiet.Refresh();
                
                FormatDataGridView();
                
                System.Diagnostics.Debug.WriteLine("✅ InitializeChiTiet thành công");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InitializeChiTiet Error: {ex.Message}");
            }
        }

        private void LoadInitialData()
        {
            try
            {
                if (!string.IsNullOrEmpty(maNVDangNhap))
                {
                    txtMaNV.Text = maNVDangNhap;
                    txtTenNV.Text = tenNVDangNhap;
                    txtMaNV.ReadOnly = true;
                    txtTenNV.ReadOnly = true;
                    txtMaNV.BackColor = Color.FromArgb(240, 240, 240);
                    txtTenNV.BackColor = Color.FromArgb(240, 240, 240);
                }
                else
                {
                    maNVDangNhap = "admin";
                    tenNVDangNhap = "Administrator";
                    txtMaNV.Text = maNVDangNhap;
                    txtTenNV.Text = tenNVDangNhap;
                }

                // ✅ SET READONLY CHO MÃ KH, TÊN KH, ĐỊA CHỈ NGAY TỪ ĐẦU
                txtMaKH.ReadOnly = true;
                txtTenKH.ReadOnly = true;
                txtDiaChi.ReadOnly = true;
                txtMaKH.BackColor = Color.FromArgb(240, 240, 240);
                txtTenKH.BackColor = Color.FromArgb(240, 240, 240);
                txtDiaChi.BackColor = Color.FromArgb(240, 240, 240);

                LoadMaHoaDon();
                LoadPhongVaDichVu();
                TaoHoaDonMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi load dữ liệu:\n\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadMaHoaDon()
        {
            try
            {
                DataTable dt = HoaDonBUS.Instance.GetAllMaHoaDon();
                
                if (dt == null || dt.Rows.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Không có hóa đơn nào");
                    return;
                }

                cboMaHD.Items.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    cboMaHD.Items.Add(row["MaHD"].ToString());
                }
                cboMaHD.SelectedIndex = -1;
                
                System.Diagnostics.Debug.WriteLine($"✅ Load {dt.Rows.Count} hóa đơn");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ LoadMaHoaDon Error: {ex.Message}");
            }
        }

        private void LoadPhongVaDichVu()
        {
            try
            {
                DataTable dtDichVu = DichVuBUS.Instance.GetAllDichVu();

                if (dtDichVu == null) dtDichVu = new DataTable();

                cboMaHang.Items.Clear();
                
                if (dtDichVu.Rows.Count > 0)
                {
                    foreach (DataRow row in dtDichVu.Rows)
                    {
                        string display = $"{row["MaDV"]} - {row["TenDV"]} - {Convert.ToDecimal(row["GiaDV"]):N0} VNĐ/{row["DonVi"]}";
                        cboMaHang.Items.Add("DICHVU|" + row["MaDV"].ToString() + "|" + display);
                    }
                }

                cboMaHang.SelectedIndex = -1;
                
                System.Diagnostics.Debug.WriteLine($"✅ Load {dtDichVu.Rows.Count} dịch vụ");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ LoadPhongVaDichVu Error: {ex.Message}");
            }
        }

        private void cboMaHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMaHang.SelectedIndex < 0 || cboMaHang.SelectedItem == null)
                    return;

                string selected = cboMaHang.SelectedItem.ToString();
                string[] parts = selected.Split('|');
                if (parts.Length < 2)
                    return;

                string loai = parts[0];
                string ma = parts[1];

                if (loai == "DICHVU")
                {
                    DataTable dt = DichVuBUS.Instance.GetDichVuByMa(ma);
                    if (dt.Rows.Count > 0)
                    {
                        txtTenHang.Text = dt.Rows[0]["TenDV"].ToString();
                        txtDonGia.Text = Convert.ToDecimal(dt.Rows[0]["GiaDV"]).ToString("N0");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ cboMaHang_SelectedIndexChanged Error: {ex.Message}");
            }
        }

        private void btnThemHang_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboMaHang.SelectedIndex < 0)
                {
                    MessageBox.Show("⚠️ Vui lòng chọn dịch vụ!", "Thông báo");
                    return;
                }

                if (string.IsNullOrEmpty(txtSoLuong.Text) || int.Parse(txtSoLuong.Text) <= 0)
                {
                    MessageBox.Show("⚠️ Vui lòng nhập số lượng hợp lệ!", "Thông báo");
                    return;
                }

                string selected = cboMaHang.SelectedItem.ToString();
                string[] parts = selected.Split('|');
                string maDV = parts[1];

                foreach (DataRow row in dtChiTiet.Rows)
                {
                    if (row["MaDV"].ToString() == maDV)
                    {
                        MessageBox.Show("⚠️ Dịch vụ này đã có trong hóa đơn!", "Thông báo");
                        return;
                    }
                }

                DataRow newRow = dtChiTiet.NewRow();
                newRow["MaDV"] = maDV;
                newRow["TenDichVu"] = txtTenHang.Text;
                newRow["SoLuong"] = int.Parse(txtSoLuong.Text);
                newRow["DonGia"] = decimal.Parse(txtDonGia.Text.Replace(",", ""));
                newRow["GiamGia"] = string.IsNullOrEmpty(txtGiamGia.Text) ? 0 : decimal.Parse(txtGiamGia.Text);
                newRow["ThanhTien"] = decimal.Parse(txtThanhTien.Text.Replace(",", "").Replace(" VNĐ", ""));
                newRow["LoaiDichVu"] = "DV";

                dtChiTiet.Rows.Add(newRow);
                
                dgvChiTiet.DataSource = null;
                dgvChiTiet.DataSource = dtChiTiet;
                FormatDataGridView();
                
                TinhTongTien();
                ClearHangHoa();
                
                MessageBox.Show("✅ Đã thêm vào hóa đơn!", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi: {ex.Message}", "Lỗi");
            }
        }

        private void TinhTongTien()
        {
            tongTien = 0;
            foreach (DataRow row in dtChiTiet.Rows)
            {
                tongTien += Convert.ToDecimal(row["ThanhTien"]);
            }
            txtTongTien.Text = tongTien.ToString("#,##0 VNĐ");
        }

        private void TinhThanhTien()
        {
            try
            {
                if (string.IsNullOrEmpty(txtDonGia.Text) || string.IsNullOrEmpty(txtSoLuong.Text))
                    return;

                decimal donGia = decimal.Parse(txtDonGia.Text.Replace(",", ""));
                int soLuong = int.Parse(txtSoLuong.Text);
                decimal giamGia = string.IsNullOrEmpty(txtGiamGia.Text) ? 0 : decimal.Parse(txtGiamGia.Text);

                decimal thanhTien = HoaDonBUS.Instance.CalculateThanhTien(donGia, soLuong, giamGia);
                txtThanhTien.Text = thanhTien.ToString("#,##0 VNĐ");
            }
            catch { }
        }

        private void txtSoLuong_TextChanged(object sender, EventArgs e)
        {
            TinhThanhTien();
        }

        private void txtGiamGia_TextChanged(object sender, EventArgs e)
        {
            TinhThanhTien();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string maHD = cboMaHD.Text.Trim();

                if (string.IsNullOrEmpty(maHD))
                {
                    MessageBox.Show("⚠️ Vui lòng chọn hoặc nhập mã hóa đơn!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dtHD = HoaDonBUS.Instance.GetHoaDonByMa(maHD);

                if (dtHD.Rows.Count == 0)
                {
                    MessageBox.Show("🔍 Không tìm thấy hóa đơn!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataRow row = dtHD.Rows[0];
                txtMaHD.Text = row["MaHD"].ToString();
                txtMaKH.Text = row["MaKH"].ToString();
                
                DataTable dtKH = KhachHangBUS.Instance.GetKhachHangByMa(row["MaKH"].ToString());
                if (dtKH.Rows.Count > 0)
                {
                    txtTenKH.Text = dtKH.Rows[0]["TenKH"].ToString();
                    txtDiaChi.Text = dtKH.Rows[0]["DiaChi"].ToString();
                    txtSDT.Text = dtKH.Rows[0]["SoDienThoai"].ToString();
                }

                dtpNgayLap.Value = Convert.ToDateTime(row["NgayLap"]);
                txtGhiChu.Text = row["GhiChu"].ToString();

                DataTable dtCT = HoaDonBUS.Instance.GetChiTietHoaDon(maHD);
                
                dtChiTiet.Clear();
                
                foreach (DataRow r in dtCT.Rows)
                {
                    DataRow newRow = dtChiTiet.NewRow();
                    
                    newRow["MaDV"] = r["MaDV"];
                    newRow["TenDichVu"] = r.Table.Columns.Contains("TenHang") ? r["TenHang"] : r["TenDichVu"];
                    newRow["LoaiDichVu"] = r.Table.Columns.Contains("LoaiHang") ? r["LoaiHang"] : r["LoaiDichVu"];
                    newRow["SoLuong"] = r["SoLuong"];
                    newRow["DonGia"] = r["DonGia"];
                    newRow["GiamGia"] = r["GiamGia"];
                    newRow["ThanhTien"] = r["ThanhTien"];
                    
                    dtChiTiet.Rows.Add(newRow);
                }
                
                dgvChiTiet.DataSource = null;
                dgvChiTiet.DataSource = dtChiTiet;
                FormatDataGridView();

                tongTien = Convert.ToDecimal(row["TongTien"]);
                txtTongTien.Text = tongTien.ToString("#,##0 VNĐ");

                MessageBox.Show("✅ Tìm thấy hóa đơn!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi tìm kiếm: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSDT_Leave(object sender, EventArgs e)
        {
            try
            {
                string sdt = txtSDT.Text.Trim();

                if (string.IsNullOrEmpty(sdt))
                {
                    ClearCustomerInfo();
                    return;
                }

                if (sdt.Length < 10 || sdt.Length > 11)
                {
                    MessageBox.Show("⚠️ Số điện thoại phải có 10-11 số!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSDT.Clear();
                    txtSDT.Focus();
                    ClearCustomerInfo();
                    return;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(sdt, @"^\d+$"))
                {
                    MessageBox.Show("⚠️ Số điện thoại chỉ được chứa chữ số!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSDT.Clear();
                    txtSDT.Focus();
                    ClearCustomerInfo();
                    return;
                }

                DataTable dtKH = KhachHangBUS.Instance.GetKhachHangBySDT(sdt);

                if (dtKH.Rows.Count > 0)
                {
                    DataRow row = dtKH.Rows[0];
                    
                    txtMaKH.Text = row["MaKH"].ToString();
                    txtTenKH.Text = row["TenKH"].ToString();
                    txtDiaChi.Text = row["DiaChi"].ToString();

                    LoadDichVuChuaThanhToan(row["MaKH"].ToString());
                }
                else
                {
                    DialogResult dr = MessageBox.Show(
                        $"🆕 Số điện thoại '{sdt}' chưa có trong hệ thống!\n\n" +
                        "Bạn có muốn thêm khách hàng mới không?", 
                        "Khách hàng mới", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        using (var frmThem = new Forms.frmThemKhachHang(sdt))
                        {
                            if (frmThem.ShowDialog() == DialogResult.OK)
                            {
                                txtMaKH.Text = frmThem.MaKH;
                                txtTenKH.Text = frmThem.TenKH;
                                txtDiaChi.Text = frmThem.DiaChi;
                                
                                MessageBox.Show(
                                    $"✅ Đã thêm khách hàng thành công!\n\n" +
                                    $"Mã KH: {txtMaKH.Text}\n" +
                                    $"Tên: {txtTenKH.Text}", 
                                    "Thành công", 
                                    MessageBoxButtons.OK, 
                                    MessageBoxIcon.Information);
                            }
                            else
                            {
                                txtSDT.Clear();
                                ClearCustomerInfo();
                                txtSDT.Focus();
                            }
                        }
                    }
                    else
                    {
                        txtSDT.Clear();
                        ClearCustomerInfo();
                        txtSDT.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi kiểm tra số điện thoại:\n\n{ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearCustomerInfo();
            }
        }

        private void ClearCustomerInfo()
        {
            txtMaKH.Clear();
            txtTenKH.Clear();
            txtDiaChi.Clear();
        }

        private void btnThemHoaDon_Click(object sender, EventArgs e)
        {
            TaoHoaDonMoi();
        }

        private void TaoHoaDonMoi()
        {
            try
            {
                ClearForm();

                string ngayHienTai = DateTime.Now.ToString("ddMMyyyy");
                string maHD = HoaDonBUS.Instance.GenerateMaHoaDon(ngayHienTai);
                
                txtMaHD.Text = maHD;
                dtpNgayLap.Value = DateTime.Now;

                dtChiTiet.Clear();
                tongTien = 0;
                txtTongTien.Text = "0 VNĐ";

                MessageBox.Show($"✅ Tạo hóa đơn mới: {maHD}", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi: {ex.Message}", "Lỗi");
            }
        }

        private void btnLuuHoaDon_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtSDT.Text))
                {
                    MessageBox.Show("⚠️ Vui lòng nhập số điện thoại khách hàng!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSDT.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtMaKH.Text))
                {
                    MessageBox.Show("⚠️ Vui lòng thêm thông tin khách hàng trước khi lưu hóa đơn!", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSDT.Focus();
                    return;
                }

                if (dtChiTiet.Rows.Count == 0)
                {
                    MessageBox.Show("⚠️ Chưa có dịch vụ nào trong hóa đơn!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (HoaDonDAL.Instance.CheckMaHDExists(txtMaHD.Text))
                {
                    string ngayHienTai = DateTime.Now.ToString("ddMMyyyy");
                    string maHDMoi = HoaDonBUS.Instance.GenerateMaHoaDon(ngayHienTai);
                    txtMaHD.Text = maHDMoi;
                }

                DialogResult confirm = MessageBox.Show(
                    $"💾 Xác nhận lưu hóa đơn:\n\n" +
                    $"📄 Mã HĐ: {txtMaHD.Text}\n" +
                    $"👤 Khách hàng: {txtTenKH.Text} ({txtMaKH.Text})\n" +
                    $"📞 SĐT: {txtSDT.Text}\n" +
                    $"💰 Tổng tiền: {txtTongTien.Text}\n" +
                    $"📦 Số dịch vụ: {dtChiTiet.Rows.Count}\n\n" +
                    "Bạn có chắc chắn muốn lưu?", 
                    "Xác nhận", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                bool result = HoaDonBUS.Instance.SaveHoaDon(
                    txtMaHD.Text,
                    txtMaKH.Text,
                    maNVDangNhap,
                    dtpNgayLap.Value,
                    tongTien,
                    txtGhiChu.Text.Trim(),
                    dtChiTiet
                );

                if (result)
                {
                    MessageBox.Show(
                        $"✅ Lưu hóa đơn thành công!\n\n" +
                        $"📄 Mã HĐ: {txtMaHD.Text}\n" +
                        $"💰 Tổng tiền: {txtTongTien.Text}", 
                        "Thành công", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Information);

                    LoadMaHoaDon();
                    TaoHoaDonMoi();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi lưu hóa đơn:\n\n{ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvChiTiet_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.RowIndex < dtChiTiet.Rows.Count)
                {
                    string ten = dtChiTiet.Rows[e.RowIndex]["TenDichVu"].ToString();
                    
                    DialogResult dr = MessageBox.Show($"❓ Xóa '{ten}'?", "Xác nhận", 
                        MessageBoxButtons.YesNo);

                    if (dr == DialogResult.Yes)
                    {
                        dtChiTiet.Rows.RemoveAt(e.RowIndex);
                        
                        dgvChiTiet.DataSource = null;
                        dgvChiTiet.DataSource = dtChiTiet;
                        FormatDataGridView();
                        
                        TinhTongTien();
                        MessageBox.Show("✅ Đã xóa!", "Thông báo");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi: {ex.Message}", "Lỗi");
            }
        }

        private void btnHuyHoaDon_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaHD.Text))
                {
                    MessageBox.Show("⚠️ Vui lòng chọn hóa đơn cần hủy!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult dr = MessageBox.Show(
                    $"❓ Bạn có chắc muốn hủy hóa đơn:\n\n" +
                    $"📄 Mã HĐ: {txtMaHD.Text}\n" +
                    $"👤 Khách hàng: {txtTenKH.Text}\n" +
                    $"💰 Tổng tiền: {txtTongTien.Text}\n\n" +
                    $"⚠️ Số lượng hàng sẽ được hoàn lại!", 
                    "Xác nhận hủy", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    bool result = HoaDonBUS.Instance.CancelHoaDon(txtMaHD.Text);

                    if (result)
                    {
                        MessageBox.Show("✅ Hủy hóa đơn thành công!", "Thông báo", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadMaHoaDon();
                        TaoHoaDonMoi();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi hủy hóa đơn:\n{ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaHD.Text))
                {
                    MessageBox.Show("⚠️ Vui lòng chọn hóa đơn cần in!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                HoaDonBUS.Instance.ExportToExcel(txtMaHD.Text, dtChiTiet, txtTenKH.Text, txtSDT.Text, tongTien);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xuất Excel:\n{ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtMaHD.Clear();
            txtMaKH.Clear();
            txtTenKH.Clear();
            txtDiaChi.Clear();
            txtSDT.Clear();
            txtGhiChu.Clear();
            txtTongTien.Text = "0 VNĐ";
            dtChiTiet.Clear();
            ClearHangHoa();
        }

        private void ClearHangHoa()
        {
            cboMaHang.SelectedIndex = -1;
            txtTenHang.Clear();
            txtDonGia.Clear();
            txtSoLuong.Text = "0";
            txtGiamGia.Text = "0";
            txtThanhTien.Clear();
        }

        private void FormatDataGridView()
        {
            if (dgvChiTiet == null || dgvChiTiet.Columns.Count == 0)
                return;
            
            try
            {
                if (dgvChiTiet.Columns.Contains("MaDV"))
                {
                    dgvChiTiet.Columns["MaDV"].HeaderText = "Mã";
                    dgvChiTiet.Columns["MaDV"].Width = 80;
                }

                if (dgvChiTiet.Columns.Contains("TenDichVu"))
                {
                    dgvChiTiet.Columns["TenDichVu"].HeaderText = "Tên dịch vụ";
                    dgvChiTiet.Columns["TenDichVu"].Width = 300;
                }
                
                if (dgvChiTiet.Columns.Contains("LoaiDichVu"))
                {
                    dgvChiTiet.Columns["LoaiDichVu"].Visible = false;
                }
                
                if (dgvChiTiet.Columns.Contains("SoLuong"))
                {
                    dgvChiTiet.Columns["SoLuong"].HeaderText = "Số lượng";
                    dgvChiTiet.Columns["SoLuong"].Width = 80;
                }
                
                if (dgvChiTiet.Columns.Contains("DonGia"))
                {
                    dgvChiTiet.Columns["DonGia"].HeaderText = "Đơn giá";
                    dgvChiTiet.Columns["DonGia"].DefaultCellStyle.Format = "#,##0";
                    dgvChiTiet.Columns["DonGia"].Width = 120;
                }
                
                if (dgvChiTiet.Columns.Contains("GiamGia"))
                {
                    dgvChiTiet.Columns["GiamGia"].HeaderText = "Giảm giá (%)";
                    dgvChiTiet.Columns["GiamGia"].Width = 80;
                }
                
                if (dgvChiTiet.Columns.Contains("ThanhTien"))
                {
                    dgvChiTiet.Columns["ThanhTien"].HeaderText = "Thành tiền";
                    dgvChiTiet.Columns["ThanhTien"].DefaultCellStyle.Format = "#,##0";
                    dgvChiTiet.Columns["ThanhTien"].DefaultCellStyle.ForeColor = Color.FromArgb(76, 175, 80);
                    dgvChiTiet.Columns["ThanhTien"].DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    dgvChiTiet.Columns["ThanhTien"].Width = 150;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ FormatDataGridView Error: {ex.Message}");
            }
        }

        private void InitializeEvents()
        {
            // ✅ CHỈ CHO PHÉP NHẬP SỐ VÀO SĐT
            txtSDT.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            };
            
            txtSDT.TextChanged += (s, e) =>
            {
                if (!string.IsNullOrEmpty(txtMaKH.Text) && txtSDT.Focused)
                {
                    ClearCustomerInfo();
                }
            };
            
            // ✅ NGĂN NHẬP VÀO MÃ KH, TÊN KH, ĐỊA CHỈ
            txtMaKH.KeyDown += PreventEditWhenLocked;
            txtTenKH.KeyDown += PreventEditWhenLocked;
            txtDiaChi.KeyDown += PreventEditWhenLocked;
        }

        private void PreventEditWhenLocked(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt != null && txt.ReadOnly)
            {
                if (e.KeyCode != Keys.Tab && 
                    e.KeyCode != Keys.Left && 
                    e.KeyCode != Keys.Right &&
                    e.KeyCode != Keys.Up &&
                    e.KeyCode != Keys.Down)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void LoadDichVuChuaThanhToan(string maKH)
        {
            try
            {
                DataTable dtDichVu = HoaDonBUS.Instance.GetDichVuChuaThanhToanByKH(maKH);

                if (dtDichVu == null || dtDichVu.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "ℹ️ Khách hàng không có dịch vụ hoặc phòng chưa thanh toán.", 
                        "Thông báo", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Information);
                    return;
                }

                dtChiTiet.Rows.Clear();

                foreach (DataRow row in dtDichVu.Rows)
                {
                    DataRow newRow = dtChiTiet.NewRow();
                    newRow["MaDV"] = row["MaDV"];
                    newRow["TenDichVu"] = row["TenDichVu"];
                    newRow["LoaiDichVu"] = row["LoaiDichVu"];
                    newRow["SoLuong"] = row["SoLuong"];
                    newRow["DonGia"] = row["DonGia"];
                    newRow["GiamGia"] = row["GiamGia"];
                    newRow["ThanhTien"] = row["ThanhTien"];

                    dtChiTiet.Rows.Add(newRow);
                }

                TinhTongTien();

                MessageBox.Show(
                    $"✅ Đã tải {dtDichVu.Rows.Count} dịch vụ/phòng chưa thanh toán!\n\n" +
                    $"💰 Tổng tiền: {txtTongTien.Text}", 
                    "Thông báo", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi tải dịch vụ:\n\n{ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
