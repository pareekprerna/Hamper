namespace HamperStore.Core.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int HamperId { get; set; }
        public Hamper Hamper { get; set; } = null!;

        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }
    }
}
