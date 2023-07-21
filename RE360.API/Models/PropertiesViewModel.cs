using RE360.API.DBModels;

namespace RE360.API.Models
{
    public class PropertiesViewModel
    {
        public ContractDetail contractDetail { get; set; } = new ContractDetail();
        public List<ClientDetail> clientDetail { get; set; } = new List<ClientDetail>();
        public ContractRate contractRate { get; set; } = new ContractRate();
        public Estimates estimates { get; set; } = new Estimates();
        public Execution execution { get; set; } = new Execution();
        public LegalDetail legalDetail { get; set; } = new LegalDetail();
        public ListingAddress listingAddress { get; set; } = new ListingAddress();
        public MethodOfSale methodOfSale { get; set; } = new MethodOfSale();
        public ParticularDetail particularDetail { get; set; } = new ParticularDetail();

        public PriorAgencyMarketing priorAgencyMarketing { get; set; } = new PriorAgencyMarketing();
        public PropertyInformation propertyInformation { get; set; } = new PropertyInformation();



    }
}
