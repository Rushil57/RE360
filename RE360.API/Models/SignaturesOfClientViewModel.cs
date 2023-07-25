using System.ComponentModel.DataAnnotations.Schema;

namespace RE360.API.Models
{
    public class SignaturesOfClientViewModel
    {
        public int ID { get; set; }
        public int ExecutionId { get; set; }
        public int ClientId { get; set; }

        //public IFormFile? SignatureClient { get; set; }
        public string? SignatureOfClientName { get; set; }
    }
}
