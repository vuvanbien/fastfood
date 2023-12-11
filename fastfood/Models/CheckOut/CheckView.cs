namespace fastfood.Models.CheckOut
{
    public class CheckView
    {
        public Cart? Cart { get; set; }
        public Customer? Customer { get; set; }
        public Order? Order { get; set; }
        public OrderDetail? OrderDetail { get; set; }
    }
}
