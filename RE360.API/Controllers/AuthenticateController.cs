using RE360.API.Auth;
using RE360.API.Domain;
using RE360.API.Models;
using RE360.API.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Azure.Core.HttpHeader;
using RE360WebApp.Model;
using RE360.API.Common;

namespace RE360.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticateController : ControllerBase
{
    private readonly IAuthenticateRepository _authenticateRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RE360AppDbContext _context;
    CommonMethod common;
    public AuthenticateController(
        IAuthenticateRepository authenticateRepository, UserManager<ApplicationUser> userManager, RE360AppDbContext context)
    {
        _authenticateRepository = authenticateRepository;
        _userManager = userManager;
        _context = context;
        common = new CommonMethod(context);
    }


    /// <summary>
    /// Validate User’s Credentials and Return Token.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     {        
    ///       "Username": "user.avidclan@gmail.com",
    ///       "Password": "Test@123",
    ///       "FCMToken": "ZmNXw9mT1KfKb2gS1G15f:APA91bHF15E2I3f80U4Ry21lswfgfnIByeRTxgLINBJCBqBQTFnmtU46QAdbZDDuNBJQ6rbjAw69gb3tS0icM0-tm5rgcXO-_79MffPpGUDTs8abAeSff-JiZ86_8SRYfVVT_ol1Hf6R",
    ///       "Device": "A"
    ///     }
    /// </remarks>
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {

        try
        {
            var result = await _authenticateRepository.Login(model);
            var statuscode = (int)result.StatusCode;
            return StatusCode(statuscode, result);
        }
        catch (Exception ex)
        {
            //return Ok(new { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message + "-->" + ex.StackTrace });
            CommonDBHelper.ErrorLog("CommonDBHelper - GetDataSet", ex.Message, ex.StackTrace);
            return Ok(new { StatusCode = StatusCodes.Status403Forbidden, Message = "Something Went Wrong" });
            throw;
        }
    }

    /// <summary>
    /// New User Registration.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     Email:user.avidclan@gmail.com
    ///     FirstName:userfirstname 
    ///     LastName:test
    ///     Password:Test@123
    ///     
    /// </remarks>
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromForm] RegisterModel model)
    {

        var result = await _authenticateRepository.Register(model);
        var statuscode = (int)result.StatusCode;
        return StatusCode(statuscode, result);
    }

    /// <summary>
    /// Generate a New Access Token
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     {
    ///         "AccessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoidGVzdEBnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjBjYTg5ZjU5LWQ5OGYtNGIyMC04ODc2LWI5MmVhMGZiNzBiMiIsImp0aSI6ImZkODZmMTIxLWYyYjgtNDZlOS1hNmVmLTFlZWVhNDQwYWQ4YiIsImV4cCI6MTY4MDA4NzA4NCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo0MjAwIn0.3SNEgBgTFgDmOXg-zBPJ9sGKRAFrzPHnhu_Z0_Y6nuw",
    ///         "RefreshToken": "7Hn1cGy2r+/z9fdKIL1j0kZ7MWjV1n7NKnkYJ+bE4MvTfgpl5ORoX7aUQRr+c/C00MhLcYUISjaqi8ofZO7bOw=="
    ///     }
    ///     
    /// </remarks>
    [HttpPost]
    [Route("Refresh-Token")]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {

        var result = await _authenticateRepository.RefreshToken(tokenModel);
        var statuscode = (int)result.StatusCode;
        return StatusCode(statuscode, result);
    }



    /// <summary>
    /// Regenerate Password of User and Send it to an existing Email Id
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     {
    ///         "UserEmail": "user.avidclan@gmail.com"
    ///     }
    ///     
    /// </remarks>
    [HttpPost]
    [Route("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel model)
    {
        var result = await _authenticateRepository.ForgotPassword(model);
        var statuscode = (int)result.StatusCode;
        return StatusCode(statuscode, result);

    }

    [HttpPost]
    [Route("GetAgentByID")]
    public async Task<IActionResult> GetAgentByID(string AgentID)
    {
        UserDetailModel userDetailModel = new UserDetailModel();
        try
        {
            if (AgentID == "0")
            {
                return StatusCode(StatusCodes.Status200OK, new APIResponseModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success",
                    Result = userDetailModel
                });
            }
            if (!string.IsNullOrEmpty(AgentID))
            {
                var user = await _userManager.FindByIdAsync(AgentID.Trim());
                if (user != null)
                {
                    Guid guAgentID;
                    Guid.TryParse(user.Id, out guAgentID);
                    userDetailModel.AgentID = user.Id;
                    userDetailModel.Email = user.Email;
                    userDetailModel.FirstName = user.FirstName;
                    userDetailModel.LastName = user.LastName;
                    userDetailModel.CompanyName = user.CompanyName;
                    userDetailModel.OffinceName = user.OffinceName;
                    userDetailModel.ManagerEmail = user.ManagerEmail;
                    userDetailModel.BaseAmount = Convert.ToDecimal(user.BaseAmount);
                    userDetailModel.SalePricePercantage = Convert.ToDecimal(user.SalePricePercentage);
                    userDetailModel.MinimumCommission = Convert.ToDecimal(user.MinimumCommission);
                    userDetailModel.Commisions = _context.CommissionDetails.Where(o => o.AgentID == guAgentID).ToList();
                    return StatusCode(StatusCodes.Status200OK, new APIResponseModel
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Success",
                        Result = userDetailModel
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
            CommonDBHelper.ErrorLog("UserController - GetAgentByID", ex.Message, ex.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Something Went Wrong." });
        }
    }

    [HttpPost]
    [Route("ADDUpdateAgent")]
    public async Task<IActionResult> ADDUpdateAgent([FromBody] UserDetailModel model)
    {
        string ErrMsg = "";
        try
        {
            if (string.IsNullOrEmpty(model.AgentID.ToString()) || model.AgentID.ToString() == "0")
            {
                var checkEmail = await _userManager.FindByEmailAsync(model.Email.ToString().Trim());
                if (checkEmail == null)
                {
                    ApplicationUser user = new()
                    {
                        Email = model.Email,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        CreatedDateTime = DateTime.UtcNow,
                        CompanyName = model.CompanyName,
                        ManagerEmail = model.ManagerEmail,
                        OffinceName = model.OffinceName,
                        BaseAmount = model.BaseAmount,
                        SalePricePercentage = model.SalePricePercantage,
                        MinimumCommission = model.MinimumCommission,
                        IsPasswordChange = true
                    };
                    string Password = common.GenerateRandomPassword();
                    //string Password = "Test@123";
                    var result = await _userManager.CreateAsync(user, Password);
                    if (result != null)
                    {
                        Guid guAgentID;
                        Guid.TryParse(user.Id, out guAgentID);
                        if (model.Commisions.Count() > 0)
                        {
                            foreach (var item in model.Commisions)
                            {
                                item.AgentID = guAgentID;
                            }
                            _context.CommissionDetails.AddRange(model.Commisions);
                            _context.SaveChanges();
                        }
                        var mailSend = common.SendMailForAgentRegi(model.Email, model.FirstName + " " + model.LastName, Password);
                        //if (mailSend.Result == "Mail Sent Successfully")
                        //{
                        //    return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "We have sent an updated password to your registered email address. You can use that password and we recommend updating the password after login from the mobile application" };
                        //}
                        //else
                        //{
                        //    return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Mail not sent" };
                        //}
                        return StatusCode(StatusCodes.Status200OK, new APIResponseModel
                        {
                            StatusCode = StatusCodes.Status200OK,
                            Message = "User inserted successfully!"
                        });
                    }
                }
                else
                {
                    ErrMsg = "Email ID is already Exist!";
                }
            }
            else
            {
                Guid guAgentID;
                Guid.TryParse(model.AgentID, out guAgentID);
                var user = await _userManager.FindByIdAsync(model.AgentID.ToString().Trim());
                if (user != null)
                {
                    user.FirstName = model.FirstName.Trim();
                    user.LastName = model.LastName.Trim();
                    user.Email = model.Email;
                    user.SecurityStamp = Guid.NewGuid().ToString();
                    user.UserName = model.Email;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.CreatedDateTime = DateTime.UtcNow;
                    user.CompanyName = model.CompanyName;
                    user.ManagerEmail = model.ManagerEmail;
                    user.OffinceName = model.OffinceName;
                    user.BaseAmount = model.BaseAmount;
                    user.SalePricePercentage = model.SalePricePercantage;
                    user.MinimumCommission = model.MinimumCommission;

                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        ErrMsg = "User Updated failed! Please check user details and try again";
                    }
                    else
                    {
                        foreach (var item in model.Commisions)
                        {
                            item.AgentID = guAgentID;
                        }
                        if (_context.CommissionDetails.Where(o => o.AgentID == guAgentID).Count() > 0)
                        {
                            _context.CommissionDetails.RemoveRange(_context.CommissionDetails.Where(o => o.AgentID == guAgentID));
                        }
                        if (model.Commisions.Count() > 0)
                        {
                            _context.CommissionDetails.AddRange(model.Commisions);
                        }
                        _context.SaveChanges();
                        return StatusCode(StatusCodes.Status200OK, new APIResponseModel
                        {
                            StatusCode = StatusCodes.Status200OK,
                            Message = "User Updated successfully!"
                        });

                    }
                }
                else
                {
                    ErrMsg = "User Details not found.";
                }
            }
            return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = string.IsNullOrEmpty(ErrMsg) ? "Something Went Wrong." : ErrMsg });

        }
        catch (Exception ex)
        {
            CommonDBHelper.ErrorLog("UserController - ADDUpdateAgent", ex.Message, ex.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Something Went Wrong." });
        }
    }

    [HttpPost]
    [Route("GetAgentReport")]
    public async Task<IActionResult> GetAgentReport()
    {
        List<UserDetailModel> userlist = new List<UserDetailModel>();
        try
        {
            var user = _context.Users.ToList();
            if (user != null)
            {
                foreach (var item in user)
                {
                    userlist.Add(new UserDetailModel
                    {
                        AgentID = item.Id,
                        Email = item.Email,
                        FirstName = item.FirstName + " " + item.LastName,
                        CompanyName = item.CompanyName,
                        OffinceName = item.OffinceName,
                        ManagerEmail = item.ManagerEmail,
                    });
                }
                return StatusCode(StatusCodes.Status200OK, new APIResponseModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success",
                    Result = userlist
                });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "User details does not found" });
            }

        }
        catch (Exception ex)
        {
            CommonDBHelper.ErrorLog("UserController - GetAgentReport", ex.Message, ex.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Something Went Wrong." });
        }
    }

    [HttpPost]
    [Route("DeleteAgentByID")]
    public async Task<IActionResult> DeleteAgentByID(string AgentID)
    {
        try
        {
            if (!string.IsNullOrEmpty(AgentID))
            {
                var user = await _userManager.FindByIdAsync(AgentID.Trim());
                if (user != null)
                {
                    Guid guAgentID;
                    Guid.TryParse(user.Id, out guAgentID);
                    var res = _userManager.DeleteAsync(user);
                    return StatusCode(StatusCodes.Status200OK, new APIResponseModel
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "User Deleted successfully!"
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
            CommonDBHelper.ErrorLog("UserController - DeleteAgentByID", ex.Message, ex.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError, new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Something Went Wrong." });
        }
    }
}