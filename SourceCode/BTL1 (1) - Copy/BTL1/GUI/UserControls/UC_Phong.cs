using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BTL1.BUS;

namespace BTL1.GUI.UserControls
{
    public partial class UC_Phong : UserControl
    {
        public UC_Phong()
        {
            InitializeComponent();
            InitializeEvents();
            LoadData();
        }

        private void InitializeEvents()
        {
            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLamMoi.Click += btnLamMoi_Click;
            btnTimKiem.Click += btnTimKiem_Click;
            
            dgvPhong.CellClick += dgvPhong_CellClick;
            cboLocTrangThai.SelectedIndexChanged += cboLocTrangThai_SelectedIndexChanged;
            
            txtTimKiem.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    btnTimKiem_Click(s, e);
                    e.Handled = true;
                }
            };

            txtGiaPhong.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
                {
                    e.Handled = true;
                }
            };

            txtGiaPhong.Leave += (s, e) =>
            {
                if (decimal.TryParse(txtGiaPhong.Text.Replace(",", ""), out decimal gia))
                {
                    txtGiaPhong.Text = gia.ToString("#,##0");
                }
            };

            // ✅ Set mặc định - Kiểm tra null và Items.Count trước
            try
            {
                if (cboTrangThai != null && cboTrangThai.Items.Count > 0)
                    cboTrangThai.SelectedIndex = 0;
                    
                if (cboLocTrangThai != null && cboLocTrangThai.Items.Count > 0)
                    cboLocTrangThai.SelectedIndex = 0;
            }
            catch
            {
                // Ignore nếu không set được mặc định
            }
        }

        private void LoadData()
        {
            try
            {
                DataTable dt = PhongBUS.Instance.GetAllPhong();
                dgvPhong.DataSource = dt;
                FormatDataGridView();
                ClearInputs();
                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load dữ liệu: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            if (dgvPhong.Columns.Count == 0) return;

            dgvPhong.Columns["MaPhong"].HeaderText = "Mã phòng";
            dgvPhong.Columns["MaPhong"].Width = 120;
            dgvPhong.Columns["MaPhong"].DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvPhong.Columns["MaPhong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            dgvPhong.Columns["TenPhong"].HeaderText = "Tên phòng";
            dgvPhong.Columns["TenPhong"].Width = 150;
            dgvPhong.Columns["TenPhong"].DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            dgvPhong.Columns["LoaiPhong"].HeaderText = "Loại phòng";
            dgvPhong.Columns["LoaiPhong"].Width = 150;
            
            // ✅ Thêm cột Số người tối đa
            if (dgvPhong.Columns.Contains("SoNguoiToiDa"))
            {
                dgvPhong.Columns["SoNguoiToiDa"].HeaderText = "Số người tối đa";
                dgvPhong.Columns["SoNguoiToiDa"].Width = 130;
                dgvPhong.Columns["SoNguoiToiDa"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            
            dgvPhong.Columns["GiaPhong"].HeaderText = "Giá phòng/đêm";
            dgvPhong.Columns["GiaPhong"].DefaultCellStyle.Format = "#,##0 VNĐ";
            dgvPhong.Columns["GiaPhong"].Width = 150;
            dgvPhong.Columns["GiaPhong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPhong.Columns["GiaPhong"].DefaultCellStyle.ForeColor = Color.FromArgb(76, 175, 80);
            dgvPhong.Columns["GiaPhong"].DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            dgvPhong.Columns["TrangThai"].HeaderText = "Trạng thái";
            dgvPhong.Columns["TrangThai"].Width = 150;
            dgvPhong.Columns["TrangThai"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPhong.Columns["TrangThai"].DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            // ✅ Ẩn cột MoTa nếu có
            if (dgvPhong.Columns.Contains("MoTa"))
                dgvPhong.Columns["MoTa"].Visible = false;

            // Format màu cho trạng thái
            foreach (DataGridViewRow row in dgvPhong.Rows)
            {
                string trangThai = row.Cells["TrangThai"].Value?.ToString();
                switch (trangThai)
                {
                    case "Trống":
                        row.Cells["TrangThai"].Style.BackColor = Color.FromArgb(76, 175, 80);
                        row.Cells["TrangThai"].Style.ForeColor = Color.White;
                        break;
                    case "Đã đặt":
                        row.Cells["TrangThai"].Style.BackColor = Color.FromArgb(33, 150, 243);
                        row.Cells["TrangThai"].Style.ForeColor = Color.White;
                        break;
                    case "Đang sử dụng":
                        row.Cells["TrangThai"].Style.BackColor = Color.FromArgb(255, 152, 0);
                        row.Cells["TrangThai"].Style.ForeColor = Color.White;
                        break;
                    case "Đang sửa chữa":
                        row.Cells["TrangThai"].Style.BackColor = Color.FromArgb(244, 67, 54);
                        row.Cells["TrangThai"].Style.ForeColor = Color.White;
                        break;
                }
            }
        }

        private void ClearInputs()
        {
            txtMaPhong.Clear();
            txtTenPhong.Clear();
            txtLoaiPhong.Clear();
            numSoNguoiToiDa.Value = 2;
            txtGiaPhong.Clear();
            txtMoTa.Clear();
            cboTrangThai.SelectedIndex = 0;
            
            txtTenPhong.Focus();
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = !string.IsNullOrEmpty(txtMaPhong.Text);
            btnThem.Enabled = true;
            btnSua.Enabled = hasSelection;
            btnXoa.Enabled = hasSelection;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtTenPhong.Text))
            {
                MessageBox.Show("⚠️ Vui lòng nhập tên phòng!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenPhong.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLoaiPhong.Text))
            {
                MessageBox.Show("⚠️ Vui lòng nhập loại phòng!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLoaiPhong.Focus();
                return false;
            }

            if (numSoNguoiToiDa.Value < 1)
            {
                MessageBox.Show("⚠️ Số người tối đa phải lớn hơn 0!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numSoNguoiToiDa.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtGiaPhong.Text))
            {
                MessageBox.Show("⚠️ Vui lòng nhập giá phòng!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaPhong.Focus();
                return false;
            }

            if (!decimal.TryParse(txtGiaPhong.Text.Replace(",", ""), out decimal gia) || gia <= 0)
            {
                MessageBox.Show("⚠️ Giá phòng không hợp lệ!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaPhong.Focus();
                return false;
            }

            return true;
        }

        private void dgvPhong_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow row = dgvPhong.Rows[e.RowIndex];
                
                txtMaPhong.Text = row.Cells["MaPhong"].Value?.ToString();
                txtTenPhong.Text = row.Cells["TenPhong"].Value?.ToString();
                txtLoaiPhong.Text = row.Cells["LoaiPhong"].Value?.ToString();
                
                // ✅ Load số người tối đa
                if (row.Cells["SoNguoiToiDa"].Value != DBNull.Value)
                    numSoNguoiToiDa.Value = Convert.ToInt32(row.Cells["SoNguoiToiDa"].Value);
                
                if (row.Cells["GiaPhong"].Value != DBNull.Value)
                {
                    decimal gia = Convert.ToDecimal(row.Cells["GiaPhong"].Value);
                    txtGiaPhong.Text = gia.ToString("#,##0");
                }
                
                cboTrangThai.Text = row.Cells["TrangThai"].Value?.ToString();
                txtMoTa.Text = row.Cells["MoTa"].Value?.ToString();

                UpdateButtonStates();
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

                decimal giaPhong = decimal.Parse(txtGiaPhong.Text.Replace(",", ""));
                int soNguoiToiDa = (int)numSoNguoiToiDa.Value;

                DialogResult result = MessageBox.Show(
                    $"✅ Xác nhận thêm phòng:\n\n" +
                    $"📌 Tên: {txtTenPhong.Text}\n" +
                    $"🏷 Loại: {txtLoaiPhong.Text}\n" +
                    $"👥 Số người tối đa: {soNguoiToiDa}\n" +
                    $"💰 Giá: {giaPhong:#,##0} VNĐ", 
                    "Xác nhận", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return;

                bool success = PhongBUS.Instance.InsertPhong(
                    txtTenPhong.Text.Trim(),
                    txtLoaiPhong.Text.Trim(),
                    soNguoiToiDa,
                    giaPhong,
                    txtMoTa.Text.Trim(),
                    cboTrangThai.Text
                );

                if (success)
                {
                    MessageBox.Show("✅ Thêm phòng thành công!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaPhong.Text))
                {
                    MessageBox.Show("⚠️ Vui lòng chọn phòng cần sửa!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateInputs()) return;

                decimal giaPhong = decimal.Parse(txtGiaPhong.Text.Replace(",", ""));
                int soNguoiToiDa = (int)numSoNguoiToiDa.Value;

                DialogResult result = MessageBox.Show(
                    $"✏️ Xác nhận cập nhật phòng:\n\n" +
                    $"🔖 Mã: {txtMaPhong.Text}\n" +
                    $"📌 Tên: {txtTenPhong.Text}\n" +
                    $"🏷 Loại: {txtLoaiPhong.Text}\n" +
                    $"👥 Số người tối đa: {soNguoiToiDa}\n" +
                    $"💰 Giá: {giaPhong:#,##0} VNĐ", 
                    "Xác nhận", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return;

                bool success = PhongBUS.Instance.UpdatePhong(
                    txtMaPhong.Text,
                    txtTenPhong.Text.Trim(),
                    txtLoaiPhong.Text.Trim(),
                    soNguoiToiDa,
                    giaPhong,
                    txtMoTa.Text.Trim(),
                    cboTrangThai.Text
                );

                if (success)
                {
                    MessageBox.Show("✅ Cập nhật thành công!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaPhong.Text))
                {
                    MessageBox.Show("⚠️ Vui lòng chọn phòng cần xóa!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboTrangThai.Text == "Đang sử dụng" || cboTrangThai.Text == "Đã đặt")
                {
                    MessageBox.Show("⚠️ Không thể xóa phòng đang được sử dụng hoặc đã đặt!", "Cảnh báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"🗑️ Bạn có chắc muốn xóa phòng?\n\n" +
                    $"Mã: {txtMaPhong.Text}\n" +
                    $"Tên: {txtTenPhong.Text}\n\n" +
                    $"⚠️ Lưu ý: Không thể khôi phục sau khi xóa!", 
                    "Xác nhận xóa", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes) return;

                bool success = PhongBUS.Instance.DeletePhong(txtMaPhong.Text);
                
                if (success)
                {
                    MessageBox.Show("✅ Xóa thành công!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadData();
            txtTimKiem.Clear();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string maPhong = txtTimKiem.Text.Trim();
                
                DataTable dt;
                if (string.IsNullOrEmpty(maPhong))
                {
                    dt = PhongBUS.Instance.GetAllPhong();
                }
                else
                {
                    // ✅ CHỈ TÌM THEO MÃ PHÒNG
                    dt = PhongBUS.Instance.SearchPhongByMa(maPhong);
                }

                dgvPhong.DataSource = dt;
                FormatDataGridView();
                
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"Không tìm thấy phòng có mã: {maPhong}", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi tìm kiếm: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboLocTrangThai_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string trangThai = cboLocTrangThai.Text;
                DataTable dt = PhongBUS.Instance.GetPhongByTrangThai(trangThai);
                dgvPhong.DataSource = dt;
                FormatDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi lọc: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
