namespace HamperStore.Core.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public decimal DeliveryFee { get; set; }
        public int DeliveryDays { get; set; }

        public ICollection<Hamper> Hampers { get; set; } = new List<Hamper>();
        public ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
