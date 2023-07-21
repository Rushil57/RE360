using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RE360.API.DBModels
{
    public class SignaturesOfClient
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int ExecutionId { get; set; }
        public int ClientId { get; set; }
        public string? SignatureOfClient { get; set; }
    }
}
