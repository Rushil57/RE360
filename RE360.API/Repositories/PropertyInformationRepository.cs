﻿using AutoMapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RE360.API.Auth;
using RE360.API.Common;
using RE360.API.DBModels;
using RE360.API.Domain;
using RE360.API.Models;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using static iTextSharp.text.pdf.AcroFields;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace RE360.API.Repositories
{
    public class PropertyInformationRepository : IPropertyInformationRepository
    {
        private readonly IMapper _mapper;
        private readonly RE360AppDbContext _context;
        CommonMethod common;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        public PropertyInformationRepository(IMapper mapper, UserManager<ApplicationUser> userManager, RE360AppDbContext context = null, IConfiguration configuration = null)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _configuration = configuration;
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
                model.ClientDetails = clientDetailVM;
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

        public async Task<APIResponseModel> AddSolicitorDetail(SolicitorDetailListViewModel model)
        {
            try
            {
                var solicitorDetail = _mapper.Map<List<SolicitorDetail>>(model.SolicitorDetail);

                var updateList = solicitorDetail.Where(x => x.ID > 0).ToList();
                model.SolicitorDetail = new List<SolicitorDetailViewModel>();
                if (updateList.Count > 0)
                {
                    _context.SolicitorDetail.UpdateRange(updateList);
                    _context.SaveChanges();
                    var update = _mapper.Map<List<SolicitorDetailViewModel>>(updateList);
                    model.SolicitorDetail.AddRange(update);
                }

                var addList = solicitorDetail.Where(x => x.ID == 0).ToList();

                if (addList.Count > 0)
                {
                    _context.SolicitorDetail.AddRange(addList);
                    _context.SaveChanges();
                    var add = _mapper.Map<List<SolicitorDetailViewModel>>(addList);
                    model.SolicitorDetail.AddRange(add);
                }



                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = model };
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
                EstimateViewModel estimateViewModel = new EstimateViewModel();
                var estimateDetail = _mapper.Map<EstimatesDetail>(model.EstimatesDetail);
                if (estimateDetail.ID > 0)
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

                var estimateAdd = estimates.Where(x => x.ID == 0).ToList();

                if (estimateAdd.Count > 0)
                {
                    _context.Estimates.AddRange(estimateAdd);
                    _context.SaveChanges();
                    var listEstimate = _mapper.Map<List<EstimatesViewModel>>(estimateAdd);
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
                Execution exeList = _context.Execution.Find(model.ID);

                if (model.SignedOnBehalfOfTheAgentFile != null)
                {
                    if (model.SignedOnBehalfOfTheAgentFile.Length > 0)
                    {
                        model.SignedOnBehalfOfTheAgent = await common.UploadBlobFile(model.SignedOnBehalfOfTheAgentFile, "images");
                        if (exeList != null)
                        {
                            exeList.SignedOnBehalfOfTheAgent = model.SignedOnBehalfOfTheAgent;
                        }
                    }
                }
                else
                {
                    model.SignedOnBehalfOfTheAgent = exeList.SignedOnBehalfOfTheAgent;
                }
                if (model.AgentToSignHereFile != null)
                {
                    if (model.AgentToSignHereFile.Length > 0)
                    {
                        model.AgentToSignHere = await common.UploadBlobFile(model.AgentToSignHereFile, "images");
                        if (exeList != null)
                        {
                            exeList.AgentToSignHere = model.AgentToSignHere;
                        }
                    }
                }
                else
                {
                    model.AgentToSignHere = exeList.AgentToSignHere;
                }
                var execution = _mapper.Map<Execution>(model);
                if (execution.ID > 0)
                {
                    exeList.PID = model.PID;
                    exeList.SignedOnBehalfOfTheAgentDate = model.SignedOnBehalfOfTheAgentDate;
                    exeList.SignedOnBehalfOfTheAgentTime = model.SignedOnBehalfOfTheAgentTime;
                    exeList.AgentToSignHereDate = model.AgentToSignHereDate;
                    _context.Execution.Update(exeList);
                }
                else
                {
                    execution.CreatedDate = DateTime.Now;
                    _context.Execution.Add(execution);
                }

                _context.SaveChanges();

                if (!string.IsNullOrEmpty(execution.SignedOnBehalfOfTheAgent))
                {
                    execution.SignedOnBehalfOfTheAgent = _configuration["BlobStorageSettings:ImagesPath"].ToString() + execution.SignedOnBehalfOfTheAgent + _configuration["BlobStorageSettings:ImageToken"].ToString();
                }
                if (!string.IsNullOrEmpty(execution.AgentToSignHere))
                {
                    execution.AgentToSignHere = _configuration["BlobStorageSettings:ImagesPath"].ToString() + execution.AgentToSignHere + _configuration["BlobStorageSettings:ImageToken"].ToString();
                }
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

        public async Task<APIResponseModel> AddCalculationOfCommission(CaclulationCommissionNewModel model)
        {
            try
            {
                var CalculationOfCommission = _mapper.Map<CalculationOfCommission>(model);
                if (CalculationOfCommission.ID > 0)
                {
                    _context.CalculationOfCommission.Update(CalculationOfCommission);
                }
                else
                {
                    _context.CalculationOfCommission.Add(CalculationOfCommission);

                }
                if (CalculationOfCommission.IsStandard == true)
                {
                    Guid AgentID = _context.ListingAddress.Where(o => o.ID == model.PID).FirstOrDefault().AgentID;
                    model.ClientCommissionDetails = new List<ClientCommissionDetails>();
                    foreach (var item in _context.CommissionDetails.Where(o => o.AgentID == AgentID))
                    {
                        model.ClientCommissionDetails.Add(new ClientCommissionDetails { PID = model.PID, Percent = item.Percent, UpToAmount = item.UpToAmount, Sequence = item.Sequence });
                    }
                }
                if (_context.ClientCommissionDetails.Where(o => o.PID == model.PID).Count() > 0)
                {
                    _context.ClientCommissionDetails.RemoveRange(_context.ClientCommissionDetails.Where(o => o.PID == model.PID));
                }
                if (model.ClientCommissionDetails.Count() > 0)
                {
                    _context.ClientCommissionDetails.AddRange(model.ClientCommissionDetails);
                }
                _context.SaveChanges();
                model.ID = CalculationOfCommission.ID;
                if (CalculationOfCommission.IsStandard == true)
                {
                    model.ClientCommissionDetails = new List<ClientCommissionDetails>();
                }
                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = model };
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
                if (model.PropertyInformationViewModel.Count > 0)
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
                }
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
                Guid AgentID = _context.ListingAddress.Where(o => o.ID == id).FirstOrDefault().AgentID;
                var clientDetailsList = _context.ClientDetail.Where(x => x.PID == id).ToList();
                var propertyInformationsList = _context.PropertyInformation.Where(x => x.PID == id).ToList();
                var estimatesList = _context.Estimates.Where(x => x.PID == id).ToList();
                var signaturesOfClientList = _context.SignaturesOfClient.Where(x => x.PID == id).ToList();
                var solicitorList = _context.SolicitorDetail.Where(x => x.PID == id).ToList();
                var execution = _context.Execution.Where(x => x.PID == id).FirstOrDefault();
                var ClientCalOfCommList = _context.ClientCommissionDetails.Where(x => x.PID == id).ToList();
                var AgentCalOfCommList = _context.CommissionDetails.Where(o => o.AgentID == AgentID).ToList();
                var user = await _userManager.FindByIdAsync(AgentID.ToString().Trim());
                var AgentCalofComm = new { BaseAmount = user.BaseAmount, MinimumCommission = user.MinimumCommission, SalePricePercentage = user.SalePricePercentage };
                if (execution != null)
                {
                    if (!string.IsNullOrEmpty(execution.SignedOnBehalfOfTheAgent))
                    {
                        execution.SignedOnBehalfOfTheAgent = _configuration["BlobStorageSettings:ImagesPath"].ToString() + execution.SignedOnBehalfOfTheAgent + _configuration["BlobStorageSettings:ImageToken"].ToString();
                    }
                    if (!string.IsNullOrEmpty(execution.AgentToSignHere))
                    {
                        execution.AgentToSignHere = _configuration["BlobStorageSettings:ImagesPath"].ToString() + execution.AgentToSignHere + _configuration["BlobStorageSettings:ImageToken"].ToString();
                    }
                }
                foreach (var item in signaturesOfClientList)
                {
                    item.SignatureOfClientName = _configuration["BlobStorageSettings:ImagesPath"].ToString() + item.SignatureOfClientName + _configuration["BlobStorageSettings:ImageToken"].ToString();
                }
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
                                          join ted in _context.TenancyDetail
                                          on l.ID equals ted.PID into tenancyDetail
                                          from td in tenancyDetail.DefaultIfEmpty()
                                          join etdl in _context.EstimatesDetail
                                          on l.ID equals etdl.PID into estimatesDetail
                                          from etd in estimatesDetail.DefaultIfEmpty()
                                          join calc in _context.CalculationOfCommission
                                          on l.ID equals calc.PID into CalcOfCommission
                                          from calc in CalcOfCommission.DefaultIfEmpty()
                                          where l.ID == id
                                          select new
                                          {
                                              listingAddress = l,
                                              clientDetail = clientDetailsList,
                                              solicitorDetail = solicitorList,
                                              particularDetail = pd,
                                              legalDetail = ld,
                                              contractDetail = cd,
                                              contractRate = cr,
                                              methodOfSale = mos,
                                              propertyInformation = propertyInformationsList,
                                              propertyInformationDetail = pid,
                                              tenancyDetail = td,
                                              priorAgencyMarketing = pam,
                                              estimates = estimatesList,
                                              estimatesDetail = etd,
                                              execution = execution,
                                              executionDetail = signaturesOfClientList,
                                              calculationOfcommission = calc,
                                              ClientCalOfCommList = ClientCalOfCommList,
                                              AgentCalOfCommList = AgentCalOfCommList,
                                              AgentCalofComm = AgentCalofComm
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
                                              address = a.Address,
                                              Unit = a.Unit,
                                              Suburb = a.Suburb,
                                              PostCode = a.PostCode,
                                              StreetNumber = a.StreetNumber,
                                              StreetName = a.StreetName,
                                              clientName = cd.Title + " " + cd.SurName + " " + cd.FirstName,
                                              companyTrustName = cd.CompanyTrustName,
                                              Date = exe.CreatedDate == null ? "In Progress" : exe.CreatedDate.ToString(),
                                              PDFUrl = $"{_configuration["BlobStorageSettings:AgentDocPath"]}{a.ID}.pdf{_configuration["BlobStorageSettings:AgentDocToken"]}",
                                          }).GroupBy(m => new { m.Id }).Select(group => group.First()).ToList();




                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = listingAddressList.OrderByDescending(o => o.Id) };
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

        public async Task<APIResponseModel> AddExecutionDetails(SignaturesOfClientViewModel model)
        {
            try
            {
                SignaturesOfClient SignList = _context.SignaturesOfClient.Find(model.ID);
                string StrSignatureOfClientName = "";
                if (model.SignatureClient != null)
                {
                    if (model.SignatureClient.Length > 0)
                    {
                        model.SignatureOfClientName = await common.UploadBlobFile(model.SignatureClient, "images");
                        StrSignatureOfClientName = model.SignatureOfClientName;
                        if (SignList != null)
                        {
                            SignList.SignatureOfClientName = model.SignatureOfClientName;
                        }
                    }
                }

                var signaturesOfClient = _mapper.Map<SignaturesOfClient>(model);
                if (signaturesOfClient.ID > 0)
                {
                    SignList.PID = signaturesOfClient.PID;
                    SignList.ClientId = signaturesOfClient.ClientId;
                    _context.SignaturesOfClient.Update(SignList);
                }
                else
                {
                    _context.SignaturesOfClient.Add(signaturesOfClient);
                }

                _context.SaveChanges();

                if (!string.IsNullOrEmpty(StrSignatureOfClientName))
                {
                    signaturesOfClient.SignatureOfClientName = _configuration["BlobStorageSettings:ImagesPath"].ToString() + StrSignatureOfClientName + _configuration["BlobStorageSettings:ImageToken"].ToString();
                }

                var signaturesOfClientViewModel = _mapper.Map<SignaturesOfClientViewModel>(signaturesOfClient);
                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = signaturesOfClientViewModel };
            }
            catch (Exception ex)
            {
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }

        public async Task<APIResponseModel> GetList()
        {
            try
            {

                var result = (from p in _context.PropertyAttributeType
                              select new
                              {
                                  Name = p.Name,
                                  list = p.PropertyAttribute.ToList()
                              }).ToList();

                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = result };
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<APIResponseModel> DeleteClientByID(int ID)
        {
            try
            {
                var execution = _context.ClientDetail.Where(x => x.ID == ID).FirstOrDefault();
                if (execution != null)
                {
                    _context.ClientDetail.Remove(execution);
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

        public async Task<APIResponseModel> DeleteSolicitorByID(int ID)
        {
            try
            {
                var execution = _context.SolicitorDetail.Where(x => x.ID == ID).FirstOrDefault();
                if (execution != null)
                {
                    _context.SolicitorDetail.Remove(execution);
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
        public async Task<APIResponseModel> DeletePropertyByID(int ID)
        {
            try
            {
                var execution = _context.ListingAddress.Where(x => x.ID == ID).FirstOrDefault();
                if (execution != null)
                {
                    _context.ListingAddress.Remove(execution);
                    _context.ClientDetail.RemoveRange(_context.ClientDetail.Where(x => x.PID == ID));
                    _context.LegalDetail.Remove(_context.LegalDetail.Where(x => x.PID == ID).FirstOrDefault());
                    _context.ParticularDetail.Remove(_context.ParticularDetail.Where(x => x.PID == ID).FirstOrDefault());
                    _context.SolicitorDetail.RemoveRange(_context.SolicitorDetail.Where(x => x.PID == ID));
                    _context.ContractDetail.Remove(_context.ContractDetail.Where(x => x.PID == ID).FirstOrDefault());
                    _context.ContractRate.Remove(_context.ContractRate.Where(x => x.PID == ID).FirstOrDefault());
                    _context.EstimatesDetail.Remove(_context.EstimatesDetail.Where(x => x.PID == ID).FirstOrDefault());
                    _context.Estimates.RemoveRange(_context.Estimates.Where(x => x.PID == ID));
                    _context.Execution.RemoveRange(_context.Execution.Where(x => x.PID == ID));
                    _context.MethodOfSale.Remove(_context.MethodOfSale.Where(x => x.PID == ID).FirstOrDefault());
                    _context.PriorAgencyMarketing.Remove(_context.PriorAgencyMarketing.Where(x => x.PID == ID).FirstOrDefault());
                    _context.CalculationOfCommission.Remove(_context.CalculationOfCommission.Where(x => x.PID == ID).FirstOrDefault());
                    _context.TenancyDetail.Remove(_context.TenancyDetail.Where(x => x.PID == ID).FirstOrDefault());
                    _context.PropertyInformation.RemoveRange(_context.PropertyInformation.Where(x => x.PID == ID));
                    _context.PropertyInformationDetail.Remove(_context.PropertyInformationDetail.Where(x => x.PID == ID).FirstOrDefault());
                    _context.SignaturesOfClient.RemoveRange(_context.SignaturesOfClient.Where(x => x.PID == ID));
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

        //public async Task<APIResponseModel> GeneratePDF(int id)
        //{
        //    try
        //    {

        //        Guid guAgentID;
        //        Guid item = _context.ListingAddress.Where(x => x.ID == id).FirstOrDefault().AgentID;

        //        Guid.TryParse(item.ToString(), out guAgentID);

        //        ApplicationUser user = await _userManager.FindByIdAsync(guAgentID.ToString());
        //        //var html = System.IO.File.ReadAllText(@"D:/Projects/RE360/RE360/RE360.API/Document/htmlpage.html");
        //        //var CSS = System.IO.File.ReadAllText(@"D:/Projects/RE360/RE360/RE360.API/Document/StyleSheet.css");
        //        GeneratePDF pDFHelper = new GeneratePDF(user, _context, _configuration);
        //        pDFHelper.DownloadPDF(id);
        //        return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success" };

        //    }
        //    catch (Exception ex)
        //    {

        //        return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
        //    }
        //}



        //public async Task<APIResponseModel> GeneratePDF(int id, IConfiguration configuration)
        //{
        //    try
        //    {
        //        Guid guAgentID;
        //        Guid item = _context.ListingAddress.Where(x => x.ID == id).FirstOrDefault().AgentID;

        //        Guid.TryParse(item.ToString(), out guAgentID);

        //        ApplicationUser user = await _userManager.FindByIdAsync(guAgentID.ToString());
        //        GeneratePDF pDFHelper = new GeneratePDF(user, _context, configuration);

        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await pDFHelper.DownloadPDF(id, memoryStream);

        //            // Upload the PDF to Azure Blob Storage
        //            string fileName = id.ToString() + ".pdf";
        //            string blobContainerName = "agentdoc";
        //            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_configuration["BlobStorageSettings:BlobStorageConnStr"].ToString());
        //            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
        //            CloudBlobContainer container = blobClient.GetContainerReference(blobContainerName);
        //            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

        //            // Upload the file to Azure Blob Storage using the saved memoryStream
        //            memoryStream.Position = 0; // Reset memoryStream position
        //            await blockBlob.UploadFromStreamAsync(memoryStream);
        //        }

        //        return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
        //    }
        //}


        //public async Task<APIResponseModel> GeneratePDF(int id, IConfiguration configuration)
        //{
        //    try
        //    {
        //        Guid guAgentID;
        //        Guid item = _context.ListingAddress.Where(x => x.ID == id).FirstOrDefault().AgentID;

        //        Guid.TryParse(item.ToString(), out guAgentID);

        //        ApplicationUser user = await _userManager.FindByIdAsync(guAgentID.ToString());
        //        GeneratePDF pDFHelper = new GeneratePDF(user, _context, configuration);

        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await pDFHelper.DownloadPDF(id, memoryStream);

        //            // Reset memoryStream position
        //            memoryStream.Position = 0;

        //            // Read memoryStream contents into a byte array
        //            byte[] pdfBytes = new byte[memoryStream.Length];
        //            memoryStream.Read(pdfBytes, 0, pdfBytes.Length);

        //            // Save the PDF to a local file
        //            string localFilePath = @"C:\Users\HP\Source\Repos\RE360\RE360.API\Document\" + id.ToString() + ".pdf";
        //            System.IO.File.WriteAllBytes(localFilePath, pdfBytes);

        //            // Upload the PDF to Azure Blob Storage
        //            string fileName = id.ToString() + ".pdf";
        //            string blobContainerName = "agentdoc";
        //            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_configuration["BlobStorageSettings:BlobStorageConnStr"].ToString());
        //            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
        //            CloudBlobContainer container = blobClient.GetContainerReference(blobContainerName);
        //            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

        //            // Upload the file to Azure Blob Storage using the byte array
        //            using (var azureStream = new MemoryStream(pdfBytes))
        //            {
        //                await blockBlob.UploadFromStreamAsync(azureStream);
        //            }
        //        }

        //        return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
        //    }
        //}
        public async Task<APIResponseModel> GeneratePDF(int id, IConfiguration configuration)
        {
            try
            {
                Guid guAgentID;
                Guid item = _context.ListingAddress.Where(x => x.ID == id).FirstOrDefault().AgentID;

                Guid.TryParse(item.ToString(), out guAgentID);

                ApplicationUser user = await _userManager.FindByIdAsync(guAgentID.ToString());
                GeneratePDF pDFHelper = new GeneratePDF(user, _context, configuration);

                using (var memoryStream = new MemoryStream())
                {
                    await pDFHelper.DownloadPDF(id, memoryStream);

                    // Save the PDF to a local file using memoryStream content
                    //string localFilePath = @"D:\Projects\RE360\RE360\RE360.API\Document\" + id.ToString() + ".pdf";
                    //System.IO.File.WriteAllBytes(localFilePath, memoryStream.ToArray());

                    // Upload the PDF to Azure Blob Storage
                    string fileName = id.ToString() + ".pdf";
                    string blobContainerName = "agentdoc";
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_configuration["BlobStorageSettings:BlobStorageConnStr"].ToString());
                    CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference(blobContainerName);
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                    blockBlob.Properties.ContentType = "application/pdf";
                    // Upload the file to Azure Blob Storage using the memoryStream content
                    using (var azureStream = new MemoryStream(memoryStream.ToArray()))
                    {
                        await blockBlob.UploadFromStreamAsync(azureStream);
                    }
                }
                var filePath = $"{configuration["BlobStorageSettings:AgentDocPath"]}{id}.pdf{configuration["BlobStorageSettings:AgentDocToken"]}";
                var WebUrl = $"{configuration["WebAppUrl"]}User/AgentDocView?pid={id}";

                return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "Success", Result = new { filePath = filePath, WebUrl = WebUrl } };
            }
            catch (Exception ex)
            {
                // Log the exception for further investigation
                Console.WriteLine($"Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
            }
        }

        private async Task<string> UploadBlobPdf(MemoryStream memoryStream, string fileName, string blobContainerName)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_configuration["BlobStorageSettings:BlobStorageConnStr"].ToString());
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(blobContainerName);
                await container.CreateIfNotExistsAsync();
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

                memoryStream.Position = 0;
                await blockBlob.UploadFromStreamAsync(memoryStream);

                // Optionally, you can set the appropriate blob properties here, such as Content-Type or metadata

                // Dispose the memory stream
                memoryStream.Dispose();

                return blockBlob.Uri.ToString(); // Return the blob URL
            }
            catch (Exception ex)
            {
                // Handle the exception, e.g., log it or throw it back
                throw;
            }
        }

        //private async Task UploadBlobPdf(MemoryStream memoryStream, string fileName, string blobContainerName)
        //{
        //    try
        //    {
        //        CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_configuration["BlobStorageSettings:BlobStorageConnStr"].ToString());
        //        CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
        //        CloudBlobContainer container = blobClient.GetContainerReference(blobContainerName);
        //        await container.CreateIfNotExistsAsync(); // Create the container if it doesn't exist
        //        CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

        //        memoryStream.Position = 0; // Reset the memoryStream position to the beginning
        //        await blockBlob.UploadFromStreamAsync(memoryStream); // Upload the PDF from the memory stream to the blob

        //        // Optionally, you can set the appropriate blob properties here, such as Content-Type or metadata

        //        // Once the upload is complete, you might want to close/dispose the memoryStream
        //        memoryStream.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle the exception, e.g., log it or throw it back
        //        throw;
        //    }
        //}




        //public async Task<APIResponseModel> GenerateAndStorePdf(int pid)
        //{
        //    try
        //    {
        //        ApplicationUser user = await _userManager.FindByIdAsync(/* provide the user ID */);

        //        GeneratePDF pDFHelper = new GeneratePDF(user, _context, _configuration);
        //        MemoryStream memoryStream = pDFHelper.GeneratePDF(pid);

        //        string pdfContainerName = "pdfs";
        //        string pdfFileName = pid.ToString() + ".pdf";
        //        string pdfAzureFilePath = await UploadBlobPdf(memoryStream, pdfFileName, pdfContainerName);

        //        Execution exeList = _context.Execution.Find(pid);
        //        if (exeList != null)
        //        {
        //            exeList.PdfUrl = pdfAzureFilePath;
        //            _context.SaveChanges();
        //        }

        //        return new APIResponseModel { StatusCode = StatusCodes.Status200OK, Message = "PDF generated and URL stored successfully." };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResponseModel { StatusCode = StatusCodes.Status403Forbidden, Message = ex.Message.ToString() };
        //    }
        //}

    }
}
