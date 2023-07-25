using AutoMapper;
using RE360.API.DBModels;

namespace RE360.API.Models
{
    public class MappingModel : Profile
    {
        public MappingModel()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<ListingAddress, ListingAddressViewModel>();
            CreateMap<ListingAddressViewModel, ListingAddress>();

            CreateMap<ClientDetail, ClientDetailViewModel>();
            CreateMap<ClientDetailViewModel, ClientDetail>();

            CreateMap<LegalDetail, LegalDetailViewModel>();
            CreateMap<LegalDetailViewModel, LegalDetail>();

            CreateMap<ParticularDetail, ParticularDetailViewModel>();
            CreateMap<ParticularDetailViewModel, ParticularDetail>();

            CreateMap<SolicitorDetail, SolicitorDetailViewModel>();
            CreateMap<SolicitorDetailViewModel, SolicitorDetail>();

            CreateMap<ContractDetail, ContractDetailViewModel>();
            CreateMap<ContractDetailViewModel, ContractDetail>();

            CreateMap<ContractRate, ContractRateViewModel>();
            CreateMap<ContractRateViewModel, ContractRate>();

            CreateMap<MethodOfSale, MethodOfSaleViewModel>();
            CreateMap<MethodOfSaleViewModel, MethodOfSale>();

            CreateMap<TenancyDetail, TenancyDetailViewModel>();
            CreateMap<TenancyDetailViewModel, TenancyDetail>();

            CreateMap<PriorAgencyMarketing, PriorAgencyMarketingViewModel>();
            CreateMap<PriorAgencyMarketingViewModel, PriorAgencyMarketing>();

            CreateMap<Estimates, EstimatesViewModel>();
            CreateMap<EstimatesViewModel, Estimates>();

            CreateMap<Execution, ExecutionViewModel>();
            CreateMap<ExecutionViewModel, Execution>();

            CreateMap<SignaturesOfClient, SignaturesOfClientViewModel>();
            CreateMap<SignaturesOfClientViewModel, SignaturesOfClient>();

            CreateMap<PropertyInformation, PropertyInformationViewModel>();
            CreateMap<PropertyInformationViewModel, PropertyInformation>();

            CreateMap<PropertyInformationDetail, PropertyInformationDetailViewModel>();
            CreateMap<PropertyInformationDetailViewModel, PropertyInformationDetail>();

            CreateMap<EstimatesDetail, EstimatesDetailViewModel>();
            CreateMap<EstimatesDetailViewModel, EstimatesDetail>();
        }
    }
}
