using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RE360.API.Models
{
    public class SignaturesOfClientViewModel
    {
        public int ID { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public int ClientId { get; set; }

        public IFormFile? SignatureClient { get; set; }
        public string? SignatureOfClientName { get; set; }
    }
}
