namespace project2
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // Panels
            panelHeader = new System.Windows.Forms.Panel();
            panelMain = new System.Windows.Forms.Panel();
            panelLeft = new System.Windows.Forms.Panel();
            panelRight = new System.Windows.Forms.Panel();
            panelSearch = new System.Windows.Forms.Panel();
            panelCategory = new System.Windows.Forms.Panel();
            panelProductInfo = new System.Windows.Forms.Panel();
            panelCart = new System.Windows.Forms.Panel();
            panelCartBottom = new System.Windows.Forms.Panel();

            // Header Controls
            labelHeader = new System.Windows.Forms.Label();

            // Search & Filter
            labelSearch = new System.Windows.Forms.Label();
            textBoxSearch = new System.Windows.Forms.TextBox();
            buttonClearSearch = new System.Windows.Forms.Button();
            labelCategory = new System.Windows.Forms.Label();
            comboBoxCategory = new System.Windows.Forms.ComboBox();
            labelProductCount = new System.Windows.Forms.Label();

            // Product List
            listBoxProducts = new System.Windows.Forms.ListBox();
            labelNoResults = new System.Windows.Forms.Label();

            // Product Details
            labelProductDetailsTitle = new System.Windows.Forms.Label();
            labelProductNameLabel = new System.Windows.Forms.Label();
            textBoxProductName = new System.Windows.Forms.TextBox();
            labelPriceLabel = new System.Windows.Forms.Label();
            textBoxPrice = new System.Windows.Forms.TextBox();
            labelBarcodeLabel = new System.Windows.Forms.Label();
            textBoxBarcode = new System.Windows.Forms.TextBox();
            labelCategoryLabel = new System.Windows.Forms.Label();
            textBoxCategory = new System.Windows.Forms.TextBox();
            labelStockLabel = new System.Windows.Forms.Label();
            textBoxStock = new System.Windows.Forms.TextBox();
            buttonAddToCart = new System.Windows.Forms.Button();

            // Cart
            labelCartTitle = new System.Windows.Forms.Label();
            dataGridViewCart = new System.Windows.Forms.DataGridView();
            labelItemCount = new System.Windows.Forms.Label();
            labelCartTotal = new System.Windows.Forms.Label();
            buttonRemoveFromCart = new System.Windows.Forms.Button();
            buttonCheckout = new System.Windows.Forms.Button();

            // Notification
            labelNotification = new System.Windows.Forms.Label();
            timerNotification = new System.Windows.Forms.Timer(components);

            // Form Setup
            SuspendLayout();
            ClientSize = new System.Drawing.Size(1500, 900);
            Text = "🛒 Grocery Store Management System";
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            BackColor = System.Drawing.Color.FromArgb(240, 245, 250);
            Font = new System.Drawing.Font("Segoe UI", 10F);
            MinimumSize = new System.Drawing.Size(1200, 700);

            // ============ HEADER ============
            panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            panelHeader.Height = 70;
            panelHeader.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            panelHeader.Padding = new System.Windows.Forms.Padding(20);

            labelHeader.Text = "🛒 Welcome to Grocery Store";
            labelHeader.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            labelHeader.ForeColor = System.Drawing.Color.White;
            labelHeader.Dock = System.Windows.Forms.DockStyle.Left;
            labelHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            panelHeader.Controls.Add(labelHeader);

            // ============ MAIN CONTAINER ============
            panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            panelMain.BackColor = System.Drawing.Color.FromArgb(240, 245, 250);

            // ============ LEFT PANEL ============
            panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            panelLeft.Width = 650;
            panelLeft.BackColor = System.Drawing.Color.White;
            panelLeft.Padding = new System.Windows.Forms.Padding(20);

            // Search Panel
            panelSearch.Location = new System.Drawing.Point(20, 20);
            panelSearch.Size = new System.Drawing.Size(610, 80);
            panelSearch.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            panelSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            labelSearch.Text = "🔍 Search Products:";
            labelSearch.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            labelSearch.Location = new System.Drawing.Point(10, 8);
            labelSearch.Size = new System.Drawing.Size(200, 20);
            labelSearch.ForeColor = System.Drawing.Color.FromArgb(44, 62, 80);
            panelSearch.Controls.Add(labelSearch);

            textBoxSearch.Location = new System.Drawing.Point(10, 30);
            textBoxSearch.Size = new System.Drawing.Size(520, 35);
            textBoxSearch.Font = new System.Drawing.Font("Segoe UI", 11F);
            textBoxSearch.TextChanged += textBoxSearch_TextChanged;
            panelSearch.Controls.Add(textBoxSearch);

            buttonClearSearch.Text = "Clear";
            buttonClearSearch.Location = new System.Drawing.Point(540, 30);
            buttonClearSearch.Size = new System.Drawing.Size(60, 35);
            buttonClearSearch.BackColor = System.Drawing.Color.FromArgb(189, 195, 199);
            buttonClearSearch.ForeColor = System.Drawing.Color.White;
            buttonClearSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonClearSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            buttonClearSearch.Click += buttonClearSearch_Click;
            panelSearch.Controls.Add(buttonClearSearch);

            // Category Panel
            panelCategory.Location = new System.Drawing.Point(20, 105);
            panelCategory.Size = new System.Drawing.Size(610, 70);
            panelCategory.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            panelCategory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            labelCategory.Text = "📂 Category:";
            labelCategory.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            labelCategory.Location = new System.Drawing.Point(10, 8);
            labelCategory.Size = new System.Drawing.Size(150, 20);
            labelCategory.ForeColor = System.Drawing.Color.FromArgb(44, 62, 80);
            panelCategory.Controls.Add(labelCategory);

            comboBoxCategory.Location = new System.Drawing.Point(10, 30);
            comboBoxCategory.Size = new System.Drawing.Size(590, 30);
            comboBoxCategory.Font = new System.Drawing.Font("Segoe UI", 10F);
            comboBoxCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxCategory.SelectedIndexChanged += comboBoxCategory_SelectedIndexChanged;
            panelCategory.Controls.Add(comboBoxCategory);

            // Product List Label
            labelProductCount.Text = "Showing 0 product(s)";
            labelProductCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            labelProductCount.Location = new System.Drawing.Point(20, 185);
            labelProductCount.Size = new System.Drawing.Size(610, 20);
            labelProductCount.ForeColor = System.Drawing.Color.FromArgb(127, 140, 141);

            // Product List
            listBoxProducts.Location = new System.Drawing.Point(20, 205);
            listBoxProducts.Size = new System.Drawing.Size(610, 200);
            listBoxProducts.Font = new System.Drawing.Font("Segoe UI", 10F);
            listBoxProducts.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            listBoxProducts.ItemHeight = 50;
            listBoxProducts.SelectedIndexChanged += listBoxProducts_SelectedIndexChanged;

            // No Results Label
            labelNoResults.Text = "❌ No products found";
            labelNoResults.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            labelNoResults.Location = new System.Drawing.Point(20, 280);
            labelNoResults.Size = new System.Drawing.Size(610, 50);
            labelNoResults.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            labelNoResults.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60);
            labelNoResults.Visible = false;

            // Product Details Panel
            panelProductInfo.Location = new System.Drawing.Point(20, 410);
            panelProductInfo.Size = new System.Drawing.Size(610, 350);
            panelProductInfo.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            panelProductInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            labelProductDetailsTitle.Text = "📋 Product Details";
            labelProductDetailsTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            labelProductDetailsTitle.Location = new System.Drawing.Point(10, 10);
            labelProductDetailsTitle.Size = new System.Drawing.Size(590, 25);
            labelProductDetailsTitle.ForeColor = System.Drawing.Color.FromArgb(44, 62, 80);
            panelProductInfo.Controls.Add(labelProductDetailsTitle);

            // Name
            labelProductNameLabel.Text = "Product Name:";
            labelProductNameLabel.Location = new System.Drawing.Point(10, 45);
            labelProductNameLabel.Size = new System.Drawing.Size(100, 20);
            labelProductNameLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            panelProductInfo.Controls.Add(labelProductNameLabel);

            textBoxProductName.Location = new System.Drawing.Point(120, 45);
            textBoxProductName.Size = new System.Drawing.Size(480, 30);
            textBoxProductName.ReadOnly = true;
            textBoxProductName.Font = new System.Drawing.Font("Segoe UI", 10F);
            textBoxProductName.BackColor = System.Drawing.Color.White;
            panelProductInfo.Controls.Add(textBoxProductName);

            // Price
            labelPriceLabel.Text = "Price:";
            labelPriceLabel.Location = new System.Drawing.Point(10, 85);
            labelPriceLabel.Size = new System.Drawing.Size(100, 20);
            labelPriceLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            panelProductInfo.Controls.Add(labelPriceLabel);

            textBoxPrice.Location = new System.Drawing.Point(120, 85);
            textBoxPrice.Size = new System.Drawing.Size(480, 30);
            textBoxPrice.ReadOnly = true;
            textBoxPrice.Font = new System.Drawing.Font("Segoe UI", 10F);
            textBoxPrice.BackColor = System.Drawing.Color.White;
            panelProductInfo.Controls.Add(textBoxPrice);

            // Barcode
            labelBarcodeLabel.Text = "Barcode:";
            labelBarcodeLabel.Location = new System.Drawing.Point(10, 125);
            labelBarcodeLabel.Size = new System.Drawing.Size(100, 20);
            labelBarcodeLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            panelProductInfo.Controls.Add(labelBarcodeLabel);

            textBoxBarcode.Location = new System.Drawing.Point(120, 125);
            textBoxBarcode.Size = new System.Drawing.Size(480, 30);
            textBoxBarcode.ReadOnly = true;
            textBoxBarcode.Font = new System.Drawing.Font("Segoe UI", 10F);
            textBoxBarcode.BackColor = System.Drawing.Color.White;
            panelProductInfo.Controls.Add(textBoxBarcode);

            // Category
            labelCategoryLabel.Text = "Category:";
            labelCategoryLabel.Location = new System.Drawing.Point(10, 165);
            labelCategoryLabel.Size = new System.Drawing.Size(100, 20);
            labelCategoryLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            panelProductInfo.Controls.Add(labelCategoryLabel);

            textBoxCategory.Location = new System.Drawing.Point(120, 165);
            textBoxCategory.Size = new System.Drawing.Size(480, 30);
            textBoxCategory.ReadOnly = true;
            textBoxCategory.Font = new System.Drawing.Font("Segoe UI", 10F);
            textBoxCategory.BackColor = System.Drawing.Color.White;
            panelProductInfo.Controls.Add(textBoxCategory);

            // Stock
            labelStockLabel.Text = "Stock:";
            labelStockLabel.Location = new System.Drawing.Point(10, 205);
            labelStockLabel.Size = new System.Drawing.Size(100, 20);
            labelStockLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            panelProductInfo.Controls.Add(labelStockLabel);

            textBoxStock.Location = new System.Drawing.Point(120, 205);
            textBoxStock.Size = new System.Drawing.Size(480, 30);
            textBoxStock.ReadOnly = true;
            textBoxStock.Font = new System.Drawing.Font("Segoe UI", 10F);
            textBoxStock.BackColor = System.Drawing.Color.White;
            panelProductInfo.Controls.Add(textBoxStock);

            // Add to Cart Button
            buttonAddToCart.Text = "➕ Add to Cart";
            buttonAddToCart.Location = new System.Drawing.Point(10, 250);
            buttonAddToCart.Size = new System.Drawing.Size(590, 80);
            buttonAddToCart.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            buttonAddToCart.ForeColor = System.Drawing.Color.White;
            buttonAddToCart.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            buttonAddToCart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonAddToCart.Cursor = System.Windows.Forms.Cursors.Hand;
            buttonAddToCart.Click += buttonAddToCart_Click;
            buttonAddToCart.Enabled = false;
            panelProductInfo.Controls.Add(buttonAddToCart);

            panelLeft.Controls.Add(panelSearch);
            panelLeft.Controls.Add(panelCategory);
            panelLeft.Controls.Add(labelProductCount);
            panelLeft.Controls.Add(listBoxProducts);
            panelLeft.Controls.Add(labelNoResults);
            panelLeft.Controls.Add(panelProductInfo);

            // ============ RIGHT PANEL ============
            panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            panelRight.BackColor = System.Drawing.Color.White;
            panelRight.Padding = new System.Windows.Forms.Padding(20);

            labelCartTitle.Text = "🛒 Shopping Cart";
            labelCartTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            labelCartTitle.Location = new System.Drawing.Point(20, 20);
            labelCartTitle.Size = new System.Drawing.Size(800, 30);
            labelCartTitle.ForeColor = System.Drawing.Color.FromArgb(44, 62, 80);

            // DataGridView
            dataGridViewCart.Location = new System.Drawing.Point(20, 60);
            dataGridViewCart.Size = new System.Drawing.Size(800, 650);
            dataGridViewCart.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCart.ReadOnly = true;
            dataGridViewCart.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCart.EnableHeadersVisualStyles = false;
            dataGridViewCart.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            dataGridViewCart.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dataGridViewCart.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCart.ColumnHeadersHeight = 40;
            dataGridViewCart.RowTemplate.Height = 35;
            dataGridViewCart.RowHeadersVisible = false;
            dataGridViewCart.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridViewCart.AllowUserToAddRows = false;
            dataGridViewCart.GridColor = System.Drawing.Color.FromArgb(220, 220, 220);
            dataGridViewCart.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCart.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);

            // Cart Bottom Panel
            panelCartBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelCartBottom.Height = 120;
            panelCartBottom.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            panelCartBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            labelItemCount.Text = "Items in Cart: 0";
            labelItemCount.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            labelItemCount.Location = new System.Drawing.Point(20, 15);
            labelItemCount.Size = new System.Drawing.Size(400, 25);
            labelItemCount.ForeColor = System.Drawing.Color.FromArgb(44, 62, 80);
            panelCartBottom.Controls.Add(labelItemCount);

            labelCartTotal.Text = "Total: $0.00";
            labelCartTotal.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            labelCartTotal.Location = new System.Drawing.Point(20, 45);
            labelCartTotal.Size = new System.Drawing.Size(400, 35);
            labelCartTotal.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60);
            panelCartBottom.Controls.Add(labelCartTotal);

            buttonRemoveFromCart.Text = "🗑️ Remove";
            buttonRemoveFromCart.Location = new System.Drawing.Point(20, 85);
            buttonRemoveFromCart.Size = new System.Drawing.Size(380, 25);
            buttonRemoveFromCart.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            buttonRemoveFromCart.ForeColor = System.Drawing.Color.White;
            buttonRemoveFromCart.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            buttonRemoveFromCart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonRemoveFromCart.Cursor = System.Windows.Forms.Cursors.Hand;
            buttonRemoveFromCart.Click += buttonRemoveFromCart_Click;
            panelCartBottom.Controls.Add(buttonRemoveFromCart);

            buttonCheckout.Text = "✓ Checkout";
            buttonCheckout.Location = new System.Drawing.Point(410, 85);
            buttonCheckout.Size = new System.Drawing.Size(390, 25);
            buttonCheckout.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            buttonCheckout.ForeColor = System.Drawing.Color.White;
            buttonCheckout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            buttonCheckout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonCheckout.Cursor = System.Windows.Forms.Cursors.Hand;
            buttonCheckout.Click += buttonCheckout_Click;
            panelCartBottom.Controls.Add(buttonCheckout);

            panelRight.Controls.Add(panelCartBottom);
            panelRight.Controls.Add(labelCartTitle);
            panelRight.Controls.Add(dataGridViewCart);

            // ============ NOTIFICATION ============
            labelNotification.Text = "Notification";
            labelNotification.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            labelNotification.Location = new System.Drawing.Point(20, 20);
            labelNotification.Size = new System.Drawing.Size(1460, 40);
            labelNotification.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            labelNotification.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            labelNotification.ForeColor = System.Drawing.Color.White;
            labelNotification.Visible = false;
            labelNotification.Dock = System.Windows.Forms.DockStyle.Top;

            timerNotification.Interval = 3000;
            timerNotification.Tick += timerNotification_Tick;

            // Add Panels
            panelMain.Controls.Add(panelRight);
            panelMain.Controls.Add(panelLeft);

            Controls.Add(panelMain);
            Controls.Add(labelNotification);
            Controls.Add(panelHeader);

            ResumeLayout(false);
        }

        // Components
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label labelHeader;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Panel panelCategory;
        private System.Windows.Forms.Panel panelProductInfo;
        private System.Windows.Forms.Panel panelCart;
        private System.Windows.Forms.Panel panelCartBottom;

        private System.Windows.Forms.Label labelSearch;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.Button buttonClearSearch;
        private System.Windows.Forms.Label labelCategory;
        private System.Windows.Forms.ComboBox comboBoxCategory;
        private System.Windows.Forms.Label labelProductCount;

        private System.Windows.Forms.ListBox listBoxProducts;
        private System.Windows.Forms.Label labelNoResults;

        private System.Windows.Forms.Label labelProductDetailsTitle;
        private System.Windows.Forms.Label labelProductNameLabel;
        private System.Windows.Forms.TextBox textBoxProductName;
        private System.Windows.Forms.Label labelPriceLabel;
        private System.Windows.Forms.TextBox textBoxPrice;
        private System.Windows.Forms.Label labelBarcodeLabel;
        private System.Windows.Forms.TextBox textBoxBarcode;
        private System.Windows.Forms.Label labelCategoryLabel;
        private System.Windows.Forms.TextBox textBoxCategory;
        private System.Windows.Forms.Label labelStockLabel;
        private System.Windows.Forms.TextBox textBoxStock;
        private System.Windows.Forms.Button buttonAddToCart;

        private System.Windows.Forms.Label labelCartTitle;
        private System.Windows.Forms.DataGridView dataGridViewCart;
        private System.Windows.Forms.Label labelItemCount;
        private System.Windows.Forms.Label labelCartTotal;
        private System.Windows.Forms.Button buttonRemoveFromCart;
        private System.Windows.Forms.Button buttonCheckout;

        private System.Windows.Forms.Label labelNotification;
        private System.Windows.Forms.Timer timerNotification;
    }
}
