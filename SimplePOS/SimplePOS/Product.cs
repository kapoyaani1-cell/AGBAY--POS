namespace SimplePOS
{
    public class Product
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public int Stock { get; set; }

        public bool IsOutOfStock => Stock <= 0;
        public bool IsLowStock => Stock > 0 && Stock <= 5;

        public Product(string barcode, string name, decimal price, string category, int stock = 50)
        {
            Barcode = barcode;
            Name = name;
            Price = price;
            Category = category;
            Stock = stock;
        }
    }
}