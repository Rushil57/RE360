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
        public bool IsNonStandard { get; set; }
        public decimal? FirstlyFee { get; set; }
        public decimal? Secondly { get; set; }
        public decimal? OnTheFirst { get; set; }
        public decimal? Thirdly { get; set; }
        public decimal? SecondlyTwo { get; set; }
        public decimal? WithMinimumCommission { get; set; }
        public decimal? EstimatedCommission { get; set; }
        public bool IsPercentageOfTheSalePrice { get; set; }
        public bool IsFlatCommission { get; set; }

        public bool IsAppraisedValue { get; set; }
        public bool IsUnAppraisedClientAskingPrice { get; set; }
        public decimal? SalePrice { get; set; }

        public decimal? EstimatedCommissionIncGST { get; set; }
        
    }
}
