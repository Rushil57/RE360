using RE360.API.Auth;
using RE360.API.Models;
using RE360.API.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace RE360.API.Domain
{
    public interface IAuthenticateRepository
    {
        Task<APIResponseModel> Login(LoginModel model);

        Task<APIResponseModel> Register(RegisterModel model);

        Task<APIResponseModel> RefreshToken(TokenModel tokenModel);

        Task<APIResponseModel> ForgotPassword(ForgotPasswordRequestModel model);
    }
}
