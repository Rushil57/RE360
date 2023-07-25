using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RE360.API.DBModels
{
    public class ListingAddress
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public Guid AgentID { get; set; }
        public string? Address { get; set; }
        public string? Unit { get; set; }
        public string? Suburb { get; set; }
        public string? PostCode { get; set; }
        public string? StreetNumber { get; set; }
        public string? StreetName { get; set; }  
        //public string? AgentName { get; set; }
    }
}
