using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BTL1.BUS;

namespace BTL1.GUI.UserControls
{
    public partial class UC_DichVu : UserControl
    {
        public UC_DichVu()
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
            dgvDichVu.CellClick += dgvDichVu_CellClick;

            txtTimKiem.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    btnTimKiem_Click(s, e);
                    e.Handled = true;
                }
            };

            txtGiaDV.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
                {
                    e.Handled = true;
                }
            };

            txtGiaDV.Leave += (s, e) =>
            {
                if (decimal.TryParse(txtGiaDV.Text.Replace(",", ""), out decimal gia))
                {
                    txtGiaDV.Text = gia.ToString("#,##0");
                }
            };

            txtGiaDV.Enter += (s, e) =>
            {
                txtGiaDV.Text = txtGiaDV.Text.Replace(",", "");
                txtGiaDV.SelectAll();
            };
        }

        private void LoadData()
        {
            try
            {
                DataTable dt = DichVuBUS.Instance.GetAllDichVu();
                dgvDichVu.DataSource = dt;
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
            if (dgvDichVu.Columns.Count == 0) return;

            dgvDichVu.EnableHeadersVisualStyles = false;
            dgvDichVu.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(233, 30, 99);
            dgvDichVu.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDichVu.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvDichVu.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDichVu.ColumnHeadersHeight = 40;

            dgvDichVu.RowTemplate.Height = 35;
            dgvDichVu.DefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 187, 208);
            dgvDichVu.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvDichVu.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            dgvDichVu.Columns["MaDV"].HeaderText = "Mã DV";
            dgvDichVu.Columns["MaDV"].Width = 100;
            dgvDichVu.Columns["MaDV"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDichVu.Columns["MaDV"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            
            dgvDichVu.Columns["TenDV"].HeaderText = "Tên dịch vụ";
            dgvDichVu.Columns["TenDV"].Width = 300;
            dgvDichVu.Columns["TenDV"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            
            dgvDichVu.Columns["GiaDV"].HeaderText = "Giá (VNĐ)";
            dgvDichVu.Columns["GiaDV"].DefaultCellStyle.Format = "#,##0";
            dgvDichVu.Columns["GiaDV"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDichVu.Columns["GiaDV"].Width = 150;
            dgvDichVu.Columns["GiaDV"].DefaultCellStyle.ForeColor = Color.FromArgb(0, 150, 136);
            dgvDichVu.Columns["GiaDV"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            
            dgvDichVu.Columns["DonVi"].HeaderText = "Đơn vị";
            dgvDichVu.Columns["DonVi"].Width = 120;
            dgvDichVu.Columns["DonVi"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            dgvDichVu.Columns["MoTa"].HeaderText = "Mô tả";
            dgvDichVu.Columns["MoTa"].Width = 350;
            dgvDichVu.Columns["MoTa"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        private void UpdateStatusLabel(int count)
        {
            this.Text = $"Tổng số dịch vụ: {count}";
        }

        private void ClearInputs()
        {
            txtMaDV.Clear();
            txtTenDV.Clear();
            txtGiaDV.Clear();
            txtDonVi.Clear();
            txtMoTa.Clear();
            txtTenDV.Focus();

            btnThem.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtTenDV.Text))
            {
                MessageBox.Show("Vui lòng nhập tên dịch vụ!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenDV.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtGiaDV.Text))
            {
                MessageBox.Show("Vui lòng nhập giá dịch vụ!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaDV.Focus();
                return false;
            }

            decimal giaDV;
            string cleanPrice = txtGiaDV.Text.Replace(",", "").Trim();
            if (!decimal.TryParse(cleanPrice, out giaDV) || giaDV <= 0)
            {
                MessageBox.Show("Giá dịch vụ không hợp lệ!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaDV.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDonVi.Text))
            {
                MessageBox.Show("Vui lòng nhập đơn vị tính!", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDonVi.Focus();
                return false;
            }

            return true;
        }

        private void dgvDichVu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow row = dgvDichVu.Rows[e.RowIndex];
                txtMaDV.Text = row.Cells["MaDV"].Value?.ToString();
                txtTenDV.Text = row.Cells["TenDV"].Value?.ToString();
                
                if (row.Cells["GiaDV"].Value != null)
                {
                    decimal giaDV = Convert.ToDecimal(row.Cells["GiaDV"].Value);
                    txtGiaDV.Text = giaDV.ToString("#,##0");
                }
                
                txtDonVi.Text = row.Cells["DonVi"].Value?.ToString();
                txtMoTa.Text = row.Cells["MoTa"].Value?.ToString();

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

                decimal giaDV = decimal.Parse(txtGiaDV.Text.Replace(",", ""));

                DialogResult result = MessageBox.Show(
                    $"Xác nhận thêm dịch vụ:\n\nTên: {txtTenDV.Text}\nGiá: {giaDV:#,##0} VNĐ\nĐơn vị: {txtDonVi.Text}", 
                    "Xác nhận", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return;

                bool success = DichVuBUS.Instance.InsertDichVu(
                    txtTenDV.Text.Trim(),
                    giaDV,
                    txtDonVi.Text.Trim(),
                    txtMoTa.Text.Trim()
                );

                if (success)
                {
                    MessageBox.Show("Thêm dịch vụ thành công!", "Thông báo", 
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
                if (string.IsNullOrEmpty(txtMaDV.Text))
                {
                    MessageBox.Show("Vui lòng chọn dịch vụ cần sửa!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateInputs()) return;

                decimal giaDV = decimal.Parse(txtGiaDV.Text.Replace(",", ""));

                DialogResult result = MessageBox.Show(
                    $"Xác nhận cập nhật dịch vụ:\n\nMã DV: {txtMaDV.Text}\nTên: {txtTenDV.Text}\nGiá: {giaDV:#,##0} VNĐ", 
                    "Xác nhận", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return;

                bool success = DichVuBUS.Instance.UpdateDichVu(
                    txtMaDV.Text,
                    txtTenDV.Text.Trim(),
                    giaDV,
                    txtDonVi.Text.Trim(),
                    txtMoTa.Text.Trim()
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
                if (string.IsNullOrEmpty(txtMaDV.Text))
                {
                    MessageBox.Show("Vui lòng chọn dịch vụ cần xóa!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"Bạn có chắc muốn xóa dịch vụ?\n\nMã DV: {txtMaDV.Text}\nTên: {txtTenDV.Text}\n\nLưu ý: Không thể khôi phục sau khi xóa!", 
                    "Xác nhận xóa", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes) return;

                bool success = DichVuBUS.Instance.DeleteDichVu(txtMaDV.Text);
                
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
                string maDV = txtTimKiem.Text.Trim();
                
                DataTable dt;
                if (string.IsNullOrEmpty(maDV))
                {
                    dt = DichVuBUS.Instance.GetAllDichVu();
                }
                else
                {
                    // ✅ CHỈ TÌM THEO MÃ DỊCH VỤ
                    dt = DichVuBUS.Instance.SearchDichVuByMa(maDV);
                }

                dgvDichVu.DataSource = dt;
                FormatDataGridView();
                
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"Không tìm thấy dịch vụ có mã: {maDV}", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
