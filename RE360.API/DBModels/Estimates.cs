using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RE360.API.DBModels
{
    public class Estimates
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PID { get; set; }
        public string? ExpensesToBeIncurred { get; set; }
        public decimal? ProviderDiscountCommission { get; set; }
        public decimal? AmountDiscountCommission { get; set; }

        public bool TickHereIfEstimate { get; set; }

    }
}
