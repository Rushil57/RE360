using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class ClientDetailViewModel
    {
        public int ID { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsCompanyTrust { get; set; }
        public string? CompanyTrustName { get; set; }
        public string? Title { get; set; }
        public string? SurName { get; set; }
        public string? FirstName { get; set; }
        public string? Address { get; set; }
        public string? Unit { get; set; }
        public string? Suburb { get; set; }
        public string? PostCode { get; set; }
        public string? StreetNumber { get; set; }
        public string? StreetName { get; set; }
        public bool IsSameAsListingAddress { get; set; }
        public string? Home { get; set; }
        public string? Mobile { get; set; }
        public string? Business { get; set; }
        public string? Email { get; set; }
        public bool IsGSTRegistered { get; set; }
        public string? GSTNumber { get; set; }
        public string? ContactPerson { get; set; }
        public string? Position { get; set; }
        public bool IsPlusGST { get; set; }
        public bool IsIncGST { get; set; }
        public bool IsStandard { get; set; }
        public bool IsCreateCustComTerm { get; set; }
        public bool IsInCaseOfLessHoldTerm { get; set; }
        public decimal? WithMinimumCommission { get; set; }
        public decimal? EstimatedCommission { get; set; }
        public bool IsPercentageOfTheSalePrice { get; set; }
        public bool IsFlatCommission { get; set; }
        public bool IsAppraisedValue { get; set; }
        public bool IsUnAppraisedClientAskingPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? EstimatedCommissionIncGST { get; set; }


    }

    public class ClientDetailListViewModel
    {
        public List<ClientDetailViewModel> ClientDetails { get; set; }
    }

}
