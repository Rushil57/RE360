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
        public string? LotNo { get; set; }
        public string? DepositedPlan { get; set; }
        public string? TitleIdentifier { get; set; }
        public bool IsPropertyUnitTitle { get; set; }
        public string? RegisteredOwner { get; set; }
        public string? AdditionalDetails { get; set; }
        public decimal? LandValue { get; set; }
        public decimal? ImprovementValue { get; set; }
        public decimal? RateableValue { get; set; }
        public DateTime? RatingValuationDate { get; set; }
        public string? LandArea { get; set; }
        public bool IsSqm { get; set; }
        public bool IsHectare { get; set; }
    }
}
