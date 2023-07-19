using RE360.API.Auth;
using RE360.API.Common;
using RE360.API.Domain;
using RE360.API.Models;
using RE360.API.RequestModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace RE360.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        //public string UserImgPath;
        private readonly IConfiguration _configuration;
        CommonMethod common;
        public UserRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, RE360AppDbContext dbContext)
        {
            _userManager = userManager;
            _configuration = configuration;
            //UserImgPath = Path.Combine(env.ContentRootPath, "UserImages");
            common = new CommonMethod(dbContext);
        }
        public async Task<object> GetUserProfile(GetUserProfileRequestModel model)
        {
            object Result = null;
            try
            {
                var filePath = "";
                var user = await _userManager.FindByIdAsync(model.UserId.Trim());
                if (user != null)
                {
                    var profileData = "";
                    if (!string.IsNullOrEmpty(user.ProfileUrl))
                    {
                        filePath = _configuration["BlobStorageSettings:UserImagesPath"].ToString() + user.ProfileUrl + _configuration["BlobStorageSettings:UserImagesPathToken"].ToString();
                        //if (System.IO.File.Exists(Path.Combine(UserImgPath, user.ProfileUrl)))
                        //{
                        //    Byte[] bytes = System.IO.File.ReadAllBytes(Path.Combine(UserImgPath, user.ProfileUrl));
                        //    profileData = common.ImageCompressed(bytes);
                        //}
                    }
                    Result = new
                    {
                        FirstName = string.IsNullOrEmpty(user.FirstName) ? "" : user.FirstName,
                        LastName = string.IsNullOrEmpty(user.LastName) ? "" : user.LastName,
                        Email = user.Email,
                        ProfileUrl = filePath,
                        ProfileImageData = profileData,
                    };

                }
                return Result;
            }
            catch (Exception ex)
            {
                CommonDBHelper.ErrorLog("UserController - GetUserProfile", ex.Message, ex.StackTrace);
                return null;
            }
        }

        //public async Task<string> DeleteAccount(GetUserProfileRequestModel model)
        //{
        //    string Result = "";
        //    try
        //    {
        //        var user = await _userManager.FindByIdAsync(model.UserId.Trim());
        //        if (user != null)
        //        {
        //            var result = await _userManager.DeleteAsync(user);
        //            if (!result.Succeeded)
        //            {
        //                Result = "0|User deleted failed! Please check user details and try again.";
        //            }
        //            else
        //            {
        //                if (!string.IsNullOrEmpty(user.ProfileUrl))
        //                {
        //                    await common.DeleteBlobFile(user.ProfileUrl, "userimages");
        //                }
        //                Result = "1|User deleted failed! Please check user details and try again.";
        //            }
        //        }
        //        Result = "0|User Details not found";
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonDBHelper.ErrorLog("UserController - DeleteAccount", ex.Message, ex.StackTrace);
        //        Result = "0|Something Went Wrong";
        //    }
        //}
    }
}
