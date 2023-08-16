using Microsoft.AspNetCore.Mvc;
using RE360.API.Auth;
using RE360.API.Migrations;
using RE360.API.Models;

namespace RE360.API.Domain
{
    public interface IPropertyInformationRepository
    {
        Task<APIResponseModel> GetListingAddressByPID(int id);
        Task<APIResponseModel> AddListingAddress(ListingAddressViewModel model);
        Task<APIResponseModel> AddClientDetail(ClientDetailListViewModel model);
        Task<APIResponseModel> AddLegalDetail(LegalDetailViewModel model);
        Task<APIResponseModel> AddParticularDetail(ParticularDetailViewModel model);
        Task<APIResponseModel> AddSolicitorDetail(SolicitorDetailListViewModel model);
        Task<APIResponseModel> AddContractDetailRate(ContractViewModel model);
        Task<APIResponseModel> AddEstimates(EstimateViewModel model);
        Task<APIResponseModel> AddExecution(ExecutionViewModel model);
        Task<APIResponseModel> AddMethodOfSale(MethodOfSaleViewModel model);
        Task<APIResponseModel> AddPriorAgencyMarketing(PriorAgencyMarketingViewModel model);
        Task<APIResponseModel> AddCalculationOfCommission(CaclulationCommissionNewModel model);
        
        Task<APIResponseModel> AddTenancyDetail(TenancyDetailViewModel model);

        Task<APIResponseModel> AddPropertyInformation(PropertyViewModel model);
        Task<APIResponseModel> GetPropertyList(Guid agentID);
        Task<APIResponseModel> DelteEstimateByID(int ID);

        Task<APIResponseModel> AddExecutionDetails(SignaturesOfClientViewModel model);
        Task<APIResponseModel> GetList();
        Task<APIResponseModel> DeleteClientByID(int ID);
        Task<APIResponseModel> DeleteSolicitorByID(int ID);
        Task<APIResponseModel> DeletePropertyByID(int ID);
        Task<APIResponseModel> GeneratePDF(int ID);

    }
}
