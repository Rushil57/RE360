using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RE360.API.DBModels
{
    public class ParticularDetail
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PID { get; set; }
        public int? ParticularTypeID { get; set;}
        public string? Bed { get; set; }
        public string? Bath { get; set; }
        public string? Ensuites { get; set; }
        public string? Toilets { get; set; }
        public string? LivingRooms { get; set; }
        public string? StudyRooms { get; set; }
        public string? Dining { get; set; }
        public string? Garages { get; set; }
        public string? Carports { get; set; }
        public string? OpenParkingSpaces { get; set; }
        public bool IsHomeLandPackage { get; set; }
        public bool IsNewConstruction { get; set; }
        public decimal? AprxFloorArea { get; set; }
        public bool IsVerified { get;}
        public bool IsNonVerified { get; }
        //public string? LandArea { get; set; }
        //public bool IsSqm { get; set; }
        //public bool IsHectare { get; set; }
        public string? AprxYearBuilt { get; set; }
        public string? Zoning { get; set; }

    }
}
