namespace RE360.API.RequestModels
{
    public class UserDetail
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; } = "";
        public bool IsPasswordChange { get; set; } 
    }
}
