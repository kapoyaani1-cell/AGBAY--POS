using System;

namespace SimplePOS
{
    public class CartItem
    {
        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Subtotal
        {
            get
            {
                return Quantity * Price;
            }
        }
    }
}