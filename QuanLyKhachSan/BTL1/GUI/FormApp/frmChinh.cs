using System;
using System.Drawing;
using System.Windows.Forms;
using BTL1.BUS;
using BTL1.GUI.UserControls;

namespace BTL1.GUI.FormApp
{
    public partial class frmChinh : Form
    {
        private Button currentButton;

        public frmChinh()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void frmChinh_Load(object sender, EventArgs e)
        {
            //MessageBox.Show($"Chức vụ hiện tại: {SessionManager.ChucVu}\n" +
            //        $"Là Admin: {SessionManager.IsAdmin()}", "Debug Quyền", MessageBoxButtons.OK, MessageBoxIcon.Information);
            try
            {
                if (!SessionManager.IsLoggedIn())
                {
                    MessageBox.Show("Chưa đăng nhập! Vui lòng đăng nhập lại.",
                        "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }  
                this.Text = $"Quản Lý Khách Sạn - {SessionManager.GetDisplayName()}";
                bool isAdmin = SessionManager.IsAdmin();
                bool isReceptionist = SessionManager.IsReceptionist();
                bool isAccountant = SessionManager.IsAccountant();
                if (!isAdmin)
                {
                    btnNhanVien.Visible = false;
                    btnPhong.Visible = false;
                    btnDichVu.Visible = false;
                }
                if (!isAccountant)
                {
                    btnThongKe.Visible = false;
                }
                if (!isReceptionist)
                {
                    btnKhachHang.Visible = false;
                    btnDatPhong.Visible = false;
                    btnSuDungDichVu.Visible = false;
                    btnHoaDon.Visible = false;
                }
                // Ưu tiên: Admin/Lễ tân -> Khách hàng
                if (isAdmin || (isReceptionist && !isAccountant))
                {
                    LoadUserControl(new UC_KhachHang());
                    ActivateButton(btnKhachHang);
                    lblTitle.Text = "QUẢN LÝ KHÁCH HÀNG";
                }
                // Tiếp theo: Kế toán -> Thống kê (nếu Kế toán không phải Lễ tân)
                else if (isAccountant)
                {
                    LoadUserControl(new UC_ThongKe());
                    ActivateButton(btnThongKe);
                    lblTitle.Text = "THỐNG KÊ BÁO CÁO";
                }
                else
                {
                    // Trường hợp không có quyền (không nên xảy ra)
                    MessageBox.Show("Tài khoản của bạn không có quyền truy cập chức năng nào!",
                        "Lỗi Quyền", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
                LoadUserControl(new UC_Phong());
                ActivateButton(btnPhong);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUserControl(UserControl userControl)
        {
            panelMain.Controls.Clear();
            userControl.Dock = DockStyle.Fill;
            panelMain.Controls.Add(userControl);
            userControl.BringToFront();
        }

        private void ActivateButton(Button btn)
        {
            if (currentButton != null)
            {
                currentButton.BackColor = Color.FromArgb(51, 51, 76);
                currentButton.ForeColor = Color.Gainsboro;
                currentButton.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            }

            currentButton = btn;
            btn.BackColor = Color.FromArgb(0, 126, 249);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            panelIndicator.Height = btn.Height;
            panelIndicator.Top = btn.Top;
            panelIndicator.Left = btn.Left;
        }

        private void btnPhong_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsAdmin()) return; // Chặn Lễ tân/Kế toán
            LoadUserControl(new UC_Phong());
            ActivateButton(btnPhong);
            lblTitle.Text = "QUẢN LÝ PHÒNG";
        }

        private void btnKhachHang_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsReceptionist()) return; // Chặn Kế toán
            LoadUserControl(new UC_KhachHang());
            ActivateButton(btnKhachHang);
            lblTitle.Text = "QUẢN LÝ KHÁCH HÀNG";
        }

        private void btnNhanVien_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsAdmin()) return;
            LoadUserControl(new UC_NhanVien());
            ActivateButton(btnNhanVien);
            lblTitle.Text = "QUẢN LÝ NHÂN VIÊN";
        }

        private void btnDichVu_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsAdmin()) return;// Chặn Lễ tân/Kế toán
            LoadUserControl(new UC_DichVu());
            ActivateButton(btnDichVu);
            lblTitle.Text = "QUẢN LÝ DỊCH VỤ";
        }

        private void btnSuDungDichVu_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsReceptionist()) return;

            LoadUserControl(new UC_SuDungDichVu());
            ActivateButton(btnSuDungDichVu);
            lblTitle.Text = "QUẢN LÝ SỬ DỤNG DỊCH VỤ";
        }

        private void btnHoaDon_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsReceptionist()) return;
            try
            {
                if (!SessionManager.IsLoggedIn())
                {
                    MessageBox.Show("Không tìm thấy thông tin đăng nhập!",
                        "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LoadUserControl(new UC_HoaDon(SessionManager.MaNV, SessionManager.TenNV));
                ActivateButton(btnHoaDon);
                lblTitle.Text = "QUẢN LÝ HÓA ĐƠN";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsAccountant()) return;
            // ✅ XÓA toàn bộ MessageBox
            LoadUserControl(new UC_ThongKe());
            ActivateButton(btnThongKe);
            lblTitle.Text = "THỐNG KÊ BÁO CÁO";
        }

        private void btnDatPhong_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsReceptionist()) return; // Chặn Kế toán
            try
            {
                if (!SessionManager.IsLoggedIn())
                {
                    MessageBox.Show("Không tìm thấy thông tin đăng nhập!",
                        "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LoadUserControl(new UC_DatPhong(SessionManager.MaNV, SessionManager.TenNV));
                ActivateButton(btnDatPhong);
                lblTitle.Text = "ĐẶT PHÒNG";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                btnMaximize.Text = "🗖";
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                btnMaximize.Text = "🗗";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát ứng dụng?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                $"Đăng xuất tài khoản:\n\n{SessionManager.GetDisplayName()}",
                "Xác nhận đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Hide();
                frmDangNhap login = new frmDangNhap();
                login.ShowDialog();
                this.Close();
            }
        }

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void panelTitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }

        private void panelTitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}