namespace HamperStore.Core.Entities
{
    public class HamperImage
    {
        public int Id { get; set; }
        public int HamperId { get; set; }
        public Hamper Hamper { get; set; } = null!;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public int SortOrder { get; set; }
    }
}
