namespace RE360.API.DBModels
{
    public class PropertyInformationDetail
    {
        public int ID { get; set; }
        public int PID { get; set; }
        public string? Double { get; set; }
        public string? Single { get; set; }
        public string? Comment { get; set; }
        public string? AdditionalFeature { get; set; }
        public string? ExcludedChattels { get; set; }
        public string? InternalRemark { get; set; }
    }
}
