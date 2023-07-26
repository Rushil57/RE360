using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RE360.API.Domain;
using RE360.API.Models;

namespace RE360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PropertyInformationController : ControllerBase
    {
        private readonly IPropertyInformationRepository _propertyInformationRepository;
        public PropertyInformationController(IPropertyInformationRepository propertyInformationRepository)
        {
                _propertyInformationRepository= propertyInformationRepository;
        }

        [HttpGet]
        [Route("GetListingAddressByPID")]
        public async Task<IActionResult> GetListingAddressByPID(int id)
        {

            var result = await _propertyInformationRepository.GetListingAddressByPID(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetPropertyList")]
        public async Task<IActionResult> GetPropertyList(Guid agentID)
        {
            if(agentID==null || agentID==Guid.Empty) {
                var error= new APIResponseModel { StatusCode = StatusCodes.Status400BadRequest, Message = "Please add agentid" };
                return Ok(error);
            }
            var result = await _propertyInformationRepository.GetPropertyList(agentID);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddListingAddress")]
        public async Task<IActionResult> AddListingAddress([FromBody] ListingAddressViewModel model)
        {

            var result = await _propertyInformationRepository.AddListingAddress(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddClientDetail")]
        public async Task<IActionResult> AddClientDetail([FromBody] ClientDetailListViewModel model)
        {

            var result = await _propertyInformationRepository.AddClientDetail(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddLegalDetail")]
        public async Task<IActionResult> AddLegalDetail([FromBody] LegalDetailViewModel model)
        {

            var result = await _propertyInformationRepository.AddLegalDetail(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddParticularDetail")]
        public async Task<IActionResult> AddParticularDetail([FromBody] ParticularDetailViewModel model)
        {

            var result = await _propertyInformationRepository.AddParticularDetail(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddSolicitorDetail")]
        public async Task<IActionResult> AddSolicitorDetail([FromBody] SolicitorDetailListViewModel model)
        {

            var result = await _propertyInformationRepository.AddSolicitorDetail(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddContractDetailRate")]
        public async Task<IActionResult> AddContractDetailRate([FromBody] ContractViewModel model)
        {

            var result = await _propertyInformationRepository.AddContractDetailRate(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddEstimates")]
        public async Task<IActionResult> AddEstimates([FromBody] EstimateViewModel model)
        {

            var result = await _propertyInformationRepository.AddEstimates(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddExecution")]
        public async Task<IActionResult> AddExecution([FromForm] ExecutionViewModel model)
        {

            var result = await _propertyInformationRepository.AddExecution(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddExecutionDetails")]
        public async Task<IActionResult> AddExecutionDetails([FromForm] SignaturesOfClientViewModel model)
        {

            var result = await _propertyInformationRepository.AddExecutionDetails(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddMethodOfSale")]
        public async Task<IActionResult> AddMethodOfSale([FromBody] MethodOfSaleViewModel model)
        {

            var result = await _propertyInformationRepository.AddMethodOfSale(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddPriorAgencyMarketing")]
        public async Task<IActionResult> AddPriorAgencyMarketing([FromBody] PriorAgencyMarketingViewModel model)
        {

            var result = await _propertyInformationRepository.AddPriorAgencyMarketing(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddTenancyDetail")]
        public async Task<IActionResult> AddTenancyDetail([FromBody] TenancyDetailViewModel model)
        {

            var result = await _propertyInformationRepository.AddTenancyDetail(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddPropertyInformation")]
        public async Task<IActionResult> AddPropertyInformation([FromBody] PropertyViewModel model)
        {

            var result = await _propertyInformationRepository.AddPropertyInformation(model);
            return Ok(result);
        }
        [HttpGet]
        [Route("DelteEstimateByID")]
        public async Task<IActionResult> DelteEstimateByID(int id)
        {

            var result = await _propertyInformationRepository.DelteEstimateByID(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetList")]
        public async Task<IActionResult> GetList()
        {

            var result = await _propertyInformationRepository.GetList();
            return Ok(result);
        }
    }
}
