namespace RE360.API.DBModels
{
    public class ContractRate
    {
        public int ID { get; set; }
        public int PID { get; set; }
        public decimal? Water { get; set; }
        public decimal? Council { get; set; }
        public bool IsPA { get; set; }
        public bool IsPQ { get; set; }
        public decimal? PQValue { get; set; }
    }
}
