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
        public bool IsPA { get; set; }
        public bool IsPQ { get; set; }
        public decimal? PQValue { get; set; }
    }
}
