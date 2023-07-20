using Microsoft.AspNetCore.Mvc;
using RE360.API.Models;

namespace RE360.API.Domain
{
    public interface IPropertyInformationRepository
    {
        Task<APIResponseModel> SavePropertyLocation(PropertyLocationViewModel model);
        Task<APIResponseModel> SaveClientDetail(List<ClientDetailViewModel> model);
        Task<APIResponseModel> SaveLegalDetail(LegalDetailViewModel model);
        Task<APIResponseModel> SaveParticularDetail(ParticularDetailViewModel model);
        Task<APIResponseModel> SaveSolicitorDetail(SolicitorDetailViewModel model);
        Task<APIResponseModel> SaveContractDetailRate(ContractViewModel model);
        Task<APIResponseModel> SaveEstimates(EstimatesViewModel model);
        Task<APIResponseModel> SaveExecution(ExecutionViewModel model);
        Task<APIResponseModel> SaveMethodOfSale(MethodOfSaleViewModel model);
        Task<APIResponseModel> SavePriorAgencyMarketing(PriorAgencyMarketingViewModel model);
        Task<APIResponseModel> SaveTenancyDetail(TenancyDetailViewModel model);

        Task<APIResponseModel> SavePropertyInformation(PropertyViewModel model);


    }
}
