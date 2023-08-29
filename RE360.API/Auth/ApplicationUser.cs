using Microsoft.AspNetCore.Identity;

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
    public string? CompanyName { get; set; }
    public string? ManagerEmail { get; set; }
    public string? OffinceName { get; set; }
    public decimal? BaseAmount { get; set; }
    public decimal? SalePricePercentage { get; set; }
    public decimal? MinimumCommission { get; set; }
    public bool IsPasswordChange { get; set; } = false;

}