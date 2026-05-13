using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimplePOS
{
    public partial class Form1 : Form
    {
        // LISTS
        List<Product> products = new List<Product>();
        List<CartItem> cart = new List<CartItem>();

        // CONTROLS
        FlowLayoutPanel productPanel = new FlowLayoutPanel();

        DataGridView dgvCart = new DataGridView();

        Label lblTotal = new Label();
        Label lblChange = new Label();
        Label lblBarcode = new Label();

        TextBox txtCash = new TextBox();
        TextBox txtBarcode = new TextBox();

        ComboBox cmbCategory = new ComboBox();

        Button btnCheckout = new Button();
        Button btnClear = new Button();

        public Form1()
        {
            InitializeComponent();

            SetupUI();

            LoadProducts();

            DisplayProducts();

            RefreshCart();
        }
        // UI
        private void SetupUI()
        {
            this.Text = "CHOYHUB POS SYSTEM";
            this.Size = new Size(1300, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // BARCODE LABEL
            lblBarcode.Text = "Scan Barcode:";
            lblBarcode.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblBarcode.Location = new Point(20, 20);
            lblBarcode.AutoSize = true;

            this.Controls.Add(lblBarcode);

            // BARCODE TEXTBOX
            txtBarcode.Location = new Point(150, 15);
            txtBarcode.Size = new Size(220, 30);
            txtBarcode.Font = new Font("Segoe UI", 12);

            txtBarcode.KeyDown += TxtBarcode_KeyDown;

            this.Controls.Add(txtBarcode);

            // CATEGORY LABEL
            Label lblCategory = new Label();

            lblCategory.Text = "Category:";
            lblCategory.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblCategory.Location = new Point(400, 20);
            lblCategory.AutoSize = true;

            this.Controls.Add(lblCategory);

            // CATEGORY COMBOBOX
            cmbCategory.Location = new Point(500, 15);
            cmbCategory.Size = new Size(180, 30);
            cmbCategory.Font = new Font("Segoe UI", 11);

            cmbCategory.Items.Add("All");
            cmbCategory.Items.Add("Drinks");
            cmbCategory.Items.Add("Snacks");
            cmbCategory.Items.Add("Fast Food");
            cmbCategory.Items.Add("Bread");
            cmbCategory.Items.Add("School");

            cmbCategory.SelectedIndex = 0;

            cmbCategory.SelectedIndexChanged +=
                CmbCategory_SelectedIndexChanged;

            this.Controls.Add(cmbCategory);

            // PRODUCT PANEL
            productPanel.Location = new Point(20, 70);
            productPanel.Size = new Size(650, 620);
            productPanel.AutoScroll = true;
            productPanel.BackColor = Color.White;

            this.Controls.Add(productPanel);

            // CART GRID
            dgvCart.Location = new Point(700, 20);
            dgvCart.Size = new Size(560, 350);

            dgvCart.ColumnCount = 4;

            dgvCart.Columns[0].Name = "Product";
            dgvCart.Columns[1].Name = "Qty";
            dgvCart.Columns[2].Name = "Price";
            dgvCart.Columns[3].Name = "Subtotal";

            dgvCart.Columns[0].Width = 200;

            this.Controls.Add(dgvCart);

            // TOTAL LABEL
            lblTotal.Text = "Total: ₱0";
            lblTotal.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTotal.ForeColor = Color.Green;
            lblTotal.Location = new Point(700, 400);
            lblTotal.AutoSize = true;

            this.Controls.Add(lblTotal);

            // CASH LABEL
            Label lblCash = new Label();

            lblCash.Text = "Cash:";
            lblCash.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblCash.Location = new Point(700, 470);
            lblCash.AutoSize = true;

            this.Controls.Add(lblCash);

            // CASH TEXTBOX
            txtCash.Location = new Point(780, 465);
            txtCash.Size = new Size(220, 35);
            txtCash.Font = new Font("Segoe UI", 12);

            this.Controls.Add(txtCash);

            // CHANGE LABEL
            lblChange.Text = "Change: ₱0";
            lblChange.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblChange.ForeColor = Color.Blue;
            lblChange.Location = new Point(700, 530);
            lblChange.AutoSize = true;

            this.Controls.Add(lblChange);

            // CHECKOUT BUTTON
            btnCheckout.Text = "CHECKOUT";
            btnCheckout.Size = new Size(180, 50);
            btnCheckout.Location = new Point(1040, 450);

            btnCheckout.BackColor = Color.Green;
            btnCheckout.ForeColor = Color.White;

            btnCheckout.FlatStyle = FlatStyle.Flat;

            btnCheckout.Click += BtnCheckout_Click;

            this.Controls.Add(btnCheckout);

            // CLEAR BUTTON
            btnClear.Text = "CLEAR CART";
            btnClear.Size = new Size(180, 50);
            btnClear.Location = new Point(1040, 520);

            btnClear.BackColor = Color.Red;
            btnClear.ForeColor = Color.White;

            btnClear.FlatStyle = FlatStyle.Flat;

            btnClear.Click += BtnClear_Click;

            this.Controls.Add(btnClear);
        }

        // PRODUCTS

        private void LoadProducts()
        {
            // DRINKS
            products.Add(new Product("1001", "Coke", 20, "Drinks"));
            products.Add(new Product("1002", "Pepsi", 18, "Drinks"));
            products.Add(new Product("1003", "Mountain Dew", 22, "Drinks"));
            products.Add(new Product("1004", "Royal", 20, "Drinks"));
            products.Add(new Product("1005", "Water", 15, "Drinks"));
            products.Add(new Product("1006", "Coffee", 50, "Drinks"));
            products.Add(new Product("1007", "Milk Tea", 65, "Drinks"));
            products.Add(new Product("1008", "Orange Juice", 45, "Drinks"));

            // SNACKS
            products.Add(new Product("2001", "Piattos", 25, "Snacks"));
            products.Add(new Product("2002", "Nova", 20, "Snacks"));
            products.Add(new Product("2003", "Vcut", 22, "Snacks"));
            products.Add(new Product("2004", "Chippy", 18, "Snacks"));
            products.Add(new Product("2005", "Cheezy", 15, "Snacks"));
            products.Add(new Product("2006", "Skyflakes", 10, "Snacks"));

            // FAST FOOD
            products.Add(new Product("3001", "Burger", 80, "Fast Food"));
            products.Add(new Product("3002", "Fries", 45, "Fast Food"));
            products.Add(new Product("3003", "Hotdog", 35, "Fast Food"));
            products.Add(new Product("3004", "Pizza Slice", 90, "Fast Food"));
            products.Add(new Product("3005", "Chicken Meal", 120, "Fast Food"));
            products.Add(new Product("3006", "Spaghetti", 75, "Fast Food"));

            // BREAD
            products.Add(new Product("4001", "Bread", 30, "Bread"));
            products.Add(new Product("4002", "Donut", 25, "Bread"));
            products.Add(new Product("4003", "Croissant", 40, "Bread"));
            products.Add(new Product("4004", "Cupcake", 35, "Bread"));

            // SCHOOL
            products.Add(new Product("5001", "Notebook", 45, "School"));
            products.Add(new Product("5002", "Ballpen", 12, "School"));
            products.Add(new Product("5003", "Pencil", 10, "School"));
            products.Add(new Product("5004", "Eraser", 8, "School"));
            products.Add(new Product("5005", "Ruler", 15, "School"));
        }

        // DISPLAY PRODUCTS
        private void DisplayProducts()
        {
            productPanel.Controls.Clear();

            string selectedCategory =
                cmbCategory.SelectedItem.ToString();

            List<Product> filteredProducts;

            if (selectedCategory == "All")
            {
                filteredProducts = products;
            }
            else
            {
                filteredProducts =
                    products.Where(x =>
                    x.Category == selectedCategory).ToList();
            }

            foreach (Product product in filteredProducts)
            {
                Panel card = new Panel();

                card.Size = new Size(220, 210);

                card.BackColor = Color.White;

                card.BorderStyle = BorderStyle.FixedSingle;

                card.Margin = new Padding(10);

                // NAME
                Label lblName = new Label();

                lblName.Text = product.Name;

                lblName.Font =
                    new Font("Segoe UI", 13, FontStyle.Bold);

                lblName.Location = new Point(15, 15);

                lblName.AutoSize = true;

                // PRICE
                Label lblPrice = new Label();

                lblPrice.Text = "₱" + product.Price;

                lblPrice.Font =
                    new Font("Segoe UI", 11);

                lblPrice.Location = new Point(15, 50);

                lblPrice.AutoSize = true;

                // CATEGORY
                Label lblCat = new Label();

                lblCat.Text = product.Category;

                lblCat.Font =
                    new Font("Segoe UI", 10);

                lblCat.ForeColor = Color.DarkBlue;

                lblCat.Location = new Point(15, 80);

                lblCat.AutoSize = true;

                // BARCODE
                Label lblCode = new Label();

                lblCode.Text = "Code: " + product.Barcode;

                lblCode.Font =
                    new Font("Segoe UI", 9);

                lblCode.ForeColor = Color.Gray;

                lblCode.Location = new Point(15, 110);

                lblCode.AutoSize = true;

                // BUTTON
                Button btnAdd = new Button();

                btnAdd.Text = "Add to Cart";

                btnAdd.Size = new Size(150, 40);

                btnAdd.Location = new Point(30, 145);

                btnAdd.BackColor = Color.DodgerBlue;

                btnAdd.ForeColor = Color.White;

                btnAdd.FlatStyle = FlatStyle.Flat;

                btnAdd.Click += (s, e) =>
                {
                    AddToCart(product);
                };

                // ADD CONTROLS
                card.Controls.Add(lblName);
                card.Controls.Add(lblPrice);
                card.Controls.Add(lblCat);
                card.Controls.Add(lblCode);
                card.Controls.Add(btnAdd);

                productPanel.Controls.Add(card);
            }
        }

        // CATEGORY FILTER
        private void CmbCategory_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            DisplayProducts();
        }

        // ADD TO CART
        private void AddToCart(Product product)
        {
            CartItem existingItem =
                cart.FirstOrDefault(x => x.Name == product.Name);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }

            RefreshCart();
        }

        // REFRESH CART
        private void RefreshCart()
        {
            dgvCart.Rows.Clear();

            decimal total = 0;

            foreach (CartItem item in cart)
            {
                dgvCart.Rows.Add(
                    item.Name,
                    item.Quantity,
                    item.Price,
                    item.Subtotal
                );

                total += item.Subtotal;
            }

            lblTotal.Text = "Total: ₱" + total;
        }

        // BARCODE SCAN
        private void TxtBarcode_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string barcode = txtBarcode.Text.Trim();

                Product foundProduct =
                    products.FirstOrDefault(
                        x => x.Barcode == barcode);

                if (foundProduct != null)
                {
                    AddToCart(foundProduct);

                    txtBarcode.Clear();
                }
                else
                {
                    MessageBox.Show(
                        "Product not found!");
                }
            }
        }

        // CHECKOUT
        private void BtnCheckout_Click(
            object sender,
            EventArgs e)
        {
            decimal total = cart.Sum(x => x.Subtotal);

            decimal cash;

            bool valid =
                decimal.TryParse(txtCash.Text, out cash);

            if (!valid)
            {
                MessageBox.Show(
                    "Enter valid cash.");

                return;
            }

            if (cash < total)
            {
                MessageBox.Show(
                    "Insufficient cash.");

                return;
            }

            decimal change = cash - total;

            lblChange.Text =
                "Change: ₱" + change;

            MessageBox.Show(
                "Payment Successful!");

            cart.Clear();

            RefreshCart();

            txtCash.Clear();
        }

        // CLEAR CART
        private void BtnClear_Click(
            object sender,
            EventArgs e)
        {
            cart.Clear();

            RefreshCart();

            txtCash.Clear();

            lblChange.Text = "Change: ₱0";
        }
    }
}