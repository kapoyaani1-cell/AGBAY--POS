namespace GroceryStore.Models
{
    public class ShoppingCart
    {
        private List<CartItem> items = new();

        public IReadOnlyList<CartItem> Items => items.AsReadOnly();

        public decimal Total => items.Sum(x => x.Total);

        public void AddItem(Product product)
        {
            var existingItem = items.FirstOrDefault(x => x.Id == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                items.Add(new CartItem(product.Id, product.Name, product.Price, product.Barcode));
            }
        }

        public void RemoveItem(int productId)
        {
            items.RemoveAll(x => x.Id == productId);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = items.FirstOrDefault(x => x.Id == productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    RemoveItem(productId);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }
        }

        public void Clear()
        {
            items.Clear();
        }
    }
}
