using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class ContractDetailViewModel
    {
        public int ID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public DateTime? AuthorityStartDate { get; set; }
        public DateTime? AuthorityEndDate { get; set; }
        public string? AgreedMarketSpend { get; set; }
    }
}
