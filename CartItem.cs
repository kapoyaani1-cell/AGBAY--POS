namespace GroceryStore.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Barcode { get; set; } = "";

        public decimal Total => Price * Quantity;

        public CartItem() { }

        public CartItem(int id, string name, decimal price, string barcode)
        {
            Id = id;
            Name = name;
            Price = price;
            Barcode = barcode;
            Quantity = 1;
        }
    }
}
