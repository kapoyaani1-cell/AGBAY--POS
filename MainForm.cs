using GroceryStore.Models;

namespace project2
{
    public partial class MainForm : Form
    {
        private ShoppingCart cart = new();
        private List<Product> products = new();

        public MainForm()
        {
            InitializeComponent();
            LoadProducts();
            LoadCategories();
            RefreshCartView();
        }

        // ============ Data Loading ============

        private void LoadProducts()
        {
            products = new List<Product>
            {
                new Product(1, "Apple", 0.99m, "1234567890001", 50, "Fruits"),
                new Product(2, "Banana", 0.59m, "1234567890002", 60, "Fruits"),
                new Product(3, "Orange", 1.29m, "1234567890003", 40, "Fruits"),
                new Product(4, "Strawberry", 2.99m, "1234567890004", 35, "Fruits"),
                new Product(5, "Milk", 3.49m, "1234567890005", 30, "Dairy"),
                new Product(6, "Cheese", 4.99m, "1234567890006", 25, "Dairy"),
                new Product(7, "Butter", 4.29m, "1234567890007", 20, "Dairy"),
                new Product(8, "Yogurt", 2.49m, "1234567890008", 40, "Dairy"),
                new Product(9, "Bread", 2.49m, "1234567890009", 35, "Bakery"),
                new Product(10, "Croissant", 3.49m, "1234567890010", 25, "Bakery"),
                new Product(11, "Donut", 1.49m, "1234567890011", 50, "Bakery"),
                new Product(12, "Tomato", 1.49m, "1234567890012", 45, "Vegetables"),
                new Product(13, "Lettuce", 1.99m, "1234567890013", 30, "Vegetables"),
                new Product(14, "Carrot", 0.89m, "1234567890014", 50, "Vegetables"),
                new Product(15, "Broccoli", 2.49m, "1234567890015", 25, "Vegetables"),
                new Product(16, "Chicken Breast", 7.99m, "1234567890016", 20, "Meat"),
                new Product(17, "Ground Beef", 8.99m, "1234567890017", 15, "Meat"),
                new Product(18, "Salmon", 12.99m, "1234567890018", 10, "Meat"),
            };

            RefreshProductList();
        }

        private void LoadCategories()
        {
            comboBoxCategory.Items.Add("All Categories");
            var categories = products.Select(p => p.Category).Distinct().OrderBy(c => c);
            foreach (var category in categories)
            {
                comboBoxCategory.Items.Add(category);
            }
            comboBoxCategory.SelectedIndex = 0;
        }

        // ============ Product Filtering & Display ============

        private void RefreshProductList()
        {
            FilterAndDisplayProducts();
        }

        private void FilterAndDisplayProducts()
        {
            listBoxProducts.Items.Clear();

            string selectedCategory = comboBoxCategory.SelectedItem?.ToString() ?? "All Categories";
            string searchTerm = textBoxSearch.Text.ToLower();

            var filteredProducts = products.Where(p =>
            {
                bool categoryMatch = selectedCategory == "All Categories" || p.Category == selectedCategory;
                bool searchMatch = string.IsNullOrWhiteSpace(searchTerm) ||
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.Barcode.Contains(searchTerm);

                return categoryMatch && searchMatch;
            }).ToList();

            foreach (var product in filteredProducts)
            {
                listBoxProducts.Items.Add(product);
            }

            if (filteredProducts.Count == 0 && !string.IsNullOrWhiteSpace(searchTerm))
            {
                labelNoResults.Visible = true;
            }
            else
            {
                labelNoResults.Visible = false;
            }

            labelProductCount.Text = $"Showing {filteredProducts.Count} product(s)";
        }

        private void DisplayProductDetails(Product product)
        {
            if (product != null)
            {
                textBoxProductName.Text = product.Name;
                textBoxPrice.Text = $"{product.Price:C}";
                textBoxBarcode.Text = product.Barcode;
                textBoxCategory.Text = product.Category;
                textBoxStock.Text = $"{product.StockQuantity} in stock";
                buttonAddToCart.Enabled = product.StockQuantity > 0;

                if (product.StockQuantity == 0)
                {
                    buttonAddToCart.Text = "Out of Stock";
                    buttonAddToCart.BackColor = System.Drawing.Color.FromArgb(189, 195, 199);
                }
                else
                {
                    buttonAddToCart.Text = "➕ Add to Cart";
                    buttonAddToCart.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
                }
            }
        }

        private void ClearProductDetails()
        {
            textBoxProductName.Clear();
            textBoxPrice.Clear();
            textBoxBarcode.Clear();
            textBoxCategory.Clear();
            textBoxStock.Clear();
            buttonAddToCart.Enabled = false;
            buttonAddToCart.Text = "➕ Add to Cart";
        }

        // ============ Cart Operations ============

        private void RefreshCartView()
        {
            dataGridViewCart.DataSource = null;
            var cartItems = cart.Items.Select(item => new
            {
                item.Name,
                item.Barcode,
                Price = item.Price.ToString("C"),
                item.Quantity,
                Total = item.Total.ToString("C")
            }).ToList();

            dataGridViewCart.DataSource = cartItems;
            labelCartTotal.Text = $"Total: {cart.Total:C}";
            labelItemCount.Text = $"Items in Cart: {cart.Items.Count}";

            buttonCheckout.Enabled = cart.Items.Count > 0;
            buttonRemoveFromCart.Enabled = cart.Items.Count > 0;
        }

        private void AddProductToCart()
        {
            if (listBoxProducts.SelectedItem is Product selectedProduct)
            {
                if (selectedProduct.StockQuantity > 0)
                {
                    cart.AddItem(selectedProduct);
                    RefreshCartView();
                    ShowNotification($"✓ {selectedProduct.Name} added to cart!");
                }
                else
                {
                    ShowNotification("⚠ This product is out of stock!", true);
                }
            }
            else
            {
                ShowNotification("⚠ Please select a product!", true);
            }
        }

        private void RemoveFromCart()
        {
            if (dataGridViewCart.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewCart.SelectedRows[0].Index;
                if (selectedIndex >= 0 && selectedIndex < cart.Items.Count)
                {
                    var itemToRemove = cart.Items[selectedIndex];
                    cart.RemoveItem(itemToRemove.Id);
                    RefreshCartView();
                    ShowNotification($"✓ {itemToRemove.Name} removed from cart!");
                }
            }
            else
            {
                ShowNotification("⚠ Please select an item to remove!", true);
            }
        }

        private void ProcessCheckout()
        {
            if (cart.Items.Count == 0)
            {
                ShowNotification("⚠ Your cart is empty!", true);
                return;
            }

            var summary = $"Thank you for your purchase!\n\n";
            summary += $"📦 Total Items: {cart.Items.Count}\n";
            summary += $"💰 Total Amount: {cart.Total:C}\n\n";
            summary += "Items:\n";
            foreach (var item in cart.Items)
            {
                summary += $"  • {item.Name} x {item.Quantity} = {item.Total:C}\n";
            }

            MessageBox.Show(summary, "✓ Checkout Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
            cart.Clear();
            RefreshCartView();
            listBoxProducts.ClearSelected();
            ClearProductDetails();
            ShowNotification("✓ Order completed! Thank you for shopping!");
        }

        private void ShowNotification(string message, bool isError = false)
        {
            labelNotification.Text = message;
            labelNotification.ForeColor = isError ? System.Drawing.Color.FromArgb(231, 76, 60) : System.Drawing.Color.FromArgb(39, 174, 96);
            labelNotification.Visible = true;

            timerNotification.Stop();
            timerNotification.Start();
        }

        // ============ Event Handlers ============

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            FilterAndDisplayProducts();
        }

        private void buttonClearSearch_Click(object sender, EventArgs e)
        {
            textBoxSearch.Clear();
            comboBoxCategory.SelectedIndex = 0;
            FilterAndDisplayProducts();
        }

        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterAndDisplayProducts();
        }

        private void listBoxProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxProducts.SelectedItem is Product product)
            {
                DisplayProductDetails(product);
            }
            else
            {
                ClearProductDetails();
            }
        }

        private void buttonAddToCart_Click(object sender, EventArgs e)
        {
            AddProductToCart();
        }

        private void buttonRemoveFromCart_Click(object sender, EventArgs e)
        {
            RemoveFromCart();
        }

        private void buttonCheckout_Click(object sender, EventArgs e)
        {
            ProcessCheckout();
        }

        private void timerNotification_Tick(object sender, EventArgs e)
        {
            labelNotification.Visible = false;
            timerNotification.Stop();
        }
    }
}
