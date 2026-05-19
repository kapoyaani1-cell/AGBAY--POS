using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace SimplePOS
{
    public partial class Form1 : Form
    {
        // ── DATA ──────────────────────────────────────────────
        List<Product> products = new List<Product>();
        List<CartItem> cart = new List<CartItem>();

        // ── LIGHT COLOR PALETTE ───────────────────────────────
        Color clrPageBg = Color.FromArgb(236, 240, 245);
        Color clrSidebar = Color.FromArgb(255, 255, 255);
        Color clrTopbar = Color.FromArgb(255, 255, 255);
        Color clrRightBg = Color.FromArgb(245, 247, 251);

        Color clrBlue = Color.FromArgb(41, 121, 255);
        Color clrBlueDark = Color.FromArgb(25, 95, 210);
        Color clrBlueLight = Color.FromArgb(235, 242, 255);

        Color clrGreen = Color.FromArgb(34, 197, 94);
        Color clrGreenBg = Color.FromArgb(220, 252, 231);
        Color clrRed = Color.FromArgb(239, 68, 68);
        Color clrRedBg = Color.FromArgb(254, 226, 226);
        Color clrOrange = Color.FromArgb(245, 158, 11);
        Color clrOrangeBg = Color.FromArgb(254, 243, 199);

        Color clrText = Color.FromArgb(17, 24, 39);
        Color clrTextSub = Color.FromArgb(75, 85, 99);
        Color clrMuted = Color.FromArgb(156, 163, 175);
        Color clrBorder = Color.FromArgb(209, 213, 219);
        Color clrCardBg = Color.White;
        Color clrRowAlt = Color.FromArgb(249, 250, 251);

        // ── CONTROLS ─────────────────────────────────────────
        FlowLayoutPanel productPanel;
        DataGridView dgvCart;
        Label lblTotal;
        Label lblChange;
        Label lblCartCount;
        Label lblClock;
        TextBox txtCash;
        TextBox txtSearch;
        ComboBox cmbCategory;
        Panel rightPanel;
        Panel toastPanel;
        Label toastLabel;
        Timer toastTimer;

        // ── DRAG-SCROLL STATE ─────────────────────────────────
        bool _isDragging = false;
        Point _dragStart = Point.Empty;
        int _scrollStart = 0;
        bool _dragConfirmed = false;
        float _velocity = 0f;
        DateTime _lastMoveTime = DateTime.Now;
        int _lastMouseY = 0;
        Timer _inertiaTimer;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            SetupUI();
            LoadProducts();
            DisplayProducts();
            RefreshCart();
            StartClock();
        }

        // ═══════════════════════════════════════════════════════
        //  LAYOUT
        // ═══════════════════════════════════════════════════════
        private void SetupUI()
        {
            this.Text = "CHOYHUB POS SYSTEM";
            this.Size = new Size(1440, 860);
            this.MinimumSize = new Size(1280, 740);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = clrPageBg;

            BuildTopBar();
            BuildLeftSection();
            BuildRightSection();
            BuildToast();
        }

        // ── TOP BAR ───────────────────────────────────────────
        private void BuildTopBar()
        {
            Panel topBar = new Panel();
            topBar.Dock = DockStyle.Top;
            topBar.Height = 58;
            topBar.BackColor = clrTopbar;
            topBar.Paint += (s, e) =>
            {
                using (var b = new LinearGradientBrush(
                    new Rectangle(0, 55, topBar.Width, 3),
                    Color.FromArgb(30, 0, 0, 0), Color.Transparent,
                    LinearGradientMode.Vertical))
                    e.Graphics.FillRectangle(b, 0, 55, topBar.Width, 3);
                using (var b2 = new SolidBrush(clrBlue))
                    e.Graphics.FillRectangle(b2, 0, 0, 4, 58);
            };
            this.Controls.Add(topBar);

            Label brand = new Label();
            brand.Text = "CHOYHUB  POS";
            brand.Font = new Font("Trebuchet MS", 17, FontStyle.Bold);
            brand.ForeColor = Color.White;
            brand.Location = new Point(20, 13);
            brand.AutoSize = true;
            topBar.Controls.Add(brand);

            Panel scanBox = new Panel();
            scanBox.Size = new Size(300, 34);
            scanBox.BackColor = clrBlueLight;
            scanBox.Paint += (s, e) =>
            {
                using (var pen = new Pen(clrBlue))
                    e.Graphics.DrawRectangle(pen, 0, 0, scanBox.Width - 1, scanBox.Height - 1);
            };
            topBar.Controls.Add(scanBox);

            Label scanIcon = new Label { Text = "⊟", Font = new Font("Segoe UI", 13), ForeColor = clrBlue, Location = new Point(6, 5), AutoSize = true };
            scanBox.Controls.Add(scanIcon);

            Label scanLbl = new Label { Text = "Barcode:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = clrTextSub, Location = new Point(30, 10), AutoSize = true };
            scanBox.Controls.Add(scanLbl);

            TextBox txtBarcode = new TextBox();
            txtBarcode.Location = new Point(96, 7);
            txtBarcode.Size = new Size(196, 20);
            txtBarcode.Font = new Font("Segoe UI", 10);
            txtBarcode.BackColor = clrBlueLight;
            txtBarcode.ForeColor = clrText;
            txtBarcode.BorderStyle = BorderStyle.None;
            txtBarcode.KeyDown += (s, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                string bc = txtBarcode.Text.Trim();
                var p = products.FirstOrDefault(x => x.Barcode == bc);
                if (p == null) { ShowToast("Product not found: " + bc, false); }
                else if (p.IsOutOfStock) { ShowToast("Out of stock: " + p.Name, false); }
                else { AddToCart(p, 1); ShowToast("Added: " + p.Name, true); }
                txtBarcode.Clear();
            };
            scanBox.Controls.Add(txtBarcode);

            topBar.Layout += (s, e) => scanBox.Location = new Point(topBar.Width / 2 - 150, 12);

            lblClock = new Label();
            lblClock.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblClock.ForeColor = clrTextSub;
            lblClock.AutoSize = true;
            topBar.Controls.Add(lblClock);
            topBar.Layout += (s, e) => lblClock.Location = new Point(topBar.Width - lblClock.Width - 20, 18);

            lblCartCount = new Label();
            lblCartCount.Text = "Cart: 0 items";
            lblCartCount.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblCartCount.ForeColor = clrMuted;
            lblCartCount.AutoSize = true;
            topBar.Controls.Add(lblCartCount);
            topBar.Layout += (s, e) => lblCartCount.Location = new Point(topBar.Width - 220, 20);
        }

        // ── LEFT SECTION ──────────────────────────────────────
        private void BuildLeftSection()
        {
            Panel left = new Panel();
            left.BackColor = clrSidebar;
            left.Location = new Point(0, 58);
            left.Width = 820;
            left.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            left.Paint += (s, e) =>
            {
                using (var b = new LinearGradientBrush(
                    new Rectangle(left.Width - 3, 0, 3, left.Height),
                    Color.FromArgb(25, 0, 0, 0), Color.Transparent,
                    LinearGradientMode.Horizontal))
                    e.Graphics.FillRectangle(b, left.Width - 3, 0, 3, left.Height);
            };
            this.Controls.Add(left);
            this.Resize += (s, e) => left.Height = this.ClientSize.Height - 58;

            Panel filterRow = new Panel();
            filterRow.Location = new Point(0, 0);
            filterRow.Size = new Size(820, 52);
            filterRow.BackColor = Color.FromArgb(248, 250, 252);
            filterRow.Paint += (s, e) =>
            {
                using (var pen = new Pen(clrBorder))
                    e.Graphics.DrawLine(pen, 0, 51, filterRow.Width, 51);
            };
            left.Controls.Add(filterRow);

            Panel searchWrap = new Panel();
            searchWrap.Location = new Point(12, 10);
            searchWrap.Size = new Size(260, 32);
            searchWrap.BackColor = Color.White;
            searchWrap.Paint += (s, e) =>
            {
                using (var pen = new Pen(clrBorder))
                    e.Graphics.DrawRectangle(pen, 0, 0, searchWrap.Width - 1, searchWrap.Height - 1);
            };
            filterRow.Controls.Add(searchWrap);

            Label srchIcon = new Label { Text = "🔍", Location = new Point(6, 6), AutoSize = true };
            searchWrap.Controls.Add(srchIcon);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(30, 6);
            txtSearch.Size = new Size(222, 20);
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.BackColor = Color.White;
            txtSearch.ForeColor = clrText;
            txtSearch.BorderStyle = BorderStyle.None;
            txtSearch.TextChanged += (s, e) => DisplayProducts();
            searchWrap.Controls.Add(txtSearch);

            string[] cats = { "All", "Drinks", "Snacks", "Fast Food", "Bread", "School" };
            int tx = 282;
            foreach (string cat in cats)
            {
                string c = cat;
                Button tab = new Button();
                tab.Text = cat;
                tab.Size = new Size(cat == "Fast Food" ? 82 : 68, 32);
                tab.Location = new Point(tx, 10);
                tab.FlatStyle = FlatStyle.Flat;
                tab.FlatAppearance.BorderSize = 1;
                tab.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                tab.Cursor = Cursors.Hand;
                tab.Tag = cat;

                SetTabStyle(tab, cat == "All");

                tab.Click += (s, e) =>
                {
                    foreach (Control ctrl in filterRow.Controls)
                        if (ctrl is Button b && b.Tag is string) SetTabStyle(b, (string)b.Tag == c);
                    cmbCategory.SelectedItem = c;
                    DisplayProducts();
                };
                filterRow.Controls.Add(tab);
                tx += tab.Width + 4;
            }

            cmbCategory = new ComboBox { Visible = false };
            foreach (var c in cats) cmbCategory.Items.Add(c);
            cmbCategory.SelectedIndex = 0;
            filterRow.Controls.Add(cmbCategory);

            Panel legendStrip = new Panel();
            legendStrip.Location = new Point(0, 52);
            legendStrip.Size = new Size(820, 28);
            legendStrip.BackColor = Color.FromArgb(252, 252, 253);
            legendStrip.Paint += (s, e) =>
            {
                using (var pen = new Pen(clrBorder))
                    e.Graphics.DrawLine(pen, 0, 27, legendStrip.Width, 27);
                int x = 14;
                DrawLegendDot(e.Graphics, x, 9, clrGreen, "In Stock"); x += 80;
                DrawLegendDot(e.Graphics, x, 9, clrOrange, "Low Stock (≤5)"); x += 104;
                DrawLegendDot(e.Graphics, x, 9, clrRed, "Out of Stock");
            };
            left.Controls.Add(legendStrip);

            productPanel = new FlowLayoutPanel();
            productPanel.Location = new Point(0, 80);
            productPanel.BackColor = clrPageBg;
            productPanel.AutoScroll = true;
            productPanel.Padding = new Padding(10, 10, 0, 10);
            left.Controls.Add(productPanel);

            AttachDragScroll(productPanel);

            left.Resize += (s, e) =>
            {
                filterRow.Width = left.Width;
                legendStrip.Width = left.Width;
                productPanel.Size = new Size(left.Width, left.Height - 80);
            };
        }

        // ═══════════════════════════════════════════════════════
        //  DRAG-SCROLL
        // ═══════════════════════════════════════════════════════
        private void AttachDragScroll(FlowLayoutPanel panel)
        {
            _inertiaTimer = new Timer { Interval = 16 };
            _inertiaTimer.Tick += (s, e) =>
            {
                if (Math.Abs(_velocity) < 0.5f)
                {
                    _inertiaTimer.Stop();
                    _velocity = 0;
                    return;
                }
                int newVal = panel.VerticalScroll.Value - (int)_velocity;
                newVal = Math.Max(panel.VerticalScroll.Minimum,
                         Math.Min(panel.VerticalScroll.Maximum, newVal));
                panel.VerticalScroll.Value = newVal;
                panel.PerformLayout();
                _velocity *= 0.88f;
            };

            HookDragEvents(panel, panel);
            panel.ControlAdded += (s, e) => HookDragEvents(e.Control, panel);
        }

        private void HookDragEvents(Control target, FlowLayoutPanel panel)
        {
            target.MouseDown += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                _isDragging = true;
                _dragConfirmed = false;
                _dragStart = target.PointToScreen(e.Location);
                _scrollStart = panel.VerticalScroll.Value;
                _lastMouseY = _dragStart.Y;
                _lastMoveTime = DateTime.Now;
                _velocity = 0;
                _inertiaTimer.Stop();
                panel.Cursor = Cursors.Hand;
            };

            target.MouseMove += (s, e) =>
            {
                if (!_isDragging) return;
                Point cur = target.PointToScreen(e.Location);
                int dy = _dragStart.Y - cur.Y;

                if (!_dragConfirmed)
                {
                    if (Math.Abs(dy) < 5) return;
                    _dragConfirmed = true;
                }

                double ms = (DateTime.Now - _lastMoveTime).TotalMilliseconds;
                if (ms > 0)
                    _velocity = (float)((_lastMouseY - cur.Y) / ms) * 16f;

                _lastMouseY = cur.Y;
                _lastMoveTime = DateTime.Now;

                int newVal = Math.Max(panel.VerticalScroll.Minimum,
                             Math.Min(panel.VerticalScroll.Maximum, _scrollStart + dy));
                panel.VerticalScroll.Value = newVal;
                panel.PerformLayout();
                panel.Cursor = Cursors.NoMoveVert;
            };

            target.MouseUp += (s, e) =>
            {
                if (!_isDragging) return;
                _isDragging = false;
                panel.Cursor = Cursors.Default;
                if (Math.Abs(_velocity) > 1f)
                    _inertiaTimer.Start();
                panel.BeginInvoke((Action)(() => { _dragConfirmed = false; }));
            };

            foreach (Control child in target.Controls)
                HookDragEvents(child, panel);

            target.ControlAdded += (s, e) =>
            {
                foreach (Control child in e.Control.Controls)
                    HookDragEvents(child, panel);
            };
        }

        private void SetTabStyle(Button tab, bool active)
        {
            tab.BackColor = active ? clrBlue : Color.White;
            tab.ForeColor = active ? Color.White : clrTextSub;
            tab.FlatAppearance.BorderColor = active ? clrBlue : clrBorder;
        }

        private void DrawLegendDot(Graphics g, int x, int y, Color c, string label)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var b = new SolidBrush(c))
                g.FillEllipse(b, x, y, 10, 10);
            using (var b = new SolidBrush(clrTextSub))
            using (var f = new Font("Segoe UI", 8))
                g.DrawString(label, f, b, x + 14, y - 1);
        }

        // ── RIGHT SECTION ─────────────────────────────────────
        private void BuildRightSection()
        {
            rightPanel = new Panel();
            rightPanel.BackColor = clrRightBg;
            rightPanel.Location = new Point(820, 58);
            rightPanel.Width = 620;
            rightPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            this.Controls.Add(rightPanel);
            this.Resize += (s, e) =>
            {
                rightPanel.Location = new Point(this.ClientSize.Width - rightPanel.Width, 58);
                rightPanel.Height = this.ClientSize.Height - 58;
                left_Resize();
            };

            Panel orderHeader = new Panel();
            orderHeader.Location = new Point(0, 0);
            orderHeader.Size = new Size(620, 44);
            orderHeader.BackColor = clrBlue;
            rightPanel.Controls.Add(orderHeader);

            Label lblOrderTitle = new Label { Text = "🛒  ORDER", Font = new Font("Trebuchet MS", 13, FontStyle.Bold), ForeColor = Color.White, Location = new Point(16, 10), AutoSize = true };
            orderHeader.Controls.Add(lblOrderTitle);

            Label lblOrderSub = new Label { Text = DateTime.Now.ToString("MMM dd, yyyy"), Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(200, 220, 255), AutoSize = true };
            orderHeader.Controls.Add(lblOrderSub);
            orderHeader.Layout += (s, e) => lblOrderSub.Location = new Point(orderHeader.Width - lblOrderSub.Width - 14, 14);

            dgvCart = new DataGridView();
            dgvCart.Location = new Point(0, 44);
            dgvCart.Size = new Size(620, 310);
            StyleGrid(dgvCart);

            dgvCart.ColumnCount = 5;
            dgvCart.Columns[0].Name = "Item"; dgvCart.Columns[0].Width = 190;
            dgvCart.Columns[1].Name = "Qty"; dgvCart.Columns[1].Width = 52;
            dgvCart.Columns[2].Name = "Unit Price"; dgvCart.Columns[2].Width = 90;
            dgvCart.Columns[3].Name = "Subtotal"; dgvCart.Columns[3].Width = 95;
            dgvCart.Columns[4].Name = "Actions"; dgvCart.Columns[4].Width = 170;
            dgvCart.Columns[4].DefaultCellStyle.ForeColor = clrBlue;

            dgvCart.CellClick += DgvCart_CellClick;
            dgvCart.CellDoubleClick += DgvCart_CellDoubleClick;
            rightPanel.Controls.Add(dgvCart);

            Label lblGridHint = new Label { Text = "  ➖ / ➕ to adjust qty   |   🗑 to remove   |   double-click to edit", Font = new Font("Segoe UI", 8), ForeColor = clrMuted, Location = new Point(0, 354), Size = new Size(620, 20), TextAlign = ContentAlignment.MiddleLeft };
            rightPanel.Controls.Add(lblGridHint);

            Panel summaryBox = new Panel();
            summaryBox.Location = new Point(10, 378);
            summaryBox.Size = new Size(600, 190);
            summaryBox.BackColor = Color.White;
            summaryBox.Paint += (s, e) =>
            {
                using (var pen = new Pen(clrBorder))
                    e.Graphics.DrawRectangle(pen, 0, 0, summaryBox.Width - 1, summaryBox.Height - 1);
                using (var b = new SolidBrush(clrBlue))
                    e.Graphics.FillRectangle(b, 0, 0, summaryBox.Width, 3);
            };
            rightPanel.Controls.Add(summaryBox);

            Label lblTotalCaption = new Label { Text = "TOTAL", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = clrMuted, Location = new Point(16, 16), AutoSize = true };
            summaryBox.Controls.Add(lblTotalCaption);

            lblTotal = new Label();
            lblTotal.Text = "₱0.00";
            lblTotal.Font = new Font("Trebuchet MS", 28, FontStyle.Bold);
            lblTotal.ForeColor = clrGreen;
            lblTotal.Location = new Point(16, 34);
            lblTotal.AutoSize = true;
            summaryBox.Controls.Add(lblTotal);

            Panel sumDiv = new Panel { Location = new Point(16, 88), Size = new Size(568, 1), BackColor = clrBorder };
            summaryBox.Controls.Add(sumDiv);

            Label lblCashCaption = new Label { Text = "CASH TENDERED", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = clrMuted, Location = new Point(16, 98), AutoSize = true };
            summaryBox.Controls.Add(lblCashCaption);

            Panel cashBorder = new Panel { Location = new Point(16, 116), Size = new Size(214, 42), BackColor = clrBorder };
            summaryBox.Controls.Add(cashBorder);
            Panel cashBg = new Panel { Location = new Point(1, 1), Size = new Size(212, 40), BackColor = Color.White };
            cashBorder.Controls.Add(cashBg);

            Label cashSymbol = new Label { Text = "₱", Font = new Font("Trebuchet MS", 14, FontStyle.Bold), ForeColor = clrTextSub, Location = new Point(6, 8), AutoSize = true };
            cashBg.Controls.Add(cashSymbol);

            txtCash = new TextBox();
            txtCash.Location = new Point(26, 9);
            txtCash.Size = new Size(178, 22);
            txtCash.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            txtCash.BackColor = Color.White;
            txtCash.ForeColor = clrText;
            txtCash.BorderStyle = BorderStyle.None;
            txtCash.TextChanged += (s, e) => UpdateChange();
            txtCash.Enter += (s, e) => cashBorder.BackColor = clrBlue;
            txtCash.Leave += (s, e) => cashBorder.BackColor = clrBorder;
            cashBg.Controls.Add(txtCash);

            Label lblChangeCaption = new Label { Text = "CHANGE", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = clrMuted, Location = new Point(248, 98), AutoSize = true };
            summaryBox.Controls.Add(lblChangeCaption);

            lblChange = new Label();
            lblChange.Text = "₱0.00";
            lblChange.Font = new Font("Trebuchet MS", 20, FontStyle.Bold);
            lblChange.ForeColor = clrBlue;
            lblChange.Location = new Point(248, 118);
            lblChange.AutoSize = true;
            summaryBox.Controls.Add(lblChange);

            int[] quickAmounts = { 20, 50, 100, 200, 500 };
            int qx = 16;
            Label quickLbl = new Label { Text = "Quick:", Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = clrMuted, Location = new Point(qx, 163), AutoSize = true };
            summaryBox.Controls.Add(quickLbl);
            qx += 46;
            foreach (int amt in quickAmounts)
            {
                int a = amt;
                Button qBtn = new Button();
                qBtn.Text = "₱" + amt;
                qBtn.Size = new Size(52, 22);
                qBtn.Location = new Point(qx, 160);
                qBtn.BackColor = clrBlueLight;
                qBtn.ForeColor = clrBlue;
                qBtn.FlatStyle = FlatStyle.Flat;
                qBtn.FlatAppearance.BorderColor = clrBlue;
                qBtn.FlatAppearance.BorderSize = 1;
                qBtn.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                qBtn.Cursor = Cursors.Hand;
                qBtn.Click += (s, e) =>
                {
                    decimal cur;
                    decimal.TryParse(txtCash.Text, out cur);
                    txtCash.Text = (cur + a).ToString();
                };
                summaryBox.Controls.Add(qBtn);
                qx += 56;
            }

            Panel btnRow = new Panel();
            btnRow.Location = new Point(10, 574);
            btnRow.Size = new Size(600, 62);
            btnRow.BackColor = Color.Transparent;
            rightPanel.Controls.Add(btnRow);

            Button btnCheckout = MakeActionButton("✔  CHECKOUT", clrGreen, new Point(0, 0), new Size(390, 58));
            btnCheckout.Click += BtnCheckout_Click;
            btnRow.Controls.Add(btnCheckout);

            Button btnClear = MakeActionButton("✕  CLEAR", clrRed, new Point(398, 0), new Size(202, 58));
            btnClear.Click += BtnClear_Click;
            btnRow.Controls.Add(btnClear);

            rightPanel.Resize += (s, e) =>
            {
                summaryBox.Width = rightPanel.Width - 20;
                sumDiv.Width = summaryBox.Width - 32;
                orderHeader.Width = rightPanel.Width;
                dgvCart.Width = rightPanel.Width;
                lblGridHint.Width = rightPanel.Width;
                btnRow.Width = rightPanel.Width - 20;
                btnCheckout.Width = btnRow.Width - 210;
                btnClear.Location = new Point(btnCheckout.Width + 8, 0);
                btnClear.Width = 200;
            };
        }

        private void left_Resize()
        {
            foreach (Control c in this.Controls)
            {
                if (c is Panel p && p != rightPanel && p.Dock != DockStyle.Top)
                {
                    p.Width = this.ClientSize.Width - rightPanel.Width;
                    foreach (Control inner in p.Controls)
                    {
                        if (inner is FlowLayoutPanel flp) flp.Width = p.Width;
                        if (inner is Panel sub && sub.Name == "") sub.Width = p.Width;
                    }
                }
            }
        }

        // ── TOAST ─────────────────────────────────────────────
        private void BuildToast()
        {
            toastPanel = new Panel();
            toastPanel.Size = new Size(320, 44);
            toastPanel.BackColor = Color.FromArgb(30, 40, 60);
            toastPanel.Visible = false;
            toastPanel.BringToFront();
            this.Controls.Add(toastPanel);

            toastLabel = new Label();
            toastLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            toastLabel.ForeColor = Color.White;
            toastLabel.Dock = DockStyle.Fill;
            toastLabel.TextAlign = ContentAlignment.MiddleCenter;
            toastPanel.Controls.Add(toastLabel);

            toastTimer = new Timer { Interval = 2200 };
            toastTimer.Tick += (s, e) =>
            {
                toastPanel.Visible = false;
                toastTimer.Stop();
            };

            this.Resize += (s, e) =>
                toastPanel.Location = new Point(this.ClientSize.Width / 2 - 160, this.ClientSize.Height - 80);
        }

        private void ShowToast(string msg, bool success)
        {
            toastPanel.BackColor = success ? Color.FromArgb(22, 101, 52) : Color.FromArgb(127, 29, 29);
            toastLabel.Text = (success ? "✔  " : "✕  ") + msg;
            toastPanel.Location = new Point(this.ClientSize.Width / 2 - 160, this.ClientSize.Height - 80);
            toastPanel.BringToFront();
            toastPanel.Visible = true;
            toastTimer.Stop();
            toastTimer.Start();
        }

        private void StartClock()
        {
            Timer clock = new Timer { Interval = 1000 };
            clock.Tick += (s, e) => { if (lblClock != null) lblClock.Text = DateTime.Now.ToString("hh:mm:ss tt"); };
            clock.Start();
            if (lblClock != null) lblClock.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        // ═══════════════════════════════════════════════════════
        //  PRODUCTS
        // ═══════════════════════════════════════════════════════
        private void LoadProducts()
        {
            products.Add(new Product("1001", "Coke", 20, "Drinks", 30));
            products.Add(new Product("1002", "Pepsi", 18, "Drinks", 25));
            products.Add(new Product("1003", "Mountain Dew", 22, "Drinks", 0));
            products.Add(new Product("1004", "Royal", 20, "Drinks", 3));
            products.Add(new Product("1005", "Water", 15, "Drinks", 100));
            products.Add(new Product("1006", "Coffee", 50, "Drinks", 20));
            products.Add(new Product("1007", "Milk Tea", 65, "Drinks", 15));
            products.Add(new Product("1008", "Orange Juice", 45, "Drinks", 8));

            products.Add(new Product("2001", "Piattos", 25, "Snacks", 40));
            products.Add(new Product("2002", "Nova", 20, "Snacks", 0));
            products.Add(new Product("2003", "Vcut", 22, "Snacks", 35));
            products.Add(new Product("2004", "Chippy", 18, "Snacks", 4));
            products.Add(new Product("2005", "Cheezy", 15, "Snacks", 50));
            products.Add(new Product("2006", "Skyflakes", 10, "Snacks", 60));

            products.Add(new Product("3001", "Burger", 80, "Fast Food", 20));
            products.Add(new Product("3002", "Fries", 45, "Fast Food", 30));
            products.Add(new Product("3003", "Hotdog", 35, "Fast Food", 25));
            products.Add(new Product("3004", "Pizza Slice", 90, "Fast Food", 0));
            products.Add(new Product("3005", "Chicken Meal", 120, "Fast Food", 18));
            products.Add(new Product("3006", "Spaghetti", 75, "Fast Food", 12));

            products.Add(new Product("4001", "Bread", 30, "Bread", 50));
            products.Add(new Product("4002", "Donut", 25, "Bread", 2));
            products.Add(new Product("4003", "Croissant", 40, "Bread", 15));
            products.Add(new Product("4004", "Cupcake", 35, "Bread", 20));

            products.Add(new Product("5001", "Notebook", 45, "School", 30));
            products.Add(new Product("5002", "Ballpen", 12, "School", 100));
            products.Add(new Product("5003", "Pencil", 10, "School", 80));
            products.Add(new Product("5004", "Eraser", 8, "School", 60));
            products.Add(new Product("5005", "Ruler", 15, "School", 0));
        }

        private void DisplayProducts()
        {
            productPanel.Controls.Clear();
            string cat = cmbCategory.SelectedItem?.ToString() ?? "All";
            string search = txtSearch?.Text?.ToLower() ?? "";

            var list = products
                .Where(p => cat == "All" || p.Category == cat)
                .Where(p => string.IsNullOrEmpty(search)
                            || p.Name.ToLower().Contains(search)
                            || p.Barcode.Contains(search))
                .ToList();

            foreach (var p in list)
                productPanel.Controls.Add(MakeProductCard(p));
        }

        private Panel MakeProductCard(Product product)
        {
            Panel card = new Panel();
            card.Size = new Size(185, 175);
            card.BackColor = product.IsOutOfStock ? Color.FromArgb(250, 250, 250) : clrCardBg;
            card.Margin = new Padding(5);
            card.Cursor = product.IsOutOfStock ? Cursors.No : Cursors.Hand;
            card.Paint += (s, e) => PaintCard(e.Graphics, card, product);

            if (!product.IsOutOfStock)
            {   
                card.MouseEnter += (s, e) => { card.BackColor = Color.White; card.Refresh(); };
                card.MouseLeave += (s, e) => { card.BackColor = clrCardBg; card.Refresh(); };
            }

            int stockW = product.IsOutOfStock ? 80 : product.IsLowStock ? 66 : 56;
            Panel badge = new Panel { Size = new Size(stockW, 20), Location = new Point(card.Width - stockW - 6, 6) };
            badge.BackColor = product.IsOutOfStock ? clrRedBg :
                              product.IsLowStock ? clrOrangeBg : clrGreenBg;
            badge.Paint += (s, e) =>
            {
                string t = product.IsOutOfStock ? "OUT OF STOCK" :
                            product.IsLowStock ? $"LOW  {product.Stock}" : $"✓ {product.Stock}";
                Color fc = product.IsOutOfStock ? clrRed :
                            product.IsLowStock ? clrOrange : clrGreen;
                using (var f = new Font("Segoe UI", 7, FontStyle.Bold))
                using (var b = new SolidBrush(fc))
                {
                    var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    e.Graphics.DrawString(t, f, b, new Rectangle(0, 0, badge.Width, badge.Height), fmt);
                }
            };
            card.Controls.Add(badge);

            Label catTag = new Label { Text = product.Category.ToUpper(), Font = new Font("Segoe UI", 7, FontStyle.Bold), ForeColor = clrBlue, Location = new Point(10, 10), AutoSize = true };
            Label lblName = new Label { Text = product.Name, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = product.IsOutOfStock ? clrMuted : clrText, Location = new Point(10, 30), Size = new Size(165, 28), AutoEllipsis = true };
            Label lblPrice = new Label { Text = "₱" + product.Price.ToString("N2"), Font = new Font("Trebuchet MS", 14, FontStyle.Bold), ForeColor = product.IsOutOfStock ? clrMuted : clrGreen, Location = new Point(10, 60), AutoSize = true };
            Label lblBarcode = new Label { Text = "# " + product.Barcode, Font = new Font("Segoe UI", 8), ForeColor = clrMuted, Location = new Point(10, 90), AutoSize = true };
            card.Controls.Add(catTag);
            card.Controls.Add(lblName);
            card.Controls.Add(lblPrice);
            card.Controls.Add(lblBarcode);

            if (product.IsOutOfStock)
            {
                Label oosLbl = new Label { Text = "NOT AVAILABLE", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = clrRed, Location = new Point(10, 115), AutoSize = true };
                card.Controls.Add(oosLbl);
            }
            else
            {
                Panel qRow = new Panel { Location = new Point(8, 108), Size = new Size(169, 28), BackColor = Color.Transparent };
                card.Controls.Add(qRow);

                Button bMinus = SmallQtyBtn("−", new Point(0, 0));
                NumericUpDown nud = new NumericUpDown { Location = new Point(28, 2), Size = new Size(50, 24), Minimum = 1, Maximum = product.Stock, Value = 1, Font = new Font("Segoe UI", 10), BackColor = Color.White, ForeColor = clrText, BorderStyle = BorderStyle.FixedSingle, TextAlign = HorizontalAlignment.Center };
                Button bPlus = SmallQtyBtn("+", new Point(80, 0));

                bMinus.Click += (s, e) => { if (_dragConfirmed) return; if (nud.Value > 1) nud.Value--; };
                bPlus.Click += (s, e) => { if (_dragConfirmed) return; if (nud.Value < product.Stock) nud.Value++; };

                qRow.Controls.Add(bMinus);
                qRow.Controls.Add(nud);
                qRow.Controls.Add(bPlus);

                Button btnAdd = new Button { Text = "Add to Cart", Size = new Size(169, 30), Location = new Point(8, 140), BackColor = clrBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
                btnAdd.FlatAppearance.BorderSize = 0;
                btnAdd.MouseEnter += (s, e) => btnAdd.BackColor = clrBlueDark;
                btnAdd.MouseLeave += (s, e) => btnAdd.BackColor = clrBlue;
                btnAdd.Click += (s, e) =>
                {
                    if (_dragConfirmed) return;
                    AddToCart(product, (int)nud.Value);
                    nud.Value = 1;
                };
                card.Controls.Add(btnAdd);
            }

            return card;
        }

        private void PaintCard(Graphics g, Panel card, Product product)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color border = product.IsOutOfStock ? Color.FromArgb(254, 202, 202) :
                           product.IsLowStock ? Color.FromArgb(253, 230, 138) : clrBorder;
            using (var pen = new Pen(border))
                g.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            Color top = product.IsOutOfStock ? clrRed :
                        product.IsLowStock ? clrOrange : clrBlue;
            using (var b = new SolidBrush(top))
                g.FillRectangle(b, 0, 0, card.Width, 3);
        }

        private Button SmallQtyBtn(string text, Point loc)
        {
            var btn = new Button { Text = text, Size = new Size(28, 24), Location = loc, BackColor = Color.FromArgb(239, 246, 255), ForeColor = clrBlue, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderColor = clrBorder;
            btn.FlatAppearance.BorderSize = 1;
            return btn;
        }

        // ═══════════════════════════════════════════════════════
        //  CART LOGIC
        // ═══════════════════════════════════════════════════════
        private void AddToCart(Product product, int qty)
        {
            int inCart = cart.Where(x => x.Barcode == product.Barcode).Sum(x => x.Quantity);
            if (inCart + qty > product.Stock)
            {
                ShowToast($"Only {product.Stock} in stock for {product.Name}", false);
                return;
            }
            CartItem existing = cart.FirstOrDefault(x => x.Barcode == product.Barcode);
            if (existing != null) existing.Quantity += qty;
            else cart.Add(new CartItem { Barcode = product.Barcode, Name = product.Name, Price = product.Price, Quantity = qty });
            RefreshCart();
            ShowToast($"Added {qty}× {product.Name}", true);
        }

        private void RefreshCart()
        {
            dgvCart.Rows.Clear();

            decimal subtotal = 0;

            for (int i = 0; i < cart.Count; i++)
            {
                CartItem item = cart[i];

                int r = dgvCart.Rows.Add(
                    item.Name,
                    item.Quantity,
                    "₱" + item.Price.ToString("N2"),
                    "₱" + item.Subtotal.ToString("N2"),
                    "  ➖   qty   ➕      🗑"
                );

                dgvCart.Rows[r].Tag = item;
                dgvCart.Rows[r].DefaultCellStyle.BackColor =
                    i % 2 == 0 ? Color.White : clrRowAlt;

                subtotal += item.Subtotal;
            }

            decimal tax = subtotal * 0.12m;
            decimal total = subtotal + tax;

            lblTotal.Text = "₱" + total.ToString("N2");

            lblCartCount.Text =
                $"Cart: {cart.Sum(x => x.Quantity)} item(s)";

            UpdateChange();
        }

        private void UpdateChange()
        {
            decimal subtotal = cart.Sum(x => x.Subtotal);
            decimal tax = subtotal * 0.12m;
            decimal total = subtotal + tax;

            if (decimal.TryParse(txtCash.Text, out decimal cash))
            {
                decimal ch = cash - total;

                lblChange.Text = "₱" +
                    (ch >= 0 ? ch : 0).ToString("N2");

                lblChange.ForeColor =
                    ch >= 0 ? clrGreen : clrRed;
            }
            else
            {
                lblChange.Text = "₱0.00";
                lblChange.ForeColor = clrBlue;
            }
        }

        private void DgvCart_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 4) return;
            Rectangle cellRect = dgvCart.GetCellDisplayRectangle(4, e.RowIndex, false);
            int relX = dgvCart.PointToClient(Cursor.Position).X - cellRect.X;
            int w = cellRect.Width;

            CartItem item = dgvCart.Rows[e.RowIndex].Tag as CartItem;
            if (item == null) return;
            Product prod = products.FirstOrDefault(x => x.Barcode == item.Barcode);

            if (relX < w * 0.22)
            { if (item.Quantity > 1) item.Quantity--; else cart.Remove(item); RefreshCart(); }
            else if (relX > w * 0.40 && relX < w * 0.66)
            {
                if (prod != null && item.Quantity < prod.Stock) { item.Quantity++; RefreshCart(); }
                else ShowToast("Max stock reached", false);
            }
            else if (relX > w * 0.78)
            { cart.Remove(item); RefreshCart(); }
        }

        private void DgvCart_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            CartItem item = dgvCart.Rows[e.RowIndex].Tag as CartItem;
            if (item == null) return;
            Product prod = products.FirstOrDefault(x => x.Barcode == item.Barcode);
            ShowQtyDialog(item, prod?.Stock ?? 999);
        }

        private void ShowQtyDialog(CartItem item, int maxStock)
        {
            Form dlg = new Form { Text = "Edit Quantity", Size = new Size(300, 190), StartPosition = FormStartPosition.CenterParent, BackColor = Color.White, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false };

            new Label { Text = item.Name, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = clrText, Location = new Point(20, 18), AutoSize = true }.Bind(dlg);
            new Label { Text = "Quantity (max " + maxStock + "):", Font = new Font("Segoe UI", 9), ForeColor = clrTextSub, Location = new Point(20, 46), AutoSize = true }.Bind(dlg);

            NumericUpDown nud = new NumericUpDown { Location = new Point(20, 66), Size = new Size(100, 30), Minimum = 1, Maximum = maxStock, Value = item.Quantity, Font = new Font("Segoe UI", 13), BorderStyle = BorderStyle.FixedSingle };
            dlg.Controls.Add(nud);

            Button ok = MakeActionButton("Apply", clrBlue, new Point(20, 108), new Size(120, 36));
            Button del = MakeActionButton("Remove", clrRed, new Point(152, 108), new Size(120, 36));
            ok.Click += (s, e) => { item.Quantity = (int)nud.Value; RefreshCart(); dlg.Close(); };
            del.Click += (s, e) => { cart.Remove(item); RefreshCart(); dlg.Close(); };
            dlg.Controls.Add(ok);
            dlg.Controls.Add(del);
            dlg.ShowDialog(this);
        }

        // ═══════════════════════════════════════════════════════
        //  CHECKOUT / CLEAR
        // ═══════════════════════════════════════════════════════
        private void BtnCheckout_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0)
            {
                ShowToast("Cart is empty!", false);
                return;
            }

            decimal subtotal = cart.Sum(x => x.Subtotal);
            decimal tax = subtotal * 0.12m;
            decimal total = subtotal + tax;

            if (!decimal.TryParse(txtCash.Text, out decimal cash))
            {
                ShowToast("Enter a valid cash amount.", false);
                return;
            }

            if (cash < total)
            {
                ShowToast($"Need ₱{(total - cash):N2} more.", false);
                return;
            }

            decimal change = cash - total;

            lblChange.Text = "₱" + change.ToString("N2");

            foreach (CartItem ci in cart)
            {
                Product p = products.FirstOrDefault(
                    x => x.Barcode == ci.Barcode);

                if (p != null)
                    p.Stock -= ci.Quantity;
            }

            ShowReceiptDialog(subtotal, tax, total, cash, change);

            cart.Clear();

            RefreshCart();

            txtCash.Clear();

            DisplayProducts();
        }

        // ═══════════════════════════════════════════════════════
        //  RECEIPT DIALOG  (thermal-style, matches uploaded image)
        // ═══════════════════════════════════════════════════════
        private void ShowReceiptDialog(
        decimal subtotal,
        decimal tax,
        decimal total,
        decimal cash,
        decimal change)
        {
            // Snapshot cart BEFORE the caller clears it
            var snapshot = cart.ToList();

            // ── Dialog window ──────────────────────────────────
            Form dlg = new Form
            {
                Text = "Receipt — CHOYHUB POS",
                Size = new Size(370, 660),
                MinimumSize = new Size(340, 560),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            // Bottom button bar (must be added before the scroll panel so z-order is correct)
            Panel btnBar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            btnBar.Paint += (s, e) =>
            {
                using (var pen = new Pen(clrBorder))
                    e.Graphics.DrawLine(pen, 0, 0, btnBar.Width, 0);
            };
            dlg.Controls.Add(btnBar);

            Button btnPrint = MakeActionButton("🖨  Print", clrBlue, new Point(10, 10), new Size(140, 36));
            btnPrint.Click += (s, e) =>
    PrintReceipt(snapshot, subtotal, tax, total, cash, change);

            Button btnClose = MakeActionButton("✕  Close", clrRed, new Point(160, 10), new Size(140, 36));
            btnClose.Click += (s, e) => dlg.Close();
            btnBar.Controls.Add(btnClose);

            btnBar.Resize += (s, e) =>
            {
                btnClose.Location = new Point(btnBar.Width - 150, 10);
                btnPrint.Width = btnBar.Width - 170;
            };

            // Scrollable receipt area
            Panel receiptScroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(230, 230, 230)   // grey surround like a receipt printer
            };
            dlg.Controls.Add(receiptScroll);

            // White paper panel (centred inside scroll area)
            Panel paper = new Panel
            {
                Width = 300,
                BackColor = Color.White,
                Location = new Point(20, 16)
            };
            // Subtle drop shadow
            paper.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(200, 200, 200)))
                    e.Graphics.DrawRectangle(pen, 0, 0, paper.Width - 1, paper.Height - 1);
            };
            receiptScroll.Controls.Add(paper);
            receiptScroll.Resize += (s, e) =>
                paper.Location = new Point(Math.Max(0, (receiptScroll.Width - paper.Width) / 2), 16);

            // ── Build receipt content ──────────────────────────
            int y = 18;

            // Store name
            y = RC_CenterLabel(paper, "CHOYHUB", y,
                new Font("Trebuchet MS", 16, FontStyle.Bold), clrText);

            y = RC_CenterLabel(paper, "Point of Sale System", y + 2,
                new Font("Segoe UI", 8, FontStyle.Italic), clrTextSub);

            // Store info
            y = RC_CenterLabel(paper, "City Index - 2025", y + 4,
                new Font("Segoe UI", 8), clrMuted);
            y = RC_CenterLabel(paper, "Tel: +456-468-987-02", y + 1,
                new Font("Segoe UI", 8), clrMuted);

            y = RC_DashedLine(paper, y + 10);

            // Cashier row
            string txnNo = new Random().Next(1, 99).ToString();
            y = RC_TwoCol(paper, "Cashier: Admin",
                          "#" + txnNo, y,
                          new Font("Segoe UI", 8), clrTextSub, clrTextSub);
            y = RC_TwoCol(paper, "Manager: Eric Steer",
                          DateTime.Now.ToString("MM/dd/yyyy"), y,
                          new Font("Segoe UI", 8), clrTextSub, clrTextSub);

            y = RC_DashedLine(paper, y + 6);

            // Column header
            y = RC_ItemRow(paper, "Name", "Qty", "Price", y,
                new Font("Segoe UI", 8, FontStyle.Bold), clrTextSub, isHeader: true);

            y = RC_DashedLine(paper, y + 2);

            // Line items
            foreach (var item in snapshot)
            {
                y = RC_ItemRow(paper,
                    item.Name,
                    item.Quantity.ToString(),
                    "₱" + item.Subtotal.ToString("N2"),
                    y,
                    new Font("Segoe UI", 9), clrText);
            }

            y = RC_DashedLine(paper, y + 6);

            // Sub Total
            // Subtotal
            y = RC_TwoCol(
                paper,
                "Subtotal",
                "₱" + subtotal.ToString("N2"),
                y + 4,
                new Font("Segoe UI", 10, FontStyle.Bold),
                clrText,
                clrText);

            // Tax
            y = RC_TwoCol(
                paper,
                "VAT (12%)",
                "₱" + tax.ToString("N2"),
                y,
                new Font("Segoe UI", 10, FontStyle.Bold),
                clrOrange,
                clrOrange);

            // Final Total
            y = RC_TwoCol(
                paper,
                "TOTAL",
                "₱" + total.ToString("N2"),
                y + 4,
                new Font("Segoe UI", 12, FontStyle.Bold),
                clrGreen,
                clrGreen);

            y = RC_DashedLine(paper, y + 6);

            // Cash / Change
            y = RC_TwoCol(paper, "CASH",
                          "₱" + cash.ToString("N2"), y + 4,
                          new Font("Segoe UI", 10, FontStyle.Bold), clrText, clrText);
            y = RC_TwoCol(paper, "CHANGE",
                          "₱" + change.ToString("N2"), y,
                          new Font("Segoe UI", 10, FontStyle.Bold), clrGreen, clrGreen);

            y = RC_DashedLine(paper, y + 10);

            // Simulated barcode
            Panel barcodeBox = new Panel
            {
                Location = new Point(20, y + 6),
                Size = new Size(paper.Width - 40, 46),
                BackColor = Color.White
            };
            barcodeBox.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.None;
                Random rnd = new Random(99);   // fixed seed → stable bars on every repaint
                int bx = 0;
                while (bx < barcodeBox.Width)
                {
                    int bw = rnd.Next(1, 4);
                    if (rnd.Next(2) == 0)
                        g.FillRectangle(Brushes.Black, bx, 0, bw, barcodeBox.Height - 14);
                    bx += bw;
                }
                using (var f = new Font("Courier New", 7))
                using (var b = new SolidBrush(clrMuted))
                {
                    var fmt = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString("1001 2025 0516 0001", f, b,
                        new Rectangle(0, barcodeBox.Height - 14, barcodeBox.Width, 14), fmt);
                }
            };
            paper.Controls.Add(barcodeBox);
            y += barcodeBox.Height + 14;

            // Thank you
            y = RC_CenterLabel(paper, "THANK YOU!", y,
                new Font("Trebuchet MS", 13, FontStyle.Bold), clrText);
            y = RC_CenterLabel(paper, "Glad to see you again!", y + 2,
                new Font("Segoe UI", 9, FontStyle.Italic), clrMuted);

            // Serrated bottom edge
            Panel serrated = new Panel
            {
                Location = new Point(0, y + 10),
                Size = new Size(paper.Width, 14),
                BackColor = Color.White
            };
            serrated.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                for (int xi = 0; xi < serrated.Width; xi += 14)
                {
                    g.FillPolygon(new SolidBrush(Color.FromArgb(230, 230, 230)),
                        new Point[] {
                            new Point(xi, 0),
                            new Point(xi + 7, 12),
                            new Point(xi + 14, 0)
                        });
                }
            };
            paper.Controls.Add(serrated);
            y += serrated.Height + 10;

            // Fix paper height
            paper.Height = y + 10;

            dlg.ShowDialog(this);
        }

        // ── Receipt layout helpers ─────────────────────────────

        private int RC_CenterLabel(Panel parent, string text, int y, Font font, Color color)
        {
            Label lbl = new Label
            {
                Text = text,
                Font = font,
                ForeColor = color,
                Location = new Point(0, y),
                Size = new Size(parent.Width, font.Height + 6),
                TextAlign = ContentAlignment.MiddleCenter
            };
            parent.Controls.Add(lbl);
            return y + lbl.Height;
        }

        private int RC_DashedLine(Panel parent, int y)
        {
            Panel line = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(parent.Width, 12),
                BackColor = Color.White
            };
            line.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(180, 180, 180), 1)
                { DashStyle = DashStyle.Dash })
                    e.Graphics.DrawLine(pen, 8, 6, parent.Width - 8, 6);
            };
            parent.Controls.Add(line);
            return y + 14;
        }

        private int RC_TwoCol(Panel parent, string left, string right, int y,
            Font font, Color leftColor, Color rightColor)
        {
            int rowH = font.Height + 8;
            int half = parent.Width / 2;

            Label lblL = new Label
            {
                Text = left,
                Font = font,
                ForeColor = leftColor,
                Location = new Point(10, y),
                Size = new Size(half + 10, rowH),
                TextAlign = ContentAlignment.MiddleLeft
            };
            Label lblR = new Label
            {
                Text = right,
                Font = font,
                ForeColor = rightColor,
                Location = new Point(half - 10, y),
                Size = new Size(half + 0, rowH),
                TextAlign = ContentAlignment.MiddleRight
            };
            parent.Controls.Add(lblL);
            parent.Controls.Add(lblR);
            return y + rowH;
        }

        private int RC_ItemRow(Panel parent, string name, string qty, string price,
            int y, Font font, Color color, bool isHeader = false)
        {
            int rowH = font.Height + 8;

            if (isHeader)
            {
                Panel hdrBg = new Panel
                {
                    Location = new Point(0, y - 2),
                    Size = new Size(parent.Width, rowH + 4),
                    BackColor = Color.FromArgb(245, 247, 251)
                };
                parent.Controls.Add(hdrBg);
            }

            // Name (~55% width)
            Label lblN = new Label
            {
                Text = name,
                Font = font,
                ForeColor = color,
                Location = new Point(10, y),
                Size = new Size(parent.Width - 108, rowH),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };
            // Qty (fixed 30px)
            Label lblQ = new Label
            {
                Text = qty,
                Font = font,
                ForeColor = color,
                Location = new Point(parent.Width - 106, y),
                Size = new Size(38, rowH),
                TextAlign = ContentAlignment.MiddleCenter
            };
            // Price (right-aligned ~30%)
            Label lblP = new Label
            {
                Text = price,
                Font = font,
                ForeColor = color,
                Location = new Point(parent.Width - 66, y),
                Size = new Size(58, rowH),
                TextAlign = ContentAlignment.MiddleRight
            };
            parent.Controls.Add(lblN);
            parent.Controls.Add(lblQ);
            parent.Controls.Add(lblP);
            return y + rowH;
        }

        // ── Windows print support ──────────────────────────────
        private void PrintReceipt(
        List<CartItem> items,
        decimal subtotal,
        decimal tax,
        decimal total,
        decimal cash,
        decimal change)
        {
            var pd = new System.Drawing.Printing.PrintDocument();
            pd.DefaultPageSettings.PaperSize =
                new System.Drawing.Printing.PaperSize("Receipt", 315, 900); // ~80mm thermal

            pd.PrintPage += (s, e) =>
            {
                var g = e.Graphics;
                var mono = new Font("Courier New", 8);
                var bold = new Font("Courier New", 9, FontStyle.Bold);
                var big = new Font("Courier New", 12, FontStyle.Bold);
                int py = 10;

                void Line(string text, Font f, bool center = false)
                {
                    float fx = center
                        ? (e.PageBounds.Width - g.MeasureString(text, f).Width) / 2f
                        : 10f;
                    g.DrawString(text, f, Brushes.Black, fx, py);
                    py += f.Height + 3;
                }
                void Dash() { Line(new string('-', 38), mono); }

                Line("CHOYHUB POS SYSTEM", big, center: true);
                Line("City Index - 2025", mono, center: true);
                Line("Tel: +456-468-987-02", mono, center: true);
                Dash();
                Line($"Cashier: Admin                 #{new Random().Next(1, 99)}", mono);
                Line($"Manager: Eric Steer", mono);
                Line($"Date: {DateTime.Now:MM/dd/yyyy  hh:mm tt}", mono);
                Dash();
                Line($"{"Name",-18} {"Qty",3} {"Price",8}", bold);
                Dash();
                foreach (var it in items)
                    Line($"{it.Name,-18} {it.Quantity,3} {"₱" + it.Subtotal.ToString("N2"),8}", mono);
                Dash();
                Line($"{"Subtotal",-22} {"₱" + subtotal.ToString("N2"),8}", bold);
                Line($"{"VAT 12%",-22} {"₱" + tax.ToString("N2"),8}", bold);
                Line($"{"TOTAL",-22} {"₱" + total.ToString("N2"),8}", big);
                Dash();
                Line($"{"CASH",-22} {"₱" + cash.ToString("N2"),8}", bold);
                Line($"{"CHANGE",-22} {"₱" + change.ToString("N2"),8}", bold);
                Dash();
                Line("THANK YOU!", big, center: true);
                Line("Glad to see you again!", mono, center: true);

                mono.Dispose(); bold.Dispose(); big.Dispose();
            };

            using (var dlg2 = new PrintDialog { Document = pd })
            {
                if (dlg2.ShowDialog() == DialogResult.OK)
                    pd.Print();
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0) return;
            if (MessageBox.Show("Clear all items?", "Confirm Clear",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cart.Clear(); RefreshCart(); txtCash.Clear();
                lblChange.Text = "₱0.00";
                lblChange.ForeColor = clrBlue;
            }
        }

        // ═══════════════════════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════════════════════
        private Button MakeActionButton(string text, Color bg, Point loc, Size sz)
        {
            var btn = new Button
            {
                Text = text,
                Size = sz,
                Location = loc,
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            Color hover = ControlPaint.Dark(bg, 0.08f);
            btn.MouseEnter += (s, e) => btn.BackColor = hover;
            btn.MouseLeave += (s, e) => btn.BackColor = bg;
            return btn;
        }

        private void StyleGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.GridColor = clrBorder;
            dgv.BorderStyle = BorderStyle.None;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = clrText;
            dgv.DefaultCellStyle.SelectionBackColor = clrBlueLight;
            dgv.DefaultCellStyle.SelectionForeColor = clrBlue;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.Padding = new Padding(4, 6, 4, 6);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = clrTextSub;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 36;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.RowTemplate.Height = 42;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ScrollBars = ScrollBars.Vertical;
        }
    }

    // ── Extension helpers ──────────────────────────────────────
    public static class ControlExtensions
    {
        public static T Bind<T>(this T control, Control parent) where T : Control
        {
            parent.Controls.Add(control);
            return control;
        }

        public static Label BindRight<T>(this Label lbl, T parent, int rightMargin, int y) where T : Control
        {
            lbl.AutoSize = true;
            parent.Controls.Add(lbl);
            parent.Layout += (s, e) => lbl.Location = new Point(parent.Width - lbl.Width - rightMargin, y);
            return lbl;
        }
    }
}