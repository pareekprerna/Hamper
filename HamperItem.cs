namespace HamperStore.Core.Entities
{
    public class HamperItem
    {
        public int Id { get; set; }
        public int? HamperId { get; set; }
        public Hamper? Hamper { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? StockNote { get; set; }

        public ICollection<InquiryItem> InquiryItems { get; set; } = new List<InquiryItem>();
    }
}
