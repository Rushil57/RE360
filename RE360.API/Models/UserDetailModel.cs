using RE360.API.DBModels;
using System.ComponentModel.DataAnnotations;

namespace RE360WebApp.Model
{
    public class UserDetailModel
    {
        public string? AgentID { get; set; } = "";
        public string Email { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string CompanyName { get; set; }
        public string OffinceName { get; set; }
        public string? ManagerEmail { get; set; }
        public decimal? BaseAmount { get; set; }
        public decimal? SalePricePercantage { get; set; }
        public decimal? MinimumCommission { get; set; }
        public List<CommissionDetails> Commisions { get; set; } = new List<CommissionDetails>() { };

    }
    public class CommissionDetailsModel
    {
        public int ID { get; set; }
        public Guid AgentID { get; set; }
        public decimal? Percent { get; set; }
        public decimal? UpToAmount { get; set; }
        public int Sequence { get; set; }

    }
}
