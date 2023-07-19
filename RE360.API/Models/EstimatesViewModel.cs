using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class EstimatesViewModel
    {
        public int ID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public string? ExpensesToBeIncurred { get; set; }
        public decimal? ProviderDiscountCommission { get; set; }
        public decimal? AmountDiscountCommission { get; set; }

        public bool TickHereIfEstimate { get; set; }
    }
}
