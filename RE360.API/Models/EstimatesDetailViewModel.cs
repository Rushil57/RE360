using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class EstimatesDetailViewModel
    {
        public int ID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public string? AdditionalDisclosures { get; set; }
    }
}
