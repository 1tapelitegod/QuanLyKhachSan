using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BTL1.BUS;

namespace BTL1.GUI.UserControls
{
    public partial class UC_SuDungDichVu : UserControl
    {
        public UC_SuDungDichVu()
        {
            InitializeComponent();
            InitializeEvents();
            LoadData();
        }

        private void InitializeEvents()
        {
            // Button events
            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnTimKiem.Click += btnTimKiem_Click;
            btnLamMoi.Click += btnLamMoi_Click;

            // DataGridView events
            dgvSuDungDichVu.CellClick += dgvSuDungDichVu_CellClick;

            // ComboBox events
            cboMaKH.SelectedIndexChanged += cboMaKH_SelectedIndexChanged;
            cboMaDV.SelectedIndexChanged += cboMaDV_SelectedIndexChanged;

            // TextBox events
            txtSoLuong.TextChanged += txtSoLuong_TextChanged;
            txtSoLuong.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };

            // Search on Enter
            txtTimKiem.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    btnTimKiem_Click(s, e);
                    e.Handled = true;
                }
            };

            // Hover effects
            AddHoverEffect(btnThem, Color.FromArgb(56, 142, 60), Color.FromArgb(76, 175, 80));
            AddHoverEffect(btnSua, Color.FromArgb(230, 126, 0), Color.FromArgb(255, 152, 0));
            AddHoverEffect(btnXoa, Color.FromArgb(211, 47, 47), Color.FromArgb(244, 67, 54));
            AddHoverEffect(btnTimKiem, Color.FromArgb(0, 130, 116), Color.White);
            AddHoverEffect(btnLamMoi, Color.FromArgb(230, 126, 0), Color.FromArgb(255, 152, 0));
        }

        private void AddHoverEffect(Button btn, Color hoverColor, Color normalColor)
        {
            Color originalBack = btn.BackColor;
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = normalColor;
        }

        private void LoadData()
        {
            try
            {
                // Load danh sách sử dụng dịch vụ
                DataTable dt = SuDungDichVuBUS.Instance.GetAllSuDungDichVu();
                dgvSuDungDichVu.DataSource = dt;
                FormatDataGridView();

                // Load combo boxes
                LoadComboBoxes();

                // Load trạng thái
                cboTrangThai.Items.Clear();
                cboTrangThai.Items.AddRange(new object[] { 
                    "Chưa thanh toán", 
                    "Đã thanh toán", 
                    "Đã hủy" 
                });
                cboTrangThai.SelectedIndex = 0;

                ClearInputs();
                UpdateStatusLabel(dt.Rows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load dữ liệu: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                // Load phòng
                DataTable dtPhong = PhongBUS.Instance.GetAllPhong();
                cboMaPhong.Items.Clear();
                foreach (DataRow row in dtPhong.Rows)
                {
                    cboMaPhong.Items.Add(new ComboBoxItem 
                    { 
                        Value = row["MaPhong"].ToString(), 
                        Text = $"{row["MaPhong"]} - {row["TenPhong"]}" 
                    });
                }
                cboMaPhong.DisplayMember = "Text";
                cboMaPhong.ValueMember = "Value";

                // Load khách hàng
                DataTable dtKH = KhachHangBUS.Instance.GetAllKhachHang();
                cboMaKH.Items.Clear();
                foreach (DataRow row in dtKH.Rows)
                {
                    cboMaKH.Items.Add(new ComboBoxItem 
                    { 
                        Value = row["MaKH"].ToString(), 
                        Text = $"{row["MaKH"]} - {row["TenKH"]} ({row["SoDienThoai"]})" 
                    });
                }
                cboMaKH.DisplayMember = "Text";
                cboMaKH.ValueMember = "Value";

                // Load dịch vụ
                DataTable dtDV = DichVuBUS.Instance.GetAllDichVu();
                cboMaDV.Items.Clear();
                foreach (DataRow row in dtDV.Rows)
                {
                    cboMaDV.Items.Add(new ComboBoxItem 
                    { 
                        Value = row["MaDV"].ToString(), 
                        Text = $"{row["MaDV"]} - {row["TenDV"]} - {Convert.ToDecimal(row["GiaDV"]):N0} VNĐ" 
                    });
                }
                cboMaDV.DisplayMember = "Text";
                cboMaDV.ValueMember = "Value";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load combo boxes: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            if (dgvSuDungDichVu.Columns.Count == 0) return;

            // Thiết lập chiều rộng cụ thể cho từng cột
            dgvSuDungDichVu.Columns["MaSuDung"].HeaderText = "Mã sử dụng";
            dgvSuDungDichVu.Columns["MaSuDung"].Width = 150;

            dgvSuDungDichVu.Columns["MaPhong"].HeaderText = "Phòng";
            dgvSuDungDichVu.Columns["MaPhong"].Width = 80;

            dgvSuDungDichVu.Columns["TenPhong"].HeaderText = "Tên phòng";
            dgvSuDungDichVu.Columns["TenPhong"].Width = 150;

            dgvSuDungDichVu.Columns["TenDV"].HeaderText = "Dịch vụ";
            dgvSuDungDichVu.Columns["TenDV"].Width = 200;

            dgvSuDungDichVu.Columns["TenKH"].HeaderText = "Khách hàng";
            dgvSuDungDichVu.Columns["TenKH"].Width = 180;

            dgvSuDungDichVu.Columns["SoLuong"].HeaderText = "SL";
            dgvSuDungDichVu.Columns["SoLuong"].Width = 60;
            dgvSuDungDichVu.Columns["SoLuong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvSuDungDichVu.Columns["DonGia"].HeaderText = "Đơn giá";
            dgvSuDungDichVu.Columns["DonGia"].DefaultCellStyle.Format = "#,##0";
            dgvSuDungDichVu.Columns["DonGia"].Width = 120;
            dgvSuDungDichVu.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvSuDungDichVu.Columns["ThanhTien"].HeaderText = "Thành tiền";
            dgvSuDungDichVu.Columns["ThanhTien"].DefaultCellStyle.Format = "#,##0";
            dgvSuDungDichVu.Columns["ThanhTien"].Width = 120;
            dgvSuDungDichVu.Columns["ThanhTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSuDungDichVu.Columns["ThanhTien"].DefaultCellStyle.ForeColor = Color.FromArgb(76, 175, 80);
            dgvSuDungDichVu.Columns["ThanhTien"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);

            dgvSuDungDichVu.Columns["NgaySuDung"].HeaderText = "Ngày sử dụng";
            dgvSuDungDichVu.Columns["NgaySuDung"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            dgvSuDungDichVu.Columns["NgaySuDung"].Width = 150;

            dgvSuDungDichVu.Columns["TrangThai"].HeaderText = "Trạng thái";
            dgvSuDungDichVu.Columns["TrangThai"].Width = 130;

            // Ẩn các cột không cần thiết
            if (dgvSuDungDichVu.Columns.Contains("MaDV"))
                dgvSuDungDichVu.Columns["MaDV"].Visible = false;
            if (dgvSuDungDichVu.Columns.Contains("MaKH"))
                dgvSuDungDichVu.Columns["MaKH"].Visible = false;
            if (dgvSuDungDichVu.Columns.Contains("SoDienThoai"))
                dgvSuDungDichVu.Columns["SoDienThoai"].Visible = false;
            if (dgvSuDungDichVu.Columns.Contains("GhiChu"))
                dgvSuDungDichVu.Columns["GhiChu"].Visible = false;
        }

        private void UpdateStatusLabel(int count)
        {
            lblTitle.Text = $"🛎️ QUẢN LÝ SỬ DỤNG DỊCH VỤ - Tổng: {count} bản ghi";
        }

        private void ClearInputs()
        {
            txtMaSuDung.Clear();
            cboMaPhong.SelectedIndex = -1;
            cboMaKH.SelectedIndex = -1;
            txtTenKH.Clear();
            cboMaDV.SelectedIndex = -1;
            txtDonGia.Clear();
            txtSoLuong.Text = "1";
            txtThanhTien.Clear();
            dtpNgaySuDung.Value = DateTime.Now;
            txtGhiChu.Clear();
            cboTrangThai.SelectedIndex = 0;

            btnThem.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        private bool ValidateInputs()
        {
            if (cboMaPhong.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn phòng!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMaPhong.Focus();
                return false;
            }

            if (cboMaKH.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn khách hàng!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMaKH.Focus();
                return false;
            }

            if (cboMaDV.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMaDV.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSoLuong.Text) || int.Parse(txtSoLuong.Text) <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoLuong.Focus();
                return false;
            }

            return true;
        }

        private void cboMaKH_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaKH.SelectedIndex == -1) return;

            try
            {
                string maKH = ((ComboBoxItem)cboMaKH.SelectedItem).Value;
                DataTable dt = KhachHangBUS.Instance.GetKhachHangByMa(maKH);
                
                if (dt.Rows.Count > 0)
                {
                    txtTenKH.Text = dt.Rows[0]["TenKH"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboMaDV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaDV.SelectedIndex == -1) return;

            try
            {
                string maDV = ((ComboBoxItem)cboMaDV.SelectedItem).Value;
                DataTable dt = DichVuBUS.Instance.GetDichVuByMa(maDV);
                
                if (dt.Rows.Count > 0)
                {
                    decimal giaDV = Convert.ToDecimal(dt.Rows[0]["GiaDV"]);
                    txtDonGia.Text = giaDV.ToString("#,##0");
                    TinhThanhTien();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSoLuong_TextChanged(object sender, EventArgs e)
        {
            TinhThanhTien();
        }

        private void TinhThanhTien()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDonGia.Text) || string.IsNullOrWhiteSpace(txtSoLuong.Text))
                    return;

                decimal donGia = decimal.Parse(txtDonGia.Text.Replace(",", ""));
                int soLuong = int.Parse(txtSoLuong.Text);
                decimal thanhTien = donGia * soLuong;

                txtThanhTien.Text = thanhTien.ToString("#,##0");
            }
            catch { }
        }

        private void dgvSuDungDichVu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow row = dgvSuDungDichVu.Rows[e.RowIndex];
                
                txtMaSuDung.Text = row.Cells["MaSuDung"].Value?.ToString();
                
                // Set phòng
                string maPhong = row.Cells["MaPhong"].Value?.ToString();
                foreach (ComboBoxItem item in cboMaPhong.Items)
                {
                    if (item.Value == maPhong)
                    {
                        cboMaPhong.SelectedItem = item;
                        break;
                    }
                }

                // Set khách hàng
                string maKH = row.Cells["MaKH"].Value?.ToString();
                foreach (ComboBoxItem item in cboMaKH.Items)
                {
                    if (item.Value == maKH)
                    {
                        cboMaKH.SelectedItem = item;
                        break;
                    }
                }

                // Set dịch vụ
                string maDV = row.Cells["MaDV"].Value?.ToString();
                foreach (ComboBoxItem item in cboMaDV.Items)
                {
                    if (item.Value == maDV)
                    {
                        cboMaDV.SelectedItem = item;
                        break;
                    }
                }

                txtSoLuong.Text = row.Cells["SoLuong"].Value?.ToString();
                
                if (row.Cells["NgaySuDung"].Value != DBNull.Value)
                    dtpNgaySuDung.Value = Convert.ToDateTime(row.Cells["NgaySuDung"].Value);

                txtGhiChu.Text = row.Cells["GhiChu"].Value?.ToString();
                cboTrangThai.Text = row.Cells["TrangThai"].Value?.ToString();

                btnThem.Enabled = false;
                btnSua.Enabled = true;
                btnXoa.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInputs()) return;

                string maPhong = ((ComboBoxItem)cboMaPhong.SelectedItem).Value;
                string maKH = ((ComboBoxItem)cboMaKH.SelectedItem).Value;
                string maDV = ((ComboBoxItem)cboMaDV.SelectedItem).Value;

                DialogResult result = MessageBox.Show(
                    $"Xác nhận thêm sử dụng dịch vụ:\n\n" +
                    $"Phòng: {cboMaPhong.Text}\n" +
                    $"Khách hàng: {txtTenKH.Text}\n" +
                    $"Dịch vụ: {cboMaDV.Text}\n" +
                    $"Số lượng: {txtSoLuong.Text}\n" +
                    $"Thành tiền: {txtThanhTien.Text} VNĐ", 
                    "Xác nhận", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return;

                bool success = SuDungDichVuBUS.Instance.InsertSuDungDichVu(
                    maPhong,
                    maDV,
                    maKH,
                    int.Parse(txtSoLuong.Text),
                    dtpNgaySuDung.Value,
                    txtGhiChu.Text.Trim(),
                    cboTrangThai.Text
                );

                if (success)
                {
                    MessageBox.Show("Thêm thành công!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaSuDung.Text))
                {
                    MessageBox.Show("Vui lòng chọn bản ghi cần sửa!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateInputs()) return;

                string maPhong = ((ComboBoxItem)cboMaPhong.SelectedItem).Value;
                string maKH = ((ComboBoxItem)cboMaKH.SelectedItem).Value;
                string maDV = ((ComboBoxItem)cboMaDV.SelectedItem).Value;

                DialogResult result = MessageBox.Show(
                    $"Xác nhận cập nhật:\n\nMã: {txtMaSuDung.Text}", 
                    "Xác nhận", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return;

                bool success = SuDungDichVuBUS.Instance.UpdateSuDungDichVu(
                    txtMaSuDung.Text,
                    maPhong,
                    maDV,
                    maKH,
                    int.Parse(txtSoLuong.Text),
                    dtpNgaySuDung.Value,
                    txtGhiChu.Text.Trim(),
                    cboTrangThai.Text
                );

                if (success)
                {
                    MessageBox.Show("Cập nhật thành công!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaSuDung.Text))
                {
                    MessageBox.Show("Vui lòng chọn bản ghi cần xóa!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"Bạn có chắc muốn xóa?\n\nMã: {txtMaSuDung.Text}\n\nLưu ý: Số lượng tồn kho sẽ được hoàn lại!", 
                    "Xác nhận xóa", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes) return;

                bool success = SuDungDichVuBUS.Instance.DeleteSuDungDichVu(txtMaSuDung.Text);
                
                if (success)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string maSuDung = txtTimKiem.Text.Trim();
                
                DataTable dt;
                if (string.IsNullOrEmpty(maSuDung))
                {
                    dt = SuDungDichVuBUS.Instance.GetAllSuDungDichVu();
                }
                else
                {
                    // ✅ CHỈ TÌM THEO MÃ SỬ DỤNG
                    dt = SuDungDichVuBUS.Instance.SearchByMaSuDung(maSuDung);
                }

                dgvSuDungDichVu.DataSource = dt;
                FormatDataGridView();
                UpdateStatusLabel(dt.Rows.Count);
                
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"Không tìm thấy mã sử dụng: {maSuDung}", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadData();
            txtTimKiem.Clear();
        }

        // Helper class cho ComboBox
        private class ComboBoxItem
        {
            public string Value { get; set; }
            public string Text { get; set; }
        }

        private void lblSoLuong_Click(object sender, EventArgs e)
        {

        }
    }
}
