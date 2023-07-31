using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class PriorAgencyMarketingViewModel
    {
        public int ID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public string? AgencyName { get; set; }
        public string? AgencyExpiredDate { get; set; }
        public string? AgencyName1 { get; set; }
        public string? AgencyExpiredDate1 { get; set; }
        public decimal? AgencySum { get; set; }

    }
}
