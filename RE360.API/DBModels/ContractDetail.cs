namespace RE360.API.DBModels
{
    public class ContractDetail
    {
        public int ID { get; set; }
        public int PID { get; set; }
        public DateTime? AuthorityStartDate { get; set; }
        public DateTime? AuthorityEndDate { get; set; }
        public decimal AgreedMarketSpend { get; set; }
    }
}
