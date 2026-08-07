namespace HamperStore.Core.Entities
{
    public class Hamper
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsCustomizable { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<HamperImage> Images { get; set; } = new List<HamperImage>();
        public ICollection<HamperItem> Items { get; set; } = new List<HamperItem>();
        public ICollection<City> AvailableCities { get; set; } = new List<City>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
