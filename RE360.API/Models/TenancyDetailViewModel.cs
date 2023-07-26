using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class TenancyDetailViewModel
    {
        public int ID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public bool IsVacant { get; set; }
        public bool IsTananted { get; set; }
        public DateTime? TenancyStartDate { get; set; }
        public DateTime? TenancyEndDate { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? TenancyDetails { get; set; }
        public bool IsToBeAdvised { get; set; }
    }
}
