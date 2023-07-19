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

namespace RE360.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticateController : ControllerBase
{
    private readonly IAuthenticateRepository _authenticateRepository;

    public AuthenticateController(
        IAuthenticateRepository authenticateRepository)
    {
        _authenticateRepository= authenticateRepository;
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

        var result= await _authenticateRepository.Login(model);
        var statuscode = (int)result.StatusCode;
        return StatusCode(statuscode, result);
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
}