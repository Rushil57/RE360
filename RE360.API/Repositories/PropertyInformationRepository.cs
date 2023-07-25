using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RE360.API.Auth;
using RE360.API.Common;
using RE360.API.DBModels;
using RE360.API.Domain;
using RE360.API.Models;
using System.Collections.Generic;

namespace RE360.API.Repositories
{
    public class PropertyInformationRepository : IPropertyInformationRepository
    {
        private readonly IMapper _mapper;
        private readonly RE360AppDbContext _context;
        CommonMethod common;
        public PropertyInformationRepository(IMapper mapper, RE360AppDbContext context = null)
        {
            _mapper = mapper;
            _context = context;
            common = new CommonMethod(context);
        }
        public async Task<APIResponseModel> AddListingAddress(ListingAddressViewModel model)
        {
            try
            {
                var propertyLocation = _mapper.Map<ListingAddress>(model);

                if (propertyLocation.ID > 0)
                {
                    _context.ListingAddress.Update(propertyLocation);
                }
                else
                {
                    _context.ListingAddress.Add(propertyLocation);
                }
                _context.SaveChanges();
                var propertyLocationVM = _mapper.Map<ListingAddressViewModel>(propertyLocation);

                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = propertyLocationVM };

            }
            catch (Exception ex)
            {
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }

        public async Task<APIResponseModel> AddClientDetail(ClientDetailListViewModel model)
        {
            try
            {
                var clientDetail = _mapper.Map<List<ClientDetail>>(model.ClientDetails);

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
                model.ClientDetails= clientDetailVM;
                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = model };

            }
            catch (Exception ex)
            {
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }

        public async Task<APIResponseModel> AddLegalDetail(LegalDetailViewModel model)
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

                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = legalDetailVM };

            }
            catch (Exception ex)
            {
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }

        public async Task<APIResponseModel> AddParticularDetail(ParticularDetailViewModel model)
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
                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = particularDetailVM };
            }
            catch (Exception ex)
            {

                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }

        public async Task<APIResponseModel> AddSolicitorDetail(SolicitorDetailViewModel model)
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
                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = solicitorDetailVM };
            }
            catch (Exception ex)
            {
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }

        public async Task<APIResponseModel> AddContractDetailRate(ContractViewModel model)
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
                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = model };
            }
            catch (Exception ex)
            {
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }

        public async Task<APIResponseModel> AddEstimates(EstimatesViewModel model)
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
                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = estimatesVM };
            }
            catch (Exception ex)
            {
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }

        public async Task<APIResponseModel> AddExecution(ExecutionViewModel model)
        {
            try
            {
                //foreach (var item in model.SignaturesOfClient)
                //{
                //    if (item.SignatureClient != null)
                //    {
                //        if (item.SignatureClient.Length > 0)
                //        {
                //            item.SignatureOfClientName = await common.UploadBlobFile(item.SignatureClient, "images");
                //        }
                //    }
                //}
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
                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = executionVM };
            }
            catch (Exception ex)
            {
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }

        public async Task<APIResponseModel> AddMethodOfSale(MethodOfSaleViewModel model)
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
                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = methodOfSaleVM };
            }
            catch (Exception ex)
            {
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }
        public async Task<APIResponseModel> AddPriorAgencyMarketing(PriorAgencyMarketingViewModel model)
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
                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = priorAgencyMarketingVM };
            }
            catch (Exception ex)
            {
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }

        public async Task<APIResponseModel> AddTenancyDetail(TenancyDetailViewModel model)
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
                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = tenancyDetailVM };
            }
            catch (Exception ex)
            {
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }

        public async Task<APIResponseModel> AddPropertyInformation(PropertyViewModel model)
        {
            try
            {
                var propertyInformationViewModel = model.PropertyInformationViewModel;
                var propertyInformation = _mapper.Map<List<PropertyInformation>>(propertyInformationViewModel);
                var ID = propertyInformation[0].ID;
                if (ID > 0)
                {
                    var PID = propertyInformation[0].PID;
                    var propertyInformationList = _context.PropertyInformation.Where(x => x.PID == PID).ToList();
                    if (propertyInformationList.Count > 0)
                    {
                        _context.PropertyInformation.RemoveRange(propertyInformationList);
                        await _context.SaveChangesAsync();
                    }
                }
                foreach (var item in propertyInformation)
                {
                    item.ID = 0;
                }

                _context.PropertyInformation.AddRange(propertyInformation);
                await _context.SaveChangesAsync();

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
                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = model };
            }
            catch (Exception ex)
            {
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }

        public async Task<APIResponseModel> GetListingAddressByPID(int id)
        {
            try
            {
                var listingAddressList = (from l in _context.ListingAddress
                               join cd in _context.ContractDetail on l.ID equals cd.PID
                               join cr in _context.ContractRate on cd.PID equals cr.PID
                               join est in _context.Estimates on cr.PID equals est.PID
                               join exe in _context.Execution on est.PID equals exe.PID
                               join ld in _context.LegalDetail on exe.PID equals ld.PID
                               join mos in _context.MethodOfSale on ld.PID equals mos.PID
                               join pd in _context.ParticularDetail on mos.PID equals pd.PID
                               join pam in _context.PriorAgencyMarketing on pd.PID equals pam.PID
                               join pi in _context.PropertyInformation on pam.PID equals pi.PID
                               join pid in _context.PropertyInformationDetail on pi.PID equals pid.PID
                               join sd in _context.SolicitorDetail on pid.PID equals sd.PID
                               join td in _context.TenancyDetail on sd.PID equals td.PID
                               where l.ID == id
                               select new
                               {
                                   listingAddress = l,
                                   clientDetail = _context.ClientDetail.Where(x => x.PID == id).ToList(),
                                   contractDetail = cd,
                                   contractRate = cr,
                                   estimates = est,
                                   execution = exe,
                                   legalDetail = ld,
                                   methodOfSale = mos,
                                   particularDetail = pd,
                                   priorAgencyMarketing = pam,
                                   propertyInformation = pi,
                                   propertyInformationDetail = pid,
                                   solicitorDetail = sd,
                                   tenancyDetail = td

                               }).FirstOrDefault();


                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = listingAddressList };

            }
            catch (Exception ex)
            {
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }

        public async Task<APIResponseModel> GetPropertyList(Guid agentID)
        {
            try
            {
                var listingAddressList = (from a in _context.ListingAddress
                                          join ca in _context.ClientDetail
                                          on a.ID equals ca.PID into listAdd
                                          from cd in listAdd.DefaultIfEmpty()
                                          join e in _context.Execution 
                                          on a.ID equals e.PID into cliDetail
                                          from exe in cliDetail.DefaultIfEmpty()
                                          where (a.AgentID == agentID)
                                          select new
                                          {
                                              Id = a.ID,
                                              address = a.Address + "," + a.Unit + " ," + a.Suburb + " ," + a.PostCode + " ," + a.StreetNumber + " ," + a.StreetName,
                                              clientName = cd.Title + " " + cd.SurName + " " + cd.FirstName,
                                              companyTrustName= cd.CompanyTrustName,
                                              Date = exe.CreatedDate == null ? "In Progress" : exe.CreatedDate.ToString()
                                          }).GroupBy(m => new { m.Id }).Select(group => group.First()).ToList();

                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = listingAddressList };
            }
            catch (Exception ex)
            {
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }
    }
}
