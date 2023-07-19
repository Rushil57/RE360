using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RE360.API.Domain;
using RE360.API.Models;

namespace RE360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyInformationController : ControllerBase
    {
        private readonly IPropertyInformationRepository _propertyInformationRepository;
        public PropertyInformationController(IPropertyInformationRepository propertyInformationRepository)
        {
                _propertyInformationRepository= propertyInformationRepository;
        }

        [HttpPost]
        [Route("SavePropertyLocation")]
        public async Task<IActionResult> SavePropertyLocation([FromBody] PropertyLocationViewModel model)
        {

            var result = await _propertyInformationRepository.SavePropertyLocation(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveClientDetail")]
        public async Task<IActionResult> SaveClientDetail([FromBody] List<ClientDetailViewModel> model)
        {

            var result = await _propertyInformationRepository.SaveClientDetail(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveLegalDetail")]
        public async Task<IActionResult> SaveLegalDetail([FromBody] LegalDetailViewModel model)
        {

            var result = await _propertyInformationRepository.SaveLegalDetail(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveParticularDetail")]
        public async Task<IActionResult> SaveParticularDetail([FromBody] ParticularDetailViewModel model)
        {

            var result = await _propertyInformationRepository.SaveParticularDetail(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveSolicitorDetail")]
        public async Task<IActionResult> SaveSolicitorDetail([FromBody] SolicitorDetailViewModel model)
        {

            var result = await _propertyInformationRepository.SaveSolicitorDetail(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveContractDetailRate")]
        public async Task<IActionResult> SaveContractDetailRate([FromBody] ContractViewModel model)
        {

            var result = await _propertyInformationRepository.SaveContractDetailRate(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveEstimates")]
        public async Task<IActionResult> SaveEstimates([FromBody] EstimatesViewModel model)
        {

            var result = await _propertyInformationRepository.SaveEstimates(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveExecution")]
        public async Task<IActionResult> SaveExecution([FromBody] ExecutionViewModel model)
        {

            var result = await _propertyInformationRepository.SaveExecution(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveMethodOfSale")]
        public async Task<IActionResult> SaveMethodOfSale([FromBody] MethodOfSaleViewModel model)
        {

            var result = await _propertyInformationRepository.SaveMethodOfSale(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("SavePriorAgencyMarketing")]
        public async Task<IActionResult> SavePriorAgencyMarketing([FromBody] PriorAgencyMarketingViewModel model)
        {

            var result = await _propertyInformationRepository.SavePriorAgencyMarketing(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveTenancyDetail")]
        public async Task<IActionResult> SaveTenancyDetail([FromBody] TenancyDetailViewModel model)
        {

            var result = await _propertyInformationRepository.SaveTenancyDetail(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("SavePropertyInformation")]
        public async Task<IActionResult> SavePropertyInformation([FromBody] PropertyViewModel model)
        {

            var result = await _propertyInformationRepository.SavePropertyInformation(model);
            return Ok(result);
        }
    }
}
