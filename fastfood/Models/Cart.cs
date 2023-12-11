namespace fastfood.Models
{
    public class Cart
    {
        public List<CartLine> Lines { get; set; } = new List<CartLine>();
        public void AddItem(Product product, int Quantity)
        {
            CartLine? line = Lines.Where(p => p.Product.ProId == product.ProId).FirstOrDefault();
            if (line == null)
            {
                Lines.Add(new CartLine { Product = product, Quantity = Quantity });
            }
            else
            {
                line.Quantity += Quantity;
            }
        }
        public void RemoveLine(Product product) => Lines.RemoveAll(l => l.Product.ProId == product.ProId);
        public double? ComputeTotalValue() => (double)Lines.Sum(e => e.Product.Price * e.Quantity);
        public void Clear() => Lines.Clear();
    }
    public class CartLine
    {
        public int CartLineID { get; set; }
        public Product Product { get; set; } = new();
        public int Quantity { get; set; }
    }
        
}
