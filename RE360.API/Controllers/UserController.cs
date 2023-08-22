using RE360.API.Auth;
using RE360.API.Common;
using RE360.API.Models;
using RE360.API.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Azure.Core.HttpHeader;
using static System.Net.WebRequestMethods;
using RE360.API.DBModels;
using System;
using RE360WebApp.Model;
using RE360.API.Domain;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace RE360.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticateRepository _authenticateRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public string UserImgPath;
        private readonly IConfiguration _configuration;
        CommonMethod common;
        private readonly RE360AppDbContext _context;
        private readonly IMapper _mapper;
        public UserController(IMapper mapper, IAuthenticateRepository authenticateRepository, UserManager<ApplicationUser> userManager, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, RE360AppDbContext dbContext)
        {
            _mapper = mapper;
            _authenticateRepository = authenticateRepository;
            _context = dbContext;
            _userManager = userManager;
            _configuration = configuration;
            UserImgPath = Path.Combine(env.ContentRootPath, "UserImages");
            common = new CommonMethod(dbContext);

        }

        /// <summary>
        /// Get User Details.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {        
        ///       "UserId": "565e602b-6818-406c-b470-5d162d71de77"       
        ///     }
        ///     
        /// </remarks>
        [HttpPost]
        [Route("GetUserProfile")]
        public async Task<IActionResult> GetUserProfile([FromBody] GetUserProfileRequestModel model)
        {
            try
            {
                var filePath = "";
                if (!string.IsNullOrEmpty(model.UserId))
                {
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
                        string Agentid = user.Id;
                        Guid guid;
                        if (Guid.TryParse(Agentid, out guid))
                        {
                            Console.WriteLine(guid);
                        }
                        return StatusCode(StatusCodes.Status200OK, new APIResponseModel
                        {
                            StatusCode = StatusCodes.Status200OK,
                            Message = "Success",
                            Result = new
                            {
                                FirstName = string.IsNullOrEmpty(user.FirstName) ? "" : user.FirstName,
                                LastName = string.IsNullOrEmpty(user.LastName) ? "" : user.LastName,
                                Email = user.Email,
                                ProfileUrl = filePath,
                                ProfileImageData = profileData,
                                BaseAmount = user.BaseAmount,
                                CompanyName = user.CompanyName,
                                ManagerEmail = user.ManagerEmail,
                                MinimumCommission = user.MinimumCommission,
                                OffinceName = user.OffinceName,
                                SalePricePercentage = user.SalePricePercentage,
                                CommissionDetail = _context.CommissionDetails.Where(o => o.AgentID == guid)
                            }
                        });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "User details does not found" });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "User Id is required" });
                }

            }
            catch (Exception ex)
            {
                CommonDBHelper.ErrorLog("UserController - GetUserProfile", ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Something Went Wrong." });
            }
        }

        /// <summary>
        ///  Delete User's Account.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {        
        ///       "UserId": "565e602b-6818-406c-b470-5d162d71de77"       
        ///     }
        ///     
        /// </remarks>
        [HttpPost]
        [Route("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount([FromBody] GetUserProfileRequestModel model)
        {
            string errStr = "";
            try
            {
                if (!string.IsNullOrEmpty(model.UserId))
                {
                    var user = await _userManager.FindByIdAsync(model.UserId.Trim());
                    if (user != null)
                    {
                        var result = await _userManager.DeleteAsync(user);
                        if (!result.Succeeded)
                        {
                            errStr = "User deleted failed! Please check user details and try again.";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(user.ProfileUrl))
                            {
                                //await common.DeleteBlobFile(user.ProfileUrl, "userimages");
                            }

                            return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "User deleted successfully" });
                        }
                    }
                    else
                    {
                        errStr = "User Details not found";
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = string.IsNullOrEmpty(errStr) ? "UserId is required" : errStr });
            }
            catch (Exception ex)
            {
                CommonDBHelper.ErrorLog("UserController - DeleteAccount", ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Something Went Wrong." });
            }
        }

        /// <summary>
        ///  Update User Details.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Id:0ca89f59-d98f-4b20-8876-b92ea0fb70b2 
        ///     FirstName:Riya 
        ///     LastName:Parmar    
        ///     
        /// </remarks>
        [HttpPost]
        [Route("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UserModel model)
        {
            try
            {
                var oldFilename = "";
                bool isfileChange = false;
                var user = await _userManager.FindByIdAsync(model.Id.Trim());
                if (user != null)
                {
                    user.FirstName = model.FirstName.Trim();
                    user.LastName = model.LastName.Trim();
                    oldFilename = string.IsNullOrEmpty(user.ProfileUrl) ? "" : user.ProfileUrl;
                    if (model.ProfileImage != null)
                    {
                        if (model.ProfileImage.Length > 0)
                        {
                            //user.ProfileUrl = await common.UploadBlobUserFile(model.ProfileImage, "userimages");
                            isfileChange = true;
                        }
                    }
                    else
                    {
                        user.ProfileUrl = oldFilename;
                    }
                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "User Updated failed! Please check user details and try again" });
                    }
                    else
                    {
                        if (isfileChange)
                        {
                            if (!string.IsNullOrEmpty(oldFilename))
                            {
                                //await common.DeleteBlobFile(oldFilename, "userimages");
                            }
                        }
                        return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "User Updated successfully!" });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "User Details not found." });
                }
            }
            catch (Exception ex)
            {
                CommonDBHelper.ErrorLog("UserController - UpdateProfile", ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Something Went Wrong." });
            }
        }

        /// <summary>
        ///  Change User Password.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {        
        ///       "UserId": "565e602b-6818-406c-b470-5d162d71de77"       
        ///       "CurrentPassword": "Test@123",
        ///       "NewPassword": "Testing@123"
        ///     }
        ///     
        /// </remarks>
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            string strMessage = "";
            try
            {
                if (!string.IsNullOrEmpty(model.Id))
                {
                    if (!string.IsNullOrEmpty(model.CurrentPassword))
                    {
                        if (!string.IsNullOrEmpty(model.NewPassword))
                        {
                            var user = await _userManager.FindByIdAsync(model.Id.Trim());
                            if (user != null)
                            {
                                if (await _userManager.CheckPasswordAsync(user, model.CurrentPassword))
                                {
                                    if (model.CurrentPassword.Trim() == model.NewPassword.Trim())
                                    {
                                        strMessage = "Current password and new password can not be same";
                                    }
                                    else
                                    {
                                        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword.Trim(), model.NewPassword.Trim());
                                        if (!result.Succeeded)
                                        {
                                            string msg = "Password should follow below criteria:\n";
                                            var strings = result.Errors.ToList().Select(x => x.Description).ToList();
                                            strMessage = msg + string.Join("\n", strings);
                                        }
                                        else
                                        {
                                            return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Password updated successfully" });
                                        }
                                    }
                                }
                                else
                                {
                                    strMessage = "Current password is not valid";
                                }
                            }
                            else
                            {
                                strMessage = "User details not found";
                            }
                        }
                        else
                        {
                            strMessage = "New password is required";
                        }
                    }
                    else
                    {
                        strMessage = "Current password is required";
                    }
                }
                else
                {
                    strMessage = "Id is required";
                }
                return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = string.IsNullOrEmpty(strMessage) ? "Something Went Wrong" : strMessage });
            }
            catch (Exception ex)
            {
                CommonDBHelper.ErrorLog("UserController - ChangePassword", ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Something Went Wrong" });
            }
        }

        /// <summary>
        ///  Remove Token.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {        
        ///       "UserId": "565e602b-6818-406c-b470-5d162d71de77"       
        ///     }
        ///     
        /// </remarks>
        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout([FromBody] GetUserProfileRequestModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.UserId))
                {
                    var user = await _userManager.FindByIdAsync(model.UserId.Trim());
                    if (user != null)
                    {
                        user.FCMToken = "";
                        await _userManager.UpdateAsync(user);
                        await _userManager.UpdateSecurityStampAsync(user);
                        var result = await _userManager.RemoveAuthenticationTokenAsync(user, "JWT", "JWT Token");
                        if (result.Succeeded)
                            return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success" });
                    }
                    return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "User detail not found" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "User Id is required" });
                }
            }
            catch (Exception ex)
            {
                CommonDBHelper.ErrorLog("UserController - Logout", ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Something Went Wrong." });
            }
        }

        //[HttpPost]
        //[Route("GetAgentByID")]
        //public async Task<IActionResult> GetAgentByID(string AgentID)
        //{
        //    UserDetailModel userDetailModel = new UserDetailModel();
        //    try
        //    {
        //        if (AgentID == "0")
        //        {
        //            return StatusCode(StatusCodes.Status200OK, new APIResponseModel
        //            {
        //                StatusCode = StatusCodes.Status200OK,
        //                Message = "Success",
        //                Result = userDetailModel
        //            });
        //        }
        //        if (!string.IsNullOrEmpty(AgentID))
        //        {
        //            var user = await _userManager.FindByIdAsync(AgentID.Trim());
        //            if (user != null)
        //            {
        //                Guid guAgentID;
        //                Guid.TryParse(user.Id, out guAgentID);
        //                userDetailModel.AgentID = user.Id;
        //                userDetailModel.Email = user.Email;
        //                userDetailModel.FirstName = user.FirstName;
        //                userDetailModel.LastName = user.LastName;
        //                userDetailModel.CompanyName = user.CompanyName;
        //                userDetailModel.OffinceName = user.OffinceName;
        //                userDetailModel.ManagerEmail = user.ManagerEmail;
        //                userDetailModel.BaseAmount = Convert.ToDecimal(user.BaseAmount);
        //                userDetailModel.SalePricePercantage = Convert.ToInt32(user.SalePricePercentage);
        //                userDetailModel.MinimumCommission = Convert.ToDecimal(user.MinimumCommission);
        //                userDetailModel.Commisions = _context.CommissionDetails.Where(o => o.AgentID == guAgentID).ToList();
        //                return StatusCode(StatusCodes.Status200OK, new APIResponseModel
        //                {
        //                    StatusCode = StatusCodes.Status200OK,
        //                    Message = "Success",
        //                    Result = userDetailModel
        //                });
        //            }
        //            else
        //            {
        //                return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "User details does not found" });
        //            }
        //        }
        //        else
        //        {
        //            return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "User Id is required" });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        CommonDBHelper.ErrorLog("UserController - GetUserProfile", ex.Message, ex.StackTrace);
        //        return StatusCode(StatusCodes.Status500InternalServerError, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Something Went Wrong." });
        //    }
        //}

        //[HttpPost]
        //[Route("ADDUpdateAgent")]
        //public async Task<IActionResult> ADDUpdateAgent([FromBody] UserDetailModel model)
        //{
        //    string ErrMsg = "";
        //    try
        //    {
        //        if (string.IsNullOrEmpty(model.AgentID.ToString()) || model.AgentID.ToString() == "0")
        //        {
        //            var checkEmail = await _userManager.FindByEmailAsync(model.Email.ToString().Trim());
        //            if (checkEmail == null)
        //            {
        //                ApplicationUser user = new()
        //                {
        //                    Email = model.Email,
        //                    SecurityStamp = Guid.NewGuid().ToString(),
        //                    UserName = model.Email,
        //                    FirstName = model.FirstName,
        //                    LastName = model.LastName,
        //                    CreatedDateTime = DateTime.UtcNow,
        //                    CompanyName = model.CompanyName,
        //                    ManagerEmail = model.ManagerEmail,
        //                    OffinceName = model.OffinceName,
        //                    BaseAmount = model.BaseAmount,
        //                    SalePricePercentage = model.SalePricePercantage,
        //                    MinimumCommission = model.MinimumCommission,
        //                };
        //                string Password = "Test@123";
        //                var result = await _userManager.CreateAsync(user, Password);
        //                if (result != null)
        //                {
        //                    Guid guAgentID;
        //                    Guid.TryParse(user.Id, out guAgentID);
        //                    if (model.Commisions.Count() > 0)
        //                    {
        //                        foreach (var item in model.Commisions)
        //                        {
        //                            item.AgentID = guAgentID;
        //                        }
        //                        _context.CommissionDetails.AddRange(model.Commisions);
        //                        _context.SaveChanges();
        //                    }
        //                    ErrMsg = "User inserted successfully!";
        //                }
        //            }
        //            else
        //            {
        //                ErrMsg = "Email ID is already Exist!";
        //            }
        //        }
        //        else
        //        {
        //            Guid guAgentID;
        //            Guid.TryParse(model.AgentID, out guAgentID);
        //            var user = await _userManager.FindByIdAsync(model.AgentID.ToString().Trim());
        //            if (user != null)
        //            {
        //                user.FirstName = model.FirstName.Trim();
        //                user.LastName = model.LastName.Trim();
        //                user.Email = model.Email;
        //                user.SecurityStamp = Guid.NewGuid().ToString();
        //                user.UserName = model.Email;
        //                user.FirstName = model.FirstName;
        //                user.LastName = model.LastName;
        //                user.CreatedDateTime = DateTime.UtcNow;
        //                user.CompanyName = model.CompanyName;
        //                user.ManagerEmail = model.ManagerEmail;
        //                user.OffinceName = model.OffinceName;
        //                user.BaseAmount = model.BaseAmount;
        //                user.SalePricePercentage = model.SalePricePercantage;
        //                user.MinimumCommission = model.MinimumCommission;

        //                var result = await _userManager.UpdateAsync(user);
        //                if (!result.Succeeded)
        //                {
        //                    ErrMsg = "User Updated failed! Please check user details and try again";
        //                }
        //                else
        //                {
        //                    foreach (var item in model.Commisions)
        //                    {
        //                        item.AgentID = guAgentID;
        //                    }
        //                    if (_context.CommissionDetails.Where(o => o.AgentID == guAgentID).Count() > 0)
        //                    {
        //                        _context.CommissionDetails.RemoveRange(_context.CommissionDetails.Where(o => o.AgentID == guAgentID));
        //                    }
        //                    if (model.Commisions.Count() > 0)
        //                    {
        //                        _context.CommissionDetails.AddRange(model.Commisions);
        //                    }
        //                    _context.SaveChanges();
        //                    ErrMsg = "User Updated successfully!";
        //                }
        //            }
        //            else
        //            {
        //                ErrMsg = "User Details not found.";
        //            }
        //        }
        //        return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = string.IsNullOrEmpty(ErrMsg) ? "Something Went Wrong." : ErrMsg });

        //    }
        //    catch (Exception ex)
        //    {
        //        CommonDBHelper.ErrorLog("UserController - ADDUpdateAgent", ex.Message, ex.StackTrace);
        //        return StatusCode(StatusCodes.Status500InternalServerError, new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Something Went Wrong." });
        //    }
        //}

        //[HttpPost]
        //[Route("GetAgentReport")]
        //public async Task<IActionResult> GetAgentReport()
        //{
        //    List<UserDetailModel> userlist = new List<UserDetailModel>();
        //    try
        //    {
        //        var user = _context.Users.ToList();
        //        if (user != null)
        //        {
        //            foreach (var item in user)
        //            {
        //                userlist.Add(new UserDetailModel
        //                {
        //                    AgentID = item.Id,
        //                    Email = item.Email,
        //                    FirstName = item.FirstName + " " + item.LastName,
        //                    CompanyName = item.CompanyName,
        //                    OffinceName = item.OffinceName,
        //                    ManagerEmail = item.ManagerEmail,
        //                });
        //            }
        //            return StatusCode(StatusCodes.Status200OK, new APIResponseModel
        //            {
        //                StatusCode = StatusCodes.Status200OK,
        //                Message = "Success",
        //                Result = userlist
        //            });
        //        }
        //        else
        //        {
        //            return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "User details does not found" });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        CommonDBHelper.ErrorLog("UserController - GetUserProfile", ex.Message, ex.StackTrace);
        //        return StatusCode(StatusCodes.Status500InternalServerError, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Something Went Wrong." });
        //    }
        //}

        //[HttpPost]
        //[Route("DeleteAgentByID")]
        //public async Task<IActionResult> DeleteAgentByID(string AgentID)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(AgentID))
        //        {
        //            var user = await _userManager.FindByIdAsync(AgentID.Trim());
        //            if (user != null)
        //            {
        //                Guid guAgentID;
        //                Guid.TryParse(user.Id, out guAgentID);
        //                var res = _userManager.DeleteAsync(user);
        //                return StatusCode(StatusCodes.Status200OK, new APIResponseModel
        //                {
        //                    StatusCode = StatusCodes.Status200OK,
        //                    Message = "User Deleted successfully!"
        //                });
        //            }
        //            else
        //            {
        //                return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "User details does not found" });
        //            }
        //        }
        //        else
        //        {
        //            return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "User Id is required" });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        CommonDBHelper.ErrorLog("UserController - GetUserProfile", ex.Message, ex.StackTrace);
        //        return StatusCode(StatusCodes.Status500InternalServerError, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Something Went Wrong." });
        //    }
        //}
    }
}
