using RE360.API.DBModels;
using System.ComponentModel.DataAnnotations;

namespace RE360WebApp.Model
{
    public class UserDetailModel
    {
        public string AgentID { get; set; }
        [Required(ErrorMessage = "Please enter Email")]
        public string Email { get; set; } = "a@a.com";

        [Required(ErrorMessage = "Please enter First Name")]
        public string FirstName { get; set; } = "test data";
        public string LastName { get; set; } = "Last name";

        public string CompanyName { get; set; }
        public string OfficeName { get; set; }
        public string ManagerEmail { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal SalePrice { get; set; }
        public decimal MinCommision { get; set; }
        public List<CommissionDetails> Commisions { get; set; } = new List<CommissionDetails>() { };

    }
}
