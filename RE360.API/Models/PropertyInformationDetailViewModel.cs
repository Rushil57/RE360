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

    public class PropertyAttributeTypeWithLegalParticularDetailModel
    {
        public int PropertyAttributeTypeID { get; set; }
        public string? Name { get; set; }
        public int PropertyAttributeID { get; set; }
        public string? PropertyAttributeName { get; set; }
        int PID { get; set; }
        int TitleTypeId { get; set; }
        int ParticularTypeID { get; set; }
        public bool Checkbox { get; set; }

    }
    public class PropertyAttributeTypeWithMethodOfSaleModel
    {
        public int PropertyAttributeTypeID { get; set; }
        public string? Name { get; set; }
        public int PropertyAttributeID { get; set; }
        public string? PropertyAttributeName { get; set; }
        int PID { get; set; }
        public bool Checkbox { get; set; }
        public int AgencyTypeID { get; set; }
        public string? AgencyOtherTypeRemark { get; set; }
        public int MethodOfSaleID { get; set; }
        public decimal? Price { get; set; }
        public string? PriceRemark { get; set; }
        public DateTime? AuctionDate { get; set; }
        public string? AuctionTime { get; set; }
        public string? AuctionVenue { get; set; }
        public string? Auctioneer { get; set; }
        public DateTime? TenderDate { get; set; }
        public string? TenderTime { get; set; }
        public DateTime? DeadLineDate { get; set; }
        public string? DeadLineTime { get; set; }
        public bool IsMortgageeSale { get; set; }
        public bool IsAsIs { get; set; }
        public bool IsAuctionUnlessSoldPrior { get; set; }
        public bool IsTenderUnlessSoldPrior { get; set; }
        public bool IsAuctionOnSite { get; set; }
        public string? TenderVenue { get; set; }

    }
}
