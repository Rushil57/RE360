using Microsoft.AspNetCore.Mvc;
using RE360.API.Models;

namespace RE360.API.Domain
{
    public interface IPropertyInformationRepository
    {
        Task<PropertyLocationViewModel> SavePropertyLocation(PropertyLocationViewModel model);
        Task<List<ClientDetailViewModel>> SaveClientDetail(List<ClientDetailViewModel> model);
        Task<LegalDetailViewModel> SaveLegalDetail(LegalDetailViewModel model);
        Task<ParticularDetailViewModel> SaveParticularDetail(ParticularDetailViewModel model);
        Task<SolicitorDetailViewModel> SaveSolicitorDetail(SolicitorDetailViewModel model);
        Task<ContractViewModel> SaveContractDetailRate(ContractViewModel model);
        Task<EstimatesViewModel> SaveEstimates(EstimatesViewModel model);
        Task<ExecutionViewModel> SaveExecution(ExecutionViewModel model);
        Task<MethodOfSaleViewModel> SaveMethodOfSale(MethodOfSaleViewModel model);
        Task<PriorAgencyMarketingViewModel> SavePriorAgencyMarketing(PriorAgencyMarketingViewModel model);
        Task<TenancyDetailViewModel> SaveTenancyDetail(TenancyDetailViewModel model);

        Task<PropertyViewModel> SavePropertyInformation(PropertyViewModel model);


    }
}
