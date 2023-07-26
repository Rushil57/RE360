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
        public string? Unit { get; set; }
        public string? Suburb { get; set; }
        public string? PostCode { get; set; }
        public string? StreetNumber { get; set; }
        public string? StreetName { get; set; }

    }

    public class SolicitorDetailListViewModel
    {
        public List<SolicitorDetailViewModel> SolicitorDetail { get; set; }
    }
}
