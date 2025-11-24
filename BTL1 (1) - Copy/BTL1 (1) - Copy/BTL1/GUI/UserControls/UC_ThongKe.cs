using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BTL1.BUS;

namespace BTL1.GUI.UserControls
{
    public partial class UC_ThongKe : UserControl
    {
        public UC_ThongKe()
        {
            InitializeComponent();
            InitializeEvents();
            LoadInitialData();
        }

        private void InitializeEvents()
        {
            btnThongKe.Click += btnThongKe_Click;
            
            cboLoaiThongKe.SelectedIndexChanged += (s, e) =>
            {
                UpdateDatePickerVisibility();
            };

            dtpTuNgay.ValueChanged += (s, e) =>
            {
                if (dtpTuNgay.Value > dtpDenNgay.Value)
                {
                    dtpDenNgay.Value = dtpTuNgay.Value;
                }
            };
            dtpTuNgay.Format = DateTimePickerFormat.Custom;
            dtpTuNgay.CustomFormat = "dd/MM/yyyy";
            dtpDenNgay.Format = DateTimePickerFormat.Custom;
            dtpDenNgay.CustomFormat = "dd/MM/yyyy";
        }

        private void LoadInitialData()
        {
            try
            {
                if (cboLoaiThongKe.Items.Count > 0)
                {
                    cboLoaiThongKe.SelectedIndex = 0;
                }

                dtpTuNgay.Value = DateTime.Now.AddMonths(-1);
                dtpDenNgay.Value = DateTime.Now;

                LoadCardData();
                btnThongKe_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateDatePickerVisibility()
        {
            bool needDateRange = cboLoaiThongKe.SelectedIndex != 2;
            dtpTuNgay.Enabled = needDateRange;
            dtpDenNgay.Enabled = needDateRange;
        }

        private void LoadCardData()
        {
            try
            {
                DateTime tuNgay = dtpTuNgay.Value.Date;
                DateTime denNgay = dtpDenNgay.Value.Date.AddDays(1).AddSeconds(-1);

                decimal doanhThu = ThongKeBUS.Instance.GetTongDoanhThu(tuNgay, denNgay);
                lblDoanhThu.Text = doanhThu.ToString("#,##0");

                int tongHD = ThongKeBUS.Instance.GetTongHoaDon(tuNgay, denNgay);
                lblTongHoaDon.Text = tongHD.ToString("#,##0");

                int tongKH = ThongKeBUS.Instance.GetTongKhachHang();
                lblTongKhachHang.Text = tongKH.ToString("#,##0");

                int phongTrong = ThongKeBUS.Instance.GetSoPhongTrong();
                lblSoPhongTrong.Text = phongTrong.ToString("#,##0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load dữ liệu cards: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboLoaiThongKe.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng chọn loại thống kê!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LoadCardData();

                DateTime tuNgay = dtpTuNgay.Value.Date;
                DateTime denNgay = dtpDenNgay.Value.Date.AddDays(1).AddSeconds(-1);

                DataTable dt = null;

                switch (cboLoaiThongKe.SelectedIndex)
                {
                    case 0:
                        dt = ThongKeBUS.Instance.ThongKeDoanhThuTheoNgay(tuNgay, denNgay);
                        break;

                    case 1:
                        dt = ThongKeBUS.Instance.ThongKeDoanhThuTheoThang(tuNgay, denNgay);
                        break;

                    case 2:
                        dt = ThongKeBUS.Instance.ThongKePhongTheoTrangThai();
                        break;

                    case 3:
                        dt = ThongKeBUS.Instance.ThongKeTopKhachHang(tuNgay, denNgay, 10);
                        break;

                    case 4:
                        dt = ThongKeBUS.Instance.ThongKeNhanVien(tuNgay, denNgay);
                        break;
                }

                dgvThongKe.DataSource = dt;
                FormatDataGridView();

                /*if (dt != null && dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu trong khoảng thời gian này!", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thống kê: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            if (dgvThongKe.Columns.Count == 0) return;

            dgvThongKe.EnableHeadersVisualStyles = false;
            dgvThongKe.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 150, 136);
            dgvThongKe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvThongKe.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvThongKe.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvThongKe.ColumnHeadersHeight = 40;

            dgvThongKe.RowTemplate.Height = 35;
            dgvThongKe.DefaultCellStyle.SelectionBackColor = Color.FromArgb(178, 223, 219);
            dgvThongKe.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvThongKe.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            foreach (DataGridViewColumn col in dgvThongKe.Columns)
            {
                if (col.Name.Contains("DoanhThu") || col.Name.Contains("TongChiTieu"))
                {
                    col.DefaultCellStyle.Format = "#,##0";
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    col.DefaultCellStyle.ForeColor = Color.FromArgb(76, 175, 80);
                    col.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                }
                else if (col.Name.Contains("So") || col.Name.Contains("SoLuong") || col.Name.Contains("TyLe"))
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
        }
    }
}
