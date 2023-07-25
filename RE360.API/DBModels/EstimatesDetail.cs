using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RE360.API.DBModels
{
    public class EstimatesDetail
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PID { get; set; }
        public string? AdditionalDisclosures { get; set; }
    }
}
