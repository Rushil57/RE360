﻿using Microsoft.AspNetCore.Identity;

namespace RE360.API.Auth;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileUrl { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public string? FCMToken { get; set; }
    public string? Device { get; set; }

}