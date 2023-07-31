using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RE360.API.DBModels
{
    public class PriorAgencyMarketing
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PID { get; set; }
        public string? AgencyName { get; set; }
        public string? AgencyExpiredDate { get; set; }
        public string? AgencyName1 { get; set; }
        public string? AgencyExpiredDate1 { get; set; }
        public decimal? AgencySum { get;set; }
    
    }
}
