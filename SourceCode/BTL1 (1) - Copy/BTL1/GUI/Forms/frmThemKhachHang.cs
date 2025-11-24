using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BTL1.BUS;

namespace BTL1.GUI.Forms
{
    public partial class frmThemKhachHang : Form
    {
        // ✅ Properties để trả về dữ liệu
        public string MaKH { get; private set; }
        public string TenKH { get; private set; }
        public string DiaChi { get; private set; }
        public string SoDienThoai { get; private set; }

        // ✅ Controls
        private TextBox txtSDT;
        private TextBox txtTenKH;
        private TextBox txtDiaChi;
        private TextBox txtCMND;
        private DateTimePicker dtpNgaySinh;
        private ComboBox cboGioiTinh;
        private TextBox txtQuocTich;
        private Button btnLuu;
        private Button btnHuy;

        public frmThemKhachHang(string sdt)
        {
            InitializeComponent();
            InitializeForm();
            
            // Tự động điền SĐT
            txtSDT.Text = sdt;
            txtSDT.ReadOnly = true;
            txtSDT.BackColor = Color.FromArgb(240, 240, 240);
            
            // Focus vào Tên
            this.Shown += (s, e) => txtTenKH.Focus();
        }

        private void InitializeForm()
        {
            // ✅ Cấu hình Form
            this.Text = "Thêm Khách Hàng Mới";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(540, 640); // ✅ Tăng thêm width
            this.BackColor = Color.White;
            this.AutoScroll = true;

            InitializeControls();
            InitializeEvents();
        }

        private void InitializeControls()
        {
            // ✅ Panel chính với AutoScroll
            Panel pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            // ✅ Panel nội dung
            Panel pnlContent = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top,
                BackColor = Color.White
            };

            // ✅ Label tiêu đề
            Label lblTitle = new Label
            {
                Text = "📝 THÔNG TIN KHÁCH HÀNG MỚI",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = Color.White
            };

            // ✅ Panel chứa các field
            FlowLayoutPanel flowPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top,
                Padding = new Padding(5)
            };

            // ✅ Số điện thoại
            flowPanel.Controls.Add(CreateInputPanel("📞 Số điện thoại: *", out txtSDT));

            // ✅ Tên khách hàng
            flowPanel.Controls.Add(CreateInputPanel("👤 Họ và tên: *", out txtTenKH));

            // ✅ CMND/CCCD
            flowPanel.Controls.Add(CreateInputPanel("🪪 CMND/CCCD:", out txtCMND));

            // ✅ Ngày sinh với DateTimePicker
            flowPanel.Controls.Add(CreateDatePickerPanel("🎂 Ngày sinh: *", out dtpNgaySinh));

            // ✅ Giới tính với ComboBox dropdown
            flowPanel.Controls.Add(CreateComboBoxPanel("⚧ Giới tính: *", out cboGioiTinh));

            // ✅ Địa chỉ
            flowPanel.Controls.Add(CreateInputPanel("📍 Địa chỉ: *", out txtDiaChi));

            // ✅ Quốc tịch
            flowPanel.Controls.Add(CreateInputPanel("🌍 Quốc tịch:", out txtQuocTich));
            txtQuocTich.Text = "Việt Nam";

            // ✅ Panel buttons (cố định ở dưới)
            Panel pnlButtons = new Panel
            {
                Height = 70,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(10)
            };

            btnLuu = new Button
            {
                Text = "✔️ Lưu",
                Width = 130,
                Height = 45,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLuu.FlatAppearance.BorderSize = 0;
            btnLuu.Location = new Point(110, 12);

            btnHuy = new Button
            {
                Text = "✖️ Hủy",
                Width = 130,
                Height = 45,
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnHuy.FlatAppearance.BorderSize = 0;
            btnHuy.Location = new Point(260, 12);

            pnlButtons.Controls.Add(btnLuu);
            pnlButtons.Controls.Add(btnHuy);

            // ✅ Thêm controls vào form
            pnlContent.Controls.Add(flowPanel);
            pnlContent.Controls.Add(lblTitle);
            
            pnlMain.Controls.Add(pnlContent);
            
            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlButtons);
        }

        // ✅ Phương thức tạo panel input (TextBox)
        private Panel CreateInputPanel(string labelText, out TextBox textBox)
        {
            Panel panel = new Panel
            {
                Width = 480,
                Height = 70,
                Margin = new Padding(3),
                BackColor = Color.White
            };

            Label lbl = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, 5),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            textBox = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Width = 470,
                Height = 28,
                Location = new Point(5, 30),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            panel.Controls.Add(lbl);
            panel.Controls.Add(textBox);

            return panel;
        }

        // ✅ Phương thức tạo panel DateTimePicker - FIX: Giảm width để icon calendar không bị che
        private Panel CreateDatePickerPanel(string labelText, out DateTimePicker dateTimePicker)
        {
            Panel panel = new Panel
            {
                Width = 480,
                Height = 70,
                Margin = new Padding(3),
                BackColor = Color.White
            };

            Label lbl = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, 5),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            // ✅ FIX: Giảm width xuống để có chỗ cho icon calendar
            dateTimePicker = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10F),
                Width = 470,  // ✅ Đủ rộng để hiển thị icon
                Height = 28,
                Location = new Point(5, 30),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                ShowUpDown = false,  // ✅ Hiển thị icon calendar
                Value = DateTime.Now.AddYears(-18)
            };

            panel.Controls.Add(lbl);
            panel.Controls.Add(dateTimePicker);

            return panel;
        }

        // ✅ Phương thức tạo panel ComboBox - FIX: Thêm RightToLeft để mũi tên hiện rõ
        private Panel CreateComboBoxPanel(string labelText, out ComboBox comboBox)
        {
            Panel panel = new Panel
            {
                Width = 480,
                Height = 70,
                Margin = new Padding(3),
                BackColor = Color.White
            };

            Label lbl = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, 5),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            // ✅ FIX: ComboBox với mũi tên dropdown rõ ràng
            comboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Width = 470,
                Height = 28,
                Location = new Point(5, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,  // ✅ Chỉ chọn
                FlatStyle = FlatStyle.Standard,  // ✅ Standard để hiện mũi tên
                BackColor = Color.White,
                RightToLeft = RightToLeft.No  // ✅ Đảm bảo mũi tên ở bên phải
            };

            // ✅ Thêm các option giới tính
            comboBox.Items.AddRange(new object[] { "Nam", "Nữ", "Khác" });
            comboBox.SelectedIndex = 0; // Mặc định "Nam"

            panel.Controls.Add(lbl);
            panel.Controls.Add(comboBox);

            return panel;
        }

        private void InitializeEvents()
        {
            btnLuu.Click += btnLuu_Click;
            btnHuy.Click += btnHuy_Click;

            // ✅ Ctrl+Enter để lưu, Esc để hủy
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Control)
                {
                    e.Handled = true;
                    btnLuu_Click(s, e);
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    e.Handled = true;
                    btnHuy_Click(s, e);
                }
            };

            // ✅ Chỉ cho nhập số vào CMND
            txtCMND.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };

            // ✅ Hover effects cho buttons
            btnLuu.MouseEnter += (s, e) => btnLuu.BackColor = Color.FromArgb(56, 142, 60);
            btnLuu.MouseLeave += (s, e) => btnLuu.BackColor = Color.FromArgb(76, 175, 80);

            btnHuy.MouseEnter += (s, e) => btnHuy.BackColor = Color.FromArgb(211, 47, 47);
            btnHuy.MouseLeave += (s, e) => btnHuy.BackColor = Color.FromArgb(244, 67, 54);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // ✅ Validate tên khách hàng
                if (string.IsNullOrWhiteSpace(txtTenKH.Text))
                {
                    MessageBox.Show("⚠️ Vui lòng nhập tên khách hàng!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenKH.Focus();
                    return;
                }

                // ✅ Validate địa chỉ
                if (string.IsNullOrWhiteSpace(txtDiaChi.Text))
                {
                    MessageBox.Show("⚠️ Vui lòng nhập địa chỉ!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiaChi.Focus();
                    return;
                }

                // ✅ Validate ngày sinh (phải trên 16 tuổi)
                if (dtpNgaySinh.Value > DateTime.Now.AddYears(-16))
                {
                    MessageBox.Show("⚠️ Khách hàng phải từ 16 tuổi trở lên!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtpNgaySinh.Focus();
                    return;
                }

                // ✅ Validate giới tính
                if (cboGioiTinh.SelectedIndex == -1)
                {
                    MessageBox.Show("⚠️ Vui lòng chọn giới tính!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboGioiTinh.Focus();
                    return;
                }

                // ✅ Hiển thị progress
                this.Cursor = Cursors.WaitCursor;
                btnLuu.Enabled = false;

                // ✅ Thêm vào CSDL
                bool result = KhachHangBUS.Instance.InsertKhachHang(
                    txtTenKH.Text.Trim(),
                    txtCMND.Text.Trim(),
                    txtSDT.Text.Trim(),
                    txtDiaChi.Text.Trim(),
                    dtpNgaySinh.Value,
                    cboGioiTinh.SelectedItem.ToString(),
                    string.IsNullOrWhiteSpace(txtQuocTich.Text) ? "Việt Nam" : txtQuocTich.Text.Trim()
                );

                if (result)
                {
                    // ✅ Lấy lại thông tin khách hàng vừa thêm
                    DataTable dtKH = KhachHangBUS.Instance.GetKhachHangBySDT(txtSDT.Text.Trim());
                    
                    if (dtKH != null && dtKH.Rows.Count > 0)
                    {
                        // ✅ Gán dữ liệu để trả về
                        MaKH = dtKH.Rows[0]["MaKH"].ToString();
                        TenKH = txtTenKH.Text.Trim();
                        DiaChi = txtDiaChi.Text.Trim();
                        SoDienThoai = txtSDT.Text.Trim();

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("❌ Lỗi: Không tìm thấy khách hàng vừa thêm!",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi thêm khách hàng:\n\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnLuu.Enabled = true;
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}