using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class ContractRateViewModel
    {
        public int ID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public decimal? Water { get; set; }
        public decimal? Council { get; set; }
        public bool IsPerAnnum { get; set; }
        public bool IsPerQuarter { get; set; }
        public decimal? OtherValue { get; set; }
    }
}
