using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RE360.API.DBModels
{
    public class SolicitorDetail
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PID { get; set; }
        public string? Firm { get; set; }
        public string? IndividualActing { get; set;}
        public string? Phone { get; set; }
        public string? EmailID { get; set; }
        public string? Address { get; set; }

    }
}
