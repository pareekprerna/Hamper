namespace HamperStore.Core.Entities
{
    public class InquiryItem
    {
        public int Id { get; set; }
        public int InquiryId { get; set; }
        public Inquiry Inquiry { get; set; } = null!;

        public int HamperItemId { get; set; }
        public HamperItem HamperItem { get; set; } = null!;

        public int Quantity { get; set; } = 1;
    }
}
