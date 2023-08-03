using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class PropertyInformationViewModel
    {
        public int ID { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public int PropAttrId { get; set; }
        public int? Count { get; set; }
        public string? Remarks { get; set; }
    }
}
