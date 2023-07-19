using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class LegalDetailViewModel
    {
        public int ID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public int TitleTypeID { get; set; }
        public string? LOT { get; set; }
        public string? DP { get; set; }
        public string? Title { get; set; }
        public bool IsPropertyUnitTitle { get; set; }
        public string? RegisteredOwner { get; set; }
        public string? AdditionalDetails { get; set; }
        public decimal? LandValue { get; set; }
        public decimal? ImprovementValue { get; set; }
        public decimal? RateableValue { get; set; }
        public DateTime? RatingValuationDate { get; set; }
    }
}
