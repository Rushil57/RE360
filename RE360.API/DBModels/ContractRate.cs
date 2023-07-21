using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RE360.API.DBModels
{
    public class ContractRate
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PID { get; set; }
        public decimal? Water { get; set; }
        public decimal? Council { get; set; }
        public bool IsPerAnnum { get; set; }
        public bool IsPerQuarter { get; set; }
        public decimal? OtherValue { get; set; }
    }
}
