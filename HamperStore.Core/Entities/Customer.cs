namespace HamperStore.Core.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }

        public int? PreferredCityId { get; set; }
        public City? PreferredCity { get; set; }

        public ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
