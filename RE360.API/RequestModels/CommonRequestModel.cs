using RE360.API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RE360.API.RequestModels
{
    public class ForgotPasswordRequestModel
    {
        public string UserEmail { get; set; } = "";
    }
    public class GetUserProfileRequestModel
    {
        public string UserId { get; set; } = "";
    }
    
}
