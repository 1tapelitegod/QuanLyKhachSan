using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BTL1.BUS;

namespace BTL1.GUI.UserControls
{
    public partial class UC_DatPhong : UserControl
    {
        private string maNVDangNhap;
        private string tenNVDangNhap;
        private string currentMaDatPhong = "";

        public UC_DatPhong()
        {
            InitializeComponent();
            this.maNVDangNhap = "admin";
            this.tenNVDangNhap = "Administrator";
            this.Load += (s, e) => LoadInitialData();
        }

        public UC_DatPhong(string maNV, string tenNV)
        {
            InitializeComponent();
            this.maNVDangNhap = maNV ?? "admin";
            this.tenNVDangNhap = tenNV ?? "Administrator";
            this.Load += (s, e) => LoadInitialData();
        }

        private void LoadInitialData()
        {
            try
            {
                // Set format ngày DD/MM/YYYY cho DateTimePicker
                dtpNgayNhanPhong.Format = DateTimePickerFormat.Custom;
                dtpNgayNhanPhong.CustomFormat = "dd/MM/yyyy";
                dtpNgayTraPhong.Format = DateTimePickerFormat.Custom;
                dtpNgayTraPhong.CustomFormat = "dd/MM/yyyy";
                
                // Set ngày mặc định
                dtpNgayNhanPhong.Value = DateTime.Now.Date;
                dtpNgayTraPhong.Value = DateTime.Now.Date.AddDays(1);
                
                // Gán event handlers
                InitializeEvents();
                
                // Load dữ liệu
                LoadKhachHang();
                LoadPhong();
                LoadData();
                
                // Tính tổng tiền ban đầu
                TinhTongTien();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi load dữ liệu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeEvents()
        {
            // Button events
            btnDatPhong.Click += btnDatPhong_Click;
            btnCapNhat.Click += btnCapNhat_Click;
            btnLamMoi.Click += btnLamMoi_Click;
            
            // ComboBox events
            cboMaKH.SelectedIndexChanged += cboMaKH_SelectedIndexChanged;
            cboPhong.SelectedIndexChanged += cboPhong_SelectedIndexChanged;
            
            // DateTimePicker events
            dtpNgayNhanPhong.ValueChanged += (s, e) => TinhTongTien();
            dtpNgayTraPhong.ValueChanged += (s, e) => TinhTongTien();

            // TextBox validation
            txtSoNguoi.KeyPress += (s, e) => {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };

            txtTienCoc.KeyPress += (s, e) => {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
                    e.Handled = true;
            };

            txtTienCoc.Leave += (s, e) => {
                if (decimal.TryParse(txtTienCoc.Text.Replace(",", ""), out decimal tien))
                    txtTienCoc.Text = tien.ToString("#,##0");
            };

            txtTienCoc.Enter += (s, e) => {
                txtTienCoc.Text = txtTienCoc.Text.Replace(",", "");
                txtTienCoc.SelectAll();
            };

            // DataGridView event
            dgvDatPhong.CellClick += dgvDatPhong_CellClick;
        }

        private void LoadKhachHang()
        {
            try
            {
                DataTable dt = KhachHangBUS.Instance.GetAllKhachHang();
                
                cboMaKH.DisplayMember = "MaKH";
                cboMaKH.ValueMember = "MaKH";
                cboMaKH.DataSource = dt;
                
                if (cboMaKH.Items.Count > 0)
                    cboMaKH.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi load khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPhong()
        {
            try
            {
                DataTable dt = PhongBUS.Instance.GetPhongTrong();
                
                cboPhong.DisplayMember = "TenPhong";
                cboPhong.ValueMember = "MaPhong";
                cboPhong.DataSource = dt;
                
                if (cboPhong.Items.Count > 0)
                    cboPhong.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi load phòng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboMaKH_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMaKH.SelectedIndex < 0) return;

                DataRowView row = (DataRowView)cboMaKH.SelectedItem;
                if (row != null)
                {
                    txtTenKH.Text = row["TenKH"]?.ToString() ?? "";
                    txtSDT.Text = row["SoDienThoai"]?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ cboMaKH_SelectedIndexChanged Error: {ex.Message}");
            }
        }

        private void cboPhong_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPhong.SelectedIndex < 0) return;

                DataRowView row = (DataRowView)cboPhong.SelectedItem;
                if (row != null)
                {
                    txtLoaiPhong.Text = row["LoaiPhong"]?.ToString() ?? "";
                    
                    if (decimal.TryParse(row["GiaPhong"]?.ToString(), out decimal giaPhong))
                    {
                        txtGiaPhong.Text = giaPhong.ToString("#,##0 VNĐ");
                        TinhTongTien();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ cboPhong_SelectedIndexChanged Error: {ex.Message}");
            }
        }

        private void LoadData()
        {
            try
            {
                DataTable dt = DatPhongBUS.Instance.GetAllDatPhong();
                dgvDatPhong.DataSource = dt;
                FormatDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi load dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            if (dgvDatPhong.Columns.Count == 0) return;

            try
            {
                // Tắt auto size để tự set độ rộng
                dgvDatPhong.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                
                dgvDatPhong.EnableHeadersVisualStyles = false;
                dgvDatPhong.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(3, 169, 244);
                dgvDatPhong.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvDatPhong.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                dgvDatPhong.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvDatPhong.ColumnHeadersHeight = 40;
                dgvDatPhong.RowTemplate.Height = 35;
                dgvDatPhong.DefaultCellStyle.SelectionBackColor = Color.FromArgb(187, 222, 251);
                dgvDatPhong.DefaultCellStyle.SelectionForeColor = Color.Black;
                dgvDatPhong.DefaultCellStyle.Font = new Font("Segoe UI", 9F);

                // Ẩn các cột không cần thiết trước
                string[] hiddenColumns = { "MaKH", "MaPhong", "MaNV", "NgayDat", "GhiChu", "TenNV", "GiaPhong", "TrangThai" };
                foreach (string col in hiddenColumns)
                {
                    if (dgvDatPhong.Columns.Contains(col))
                        dgvDatPhong.Columns[col].Visible = false;
                }

                // Format cột với độ rộng cân đối
                if (dgvDatPhong.Columns.Contains("MaDatPhong"))
                {
                    dgvDatPhong.Columns["MaDatPhong"].HeaderText = "Mã đặt phòng";
                    dgvDatPhong.Columns["MaDatPhong"].Width = 140;
                    dgvDatPhong.Columns["MaDatPhong"].DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    dgvDatPhong.Columns["MaDatPhong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (dgvDatPhong.Columns.Contains("TenKH"))
                {
                    dgvDatPhong.Columns["TenKH"].HeaderText = "Khách hàng";
                    dgvDatPhong.Columns["TenKH"].Width = 160;
                }

                if (dgvDatPhong.Columns.Contains("SoDienThoai"))
                {
                    dgvDatPhong.Columns["SoDienThoai"].HeaderText = "SĐT";
                    dgvDatPhong.Columns["SoDienThoai"].Width = 100;
                    dgvDatPhong.Columns["SoDienThoai"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (dgvDatPhong.Columns.Contains("TenPhong"))
                {
                    dgvDatPhong.Columns["TenPhong"].HeaderText = "Phòng";
                    dgvDatPhong.Columns["TenPhong"].Width = 100;
                    dgvDatPhong.Columns["TenPhong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvDatPhong.Columns["TenPhong"].DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                }

                if (dgvDatPhong.Columns.Contains("LoaiPhong"))
                {
                    dgvDatPhong.Columns["LoaiPhong"].HeaderText = "Loại";
                    dgvDatPhong.Columns["LoaiPhong"].Width = 100;
                }

                if (dgvDatPhong.Columns.Contains("NgayNhanPhong"))
                {
                    dgvDatPhong.Columns["NgayNhanPhong"].HeaderText = "Ngày nhận";
                    dgvDatPhong.Columns["NgayNhanPhong"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvDatPhong.Columns["NgayNhanPhong"].Width = 110;
                    dgvDatPhong.Columns["NgayNhanPhong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (dgvDatPhong.Columns.Contains("NgayTraPhong"))
                {
                    dgvDatPhong.Columns["NgayTraPhong"].HeaderText = "Ngày trả";
                    dgvDatPhong.Columns["NgayTraPhong"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvDatPhong.Columns["NgayTraPhong"].Width = 110;
                    dgvDatPhong.Columns["NgayTraPhong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (dgvDatPhong.Columns.Contains("SoNguoi"))
                {
                    dgvDatPhong.Columns["SoNguoi"].HeaderText = "Số người";
                    dgvDatPhong.Columns["SoNguoi"].Width = 90;
                    dgvDatPhong.Columns["SoNguoi"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (dgvDatPhong.Columns.Contains("TienCoc"))
                {
                    dgvDatPhong.Columns["TienCoc"].HeaderText = "Tiền cọc";
                    dgvDatPhong.Columns["TienCoc"].DefaultCellStyle.Format = "#,##0";
                    dgvDatPhong.Columns["TienCoc"].Width = 110;
                    dgvDatPhong.Columns["TienCoc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if (dgvDatPhong.Columns.Contains("TrangThai"))
                {
                    dgvDatPhong.Columns["TrangThai"].HeaderText = "Trạng thái";
                    dgvDatPhong.Columns["TrangThai"].Width = 110;
                    dgvDatPhong.Columns["TrangThai"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvDatPhong.Columns["TrangThai"].DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ FormatDataGridView Error: {ex.Message}");
            }
        }

        private void TinhTongTien()
        {
            try
            {
                int soNgay = (dtpNgayTraPhong.Value.Date - dtpNgayNhanPhong.Value.Date).Days;
                if (soNgay <= 0) soNgay = 1;

                decimal giaPhong = 0;
                string giaText = txtGiaPhong.Text.Replace(",", "").Replace(" VNĐ", "").Trim();
                if (!string.IsNullOrEmpty(giaText))
                    decimal.TryParse(giaText, out giaPhong);

                decimal tongTien = giaPhong * soNgay;
                txtTongTien.Text = tongTien.ToString("#,##0 VNĐ");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ TinhTongTien Error: {ex.Message}");
            }
        }

        private void btnDatPhong_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                DialogResult dr = MessageBox.Show(
                    $"📋 Xác nhận đặt phòng?\n\n" +
                    $"👤 Khách hàng: {txtTenKH.Text}\n" +
                    $"🏨 Phòng: {cboPhong.Text}\n" +
                    $"📅 Ngày nhận: {dtpNgayNhanPhong.Value:dd/MM/yyyy}\n" +
                    $"📅 Ngày trả: {dtpNgayTraPhong.Value:dd/MM/yyyy}\n" +
                    $"👥 Số người: {txtSoNguoi.Text}\n" +
                    $"💰 Tiền cọc: {txtTienCoc.Text} VNĐ\n" +
                    $"💵 Tổng tiền: {txtTongTien.Text}",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dr != DialogResult.Yes)
                    return;

                string maKH = cboMaKH.SelectedValue.ToString();
                string maPhong = cboPhong.SelectedValue.ToString();
                int soNguoi = int.Parse(txtSoNguoi.Text);
                decimal tienCoc = decimal.Parse(txtTienCoc.Text.Replace(",", "").Replace(" VNĐ", ""));
                string ghiChu = txtGhiChu.Text.Trim();

                bool result = DatPhongBUS.Instance.DatPhong(
                    maKH,
                    maPhong,
                    maNVDangNhap,
                    dtpNgayNhanPhong.Value.Date,
                    dtpNgayTraPhong.Value.Date,
                    soNguoi,
                    tienCoc,
                    ghiChu
                );

                if (result)
                {
                    MessageBox.Show("✅ Đặt phòng thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    LoadData();
                    LoadPhong();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi đặt phòng:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(currentMaDatPhong))
                {
                    MessageBox.Show("⚠️ Vui lòng chọn đặt phòng cần cập nhật!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateInput())
                    return;

                DialogResult dr = MessageBox.Show(
                    $"📝 Xác nhận cập nhật đặt phòng?\n\n" +
                    $"Mã đặt phòng: {currentMaDatPhong}\n" +
                    $"👤 Khách hàng: {txtTenKH.Text}\n" +
                    $"🏨 Phòng: {cboPhong.Text}\n" +
                    $"📅 Ngày nhận: {dtpNgayNhanPhong.Value:dd/MM/yyyy}\n" +
                    $"📅 Ngày trả: {dtpNgayTraPhong.Value:dd/MM/yyyy}\n" +
                    $"👥 Số người: {txtSoNguoi.Text}\n" +
                    $"💰 Tiền cọc: {txtTienCoc.Text} VNĐ",
                    "Xác nhận cập nhật",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dr != DialogResult.Yes)
                    return;

                string maKH = cboMaKH.SelectedValue.ToString();
                string maPhong = cboPhong.SelectedValue.ToString();
                int soNguoi = int.Parse(txtSoNguoi.Text);
                decimal tienCoc = decimal.Parse(txtTienCoc.Text.Replace(",", "").Replace(" VNĐ", ""));
                string ghiChu = txtGhiChu.Text.Trim();

                bool result = DatPhongBUS.Instance.UpdateDatPhong(
                    currentMaDatPhong,
                    maKH,
                    maPhong,
                    dtpNgayNhanPhong.Value.Date,
                    dtpNgayTraPhong.Value.Date,
                    soNguoi,
                    tienCoc,
                    ghiChu
                );

                if (result)
                {
                    MessageBox.Show("✅ Cập nhật đặt phòng thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    LoadData();
                    LoadPhong();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi cập nhật:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            try
            {
                LoadData();
                LoadPhong();
                LoadKhachHang();
                ClearForm();
                MessageBox.Show("✅ Đã làm mới dữ liệu!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi làm mới: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDatPhong_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow row = dgvDatPhong.Rows[e.RowIndex];

                currentMaDatPhong = row.Cells["MaDatPhong"].Value?.ToString() ?? "";

                if (row.Cells["MaKH"].Value != DBNull.Value)
                {
                    string maKH = row.Cells["MaKH"].Value.ToString();
                    cboMaKH.SelectedValue = maKH;
                }

                if (row.Cells["MaPhong"].Value != DBNull.Value)
                {
                    string maPhong = row.Cells["MaPhong"].Value.ToString();
                    bool found = false;
                    foreach (DataRowView item in cboPhong.Items)
                    {
                        if (item["MaPhong"].ToString() == maPhong)
                        {
                            found = true;
                            break;
                        }
                    }
                    
                    if (found)
                        cboPhong.SelectedValue = maPhong;
                    else
                    {
                        txtLoaiPhong.Text = row.Cells["LoaiPhong"].Value?.ToString() ?? "";
                        if (row.Cells["GiaPhong"].Value != DBNull.Value)
                        {
                            decimal giaPhong = Convert.ToDecimal(row.Cells["GiaPhong"].Value);
                            txtGiaPhong.Text = giaPhong.ToString("#,##0 VNĐ");
                        }
                    }
                }

                if (row.Cells["NgayNhanPhong"].Value != DBNull.Value)
                    dtpNgayNhanPhong.Value = Convert.ToDateTime(row.Cells["NgayNhanPhong"].Value);

                if (row.Cells["NgayTraPhong"].Value != DBNull.Value)
                    dtpNgayTraPhong.Value = Convert.ToDateTime(row.Cells["NgayTraPhong"].Value);

                if (row.Cells["SoNguoi"].Value != DBNull.Value)
                    txtSoNguoi.Text = row.Cells["SoNguoi"].Value.ToString();

                if (row.Cells["TienCoc"].Value != DBNull.Value)
                {
                    decimal tienCoc = Convert.ToDecimal(row.Cells["TienCoc"].Value);
                    txtTienCoc.Text = tienCoc.ToString("#,##0");
                }

                if (row.Cells["GhiChu"].Value != DBNull.Value)
                    txtGhiChu.Text = row.Cells["GhiChu"].Value.ToString();

                TinhTongTien();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (cboMaKH.SelectedIndex < 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn khách hàng!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMaKH.Focus();
                return false;
            }

            if (cboPhong.SelectedIndex < 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn phòng!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboPhong.Focus();
                return false;
            }

            if (dtpNgayTraPhong.Value.Date <= dtpNgayNhanPhong.Value.Date)
            {
                MessageBox.Show("⚠️ Ngày trả phòng phải sau ngày nhận phòng!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpNgayTraPhong.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSoNguoi.Text) || !int.TryParse(txtSoNguoi.Text, out int soNguoi) || soNguoi <= 0)
            {
                MessageBox.Show("⚠️ Vui lòng nhập số người hợp lệ (> 0)!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoNguoi.Focus();
                return false;
            }

            string tienCocText = txtTienCoc.Text.Replace(",", "").Replace(" VNĐ", "").Trim();
            if (!decimal.TryParse(tienCocText, out decimal tienCoc) || tienCoc < 0)
            {
                MessageBox.Show("⚠️ Tiền cọc không hợp lệ!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTienCoc.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            currentMaDatPhong = "";
            cboMaKH.SelectedIndex = -1;
            cboPhong.SelectedIndex = -1;
            txtTenKH.Clear();
            txtSDT.Clear();
            txtLoaiPhong.Clear();
            txtGiaPhong.Clear();
            dtpNgayNhanPhong.Value = DateTime.Now.Date;
            dtpNgayTraPhong.Value = DateTime.Now.Date.AddDays(1);
            txtSoNguoi.Text = "1";
            txtTienCoc.Text = "0";
            txtGhiChu.Clear();
            txtTongTien.Text = "0 VNĐ";
        }
    }
}
