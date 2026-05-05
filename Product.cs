namespace GroceryStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string Barcode { get; set; } = "";
        public int StockQuantity { get; set; }
        public string Category { get; set; } = "";
        public string ImagePath { get; set; } = "";

        public Product() { }

        public Product(int id, string name, decimal price, string barcode, int stockQuantity, string category)
        {
            Id = id;
            Name = name;
            Price = price;
            Barcode = barcode;
            StockQuantity = stockQuantity;
            Category = category;
        }

        public override string ToString() => $"{Name} - {Price:C}";
    }
}
