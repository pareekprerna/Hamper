namespace HamperStore.Core.Entities
{
    public enum InquiryStatus { New, Contacted, Converted, Closed }

    public class Inquiry
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public int? HamperId { get; set; }
        public Hamper? Hamper { get; set; }

        public int CityId { get; set; }
        public City City { get; set; } = null!;

        public string? Occasion { get; set; }
        public decimal? Budget { get; set; }
        public string? Message { get; set; }
        public InquiryStatus Status { get; set; } = InquiryStatus.New;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<InquiryItem> InquiryItems { get; set; } = new List<InquiryItem>();
        public Order? Order { get; set; }
    }
}
