using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class PriorAgencyMarketingViewModel
    {
        public int ID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public string? AgencyName { get; set; }
        public string? AgencyPeriod { get; set; }
        public string? AgencyName1 { get; set; }
        public string? AgencyPeriod1 { get; set; }
        public decimal? AgencySum { get; set; }
        public decimal? InitialFee { get; set; }
        public decimal? CommissionOnInitial { get; set; }
        public decimal? OfThePurchasePrice { get; set; }
        public decimal? CommissionOnBalance { get; set; }
        public decimal? WithMinimumCommission { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? EstimatedCommission { get; set; }
        public bool IsAppraisedValue { get; set; }
        public bool IsUnAppraisedClientAskingPrice { get; set; }
    }
}
