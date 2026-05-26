using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SimplePOS
{
    public class LoginForm : Form
    {
        private Color clrBg = Color.FromArgb(240, 244, 248);
        private Color clrCard = Color.White;
        private Color clrBlue = Color.FromArgb(41, 121, 255);
        private Color clrBlueDark = Color.FromArgb(25, 95, 210);
        private Color clrText = Color.FromArgb(30, 40, 60);
        private Color clrMuted = Color.FromArgb(120, 135, 160);
        private Color clrBorder = Color.FromArgb(210, 218, 230);
        private Color clrInput = Color.FromArgb(248, 250, 252);

        private TextBox txtUsername = new TextBox();
        private TextBox txtPassword = new TextBox();
        private Label lblError = new Label();
        private Button btnLogin = new Button();

        public LoginForm()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Choyhub POS — Login";
            this.Size = new Size(460, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = clrBg;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.DoubleBuffered = true;

            Panel card = new Panel();
            card.Size = new Size(380, 490);
            card.Location = new Point(40, 40);
            card.BackColor = clrCard;
            card.Paint += (s, e) =>
            {
                using (var pen = new Pen(clrBorder))
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };
            this.Controls.Add(card);

            // Blue top bar
            Panel topBar = new Panel { Location = new Point(0, 0), Size = new Size(380, 5), BackColor = clrBlue };
            card.Controls.Add(topBar);

            // Logo circle
            PictureBox logo = new PictureBox { Size = new Size(70, 70), Location = new Point(155, 28), BackColor = Color.Transparent };
            logo.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var b = new SolidBrush(clrBlue))
                    g.FillEllipse(b, 0, 0, 70, 70);
                using (var p = new Pen(Color.White, 2.5f))
                {
                    g.DrawRectangle(p, 16, 22, 38, 26);
                    g.DrawLine(p, 26, 22, 26, 14);
                    g.DrawLine(p, 44, 22, 44, 14);
                    g.DrawLine(p, 26, 14, 44, 14);
                    g.DrawLine(p, 22, 36, 22, 40);
                    g.DrawLine(p, 30, 36, 30, 40);
                    g.DrawLine(p, 38, 36, 38, 40);
                    g.DrawLine(p, 46, 36, 46, 40);
                }
            };
            card.Controls.Add(logo);

            // Title
            var lblTitle = new Label { Text = "CHOYHUB", Font = new Font("Trebuchet MS", 22, FontStyle.Bold), ForeColor = clrBlue, Size = new Size(380, 36), Location = new Point(0, 110), TextAlign = ContentAlignment.MiddleCenter };
            card.Controls.Add(lblTitle);

            var lblSub = new Label { Text = "Point of Sale System", Font = new Font("Segoe UI", 10), ForeColor = clrMuted, Size = new Size(380, 22), Location = new Point(0, 146), TextAlign = ContentAlignment.MiddleCenter };
            card.Controls.Add(lblSub);

            var div = new Panel { Location = new Point(40, 178), Size = new Size(300, 1), BackColor = clrBorder };
            card.Controls.Add(div);

            // Username field
            AddLabel(card, "USERNAME", new Point(40, 194));
            txtUsername = AddTextBox(card, new Point(40, 214), false);

            // Password field
            AddLabel(card, "PASSWORD", new Point(40, 278));
            txtPassword = AddTextBox(card, new Point(40, 298), true);

            // Error
            lblError.Font = new Font("Segoe UI", 9); lblError.ForeColor = Color.FromArgb(220, 53, 69);
            lblError.Location = new Point(40, 358); lblError.Size = new Size(300, 20);
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            card.Controls.Add(lblError);

            // Login button
            btnLogin.Text = "SIGN IN"; btnLogin.Size = new Size(300, 46); btnLogin.Location = new Point(40, 382);
            btnLogin.BackColor = clrBlue; btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat; btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Font = new Font("Segoe UI", 12, FontStyle.Bold); btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = clrBlueDark;
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = clrBlue;
            card.Controls.Add(btnLogin);



            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnLogin_Click(s, e); };
            txtUsername.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtPassword.Focus(); };
        }

        private void AddLabel(Panel p, string text, Point loc)
        {
            p.Controls.Add(new Label { Text = text, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = clrMuted, Location = loc, AutoSize = true });
        }

        private TextBox AddTextBox(Panel parent, Point loc, bool password)
        {
            Panel border = new Panel { Location = new Point(loc.X - 1, loc.Y - 1), Size = new Size(302, 54), BackColor = clrBorder };
            parent.Controls.Add(border);
            Panel bg = new Panel { Location = new Point(1, 1), Size = new Size(300, 52), BackColor = clrInput };
            border.Controls.Add(bg);
            TextBox txt = new TextBox { Location = new Point(10, 13), Size = new Size(278, 26), Font = new Font("Segoe UI", 12), BackColor = clrInput, ForeColor = clrText, BorderStyle = BorderStyle.None };
            if (password) txt.PasswordChar = '●';
            txt.Enter += (s, e) => border.BackColor = clrBlue;
            txt.Leave += (s, e) => border.BackColor = clrBorder;
            bg.Controls.Add(txt);
            return txt;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == "1" && txtPassword.Text == "1")
            {
                new Form1().Show();
                this.Hide();
            }
            else
            {
                lblError.Text = "✕  Invalid username or password";
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}