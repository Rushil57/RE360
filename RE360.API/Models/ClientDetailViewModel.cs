using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class ClientDetailViewModel
    {
        public int ID { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public string? Title { get; set; }
        public string? SurName { get; set; }
        public string? FirstName { get; set; }
        public string? Address { get; set; }
        public string? PostCode { get; set; }
        public string? Home { get; set; }
        public string? Mobile { get; set; }
        public string? Business { get; set; }
        public string? Email { get; set; }
        public string? CompanyTrust { get; set; }
        public string? Position { get; set; }
        public bool IsGSTRegistered { get; set; } = false;
        public string? GSTNumber { get; set; }

    }
}
