using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RE360.API.DBModels
{
    public class CalculationOfCommission
    {
        public int ID { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
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
        public decimal? BaseAmount { get; set; }
        public int? SalePricePercentage { get; set; }

    }
}
