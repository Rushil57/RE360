namespace RE360.API.Models
{
    public class SignaturesOfClientViewModel
    {
        public int ID { get; set; }
        public int ExecutionId { get; set; }
        public int ClientId { get; set; }
        public string? SignatureOfClient { get; set; }
    }
}
