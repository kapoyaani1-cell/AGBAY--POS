using System;

namespace SimplePOS
{
    public class Product
    {
        public string Barcode { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public Product(
            string barcode,
            string name,
            decimal price,
            string category)
        {
            Barcode = barcode;
            Name = name;
            Price = price;
            Category = category;
        }
    }
}