using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimplePOS
{
    public class InventoryForm : Form
    {
        // ── Product List Data Source ──────────────────
        private List<Product> _products;

        // ── Modern Color Palette ──────────────────────
        private readonly Color clrPageBg = Color.FromArgb(241, 245, 249);
        private readonly Color clrTopbar = Color.White;
        private readonly Color clrBlue = Color.FromArgb(37, 99, 235);
        private readonly Color clrBlueLight = Color.FromArgb(239, 246, 255);
        private readonly Color clrGreen = Color.FromArgb(22, 163, 74);
        private readonly Color clrGreenBg = Color.FromArgb(220, 252, 231);
        private readonly Color clrRed = Color.FromArgb(220, 38, 38);
        private readonly Color clrRedBg = Color.FromArgb(254, 226, 226);
        private readonly Color clrOrange = Color.FromArgb(217, 119, 6);
        private readonly Color clrOrangeBg = Color.FromArgb(254, 243, 199);
        private readonly Color clrText = Color.FromArgb(15, 23, 42);
        private readonly Color clrTextSub = Color.FromArgb(71, 85, 105);
        private readonly Color clrMuted = Color.FromArgb(148, 163, 184);
        private readonly Color clrBorder = Color.FromArgb(226, 232, 240);
        private readonly Color clrRowAlt = Color.FromArgb(248, 250, 252);

        // ── UI Controls ───────────────────────────────
        private DataGridView dgvInventory;
        private TextBox txtSearch;
        private ComboBox cmbCategory;
        private ComboBox cmbStockFilter;
        private Label lblSummaryTotal;
        private Label lblSummaryLow;
        private Label lblSummaryOut;
        private Panel toastPanel;
        private Label toastLabel;
        private Timer toastTimer;

        private static readonly string[] Categories = { "Drinks", "Snacks", "Fast Food", "Bread", "School" };

        public InventoryForm(List<Product> products)
        {
            _products = products;
            InitializeComponent();
            this.DoubleBuffered = true;
            BuildUI();
            RefreshGrid();
            BuildToast();
        }

        private void InitializeComponent()
        {
            this.Text = "CHOYHUB POS — Inventory Management";
            this.Size = new Size(1200, 800);
            this.MinimumSize = new Size(1050, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = clrPageBg;
        }

        private void BuildUI()
        {
            // Grid is built first so docking layers construct cleanly
            BuildGrid();
            BuildTopBar();
            BuildSummaryStrip();
            BuildToolbar();
        }

        // ── TOP NAVIGATION BAR ────────────────────────
        private void BuildTopBar()
        {
            Panel topBar = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = clrTopbar };
            topBar.Paint += (s, e) => {
                using (var pen = new Pen(clrBorder, 1))
                    e.Graphics.DrawLine(pen, 0, topBar.Height - 1, topBar.Width, topBar.Height - 1);
            };
            this.Controls.Add(topBar);

            Label brand = new Label
            {
                Text = "📦  Inventory Dashboard",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = clrText,
                Location = new Point(20, 16),
                AutoSize = true
            };
            topBar.Controls.Add(brand);

            Button btnClose = MakeButton("✕  Back to POS", clrTextSub, new Point(0, 12), new Size(130, 36));
            btnClose.BackColor = Color.FromArgb(241, 245, 249);
            btnClose.ForeColor = clrText;
            btnClose.Click += (s, e) => this.Close();
            topBar.Controls.Add(btnClose);

            topBar.Layout += (s, e) => btnClose.Location = new Point(topBar.Width - 150, 12);
        }

        // ── STATS CARDS STRIP ─────────────────────────
        private void BuildSummaryStrip()
        {
            Panel strip = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = clrPageBg, Padding = new Padding(16, 12, 16, 12) };
            this.Controls.Add(strip);

            lblSummaryTotal = MakeSummaryCard(strip, 0, "TOTAL PRODUCTS", "0", clrBlue);
            lblSummaryLow = MakeSummaryCard(strip, 230, "LOW STOCK ALERTS", "0", clrOrange);
            lblSummaryOut = MakeSummaryCard(strip, 460, "OUT OF STOCK", "0", clrRed);
        }

        private Label MakeSummaryCard(Panel parent, int x, string caption, string val, Color accent)
        {
            Panel card = new Panel { Location = new Point(x + 20, 10), Size = new Size(210, 60), BackColor = Color.White };
            card.Paint += (s, e) => {
                using (var pen = new Pen(clrBorder))
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                using (var b = new SolidBrush(accent))
                    e.Graphics.FillRectangle(b, 0, 0, 4, card.Height);
            };
            parent.Controls.Add(card);

            Label capLbl = new Label { Text = caption, Font = new Font("Segoe UI", 7.5f, FontStyle.Bold), ForeColor = clrMuted, Location = new Point(16, 10), AutoSize = true };
            card.Controls.Add(capLbl);

            Label valLbl = new Label { Text = val, Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = clrText, Location = new Point(14, 24), AutoSize = true };
            card.Controls.Add(valLbl);
            return valLbl;
        }

        // ── FILTER & SEARCH TOOLBAR ────────────────────
        private void BuildToolbar()
        {
            Panel toolbar = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Color.White, Padding = new Padding(20, 0, 20, 0) };
            toolbar.Paint += (s, e) => {
                using (var pen = new Pen(clrBorder))
                    e.Graphics.DrawLine(pen, 0, toolbar.Height - 1, toolbar.Width, toolbar.Height - 1);
            };
            this.Controls.Add(toolbar);

            Panel searchWrap = new Panel { Location = new Point(20, 12), Size = new Size(260, 32), BackColor = Color.FromArgb(248, 250, 252) };
            searchWrap.Paint += (s, e) => {
                using (var pen = new Pen(clrBorder))
                    e.Graphics.DrawRectangle(pen, 0, 0, searchWrap.Width - 1, searchWrap.Height - 1);
            };
            toolbar.Controls.Add(searchWrap);

            Label iconLabel = new Label { Text = "🔍", Location = new Point(8, 7), AutoSize = true, ForeColor = clrMuted };
            searchWrap.Controls.Add(iconLabel);

            txtSearch = new TextBox { Location = new Point(32, 7), Size = new Size(215, 20), Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(248, 250, 252), ForeColor = clrText };
            txtSearch.TextChanged += (s, e) => RefreshGrid();
            searchWrap.Controls.Add(txtSearch);

            Label lblCat = new Label { Text = "Category:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = clrTextSub, Location = new Point(300, 19), AutoSize = true };
            toolbar.Controls.Add(lblCat);

            cmbCategory = new ComboBox { Location = new Point(365, 15), Size = new Size(130, 25), Font = new Font("Segoe UI", 9), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCategory.Items.Add("All Categories");
            foreach (var c in Categories) cmbCategory.Items.Add(c);
            cmbCategory.SelectedIndex = 0;
            cmbCategory.SelectedIndexChanged += (s, e) => RefreshGrid();
            toolbar.Controls.Add(cmbCategory);

            Label lblStock = new Label { Text = "Stock Status:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = clrTextSub, Location = new Point(515, 19), AutoSize = true };
            toolbar.Controls.Add(lblStock);

            cmbStockFilter = new ComboBox { Location = new Point(600, 15), Size = new Size(130, 25), Font = new Font("Segoe UI", 9), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStockFilter.Items.AddRange(new object[] { "All Statuses", "In Stock", "Low Stock (≤5)", "Out of Stock" });
            cmbStockFilter.SelectedIndex = 0;
            cmbStockFilter.SelectedIndexChanged += (s, e) => RefreshGrid();
            toolbar.Controls.Add(cmbStockFilter);

            Button btnAdd = MakeButton("＋ Add Product", clrGreen, new Point(0, 12), new Size(130, 32));
            btnAdd.Click += BtnAddProduct_Click;
            toolbar.Controls.Add(btnAdd);

            Button btnRestock = MakeButton("↑ Restock Low", clrOrange, new Point(0, 12), new Size(130, 32));
            btnRestock.Click += BtnRestockAll_Click;
            toolbar.Controls.Add(btnRestock);

            toolbar.Layout += (s, e) => {
                btnAdd.Location = new Point(toolbar.Width - 150, 12);
                btnRestock.Location = new Point(toolbar.Width - 290, 12);
            };
        }

        // ── CORE DATA GRIDVIEW ─────────────────────────
        private void BuildGrid()
        {
            dgvInventory = new DataGridView { Dock = DockStyle.Fill, Margin = new Padding(20) };
            StyleGrid(dgvInventory);

            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBarcode", HeaderText = "BARCODE", Width = 90 });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colName", HeaderText = "PRODUCT NAME", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCat", HeaderText = "CATEGORY", Width = 120 });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPrice", HeaderText = "PRICE", Width = 110, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStock", HeaderText = "QTY", Width = 80, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "STATUS", Width = 130 });

            // Management Action Buttons
            var btnEdit = new DataGridViewButtonColumn { Name = "btnEditAction", HeaderText = "", Text = "Edit", UseColumnTextForButtonValue = true, Width = 70, FlatStyle = FlatStyle.Flat, DefaultCellStyle = { BackColor = Color.White, ForeColor = clrBlue, Font = new Font("Segoe UI", 9, FontStyle.Bold) } };
            dgvInventory.Columns.Add(btnEdit);

            var btnStock = new DataGridViewButtonColumn { Name = "btnStockAction", HeaderText = "", Text = "+ Stock", UseColumnTextForButtonValue = true, Width = 85, FlatStyle = FlatStyle.Flat, DefaultCellStyle = { BackColor = Color.White, ForeColor = clrGreen, Font = new Font("Segoe UI", 9, FontStyle.Bold) } };
            dgvInventory.Columns.Add(btnStock);

            var btnDel = new DataGridViewButtonColumn { Name = "btnDeleteAction", HeaderText = "", Text = "🗑", UseColumnTextForButtonValue = true, Width = 50, FlatStyle = FlatStyle.Flat, DefaultCellStyle = { BackColor = Color.White, ForeColor = clrRed } };
            dgvInventory.Columns.Add(btnDel);

            // Connect UI logic events 
            dgvInventory.CellPainting += DgvInventory_CellPainting;
            dgvInventory.CellClick += dgvInventory_CellClick;

            this.Controls.Add(dgvInventory);
            dgvInventory.SendToBack(); // Forces grid to snap underneath toolbars cleanly
        }

        // ── LIVE GRID FILTERING & BINDING ──────────────
        public void RefreshGrid()
        {
            string search = txtSearch?.Text?.ToLower() ?? "";
            string cat = cmbCategory?.SelectedItem?.ToString() ?? "All Categories";
            string stockF = cmbStockFilter?.SelectedItem?.ToString() ?? "All Statuses";

            var filtered = _products
                .Where(p => cat == "All Categories" || p.Category == cat)
                .Where(p => string.IsNullOrEmpty(search) || p.Name.ToLower().Contains(search) || p.Barcode.Contains(search))
                .Where(p => {
                    if (stockF == "In Stock") return !p.IsOutOfStock && !p.IsLowStock;
                    if (stockF == "Low Stock (≤5)") return p.IsLowStock;
                    if (stockF == "Out of Stock") return p.IsOutOfStock;
                    return true;
                }).ToList();

            dgvInventory.Rows.Clear();

            for (int i = 0; i < filtered.Count; i++)
            {
                var p = filtered[i];
                string status = p.IsOutOfStock ? "Out of Stock" : p.IsLowStock ? $"Low Stock ({p.Stock})" : "In Stock";

                int r = dgvInventory.Rows.Add(p.Barcode, p.Name, p.Category, "₱" + p.Price.ToString("N2"), p.Stock, status);
                dgvInventory.Rows[r].Tag = p;
                dgvInventory.Rows[r].DefaultCellStyle.BackColor = i % 2 == 0 ? Color.White : clrRowAlt;
            }

            if (lblSummaryTotal != null)
            {
                lblSummaryTotal.Text = _products.Count.ToString();
                lblSummaryLow.Text = _products.Count(p => p.IsLowStock).ToString();
                lblSummaryOut.Text = _products.Count(p => p.IsOutOfStock).ToString();
            }
        }

        // ── CUSTOM PILL BADGE DESIGNER ────────────────
        private void DgvInventory_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dgvInventory.Columns[e.ColumnIndex].Name == "colStatus")
            {
                if (!(dgvInventory.Rows[e.RowIndex].Tag is Product p)) return;

                e.PaintBackground(e.ClipBounds, true);

                Color bg = p.IsOutOfStock ? clrRedBg : p.IsLowStock ? clrOrangeBg : clrGreenBg;
                Color fc = p.IsOutOfStock ? clrRed : p.IsLowStock ? clrOrange : clrGreen;

                Rectangle pill = new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + (e.CellBounds.Height - 24) / 2, e.CellBounds.Width - 24, 24);

                using (var b = new SolidBrush(bg)) e.Graphics.FillRectangle(b, pill);
                using (var f = new Font("Segoe UI", 8.5f, FontStyle.Bold))
                using (var sb = new SolidBrush(fc))
                {
                    var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    e.Graphics.DrawString((string)e.Value, f, sb, pill, fmt);
                }
                e.Handled = true;
            }
        }

        // ── GRID ACTION ROUTER ────────────────────────
        private void dgvInventory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (!(dgvInventory.Rows[e.RowIndex].Tag is Product p)) return;

            string colName = dgvInventory.Columns[e.ColumnIndex].Name;

            if (colName == "btnEditAction") ShowEditDialog(p);
            else if (colName == "btnStockAction") ShowRestockDialog(p);
            else if (colName == "btnDeleteAction") ConfirmDelete(p);
        }

        // ── OPERATIONS DIALOGUES ──────────────────────
        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            Form dlg = MakeDialog("Add New Product", 400, 380);
            int y = 20;

            y = DlgLabel(dlg, "Barcode", y); TextBox txtBarcode = DlgTextBox(dlg, y, ""); y += 54;
            y = DlgLabel(dlg, "Product Name", y); TextBox txtName = DlgTextBox(dlg, y, ""); y += 54;

            y = DlgLabel(dlg, "Category", y);
            ComboBox cmbCat = new ComboBox { Location = new Point(20, y + 22), Size = new Size(350, 30), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var c in Categories) cmbCat.Items.Add(c);
            cmbCat.SelectedIndex = 0; dlg.Controls.Add(cmbCat); y += 54;

            y = DlgLabel(dlg, "Price (₱)", y); TextBox txtPrice = DlgTextBox(dlg, y, "0.00"); y += 54;

            y = DlgLabel(dlg, "Initial Stock", y);
            NumericUpDown nudStock = new NumericUpDown { Location = new Point(20, y + 22), Size = new Size(180, 30), Minimum = 0, Maximum = 9999, Value = 50, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
            dlg.Controls.Add(nudStock); y += 64;

            Button btnSave = MakeButton("✔  Add Product", clrGreen, new Point(20, y), new Size(170, 38));
            Button btnCancel = MakeButton("✕  Cancel", clrRed, new Point(200, y), new Size(170, 38));

            btnSave.Click += (s2, e2) => {
                string bc = txtBarcode.Text.Trim(); string name = txtName.Text.Trim();
                if (string.IsNullOrEmpty(bc) || string.IsNullOrEmpty(name)) { ShowToast("Barcode and Name required.", false); return; }
                if (_products.Any(x => x.Barcode == bc)) { ShowToast("Barcode already exists.", false); return; }
                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0) { ShowToast("Enter valid price.", false); return; }

                _products.Add(new Product(bc, name, price, cmbCat.SelectedItem.ToString(), (int)nudStock.Value));
                RefreshGrid(); ShowToast($"'{name}' added!", true); dlg.Close();
            };
            btnCancel.Click += (s2, e2) => dlg.Close();

            dlg.Controls.Add(btnSave); dlg.Controls.Add(btnCancel);
            dlg.ShowDialog(this);
        }

        private void ShowEditDialog(Product p)
        {
            Form dlg = MakeDialog($"Edit — {p.Name}", 400, 380);
            int y = 20;

            y = DlgLabel(dlg, "Barcode (read-only)", y); TextBox txtBarcode = DlgTextBox(dlg, y, p.Barcode); txtBarcode.ReadOnly = true; txtBarcode.BackColor = Color.FromArgb(245, 245, 245); y += 54;
            y = DlgLabel(dlg, "Product Name", y); TextBox txtName = DlgTextBox(dlg, y, p.Name); y += 54;

            y = DlgLabel(dlg, "Category", y);
            ComboBox cmbCat = new ComboBox { Location = new Point(20, y + 22), Size = new Size(350, 30), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var c in Categories) cmbCat.Items.Add(c);
            cmbCat.SelectedItem = p.Category; dlg.Controls.Add(cmbCat); y += 54;

            y = DlgLabel(dlg, "Price (₱)", y); TextBox txtPrice = DlgTextBox(dlg, y, p.Price.ToString("N2")); y += 54;

            y = DlgLabel(dlg, "Stock", y);
            NumericUpDown nudStock = new NumericUpDown { Location = new Point(20, y + 22), Size = new Size(180, 30), Minimum = 0, Maximum = 9999, Value = p.Stock, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
            dlg.Controls.Add(nudStock); y += 64;

            Button btnSave = MakeButton("✔  Save Changes", clrBlue, new Point(20, y), new Size(170, 38));
            Button btnCancel = MakeButton("✕  Cancel", clrRed, new Point(200, y), new Size(170, 38));

            btnSave.Click += (s2, e2) => {
                string name = txtName.Text.Trim();
                if (string.IsNullOrEmpty(name)) { ShowToast("Name cannot be empty.", false); return; }
                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0) { ShowToast("Enter valid price.", false); return; }

                p.Name = name; p.Category = cmbCat.SelectedItem.ToString(); p.Price = price; p.Stock = (int)nudStock.Value;
                RefreshGrid(); ShowToast($"'{name}' updated!", true); dlg.Close();
            };
            btnCancel.Click += (s2, e2) => dlg.Close();

            dlg.Controls.Add(btnSave); dlg.Controls.Add(btnCancel);
            dlg.ShowDialog(this);
        }

        private void ShowRestockDialog(Product p)
        {
            Form dlg = MakeDialog($"Restock — {p.Name}", 360, 240);

            Label lblInfo = new Label { Text = $"Current stock: {p.Stock}", Font = new Font("Segoe UI", 10), ForeColor = p.IsOutOfStock ? clrRed : p.IsLowStock ? clrOrange : clrGreen, Location = new Point(20, 20), AutoSize = true };
            dlg.Controls.Add(lblInfo);

            DlgLabel(dlg, "Add Quantity", 48);
            NumericUpDown nud = new NumericUpDown { Location = new Point(20, 70), Size = new Size(180, 34), Minimum = 1, Maximum = 9999, Value = 10, Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };
            dlg.Controls.Add(nud);

            Label lblPreview = new Label { Text = $"New stock after: {p.Stock + (int)nud.Value}", Font = new Font("Segoe UI", 9, FontStyle.Italic), ForeColor = clrTextSub, Location = new Point(20, 112), AutoSize = true };
            dlg.Controls.Add(lblPreview);
            nud.ValueChanged += (s2, e2) => { lblPreview.Text = $"New stock after: {p.Stock + (int)nud.Value}"; };

            Button btnAdd = MakeButton("↑  Add Stock", clrGreen, new Point(20, 148), new Size(150, 38));
            Button btnCancel = MakeButton("Cancel", clrMuted, new Point(180, 148), new Size(150, 38));

            btnAdd.Click += (s2, e2) => {
                p.Stock += (int)nud.Value;
                RefreshGrid(); ShowToast($"Restocked {p.Name}!", true); dlg.Close();
            };
            btnCancel.Click += (s2, e2) => dlg.Close();

            dlg.Controls.Add(btnAdd); dlg.Controls.Add(btnCancel);
            dlg.ShowDialog(this);
        }

        private void BtnRestockAll_Click(object sender, EventArgs e)
        {
            var low = _products.Where(p => p.Stock < 10).ToList();
            if (low.Count == 0) { ShowToast("No products need restocking.", true); return; }

            Form dlg = MakeDialog("Restock All Low Stock", 380, 210);
            Label lblDesc = new Label { Text = $"{low.Count} items are low/out of stock.", Font = new Font("Segoe UI", 10), Location = new Point(20, 20), Size = new Size(340, 40) };
            dlg.Controls.Add(lblDesc);

            DlgLabel(dlg, "Set stock level to:", 68);
            NumericUpDown nud = new NumericUpDown { Location = new Point(20, 90), Size = new Size(150, 34), Minimum = 1, Maximum = 9999, Value = 50, Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };
            dlg.Controls.Add(nud);

            Button btnGo = MakeButton("✔  Restock All", clrOrange, new Point(20, 144), new Size(150, 38));
            btnGo.Click += (s2, e2) => {
                foreach (var p in low) p.Stock = (int)nud.Value;
                RefreshGrid(); ShowToast($"Restocked {low.Count} items!", true); dlg.Close();
            };
            dlg.Controls.Add(btnGo);
            dlg.ShowDialog(this);
        }

        private void ConfirmDelete(Product p)
        {
            if (MessageBox.Show($"Delete '{p.Name}'?\nThis action is permanent.", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _products.Remove(p);
                RefreshGrid();
                ShowToast($"'{p.Name}' removed.", true);
            }
        }

        // ── TOAST ENGINE CONTROLS ─────────────────────
        private void BuildToast()
        {
            toastPanel = new Panel { Size = new Size(340, 44), BackColor = Color.FromArgb(30, 40, 60), Visible = false };
            this.Controls.Add(toastPanel);
            toastLabel = new Label { Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            toastPanel.Controls.Add(toastLabel);

            toastTimer = new Timer { Interval = 2200 };
            toastTimer.Tick += (s, e) => { toastPanel.Visible = false; toastTimer.Stop(); };
            this.Resize += (s, e) => toastPanel.Location = new Point(this.ClientSize.Width / 2 - 170, this.ClientSize.Height - 80);
        }

        private void ShowToast(string msg, bool success)
        {
            toastPanel.BackColor = success ? Color.FromArgb(22, 101, 52) : Color.FromArgb(127, 29, 29);
            toastLabel.Text = (success ? "✔  " : "✕  ") + msg;
            toastPanel.Location = new Point(this.ClientSize.Width / 2 - 170, this.ClientSize.Height - 80);
            toastPanel.Visible = true; toastPanel.BringToFront();
            toastTimer.Stop(); toastTimer.Start();
        }

        // ── UI BUILDER CORES ──────────────────────────
        private Form MakeDialog(string t, int w, int h) =>
            new Form { Text = t, Width = w, Height = h, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, StartPosition = FormStartPosition.CenterParent, BackColor = Color.White };

        private int DlgLabel(Form f, string text, int y)
        {
            Label l = new Label { Text = text, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = clrTextSub };
            f.Controls.Add(l); return y;
        }

        private TextBox DlgTextBox(Form f, int y, string val)
        {
            TextBox t = new TextBox { Location = new Point(20, y + 22), Width = 350, Font = new Font("Segoe UI", 10), Text = val };
            f.Controls.Add(t); return t;
        }

        private Button MakeButton(string txt, Color clr, Point pt, Size sz) =>
            new Button { Text = txt, BackColor = clr, ForeColor = Color.White, Location = pt, Size = sz, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

        private void StyleGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.GridColor = Color.FromArgb(241, 245, 249);
            dgv.BorderStyle = BorderStyle.None;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.ForeColor = clrText;
            dgv.DefaultCellStyle.SelectionBackColor = clrBlueLight;
            dgv.DefaultCellStyle.SelectionForeColor = clrBlue;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = clrTextSub;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 44;
            dgv.RowTemplate.Height = 48;
            dgv.EnableHeadersVisualStyles = false;
        }
    }
}