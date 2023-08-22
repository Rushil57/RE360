using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class PropertyInformationDetailViewModel
    {
        public int ID { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public string? Double { get; set; }
        public string? Single { get; set; }
        public string? Comment { get; set; }
        public string? AdditionalFeature { get; set; }
        public string? ExcludedChattels { get; set; }
        public string? InternalRemark { get; set; }
    }

    public class PropertyAttributeTypeWithAllDataViewModel
    {
        public int PropertyAttributeTypeID { get; set; }
        public string? Name { get; set; }
        public int PropertyAttributeID { get; set; }
        public string? PropertyAttributeName { get; set; }
        public bool Checkbox { get; set; }
        public string? Remarks { get; set; }
        public int Count { get; set; }
    }
}
