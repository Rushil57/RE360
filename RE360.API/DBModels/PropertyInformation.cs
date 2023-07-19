namespace RE360.API.DBModels
{
    public class PropertyInformation
    {
        public int ID { get; set; }
        public int PID { get; set; }
        public int PropAttrId { get; set; }
        public decimal? Count { get; set; }
        public string? Remarks { get; set; }
    }
}
