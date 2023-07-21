namespace RE360.API.Models
{
    public class ListingAddressViewModel
    {
        public int ID { get; set; }
        public string? Address { get; set; }
        public string? Unit { get; set; }
        public string? Suburb { get; set; }
        public string? PostCode { get; set; }
        public string? StreetNumber { get; set; }
        public string? StreetName { get; set; }  
        //public string? AgentName { get; set; }
    }
}
