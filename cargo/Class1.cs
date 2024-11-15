using System;

namespace cargo
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }

        public override string ToString()
        {
            return ProductName; // Это будет отображаться в ComboBox
        }
    }
}
