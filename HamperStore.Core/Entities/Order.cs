namespace HamperStore.Core.Entities
{
    public enum OrderStatus { Pending, Confirmed, Delivered, Cancelled }

    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public int? InquiryId { get; set; }
        public Inquiry? Inquiry { get; set; }

        public int CityId { get; set; }
        public City City { get; set; } = null!;

        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime? DeliveryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
