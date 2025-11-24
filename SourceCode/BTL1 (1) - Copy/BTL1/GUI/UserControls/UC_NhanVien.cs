using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BTL1.BUS;

namespace BTL1.GUI.UserControls
{
    public partial class UC_NhanVien : UserControl
    {
        public UC_NhanVien()
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
            dgvNhanVien.CellClick += dgvNhanVien_CellClick;

            txtTimKiem.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    btnTimKiem_Click(s, e);
                    e.Handled = true;
                }
            };

            txtSoDienThoai.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            };

            txtCMND.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            };

            txtLuong.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
                {
                    e.Handled = true;
                }
            };

            txtLuong.Leave += (s, e) =>
            {
                if (decimal.TryParse(txtLuong.Text.Replace(",", ""), out decimal luong))
                {
                    txtLuong.Text = luong.ToString("#,##0");
                }
            };

            txtLuong.Enter += (s, e) =>
            {
                txtLuong.Text = txtLuong.Text.Replace(",", "");
                txtLuong.SelectAll();
            };
            dtpNgaySinh.Format = DateTimePickerFormat.Custom;
            dtpNgaySinh.CustomFormat = "dd/MM/yyyy";
        }

        private void LoadData()
        {
            try
            {
                DataTable dt = NhanVienBUS.Instance.GetAllNhanVien();
                dgvNhanVien.DataSource = dt;
                FormatDataGridView();
                ClearInputs();
                UpdateStatusLabel(dt.Rows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load dữ liệu: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            if (dgvNhanVien.Columns.Count == 0) return;

            dgvNhanVien.EnableHeadersVisualStyles = false;
            dgvNhanVien.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 87, 34);
            dgvNhanVien.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvNhanVien.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvNhanVien.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvNhanVien.ColumnHeadersHeight = 40;

            dgvNhanVien.RowTemplate.Height = 35;
            dgvNhanVien.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 224, 178);
            dgvNhanVien.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvNhanVien.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            dgvNhanVien.Columns["MaNV"].HeaderText = "Mã NV";
            dgvNhanVien.Columns["MaNV"].Width = 100;
            dgvNhanVien.Columns["MaNV"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvNhanVien.Columns["MaNV"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            
            dgvNhanVien.Columns["TenNV"].HeaderText = "Tên nhân viên";
            dgvNhanVien.Columns["TenNV"].Width = 200;
            dgvNhanVien.Columns["TenNV"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            
            dgvNhanVien.Columns["CMND"].HeaderText = "CMND/CCCD";
            dgvNhanVien.Columns["CMND"].Width = 120;
            dgvNhanVien.Columns["CMND"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            dgvNhanVien.Columns["SoDienThoai"].HeaderText = "Số điện thoại";
            dgvNhanVien.Columns["SoDienThoai"].Width = 120;
            dgvNhanVien.Columns["SoDienThoai"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvNhanVien.Columns["SoDienThoai"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            
            dgvNhanVien.Columns["DiaChi"].HeaderText = "Địa chỉ";
            dgvNhanVien.Columns["DiaChi"].Width = 200;
            
            dgvNhanVien.Columns["NgaySinh"].HeaderText = "Ngày sinh";
            dgvNhanVien.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvNhanVien.Columns["NgaySinh"].Width = 120;
            dgvNhanVien.Columns["NgaySinh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            dgvNhanVien.Columns["GioiTinh"].HeaderText = "Giới tính";
            dgvNhanVien.Columns["GioiTinh"].Width = 100;
            dgvNhanVien.Columns["GioiTinh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            dgvNhanVien.Columns["ChucVu"].HeaderText = "Chức vụ";
            dgvNhanVien.Columns["ChucVu"].Width = 150;
            dgvNhanVien.Columns["ChucVu"].DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 224);
            
            dgvNhanVien.Columns["Luong"].HeaderText = "Lương (VNĐ)";
            dgvNhanVien.Columns["Luong"].DefaultCellStyle.Format = "#,##0";
            dgvNhanVien.Columns["Luong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvNhanVien.Columns["Luong"].Width = 150;
            dgvNhanVien.Columns["Luong"].DefaultCellStyle.ForeColor = Color.FromArgb(76, 175, 80);
            dgvNhanVien.Columns["Luong"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);

            if (dgvNhanVien.Columns.Contains("Passw"))
            {
                dgvNhanVien.Columns["Passw"].Visible = false;
            }
        }

        private void UpdateStatusLabel(int count)
        {
            this.Text = $"Tổng số nhân viên: {count}";
        }

        private void ClearInputs()
        {
            txtMaNV.Clear();
            txtTenNV.Clear();
            txtCMND.Clear();
            txtSoDienThoai.Clear();
            txtDiaChi.Clear();
            txtLuong.Clear();
            dtpNgaySinh.Value = DateTime.Now.AddYears(-22);
            cboGioiTinh.SelectedIndex = 0;
            cboChucVu.SelectedIndex = 0;
            txtTenNV.Focus();

            btnThem.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtTenNV.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhân viên!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNV.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoDienThoai.Focus();
                return false;
            }

            if (txtSoDienThoai.Text.Length < 10 || txtSoDienThoai.Text.Length > 11)
            {
                MessageBox.Show("Số điện thoại phải có 10-11 số!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoDienThoai.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLuong.Text))
            {
                MessageBox.Show("Vui lòng nhập lương!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLuong.Focus();
                return false;
            }

            if (cboChucVu.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn chức vụ!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboChucVu.Focus();
                return false;
            }

            int tuoi = DateTime.Now.Year - dtpNgaySinh.Value.Year;
            if (tuoi < 18)
            {
                MessageBox.Show("Nhân viên phải từ 18 tuổi trở lên!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpNgaySinh.Focus();
                return false;
            }

            return true;
        }

        private void dgvNhanVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow row = dgvNhanVien.Rows[e.RowIndex];
                txtMaNV.Text = row.Cells["MaNV"].Value?.ToString();
                txtTenNV.Text = row.Cells["TenNV"].Value?.ToString();
                txtCMND.Text = row.Cells["CMND"].Value?.ToString();
                txtSoDienThoai.Text = row.Cells["SoDienThoai"].Value?.ToString();
                txtDiaChi.Text = row.Cells["DiaChi"].Value?.ToString();
                
                if (row.Cells["NgaySinh"].Value != DBNull.Value)
                    dtpNgaySinh.Value = Convert.ToDateTime(row.Cells["NgaySinh"].Value);
                
                cboGioiTinh.Text = row.Cells["GioiTinh"].Value?.ToString();
                cboChucVu.Text = row.Cells["ChucVu"].Value?.ToString();
                
                if (row.Cells["Luong"].Value != DBNull.Value)
                {
                    decimal luong = Convert.ToDecimal(row.Cells["Luong"].Value);
                    txtLuong.Text = luong.ToString("#,##0");
                }

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

                decimal luong = decimal.Parse(txtLuong.Text.Replace(",", ""));

                DialogResult result = MessageBox.Show(
                    $"Xác nhận thêm nhân viên:\n\nTên: {txtTenNV.Text}\nChức vụ: {cboChucVu.Text}\nLương: {luong:#,##0} VNĐ", 
                    "Xác nhận", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return;

                bool success = NhanVienBUS.Instance.InsertNhanVien(
                    txtTenNV.Text.Trim(),
                    txtCMND.Text.Trim(),
                    txtSoDienThoai.Text.Trim(),
                    txtDiaChi.Text.Trim(),
                    dtpNgaySinh.Value,
                    cboGioiTinh.Text,
                    cboChucVu.Text,
                    luong
                );

                if (success)
                {
                    MessageBox.Show("Thêm nhân viên thành công!", "Thông báo", 
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
                if (string.IsNullOrEmpty(txtMaNV.Text))
                {
                    MessageBox.Show("Vui lòng chọn nhân viên cần sửa!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateInputs()) return;

                decimal luong = decimal.Parse(txtLuong.Text.Replace(",", ""));

                DialogResult result = MessageBox.Show(
                    $"Xác nhận cập nhật nhân viên:\n\nMã NV: {txtMaNV.Text}\nTên: {txtTenNV.Text}\nLương: {luong:#,##0} VNĐ", 
                    "Xác nhận", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return;

                bool success = NhanVienBUS.Instance.UpdateNhanVien(
                    txtMaNV.Text,
                    txtTenNV.Text.Trim(),
                    txtCMND.Text.Trim(),
                    txtSoDienThoai.Text.Trim(),
                    txtDiaChi.Text.Trim(),
                    dtpNgaySinh.Value,
                    cboGioiTinh.Text,
                    cboChucVu.Text,
                    luong
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
                if (string.IsNullOrEmpty(txtMaNV.Text))
                {
                    MessageBox.Show("Vui lòng chọn nhân viên cần xóa!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"Bạn có chắc muốn xóa nhân viên?\n\nMã NV: {txtMaNV.Text}\nTên: {txtTenNV.Text}\nChức vụ: {cboChucVu.Text}\n\nLưu ý: Không thể khôi phục sau khi xóa!", 
                    "Xác nhận xóa", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes) return;

                bool success = NhanVienBUS.Instance.DeleteNhanVien(txtMaNV.Text);
                
                if (success)
                {
                    MessageBox.Show("Xóa nhân viên thành công!", "Thông báo", 
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

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadData();
            txtTimKiem.Clear();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string maNV = txtTimKiem.Text.Trim();
                
                DataTable dt;
                if (string.IsNullOrEmpty(maNV))
                {
                    dt = NhanVienBUS.Instance.GetAllNhanVien();
                }
                else
                {
                    // ✅ CHỈ TÌM THEO MÃ NHÂN VIÊN
                    dt = NhanVienBUS.Instance.SearchNhanVienByMa(maNV);
                }

                dgvNhanVien.DataSource = dt;
                FormatDataGridView();
                
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"Không tìm thấy nhân viên có mã: {maNV}", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi tìm kiếm: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtpNgaySinh_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
