using RE360.API.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace RE360.API.Domain
{
    public interface IUserRepository
    {
        Task<object> GetUserProfile(GetUserProfileRequestModel model);
        //Task<string> DeleteAccount(GetUserProfileRequestModel model);
    }
}
