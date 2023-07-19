using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class SolicitorDetailViewModel
    {
        public int ID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public string? Firm { get; set; }
        public string? IndividualActing { get; set;}
        public string? Phone { get; set; }
        public string? EmailID { get; set; }
        public string? Address { get; set; }

    }
}
