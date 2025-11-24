using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BTL1.BUS;

namespace BTL1.GUI.UserControls
{
    public partial class UC_KhachHang : UserControl
    {
        public UC_KhachHang()
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
            dgvKhachHang.CellClick += dgvKhachHang_CellClick;

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
            dtpNgaySinh.Format = DateTimePickerFormat.Custom;
            dtpNgaySinh.CustomFormat = "dd/MM/yyyy";
        }

        private void LoadData()
        {
            try
            {
                DataTable dt = KhachHangBUS.Instance.GetAllKhachHang();
                dgvKhachHang.DataSource = dt;
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
            if (dgvKhachHang.Columns.Count == 0) return;

            dgvKhachHang.EnableHeadersVisualStyles = false;
            dgvKhachHang.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(156, 39, 176);
            dgvKhachHang.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvKhachHang.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvKhachHang.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvKhachHang.ColumnHeadersHeight = 40;

            dgvKhachHang.RowTemplate.Height = 35;
            dgvKhachHang.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 190, 231);
            dgvKhachHang.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvKhachHang.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            dgvKhachHang.Columns["MaKH"].HeaderText = "Mã KH";
            dgvKhachHang.Columns["MaKH"].Width = 100;
            dgvKhachHang.Columns["MaKH"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvKhachHang.Columns["MaKH"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            
            dgvKhachHang.Columns["TenKH"].HeaderText = "Tên khách hàng";
            dgvKhachHang.Columns["TenKH"].Width = 200;
            dgvKhachHang.Columns["TenKH"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            
            dgvKhachHang.Columns["CMND"].HeaderText = "CMND/CCCD";
            dgvKhachHang.Columns["CMND"].Width = 120;
            dgvKhachHang.Columns["CMND"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            dgvKhachHang.Columns["SoDienThoai"].HeaderText = "Số điện thoại";
            dgvKhachHang.Columns["SoDienThoai"].Width = 120;
            dgvKhachHang.Columns["SoDienThoai"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvKhachHang.Columns["SoDienThoai"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            
            dgvKhachHang.Columns["DiaChi"].HeaderText = "Địa chỉ";
            dgvKhachHang.Columns["DiaChi"].Width = 250;
            
            dgvKhachHang.Columns["NgaySinh"].HeaderText = "Ngày sinh";
            dgvKhachHang.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvKhachHang.Columns["NgaySinh"].Width = 120;
            dgvKhachHang.Columns["NgaySinh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            dgvKhachHang.Columns["GioiTinh"].HeaderText = "Giới tính";
            dgvKhachHang.Columns["GioiTinh"].Width = 100;
            dgvKhachHang.Columns["GioiTinh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            dgvKhachHang.Columns["QuocTich"].HeaderText = "Quốc tịch";
            dgvKhachHang.Columns["QuocTich"].Width = 120;
            dgvKhachHang.Columns["QuocTich"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void UpdateStatusLabel(int count)
        {
            this.Text = $"Tổng số khách hàng: {count}";
        }

        private void ClearInputs()
        {
            txtMaKH.Clear();
            txtTenKH.Clear();
            txtCMND.Clear();
            txtSoDienThoai.Clear();
            txtDiaChi.Clear();
            txtQuocTich.Text = "Việt Nam";
            dtpNgaySinh.Value = DateTime.Now.AddYears(-18);
            cboGioiTinh.SelectedIndex = 0;
            txtTenKH.Focus();

            btnThem.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtTenKH.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKH.Focus();
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

            if (dtpNgaySinh.Value > DateTime.Now.AddYears(-16))
            {
                MessageBox.Show("Khách hàng phải trên 16 tuổi!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpNgaySinh.Focus();
                return false;
            }

            return true;
        }

        private void dgvKhachHang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow row = dgvKhachHang.Rows[e.RowIndex];
                txtMaKH.Text = row.Cells["MaKH"].Value?.ToString();
                txtTenKH.Text = row.Cells["TenKH"].Value?.ToString();
                txtCMND.Text = row.Cells["CMND"].Value?.ToString();
                txtSoDienThoai.Text = row.Cells["SoDienThoai"].Value?.ToString();
                txtDiaChi.Text = row.Cells["DiaChi"].Value?.ToString();
                
                if (row.Cells["NgaySinh"].Value != DBNull.Value)
                    dtpNgaySinh.Value = Convert.ToDateTime(row.Cells["NgaySinh"].Value);
                
                cboGioiTinh.Text = row.Cells["GioiTinh"].Value?.ToString();
                txtQuocTich.Text = row.Cells["QuocTich"].Value?.ToString();

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

                DialogResult result = MessageBox.Show(
                    $"Xác nhận thêm khách hàng:\n\nTên: {txtTenKH.Text}\nSĐT: {txtSoDienThoai.Text}", 
                    "Xác nhận", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return;

                bool success = KhachHangBUS.Instance.InsertKhachHang(
                    txtTenKH.Text.Trim(),
                    txtCMND.Text.Trim(),
                    txtSoDienThoai.Text.Trim(),
                    txtDiaChi.Text.Trim(),
                    dtpNgaySinh.Value,
                    cboGioiTinh.Text,
                    txtQuocTich.Text.Trim()
                );

                if (success)
                {
                    MessageBox.Show("Thêm khách hàng thành công!", "Thông báo", 
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
                if (string.IsNullOrEmpty(txtMaKH.Text))
                {
                    MessageBox.Show("Vui lòng chọn khách hàng cần sửa!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateInputs()) return;

                DialogResult result = MessageBox.Show(
                    $"Xác nhận cập nhật khách hàng:\n\nMã KH: {txtMaKH.Text}\nTên: {txtTenKH.Text}", 
                    "Xác nhận", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return;

                bool success = KhachHangBUS.Instance.UpdateKhachHang(
                    txtMaKH.Text,
                    txtTenKH.Text.Trim(),
                    txtCMND.Text.Trim(),
                    txtSoDienThoai.Text.Trim(),
                    txtDiaChi.Text.Trim(),
                    dtpNgaySinh.Value,
                    cboGioiTinh.Text,
                    txtQuocTich.Text.Trim()
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
                if (string.IsNullOrEmpty(txtMaKH.Text))
                {
                    MessageBox.Show("Vui lòng chọn khách hàng cần xóa!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"Bạn có chắc muốn xóa khách hàng?\n\nMã KH: {txtMaKH.Text}\nTên: {txtTenKH.Text}\n\nLưu ý: Không thể khôi phục sau khi xóa!", 
                    "Xác nhận xóa", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes) return;

                bool success = KhachHangBUS.Instance.DeleteKhachHang(txtMaKH.Text);
                
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

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadData();
            txtTimKiem.Clear();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string maKH = txtTimKiem.Text.Trim();
                
                DataTable dt;
                if (string.IsNullOrEmpty(maKH))
                {
                    dt = KhachHangBUS.Instance.GetAllKhachHang();
                }
                else
                {
                    // ✅ CHỈ TÌM THEO MÃ KHÁCH HÀNG
                    dt = KhachHangBUS.Instance.SearchKhachHangByMa(maKH);
                }

                dgvKhachHang.DataSource = dt;
                FormatDataGridView();
                
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"Không tìm thấy khách hàng có mã: {maKH}", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi tìm kiếm: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
