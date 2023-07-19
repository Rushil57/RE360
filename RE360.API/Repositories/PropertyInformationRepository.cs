using AutoMapper;
using RE360.API.Auth;
using RE360.API.DBModels;
using RE360.API.Domain;
using RE360.API.Models;

namespace RE360.API.Repositories
{
    public class PropertyInformationRepository : IPropertyInformationRepository
    {
        private readonly IMapper _mapper;
        private readonly RE360AppDbContext _context;
        public PropertyInformationRepository(IMapper mapper, RE360AppDbContext context = null)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<PropertyLocationViewModel> SavePropertyLocation(PropertyLocationViewModel model)
        {
            try
            {
                var propertyLocation = _mapper.Map<PropertyLocation>(model);

                if (propertyLocation.ID > 0)
                {
                    _context.PropertyLocation.Update(propertyLocation);
                }
                else
                {
                    _context.PropertyLocation.Add(propertyLocation);
                }
                _context.SaveChanges();
                var propertyLocationVM = _mapper.Map<PropertyLocationViewModel>(propertyLocation);
                return propertyLocationVM;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<ClientDetailViewModel>> SaveClientDetail(List<ClientDetailViewModel> model)
        {
            try
            {
                var clientDetail = _mapper.Map<List<ClientDetail>>(model);

                var addRange = clientDetail.Where(x => x.ID == 0).ToList();
                var updateRange = clientDetail.Where(x => x.ID != 0).ToList();

                if (updateRange.Count > 0)
                {
                    _context.ClientDetail.UpdateRange(updateRange);
                }
                if (addRange.Count > 0)
                {
                    _context.ClientDetail.AddRange(addRange);
                }
                _context.SaveChanges();

                var clientDetailVM = _mapper.Map<List<ClientDetailViewModel>>(clientDetail);
                return clientDetailVM;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<LegalDetailViewModel> SaveLegalDetail(LegalDetailViewModel model)
        {
            try
            {
                var legalDetail = _mapper.Map<LegalDetail>(model);

                if (legalDetail.ID > 0)
                {
                    _context.LegalDetail.Update(legalDetail);
                }
                else
                {
                    _context.LegalDetail.Add(legalDetail);
                }
                _context.SaveChanges();
                var legalDetailVM = _mapper.Map<LegalDetailViewModel>(legalDetail);
                return legalDetailVM;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ParticularDetailViewModel> SaveParticularDetail(ParticularDetailViewModel model)
        {
            try
            {
                var particularDetail = _mapper.Map<ParticularDetail>(model);

                if (particularDetail.ID > 0)
                {
                    _context.ParticularDetail.Update(particularDetail);
                }
                else
                {
                    _context.ParticularDetail.Add(particularDetail);
                }
                _context.SaveChanges();

                var particularDetailVM = _mapper.Map<ParticularDetailViewModel>(particularDetail);
                return particularDetailVM;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<SolicitorDetailViewModel> SaveSolicitorDetail(SolicitorDetailViewModel model)
        {
            try
            {
                var solicitorDetail = _mapper.Map<SolicitorDetail>(model);

                if (solicitorDetail.ID > 0)
                {
                    _context.SolicitorDetail.Update(solicitorDetail);
                }
                else
                {
                    _context.SolicitorDetail.Add(solicitorDetail);
                }
                _context.SaveChanges();

                var solicitorDetailVM = _mapper.Map<SolicitorDetailViewModel>(solicitorDetail);
                return solicitorDetailVM;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ContractViewModel> SaveContractDetailRate(ContractViewModel model)
        {
            try
            {
                var contractDetailViewModel = model.ContractDetailViewModel;
                var contractDetail = _mapper.Map<ContractDetail>(contractDetailViewModel);

                if (contractDetail.ID > 0)
                {
                    _context.ContractDetail.Update(contractDetail);
                }
                else
                {
                    _context.ContractDetail.Add(contractDetail);
                }
                _context.SaveChanges();

                var contractDetailVM = _mapper.Map<ContractDetailViewModel>(contractDetail);

                model.ContractDetailViewModel = contractDetailVM;

                var contractRateViewModel = model.ContractRateViewModel;
                var contractRate = _mapper.Map<ContractRate>(contractRateViewModel);

                if (contractRate.ID > 0)
                {
                    _context.ContractRate.Update(contractRate);
                }
                else
                {
                    _context.ContractRate.Add(contractRate);
                }
                _context.SaveChanges();

                var contractRateVM = _mapper.Map<ContractRateViewModel>(contractRate);
                model.ContractRateViewModel = contractRateVM;
                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<EstimatesViewModel> SaveEstimates(EstimatesViewModel model)
        {
            try
            {
                var estimates = _mapper.Map<Estimates>(model);
                if (estimates.ID > 0)
                {
                    _context.Estimates.Update(estimates);
                }
                else
                {
                    _context.Estimates.Add(estimates);
                }
                _context.SaveChanges();

                var estimatesVM = _mapper.Map<EstimatesViewModel>(estimates);
                return estimatesVM;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ExecutionViewModel> SaveExecution(ExecutionViewModel model)
        {
            try
            {
                var execution = _mapper.Map<Execution>(model);
                if (execution.ID > 0)
                {
                    _context.Execution.Update(execution);
                }
                else
                {
                    _context.Execution.Add(execution);
                }
                _context.SaveChanges();

                var executionVM = _mapper.Map<ExecutionViewModel>(execution);
                return executionVM;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<MethodOfSaleViewModel> SaveMethodOfSale(MethodOfSaleViewModel model)
        {
            try
            {
                var methodOfSale = _mapper.Map<MethodOfSale>(model);
                if (methodOfSale.ID > 0)
                {
                    _context.MethodOfSale.Update(methodOfSale);
                }
                else
                {
                    _context.MethodOfSale.Add(methodOfSale);
                }

                _context.SaveChanges();

                var methodOfSaleVM = _mapper.Map<MethodOfSaleViewModel>(methodOfSale);
                return methodOfSaleVM;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<PriorAgencyMarketingViewModel> SavePriorAgencyMarketing(PriorAgencyMarketingViewModel model)
        {
            try
            {
                var priorAgencyMarketing = _mapper.Map<PriorAgencyMarketing>(model);
                if (priorAgencyMarketing.ID > 0)
                {
                    _context.PriorAgencyMarketing.Update(priorAgencyMarketing);
                }
                else
                {
                    _context.PriorAgencyMarketing.Add(priorAgencyMarketing);
                }
                _context.SaveChanges();

                var priorAgencyMarketingVM = _mapper.Map<PriorAgencyMarketingViewModel>(priorAgencyMarketing);
                return priorAgencyMarketingVM;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<TenancyDetailViewModel> SaveTenancyDetail(TenancyDetailViewModel model)
        {
            try
            {
                var tenancyDetail = _mapper.Map<TenancyDetail>(model);
                if (tenancyDetail.ID > 0)
                {
                    _context.TenancyDetail.Update(tenancyDetail);
                }
                else
                {
                    _context.TenancyDetail.Add(tenancyDetail);
                }
                _context.SaveChanges();

                var tenancyDetailVM = _mapper.Map<TenancyDetailViewModel>(tenancyDetail);
                return tenancyDetailVM;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PropertyViewModel> SavePropertyInformation(PropertyViewModel model)
        {
            try
            {
                var propertyInformationViewModel = model.PropertyInformationViewModel;
                var propertyInformation = _mapper.Map<List<PropertyInformation>>(propertyInformationViewModel);

                _context.PropertyInformation.AddRange(propertyInformation);
                _context.SaveChanges();

                var propertyInformationVM = _mapper.Map<List<PropertyInformationViewModel>>(propertyInformation);

                model.PropertyInformationViewModel = propertyInformationVM;

                var propertyInformationDetailViewModel = model.PropertyInformationDetailViewModel;
                var propertyInformationDetail = _mapper.Map<PropertyInformationDetail>(propertyInformationDetailViewModel);
                if (propertyInformationDetail.ID > 0)
                {
                    _context.PropertyInformationDetail.Update(propertyInformationDetail);
                }
                else
                {
                    _context.PropertyInformationDetail.Add(propertyInformationDetail);
                }
                _context.SaveChanges();

                var propertyInformationDetailVM = _mapper.Map<PropertyInformationDetailViewModel>(propertyInformationDetail);
                model.PropertyInformationDetailViewModel = propertyInformationDetailVM;
                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
