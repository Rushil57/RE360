using RE360.API.Auth;
using RE360.API.Common;
using RE360.API.Domain;
using RE360.API.Models;
using RE360.API.RequestModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RE360.API.Repositories
{
    public class AuthenticateRepository : IAuthenticateRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnv;
        CommonMethod common;

        public AuthenticateRepository(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        Microsoft.AspNetCore.Hosting.IHostingEnvironment env, RE360AppDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _hostingEnv = env;
            common = new CommonMethod(dbContext);
        }
        public async Task<APIResponseModel> Login(LoginModel model)
        {
            UserDetail userDetail = new UserDetail();
            string strError = "";
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    var authPass = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (authPass)
                    {
                        if (!string.IsNullOrEmpty(model.FCMToken))
                        {
                            if (!string.IsNullOrEmpty(model.Device))
                            {
                                var userRoles = await _userManager.GetRolesAsync(user);

                                var authClaims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(ClaimTypes.NameIdentifier, user.Id),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            };

                                foreach (var userRole in userRoles)
                                {
                                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                                }

                                var token = GetToken(authClaims);
                                var refreshToken = GenerateRefreshToken();

                                int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                                user.RefreshToken = refreshToken;
                                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);
                                user.FCMToken = model.FCMToken.Trim();
                                user.Device = model.Device.Trim().ToUpper();
                                await _userManager.UpdateAsync(user);
                                userDetail.UserId = user.Id;
                                userDetail.UserName = user.FirstName + " " + user.LastName;
                                userDetail.Email = user.UserName;
                                if (!string.IsNullOrEmpty(user.ProfileUrl))
                                {
                                    userDetail.ProfileImage = _configuration["BlobStorageSettings:UserImagesPath"].ToString() + user.ProfileUrl + _configuration["BlobStorageSettings:UserImagesPathToken"].ToString();
                                }

                                return new APIResponseModel
                                {
                                    StatusCode = StatusCodes.Status200OK,
                                    Message = "Success",
                                    Result = new
                                    {
                                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                                        RefreshToken = refreshToken,
                                        Expiration = token.ValidTo,
                                        RefreshTokenExpiration = user.RefreshTokenExpiryTime.ToUniversalTime(),
                                        Id = user.Id,
                                        UserDetail = userDetail
                                    }
                                };
                            }
                            else
                            {
                                strError = "Device is required";
                            }
                        }
                        else
                        {
                            strError = "FCMToken is required";
                        }
                    }
                    else
                    {
                        strError = "Please enter valid credentials";
                    }
                }
                else
                {
                    strError = "User does not exists with this email address";
                }
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = !string.IsNullOrEmpty(strError) ? strError : "Something Went Wrong" };
            }
            catch (Exception ex)
            {
                CommonDBHelper.ErrorLog("AuthenticateController - Login", ex.Message, ex.StackTrace);
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Something Went Wrong" };
            }
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(24),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<APIResponseModel> Register(RegisterModel model)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(model.Email);
                if (userExists != null)
                    return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "User already exists with this email address" };

                ApplicationUser user = new()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    CreatedDateTime = DateTime.UtcNow,
                };

                if (model.ProfileImage != null)
                {
                    if (model.ProfileImage.Length > 0)
                    {
                        //user.ProfileUrl = await common.UploadBlobUserFile(model.ProfileImage, "userimages");
                    }
                }
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    string msg = "Password should follow below criteria:\n";
                    var strings = result.Errors.ToList().Select(x => x.Description).ToList();
                    return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = msg + string.Join("\n", strings) };
                }
                else
                {
                    return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "User created successfully" };
                }
            }
            catch (Exception ex)
            {
                CommonDBHelper.ErrorLog("AuthenticateController - Register", ex.Message, ex.StackTrace);
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Something Went Wrong" };
            }
        }

        public async Task<APIResponseModel> RefreshToken(TokenModel tokenModel)
        {
            UserDetail userDetail = new UserDetail();
            try
            {
                if (tokenModel is null)
                {
                    return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Invalid client request" };
                }

                string? accessToken = tokenModel.AccessToken;
                string? refreshToken = tokenModel.RefreshToken;

                var principal = GetPrincipalFromExpiredToken(accessToken);
                if (principal == null)
                {
                    return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Invalid access token or refresh token" };
                }

                string username = principal.Identity.Name;

                var user = await _userManager.FindByNameAsync(username);

                if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Invalid access token or refresh token" };
                }

                var newAccessToken = GetToken(principal.Claims.ToList());
                var newRefreshToken = GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                await _userManager.UpdateAsync(user);

                userDetail.UserId = user.Id;
                userDetail.UserName = user.FirstName + " " + user.LastName;
                userDetail.Email = user.UserName;
                if (!string.IsNullOrEmpty(user.ProfileUrl))
                {
                    userDetail.ProfileImage = _configuration["BlobStorageSettings:UserImagesPath"].ToString() + user.ProfileUrl + _configuration["BlobStorageSettings:UserImagesPathToken"].ToString();

                }

                return new APIResponseModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success",
                    Result = new
                    {
                        AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                        RefreshToken = newRefreshToken,
                        Expiration = newAccessToken.ValidTo,
                        UserDetail = userDetail
                    }
                };
            }
            catch (Exception ex)
            {
                CommonDBHelper.ErrorLog("AuthenticateController - RefreshToken", ex.Message, ex.StackTrace);
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Something Went Wrong." };
            }
        }

        public async Task<APIResponseModel> ForgotPassword(ForgotPasswordRequestModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.UserEmail.Trim());
                if (user != null)
                {
                    string newPassword = common.GenerateRandomPassword();
                    var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var updatedPassword = await _userManager.ResetPasswordAsync(user, resetPasswordToken, newPassword);
                    if (updatedPassword.Succeeded)
                    {
                        var mailSend = common.SendMail(user.ToString(), newPassword, user.FirstName);
                        if (mailSend.Result == "Mail Sent Successfully")
                        {
                            return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "We have sent an updated password to your registered email address. You can use that password and we recommend updating the password after login from the mobile application" };
                        }
                        else
                        {
                            return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Mail not sent" };
                        }
                    }
                    else
                    {
                        return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Password does not updated" };
                    }
                }
                else
                {
                    return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = "Email address does not exists" };
                }
            }
            catch (Exception ex)
            {
                CommonDBHelper.ErrorLog("UserController - ForgotPassword", ex.Message, ex.StackTrace);
                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Something Went Wrong" };
            }

        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
