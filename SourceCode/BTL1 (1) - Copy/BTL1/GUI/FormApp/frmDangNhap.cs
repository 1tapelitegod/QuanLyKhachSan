using System;
using System.Windows.Forms;
using BTL1.BUS;
using BTL1.DAL;

namespace BTL1.GUI
{
    public partial class frmDangNhap : Form
    {
        public frmDangNhap()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    labelError.Text = "Vui lòng nhập đầy đủ thông tin!";
                    labelError.Visible = true;
                    return;
                }

                var result = NhanVienBUS.Instance.DangNhap(username, password);

                if (result.Rows.Count > 0)
                {
                    string maNV = result.Rows[0]["MaNV"].ToString();
                    string tenNV = result.Rows[0]["TenNV"].ToString();
                    string chucVu = result.Rows[0]["ChucVu"].ToString();
                    string sdt = result.Rows[0]["SoDienThoai"].ToString();

                    SessionManager.SetSession(maNV, tenNV, chucVu, sdt);

                    MessageBox.Show($"Đăng nhập thành công!\n\nXin chào: {tenNV}\nChức vụ: {chucVu}",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    BTL1.GUI.FormApp.frmChinh frmMain = new BTL1.GUI.FormApp.frmChinh();
                    this.Hide();
                    frmMain.ShowDialog();
                    
                    SessionManager.ClearSession();
                    this.Close();
                }
                else
                {
                    labelError.Text = "Tên đăng nhập hoặc mật khẩu sai!";
                    labelError.Visible = true;
                    txtPassword.Clear();
                    txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            labelError.Visible = false;
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}
