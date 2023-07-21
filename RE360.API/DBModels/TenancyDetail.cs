using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RE360.API.DBModels
{
    public class TenancyDetail
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PID { get; set; }
        public bool IsVacant { get; set; }
        public bool IsTananted { get; set; }
        public DateTime? Date { get; set; }
        public string? Time { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? TenancyDetails { get; set;}
        public bool IsToBeAdvised { get; set; }

    }
}
