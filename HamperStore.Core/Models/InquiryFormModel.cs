using System.ComponentModel.DataAnnotations;

namespace HamperStore.Web.Models
{
    public class InquiryFormModel
    {
        public int? HamperId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [Required]
        public int CityId { get; set; }

        [StringLength(100)]
        public string? Occasion { get; set; }

        [Range(0, 1000000)]
        public decimal? Budget { get; set; }

        [StringLength(1000)]
        public string? Message { get; set; }
    }
}
