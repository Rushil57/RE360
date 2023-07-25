using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RE360.API.Auth;
using RE360.API.Common;
using RE360.API.DBModels;
using RE360.API.Domain;
using RE360.API.Models;
using System.Collections.Generic;
using System.Security.Cryptography;

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

        public async Task<APIResponseModel> AddEstimates(EstimateViewModel model)
        {
            try
            {
                EstimateViewModel estimateViewModel= new EstimateViewModel();
                var estimateDetail= _mapper.Map<EstimatesDetail>(model.EstimatesDetail);
                if(estimateDetail.ID > 0)
                {
                    _context.EstimatesDetail.Update(estimateDetail);
                }
                else
                {
                    _context.EstimatesDetail.Add(estimateDetail);
                }
                _context.SaveChanges();

                estimateViewModel.EstimatesDetail = _mapper.Map<EstimatesDetailViewModel>(estimateDetail);

                var estimates = _mapper.Map<List<Estimates>>(model.Estimates);
                var estimate = estimates.Where(x => x.ID > 0).ToList();
                if (estimate.Count > 0)
                {
                    _context.Estimates.UpdateRange(estimate);
                    _context.SaveChanges();
                    var listEstimate = _mapper.Map<List<EstimatesViewModel>>(estimate);
                    estimateViewModel.Estimates.AddRange(listEstimate);
                }

                var estimateAdd = estimates.Where(x => x.ID== 0).ToList();

                if (estimateAdd.Count > 0)
                {
                    _context.Estimates.AddRange(estimateAdd);
                    _context.SaveChanges();
                    var listEstimate= _mapper.Map<List<EstimatesViewModel>>(estimateAdd);
                    estimateViewModel.Estimates.AddRange(listEstimate);
                }

                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = estimateViewModel };
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
                                          join cod in _context.ContractDetail
                                          on l.ID equals cod.PID into contractdetail
                                          from cd in contractdetail.DefaultIfEmpty()
                                          join cor in _context.ContractRate
                                          on l.ID equals cor.PID into contractRate
                                          from cr in contractRate.DefaultIfEmpty()
                                          join esti in _context.Estimates
                                          on l.ID equals esti.PID into estimate
                                          from est in estimate.DefaultIfEmpty()
                                          join exec in _context.Execution
                                          on l.ID equals exec.PID into execution
                                          from exe in execution.DefaultIfEmpty()
                                          join led in _context.LegalDetail
                                          on l.ID equals led.PID into legalDetail
                                          from ld in legalDetail.DefaultIfEmpty()
                                          join meos in _context.MethodOfSale
                                          on l.ID equals meos.PID into methodOfSale
                                          from mos in methodOfSale.DefaultIfEmpty()
                                          join pad in _context.ParticularDetail
                                          on l.ID equals pad.PID into particularDetail
                                          from pd in particularDetail.DefaultIfEmpty()
                                          join pram in _context.PriorAgencyMarketing
                                          on l.ID equals pram.PID into priorAgencyMarketing
                                          from pam in priorAgencyMarketing.DefaultIfEmpty()
                                          join prid in _context.PropertyInformationDetail
                                          on l.ID equals prid.PID into propertyInformationDetail
                                          from pid in propertyInformationDetail.DefaultIfEmpty()
                                          join sod in _context.SolicitorDetail
                                          on l.ID equals sod.PID into solicitorDetail
                                          from sd in solicitorDetail.DefaultIfEmpty()
                                          join ted in _context.TenancyDetail
                                          on l.ID equals ted.PID into tenancyDetail
                                          from td in tenancyDetail.DefaultIfEmpty()
                                          join etdl in _context.EstimatesDetail
                                          on l.ID equals etdl.PID into estimatesDetail
                                          from etd in estimatesDetail.DefaultIfEmpty()
                                          where l.ID == id
                                          select new
                                          {
                                              listingAddress = l,
                                              clientDetail = _context.ClientDetail.Where(x => x.PID == id).ToList(),
                                              solicitorDetail = sd,
                                              particularDetail = pd,
                                              legalDetail = ld,
                                              contractDetail = cd,
                                              contractRate = cr,
                                              methodOfSale = mos,
                                              propertyInformation = _context.PropertyInformation.Where(x => x.PID == id).ToList(),
                                              propertyInformationDetail = pid,
                                              tenancyDetail = td,
                                              priorAgencyMarketing = pam,
                                              estimates = _context.Estimates.Where(x => x.PID == id).ToList(),
                                              estimatesDetail= etd,
                                              execution = exe
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

        public async Task<APIResponseModel> DelteEstimateByID(int ID)
        {
            try
            {
                var execution = _context.Estimates.Where(x => x.ID == ID).FirstOrDefault();
                if (execution != null)
                {
                    _context.Estimates.Remove(execution);
                    _context.SaveChanges();
                    return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success" };
                }
                else
                {
                    return new APIResponseModel { StatusCode = StatusCodes.Status400BadRequest, Message = "Please enter valid id." };
                }
            }
            catch (Exception ex)
            {

                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }
    }
}
