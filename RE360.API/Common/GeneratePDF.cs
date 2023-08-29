using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;
using Font = iTextSharp.text.Font;
using TextField = iTextSharp.text.pdf.TextField;
using RE360.API.Auth;
using Microsoft.AspNetCore.Http;
using RE360.API.DBModels;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;
using Dapper;
using Microsoft.Data.SqlClient;
using RE360.API.Models;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Validations;

namespace RE360.API.Common
{
    public class GeneratePDF : ControllerBase
    {
        private string paragraphText;
        private readonly RE360AppDbContext _context;
        private readonly IConfiguration _configuration;
        bool isRegistered = true;
        bool isNotregistered = true;
        bool isSoleSelected = false;
        bool isGeneralSelected = false;
        bool isPriceSelected = false;
        bool isNoPriceSelected = false;
        bool isAuctionSelected = false;
        bool isTenderSelected = false;
        bool isDeadlinesaleSelected = false;
        bool isUnlesssoldpriorSelected = false;
        
        private readonly ApplicationUser _user;
        string propertyAddress = string.Empty;

        public GeneratePDF(ApplicationUser user, RE360AppDbContext context = null, IConfiguration configuration = null)
        {

            _context = context;
            _configuration = configuration;
            _user = user;

        }
        public void DownloadPDF(int id)
        {
            List<ClientDetail> clientDetailsList = new List<ClientDetail>();
            ListingAddress listingAddressList = new ListingAddress();
            List<SolicitorDetail> solicitorList = new List<SolicitorDetail>();
            List<ParticularDetail> particularList = new List<ParticularDetail>();
            List<PriorAgencyMarketing> priorAgencyMarketingList = new List<PriorAgencyMarketing>();
            List<Estimates> estimatesList = new List<Estimates>();
            List<EstimatesDetail> additionaldisclosure = new List<EstimatesDetail>();
            List<MethodOfSale> methodOfSaleList = new List<MethodOfSale>();
            List<TenancyDetail> tenancyDetailsList = new List<TenancyDetail>();
            List<LegalDetail> legaldetailsList = new List<LegalDetail>();
            List<ContractDetail> contractdetailsList = new List<ContractDetail>();
            List<ContractRate> contractRatelsList = new List<ContractRate>();
            List<SignaturesOfClient> signatureOfClients = new List<SignaturesOfClient>();
            Execution signatureOfAgent = new Execution();

            var PropertyAttrList = (from p in _context.PropertyAttributeType
                                    select new
                                    {
                                        Name = p.Name,
                                        list = p.PropertyAttribute.ToList()
                                    }).ToList();

            //var clientDetailsList = _context.ClientDetail.Where(x => x.PID == id).ToList();
            methodOfSaleList = _context.MethodOfSale.Where(x => x.PID == id).ToList();
            tenancyDetailsList = _context.TenancyDetail.Where(x => x.PID == id).ToList();
            contractdetailsList = _context.ContractDetail.Where(x => x.PID == id).ToList();
            contractRatelsList = _context.ContractRate.Where(x => x.PID == id).ToList();
            signatureOfAgent = _context.Execution.Where(x => x.PID == id).FirstOrDefault();
            signatureOfClients = _context.SignaturesOfClient.Where(x => x.PID == id).ToList();
            priorAgencyMarketingList = _context.PriorAgencyMarketing.Where(x => x.PID == id).ToList();
            legaldetailsList = _context.LegalDetail.Where(x => x.PID == id).ToList();
            additionaldisclosure = _context.EstimatesDetail.Where(x => x.PID == id).ToList();
            clientDetailsList = _context.ClientDetail.Where(x => x.PID == id).ToList();
            listingAddressList = _context.ListingAddress.Where(x => x.ID == id).FirstOrDefault(); ;
            particularList = _context.ParticularDetail.Where(x => x.PID == id).ToList();
            var propertyInformationsList = _context.PropertyInformation.Where(x => x.PID == id).ToList();
            var othercommentList = _context.PropertyInformationDetail.Where(w => w.PID == id).ToList();
            estimatesList = _context.Estimates.Where(x => x.PID == id).ToList();
            var signaturesOfClientList = _context.SignaturesOfClient.Where(x => x.PID == id).ToList();
            solicitorList = _context.SolicitorDetail.Where(x => x.PID == id).ToList();
            var execution = _context.Execution.Where(x => x.PID == id).FirstOrDefault();
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
            var FinalList = (from l in _context.ListingAddress
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
                                 executionDetail = signaturesOfClientList
                             }).FirstOrDefault();

            var data = new List<PropertyAttributeTypeWithAllDataViewModel>();
            using (var connection = new SqlConnection(_configuration["ConnectionStrings:ConnStr"].ToString()))
            {
                var sql = string.Format("SELECT PAT.ID as PropertyAttributeTypeID,PAT.Name,PA.ID as PropertyAttributeID,pa.Name as PropertyAttributeName, case when ISNULL(PIL.ID,0)=0 THEN 0 ELSE 1 END as Checkbox, PIL.Remarks, PIL.Count FROM PropertyAttributeType PAT JOIN PropertyAttribute PA ON PAT.ID = PA.PropertyAttributeTypeID left JOIN PropertyInformation PIL ON PA.ID = PIL.PropAttrId AND PIL.PID={0}", id);
                data = connection.Query<PropertyAttributeTypeWithAllDataViewModel>(sql).ToList();
            }
            propertyAddress = listingAddressList != null ? listingAddressList.Address + "," + listingAddressList.Unit + "," + listingAddressList.Suburb + "," + listingAddressList.PostCode + "," + listingAddressList.StreetNumber + "," + listingAddressList.StreetName : string.Empty;
            propertyAddress = Regex.Replace(propertyAddress.Trim(','), ",,+", ",");
         

            float cellHeight = 100f;
            Document document = new Document();
            var memoryStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            document.Open();


            //writer.PageEvent = new PdfFooter();
            writer.PageEvent = new PdfFooter(_user, signatureOfAgent);


            PdfPTable table = new PdfPTable(3);
            Font lightblue = new Font(Font.FontFamily.COURIER, 9f, Font.BOLD, new BaseColor(43, 145, 175));
            Font brown = new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.NORMAL, new BaseColor(163, 21, 21));
            table.WidthPercentage = 108;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 12.5f;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 12.5f;

            PdfPTable lefttable = new PdfPTable(1);
            PdfPCell leftCell = new PdfPCell(new Phrase("PROPERTY INFORMATION", FontFactory.GetFont("verdana", 12, Font.BOLD)));
            leftCell.Border = Rectangle.NO_BORDER;
            lefttable.DefaultCell.Border = 0;
            lefttable.AddCell(leftCell);


            PdfPTable centertable = new PdfPTable(1);
            //PdfPCell centerCell = new PdfPCell(new Phrase("CONFIDENTIAL", new Font(Font.FontFamily.COURIER, 17f, Font.BOLDITALIC, new BaseColor(0, 48, 143))));
            PdfPCell centerCell = new PdfPCell(new Phrase(" "));
            centerCell.HorizontalAlignment = Element.ALIGN_CENTER;
            centerCell.Padding = 7.5f;

            IPdfPCellEvent roundedRectangle = new RoundedCellEvent(5, new BaseColor(System.Drawing.Color.Gray)); // Specify corner radius and fill color
            //centerCell.CellEvent = roundedRectangle;
            centerCell.Border = Rectangle.NO_BORDER;
            centerCell.PaddingBottom = 5f;
            centertable.DefaultCell.Border = 0;
            centertable.AddCell(centerCell);


            PdfPTable righttable = new PdfPTable(1);
            PdfPCell rightCell = new PdfPCell();
            rightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            rightCell.Border = Rectangle.NO_BORDER;
            righttable.DefaultCell.Border = 0;
            string logoPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "logopdf.png");
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
            if (System.IO.File.Exists(logoPath))
            {

                logo.ScaleAbsolute(70f, 70f);
                rightCell.PaddingLeft = 80f;
                rightCell.PaddingBottom = -75f;
                rightCell.PaddingTop = 0f;
                rightCell.PaddingRight = 0f;

                logo.SetAbsolutePosition(iTextSharp.text.PageSize.A4.Rotate().Width - 0, 20);
                rightCell.AddElement(logo);

            }
            righttable.AddCell(rightCell);
            table.DefaultCell.Border = 0;
            table.AddCell(lefttable);
            table.AddCell(centertable);
            table.AddCell(righttable);
            document.Add(table);

            AddContentToPDF(document, writer, listingAddressList,_user);

           

            if (clientDetailsList.Count == 1)
            {
                var batchList = clientDetailsList.Batch(1).ToList();
                foreach (var item in batchList)
                {
                    if (item.ToList().Count() == 1)
                    {
                        List<ClientDetail> clientDetails = new List<ClientDetail>();
                        clientDetails.Add(item.ToArray()[0]);
                        ClientDetail obj = new ClientDetail();
                        clientDetails.Add(obj);
                        AddClientTables(document, writer, clientDetails);
                    }
                    else
                    {
                        AddClientTables(document, writer, item.ToList());
                    }
                }
            }
            else
            {
                var batchList = clientDetailsList.Batch(2).ToList();

                foreach (var item in batchList)
                {
                    if (item.ToList().Count() == 1)
                    {
                        List<ClientDetail> clientDetails = new List<ClientDetail>();
                        clientDetails.Add(item.ToArray()[0]);
                        ClientDetail obj = new ClientDetail();
                        clientDetails.Add(obj);
                        AddClientTables(document, writer, clientDetails);
                    }
                    else
                    {
                        AddClientTables(document, writer, item.ToList());
                    }
                }
            }

            document.Add(new Paragraph("\r\n"));
            if (solicitorList.Count == 0)
            {
                SolicitorDetail obj = new SolicitorDetail();
                solicitorList.Add(obj);
            }
            for (int i = 0; i < solicitorList.Count; i++)
            {

                AddContentToSolicitordetails(document, writer, solicitorList[i]);
                document.Add(new Paragraph("\r\n"));
            }
            if (particularList.Count == 0)
            {
                ParticularDetail obj = new ParticularDetail();
                particularList.Add(obj);
            }
            if (legaldetailsList.Count == 0)
            {
                LegalDetail obj = new LegalDetail();
                legaldetailsList.Add(obj);
            }

            document.Add(new Paragraph("\r\n"));

            AddContentToParticulars(document, writer, particularList, data, legaldetailsList, id);


            document.NewPage();
           
            PdfPCell leftCell1 = new PdfPCell(new Phrase("PROPERTY INFORMATION", GetFont()));
            leftCell1.Border = Rectangle.NO_BORDER;

            table.AddCell(leftCell1);
            document.Add(table);

            var legalDetailsCheckboxList = data.Where(x => x.Name == "Legal Detail Title Type").ToList();
            int propertyID = _context.PropertyAttributeType.Where(x => x.Name == "Legal Detail Title Type").Select(x => x.ID).FirstOrDefault();
            var chkboxlistForLegalDetails = new List<PropertyAttributeTypeWithLegalParticularDetailModel>();
            using (var connection = new SqlConnection(_configuration["ConnectionStrings:ConnStr"].ToString()))
            {
                var sql = string.Format("SELECT PAT.ID as PropertyAttributeTypeID,PA.Name as PropertyAttributeName,PA.ID as PropertyAttributeID, LD.TitleTypeID,LD.PID, case when ISNULL(LD.TitleTypeID,0)=0 THEN 0 ELSE 1 END as Checkbox FROM PropertyAttributeType as PAT INNER JOIN PropertyAttribute as PA  ON PAT.ID = PA.PropertyAttributeTypeID LEFT JOIN legaldetail as LD on PA.id = LD.TitleTypeID and  LD.PID={0}", id + " WHERE  PA.PropertyAttributeTypeID=" + propertyID);
                chkboxlistForLegalDetails = connection.Query<PropertyAttributeTypeWithLegalParticularDetailModel>(sql).ToList();
            }



            PdfPTable legalstable = new PdfPTable(1);
            PdfPCell outercell1 = new PdfPCell();
            legalstable.DefaultCell.Border = 0;
            outercell1.BorderColor = BaseColor.DARK_GRAY;
            outercell1.FixedHeight = 134f;
            legalstable.WidthPercentage = 108; // Set the width percentage of the parent table        
            legalstable.SpacingBefore = 2f;
            legalstable.SpacingAfter = 5f;
            outercell1.CellEvent = new RectangleOverPdfPCellBorder("Legal details", 30f, 747, 65f, 20f, 32f, 760f);


            PdfPTable innerTable1 = new PdfPTable(2);
            innerTable1.DefaultCell.Border = 0;
            innerTable1.WidthPercentage = 100f;
            innerTable1.SpacingBefore = 10f;

            PdfContentByte cb2 = writer.DirectContent;
            BaseFont bf2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
            string text2 = "Title Type";
            cb2.BeginText();
            //put the alignment and coordinates here
            cb2.SetFontAndSize(bf2, 8f);
            cb2.ShowTextAligned(2, text2, 60, 733, 0);
            cb2.EndText();


            // Create the first inner cell
            PdfPCell innerCell3 = new PdfPCell();
            innerCell3.Border = 0;
            innerCell3.HorizontalAlignment = Element.ALIGN_CENTER;
            innerCell3.VerticalAlignment = Element.ALIGN_MIDDLE;
            innerCell3.FixedHeight = cellHeight;
            innerTable1.AddCell(innerCell3);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 720, 23, 730), chkboxlistForLegalDetails[0].Checkbox, 23, 720);
            PdfContentByte content42 = writer.DirectContent;
            BaseFont baseF14 = BaseFont.CreateFont();
            content42.SetFontAndSize(baseF14, 8);
            content42.BeginText();
            content42.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForLegalDetails[0].PropertyAttributeName, 53, 720, 0);
            content42.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 705, 23, 715), chkboxlistForLegalDetails[1].Checkbox, 23, 705);
            PdfContentByte content43 = writer.DirectContent;
            BaseFont baseF15 = BaseFont.CreateFont();
            content43.SetFontAndSize(baseF15, 8);
            content43.BeginText();
            content43.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForLegalDetails[1].PropertyAttributeName, 73, 705, 0);
            content43.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 700, 23, 690), chkboxlistForLegalDetails[2].Checkbox, 23, 690);
            PdfContentByte content44 = writer.DirectContent;
            BaseFont baseF16 = BaseFont.CreateFont();
            content44.SetFontAndSize(baseF16, 8);
            content44.BeginText();
            content44.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForLegalDetails[2].PropertyAttributeName, 73, 692, 0);
            content44.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 685, 23, 675), chkboxlistForLegalDetails[3].Checkbox, 23, 675);
            PdfContentByte content45 = writer.DirectContent;
            BaseFont baseF17 = BaseFont.CreateFont();
            content45.SetFontAndSize(baseF17, 8);
            content45.BeginText();
            content45.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForLegalDetails[3].PropertyAttributeName, 53, 678, 0);
            content45.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 670, 23, 660), chkboxlistForLegalDetails[4].Checkbox, 23, 660);
            PdfContentByte content46 = writer.DirectContent;
            BaseFont baseF18 = BaseFont.CreateFont();
            content46.SetFontAndSize(baseF18, 8);
            content46.BeginText();
            content46.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForLegalDetails[4].PropertyAttributeName, 70, 663, 0);
            content46.EndText();


            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 655, 23, 645), chkboxlistForLegalDetails[5].Checkbox, 23, 645);
            PdfContentByte content47 = writer.DirectContent;
            BaseFont baseF19 = BaseFont.CreateFont();
            content47.SetFontAndSize(baseF19, 8);
            content47.BeginText();
            content47.ShowTextAligned(PdfContentByte.ALIGN_CENTER, legalDetailsCheckboxList[5].PropertyAttributeName, 68, 648, 0);
            content47.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 640, 23, 630), chkboxlistForLegalDetails[6].Checkbox, 23, 630);
            PdfContentByte content48 = writer.DirectContent;
            BaseFont baseF20 = BaseFont.CreateFont();
            content48.SetFontAndSize(baseF20, 8);
            content48.BeginText();
            content48.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForLegalDetails[6].PropertyAttributeName, 45, 633, 0);
            content48.EndText();

            ////ADDING MORE FIELDS IN CELL
            TextField tf54 = new TextField(writer, new Rectangle(170, 745, 254, 730), legaldetailsList[0].LotNo);
            tf54.BorderColor = new BaseColor(232, 232, 232);
            tf54.BackgroundColor = new BaseColor(232, 232, 232);
            tf54.Text = legaldetailsList[0].LotNo == null ? " " : legaldetailsList[0].LotNo.ToString();
            tf54.FontSize = 8;
            tf54.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf54.GetTextField());
            PdfContentByte overContent58 = writer.DirectContent;
            BaseFont baseFont58 = BaseFont.CreateFont();
            overContent58.SetFontAndSize(baseFont58, 10);
            BaseFont overContent1 = BaseFont.CreateFont();
            overContent58.SetFontAndSize(overContent1, 8);
            overContent58.BeginText();
            overContent58.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Lot", 160, 733, 0);
            overContent58.EndText();

            TextField tf58 = new TextField(writer, new Rectangle(280, 745, 358, 730), "DP");
            tf58.BorderColor = new BaseColor(232, 232, 232);
            tf58.BackgroundColor = new BaseColor(232, 232, 232);
            tf58.FontSize = 8;
            tf58.Text = legaldetailsList[0].DepositedPlan == null ? " " : legaldetailsList[0].DepositedPlan.ToString();
            tf58.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf58.GetTextField());
            PdfContentByte overContent59 = writer.DirectContent;
            BaseFont baseFont59 = BaseFont.CreateFont();
            overContent59.SetFontAndSize(baseFont59, 8);
            overContent59.BeginText();
            overContent59.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "DP", 270, 733, 0);
            overContent59.EndText();

            TextField tf60 = new TextField(writer, new Rectangle(380, 745, 455, 730), "Title");
            tf60.BorderColor = new BaseColor(232, 232, 232);
            tf60.BackgroundColor = new BaseColor(232, 232, 232);
            tf60.FontSize = 8;
            tf60.Text = legaldetailsList[0].TitleIdentifier == null ? " " : legaldetailsList[0].TitleIdentifier.ToString();
            tf60.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf60.GetTextField());
            PdfContentByte overContent60 = writer.DirectContent;
            BaseFont baseFont60 = BaseFont.CreateFont();
            overContent60.SetFontAndSize(baseFont60, 8);
            overContent60.BeginText();
            overContent60.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Title", 369, 733, 0);
            overContent60.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(465, 740, 475, 730), legaldetailsList[0].IsPropertyUnitTitle, 465, 730);
            PdfContentByte overContent61 = writer.DirectContent;
            BaseFont baseFont61 = BaseFont.CreateFont();
            overContent61.SetFontAndSize(baseFont61, 8);
            overContent61.BeginText();
            overContent61.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Property is a unit title", 521, 730, 0);
            overContent61.EndText();

            TextField tf61 = new TextField(writer, new Rectangle(228, 720, 565, 703), "Registered");
            tf61.BorderColor = new BaseColor(232, 232, 232);
            tf61.BackgroundColor = new BaseColor(232, 232, 232);
            tf61.Text = legaldetailsList[0].RegisteredOwner == null ? " " : legaldetailsList[0].RegisteredOwner;
            tf61.FontSize = 8;
            tf61.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf61.GetTextField());
            PdfContentByte overContent62 = writer.DirectContent;
            BaseFont baseFont62 = BaseFont.CreateFont();
            overContent61.SetFontAndSize(baseFont62, 8);
            overContent61.BeginText();
            overContent61.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Registered owners", 190, 710, 0);
            overContent61.EndText();
            PdfContentByte overContent63 = writer.DirectContent;
            BaseFont baseFont63 = BaseFont.CreateFont();
            overContent63.SetFontAndSize(baseFont63, 8);
            overContent63.BeginText();
            overContent63.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "(Full name of client/trustees, used for legal documentation)", 260, 690, 0);
            overContent63.EndText();

            TextField tf64 = new TextField(writer, new Rectangle(222, 668, 393, 683), "Additional");
            tf64.BorderColor = new BaseColor(232, 232, 232);
            tf64.BackgroundColor = new BaseColor(232, 232, 232);
            tf64.FontSize = 8;
            tf64.Text = legaldetailsList[0].AdditionalDetails == null ? " " : legaldetailsList[0].AdditionalDetails;
            tf64.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf64.GetTextField());
            PdfContentByte overContent64 = writer.DirectContent;
            BaseFont baseFont64 = BaseFont.CreateFont();
            overContent64.SetFontAndSize(baseFont64, 8);
            overContent64.BeginText();
            overContent64.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Additional details", 188, 670, 0);
            overContent64.EndText();

            TextField tf65 = new TextField(writer, new Rectangle(460, 668, 560, 683), "Land value $");
            tf65.BorderColor = new BaseColor(232, 232, 232);
            tf65.BackgroundColor = new BaseColor(232, 232, 232);
            tf65.Text = legaldetailsList[0].LandValue == null ? " " : legaldetailsList[0].LandValue.ToString();
            tf65.FontSize = 8;
            tf65.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf65.GetTextField());
            PdfContentByte overContent65 = writer.DirectContent;
            BaseFont baseFont65 = BaseFont.CreateFont();
            overContent65.SetFontAndSize(baseFont65, 8);
            overContent65.BeginText();
            overContent65.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Land value $", 428, 670, 0);
            overContent65.EndText();

            TextField tf66 = new TextField(writer, new Rectangle(232, 655, 290, 640), "Improvement");
            tf66.BorderColor = new BaseColor(232, 232, 232);
            tf66.BackgroundColor = new BaseColor(232, 232, 232);
            tf66.Text = legaldetailsList[0].ImprovementValue == null ? " " : legaldetailsList[0].ImprovementValue.ToString();
            tf66.FontSize = 8;
            tf66.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf66.GetTextField());
            PdfContentByte overContent66 = writer.DirectContent;
            BaseFont baseFont66 = BaseFont.CreateFont();
            overContent66.SetFontAndSize(baseFont66, 8);
            overContent66.BeginText();
            overContent66.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Improvement value $", 192, 645, 0);
            overContent66.EndText();

            TextField tf67 = new TextField(writer, new Rectangle(362, 655, 430, 640), "Rateable");
            tf67.BorderColor = new BaseColor(232, 232, 232);
            tf67.BackgroundColor = new BaseColor(232, 232, 232);
            tf67.FontSize = 8;
            tf67.Text = legaldetailsList[0].RateableValue == null ? " " : legaldetailsList[0].RateableValue.ToString();
            tf67.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf67.GetTextField());
            PdfContentByte overContent67 = writer.DirectContent;
            BaseFont baseFont67 = BaseFont.CreateFont();
            overContent67.SetFontAndSize(baseFont67, 8);
            overContent67.BeginText();
            overContent67.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Rateable value $ ", 330, 645, 0);
            overContent67.EndText();



            TextField tf68 = new TextField(writer, new Rectangle(520, 655, 565, 640), "Rating");
            tf68.BorderColor = new BaseColor(232, 232, 232);
            tf68.BackgroundColor = new BaseColor(232, 232, 232);
            tf68.FontSize = 8;

            // Cast RatingValuationDate to DateTime and format the date as mm-yyyy
            string formattedDate = legaldetailsList[0].RatingValuationDate == null ? " " : ((DateTime)legaldetailsList[0].RatingValuationDate).ToString("MM-yyyy");
            tf68.Text = formattedDate;

            tf68.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf68.GetTextField());
            PdfContentByte overContent68 = writer.DirectContent;
            BaseFont baseFont68 = BaseFont.CreateFont();
            overContent68.SetFontAndSize(baseFont68, 8);
            overContent68.BeginText();
            overContent68.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Rating valuation date", 475, 645, 0);
            overContent68.EndText();






            PdfPCell innerCell4 = new PdfPCell();
            innerCell4.FixedHeight = cellHeight;
            innerCell4.Border = 0;
            innerCell4.HorizontalAlignment = Element.ALIGN_CENTER;
            innerCell4.VerticalAlignment = Element.ALIGN_MIDDLE;
            innerTable1.AddCell(innerCell3);
            outercell1.AddElement(innerTable1);
            legalstable.AddCell(outercell1);
            document.Add(legalstable);

            Phrase phrase2 = new Phrase();
            Chunk chunk1 = new Chunk(" ");

            var para1 = new Paragraph(chunk1);
            para1.PaddingTop = 2f;
            para1.Alignment = Element.ALIGN_TOP;
            para1.Font.Color = BaseColor.BLACK;

            //ADD CONTRACT DETAILS
            PdfPTable parentTable1 = new PdfPTable(3);
            parentTable1.DefaultCell.Border = 0;
            parentTable1.WidthPercentage = 108; // Set the width percentage of the parent table        
            parentTable1.SpacingBefore = 10f;
            parentTable1.SpacingAfter = 20f;

            float[] testcolumnWidths = new float[] { 50f, 1f, 50f };
            parentTable1.SetWidths(testcolumnWidths);


            PdfPCell cell12 = new PdfPCell();

            cell12.AddElement(para1);
            cell12.AddElement(phrase2);
            cell12.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell12.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            cell12.PaddingTop = 53;
            cell12.BorderColor = BaseColor.DARK_GRAY;
            cell12.BorderWidth = 1f;


          

            cell12.CellEvent = new RectangleOverPdfPCellBorder("Contract Details", 30f, 596f, 80f, 20f, 32f, 608f);
            PdfPCell cell21 = new PdfPCell();

            cell21.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell21.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            cell21.BorderColor = BaseColor.DARK_GRAY;
            cell21.BorderWidth = 1f;
            cell21.CellEvent = new RectangleOverPdfPCellBorder("Rates", 310f, 596f, 30f, 20f, 312f, 608f);
            //cell21.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Rates", 310f, 20f, 80f, 595f); //380, 630, 322, 645
            parentTable1.AddCell(cell12);
            //dummy cell created to have an empty space with width `0.1f` which was declared //above.
            PdfPCell cell0222 = new PdfPCell(new Phrase(" "));
            cell0222.Border = 0;
            parentTable1.AddCell(cell0222);
            parentTable1.AddCell(cell21);

            PdfPTable mainTable1 = new PdfPTable(1);
            mainTable1.DefaultCell.Border = 0;
            mainTable1.WidthPercentage = 108; // Set the width percentage of the main table

            // Add the parent table to the main table
            mainTable1.AddCell(parentTable1);
            mainTable1.DefaultCell.Border = 0;


            // Add the main table to the document
            document.Add(mainTable1);


           


            //ADD TEXT FIELDS FOR CONTRACT DETAILS
            TextField tf70 = new TextField(writer, new Rectangle(105, 590, 280, 575), "Authoritystart ");
            tf70.BorderColor = new BaseColor(232, 232, 232);
            tf70.BackgroundColor = new BaseColor(232, 232, 232);
            tf70.FontSize = 8;
            //tf70.Text = ((DateTime)FinalList.contractDetail.AuthorityStartDate).ToString("dd-MM-yyyy");
            if (FinalList != null && FinalList.contractDetail != null && FinalList.contractDetail.AuthorityStartDate != null)
            {
                string formattedDate1 = ((DateTime)FinalList.contractDetail.AuthorityStartDate).ToString("dd-MM-yyyy");
                tf70.Text = formattedDate1;
            }
            else
            {
                tf70.Text = " "; // Set a default value if the data is not available
            }


            tf70.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf70.GetTextField());
            PdfContentByte overContent70 = writer.DirectContent;
            BaseFont baseFont70 = BaseFont.CreateFont();
            overContent70.SetFontAndSize(baseFont70, 8);
            overContent70.BeginText();
            overContent70.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Authority start date", 90, 580, 0);
            overContent70.EndText();


            TextField tf71 = new TextField(writer, new Rectangle(105, 568, 280, 553), "Authorityend");
            tf71.BorderColor = new BaseColor(232, 232, 232);
            tf71.BackgroundColor = new BaseColor(232, 232, 232);
            tf71.FontSize = 8;
           
            if (FinalList != null && FinalList.contractDetail != null && FinalList.contractDetail.AuthorityEndDate != null)
            {
                string formattedDate2 = ((DateTime)FinalList.contractDetail.AuthorityEndDate).ToString("dd-MM-yyyy");
                tf71.Text = formattedDate2;
            }
            else
            {
                tf71.Text = " "; // Set a default value if the data is not available
            }
            tf71.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf71.GetTextField());
            PdfContentByte overContent71 = writer.DirectContent;
            BaseFont baseFont71 = BaseFont.CreateFont();
            overContent71.SetFontAndSize(baseFont71, 8);
            overContent71.BeginText();
            overContent71.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Authority end date", 90, 555, 0);
            overContent71.EndText();

            TextField tf72 = new TextField(writer, new Rectangle(130, 545, 245, 530), " marketing spend $ ");
            tf72.BorderColor = new BaseColor(232, 232, 232);
            tf72.BackgroundColor = new BaseColor(232, 232, 232);
            // tf72.Text = FinalList.contractDetail.AgreedMarketSpend.ToString();
            if (FinalList != null && FinalList.contractDetail != null && FinalList.contractDetail.AgreedMarketSpend != null)
            {
                string AgreedMarketSpend = FinalList.contractDetail.AgreedMarketSpend.ToString();
                tf72.Text = AgreedMarketSpend;
            }
            else
            {
                tf72.Text = " "; // Set a default value if the data is not available
            }
            tf72.FontSize = 8;

            tf72.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf72.GetTextField());
            PdfContentByte overContent72 = writer.DirectContent;
            BaseFont baseFont72 = BaseFont.CreateFont();
            overContent72.SetFontAndSize(baseFont72, 8);
            overContent72.BeginText();
            overContent72.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Agreed marketing spend $ ", 120, 533, 0);
            overContent72.EndText();

            PdfContentByte overContent73 = writer.DirectContent;
            BaseFont baseFont73 = BaseFont.CreateFont();
            overContent73.SetFontAndSize(baseFont73, 8);
            overContent73.BeginText();
            overContent73.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "(inc. GST) ", 285, 533, 0);
            overContent73.EndText();

            //ADDING MORE CONTRACT DETAILS

            TextField tf74 = new TextField(writer, new Rectangle(345, 590, 550, 575), "Water");
            tf74.BorderColor = new BaseColor(232, 232, 232);
            tf74.BackgroundColor = new BaseColor(232, 232, 232);
            if (FinalList != null && FinalList.contractRate != null && FinalList.contractRate.Water != null)
            {
                string Water = FinalList.contractRate.Water.ToString();
                tf74.Text = Water;
            }
            else
            {
                tf74.Text = " "; // Set a default value if the data is not available
            }
            //tf74.Text = FinalList.contractRate.Water.ToString();
            tf74.FontSize = 8;
            tf74.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf74.GetTextField());
            PdfContentByte overContent74 = writer.DirectContent;
            BaseFont baseFont74 = BaseFont.CreateFont();
            overContent74.SetFontAndSize(baseFont74, 8);
            overContent74.BeginText();
            overContent74.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Water $", 335, 580, 0);
            overContent74.EndText();
            PdfContentByte overContent76 = writer.DirectContent;
            BaseFont baseFont76 = BaseFont.CreateFont();
            overContent76.SetFontAndSize(baseFont76, 8);
            overContent76.BeginText();
            overContent76.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "pa", 565, 580, 0);
            overContent76.EndText();


            TextField tf75 = new TextField(writer, new Rectangle(345, 568, 550, 553), "Council");
            tf75.BorderColor = new BaseColor(232, 232, 232);
            tf75.BackgroundColor = new BaseColor(232, 232, 232);
            //tf75.Text = FinalList.contractRate.Council.ToString();
            if (FinalList != null && FinalList.contractRate != null && FinalList.contractRate.Council != null)
            {
                string Council = FinalList.contractRate.Council.ToString();
                tf75.Text = Council;
            }
            else
            {
                tf75.Text = " "; // Set a default value if the data is not available
            }
            tf75.FontSize = 8;
            tf75.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf75.GetTextField());
            PdfContentByte overContent75 = writer.DirectContent;
            BaseFont baseFont75 = BaseFont.CreateFont();
            overContent75.SetFontAndSize(baseFont75, 8);
            overContent75.BeginText();
            overContent75.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Council $", 340, 555, 0);
            overContent75.EndText();
            PdfContentByte overContent77 = writer.DirectContent;
            BaseFont baseFont77 = BaseFont.CreateFont();
            overContent77.SetFontAndSize(baseFont77, 8);
            overContent77.BeginText();
            overContent77.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "pa", 565, 555, 0);
            overContent77.EndText();

            TextField tf78 = new TextField(writer, new Rectangle(345, 545, 490, 525), "other");
            tf78.BorderColor = new BaseColor(232, 232, 232);
            tf78.BackgroundColor = new BaseColor(232, 232, 232);
            //tf78.Text = FinalList.contractRate.OtherValue.ToString();
            if (FinalList != null && FinalList.contractRate != null && FinalList.contractRate.OtherValue != null)
            {
                string OtherValue = FinalList.contractRate.OtherValue.ToString();
                tf78.Text = OtherValue;
            }
            else
            {
                tf78.Text = " "; // Set a default value if the data is not available
            }
            tf78.FontSize = 8;
            tf78.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf78.GetTextField());
            PdfContentByte overContent78 = writer.DirectContent;
            BaseFont baseFont78 = BaseFont.CreateFont();
            overContent78.SetFontAndSize(baseFont78, 8);
            overContent78.BeginText();
            overContent78.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Other $", 335, 533, 0);
            overContent78.EndText();
            if (FinalList != null && FinalList.contractRate != null)
            {
                bool isPerAnnum = FinalList.contractRate.IsPerAnnum;

                // Assuming AddCheckboxField function works similarly to your AddTextField function
                AddCheckboxField(document, writer, "myCheckbox", new Rectangle(500, 543, 512, 530), isPerAnnum, 500, 530);
            }
            else
            {
                // If the data is not available, you might want to decide the default state of the checkbox
                // For example, you could set it to unchecked
                AddCheckboxField(document, writer, "myCheckbox", new Rectangle(500, 543, 512, 530), false, 500, 530);
            }

            // AddCheckboxField(document, writer, "myCheckbox", new Rectangle(500, 543, 512, 530), FinalList.contractRate.IsPerAnnum, 500, 530);
            //AddCheckboxField(document, writer, "myCheckbox", new Rectangle(500, 543, 512, 530), FinalList.contractRate.IsPerAnnum, 500, 530);

            PdfContentByte overContent80 = writer.DirectContent;
            BaseFont baseFont80 = BaseFont.CreateFont();
            overContent80.SetFontAndSize(baseFont80, 8);
            overContent80.BeginText();
            overContent80.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "pa", 525, 535, 0);
            overContent80.EndText();
            if (FinalList != null && FinalList.contractRate != null)
            {
                bool IsPerQuarter = FinalList.contractRate.IsPerQuarter;

                // Assuming AddCheckboxField function works similarly to your AddTextField function
                AddCheckboxField(document, writer, "myCheckbox", new Rectangle(538, 543, 550, 530), IsPerQuarter, 538, 530);
            }
            else
            {
                // If the data is not available, you might want to decide the default state of the checkbox
                // For example, you could set it to unchecked
                AddCheckboxField(document, writer, "myCheckbox", new Rectangle(538, 543, 550, 530), false, 538, 530);
            }
            // AddCheckboxField(document, writer, "myCheckbox", new Rectangle(550, 543, 538, 530), FinalList.contractRate.IsPerQuarter, 538, 530);
            //AddCheckboxField(document, writer, "myCheckbox", new Rectangle(550, 543, 538, 530), FinalList.contractRate.IsPerQuarter, 538, 530);

            PdfContentByte overContent79 = writer.DirectContent;
            BaseFont baseFont79 = BaseFont.CreateFont();
            overContent79.SetFontAndSize(baseFont79, 8);
            overContent79.BeginText();
            overContent79.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "pq", 565, 535, 0);
            overContent79.EndText();


            //METHOD OF SALE DETAILS
            var methodOfSaleCheckboxAgencyList = data.Where(x => x.Name == "Method Of Sale Agency Type").ToList();
            int propertyID1 = _context.PropertyAttributeType.Where(x => x.Name == "Method Of Sale Agency Type").Select(x => x.ID).FirstOrDefault();
            var chkboxlistForAgencyType = new List<PropertyAttributeTypeWithMethodOfSaleModel>();
            using (var connection = new SqlConnection(_configuration["ConnectionStrings:ConnStr"].ToString()))
            {
                var sql = string.Format("SELECT PAT.ID,PA.Name as PropertyAttributeName, PA.PropertyAttributeTypeID,PA.Name ,PA.ID as PropertyAttributeID,MOS.AgencyTypeID,MOS.MethodOfSaleID, MOS.AgencyOtherTypeRemark, MOS.Price, MOS.PriceRemark, MOS.AuctionDate, MOS.AuctionTime, MOS.AuctionVenue, MOS.Auctioneer, MOS.TenderDate, MOS.TenderTime, MOS.IsMortgageeSale, MOS.IsAsIs, MOS.IsAuctionUnlessSoldPrior, MOS.IsTenderUnlessSoldPrior,MOS.IsAuctionOnSite,MOS.TenderVenue,MOS.PID, case when ISNULL(MOS.AgencyTypeID,0)=0 THEN 0 ELSE 1 END as Checkbox FROM PropertyAttributeType as PAT  INNER JOIN PropertyAttribute as PA  ON PAT.ID = PA.PropertyAttributeTypeID left join MethodOfSale as MOS on PA.id = MOS.AgencyTypeID  AND MOS.PID={0}", id + " WHERE  PA.PropertyAttributeTypeID=" + propertyID1);
                chkboxlistForAgencyType = connection.Query<PropertyAttributeTypeWithMethodOfSaleModel>(sql).ToList();
            }

            foreach (var item in chkboxlistForAgencyType)
            {
                if (item.PropertyAttributeName == "Sole") { isSoleSelected = item.Checkbox; }
                if (item.PropertyAttributeName == "General") { isGeneralSelected = item.Checkbox; }
            }

            var methodOfSaleCheckboxList = data.Where(x => x.Name == "Method Of Sale").ToList();
            int propertyID2 = _context.PropertyAttributeType.Where(x => x.Name == "Method Of Sale").Select(x => x.ID).FirstOrDefault();
            var chkboxlistForMethodOfSale = new List<PropertyAttributeTypeWithMethodOfSaleModel>();
            using (var connection = new SqlConnection(_configuration["ConnectionStrings:ConnStr"].ToString()))
            {
                var sql = string.Format("SELECT PAT.ID,PA.Name as PropertyAttributeName, PA.PropertyAttributeTypeID,PA.Name ,PA.ID as PropertyAttributeID,MOS.AgencyTypeID,MOS.MethodOfSaleID, MOS.AgencyOtherTypeRemark, MOS.Price, MOS.PriceRemark, MOS.AuctionDate, MOS.AuctionTime, MOS.AuctionVenue, MOS.Auctioneer, MOS.TenderDate, MOS.TenderTime, MOS.IsMortgageeSale, MOS.IsAsIs, MOS.IsAuctionUnlessSoldPrior, MOS.IsTenderUnlessSoldPrior,MOS.IsAuctionOnSite,MOS.TenderVenue,MOS.PID, case when ISNULL(MOS.MethodOfSaleID,0)=0 THEN 0 ELSE 1 END as Checkbox FROM PropertyAttributeType as PAT  INNER JOIN PropertyAttribute as PA  ON PAT.ID = PA.PropertyAttributeTypeID left join MethodOfSale as MOS on PA.id = MOS.MethodOfSaleID  AND MOS.PID={0}", id + "  WHERE  PA.PropertyAttributeTypeID=" + propertyID2);
                chkboxlistForMethodOfSale = connection.Query<PropertyAttributeTypeWithMethodOfSaleModel>(sql).ToList();
            }

            foreach (var item in chkboxlistForMethodOfSale)
            {
                if (item.PropertyAttributeName == "Price") { isPriceSelected = item.Checkbox; }
                if (item.PropertyAttributeName == "No price") { isNoPriceSelected = item.Checkbox; }
                if (item.PropertyAttributeName == "Auction") { isAuctionSelected = item.Checkbox; }
                if (item.PropertyAttributeName == "Tender") { isTenderSelected = item.Checkbox; }
                if (item.PropertyAttributeName == "Deadline sale") { isDeadlinesaleSelected = item.Checkbox; }
                if (item.PropertyAttributeName == "Unless sold prior") { isUnlesssoldpriorSelected = item.Checkbox; }
            }


            PdfPTable methodOfTable = new PdfPTable(1);

            PdfPCell outercell21 = new PdfPCell();
            methodOfTable.DefaultCell.Border = 0;
            outercell21.BorderColor = BaseColor.DARK_GRAY;
            outercell21.FixedHeight = 123f;
            methodOfTable.WidthPercentage = 108; // Set the width percentage of the parent table        
            methodOfTable.SpacingBefore = 12f;
            methodOfTable.SpacingAfter = 10f;
            outercell21.CellEvent = new RectangleOverPdfPCellBorder("Method of Sale", 30f, 490f, 77f, 20f, 32f, 500f);
            //outercell21.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Method of Sale", 50f, 20f, 80f, 490f);
            //particulartable.AddCell(outercell);

            PdfPTable innerTable21 = new PdfPTable(2);
            innerTable21.DefaultCell.Border = 0;
            innerTable21.WidthPercentage = 100f;
            innerTable21.SpacingBefore = 10f;

            PdfContentByte cb21 = writer.DirectContent;
            BaseFont bf21 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
            string text21 = "Agency Type";
            cb21.BeginText();
            cb21.SetFontAndSize(bf21, 8);
            // put the alignment and coordinates here
            cb21.ShowTextAligned(1, text21, 55, 482, 0);
            cb21.EndText();
            // Create the first inner cell
            PdfPCell innerCell21 = new PdfPCell();
            innerCell21.FixedHeight = cellHeight;
            innerCell21.Border = 0;
            innerCell21.HorizontalAlignment = Element.ALIGN_CENTER;
            innerCell21.VerticalAlignment = Element.ALIGN_MIDDLE;
            innerTable21.AddCell(innerCell21);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(25, 465, 35, 475), chkboxlistForAgencyType[0].Checkbox, 25, 465);
            PdfContentByte content21 = writer.DirectContent;
            BaseFont baseF21 = BaseFont.CreateFont();
            content21.SetFontAndSize(baseF21, 8);
            content21.BeginText();
            content21.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForAgencyType[0].PropertyAttributeName, 49, 466, 0);
            content21.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(25, 450, 35, 460), chkboxlistForAgencyType[1].Checkbox, 25, 450);
            PdfContentByte content56 = writer.DirectContent;
            BaseFont baseF56 = BaseFont.CreateFont();
            content56.SetFontAndSize(baseF56, 8);
            content56.BeginText();
            content56.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForAgencyType[1].PropertyAttributeName, 55, 452, 0);
            content56.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(25, 435, 35, 445), chkboxlistForAgencyType[2].Checkbox, 25, 435);
            PdfContentByte content57 = writer.DirectContent;
            BaseFont baseF57 = BaseFont.CreateFont();
            content57.SetFontAndSize(baseF57, 8);
            content57.BeginText();
            content57.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForAgencyType[2].PropertyAttributeName, 62, 436, 0);
            content57.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(25, 418, 35, 428), chkboxlistForAgencyType[3].Checkbox, 25, 418);
            PdfContentByte content58 = writer.DirectContent;
            BaseFont baseF58 = BaseFont.CreateFont();
            content58.SetFontAndSize(baseF58, 8);
            content58.BeginText();
            content58.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForAgencyType[3].PropertyAttributeName, 49, 419, 0);
            content58.EndText();

            TextField tf85 = new TextField(writer, new Rectangle(25, 410, 110, 390), "Other");
            tf85.BorderColor = new BaseColor(232, 232, 232);
            tf85.BackgroundColor = new BaseColor(232, 232, 232);
            if (FinalList != null && FinalList.methodOfSale != null && FinalList.methodOfSale.AgencyOtherTypeRemark != null)
            {
                string AgencyOtherTypeRemark = FinalList.methodOfSale.AgencyOtherTypeRemark.ToString();
                tf85.Text = AgencyOtherTypeRemark;
            }
            else
            {
                tf85.Text = " "; // Set a default value if the data is not available
            }
            // tf85.Text = FinalList.methodOfSale.AgencyOtherTypeRemark.ToString();
            tf85.FontSize = 8;
            tf85.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf85.GetTextField());



            PdfContentByte cb22 = writer.DirectContent;
            BaseFont bf22 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
            string text22 = "Method of sale";
            cb22.BeginText();
            cb22.SetFontAndSize(bf22, 8);
            // 2ut the alignment and coordinates here
            cb22.ShowTextAligned(1, text22, 180, 482, 0);
            cb22.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(150, 465, 160, 475), chkboxlistForMethodOfSale[0].Checkbox, 150, 465);
            PdfContentByte content25 = writer.DirectContent;
            BaseFont baseF25 = BaseFont.CreateFont();
            content25.SetFontAndSize(baseF25, 8);
            content25.BeginText();
            content25.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForMethodOfSale[0].PropertyAttributeName, 174, 465, 0);
            content25.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(220, 465, 230, 475), chkboxlistForMethodOfSale[3].Checkbox, 220, 465);
            PdfContentByte content27 = writer.DirectContent;
            BaseFont baseF27 = BaseFont.CreateFont();
            content27.SetFontAndSize(baseF25, 8);
            content27.BeginText();
            content27.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForMethodOfSale[3].PropertyAttributeName, 250, 465, 0);
            content27.EndText();


            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(150, 450, 160, 460), chkboxlistForMethodOfSale[1].Checkbox, 150, 450);
            PdfContentByte content26 = writer.DirectContent;
            BaseFont baseF26 = BaseFont.CreateFont();
            content26.SetFontAndSize(baseF26, 8);
            content26.BeginText();
            content26.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForMethodOfSale[1].PropertyAttributeName, 180, 452, 0);
            content26.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(220, 450, 230, 460), chkboxlistForMethodOfSale[4].Checkbox, 220, 450);
            PdfContentByte content29 = writer.DirectContent;
            BaseFont baseF29 = BaseFont.CreateFont();
            content29.SetFontAndSize(baseF29, 8);
            content29.BeginText();
            content29.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForMethodOfSale[4].PropertyAttributeName, 250, 452, 0);
            content29.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(150, 435, 160, 445), chkboxlistForMethodOfSale[2].Checkbox, 150, 435);
            PdfContentByte content28 = writer.DirectContent;
            BaseFont baseF28 = BaseFont.CreateFont();
            content28.SetFontAndSize(baseF28, 8);
            content28.BeginText();
            content28.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForMethodOfSale[2].PropertyAttributeName, 190, 436, 0);
            content28.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(220, 435, 230, 445), chkboxlistForMethodOfSale[5].Checkbox, 220, 435);
            PdfContentByte content30 = writer.DirectContent;
            BaseFont baseF30 = BaseFont.CreateFont();
            content30.SetFontAndSize(baseF30, 8);
            content30.BeginText();
            content30.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chkboxlistForMethodOfSale[5].PropertyAttributeName, 268, 436, 0);
            content30.EndText();

            PdfContentByte content63 = writer.DirectContent;
            BaseFont baseF63 = BaseFont.CreateFont();
            content63.SetFontAndSize(baseF63, 8);
            content63.BeginText();
            content63.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Price (specific amount)", 195, 419, 0);
            content63.EndText();

            PdfContentByte content64 = writer.DirectContent;
            BaseFont baseF64 = BaseFont.CreateFont();
            content64.SetFontAndSize(baseF63, 8);
            content64.BeginText();
            content64.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "$", 150, 395, 0);
            content64.EndText();
            TextField tf87 = new TextField(writer, new Rectangle(300, 410, 160, 390), "$ textbox");
            tf87.BorderColor = new BaseColor(232, 232, 232);
            tf87.BackgroundColor = new BaseColor(232, 232, 232);






            // tf87.Text = FinalList.methodOfSale.Price.ToString();
            if (FinalList != null && FinalList.methodOfSale != null && FinalList.methodOfSale.Price != null)
            {
                string Price = isPriceSelected == true ? FinalList.methodOfSale.Price.ToString() : "";
                tf87.Text = Price;
            }
            else
            {
                tf87.Text = " "; // Set a default value if the data is not available
            }
            tf87.FontSize = 8;
            tf87.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf87.GetTextField());


            





            PdfContentByte cb23 = writer.DirectContent;
            BaseFont bf23 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
            string text23 = "Auction";
            cb23.BeginText();
            cb23.SetFontAndSize(bf23, 8);
            // 3ut the alignment and coordinates here
            cb23.ShowTextAligned(1, text23, 325, 482, 0);
            cb23.EndText();





            string logoPath1 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "Calender.png");
            iTextSharp.text.Image logo1 = iTextSharp.text.Image.GetInstance(logoPath1);
            if (System.IO.File.Exists(logoPath1))
            {
                logo1.ScaleAbsolute(15f, 15f);
                logo1.SetAbsolutePosition(308, 465);


                document.Add(logo1);
            }
         
            TextField tf89 = new TextField(writer, new Rectangle(325, 478, 360, 465), "Auction Date");
            tf89.BorderColor = new BaseColor(232, 232, 232);
            tf89.BackgroundColor = new BaseColor(232, 232, 232);
            //tf89.Text = ((DateTime)FinalList.methodOfSale.AuctionDate).ToString("dd-MM-yyyy");
            if (FinalList != null && FinalList.methodOfSale != null && FinalList.methodOfSale.AuctionDate != null)
            {
                string AuctionDate = isAuctionSelected ==true ?  ((DateTime)FinalList.methodOfSale.AuctionDate).ToString("dd-MM-yyyy") : " " ;
                tf89.Text = AuctionDate;
            }
            else
            {
                tf89.Text = " "; // Set a default value if the data is not available
            }
            // tf89.FontSize= 8;
            tf89.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf89.GetTextField());

            string logoPath2 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "Clock.png");
            iTextSharp.text.Image logo2 = iTextSharp.text.Image.GetInstance(logoPath2);
            if (System.IO.File.Exists(logoPath2))
            {
                logo2.ScaleAbsolute(15f, 15f);
                logo2.SetAbsolutePosition(365, 465);


                document.Add(logo2);
            }

            TextField tf90 = new TextField(writer, new Rectangle(385, 478, 420, 465), "Auction Time");
            tf90.BorderColor = new BaseColor(232, 232, 232);
            tf90.BackgroundColor = new BaseColor(232, 232, 232);
            // tf90.FontSize = 8;
            //tf90.Text = DateTimeOffset.Parse(FinalList.methodOfSale.AuctionTime).ToString("hh:mm tt");
            if (FinalList != null && FinalList.methodOfSale != null && FinalList.methodOfSale.AuctionTime != null)
            {
                string AuctionTime = isAuctionSelected == true ? DateTimeOffset.Parse(FinalList.methodOfSale.AuctionTime).ToString("hh:mm tt") : "";
                tf90.Text = AuctionTime;
            }
            else
            {
                tf90.Text = " "; // Set a default value if the data is not available
            }
            tf90.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf90.GetTextField());


            TextField tf91 = new TextField(writer, new Rectangle(335, 455, 410, 435), "Venue");
            tf91.BorderColor = new BaseColor(232, 232, 232);
            tf91.BackgroundColor = new BaseColor(232, 232, 232);
            //tf91.Text = FinalList.methodOfSale.TenderVenue;
            if (FinalList != null && FinalList.methodOfSale != null && FinalList.methodOfSale.TenderVenue != null)
            {
                string TenderVenue = isAuctionSelected == true ? FinalList.methodOfSale.TenderVenue : "";
                tf91.Text = TenderVenue;
            }
            else
            {
                tf91.Text = " "; // Set a default value if the data is not available
            }

           
            tf91.Options = TextField.READ_ONLY;
            tf91.FontSize = 8;
            writer.AddAnnotation(tf91.GetTextField());
            PdfContentByte overContent91 = writer.DirectContent;
            BaseFont baseFont91 = BaseFont.CreateFont();
            overContent91.SetFontAndSize(baseFont91, 8);
            overContent91.BeginText();
            overContent91.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Venue", 330, 436, 0);
            overContent91.EndText();

            TextField tf92 = new TextField(writer, new Rectangle(350, 420, 425, 400), "Auctioneer Venue");
            tf92.BorderColor = new BaseColor(232, 232, 232);
            tf92.BackgroundColor = new BaseColor(232, 232, 232);
            //tf92.Text = FinalList.methodOfSale.AuctionVenue;
            if (FinalList != null && FinalList.methodOfSale != null && FinalList.methodOfSale.AuctionVenue != null)
            {
                string AuctionVenue = isAuctionSelected == true ? FinalList.methodOfSale.AuctionVenue:"";
                tf92.Text = AuctionVenue;
            }
            else
            {
                tf92.Text = " "; // Set a default value if the data is not available
            }
            tf92.FontSize = 8;
            tf92.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf92.GetTextField());
            PdfContentByte overContent92 = writer.DirectContent;
            BaseFont baseFont92 = BaseFont.CreateFont();
            overContent92.SetFontAndSize(baseFont91, 8);
            overContent92.BeginText();
            overContent92.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Auctioneer", 348, 410, 0);
            overContent92.EndText();

            PdfContentByte cb24 = writer.DirectContent;
            BaseFont bf24 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
            string text24 = "Tender";
            cb24.BeginText();
            cb24.SetFontAndSize(bf24, 8);
            // 4ut the alignment and coordinates here
            cb24.ShowTextAligned(1, text24, 450, 482, 0);
            cb24.EndText();

            string logoPath3 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "Calender.png");
            iTextSharp.text.Image logo3 = iTextSharp.text.Image.GetInstance(logoPath3);
            if (System.IO.File.Exists(logoPath3))
            {
                logo3.ScaleAbsolute(15f, 15f);
                logo3.SetAbsolutePosition(437, 465);
                document.Add(logo3);
            }
            TextField tf94 = new TextField(writer, new Rectangle(455, 478, 490, 465), "Tender Date");
            tf94.BorderColor = new BaseColor(232, 232, 232);
            tf94.BackgroundColor = new BaseColor(232, 232, 232);
            // tf94.Text = ((DateTime)FinalList.methodOfSale.TenderDate).ToString("dd-MM-yyyy");
            if (FinalList != null && FinalList.methodOfSale != null && FinalList.methodOfSale.TenderDate != null)
            {
                string TenderDate = isTenderSelected  == true ? ((DateTime)FinalList.methodOfSale.TenderDate).ToString("dd-MM-yyyy") : "";
                tf94.Text = TenderDate;
            }
            else
            {
                tf94.Text = " "; // Set a default value if the data is not available
            }
            //  tf94.FontSize = 8;
            tf94.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf94.GetTextField());

            string logoPath4 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "Clock.png");
            iTextSharp.text.Image logo4 = iTextSharp.text.Image.GetInstance(logoPath4);
            if (System.IO.File.Exists(logoPath4))
            {
                logo4.ScaleAbsolute(15f, 15f);
                logo4.SetAbsolutePosition(500, 465);


                document.Add(logo4);
            }
            TextField tf95 = new TextField(writer, new Rectangle(520, 478, 560, 465), "TenderTime");
            tf95.BorderColor = new BaseColor(232, 232, 232);
            tf95.BackgroundColor = new BaseColor(232, 232, 232);
            //  tf95.FontSize = 8;
            // tf95.Text = DateTimeOffset.Parse(FinalList.methodOfSale.TenderTime).ToString("hh:mm tt");
            if (FinalList != null && FinalList.methodOfSale != null && FinalList.methodOfSale.TenderTime != null)
            {
                string TenderTime = isTenderSelected == true ? DateTimeOffset.Parse(FinalList.methodOfSale.TenderTime).ToString("hh:mm tt"):"";
                tf95.Text = TenderTime;
            }
            else
            {
                tf95.Text = " "; // Set a default value if the data is not available
            }
            tf95.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf95.GetTextField());


            PdfContentByte cb25 = writer.DirectContent;
            BaseFont bf25 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
            string text25 = "Deadline sale date";
            cb25.BeginText();
            cb25.SetFontAndSize(bf25, 8);
            // 4ut the alignment and coordinates here
            cb25.ShowTextAligned(1, text25, 480, 440, 0);
            cb25.EndText();

            string logoPath5 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "Calender.png");
            iTextSharp.text.Image logo5 = iTextSharp.text.Image.GetInstance(logoPath5);
            if (System.IO.File.Exists(logoPath5))
            {
                logo5.ScaleAbsolute(15f, 15f);
                logo5.SetAbsolutePosition(437, 420);
                document.Add(logo5);
            }

            TextField tf96 = new TextField(writer, new Rectangle(455, 435, 490, 420), "Deadline Date");
            tf96.BorderColor = new BaseColor(232, 232, 232);
            tf96.BackgroundColor = new BaseColor(232, 232, 232);
            // tf96.Text = ((DateTime)FinalList.methodOfSale.DeadLineDate).ToString("dd-MM-yyyy");
            if (FinalList != null && FinalList.methodOfSale != null && FinalList.methodOfSale.DeadLineDate != null)
            {
                string DeadLineDate = isDeadlinesaleSelected == true ? ((DateTime)FinalList.methodOfSale.DeadLineDate).ToString("dd-MM-yyyy") : "";
                tf96.Text = DeadLineDate;
            }
            else
            {
                tf96.Text = " "; // Set a default value if the data is not available
            }
            tf96.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf96.GetTextField());

            string logoPath6 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "Clock.png");
            iTextSharp.text.Image logo6 = iTextSharp.text.Image.GetInstance(logoPath6);
            if (System.IO.File.Exists(logoPath6))
            {
                logo6.ScaleAbsolute(15f, 15f);
                logo6.SetAbsolutePosition(500, 420);
                document.Add(logo6);
            }

            TextField tf97 = new TextField(writer, new Rectangle(520, 435, 560, 420), "Deadline Time");
            tf97.BorderColor = new BaseColor(232, 232, 232);
            tf97.BackgroundColor = new BaseColor(232, 232, 232);
            //  tf97.FontSize = 8;
            //tf97.Text = DateTimeOffset.Parse(FinalList.methodOfSale.DeadLineTime).ToString("hh:mm:ss tt");
            if (FinalList != null && FinalList.methodOfSale != null && FinalList.methodOfSale.DeadLineTime != null)
            {
                string DeadLineTime = isDeadlinesaleSelected == true ? DateTimeOffset.Parse(FinalList.methodOfSale.DeadLineTime).ToString("hh:mm tt"):"";
                tf97.Text = DeadLineTime;
            }
            else
            {
                tf97.Text = " "; // Set a default value if the data is not available
            }
            tf97.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf97.GetTextField());

            if (methodOfSaleList != null && methodOfSaleList.Count > 0)
            {
                bool isMortgageeSale = methodOfSaleList[0].IsMortgageeSale;

                AddCheckboxField(document, writer, "myCheckbox", new Rectangle(437, 410, 448, 400), isMortgageeSale, 437, 400);
            }
            else
            {
                // Handle the case when methodOfSaleList is null or empty
                // For example, you might want to provide a default value for the checkbox
                AddCheckboxField(document, writer, "myCheckbox", new Rectangle(437, 410, 448, 400), false, 437, 400);
            }


            // AddCheckboxField(document, writer, "myCheckbox", new Rectangle(437, 410, 448, 400), methodOfSaleList[0].IsMortgageeSale, 437, 400);
            PdfContentByte content59 = writer.DirectContent;
            BaseFont baseF59 = BaseFont.CreateFont();
            content59.SetFontAndSize(baseF59, 8);
            content59.BeginText();
            content59.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Mortgagee sale", 480, 403, 0);
            content59.EndText();



            if (methodOfSaleList != null && methodOfSaleList.Count > 0)
            {
                bool isAsIs = methodOfSaleList[0].IsAsIs;

                AddCheckboxField(document, writer, "myCheckbox", new Rectangle(437, 395, 448, 385), isAsIs, 437, 400);
            }
            else
            {
                // Handle the case when methodOfSaleList is null or empty
                // For example, you might want to provide a default value for the checkbox
                AddCheckboxField(document, writer, "myCheckbox", new Rectangle(437, 395, 448, 385), false, 437, 400);
            }
            // AddCheckboxField(document, writer, "myCheckbox", new Rectangle(437, 395, 448, 385), methodOfSaleList[0].IsAsIs, 437, 385);
            PdfContentByte content60 = writer.DirectContent;
            BaseFont baseF60 = BaseFont.CreateFont();
            content60.SetFontAndSize(baseF60, 8);
            content60.BeginText();
            content60.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "As is, where is", 480, 389, 0);
            content60.EndText();



            //CODE FOR CHATTELS CHECKBOXES

            // Create the second inner cell
            PdfPCell innerCell22 = new PdfPCell();
            innerCell22.FixedHeight = cellHeight;
            innerCell22.Border = 0;
            innerCell22.HorizontalAlignment = Element.ALIGN_CENTER;
            innerCell22.VerticalAlignment = Element.ALIGN_MIDDLE;
            innerTable21.AddCell(innerCell22);
            outercell21.AddElement(innerTable21);
            methodOfTable.AddCell(outercell21);
            document.Add(methodOfTable);

            // Step 4: Create a new Table with 5 columns
            PdfPTable multipleMainTable = new PdfPTable(5);

            multipleMainTable.SpacingBefore = 10f;
            multipleMainTable.WidthPercentage = 108;
            multipleMainTable.DefaultCell.Border = 0;
            float[] columnWidths = new float[] { 36f, 30f, 30f, 30f, 30f };
            multipleMainTable.SetWidths(columnWidths);
            // Step 5: Create the cells for each column using nested tables

            var chattelslist = data.Where(w => w.Name == "Chattels").ToList();


            // Column 1 
            PdfPTable col1Table = new PdfPTable(1);
            PdfPCell cell11 = new PdfPCell(new Phrase());
            cell11.FixedHeight = 305f; // Set the height of the cell
            col1Table.DefaultCell.Border = 0;

            cell11.BorderColor = BaseColor.BLACK;
            cell11.Padding = 10;

            //col1Table.WidthPercentage = 100; // Set the width percentage of the parent table        
            col1Table.SpacingBefore = 3f;
            col1Table.SpacingAfter = 3f;
            cell11.CellEvent = new RectangleOverPdfPCellBorder("Chattels", 30f, 340f, 43f, 20f, 32f, 355f);
            //cell11.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Chattels", 40f, 20f, 80f, 340f);

            col1Table.AddCell(cell11);
            multipleMainTable.AddCell(col1Table);

            //CHECKBOX FIELDS

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 320, 23, 310), chattelslist[0].Checkbox, 23, 310);
            PdfContentByte content211 = writer.DirectContent;
            BaseFont baseF211 = BaseFont.CreateFont();
            content211.SetFontAndSize(baseF211, 8);
            content211.BeginText();
            content211.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[0].PropertyAttributeName, 49, 312, 0);
            content211.EndText();
            TextField tf211 = new TextField(writer, new Rectangle(110, 325, 130, 310), "Blinds");
            tf211.BorderColor = new BaseColor(232, 232, 232);
            tf211.BackgroundColor = new BaseColor(232, 232, 232);
            tf211.Options = TextField.READ_ONLY;
            // tf211.Text = chattelslist[0].Count.ToString();
            if (chattelslist.Count > 0)
            {
                tf211.Text = chattelslist[0].Count != 0 ? chattelslist[0].Count.ToString() : "";
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf211.Text = ""; // Set a default value or handle it based on your requirements
            }

            tf211.FontSize = 8;
            tf211.TextColor = BaseColor.BLACK;
            writer.AddAnnotation(tf211.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 305, 23, 295), chattelslist[1].Checkbox, 23, 295);
            PdfContentByte content212 = writer.DirectContent;
            BaseFont baseF212 = BaseFont.CreateFont();
            content212.SetFontAndSize(baseF212, 8);
            content212.BeginText();
            content212.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[1].PropertyAttributeName, 61, 297, 0);
            content212.EndText();
            TextField tf212 = new TextField(writer, new Rectangle(110, 308, 130, 295), "Burglar");
            tf212.BorderColor = new BaseColor(232, 232, 232);
            tf212.BackgroundColor = new BaseColor(232, 232, 232);
            tf212.Options = TextField.READ_ONLY;
            tf212.Text =  chattelslist[1].Count != 0 ? chattelslist[1].Count.ToString() : "";
            tf212.TextColor = BaseColor.BLACK;
            tf212.FontSize = 8;
            writer.AddAnnotation(tf212.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 290, 23, 280), chattelslist[2].Checkbox, 23, 280);
            PdfContentByte content213 = writer.DirectContent;
            BaseFont baseF213 = BaseFont.CreateFont();
            content213.SetFontAndSize(baseF213, 8);
            content213.BeginText();
            content213.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[2].PropertyAttributeName, 49, 282, 0);
            content213.EndText();
            TextField tf2160 = new TextField(writer, new Rectangle(110, 294, 130, 280), "Drapes");
            tf2160.BorderColor = new BaseColor(232, 232, 232);
            tf2160.BackgroundColor = new BaseColor(232, 232, 232);
            tf2160.Options = TextField.READ_ONLY;
            tf2160.Text =chattelslist[2].Count != 0 ? chattelslist[2].Count.ToString() : "";
            tf2160.TextColor = BaseColor.BLACK;
            tf2160.FontSize = 8; ;
            writer.AddAnnotation(tf2160.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 275, 23, 265), chattelslist[3].Checkbox, 23, 265);
            PdfContentByte content214 = writer.DirectContent;
            BaseFont baseF214 = BaseFont.CreateFont();
            content214.SetFontAndSize(baseF214, 8);
            content214.BeginText();
            content214.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[3].PropertyAttributeName, 49, 267, 0);
            content214.EndText();
            TextField tf216 = new TextField(writer, new Rectangle(110, 278, 130, 265), "Fixed");
            tf216.BorderColor = new BaseColor(232, 232, 232);
            tf216.BackgroundColor = new BaseColor(232, 232, 232);
            tf216.Options = TextField.READ_ONLY;
            tf216.Text = chattelslist[3].Count != 0 ? chattelslist[3].Count.ToString() : "";
            tf216.TextColor = BaseColor.BLACK;
            tf216.FontSize = 8;
            writer.AddAnnotation(tf216.GetTextField());


            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 260, 23, 250), chattelslist[4].Checkbox, 23, 250);
            PdfContentByte content215 = writer.DirectContent;
            BaseFont baseF215 = BaseFont.CreateFont();
            content215.SetFontAndSize(baseF215, 8);
            content215.BeginText();
            content215.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[4].PropertyAttributeName, 59, 252, 0);
            content215.EndText();
            TextField tf215 = new TextField(writer, new Rectangle(110, 263, 130, 250), "Dishwasher");
            tf215.BorderColor = new BaseColor(232, 232, 232);
            tf215.BackgroundColor = new BaseColor(232, 232, 232);
            tf215.Options = TextField.READ_ONLY;
            tf215.Text =  chattelslist[4].Count != 0 ? chattelslist[4].Count.ToString() : "";
            tf215.TextColor = BaseColor.BLACK;
            tf215.FontSize = 8;
            writer.AddAnnotation(tf215.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 245, 23, 235), chattelslist[5].Checkbox, 23, 235);
            PdfContentByte content216 = writer.DirectContent;
            BaseFont baseF216 = BaseFont.CreateFont();
            content216.SetFontAndSize(baseF216, 8);
            content216.BeginText();
            content216.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[5].PropertyAttributeName, 71, 237, 0);
            content216.EndText();
            TextField tf2163 = new TextField(writer, new Rectangle(110, 248, 130, 235), "Fixed");
            tf2163.BorderColor = new BaseColor(232, 232, 232);
            tf2163.BackgroundColor = new BaseColor(232, 232, 232);
            tf2163.Options = TextField.READ_ONLY;
            tf2163.Text =  chattelslist[5].Count != 0 ? chattelslist[5].Count.ToString() : "";
            tf2163.TextColor = BaseColor.BLACK;
            tf2163.FontSize = 8;
            writer.AddAnnotation(tf2163.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 230, 23, 220), chattelslist[6].Checkbox, 23, 220);
            PdfContentByte content217 = writer.DirectContent;
            BaseFont baseF217 = BaseFont.CreateFont();
            content217.SetFontAndSize(baseF217, 8);
            content217.BeginText();
            content217.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[6].PropertyAttributeName, 61, 222, 0);
            content217.EndText();
            TextField tf214 = new TextField(writer, new Rectangle(110, 233, 130, 220), "Garden");
            tf214.BorderColor = new BaseColor(232, 232, 232);
            tf214.BackgroundColor = new BaseColor(232, 232, 232);
            tf214.Options = TextField.READ_ONLY;
            tf214.Text =  chattelslist[6].Count != 0 ? chattelslist[6].Count.ToString() : "";
            tf214.TextColor = BaseColor.BLACK;
            tf214.FontSize = 8;
            writer.AddAnnotation(tf214.GetTextField());


            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 215, 23, 205), chattelslist[7].Checkbox, 23, 205);
            PdfContentByte content218 = writer.DirectContent;
            BaseFont baseF218 = BaseFont.CreateFont();
            content218.SetFontAndSize(baseF218, 8);
            content218.BeginText();
            content218.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[7].PropertyAttributeName, 74, 207, 0);
            content218.EndText();
            TextField tf218 = new TextField(writer, new Rectangle(110, 218, 130, 202), "Garage");
            tf218.BorderColor = new BaseColor(232, 232, 232);
            tf218.BackgroundColor = new BaseColor(232, 232, 232);
            tf218.Options = TextField.READ_ONLY;
            tf218.Text =  chattelslist[7].Count != 0 ? chattelslist[7].Count.ToString() : "";
            tf218.TextColor = BaseColor.BLACK;
            tf218.FontSize = 8;
            writer.AddAnnotation(tf218.GetTextField());


            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 200, 23, 190), chattelslist[8].Checkbox, 23, 190);
            PdfContentByte content219 = writer.DirectContent;
            BaseFont baseF219 = BaseFont.CreateFont();
            content219.SetFontAndSize(baseF219, 8);
            content219.BeginText();
            content219.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[8].PropertyAttributeName, 69, 192, 0);
            content219.EndText();
            TextField tf219 = new TextField(writer, new Rectangle(110, 200, 130, 185), "Heated");
            tf219.BorderColor = new BaseColor(232, 232, 232);
            tf219.BackgroundColor = new BaseColor(232, 232, 232);
            tf219.Options = TextField.READ_ONLY;
            tf219.Text =  chattelslist[8].Count != 0 ? chattelslist[8].Count.ToString() : "";
            tf219.TextColor = BaseColor.BLACK;
            tf219.FontSize = 8;
            writer.AddAnnotation(tf219.GetTextField());


            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 185, 23, 175), chattelslist[9].Checkbox, 23, 175);
            PdfContentByte content220 = writer.DirectContent;
            BaseFont baseF220 = BaseFont.CreateFont();
            content220.SetFontAndSize(baseF220, 8);
            content220.BeginText();
            content220.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[9].PropertyAttributeName, 61, 177, 0);
            content220.EndText();
            TextField tf2190 = new TextField(writer, new Rectangle(110, 184, 130, 170), "Heated");
            tf2190.BorderColor = new BaseColor(232, 232, 232);
            tf2190.BackgroundColor = new BaseColor(232, 232, 232);
            tf2190.Options = TextField.READ_ONLY;
            tf2190.Text =  chattelslist[9].Count != 0 ? chattelslist[9].Count.ToString() : "";
            tf2190.TextColor = BaseColor.BLACK;
            tf2190.FontSize = 8;
            writer.AddAnnotation(tf2190.GetTextField());


            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 170, 23, 160), chattelslist[10].Checkbox, 23, 160);
            PdfContentByte content221 = writer.DirectContent;
            BaseFont baseF221 = BaseFont.CreateFont();
            content221.SetFontAndSize(baseF221, 8);
            content221.BeginText();
            content221.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[10].PropertyAttributeName, 59, 162, 0);
            content221.EndText();
            TextField tf221 = new TextField(writer, new Rectangle(110, 168, 130, 157), "Heated");
            tf221.BorderColor = new BaseColor(232, 232, 232);
            tf221.BackgroundColor = new BaseColor(232, 232, 232);
            tf221.Options = TextField.READ_ONLY;
            tf221.Text =chattelslist[10].Count != 0 ? chattelslist[10].Count.ToString() : "";
            tf221.TextColor = BaseColor.BLACK;
            tf221.FontSize = 8;
            writer.AddAnnotation(tf221.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 155, 23, 145), chattelslist[11].Checkbox, 23, 145);
            PdfContentByte content222 = writer.DirectContent;
            BaseFont baseF222 = BaseFont.CreateFont();
            content222.SetFontAndSize(baseF222, 8);
            content222.BeginText();
            content222.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[11].PropertyAttributeName, 52, 147, 0);
            content222.EndText();
            TextField tf222 = new TextField(writer, new Rectangle(110, 155, 130, 144), "Fireplace");
            tf222.BorderColor = new BaseColor(232, 232, 232);
            tf222.BackgroundColor = new BaseColor(232, 232, 232);
            tf222.Options = TextField.READ_ONLY;
            tf222.Text = chattelslist[11].Count != 0 ? chattelslist[11].Count.ToString() : "";
            tf222.TextColor = BaseColor.BLACK;
            writer.AddAnnotation(tf222.GetTextField());


            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 140, 23, 130), chattelslist[12].Checkbox, 23, 130);
            PdfContentByte content223 = writer.DirectContent;
            BaseFont baseF223 = BaseFont.CreateFont();
            content223.SetFontAndSize(baseF223, 8);
            content223.BeginText();
            content223.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[12].PropertyAttributeName, 59, 132, 0);
            content223.EndText();
            TextField tf223 = new TextField(writer, new Rectangle(110, 142, 130, 129), "Rangehood");
            tf223.BorderColor = new BaseColor(232, 232, 232);
            tf223.BackgroundColor = new BaseColor(232, 232, 232);
            tf223.Options = TextField.READ_ONLY;
            tf223.Text = chattelslist[12].Count != 0 ? chattelslist[12].Count.ToString() : "";
            tf223.TextColor = BaseColor.BLACK;
            tf223.FontSize = 8;
            writer.AddAnnotation(tf223.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 125, 23, 115), chattelslist[13].Checkbox, 23, 115);
            PdfContentByte content228 = writer.DirectContent;
            BaseFont baseF228 = BaseFont.CreateFont();
            content228.SetFontAndSize(baseF228, 8);
            content228.BeginText();
            content228.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[13].PropertyAttributeName, 46, 117, 0);
            content228.EndText();
            TextField tf228 = new TextField(writer, new Rectangle(110, 128, 130, 114), "Stove");
            tf228.BorderColor = new BaseColor(232, 232, 232);
            tf228.BackgroundColor = new BaseColor(232, 232, 232);
            tf228.Options = TextField.READ_ONLY;
            tf228.Text = chattelslist[13].Count != 0 ? chattelslist[13].Count.ToString() : "";
            tf228.TextColor = BaseColor.BLACK;
            tf228.FontSize = 8;
            writer.AddAnnotation(tf228.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 110, 23, 100), chattelslist[14].Checkbox, 23, 100);
            PdfContentByte content224 = writer.DirectContent;
            BaseFont baseF224 = BaseFont.CreateFont();
            content224.SetFontAndSize(baseF224, 8);
            content224.BeginText();
            content224.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[14].PropertyAttributeName, 51, 102, 0);
            content224.EndText();
            TextField tf224 = new TextField(writer, new Rectangle(110, 113, 130, 100), "TV");
            tf224.BorderColor = new BaseColor(232, 232, 232);
            tf224.BackgroundColor = new BaseColor(232, 232, 232);
            tf224.Options = TextField.READ_ONLY;
            tf224.Text = chattelslist[14].Count != 0 ? chattelslist[14].Count.ToString() : "";
            tf224.TextColor = BaseColor.BLACK;
            tf224.FontSize = 8;
            writer.AddAnnotation(tf224.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 95, 23, 85), chattelslist[15].Checkbox, 23, 85);
            PdfContentByte content225 = writer.DirectContent;
            BaseFont baseF225 = BaseFont.CreateFont();
            content225.SetFontAndSize(baseF225, 8);
            content225.BeginText();
            content225.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[15].PropertyAttributeName, 67, 87, 0);
            content225.EndText();
            TextField tf225 = new TextField(writer, new Rectangle(110, 98, 130, 84), "Waste");
            tf225.BorderColor = new BaseColor(232, 232, 232);
            tf225.BackgroundColor = new BaseColor(232, 232, 232);
            tf225.Options = TextField.READ_ONLY;
            tf225.Text = chattelslist[15].Count != 0 ? chattelslist[15].Count.ToString() : "";
            tf225.TextColor = BaseColor.BLACK;
            tf225.FontSize = 8;
            writer.AddAnnotation(tf225.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 80, 23, 70), chattelslist[16].Checkbox, 23, 70);
            PdfContentByte content226 = writer.DirectContent;
            BaseFont baseF226 = BaseFont.CreateFont();
            content226.SetFontAndSize(baseF226, 8);
            content226.BeginText();
            content226.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[16].PropertyAttributeName, 49, 72, 0);
            content226.EndText();
            TextField tf2270 = new TextField(writer, new Rectangle(110, 82, 130, 69), "Central");
            tf2270.BorderColor = new BaseColor(232, 232, 232);
            tf2270.BackgroundColor = new BaseColor(232, 232, 232);
            tf2270.Options = TextField.READ_ONLY;
            tf2270.Text = chattelslist[16].Count != 0 ? chattelslist[16].Count.ToString() : "";
            tf2270.TextColor = BaseColor.BLACK;
            tf2270.FontSize = 8;
            writer.AddAnnotation(tf2270.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 65, 23, 55), chattelslist[17].Checkbox, 23, 55);
            PdfContentByte content227 = writer.DirectContent;
            BaseFont baseF227 = BaseFont.CreateFont();
            content227.SetFontAndSize(baseF227, 8);
            content227.BeginText();
            content227.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[17].PropertyAttributeName, 68, 57, 0);
            content227.EndText();
            TextField tf227 = new TextField(writer, new Rectangle(110, 68, 130, 55), "Central");
            tf227.BorderColor = new BaseColor(232, 232, 232);
            tf227.BackgroundColor = new BaseColor(232, 232, 232);
            tf227.Options = TextField.READ_ONLY;
            tf227.Text = chattelslist[17].Count != 0 ? chattelslist[17].Count.ToString() : "";
            tf227.TextColor = BaseColor.BLACK;
            tf227.FontSize = 8;
            writer.AddAnnotation(tf227.GetTextField());


            //CODE FOR INSULATION CHECKBOXES


            var insulationlist = data.Where(w => w.Name == "Insulation").ToList();
            // Column 2

            PdfPTable col2Table = new PdfPTable(1);

            PdfPCell cell211 = new PdfPCell(new Phrase());
            cell211.FixedHeight = 98f; // Set the height of the cell
            cell211.BorderColor = BaseColor.BLACK;
            cell211.CellEvent = new RectangleOverPdfPCellBorder("Insulation", 165f, 344f, 50f, 20f, 168f, 358f);
            //cell211.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Insulation", 175f, 20f, 70f, 345f);
            col2Table.AddCell(cell211);

            //AddCheckboxField(document, writer, "myCheckbox", new Rectangle(180, 280, 180, 320), true, 40, 310);
            //AddCheckboxField(document, writer, "myCheckbox", new Rectangle(150, 490, 190, 320), true, 150, 465);
            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 330, 165, 340), insulationlist[0].Checkbox, 155, 330);
            PdfContentByte content229 = writer.DirectContent;
            BaseFont baseF229 = BaseFont.CreateFont();
            content229.SetFontAndSize(baseF229, 8);
            content229.BeginText();
            content229.ShowTextAligned(PdfContentByte.ALIGN_CENTER, insulationlist[0].PropertyAttributeName, 180, 330, 0);
            content229.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 315, 165, 325), insulationlist[1].Checkbox, 155, 315);
            PdfContentByte content230 = writer.DirectContent;
            BaseFont baseF230 = BaseFont.CreateFont();
            content230.SetFontAndSize(baseF230, 8);
            content230.BeginText();
            content230.ShowTextAligned(PdfContentByte.ALIGN_CENTER, insulationlist[1].PropertyAttributeName, 180, 315, 0);
            content230.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 300, 165, 310), insulationlist[2].Checkbox, 155, 300);
            PdfContentByte content231 = writer.DirectContent;
            BaseFont baseF231 = BaseFont.CreateFont();
            content231.SetFontAndSize(baseF231, 8);
            content231.BeginText();
            content231.ShowTextAligned(PdfContentByte.ALIGN_CENTER, insulationlist[2].PropertyAttributeName, 180, 300, 0);
            content231.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 285, 165, 295), insulationlist[3].Checkbox, 155, 285);
            PdfContentByte content232 = writer.DirectContent;
            BaseFont baseF232 = BaseFont.CreateFont();
            content232.SetFontAndSize(baseF232, 8);
            content232.BeginText();
            content232.ShowTextAligned(PdfContentByte.ALIGN_CENTER, insulationlist[3].PropertyAttributeName, 180, 285, 0);
            content232.EndText();

            TextField tf231 = new TextField(writer, new Rectangle(156, 265, 250, 280), "Other");
            tf231.BorderColor = new BaseColor(232, 232, 232);
            tf231.BackgroundColor = new BaseColor(232, 232, 232);
            tf231.Options = TextField.READ_ONLY;
            tf231.Text = insulationlist[3].Remarks;
            tf231.TextColor = BaseColor.BLACK;
            tf231.FontSize = 8;
            writer.AddAnnotation(tf231.GetTextField());



            PdfPCell blankCell16 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell16.Border = PdfPCell.NO_BORDER;
            blankCell16.FixedHeight = 10f;
            col2Table.AddCell(blankCell16);


            // CODE FOR HEATING CHECKBOXES

            var heatinglist = data.Where(w => w.Name == "Heating").ToList();

            PdfPCell cell212 = new PdfPCell(new Phrase());
            cell212.FixedHeight = 180f; // Set the height of the cell
            cell212.BorderColor = BaseColor.DARK_GRAY;
            cell212.CellEvent = new RectangleOverPdfPCellBorder("Heating", 165f, 246f, 40f, 15f, 168f, 246f);
            //cell212.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Heating", 170f, 15f, 70f, 240f);
            col2Table.AddCell(cell212);
            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 225, 165, 235), heatinglist[0].Checkbox, 155, 225);
            PdfContentByte content233 = writer.DirectContent;
            BaseFont baseF233 = BaseFont.CreateFont();
            content233.SetFontAndSize(baseF233, 8);
            content233.BeginText();
            content233.ShowTextAligned(PdfContentByte.ALIGN_CENTER, heatinglist[0].PropertyAttributeName, 185, 225, 0);
            content233.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 210, 165, 220), heatinglist[1].Checkbox, 155, 210);
            PdfContentByte content234 = writer.DirectContent;
            BaseFont baseF234 = BaseFont.CreateFont();
            content234.SetFontAndSize(baseF234, 8);
            content234.BeginText();
            content234.ShowTextAligned(PdfContentByte.ALIGN_CENTER, heatinglist[1].PropertyAttributeName, 195, 210, 0);
            content234.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 195, 165, 205), heatinglist[2].Checkbox, 155, 195);
            PdfContentByte content235 = writer.DirectContent;
            BaseFont baseF235 = BaseFont.CreateFont();
            content235.SetFontAndSize(baseF235, 8);
            content235.BeginText();
            content235.ShowTextAligned(PdfContentByte.ALIGN_CENTER, heatinglist[2].PropertyAttributeName, 205, 195, 0);
            content235.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 180, 165, 190), heatinglist[3].Checkbox, 155, 180);
            PdfContentByte content236 = writer.DirectContent;
            BaseFont baseF236 = BaseFont.CreateFont();
            content236.SetFontAndSize(baseF236, 8);
            content236.BeginText();
            content236.ShowTextAligned(PdfContentByte.ALIGN_CENTER, heatinglist[3].PropertyAttributeName, 185, 180, 0);
            content236.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 165, 165, 175), heatinglist[4].Checkbox, 155, 165);
            PdfContentByte content237 = writer.DirectContent;
            BaseFont baseF237 = BaseFont.CreateFont();
            content237.SetFontAndSize(baseF237, 8);
            content237.BeginText();
            content237.ShowTextAligned(PdfContentByte.ALIGN_CENTER, heatinglist[4].PropertyAttributeName, 200, 165, 0);
            content237.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 150, 165, 160), heatinglist[5].Checkbox, 155, 150);
            PdfContentByte content238 = writer.DirectContent;
            BaseFont baseF238 = BaseFont.CreateFont();
            content238.SetFontAndSize(baseF238, 8);
            content238.BeginText();
            content238.ShowTextAligned(PdfContentByte.ALIGN_CENTER, heatinglist[5].PropertyAttributeName, 190, 150, 0);
            content238.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 135, 165, 145), heatinglist[6].Checkbox, 155, 135);
            PdfContentByte content239 = writer.DirectContent;
            BaseFont baseF239 = BaseFont.CreateFont();
            content239.SetFontAndSize(baseF239, 8);
            content239.BeginText();
            content239.ShowTextAligned(PdfContentByte.ALIGN_CENTER, heatinglist[6].PropertyAttributeName, 190, 135, 0);
            content239.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 120, 165, 130), heatinglist[7].Checkbox, 155, 120);
            PdfContentByte content240 = writer.DirectContent;
            BaseFont baseF240 = BaseFont.CreateFont();
            content240.SetFontAndSize(baseF240, 8);
            content240.BeginText();
            content240.ShowTextAligned(PdfContentByte.ALIGN_CENTER, heatinglist[7].PropertyAttributeName, 180, 120, 0);
            content240.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 105, 165, 115), heatinglist[8].Checkbox, 155, 105);
            PdfContentByte content241 = writer.DirectContent;
            BaseFont baseF241 = BaseFont.CreateFont();
            content241.SetFontAndSize(baseF241, 8);
            content241.BeginText();
            content241.ShowTextAligned(PdfContentByte.ALIGN_CENTER, heatinglist[8].PropertyAttributeName, 185, 105, 0);
            content241.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 90, 165, 100), heatinglist[9].Checkbox, 155, 90);
            PdfContentByte content242 = writer.DirectContent;
            BaseFont baseF242 = BaseFont.CreateFont();
            content242.SetFontAndSize(baseF242, 8);
            content242.BeginText();
            content242.ShowTextAligned(PdfContentByte.ALIGN_CENTER, heatinglist[9].PropertyAttributeName, 190, 90, 0);
            content242.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 75, 165, 85), heatinglist[10].Checkbox, 155, 75);
            PdfContentByte content243 = writer.DirectContent;
            BaseFont baseF243 = BaseFont.CreateFont();
            content243.SetFontAndSize(baseF243, 8);
            content243.BeginText();
            content243.ShowTextAligned(PdfContentByte.ALIGN_CENTER, heatinglist[10].PropertyAttributeName, 180, 75, 0);
            content243.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(155, 60, 165, 70), heatinglist[11].Checkbox, 155, 60);
            PdfContentByte content295 = writer.DirectContent;
            BaseFont baseF295 = BaseFont.CreateFont();
            content295.SetFontAndSize(baseF295, 8);
            content295.BeginText();
            content295.ShowTextAligned(PdfContentByte.ALIGN_CENTER, heatinglist[11].PropertyAttributeName, 190, 60, 0);
            content295.EndText();

            multipleMainTable.AddCell(col2Table);

            //CODE FOR INTERIOR CONDITION CHECKBOXES

            var interiorConditionlist = data.Where(w => w.Name == "Interior Condition").ToList();

            // Column 3
            PdfPTable col3Table = new PdfPTable(1);
            PdfPCell cell31 = new PdfPCell(new Phrase());
            cell31.FixedHeight = 80f; // Set the height of the cell
            cell31.BorderColor = BaseColor.BLACK;
            cell31.CellEvent = new RectangleOverPdfPCellBorder("Interior condition", 267f, 344f, 85f, 20f, 269f, 355f);
            //cell31.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Interior condition", 270f, 20f, 70f, 345f);
            col3Table.AddCell(cell31);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(262, 330, 272, 340), interiorConditionlist[0].Checkbox, 262, 330);
            PdfContentByte content245 = writer.DirectContent;
            BaseFont baseF245 = BaseFont.CreateFont();
            content245.SetFontAndSize(baseF245, 8);
            content245.BeginText();
            content245.ShowTextAligned(PdfContentByte.ALIGN_CENTER, interiorConditionlist[0].PropertyAttributeName, 292, 330, 0);
            content245.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(262, 315, 272, 325), interiorConditionlist[1].Checkbox, 262, 315);
            PdfContentByte content246 = writer.DirectContent;
            BaseFont baseF246 = BaseFont.CreateFont();
            content246.SetFontAndSize(baseF246, 8);
            content246.BeginText();
            content246.ShowTextAligned(PdfContentByte.ALIGN_CENTER, interiorConditionlist[1].PropertyAttributeName, 292, 315, 0);
            content246.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(262, 300, 272, 310), interiorConditionlist[2].Checkbox, 262, 300);
            PdfContentByte content247 = writer.DirectContent;
            BaseFont baseF247 = BaseFont.CreateFont();
            content247.SetFontAndSize(baseF247, 8);
            content247.BeginText();
            content247.ShowTextAligned(PdfContentByte.ALIGN_CENTER, interiorConditionlist[2].PropertyAttributeName, 286, 300, 0);
            content247.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(262, 285, 272, 295), interiorConditionlist[3].Checkbox, 262, 285);
            PdfContentByte content248 = writer.DirectContent;
            BaseFont baseF248 = BaseFont.CreateFont();
            content248.SetFontAndSize(baseF248, 8);
            content248.BeginText();
            content248.ShowTextAligned(PdfContentByte.ALIGN_CENTER, interiorConditionlist[3].PropertyAttributeName, 286, 285, 0);
            content248.EndText();


            PdfPCell blankCell17 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell17.Border = PdfPCell.NO_BORDER;
            blankCell17.FixedHeight = 10f;
            col3Table.AddCell(blankCell17);

            //CODE FOR EXTERIOR CHECKBOXES

            var exteriorList = data.Where(w => w.Name == "Exterior").ToList();

            PdfPCell cell32 = new PdfPCell(new Phrase());
            cell32.FixedHeight = 55f; // Set the height of the cell
            cell32.BorderColor = BaseColor.DARK_GRAY;
            cell32.CellEvent = new RectangleOverPdfPCellBorder("Exterior", 270f, 265f, 45f, 14f, 272f, 265f);
            //cell32.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Exterior", 270f, 20f, 70f, 255f);
            col3Table.AddCell(cell32);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(262, 240, 272, 230), exteriorList[0].Checkbox, 262, 230);
            PdfContentByte content249 = writer.DirectContent;
            BaseFont baseF249 = BaseFont.CreateFont();
            content249.SetFontAndSize(baseF249, 8);
            content249.BeginText();
            content249.ShowTextAligned(PdfContentByte.ALIGN_CENTER, exteriorList[0].PropertyAttributeName, 309, 230, 0);
            content249.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(262, 225, 272, 215), exteriorList[1].Checkbox, 262, 215);
            PdfContentByte content250 = writer.DirectContent;
            BaseFont baseF250 = BaseFont.CreateFont();
            content250.SetFontAndSize(baseF250, 8);
            content250.BeginText();
            content250.ShowTextAligned(PdfContentByte.ALIGN_CENTER, exteriorList[1].PropertyAttributeName, 317, 215, 0);
            content250.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(262, 210, 272, 200), exteriorList[2].Checkbox, 262, 200);
            PdfContentByte content251 = writer.DirectContent;
            BaseFont baseF251 = BaseFont.CreateFont();
            content251.SetFontAndSize(baseF251, 8);
            content251.BeginText();
            content251.ShowTextAligned(PdfContentByte.ALIGN_CENTER, exteriorList[2].PropertyAttributeName, 312, 200, 0);
            content251.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(262, 195, 272, 185), exteriorList[3].Checkbox, 262, 185);
            PdfContentByte content252 = writer.DirectContent;
            BaseFont baseF252 = BaseFont.CreateFont();
            content252.SetFontAndSize(baseF252, 8);
            content252.BeginText();
            content252.ShowTextAligned(PdfContentByte.ALIGN_CENTER, exteriorList[3].PropertyAttributeName, 304, 185, 0);
            content252.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(262, 180, 272, 170), exteriorList[4].Checkbox, 262, 170);
            PdfContentByte content253 = writer.DirectContent;
            BaseFont baseF253 = BaseFont.CreateFont();
            content253.SetFontAndSize(baseF253, 8);
            content253.BeginText();
            content253.ShowTextAligned(PdfContentByte.ALIGN_CENTER, exteriorList[4].PropertyAttributeName, 314, 170, 0);
            content253.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(262, 165, 272, 155), exteriorList[5].Checkbox, 262, 155);
            PdfContentByte content254 = writer.DirectContent;
            BaseFont baseF254 = BaseFont.CreateFont();
            content254.SetFontAndSize(baseF254, 8);
            content254.BeginText();
            content254.ShowTextAligned(PdfContentByte.ALIGN_CENTER, exteriorList[5].PropertyAttributeName, 302, 155, 0);
            content254.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(262, 150, 272, 140), exteriorList[6].Checkbox, 262, 140);
            PdfContentByte content255 = writer.DirectContent;
            BaseFont baseF255 = BaseFont.CreateFont();
            content255.SetFontAndSize(baseF255, 8);
            content255.BeginText();
            content255.ShowTextAligned(PdfContentByte.ALIGN_CENTER, exteriorList[6].PropertyAttributeName, 299, 140, 0);
            content255.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(262, 135, 272, 125), exteriorList[7].Checkbox, 262, 125);
            PdfContentByte content256 = writer.DirectContent;
            BaseFont baseF256 = BaseFont.CreateFont();
            content256.SetFontAndSize(baseF256, 8);
            content256.BeginText();
            content256.ShowTextAligned(PdfContentByte.ALIGN_CENTER, exteriorList[7].PropertyAttributeName, 290, 125, 0);
            content256.EndText();

            PdfContentByte cb221 = writer.DirectContent;
            BaseFont bf221 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
            string text221 = "Other comments";
            cb221.BeginText();
            cb221.SetFontAndSize(bf221, 8);
            // 2ut the alignment and coordinates here
            cb221.ShowTextAligned(1, text221, 295, 110, 0);
            cb221.EndText();
            TextField tf321 = new TextField(writer, new Rectangle(262, 105, 350, 90), "Central");
            tf321.BorderColor = new BaseColor(232, 232, 232);
            tf321.BackgroundColor = new BaseColor(232, 232, 232);
            tf321.Options = TextField.READ_ONLY;
            tf321.Text = exteriorList[7].Remarks;
            tf321.TextColor = BaseColor.BLACK;
            tf321.FontSize = 8;
            writer.AddAnnotation(tf321.GetTextField());

            multipleMainTable.AddCell(col3Table);



            //CODE FOR FLOORING CHECKBOXES

            var flooringlist = data.Where(w => w.Name == "Floring").ToList();
            // Column 4
            PdfPTable col4Table = new PdfPTable(1);
            PdfPCell cell41 = new PdfPCell(new Phrase());
            cell41.FixedHeight = 125f; // Set the height of the cell
            cell41.BorderColor = BaseColor.BLACK;
            cell41.CellEvent = new RectangleOverPdfPCellBorder("Flooring", 370f, 343f, 44f, 20f, 372f, 358f);
            //cell41.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Flooring", 370f, 20f, 70f, 345f);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 330, 380, 340), flooringlist[0].Checkbox, 370, 330);
            PdfContentByte content257 = writer.DirectContent;
            BaseFont baseF257 = BaseFont.CreateFont();
            content257.SetFontAndSize(baseF257, 8);
            content257.BeginText();
            content257.ShowTextAligned(PdfContentByte.ALIGN_CENTER, flooringlist[0].PropertyAttributeName, 393, 330, 0);
            content257.EndText();


            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 315, 380, 325), flooringlist[1].Checkbox, 370, 315);
            PdfContentByte content258 = writer.DirectContent;
            BaseFont baseF258 = BaseFont.CreateFont();
            content258.SetFontAndSize(baseF258, 8);
            content258.BeginText();
            content258.ShowTextAligned(PdfContentByte.ALIGN_CENTER, flooringlist[1].PropertyAttributeName, 396, 315, 0);
            content258.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 300, 380, 310), flooringlist[2].Checkbox, 370, 300);
            PdfContentByte content259 = writer.DirectContent;
            BaseFont baseF259 = BaseFont.CreateFont();
            content259.SetFontAndSize(baseF259, 8);
            content259.BeginText();
            content259.ShowTextAligned(PdfContentByte.ALIGN_CENTER, flooringlist[2].PropertyAttributeName, 390, 300, 0);
            content259.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 285, 380, 295), flooringlist[3].Checkbox, 370, 285);
            PdfContentByte content260 = writer.DirectContent;
            BaseFont baseF260 = BaseFont.CreateFont();
            content260.SetFontAndSize(baseF260, 8);
            content260.BeginText();
            content260.ShowTextAligned(PdfContentByte.ALIGN_CENTER, flooringlist[3].PropertyAttributeName, 393, 285, 0);
            content260.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 270, 380, 280), flooringlist[4].Checkbox, 370, 270);
            PdfContentByte content261 = writer.DirectContent;
            BaseFont baseF261 = BaseFont.CreateFont();
            content261.SetFontAndSize(baseF261, 8);
            content261.BeginText();
            content261.ShowTextAligned(PdfContentByte.ALIGN_CENTER, flooringlist[4].PropertyAttributeName, 390, 270, 0);
            content261.EndText();


            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 255, 380, 265), flooringlist[5].Checkbox, 370, 255);
            PdfContentByte content262 = writer.DirectContent;
            BaseFont baseF262 = BaseFont.CreateFont();
            content262.SetFontAndSize(baseF262, 8);
            content262.BeginText();
            content262.ShowTextAligned(PdfContentByte.ALIGN_CENTER, flooringlist[5].PropertyAttributeName, 393, 255, 0);
            content262.EndText();

            TextField tf232 = new TextField(writer, new Rectangle(370, 250, 450, 240), "Other");
            tf232.BorderColor = new BaseColor(232, 232, 232);
            tf232.BackgroundColor = new BaseColor(232, 232, 232);
            tf232.Options = TextField.READ_ONLY;
            tf232.Text = flooringlist[5].Remarks;
            tf232.TextColor = BaseColor.BLACK;
            tf232.FontSize = 8;
            writer.AddAnnotation(tf232.GetTextField());




            //CODE FOR GARAGE CHECKBOXES

            var garagelist = data.Where(w => w.Name == "Garage").ToList();

            PdfPCell cell42 = new PdfPCell(new Phrase());
            cell42.FixedHeight = 60f; // Set the height of the cell
            cell42.BorderColor = BaseColor.DARK_GRAY;
            cell42.CellEvent = new RectangleOverPdfPCellBorder("Garage", 370f, 220f, 38f, 14f, 372f, 220f);
            //cell42.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Garage", 370f, 15f, 70f, 218f);
            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 200, 380, 190), garagelist[0].Checkbox, 370, 190);
            PdfContentByte content263 = writer.DirectContent;
            BaseFont baseF263 = BaseFont.CreateFont();
            content263.SetFontAndSize(baseF263, 8);
            content263.BeginText();
            content263.ShowTextAligned(PdfContentByte.ALIGN_CENTER, garagelist[0].PropertyAttributeName, 397, 190, 0);
            content263.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 185, 380, 175), garagelist[1].Checkbox, 370, 175);
            PdfContentByte content264 = writer.DirectContent;
            BaseFont baseF264 = BaseFont.CreateFont();
            content264.SetFontAndSize(baseF264, 8);
            content264.BeginText();
            content264.ShowTextAligned(PdfContentByte.ALIGN_CENTER, garagelist[1].PropertyAttributeName, 400, 175, 0);
            content264.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 170, 380, 160), garagelist[2].Checkbox, 370, 160);
            PdfContentByte content265 = writer.DirectContent;
            BaseFont baseF265 = BaseFont.CreateFont();
            content265.SetFontAndSize(baseF265, 8);
            content265.BeginText();
            content265.ShowTextAligned(PdfContentByte.ALIGN_CENTER, garagelist[2].PropertyAttributeName, 400, 160, 0);
            content265.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 155, 380, 145), garagelist[3].Checkbox, 370, 145);
            PdfContentByte content266 = writer.DirectContent;
            BaseFont baseF266 = BaseFont.CreateFont();
            content266.SetFontAndSize(baseF266, 8);
            content266.BeginText();
            content266.ShowTextAligned(PdfContentByte.ALIGN_CENTER, garagelist[3].PropertyAttributeName, 413, 145, 0);
            content266.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 140, 380, 130), garagelist[4].Checkbox, 370, 130);
            PdfContentByte content267 = writer.DirectContent;
            BaseFont baseF267 = BaseFont.CreateFont();
            content267.SetFontAndSize(baseF267, 8);
            content267.BeginText();
            content267.ShowTextAligned(PdfContentByte.ALIGN_CENTER, garagelist[4].PropertyAttributeName, 415, 130, 0);
            content267.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 125, 380, 115), garagelist[5].Checkbox, 370, 115);
            PdfContentByte content268 = writer.DirectContent;
            BaseFont baseF268 = BaseFont.CreateFont();
            content268.SetFontAndSize(baseF268, 8);
            content268.BeginText();
            content268.ShowTextAligned(PdfContentByte.ALIGN_CENTER, garagelist[5].PropertyAttributeName, 413, 115, 0);
            content268.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 110, 380, 100), garagelist[6].Checkbox, 370, 100);
            PdfContentByte content269 = writer.DirectContent;
            BaseFont baseF269 = BaseFont.CreateFont();
            content269.SetFontAndSize(baseF269, 8);
            content269.BeginText();
            content269.ShowTextAligned(PdfContentByte.ALIGN_CENTER, garagelist[6].PropertyAttributeName, 405, 100, 0);
            content269.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 95, 380, 85), garagelist[7].Checkbox, 370, 85);
            PdfContentByte content270 = writer.DirectContent;
            BaseFont baseF270 = BaseFont.CreateFont();
            content270.SetFontAndSize(baseF270, 8);
            content270.BeginText();
            content270.ShowTextAligned(PdfContentByte.ALIGN_CENTER, garagelist[7].PropertyAttributeName, 400, 85, 0);
            content270.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 80, 380, 70), garagelist[8].Checkbox, 370, 70);
            PdfContentByte content271 = writer.DirectContent;
            BaseFont baseF271 = BaseFont.CreateFont();
            content271.SetFontAndSize(baseF271, 8);
            content271.BeginText();
            content271.ShowTextAligned(PdfContentByte.ALIGN_CENTER, garagelist[8].PropertyAttributeName, 422, 70, 0);
            content271.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(370, 65, 380, 55), garagelist[9].Checkbox, 370, 55);
            PdfContentByte content272 = writer.DirectContent;
            BaseFont baseF272 = BaseFont.CreateFont();
            content272.SetFontAndSize(baseF272, 8);
            content272.BeginText();
            content272.ShowTextAligned(PdfContentByte.ALIGN_CENTER, garagelist[9].PropertyAttributeName, 410, 55, 0);
            content272.EndText();

            col4Table.AddCell(cell41);

            PdfPCell blankCell18 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell18.Border = PdfPCell.NO_BORDER;
            blankCell18.FixedHeight = 10f;
            col4Table.AddCell(blankCell18);

            col4Table.AddCell(cell42);
            multipleMainTable.AddCell(col4Table);




            //CODE FOR WATER CHECKBOXES

            var waterlist = data.Where(w => w.Name == "Water").ToList();
            // Column 5
            PdfPTable col5Table = new PdfPTable(1);
            PdfPCell cell51 = new PdfPCell(new Phrase());
            cell51.FixedHeight = 93f; // Set the height of the cell
            cell51.CellEvent = new RectangleOverPdfPCellBorder("Water", 485f, 344f, 35f, 20f, 487f, 355f);
            //cell51.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Water", 485f, 20f, 70f, 345f);
            cell51.BorderColor = BaseColor.DARK_GRAY;
            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(480, 335, 490, 345), waterlist[0].Checkbox, 480, 335);
            PdfContentByte content273 = writer.DirectContent;
            BaseFont baseF273 = BaseFont.CreateFont();
            content273.SetFontAndSize(baseF273, 8);
            content273.BeginText();
            content273.ShowTextAligned(PdfContentByte.ALIGN_CENTER, waterlist[0].PropertyAttributeName, 502, 335, 0);
            content273.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(480, 320, 490, 330), waterlist[1].Checkbox, 480, 320);
            PdfContentByte content274 = writer.DirectContent;
            BaseFont baseF274 = BaseFont.CreateFont();
            content274.SetFontAndSize(baseF274, 8);
            content274.BeginText();
            content274.ShowTextAligned(PdfContentByte.ALIGN_CENTER, waterlist[1].PropertyAttributeName, 502, 320, 0);
            content274.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(480, 305, 490, 315), waterlist[2].Checkbox, 480, 305);
            PdfContentByte content275 = writer.DirectContent;
            BaseFont baseF275 = BaseFont.CreateFont();
            content275.SetFontAndSize(baseF275, 8);
            content275.BeginText();
            content275.ShowTextAligned(PdfContentByte.ALIGN_CENTER, waterlist[2].PropertyAttributeName, 502, 305, 0);
            content275.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(480, 290, 490, 300), waterlist[3].Checkbox, 480, 290);
            PdfContentByte content276 = writer.DirectContent;
            BaseFont baseF276 = BaseFont.CreateFont();
            content276.SetFontAndSize(baseF276, 8);
            content276.BeginText();
            content276.ShowTextAligned(PdfContentByte.ALIGN_CENTER, waterlist[3].PropertyAttributeName, 502, 290, 0);
            content276.EndText();


            TextField tf233 = new TextField(writer, new Rectangle(480, 270, 550, 285), "Central");
            tf233.BorderColor = new BaseColor(232, 232, 232);
            tf233.BackgroundColor = new BaseColor(232, 232, 232);
            tf233.Options = TextField.READ_ONLY;
            tf233.Text = waterlist[3].Remarks;
            tf233.TextColor = BaseColor.BLACK;
            tf233.FontSize = 8;
            writer.AddAnnotation(tf233.GetTextField());
            col5Table.AddCell(cell51);


            PdfPCell blankCell19 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell19.Border = PdfPCell.NO_BORDER;
            blankCell19.FixedHeight = 10f;
            col5Table.AddCell(blankCell19);





            //CODE FOR FRONTAGE CHECKBOXES

            var frontageList = data.Where(w => w.Name == "Frontage").ToList();

            PdfPCell cell52 = new PdfPCell(new Phrase());
            cell52.FixedHeight = 50f; // Set the height of the cell
            cell52.BorderColor = BaseColor.DARK_GRAY;
            cell52.CellEvent = new RectangleOverPdfPCellBorder("Frontage", 485f, 250f, 48f, 15f, 487f, 250f);
            //cell52.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Frontage", 485f, 15f, 70f, 245f);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(480, 235, 490, 245), frontageList[0].Checkbox, 480, 235);
            PdfContentByte content277 = writer.DirectContent;
            BaseFont baseF277 = BaseFont.CreateFont();
            content277.SetFontAndSize(baseF277, 8);
            content277.BeginText();
            content277.ShowTextAligned(PdfContentByte.ALIGN_CENTER, frontageList[0].PropertyAttributeName, 502, 235, 0);
            content277.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(480, 220, 490, 230), frontageList[1].Checkbox, 480, 220);
            PdfContentByte content278 = writer.DirectContent;
            BaseFont baseF278 = BaseFont.CreateFont();
            content278.SetFontAndSize(baseF278, 8);
            content278.BeginText();
            content278.ShowTextAligned(PdfContentByte.ALIGN_CENTER, frontageList[1].PropertyAttributeName, 502, 220, 0);
            content278.EndText();

            col5Table.AddCell(cell52);


            PdfPCell blankCell20 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell20.Border = PdfPCell.NO_BORDER;
            blankCell20.FixedHeight = 10f;
            col5Table.AddCell(blankCell20);



            //CODE FOR LEVELS CHECKBOXES

            var levelsList = data.Where(w => w.Name == "Level").ToList();

            PdfPCell cell53 = new PdfPCell(new Phrase());
            cell53.FixedHeight = 55f; // Set the height of the cell
            cell53.BorderColor = BaseColor.DARK_GRAY;
            cell53.CellEvent = new RectangleOverPdfPCellBorder("Levels", 485f, 190f, 40f, 15f, 485f, 193f);
            //cell53.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Levels", 485f, 15f, 70f, 190f);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(480, 175, 490, 185), levelsList[0].Checkbox, 480, 175);
            PdfContentByte content279 = writer.DirectContent;
            BaseFont baseF279 = BaseFont.CreateFont();
            content279.SetFontAndSize(baseF279, 8);
            content279.BeginText();
            content279.ShowTextAligned(PdfContentByte.ALIGN_CENTER, levelsList[0].PropertyAttributeName, 518, 175, 0);
            content279.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(480, 160, 490, 170), levelsList[1].Checkbox, 480, 160);
            PdfContentByte content280 = writer.DirectContent;
            BaseFont baseF280 = BaseFont.CreateFont();
            content280.SetFontAndSize(baseF280, 8);
            content280.BeginText();
            content280.ShowTextAligned(PdfContentByte.ALIGN_CENTER, levelsList[1].PropertyAttributeName, 518, 160, 0);
            content280.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(480, 145, 490, 155), levelsList[2].Checkbox, 480, 145);
            PdfContentByte content281 = writer.DirectContent;
            BaseFont baseF281 = BaseFont.CreateFont();
            content281.SetFontAndSize(baseF281, 8);
            content281.BeginText();
            content281.ShowTextAligned(PdfContentByte.ALIGN_CENTER, levelsList[2].PropertyAttributeName, 518, 145, 0);
            content281.EndText();


            //CODE FOR AMENITIES CHECKBOXES

            var amenitiesList = data.Where(w => w.Name == "Amenities").ToList();

            PdfPCell cell54 = new PdfPCell(new Phrase());
            cell54.FixedHeight = 30f; // Set the height of the cell
            cell54.BorderColor = BaseColor.BLACK;
            cell54.CellEvent = new RectangleOverPdfPCellBorder("Amenities", 485f, 120f, 40f, 20f, 485f, 120f);
            //cell54.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Amenities", 485f, 15f, 70f, 120f);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(480, 100, 490, 110), amenitiesList[0].Checkbox, 480, 100);
            PdfContentByte content282 = writer.DirectContent;
            BaseFont baseF282 = BaseFont.CreateFont();
            content282.SetFontAndSize(baseF282, 8);
            content282.BeginText();
            content282.ShowTextAligned(PdfContentByte.ALIGN_CENTER, amenitiesList[0].PropertyAttributeName, 518, 100, 0);
            content282.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(480, 85, 490, 95), amenitiesList[1].Checkbox, 480, 85);
            PdfContentByte content283 = writer.DirectContent;
            BaseFont baseF283 = BaseFont.CreateFont();
            content283.SetFontAndSize(baseF283, 8);
            content283.BeginText();
            content283.ShowTextAligned(PdfContentByte.ALIGN_CENTER, amenitiesList[1].PropertyAttributeName, 522, 85, 0);
            content283.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(480, 70, 490, 80), amenitiesList[2].Checkbox, 480, 70);
            PdfContentByte content284 = writer.DirectContent;
            BaseFont baseF284 = BaseFont.CreateFont();
            content284.SetFontAndSize(baseF284, 8);
            content284.BeginText();
            content284.ShowTextAligned(PdfContentByte.ALIGN_CENTER, amenitiesList[2].PropertyAttributeName, 520, 70, 0);
            content284.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(480, 55, 490, 65), amenitiesList[3].Checkbox, 480, 55);
            PdfContentByte content285 = writer.DirectContent;
            BaseFont baseF285 = BaseFont.CreateFont();
            content285.SetFontAndSize(baseF283, 8);
            content285.BeginText();
            content285.ShowTextAligned(PdfContentByte.ALIGN_CENTER, amenitiesList[3].PropertyAttributeName, 521, 55, 0);
            content285.EndText();

            col5Table.AddCell(cell53);

            PdfPCell blankCell21 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell21.Border = PdfPCell.NO_BORDER;
            blankCell21.FixedHeight = 10f;
            col5Table.AddCell(blankCell21);

            col5Table.AddCell(cell54);
            multipleMainTable.AddCell(col5Table);
            col1Table.DefaultCell.Border = 0;
            col2Table.DefaultCell.Border = 0;
            col3Table.DefaultCell.Border = 0;
            col4Table.DefaultCell.Border = 0;
            col5Table.DefaultCell.Border = 0;
            // Add the main table to the document
            document.Add(multipleMainTable);


            PdfPCell leftCell2 = new PdfPCell(new Phrase("PROPERTY INFORMATION", GetFont()));
            leftCell2.Border = Rectangle.NO_BORDER;

            table.AddCell(leftCell2);
            document.Add(table);



            PdfPTable multipleMainTable1 = new PdfPTable(5);
            multipleMainTable1.WidthPercentage = 108;
            multipleMainTable1.SpacingBefore = 10f;
            multipleMainTable1.DefaultCell.Border = 0;

            float[] columnWidths1 = new float[] { 36f, 36f, 30f, 30f, 30f };
            multipleMainTable1.SetWidths(columnWidths1);
            // Step 5: Create the cells for each column using nested tables

            // Column 1
            PdfPTable col1Table1 = new PdfPTable(1);
            col1Table1.SpacingBefore = 5f;
            col1Table1.SpacingAfter = 5f;
            PdfPCell cell111 = new PdfPCell(new Phrase());
            cell111.FixedHeight = 100f; // Set the height of the cell
            cell111.BorderColor = BaseColor.BLACK;
            //cell111.Padding = 10;
            col1Table1.AddCell(cell111);


            //CHECKBOX FIELDS FOR CHATTELS

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 720, 23, 730), chattelslist[18].Checkbox, 23, 720);
            PdfContentByte content301 = writer.DirectContent;
            BaseFont baseF301 = BaseFont.CreateFont();
            content301.SetFontAndSize(baseF301, 8);
            content301.BeginText();
            content301.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[18].PropertyAttributeName, 55, 720, 0);
            content301.EndText();
            TextField tf301 = new TextField(writer, new Rectangle(115, 720, 135, 735), "Wall oven");
            tf301.BorderColor = new BaseColor(232, 232, 232);
            tf301.BackgroundColor = new BaseColor(232, 232, 232);
            tf301.Options = TextField.READ_ONLY;
            tf301.Text = chattelslist[18].Count != 0 ? chattelslist[18].Count.ToString() : "";
            tf301.TextColor = BaseColor.BLACK;
            tf301.FontSize = 8;
            writer.AddAnnotation(tf301.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 705, 23, 715), chattelslist[19].Checkbox, 23, 705);
            PdfContentByte content302 = writer.DirectContent;
            BaseFont baseF302 = BaseFont.CreateFont();
            content302.SetFontAndSize(baseF302, 8);
            content302.BeginText();
            content302.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[19].PropertyAttributeName, 67, 705, 0);
            content302.EndText();
            TextField tf302 = new TextField(writer, new Rectangle(115, 705, 135, 718), "Smoke detectors");
            tf302.BorderColor = new BaseColor(232, 232, 232);
            tf302.BackgroundColor = new BaseColor(232, 232, 232);
            tf302.Options = TextField.READ_ONLY;
            tf302.Text = chattelslist[19].Count != 0 ? chattelslist[19].Count.ToString() : "";
            tf302.TextColor = BaseColor.BLACK;
            tf302.FontSize = 8;
            writer.AddAnnotation(tf302.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 690, 23, 700), chattelslist[20].Checkbox, 23, 690);
            PdfContentByte content303 = writer.DirectContent;
            BaseFont baseF303 = BaseFont.CreateFont();
            content303.SetFontAndSize(baseF303, 8);
            content303.BeginText();
            content303.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[20].PropertyAttributeName, 66, 690, 0);
            content303.EndText();
            TextField tf303 = new TextField(writer, new Rectangle(115, 687, 135, 703), "Security system");
            tf303.BorderColor = new BaseColor(232, 232, 232);
            tf303.BackgroundColor = new BaseColor(232, 232, 232);
            tf303.Options = TextField.READ_ONLY;
            tf303.Text = chattelslist[20].Count != 0 ? chattelslist[20].Count.ToString() : "";
            tf303.TextColor = BaseColor.BLACK;
            tf303.FontSize = 8;
            writer.AddAnnotation(tf303.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 675, 23, 685), chattelslist[21].Checkbox, 23, 675);
            PdfContentByte content304 = writer.DirectContent;
            BaseFont baseF304 = BaseFont.CreateFont();
            content304.SetFontAndSize(baseF304, 8);
            content304.BeginText();
            content304.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[21].PropertyAttributeName, 76, 675, 0);
            content304.EndText();
            TextField tf304 = new TextField(writer, new Rectangle(115, 686, 135, 672), "Bathroom extractorfan");
            tf304.BorderColor = new BaseColor(232, 232, 232);
            tf304.BackgroundColor = new BaseColor(232, 232, 232);
            tf304.Options = TextField.READ_ONLY;
            tf304.Text = chattelslist[21].Count != 0 ? chattelslist[21].Count.ToString() : "";
            tf304.TextColor = BaseColor.BLACK;
            tf304.FontSize = 8;
            writer.AddAnnotation(tf304.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 660, 23, 670), chattelslist[22].Checkbox, 23, 660);
            PdfContentByte content305 = writer.DirectContent;
            BaseFont baseF305 = BaseFont.CreateFont();
            content305.SetFontAndSize(baseF305, 8);
            content305.BeginText();
            content305.ShowTextAligned(PdfContentByte.ALIGN_CENTER, chattelslist[22].PropertyAttributeName, 49, 660, 0);
            content305.EndText();

            TextField tf01 = new TextField(writer, new Rectangle(23, 655, 130, 640), "othertestfield");
            tf01.BorderColor = new BaseColor(232, 232, 232);
            tf01.BackgroundColor = new BaseColor(232, 232, 232);
            tf01.Options = TextField.READ_ONLY;
            tf01.FontSize = 8;
            tf01.Text = chattelslist[22].Remarks;
            tf01.TextColor = BaseColor.BLACK;
            writer.AddAnnotation(tf01.GetTextField());


            PdfPCell blankCell5 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell5.Border = PdfPCell.NO_BORDER;
            blankCell5.FixedHeight = 10f;
            col1Table1.AddCell(blankCell5);

            // CODE FOR OTHER ROOMS CHECKBOXES

            var otherrroomslist = data.Where(w => w.Name == "Other Rooms").ToList();

            PdfPCell cell112 = new PdfPCell(new Phrase());
            cell112.FixedHeight = 275f; // Set the height of the cell
            cell112.BorderColor = BaseColor.BLACK;
            cell112.Padding = 10;
            cell112.CellEvent = new RectangleOverPdfPCellBorder("Other rooms", 30f, 620f, 68f, 15f, 33f, 624f);
            //cell112.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Other rooms", 50f, 20f, 70f, 610);
            col1Table1.AddCell(cell112);



            //CHECKBOX FIELDS

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 610, 23, 600), otherrroomslist[0].Checkbox, 23, 600);
            PdfContentByte content306 = writer.DirectContent;
            BaseFont baseF306 = BaseFont.CreateFont();
            content306.SetFontAndSize(baseF306, 8);
            content306.BeginText();
            content306.ShowTextAligned(PdfContentByte.ALIGN_CENTER, otherrroomslist[0].PropertyAttributeName, 62, 600, 0);
            content306.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 595, 23, 585), (otherrroomslist[1].Checkbox != null ? otherrroomslist[1].Checkbox : false), 23, 585);
            PdfContentByte content307 = writer.DirectContent;
            BaseFont baseF307 = BaseFont.CreateFont();
            content307.SetFontAndSize(baseF307, 8);
            content307.BeginText();
            content307.ShowTextAligned(PdfContentByte.ALIGN_CENTER, otherrroomslist[1].PropertyAttributeName, 64, 585, 0);
            content307.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 580, 23, 570), otherrroomslist[2].Checkbox, 23, 570);
            PdfContentByte content308 = writer.DirectContent;
            BaseFont baseF308 = BaseFont.CreateFont();
            content308.SetFontAndSize(baseF308, 8);
            content308.BeginText();
            content308.ShowTextAligned(PdfContentByte.ALIGN_CENTER, otherrroomslist[2].PropertyAttributeName, 51, 570, 0);
            content308.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 565, 23, 555), otherrroomslist[3].Checkbox, 23, 555);
            PdfContentByte content309 = writer.DirectContent;
            BaseFont baseF309 = BaseFont.CreateFont();
            content309.SetFontAndSize(baseF309, 8);
            content309.BeginText();
            content309.ShowTextAligned(PdfContentByte.ALIGN_CENTER, otherrroomslist[3].PropertyAttributeName, 57, 555, 0);
            content309.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 550, 23, 540), otherrroomslist[4].Checkbox, 23, 540);
            PdfContentByte content310 = writer.DirectContent;
            BaseFont baseF310 = BaseFont.CreateFont();
            content310.SetFontAndSize(baseF310, 8);
            content310.BeginText();
            content310.ShowTextAligned(PdfContentByte.ALIGN_CENTER, otherrroomslist[4].PropertyAttributeName, 49, 540, 0);
            content310.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 535, 23, 525), otherrroomslist[5].Checkbox, 23, 525);
            PdfContentByte content311 = writer.DirectContent;
            BaseFont baseF311 = BaseFont.CreateFont();
            content311.SetFontAndSize(baseF311, 8);
            content311.BeginText();
            content311.ShowTextAligned(PdfContentByte.ALIGN_CENTER, otherrroomslist[5].PropertyAttributeName, 56, 525, 0);
            content311.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 520, 23, 510), otherrroomslist[6].Checkbox, 23, 510);
            PdfContentByte content312 = writer.DirectContent;
            BaseFont baseF312 = BaseFont.CreateFont();
            content312.SetFontAndSize(baseF312, 8);
            content312.BeginText();
            content312.ShowTextAligned(PdfContentByte.ALIGN_CENTER, otherrroomslist[6].PropertyAttributeName, 57, 510, 0);
            content312.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 505, 23, 495), otherrroomslist[7].Checkbox, 23, 495);
            PdfContentByte content313 = writer.DirectContent;
            BaseFont baseF313 = BaseFont.CreateFont();
            content313.SetFontAndSize(baseF313, 8);
            content313.BeginText();
            content313.ShowTextAligned(PdfContentByte.ALIGN_CENTER, otherrroomslist[7].PropertyAttributeName, 63, 495, 0);
            content313.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 490, 23, 480), otherrroomslist[8].Checkbox, 23, 480);
            PdfContentByte content314 = writer.DirectContent;
            BaseFont baseF314 = BaseFont.CreateFont();
            content314.SetFontAndSize(baseF314, 8);
            content314.BeginText();
            content314.ShowTextAligned(PdfContentByte.ALIGN_CENTER, otherrroomslist[8].PropertyAttributeName, 55, 480, 0);
            content314.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 475, 23, 465), otherrroomslist[9].Checkbox, 23, 465);
            PdfContentByte content315 = writer.DirectContent;
            BaseFont baseF315 = BaseFont.CreateFont();
            content315.SetFontAndSize(baseF315, 8);
            content315.BeginText();
            content315.ShowTextAligned(PdfContentByte.ALIGN_CENTER, otherrroomslist[9].PropertyAttributeName, 63, 465, 0);
            content315.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 460, 23, 450), otherrroomslist[10].Checkbox, 23, 450);
            PdfContentByte content316 = writer.DirectContent;
            BaseFont baseF316 = BaseFont.CreateFont();
            content316.SetFontAndSize(baseF316, 8);
            content316.BeginText();
            content316.ShowTextAligned(PdfContentByte.ALIGN_CENTER, otherrroomslist[10].PropertyAttributeName, 59, 450, 0);
            content316.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 445, 23, 435), otherrroomslist[11].Checkbox, 23, 435);
            PdfContentByte content317 = writer.DirectContent;
            BaseFont baseF317 = BaseFont.CreateFont();
            content317.SetFontAndSize(baseF317, 8);
            content317.BeginText();
            content317.ShowTextAligned(PdfContentByte.ALIGN_CENTER, otherrroomslist[11].PropertyAttributeName, 50, 435, 0);
            content317.EndText();

            TextField tf02 = new TextField(writer, new Rectangle(23, 430, 130, 410), "Other");
            tf02.BorderColor = new BaseColor(232, 232, 232);
            tf02.BackgroundColor = new BaseColor(232, 232, 232);
            tf02.Options = TextField.READ_ONLY;
            tf02.Text = otherrroomslist[11].Remarks;
            tf02.TextColor = BaseColor.BLACK;
            tf02.FontSize = 8;
            writer.AddAnnotation(tf02.GetTextField());

            PdfContentByte cb01 = writer.DirectContent;
            BaseFont bf01 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
            string text01 = "Bedrooms";
            cb01.BeginText();
            cb01.SetFontAndSize(bf01, 8);
            // 2ut the alignment and coordinates here
            cb01.ShowTextAligned(1, text01, 45, 400, 0);
            cb01.EndText();

            PdfContentByte content286 = writer.DirectContent;
            BaseFont baseF286 = BaseFont.CreateFont();
            content286.SetFontAndSize(baseF286, 8);
            content286.BeginText();
            content286.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Double", 33, 390, 0);
            content286.EndText();
            TextField tf03 = new TextField(writer, new Rectangle(53, 395, 130, 380), "Double");
            tf03.BorderColor = new BaseColor(232, 232, 232);
            tf03.BackgroundColor = new BaseColor(232, 232, 232);
            tf03.Options = TextField.READ_ONLY;
            //tf03.Text = othercommentList[0].Double;
            if (othercommentList.Count > 0)
            {
                tf03.Text = othercommentList[0].Double;
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf03.Text = ""; // Set a default value or handle it based on your requirements
            }

            tf03.TextColor = BaseColor.BLACK;
            tf03.FontSize = 8;
            writer.AddAnnotation(tf03.GetTextField());


            PdfContentByte content287 = writer.DirectContent;
            BaseFont baseF287 = BaseFont.CreateFont();
            content287.SetFontAndSize(baseF287, 8);
            content287.BeginText();
            content287.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Single", 33, 365, 0);
            content287.EndText();
            TextField tf04 = new TextField(writer, new Rectangle(53, 375, 130, 360), "Single");
            tf04.BorderColor = new BaseColor(232, 232, 232);
            tf04.BackgroundColor = new BaseColor(232, 232, 232);
            tf04.Options = TextField.READ_ONLY;
            // tf04.Text = othercommentList[0].Single;
            if (othercommentList.Count > 0)
            {
                tf04.Text = othercommentList[0].Single;
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf04.Text = ""; // Set a default value or handle it based on your requirements
            }

            tf04.TextColor = BaseColor.BLACK;
            tf04.FontSize = 8;
            writer.AddAnnotation(tf04.GetTextField());

            PdfPCell blankCell6 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell6.Border = PdfPCell.NO_BORDER;
            blankCell6.FixedHeight = 10f;
            col1Table1.AddCell(blankCell6);


            // CODE FOR HOT WATER CHECKBOXES

            var hotwaterlist = data.Where(w => w.Name == "Hot Water").ToList();

            PdfPCell cell113 = new PdfPCell(new Phrase());
            cell113.FixedHeight = 110f; // Set the height of the cell
            cell113.BorderColor = BaseColor.BLACK;
            cell113.Padding = 10;
            cell113.CellEvent = new RectangleOverPdfPCellBorder("Hot Water", 30f, 334f, 53f, 15f, 33f, 338f);
            //cell113.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Hot Water", 50f, 20f, 70f, 328);
            col1Table1.AddCell(cell113);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 330, 23, 320), hotwaterlist[0].Checkbox, 23, 320);
            PdfContentByte content318 = writer.DirectContent;
            BaseFont baseF318 = BaseFont.CreateFont();
            content318.SetFontAndSize(baseF318, 8);
            content318.BeginText();
            content318.ShowTextAligned(PdfContentByte.ALIGN_CENTER, hotwaterlist[0].PropertyAttributeName, 55, 320, 0);
            content318.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 315, 23, 305), hotwaterlist[1].Checkbox, 23, 305);
            PdfContentByte content319 = writer.DirectContent;
            BaseFont baseF319 = BaseFont.CreateFont();
            content319.SetFontAndSize(baseF319, 8);
            content319.BeginText();
            content319.ShowTextAligned(PdfContentByte.ALIGN_CENTER, hotwaterlist[1].PropertyAttributeName, 48, 305, 0);
            content319.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 300, 23, 290), hotwaterlist[2].Checkbox, 23, 290);
            PdfContentByte content320 = writer.DirectContent;
            BaseFont baseF320 = BaseFont.CreateFont();
            content320.SetFontAndSize(baseF320, 8);
            content320.BeginText();
            content320.ShowTextAligned(PdfContentByte.ALIGN_CENTER, hotwaterlist[1].PropertyAttributeName, 51, 290, 0);
            content320.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 285, 23, 275), hotwaterlist[3].Checkbox, 23, 275);
            PdfContentByte content321 = writer.DirectContent;
            BaseFont baseF321 = BaseFont.CreateFont();
            content321.SetFontAndSize(baseF321, 8);
            content321.BeginText();
            content321.ShowTextAligned(PdfContentByte.ALIGN_CENTER, hotwaterlist[3].PropertyAttributeName, 53, 275, 0);
            content321.EndText();

            TextField tf322 = new TextField(writer, new Rectangle(33, 260, 130, 240), "Other comments");
            tf322.BorderColor = new BaseColor(232, 232, 232);
            tf322.BackgroundColor = new BaseColor(232, 232, 232);
            tf322.Options = TextField.READ_ONLY;
            tf322.Text = hotwaterlist[4].Remarks;
            tf322.TextColor = BaseColor.BLACK;
            tf322.FontSize = 8;
            writer.AddAnnotation(tf322.GetTextField());

            PdfContentByte cb05 = writer.DirectContent;
            BaseFont bf05 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
            string text05 = "Other comments";
            cb05.BeginText();
            cb05.SetFontAndSize(bf05, 8);
            // 5ut the alignment and coordinates here
            cb05.ShowTextAligned(1, text05, 57, 263, 0);
            cb05.EndText();
            TextField tf05 = new TextField(writer, new Rectangle(40, 237, 150, 220), "Central");
            writer.AddAnnotation(tf05.GetTextField());


            multipleMainTable1.AddCell(col1Table1);

            //// Column 2
            PdfPTable col2Table2 = new PdfPTable(1);
            PdfPCell cell22 = new PdfPCell(new Phrase());
            cell22.FixedHeight = 60f; // Set the height of the cell
            cell22.BorderColor = BaseColor.BLACK;
            col2Table2.AddCell(cell22);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 720, 158, 730), heatinglist[12].Checkbox, 148, 720);
            PdfContentByte content322 = writer.DirectContent;
            BaseFont baseF322 = BaseFont.CreateFont();
            content322.SetFontAndSize(baseF322, 8);
            content322.BeginText();
            content322.ShowTextAligned(PdfContentByte.ALIGN_CENTER, heatinglist[12].PropertyAttributeName, 185, 720, 0);
            content322.EndText();

            PdfContentByte cb03 = writer.DirectContent;
            BaseFont bf03 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
            string text03 = "Other comments";
            cb03.BeginText();
            cb03.SetFontAndSize(bf03, 8);
            //03t the alignment and coordinates here
            cb03.ShowTextAligned(1, text03, 185, 710, 0);
            cb03.EndText();
            TextField tf06 = new TextField(writer, new Rectangle(148, 690, 245, 708), "Other comments");
            tf06.BorderColor = new BaseColor(232, 232, 232);
            tf06.BackgroundColor = new BaseColor(232, 232, 232);
            tf06.Options = TextField.READ_ONLY;
            tf06.Text = heatinglist[13].Remarks;
            tf06.TextColor = BaseColor.BLACK;
            tf06.FontSize = 8;
            writer.AddAnnotation(tf06.GetTextField());

            PdfPCell blankCell3 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell3.Border = PdfPCell.NO_BORDER;
            blankCell3.FixedHeight = 10f;
            col2Table2.AddCell(blankCell3);




            // CODE FOR KITCHEN CHECKBOXES

            var kitchenlist = data.Where(w => w.Name == "Kitchen").ToList();

            PdfPCell cell311 = new PdfPCell(new Phrase());
            cell311.FixedHeight = 75f; // Set the height of the cell
            cell311.BorderColor = BaseColor.BLACK;
            cell311.CellEvent = new RectangleOverPdfPCellBorder("Kitchen", 155f, 665f, 40f, 15f, 158f, 667f);
            //cell311.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Kitchen", 170f, 20f, 70f, 655);
            col2Table2.AddCell(cell311);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 645, 158, 655), kitchenlist[0].Checkbox, 148, 645);
            PdfContentByte content323 = writer.DirectContent;
            BaseFont baseF323 = BaseFont.CreateFont();
            content323.SetFontAndSize(baseF323, 8);
            content323.BeginText();
            content323.ShowTextAligned(PdfContentByte.ALIGN_CENTER, kitchenlist[0].PropertyAttributeName, 177, 645, 0);
            content323.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 630, 158, 640), kitchenlist[1].Checkbox, 148, 630);
            PdfContentByte content324 = writer.DirectContent;
            BaseFont baseF324 = BaseFont.CreateFont();
            content324.SetFontAndSize(baseF324, 8);
            content324.BeginText();
            content324.ShowTextAligned(PdfContentByte.ALIGN_CENTER, kitchenlist[1].PropertyAttributeName, 177, 630, 0);
            content324.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 615, 158, 625), kitchenlist[2].Checkbox, 148, 615);
            PdfContentByte content325 = writer.DirectContent;
            BaseFont baseF325 = BaseFont.CreateFont();
            content325.SetFontAndSize(baseF325, 8);
            content325.BeginText();
            content325.ShowTextAligned(PdfContentByte.ALIGN_CENTER, kitchenlist[2].PropertyAttributeName, 177, 615, 0);
            content325.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 600, 158, 610), kitchenlist[3].Checkbox, 148, 600);
            PdfContentByte content326 = writer.DirectContent;
            BaseFont baseF326 = BaseFont.CreateFont();
            content326.SetFontAndSize(baseF326, 8);
            content326.BeginText();
            content326.ShowTextAligned(PdfContentByte.ALIGN_CENTER, kitchenlist[3].PropertyAttributeName, 182, 600, 0);
            content326.EndText();


            PdfPCell blankCell4 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell4.Border = PdfPCell.NO_BORDER;
            blankCell4.FixedHeight = 10f;
            col2Table2.AddCell(blankCell4);


            // CODE FOR DINING CHECKBOXES

            var dininglist = data.Where(w => w.Name == "Kitchen").ToList();

            PdfPCell cell312 = new PdfPCell(new Phrase());
            cell312.FixedHeight = 75f; // Set the height of the cell
            cell312.BorderColor = BaseColor.BLACK;
            cell312.CellEvent = new RectangleOverPdfPCellBorder("Dining", 158f, 580f, 35f, 15f, 158f, 582f);
            // cell312.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Dining", 170f, 20f, 70f, 568f);

            col2Table2.AddCell(cell312);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 545, 158, 555), dininglist[0].Checkbox, 148, 545);
            PdfContentByte content327 = writer.DirectContent;
            BaseFont baseF327 = BaseFont.CreateFont();
            content327.SetFontAndSize(baseF327, 8);
            content327.BeginText();
            content327.ShowTextAligned(PdfContentByte.ALIGN_CENTER, dininglist[0].PropertyAttributeName, 191, 545, 0);
            content327.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 530, 158, 540), dininglist[1].Checkbox, 148, 530);
            PdfContentByte content328 = writer.DirectContent;
            BaseFont baseF328 = BaseFont.CreateFont();
            content328.SetFontAndSize(baseF328, 8);
            content328.BeginText();
            content328.ShowTextAligned(PdfContentByte.ALIGN_CENTER, dininglist[1].PropertyAttributeName, 189, 530, 0);
            content328.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 515, 158, 525), dininglist[2].Checkbox, 148, 515);
            PdfContentByte content329 = writer.DirectContent;
            BaseFont baseF329 = BaseFont.CreateFont();
            content329.SetFontAndSize(baseF329, 8);
            content329.BeginText();
            content329.ShowTextAligned(PdfContentByte.ALIGN_CENTER, dininglist[2].PropertyAttributeName, 192, 515, 0);
            content329.EndText();

            PdfPCell blankCell2 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell2.Border = PdfPCell.NO_BORDER;
            blankCell2.FixedHeight = 7f;
            col2Table2.AddCell(blankCell2);

            // CODE FOR BATHROOM/TOILET CHECKBOXES

            var bathroomlist = data.Where(w => w.Name == "Bathroom Toilets").ToList();

            PdfPCell cell313 = new PdfPCell(new Phrase());
            cell313.FixedHeight = 97f; // Set the height of the cell
            cell313.BorderColor = BaseColor.BLACK;
            cell313.CellEvent = new RectangleOverPdfPCellBorder("Bathroom/toilet", 155f, 495f, 78f, 15f, 158f, 498f);
            //cell313.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Bathroom/toilet", 170f, 15f, 70f, 495);
            col2Table2.AddCell(cell313);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 470, 158, 480), bathroomlist[0].Checkbox, 148, 470);
            PdfContentByte content330 = writer.DirectContent;
            BaseFont baseF330 = BaseFont.CreateFont();
            content330.SetFontAndSize(baseF330, 8);
            content330.BeginText();
            content330.ShowTextAligned(PdfContentByte.ALIGN_CENTER, bathroomlist[0].PropertyAttributeName, 196, 470, 0);
            content330.EndText();

            TextField tf330 = new TextField(writer, new Rectangle(235, 480, 255, 468), "Separate bathrooms");//(33, 445, 23, 435)
            tf330.BorderColor = new BaseColor(232, 232, 232);
            tf330.BackgroundColor = new BaseColor(232, 232, 232);
            tf330.Options = TextField.READ_ONLY;
            tf330.Text = bathroomlist[0].Count != 0 ? bathroomlist[0].Count.ToString() : "";
            tf330.TextColor = BaseColor.BLACK;
            tf330.FontSize = 8;
            writer.AddAnnotation(tf330.GetTextField());
            //col5Table.AddCell(cell51);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 455, 158, 465), bathroomlist[1].Checkbox, 148, 455);
            PdfContentByte content331 = writer.DirectContent;
            BaseFont baseF331 = BaseFont.CreateFont();
            content331.SetFontAndSize(baseF331, 8);
            content331.BeginText();
            content331.ShowTextAligned(PdfContentByte.ALIGN_CENTER, bathroomlist[1].PropertyAttributeName, 189, 455, 0);
            content331.EndText();

            TextField tf331 = new TextField(writer, new Rectangle(235, 465, 255, 453), "Separate WCs");//(33, 445, 23, 435)
            tf331.BorderColor = new BaseColor(232, 232, 232);
            tf331.BackgroundColor = new BaseColor(232, 232, 232);
            tf331.Options = TextField.READ_ONLY;
            tf331.Text = bathroomlist[1].Count != 0 ? bathroomlist[1].Count.ToString() : "";
            tf331.TextColor = BaseColor.BLACK;
            tf331.FontSize = 8;
            writer.AddAnnotation(tf331.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 440, 158, 450), bathroomlist[2].Checkbox, 148, 440);
            PdfContentByte content332 = writer.DirectContent;
            BaseFont baseF332 = BaseFont.CreateFont();
            content332.SetFontAndSize(baseF330, 8);
            content332.BeginText();
            content332.ShowTextAligned(PdfContentByte.ALIGN_CENTER, bathroomlist[2].PropertyAttributeName, 190, 440, 0);
            content332.EndText();

            TextField tf332 = new TextField(writer, new Rectangle(235, 450, 255, 438), "Separate shower");//(33, 445, 23, 435)
            tf332.BorderColor = new BaseColor(232, 232, 232);
            tf332.BackgroundColor = new BaseColor(232, 232, 232);
            tf332.Options = TextField.READ_ONLY;
            tf332.Text = bathroomlist[2].Count != 0 ? bathroomlist[2].Count.ToString() : "";
            tf332.TextColor = BaseColor.BLACK;
            tf332.FontSize = 8;
            writer.AddAnnotation(tf332.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 425, 158, 435), bathroomlist[3].Checkbox, 148, 425);
            PdfContentByte content333 = writer.DirectContent;
            BaseFont baseF333 = BaseFont.CreateFont();
            content333.SetFontAndSize(baseF330, 8);
            content333.BeginText();
            content333.ShowTextAligned(PdfContentByte.ALIGN_CENTER, bathroomlist[3].PropertyAttributeName, 197, 425, 0);
            content333.EndText();

            TextField tf333 = new TextField(writer, new Rectangle(235, 435, 255, 423), "Combined bth/WCs ");//(33, 445, 23, 435)
            tf333.BorderColor = new BaseColor(232, 232, 232);
            tf333.BackgroundColor = new BaseColor(232, 232, 232);
            tf333.Options = TextField.READ_ONLY;
            tf333.Text = bathroomlist[3].Count != 0 ? bathroomlist[3].Count.ToString() : "";
            tf333.TextColor = BaseColor.BLACK;
            tf333.FontSize = 8;
            writer.AddAnnotation(tf333.GetTextField());

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 410, 158, 420), bathroomlist[4].Checkbox, 148, 410);
            PdfContentByte content334 = writer.DirectContent;
            BaseFont baseF334 = BaseFont.CreateFont();
            content334.SetFontAndSize(baseF330, 8);
            content334.BeginText();
            content334.ShowTextAligned(PdfContentByte.ALIGN_CENTER, bathroomlist[4].PropertyAttributeName, 178, 410, 0);
            content334.EndText();

            TextField tf334 = new TextField(writer, new Rectangle(235, 420, 255, 410), "Ensuite");//(33, 445, 23, 435)
            tf334.BorderColor = new BaseColor(232, 232, 232);
            tf334.BackgroundColor = new BaseColor(232, 232, 232);
            tf334.Options = TextField.READ_ONLY;
            tf334.Text = bathroomlist[4].Count != 0 ? bathroomlist[4].Count.ToString() : "";
            tf334.TextColor = BaseColor.BLACK;
            tf334.FontSize = 8;
            writer.AddAnnotation(tf334.GetTextField());

            PdfPCell blankCell1 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell1.Border = PdfPCell.NO_BORDER;
            blankCell1.FixedHeight = 7f;
            col2Table2.AddCell(blankCell1);


            // CODE FOR LOUNGE CHECKBOXES

            var loungelist = data.Where(w => w.Name == "Lounge").ToList();

            PdfPCell cell314 = new PdfPCell(new Phrase());
            cell314.FixedHeight = 60f; // Set the height of the cell
            cell314.BorderColor = BaseColor.BLACK;
            cell314.CellEvent = new RectangleOverPdfPCellBorder("Lounge", 158f, 393f, 40f, 14f, 160f, 396f);
            //cell314.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Lounge", 170f, 20f, 70f, 385f);
            col2Table2.AddCell(cell314);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 365, 158, 375), loungelist[0].Checkbox, 148, 365);
            PdfContentByte content335 = writer.DirectContent;
            BaseFont baseF335 = BaseFont.CreateFont();
            content335.SetFontAndSize(baseF335, 8);
            content335.BeginText();
            content335.ShowTextAligned(PdfContentByte.ALIGN_CENTER, loungelist[0].PropertyAttributeName, 204, 365, 0);
            content335.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 350, 158, 360), loungelist[1].Checkbox, 148, 350);
            PdfContentByte content336 = writer.DirectContent;
            BaseFont baseF336 = BaseFont.CreateFont();
            content336.SetFontAndSize(baseF336, 8);
            content336.BeginText();
            content336.ShowTextAligned(PdfContentByte.ALIGN_CENTER, loungelist[1].PropertyAttributeName, 176, 350, 0);
            content336.EndText();

            PdfPCell blankCell = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell.Border = PdfPCell.NO_BORDER;
            blankCell.FixedHeight = 10f;
            col2Table2.AddCell(blankCell);


            // CODE FOR STOVE CHECKBOXES

            var stovelist = data.Where(w => w.Name == "Stove").ToList();

            PdfPCell cell315 = new PdfPCell(new Phrase());
            cell315.FixedHeight = 30f; // Set the height of the cell
            cell315.BorderColor = BaseColor.BLACK;
            cell315.CellEvent = new RectangleOverPdfPCellBorder("Stove", 158f, 322f, 30f, 14f, 158f, 325f);
            // cell315.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Stove", 170f, 20f, 70f, 316);
            col2Table2.AddCell(cell315);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 310, 158, 320), stovelist[0].Checkbox, 148, 310);
            PdfContentByte content337 = writer.DirectContent;
            BaseFont baseF337 = BaseFont.CreateFont();
            content337.SetFontAndSize(baseF337, 8);
            content337.BeginText();
            content337.ShowTextAligned(PdfContentByte.ALIGN_CENTER, stovelist[0].PropertyAttributeName, 176, 310, 0);
            content337.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 295, 158, 305), stovelist[1].Checkbox, 148, 295);
            PdfContentByte content338 = writer.DirectContent;
            BaseFont baseF338 = BaseFont.CreateFont();
            content338.SetFontAndSize(baseF338, 8);
            content338.BeginText();
            content338.ShowTextAligned(PdfContentByte.ALIGN_CENTER, stovelist[1].PropertyAttributeName, 179, 295, 0);
            content338.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(148, 280, 158, 290), stovelist[2].Checkbox, 148, 280);
            PdfContentByte content339 = writer.DirectContent;
            BaseFont baseF339 = BaseFont.CreateFont();
            content339.SetFontAndSize(baseF339, 8);
            content339.BeginText();
            content339.ShowTextAligned(PdfContentByte.ALIGN_CENTER, stovelist[2].PropertyAttributeName, 182, 280, 0);
            content339.EndText();

            multipleMainTable1.AddCell(col2Table2);


            // CODE FOR EXTERIOR CONDITION CHECKBOXES

            var exteriorConditonlist = data.Where(w => w.Name == "Exterior Condition").ToList();
            // Column 3
            PdfPTable col3Table3 = new PdfPTable(1);
            PdfPCell cell44 = new PdfPCell(new Phrase());
            cell44.FixedHeight = 70f; // Set the height of the cell
            cell44.BorderColor = BaseColor.BLACK;
            cell44.CellEvent = new RectangleOverPdfPCellBorder("Exterior condition", 272f, 733f, 85f, 15f, 273f, 735f);
            //cell44.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Exterior condition", 265f, 15f, 80f, 733f);
            col3Table3.AddCell(cell44);


            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 720, 285, 730), exteriorConditonlist[0].Checkbox, 275, 720);
            PdfContentByte content340 = writer.DirectContent;
            BaseFont baseF340 = BaseFont.CreateFont();
            content340.SetFontAndSize(baseF340, 8);
            content340.BeginText();
            content340.ShowTextAligned(PdfContentByte.ALIGN_CENTER, exteriorConditonlist[0].PropertyAttributeName, 307, 720, 0);
            content340.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 705, 285, 715), exteriorConditonlist[0].Checkbox, 275, 705);
            PdfContentByte content341 = writer.DirectContent;
            BaseFont baseF341 = BaseFont.CreateFont();
            content341.SetFontAndSize(baseF341, 8);
            content341.BeginText();
            content341.ShowTextAligned(PdfContentByte.ALIGN_CENTER, exteriorConditonlist[1].PropertyAttributeName, 310, 705, 0);
            content341.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 690, 285, 700), exteriorConditonlist[2].Checkbox, 275, 690);
            PdfContentByte content342 = writer.DirectContent;
            BaseFont baseF342 = BaseFont.CreateFont();
            content342.SetFontAndSize(baseF342, 8);
            content342.BeginText();
            content342.ShowTextAligned(PdfContentByte.ALIGN_CENTER, exteriorConditonlist[2].PropertyAttributeName, 301, 690, 0);
            content342.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 675, 285, 685), exteriorConditonlist[3].Checkbox, 275, 675);
            PdfContentByte content343 = writer.DirectContent;
            BaseFont baseF343 = BaseFont.CreateFont();
            content343.SetFontAndSize(baseF343, 8);
            content343.BeginText();
            content343.ShowTextAligned(PdfContentByte.ALIGN_CENTER, exteriorConditonlist[3].PropertyAttributeName, 299, 675, 0);
            content343.EndText();

            PdfPCell blankCell7 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell7.Border = PdfPCell.NO_BORDER;
            blankCell7.FixedHeight = 10f;
            col3Table3.AddCell(blankCell7);



            // CODE FOR SWIMMING POOL CONDITION CHECKBOXES

            var swimmingpoollist = data.Where(w => w.Name == "Swimming Pool").ToList();
            PdfPCell cell45 = new PdfPCell(new Phrase());
            cell45.FixedHeight = 105f; // Set the height of the cell
            cell45.BorderColor = BaseColor.BLACK;
            cell45.CellEvent = new RectangleOverPdfPCellBorder("Swimming pool", 272f, 650f, 85f, 15f, 273f, 655f);
            //cell45.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Swimming pool", 265f, 15f, 80f, 650f);
            col3Table3.AddCell(cell45);


            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 635, 285, 645), swimmingpoollist[0].Checkbox, 275, 635);
            PdfContentByte content344 = writer.DirectContent;
            BaseFont baseF344 = BaseFont.CreateFont();
            content344.SetFontAndSize(baseF344, 8);
            content344.BeginText();
            content344.ShowTextAligned(PdfContentByte.ALIGN_CENTER, swimmingpoollist[0].PropertyAttributeName, 314, 635, 0);
            content344.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 620, 285, 630), swimmingpoollist[1].Checkbox, 275, 620);
            PdfContentByte content345 = writer.DirectContent;
            BaseFont baseF345 = BaseFont.CreateFont();
            content345.SetFontAndSize(baseF345, 8);
            content345.BeginText();
            content345.ShowTextAligned(PdfContentByte.ALIGN_CENTER, swimmingpoollist[1].PropertyAttributeName, 307, 620, 0);
            content345.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 605, 285, 615), swimmingpoollist[2].Checkbox, 275, 605);
            PdfContentByte content346 = writer.DirectContent;
            BaseFont baseF346 = BaseFont.CreateFont();
            content346.SetFontAndSize(baseF346, 8);
            content346.BeginText();
            content346.ShowTextAligned(PdfContentByte.ALIGN_CENTER, swimmingpoollist[2].PropertyAttributeName, 306, 605, 0);
            content346.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 590, 285, 600), swimmingpoollist[3].Checkbox, 275, 590);
            PdfContentByte content347 = writer.DirectContent;
            BaseFont baseF347 = BaseFont.CreateFont();
            content347.SetFontAndSize(baseF347, 8);
            content347.BeginText();
            content347.ShowTextAligned(PdfContentByte.ALIGN_CENTER, swimmingpoollist[3].PropertyAttributeName, 304, 590, 0);
            content347.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 575, 285, 585), swimmingpoollist[4].Checkbox, 275, 575);
            PdfContentByte content348 = writer.DirectContent;
            BaseFont baseF348 = BaseFont.CreateFont();
            content348.SetFontAndSize(baseF348, 8);
            content348.BeginText();
            content348.ShowTextAligned(PdfContentByte.ALIGN_CENTER, swimmingpoollist[4].PropertyAttributeName, 303, 575, 0);
            content348.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 560, 285, 570), swimmingpoollist[5].Checkbox, 275, 560);
            PdfContentByte content349 = writer.DirectContent;
            BaseFont baseF349 = BaseFont.CreateFont();
            content349.SetFontAndSize(baseF349, 8);
            content349.BeginText();
            content349.ShowTextAligned(PdfContentByte.ALIGN_CENTER, swimmingpoollist[5].PropertyAttributeName, 302, 560, 0);
            content349.EndText();

            PdfPCell blankCell8 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell8.Border = PdfPCell.NO_BORDER;
            blankCell8.FixedHeight = 10f;
            col3Table3.AddCell(blankCell8);




            // CODE FOR GENERAL CHECKBOXES

            var generallist = data.Where(w => w.Name == "General").ToList();

            PdfPCell cell46 = new PdfPCell(new Phrase());
            cell46.FixedHeight = 115f; // Set the height of the cell
            cell46.BorderColor = BaseColor.BLACK;
            cell46.CellEvent = new RectangleOverPdfPCellBorder("General", 278f, 535f, 45f, 15f, 280f, 538f);
            //cell46.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "General", 275f, 20f, 70f, 530f);
            col3Table3.AddCell(cell46);


            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 515, 285, 525), generallist[0].Checkbox, 275, 515);
            PdfContentByte content350 = writer.DirectContent;
            BaseFont baseF350 = BaseFont.CreateFont();
            content350.SetFontAndSize(baseF350, 8);
            content350.BeginText();
            content350.ShowTextAligned(PdfContentByte.ALIGN_CENTER, generallist[0].PropertyAttributeName, 315, 515, 0);
            content350.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 500, 285, 510), generallist[1].Checkbox, 275, 500);
            PdfContentByte content351 = writer.DirectContent;
            BaseFont baseF351 = BaseFont.CreateFont();
            content351.SetFontAndSize(baseF351, 8);
            content351.BeginText();
            content351.ShowTextAligned(PdfContentByte.ALIGN_CENTER, generallist[1].PropertyAttributeName, 315, 500, 0);
            content351.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 485, 285, 495), generallist[2].Checkbox, 275, 485);
            PdfContentByte content352 = writer.DirectContent;
            BaseFont baseF352 = BaseFont.CreateFont();
            content352.SetFontAndSize(baseF352, 8);
            content352.BeginText();
            content352.ShowTextAligned(PdfContentByte.ALIGN_CENTER, generallist[2].PropertyAttributeName, 313, 485, 0);
            content352.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 470, 285, 480), generallist[3].Checkbox, 275, 470);
            PdfContentByte content353 = writer.DirectContent;
            BaseFont baseF353 = BaseFont.CreateFont();
            content353.SetFontAndSize(baseF353, 8);
            content353.BeginText();
            content353.ShowTextAligned(PdfContentByte.ALIGN_CENTER, generallist[3].PropertyAttributeName, 313, 470, 0);
            content353.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 455, 285, 465), generallist[4].Checkbox, 275, 455);
            PdfContentByte content354 = writer.DirectContent;
            BaseFont baseF354 = BaseFont.CreateFont();
            content354.SetFontAndSize(baseF354, 8);
            content354.BeginText();
            content354.ShowTextAligned(PdfContentByte.ALIGN_CENTER, generallist[4].PropertyAttributeName, 321, 455, 0);
            content354.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 440, 285, 450), generallist[5].Checkbox, 275, 440);
            PdfContentByte content355 = writer.DirectContent;
            BaseFont baseF355 = BaseFont.CreateFont();
            content355.SetFontAndSize(baseF355, 8);
            content355.BeginText();
            content355.ShowTextAligned(PdfContentByte.ALIGN_CENTER, generallist[5].PropertyAttributeName, 321, 440, 0);
            content355.EndText();

            PdfPCell blankCell9 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell9.Border = PdfPCell.NO_BORDER;
            blankCell9.FixedHeight = 10f;
            col3Table3.AddCell(blankCell9);




            // CODE FOR ROOOF CHECKBOXES

            var rooflist = data.Where(w => w.Name == "Roof").ToList();


            PdfPCell cell47 = new PdfPCell(new Phrase());
            cell47.FixedHeight = 95f; // Set the height of the cell
            cell47.BorderColor = BaseColor.BLACK;
            cell47.CellEvent = new RectangleOverPdfPCellBorder("Roof", 278f, 415f, 25f, 15f, 278f, 415f);
            //cell47.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Roof", 275f, 20f, 70f, 405f);
            col3Table3.AddCell(cell47);


            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 385, 285, 395), rooflist[0].Checkbox, 275, 385);
            PdfContentByte content356 = writer.DirectContent;
            BaseFont baseF356 = BaseFont.CreateFont();
            content356.SetFontAndSize(baseF356, 8);
            content356.BeginText();
            content356.ShowTextAligned(PdfContentByte.ALIGN_CENTER, rooflist[0].PropertyAttributeName, 299, 385, 0);
            content356.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 370, 285, 380), rooflist[1].Checkbox, 275, 370);
            PdfContentByte content357 = writer.DirectContent;
            BaseFont baseF357 = BaseFont.CreateFont();
            content357.SetFontAndSize(baseF357, 8);
            content357.BeginText();
            content357.ShowTextAligned(PdfContentByte.ALIGN_CENTER, rooflist[1].PropertyAttributeName, 312, 370, 0);
            content357.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 355, 285, 365), rooflist[2].Checkbox, 275, 355);
            PdfContentByte content358 = writer.DirectContent;
            BaseFont baseF358 = BaseFont.CreateFont();
            content358.SetFontAndSize(baseF358, 8);
            content358.BeginText();
            content358.ShowTextAligned(PdfContentByte.ALIGN_CENTER, rooflist[2].PropertyAttributeName, 310, 355, 0);
            content358.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 340, 285, 350), rooflist[3].Checkbox, 275, 340);
            PdfContentByte content359 = writer.DirectContent;
            BaseFont baseF359 = BaseFont.CreateFont();
            content359.SetFontAndSize(baseF359, 8);
            content359.BeginText();
            content359.ShowTextAligned(PdfContentByte.ALIGN_CENTER, rooflist[3].PropertyAttributeName, 309, 340, 0);
            content359.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 325, 285, 335), rooflist[4].Checkbox, 275, 325);
            PdfContentByte content360 = writer.DirectContent;
            BaseFont baseF360 = BaseFont.CreateFont();
            content360.SetFontAndSize(baseF360, 8);
            content360.BeginText();
            content360.ShowTextAligned(PdfContentByte.ALIGN_CENTER, rooflist[4].PropertyAttributeName, 309, 325, 0);
            content360.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 310, 285, 320), rooflist[5].Checkbox, 275, 310);
            PdfContentByte content361 = writer.DirectContent;
            BaseFont baseF361 = BaseFont.CreateFont();
            content361.SetFontAndSize(baseF361, 8);
            content361.BeginText();
            content361.ShowTextAligned(PdfContentByte.ALIGN_CENTER, rooflist[5].PropertyAttributeName, 318, 310, 0);
            content361.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 295, 285, 305), rooflist[6].Checkbox, 275, 295);
            PdfContentByte content362 = writer.DirectContent;
            BaseFont baseF362 = BaseFont.CreateFont();
            content362.SetFontAndSize(baseF362, 8);
            content362.BeginText();
            content362.ShowTextAligned(PdfContentByte.ALIGN_CENTER, rooflist[6].PropertyAttributeName, 308, 295, 0);
            content362.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 280, 285, 290), rooflist[7].Checkbox, 275, 280);
            PdfContentByte content363 = writer.DirectContent;
            BaseFont baseF363 = BaseFont.CreateFont();
            content363.SetFontAndSize(baseF363, 8);
            content363.BeginText();
            content363.ShowTextAligned(PdfContentByte.ALIGN_CENTER, rooflist[7].PropertyAttributeName, 310, 280, 0);
            content363.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 264, 285, 275), rooflist[8].Checkbox, 275, 264);
            PdfContentByte content364 = writer.DirectContent;
            BaseFont baseF364 = BaseFont.CreateFont();
            content364.SetFontAndSize(baseF364, 8);
            content364.BeginText();
            content364.ShowTextAligned(PdfContentByte.ALIGN_CENTER, rooflist[8].PropertyAttributeName, 327, 264, 0);
            content364.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(275, 250, 285, 260), rooflist[9].Checkbox, 275, 250);
            PdfContentByte content365 = writer.DirectContent;
            BaseFont baseF365 = BaseFont.CreateFont();
            content365.SetFontAndSize(baseF365, 8);
            content365.BeginText();
            content365.ShowTextAligned(PdfContentByte.ALIGN_CENTER, rooflist[9].PropertyAttributeName, 306, 250, 0);
            content365.EndText();

            TextField tf07 = new TextField(writer, new Rectangle(275, 230, 350, 245), "Central");
            tf07.BorderColor = new BaseColor(232, 232, 232);
            tf07.BackgroundColor = new BaseColor(232, 232, 232);
            tf07.Options = TextField.READ_ONLY;
            tf07.Text = rooflist[9].Remarks;
            tf07.TextColor = BaseColor.BLACK;
            tf07.FontSize = 8;
            writer.AddAnnotation(tf07.GetTextField());

            multipleMainTable1.AddCell(col3Table3);



            //CODE FOR FENCING CHECKBOXES

            var fencingList = data.Where(w => w.Name == "Fencing").ToList();
            // Column 4
            PdfPTable col4Table4 = new PdfPTable(1);
            PdfPCell cell5 = new PdfPCell(new Phrase());
            cell5.FixedHeight = 65f; // Set the height of the cell
            cell5.BorderColor = BaseColor.BLACK;
            cell5.CellEvent = new RectangleOverPdfPCellBorder("Fencing", 378f, 735f, 40f, 15f, 378f, 738f);
            //cell5.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Fencing", 375f, 20f, 70f, 728f);
            col4Table4.AddCell(cell5);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 725, 390, 715), fencingList[0].Checkbox, 380, 715);
            PdfContentByte content366 = writer.DirectContent;
            BaseFont baseF366 = BaseFont.CreateFont();
            content366.SetFontAndSize(baseF366, 8);
            content366.BeginText();
            content366.ShowTextAligned(PdfContentByte.ALIGN_CENTER, fencingList[0].PropertyAttributeName, 415, 715, 0);
            content366.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 710, 390, 700), fencingList[1].Checkbox, 380, 700);
            PdfContentByte content367 = writer.DirectContent;
            BaseFont baseF367 = BaseFont.CreateFont();
            content367.SetFontAndSize(baseF367, 8);
            content367.BeginText();
            content367.ShowTextAligned(PdfContentByte.ALIGN_CENTER, fencingList[1].PropertyAttributeName, 421, 700, 0);
            content367.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 695, 390, 685), fencingList[2].Checkbox, 380, 685);
            PdfContentByte content368 = writer.DirectContent;
            BaseFont baseF368 = BaseFont.CreateFont();
            content368.SetFontAndSize(baseF368, 8);
            content368.BeginText();
            content368.ShowTextAligned(PdfContentByte.ALIGN_CENTER, fencingList[1].PropertyAttributeName, 420, 685, 0);
            content368.EndText();

            PdfPCell blankCell11 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell11.Border = PdfPCell.NO_BORDER;
            blankCell11.FixedHeight = 10f;
            col4Table4.AddCell(blankCell11);




            //CODE FOR ASPECT CHECKBOXES

            var aspectList = data.Where(w => w.Name == "Aspect").ToList();

            PdfPCell cell6 = new PdfPCell(new Phrase());
            cell6.FixedHeight = 85f; // Set the height of the cell
            cell6.BorderColor = BaseColor.BLACK;
            cell6.CellEvent = new RectangleOverPdfPCellBorder("Aspect", 378f, 660f, 35f, 15f, 378f, 662f);
            //cell6.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Aspect", 375f, 20f, 70f, 650f);
            col4Table4.AddCell(cell6);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 635, 390, 645), aspectList[0].Checkbox, 380, 635);
            PdfContentByte content369 = writer.DirectContent;
            BaseFont baseF369 = BaseFont.CreateFont();
            content369.SetFontAndSize(baseF369, 8);
            content369.BeginText();
            content369.ShowTextAligned(PdfContentByte.ALIGN_CENTER, aspectList[0].PropertyAttributeName, 414, 635, 0);
            content369.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 620, 390, 630), aspectList[1].Checkbox, 380, 620);
            PdfContentByte content370 = writer.DirectContent;
            BaseFont baseF370 = BaseFont.CreateFont();
            content370.SetFontAndSize(baseF370, 8);
            content370.BeginText();
            content370.ShowTextAligned(PdfContentByte.ALIGN_CENTER, aspectList[1].PropertyAttributeName, 414, 620, 0);
            content370.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 605, 390, 615), aspectList[2].Checkbox, 380, 605);
            PdfContentByte content371 = writer.DirectContent;
            BaseFont baseF371 = BaseFont.CreateFont();
            content371.SetFontAndSize(baseF371, 8);
            content371.BeginText();
            content371.ShowTextAligned(PdfContentByte.ALIGN_CENTER, aspectList[2].PropertyAttributeName, 414, 605, 0);
            content371.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 590, 390, 600), aspectList[3].Checkbox, 380, 590);
            PdfContentByte content372 = writer.DirectContent;
            BaseFont baseF372 = BaseFont.CreateFont();
            content372.SetFontAndSize(baseF372, 8);
            content372.BeginText();
            content372.ShowTextAligned(PdfContentByte.ALIGN_CENTER, aspectList[3].PropertyAttributeName, 414, 590, 0);
            content372.EndText();

            PdfPCell blankCell10 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell10.Border = PdfPCell.NO_BORDER;
            blankCell10.FixedHeight = 10f;
            col4Table4.AddCell(blankCell10);

          

            //CODE FOR VIEWS CHECKBOXES

            var viewsList = data.Where(w => w.Name == "Views").ToList();

            PdfPCell cell7 = new PdfPCell(new Phrase());
            cell7.FixedHeight = 190f; // Set the height of the cell
            cell7.BorderColor = BaseColor.BLACK;
            cell7.CellEvent = new RectangleOverPdfPCellBorder("Views", 378f, 563f, 30f, 15f, 378f, 565f);
            //cell7.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Views", 375f, 15f, 70f, 560f);
            col4Table4.AddCell(cell7);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 535, 390, 545), viewsList[0].Checkbox, 380, 535);
            PdfContentByte content373 = writer.DirectContent;
            BaseFont baseF373 = BaseFont.CreateFont();
            content373.SetFontAndSize(baseF373, 8);
            content373.BeginText();
            content373.ShowTextAligned(PdfContentByte.ALIGN_CENTER, viewsList[0].PropertyAttributeName, 405, 535, 0);
            content373.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 520, 390, 530), viewsList[1].Checkbox, 380, 520);
            PdfContentByte content374 = writer.DirectContent;
            BaseFont baseF374 = BaseFont.CreateFont();
            content374.SetFontAndSize(baseF374, 8);
            content374.BeginText();
            content374.ShowTextAligned(PdfContentByte.ALIGN_CENTER, viewsList[1].PropertyAttributeName, 405, 520, 0);
            content374.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 505, 390, 515), viewsList[2].Checkbox, 380, 505);
            PdfContentByte content375 = writer.DirectContent;
            BaseFont baseF375 = BaseFont.CreateFont();
            content375.SetFontAndSize(baseF375, 8);
            content375.BeginText();
            content375.ShowTextAligned(PdfContentByte.ALIGN_CENTER, viewsList[2].PropertyAttributeName, 407, 505, 0);
            content375.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 490, 390, 500), viewsList[3].Checkbox, 380, 490);
            PdfContentByte content376 = writer.DirectContent;
            BaseFont baseF376 = BaseFont.CreateFont();
            content376.SetFontAndSize(baseF376, 8);
            content376.BeginText();
            content376.ShowTextAligned(PdfContentByte.ALIGN_CENTER, viewsList[3].PropertyAttributeName, 408, 490, 0);
            content376.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 475, 390, 485), viewsList[4].Checkbox, 380, 475);
            PdfContentByte content377 = writer.DirectContent;
            BaseFont baseF377 = BaseFont.CreateFont();
            content377.SetFontAndSize(baseF377, 8);
            content377.BeginText();
            content377.ShowTextAligned(PdfContentByte.ALIGN_CENTER, viewsList[4].PropertyAttributeName, 409, 475, 0);
            content377.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 460, 390, 470), viewsList[5].Checkbox, 380, 460);
            PdfContentByte content378 = writer.DirectContent;
            BaseFont baseF378 = BaseFont.CreateFont();
            content378.SetFontAndSize(baseF378, 8);
            content378.BeginText();
            content378.ShowTextAligned(PdfContentByte.ALIGN_CENTER, viewsList[5].PropertyAttributeName, 408, 460, 0);
            content378.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 445, 390, 455), viewsList[6].Checkbox, 380, 445);
            PdfContentByte content379 = writer.DirectContent;
            BaseFont baseF379 = BaseFont.CreateFont();
            content379.SetFontAndSize(baseF379, 8);
            content379.BeginText();
            content379.ShowTextAligned(PdfContentByte.ALIGN_CENTER, viewsList[6].PropertyAttributeName, 410, 445, 0);
            content379.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 430, 390, 440), viewsList[7].Checkbox, 380, 430);
            PdfContentByte content380 = writer.DirectContent;
            BaseFont baseF380 = BaseFont.CreateFont();
            content380.SetFontAndSize(baseF380, 8);
            content380.BeginText();
            content380.ShowTextAligned(PdfContentByte.ALIGN_CENTER, viewsList[7].PropertyAttributeName, 405, 430, 0);
            content380.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 415, 390, 425), viewsList[8].Checkbox, 380, 415);
            PdfContentByte content381 = writer.DirectContent;
            BaseFont baseF381 = BaseFont.CreateFont();
            content381.SetFontAndSize(baseF381, 8);
            content381.BeginText();
            content381.ShowTextAligned(PdfContentByte.ALIGN_CENTER, viewsList[8].PropertyAttributeName, 415, 415, 0);
            content381.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 400, 390, 410), viewsList[9].Checkbox, 380, 400);
            PdfContentByte content382 = writer.DirectContent;
            BaseFont baseF382 = BaseFont.CreateFont();
            content382.SetFontAndSize(baseF382, 8);
            content382.BeginText();
            content382.ShowTextAligned(PdfContentByte.ALIGN_CENTER, viewsList[9].PropertyAttributeName, 412, 400, 0);
            content382.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 385, 390, 395), viewsList[10].Checkbox, 380, 385);
            PdfContentByte content383 = writer.DirectContent;
            BaseFont baseF383 = BaseFont.CreateFont();
            content383.SetFontAndSize(baseF383, 8);
            content383.BeginText();
            content383.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Other", 412, 385, 0);
            content383.EndText();

            PdfPCell blankCell12 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell12.Border = PdfPCell.NO_BORDER;
            blankCell12.FixedHeight = 10f;
            col4Table4.AddCell(blankCell12);


            //CODE FOR SEWAGE CHECKBOXES





            var sewageList = data.Where(w => w.Name == "Sewage system").ToList();

            PdfPCell cell71 = new PdfPCell(new Phrase());
            cell71.FixedHeight = 100f; // Set the height of the cell
            cell71.BorderColor = BaseColor.BLACK;
            cell71.CellEvent = new RectangleOverPdfPCellBorder("Sewage system", 378f, 363f, 75f, 15f, 378f, 365f);
            //cell71.CellEvent = new RectangleCellEvent(BaseColor.GREEN, BaseColor.BLUE, "Sewage system", 375f, 20f, 70f, 355f);
            col4Table4.AddCell(cell71);

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 350, 390, 360), sewageList[0].Checkbox, 380, 350);
            PdfContentByte content384 = writer.DirectContent;
            BaseFont baseF384 = BaseFont.CreateFont();
            content384.SetFontAndSize(baseF384, 8);
            content384.BeginText();
            content384.ShowTextAligned(PdfContentByte.ALIGN_CENTER, sewageList[0].PropertyAttributeName, 411, 350, 0);
            content384.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 335, 390, 345), sewageList[1].Checkbox, 380, 335);
            PdfContentByte content385 = writer.DirectContent;
            BaseFont baseF385 = BaseFont.CreateFont();
            content385.SetFontAndSize(baseF385, 8);
            content385.BeginText();
            content385.ShowTextAligned(PdfContentByte.ALIGN_CENTER, sewageList[1].PropertyAttributeName, 411, 335, 0);
            content385.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 320, 390, 330), sewageList[2].Checkbox, 380, 320);
            PdfContentByte content386 = writer.DirectContent;
            BaseFont baseF386 = BaseFont.CreateFont();
            content386.SetFontAndSize(baseF386, 8);
            content386.BeginText();
            content386.ShowTextAligned(PdfContentByte.ALIGN_CENTER, sewageList[2].PropertyAttributeName, 415, 320, 0);
            content386.EndText();

            AddCheckboxField(document, writer, "myCheckbox", new Rectangle(380, 305, 390, 315), sewageList[3].Checkbox, 380, 305);
            PdfContentByte content387 = writer.DirectContent;
            BaseFont baseF387 = BaseFont.CreateFont();
            content387.SetFontAndSize(baseF387, 8);
            content387.BeginText();
            content387.ShowTextAligned(PdfContentByte.ALIGN_CENTER, sewageList[3].PropertyAttributeName, 411, 305, 0);
            content387.EndText();

            TextField tf08 = new TextField(writer, new Rectangle(380, 285, 450, 300), "Other");
            tf08.BorderColor = new BaseColor(232, 232, 232);
            tf08.BackgroundColor = new BaseColor(232, 232, 232);
            tf08.Options = TextField.READ_ONLY;
            tf08.Text = sewageList[3].Remarks;
            tf08.TextColor = BaseColor.BLACK;
            tf08.FontSize = 8;
            writer.AddAnnotation(tf08.GetTextField());

            multipleMainTable1.AddCell(col4Table4);

            //CODE FOR OTHER COMMENT SECTIONS
            //var othercommentList = _context.PropertyInformationDetail.Where(w => w.PID == id).ToList();
            // Column 5
            PdfPTable col5Table5 = new PdfPTable(1);
            PdfPCell cell8 = new PdfPCell(new Phrase());
            cell8.FixedHeight = 215f; // Set the height of the cell
            cell8.BorderColor = BaseColor.BLACK;
            cell8.CellEvent = new RectangleOverPdfPCellBorder("Other", 480f, 740f, 40f, 15f, 482f, 740f);
            //cell8.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Other", 478f, 20f, 70f, 728f);
            col5Table5.AddCell(cell8);

            PdfContentByte content01 = writer.DirectContent;
            BaseFont baseF01 = BaseFont.CreateFont();
            content01.SetFontAndSize(baseF01, 8);
            content01.BeginText();
            content01.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Other Comments:", 515, 715, 0);
            content01.EndText();
            TextField tf09 = new TextField(writer, new Rectangle(485, 660, 565, 705), "Central");
            tf09.BorderColor = new BaseColor(232, 232, 232);
            tf09.BackgroundColor = new BaseColor(232, 232, 232);
            tf09.Options = TextField.READ_ONLY | TextField.MULTILINE;
            // tf09.Text = othercommentList[0].Comment;
            if (othercommentList.Count > 0)
            {
                tf09.Text = othercommentList[0].Comment;
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf09.Text = ""; // Set a default value or handle it based on your requirements
            }

            tf09.TextColor = BaseColor.BLACK;
            tf09.FontSize = 8;
            writer.AddAnnotation(tf09.GetTextField());

            PdfContentByte content02 = writer.DirectContent;
            BaseFont baseF02 = BaseFont.CreateFont();
            content02.SetFontAndSize(baseF02, 8);
            content02.BeginText();
            content02.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Other features/additionals:", 526, 650, 0);
            content02.EndText();
            TextField tf010 = new TextField(writer, new Rectangle(485, 600, 565, 640), "Other features/additionals");
            tf010.BorderColor = new BaseColor(232, 232, 232);
            tf010.BackgroundColor = new BaseColor(232, 232, 232);
            tf010.Options = TextField.READ_ONLY | TextField.MULTILINE;
            //tf010.Text = othercommentList[0].AdditionalFeature;
            if (othercommentList.Count > 0)
            {
                tf010.Text = othercommentList[0].AdditionalFeature;
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf010.Text = ""; // Set a default value or handle it based on your requirements
            }
            tf010.TextColor = BaseColor.BLACK;
            tf010.FontSize = 8;
            writer.AddAnnotation(tf010.GetTextField());

            PdfContentByte content03 = writer.DirectContent;
            BaseFont baseF03 = BaseFont.CreateFont();
            content03.SetFontAndSize(baseF03, 8);
            content03.BeginText();
            content03.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Excluded chattels:", 515, 590, 0);
            content03.EndText();
            TextField tf011 = new TextField(writer, new Rectangle(485, 540, 565, 580), "Central");
            tf011.BorderColor = new BaseColor(232, 232, 232);
            tf011.BackgroundColor = new BaseColor(232, 232, 232);
            tf011.Options = TextField.READ_ONLY | TextField.MULTILINE;
            // tf011.Text = othercommentList[0].ExcludedChattels;
            if (othercommentList.Count > 0)
            {
                tf011.Text = othercommentList[0].ExcludedChattels;
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf011.Text = ""; // Set a default value or handle it based on your requirements
            }
            tf011.TextColor = BaseColor.BLACK;
            tf011.FontSize = 8;
            writer.AddAnnotation(tf011.GetTextField());


            PdfPCell blankCell13 = new PdfPCell(new Phrase(Chunk.NEWLINE));
            blankCell13.Border = PdfPCell.NO_BORDER;
            blankCell13.FixedHeight = 10f;
            col5Table5.AddCell(blankCell13);

            PdfPCell cell10 = new PdfPCell(new Phrase());
            cell10.FixedHeight = 100f; // Set the height of the cell
            cell10.BorderColor = BaseColor.BLACK;
            cell10.CellEvent = new RectangleOverPdfPCellBorder("Internal remarks", 480f, 513f, 80f, 14f, 482f, 513f);
            // cell10.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Internal remarks", 478f, 20f, 70f, 500f);
            col5Table5.AddCell(cell10);

            TextField tf012 = new TextField(writer, new Rectangle(485, 350, 565, 495), "Internal remarks");
            tf012.BorderColor = new BaseColor(232, 232, 232);
            tf012.BackgroundColor = new BaseColor(232, 232, 232);
            tf012.Options = TextField.READ_ONLY | TextField.MULTILINE;
            // tf012.Text = othercommentList[0].InternalRemark;
            if (othercommentList.Count > 0)
            {
                tf012.Text = othercommentList[0].InternalRemark;
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf012.Text = ""; // Set a default value or handle it based on your requirements
            }
            tf012.TextColor = BaseColor.BLACK;
            tf012.FontSize = 8;
            writer.AddAnnotation(tf012.GetTextField());

            col1Table1.DefaultCell.Border = 0;
            col2Table2.DefaultCell.Border = 0;
            col3Table3.DefaultCell.Border = 0;
            col4Table4.DefaultCell.Border = 0;
            col5Table5.DefaultCell.Border = 0;
            multipleMainTable1.AddCell(col5Table5);

            // Add the main table to the document
            document.Add(multipleMainTable1);



            //tenancy Details
            PdfPTable tenancyTable = new PdfPTable(1);

            PdfPCell tenancyoutercell21 = new PdfPCell();
            tenancyTable.DefaultCell.Border = 0;
            tenancyoutercell21.BorderColor = BaseColor.BLACK;
            tenancyoutercell21.FixedHeight = 120f;
            tenancyTable.WidthPercentage = 108; // Set the width percentage of the parent table        
            tenancyTable.SpacingBefore = 12f;
            tenancyTable.SpacingAfter = 10f;
            //tenancyoutercell21.CellEvent = new RectangleOverPdfPCellBorder("Tenancy details", 33f, 210f, 80f, 14f, 482f, 200f);
            tenancyoutercell21.CellEvent = new RectangleCellEvent(BaseColor.YELLOW, BaseColor.BLUE, "Tenancy details", 33f, 20f, 80f, 200f);
            //particulartable.AddCell(outercell);

            PdfPTable tenancyinnerTable21 = new PdfPTable(2);
            tenancyinnerTable21.DefaultCell.Border = 0;
            tenancyinnerTable21.WidthPercentage = 100f;
            tenancyinnerTable21.SpacingBefore = 10f;



            PdfContentByte cb015 = writer.DirectContent;
            BaseFont bf015 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
            string text015 = "Status";

            cb015.BeginText();
            cb015.SetFontAndSize(bf015, 8);
            //015ut the alignment and coordinates here
            cb015.ShowTextAligned(1, text015, 40, 192, 0);
            cb015.EndText();
            // Create the first inner cell
            PdfPCell tenancyinnerCell21 = new PdfPCell();
            tenancyinnerCell21.FixedHeight = cellHeight;
            tenancyinnerCell21.Border = 0;
            tenancyinnerCell21.HorizontalAlignment = Element.ALIGN_CENTER;
            tenancyinnerCell21.VerticalAlignment = Element.ALIGN_MIDDLE;
            tenancyinnerTable21.AddCell(innerCell21);

            //fields and checkboxes
            if (tenancyDetailsList != null && tenancyDetailsList.Count > 0)
            {
                bool isVacant = tenancyDetailsList[0].IsVacant;

                AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 175, 23, 185), isVacant, 23, 175);
            }
            else
            {
                // Handle the case when methodOfSaleList is null or empty
                // For example, you might want to provide a default value for the checkbox
                AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 175, 23, 185), false, 23, 175);
            }
            //AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 175, 23, 185), FinalList.tenancyDetail.IsVacant, 23, 175);
            //AddCheckboxField(document, writer, "myCheckbox", new Rectangle(33, 175, 23, 185), FinalList.tenancyDetail.IsVacant, 23, 175);
            PdfContentByte content08 = writer.DirectContent;
            BaseFont baseF08 = BaseFont.CreateFont();
            content08.SetFontAndSize(baseF08, 8);
            content08.BeginText();
            content08.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Vacant", 50, 175, 0);
            content08.EndText();


            if (tenancyDetailsList != null && tenancyDetailsList.Count > 0)
            {
                bool isTananted = tenancyDetailsList[0].IsTananted;

                AddCheckboxField(document, writer, "myCheckbox", new Rectangle(90, 175, 80, 185), isTananted, 23, 175);
            }
            else
            {
                // Handle the case when methodOfSaleList is null or empty
                // For example, you might want to provide a default value for the checkbox
                AddCheckboxField(document, writer, "myCheckbox", new Rectangle(90, 175, 80, 185), false, 23, 175);
            }
            // AddCheckboxField(document, writer, "myCheckbox", new Rectangle(90, 175, 80, 185), FinalList.tenancyDetail.IsTananted, 23, 175);
            //AddCheckboxField(document, writer, "myCheckbox", new Rectangle(90, 175, 80, 185), FinalList.tenancyDetail.IsTananted, 23, 175);
            PdfContentByte content09 = writer.DirectContent;
            BaseFont baseF09 = BaseFont.CreateFont();
            content09.SetFontAndSize(baseF09, 8);
            content09.BeginText();
            content09.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Tenanted", 110, 175, 0);
            content09.EndText();

            PdfContentByte content010 = writer.DirectContent;
            BaseFont baseF010 = BaseFont.CreateFont();
            content010.SetFontAndSize(baseF010, 8);
            content010.BeginText();
            content010.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Start", 33, 150, 0);
            content010.EndText();
            string logoPath11 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "Calender.png");
            iTextSharp.text.Image logo11 = iTextSharp.text.Image.GetInstance(logoPath11);
            if (System.IO.File.Exists(logoPath11))
            {
                logo11.ScaleAbsolute(15f, 15f);
                logo11.SetAbsolutePosition(43, 150);


                document.Add(logo11);
            }
            TextField tf013 = new TextField(writer, new Rectangle(59, 170, 150, 150), "start");
            tf013.BorderColor = new BaseColor(232, 232, 232);
            tf013.BackgroundColor = new BaseColor(232, 232, 232);
            if (FinalList.tenancyDetail != null && FinalList.tenancyDetail.TenancyStartDate != null)
            {
                tf013.Text = ((DateTime)FinalList.tenancyDetail.TenancyStartDate).ToString("dd-MM-yyyy");
            }
            else
            {
                tf013.Text = "";
            }

            tf013.FontSize = 8;
            tf013.Options = TextField.READ_ONLY;


            tf013.FontSize = 8;
            tf013.Options = TextField.READ_ONLY;


            writer.AddAnnotation(tf013.GetTextField());



            PdfContentByte content011 = writer.DirectContent;
            BaseFont baseF011 = BaseFont.CreateFont();
            content011.SetFontAndSize(baseF011, 8);
            content011.BeginText();
            content011.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "End", 33, 115, 0);
            content011.EndText();
            string logoPath22 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "Calender.png");
            iTextSharp.text.Image logo22 = iTextSharp.text.Image.GetInstance(logoPath22);
            if (System.IO.File.Exists(logoPath22))
            {
                logo22.ScaleAbsolute(15f, 15f);
                logo22.SetAbsolutePosition(43, 115);


                document.Add(logo22);
            }
            TextField tf014 = new TextField(writer, new Rectangle(59, 140, 150, 120), "end");
            tf014.BorderColor = new BaseColor(232, 232, 232);
            tf014.BackgroundColor = new BaseColor(232, 232, 232);
            if (FinalList.tenancyDetail != null && FinalList.tenancyDetail.TenancyEndDate != null)
            {
                tf014.Text = ((DateTime)FinalList.tenancyDetail.TenancyEndDate).ToString("dd-MM-yyyy");
            }
            else
            {
                tf014.Text = "";
            }
            // tf014.Text = ((DateTime)FinalList.tenancyDetail.TenancyEndDate).ToString("dd-MM-yyyy");
            tf014.FontSize = 8;
            tf014.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf014.GetTextField());

            PdfContentByte cb016 = writer.DirectContent;
            BaseFont bf016 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
            string text016 = "Tenant details";
            cb016.BeginText();
            cb016.SetFontAndSize(bf016, 8);
            //016ut the alignment and coordinates here
            cb016.ShowTextAligned(1, text016, 230, 195, 0);
            cb016.EndText();

            PdfContentByte content012 = writer.DirectContent;
            BaseFont baseF012 = BaseFont.CreateFont();
            content012.SetFontAndSize(baseF012, 8);
            content012.BeginText();
            content012.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Name", 190, 175, 0);
            content012.EndText();
            TextField tf016 = new TextField(writer, new Rectangle(348, 190, 210, 170), "Name");
            tf016.BorderColor = new BaseColor(232, 232, 232);
            tf016.BackgroundColor = new BaseColor(232, 232, 232);
            tf016.Options = TextField.READ_ONLY;
            tf016.FontSize = 8;
            //tf016.Text = FinalList.tenancyDetail.Name;
            if (FinalList != null && FinalList.tenancyDetail != null && FinalList.tenancyDetail.Name != null)
            {
                string Name = FinalList.tenancyDetail.Name.ToString();
                tf016.Text = Name;
            }
            else
            {
                tf016.Text = " "; // Set a default value if the data is not available
            }


            tf016.TextColor = BaseColor.BLACK;
            writer.AddAnnotation(tf016.GetTextField());

            PdfContentByte content013 = writer.DirectContent;
            BaseFont baseF013 = BaseFont.CreateFont();
            content013.SetFontAndSize(baseF013, 8);
            content013.BeginText();
            content013.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Email", 190, 155, 0);
            content013.EndText();
            TextField tf015 = new TextField(writer, new Rectangle(348, 165, 210, 145), "Email");
            tf015.BorderColor = new BaseColor(232, 232, 232);
            tf015.BackgroundColor = new BaseColor(232, 232, 232);
            if (FinalList != null && FinalList.tenancyDetail != null && FinalList.tenancyDetail.Email != null)
            {
                string Email = FinalList.tenancyDetail.Email.ToString();
                tf015.Text = Email;
            }
            else
            {
                tf015.Text = " "; // Set a default value if the data is not available
            }
            //tf015.Text = FinalList.tenancyDetail.Email;

            tf015.FontSize = 8;
            tf015.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf015.GetTextField());

            PdfContentByte content014 = writer.DirectContent;
            BaseFont baseF014 = BaseFont.CreateFont();
            content014.SetFontAndSize(baseF014, 8);
            content014.BeginText();
            content014.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Phone", 190, 130, 0);
            content014.EndText();
            TextField tf017 = new TextField(writer, new Rectangle(348, 140, 210, 120), "Phone");
            tf017.BorderColor = new BaseColor(232, 232, 232);
            tf017.BackgroundColor = new BaseColor(232, 232, 232);
            if (FinalList != null && FinalList.tenancyDetail != null && FinalList.tenancyDetail.Phone != null)
            {
                string Phone = FinalList.tenancyDetail.Phone.ToString();
                tf017.Text = Phone;
            }
            else
            {
                tf017.Text = " "; // Set a default value if the data is not available
            }
            // tf017.Text = FinalList.tenancyDetail.Phone;
            tf017.FontSize = 8;
            tf017.Options = TextField.READ_ONLY;
            writer.AddAnnotation(tf017.GetTextField());


            PdfContentByte cb017 = writer.DirectContent;
            BaseFont bf017 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
            string text017 = "Tenancy details";
            cb017.BeginText();
            cb017.SetFontAndSize(bf017, 8);
            //017ut the alignment and coordinates here
            cb017.ShowTextAligned(1, text017, 400, 195, 0);
            cb017.EndText();
            TextField tf019 = new TextField(writer, new Rectangle(360, 190, 550, 120), "Tenancy details");
            tf019.BorderColor = new BaseColor(232, 232, 232);
            tf019.BackgroundColor = new BaseColor(232, 232, 232);
            tf019.Options = TextField.READ_ONLY | TextField.MULTILINE;
            tf019.FontSize = 8f;
            tf019.TextColor = BaseColor.BLACK;
            //tf019.Options = TextField.MULTILINE;

            //  tf019.Text = FinalList.tenancyDetail.TenancyDetails;
            if (FinalList != null && FinalList.tenancyDetail != null && FinalList.tenancyDetail.TenancyDetails != null)
            {
                string TenancyDetails = FinalList.tenancyDetail.TenancyDetails;
                tf019.Text = TenancyDetails;
            }
            else
            {
                tf019.Text = " "; // Set a default value if the data is not available
            }

            writer.AddAnnotation(tf019.GetTextField());




            PdfPCell tenancyinnerCell22 = new PdfPCell();
            tenancyinnerCell22.FixedHeight = cellHeight;
            tenancyinnerCell22.Border = 0;
            tenancyinnerCell22.HorizontalAlignment = Element.ALIGN_CENTER;
            tenancyinnerCell22.VerticalAlignment = Element.ALIGN_MIDDLE;
            tenancyinnerTable21.AddCell(innerCell22);
            tenancyoutercell21.AddElement(innerTable21);
            tenancyTable.AddCell(tenancyoutercell21);
            document.Add(tenancyTable);


            //NEW PAGE 
            //document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));
            //document.Add(new Paragraph(" "));


            // AddAggrementContent(document, writer, paragraphText, priorAgencyMarketingList, estimatesList, additionaldisclosure, methodOfSaleList);

            AddAggrementContent(document, writer, paragraphText, priorAgencyMarketingList, estimatesList, additionaldisclosure, methodOfSaleList, clientDetailsList, contractdetailsList, signatureOfClients, id, execution);

            document.Close();
            //_response.ContentType = MediaTypeNames.Application.Pdf;
            var contentDisposition = new ContentDisposition
            {
                FileName = "ClientDetails.pdf",
                Inline = false
            };

            System.IO.File.WriteAllBytes(@"C:\Users\HP\Source\Repos\RE360\RE360.API\Document\" + id.ToString() + ".pdf", memoryStream.ToArray());
            //_response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            //return File(memoryStream.ToArray(), _response.ContentType);






        }
        public enum FieldType
        {
            Text,
            Email,
            Date
        }

        public Font GetFont()
        {
            return FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.BOLD);
        }
        private static void CreateTextField(PdfWriter writer, Rectangle position, string fieldName,
                                        string fieldLabel, int fontSize)
        {
            // Create the text field
            TextField textField = new TextField(writer, position, fieldName);
            textField.FontSize = fontSize;
            textField.Alignment = Element.ALIGN_RIGHT;

            // Add the text field as an annotation
            writer.AddAnnotation(textField.GetTextField());

            // Add the content inside the text field
            PdfContentByte overContent = writer.DirectContent;
            BaseFont baseFont = BaseFont.CreateFont();
            overContent.SetFontAndSize(baseFont, fontSize);
            overContent.BeginText();
            overContent.ShowTextAligned(PdfContentByte.ALIGN_LEFT, fieldLabel, position.Left - 10, position.Bottom, 0);
            overContent.EndText();
        }


        public static void AddCheckboxField(Document document, PdfWriter writer, string fieldName, Rectangle position, bool isChecked, int x, int y)
        {

            // Create a checkbox field
            PdfFormField checkboxField = PdfFormField.CreateCheckBox(writer);
            checkboxField.FieldName = fieldName;

            if (isChecked)
            {

                string checkmarkImagePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "Checked1.png");
                iTextSharp.text.Image checkmarkImage = iTextSharp.text.Image.GetInstance(checkmarkImagePath);
                // Image checkmarkImage = Image.GetInstance(checkmarkImagePath);
                checkmarkImage.SetAbsolutePosition(x, y);
                checkmarkImage.ScaleToFit(10, 10); // Adjust the size of the image as needed

                PdfContentByte cb = writer.DirectContent;
                cb.AddImage(checkmarkImage);
                Phrase phrase = new Phrase();
                //Font zapfdingbats = new Font(Font.FontFamily.ZAPFDINGBATS);
                //phrase.Add(new Chunk("\u0033", zapfdingbats));              
                //PdfContentByte cb = writer.DirectContent;
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, phrase, x, y, 0);
            }
            //else
            //{
            //    string checkmarkImagePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images", "Unchecked1.png");
            //    iTextSharp.text.Image checkmarkImage = iTextSharp.text.Image.GetInstance(checkmarkImagePath);
            //    checkmarkImage.SetAbsolutePosition(x, y);
            //    checkmarkImage.ScaleToFit(9, 10); // Adjust the size of the image as needed

            //    PdfContentByte cb = writer.DirectContent;
            //    cb.AddImage(checkmarkImage);
            //    Phrase phrase = new Phrase();

            //    ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, phrase, x, y, 0);
            //}

            checkboxField.SetWidget(position, PdfAnnotation.APPEARANCE_NORMAL);
            int fieldFlags = PdfAnnotation.FLAGS_READONLY;
            checkboxField.SetFieldFlags(fieldFlags);
            checkboxField.SetFieldFlags(PdfFormField.FF_READ_ONLY);
            // checkboxField.MKBackgroundColor = new  BaseColor(232, 232, 232);
            checkboxField.MKBorderColor = new BaseColor(135, 206, 235);
            //43, 145, 175
            writer.AddAnnotation(checkboxField);



        }

        private void AddAggrementContent(Document doc, PdfWriter writer, string paragraphText, List<PriorAgencyMarketing> priorAgencyMarketings, List<Estimates> estimates, List<EstimatesDetail> estimatesDetails, List<MethodOfSale> methodOfSales, List<ClientDetail> clientDetails, List<ContractDetail> contractDetails, List<SignaturesOfClient> signature, int id,Execution execution)
        {
            if (clientDetails == null || clientDetails.Count == 0)
            {

                List<ClientDetail> obj = new List<ClientDetail>();		
               // ClientDetail obj = new ClientDetail();
                clientDetails = obj;
            }
            else
            {
                clientDetails = clientDetails;
            }
            PdfPTable t1 = new PdfPTable(2);
            t1.DefaultCell.Border = 0;
            t1.WidthPercentage = 100;

            PdfPCell leftCell01 = new PdfPCell(new Phrase("RESIDENTIAL LISTING AGENCY AGREEMENT", GetFont()));
            leftCell01.Border = Rectangle.NO_BORDER;
            leftCell01.HorizontalAlignment = Element.ALIGN_LEFT;
            leftCell01.Border = Rectangle.NO_BORDER;
            leftCell01.BorderColor = BaseColor.CYAN;
            leftCell01.FixedHeight = 50f;
            t1.AddCell(leftCell01);

            PdfPCell leftCell02 = new PdfPCell();
            leftCell02.Border = Rectangle.NO_BORDER;
            leftCell02.HorizontalAlignment = Element.ALIGN_RIGHT;
            leftCell02.Border = Rectangle.NO_BORDER;
            leftCell02.FixedHeight = 50f;
            string logoPath33 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "logopdf.png");
            iTextSharp.text.Image logo33 = iTextSharp.text.Image.GetInstance(logoPath33);
            if (System.IO.File.Exists(logoPath33))
            {
                logo33.ScaleAbsolute(70f, 70f);
                //logo33.SetAbsolutePosition(500, 725);

                leftCell02.PaddingLeft = 185f;
                leftCell02.PaddingBottom = 0f;
                leftCell02.PaddingTop = 0f;
                leftCell02.PaddingRight = 0f;
                leftCell02.AddElement(logo33);
            }

            t1.AddCell(leftCell02);
            doc.Add(t1);

            AddTextFieldToPDF(writer, doc, "Propertyfield", "Property Address", propertyAddress, 570, 765, 120, 785);
            

            // Create a font for the section titles


            // Create a font for the main content
            Font contentFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

            // Define the RGB color values
            int red = 0;
            int green = 128;
            int blue = 255;

            // Create a BaseColor object with the specified RGB values
            BaseColor fontColor = new BaseColor(red, green, blue);
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, fontColor);

            // Create the Font with the specified color
            Font colorFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, fontColor);

            BaseColor textColor = new BaseColor(0, 128, 255);
            string str = string.Empty;
            foreach (var item in clientDetails)
            {

                if (item.CompanyTrustName == null || item.CompanyTrustName == "")
                {

                    str += item.FirstName + " " + item.SurName + " ,";
                }
                else
                {
                    str += item.CompanyTrustName + " ";
                }
            }
            // Add the title
            Phrase formattedContent0 = new Phrase();
            Chunk agentChunk0 = new Chunk("                                                                                 (Client) appoints", contentFont);
            Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);

            doc.Add(new Paragraph("1.   APPOINTMENT", titleFont));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" 1.1.", titleFont));

            formattedContent0.Add(agentChunk0);
            doc.Add(formattedContent0);
            TextField tf211 = new TextField(writer, new Rectangle(230, 700, 70, 725), "Blinds");
            tf211.BorderColor = new BaseColor(232, 232, 232);
            tf211.BackgroundColor = new BaseColor(232, 232, 232);
            tf211.Options = TextField.READ_ONLY;
            tf211.Text = str;
            tf211.FontSize = 8;
            tf211.TextColor = BaseColor.BLACK;
            writer.AddAnnotation(tf211.GetTextField());
            //AddTextFieldToPDF(writer, doc, "1.1.", str, 230, 700, 70, 725);

            TextField tf212 = new TextField(writer, new Rectangle(540, 700, 320, 725), "(Client) appoints");
            tf212.BorderColor = new BaseColor(232, 232, 232);
            tf212.BackgroundColor = new BaseColor(232, 232, 232);
            tf212.Options = TextField.READ_ONLY;
            tf212.Text = _user.FirstName + " " + _user.LastName;
            tf212.FontSize = 8;
            tf212.TextColor = BaseColor.BLACK;
            writer.AddAnnotation(tf212.GetTextField());


            // Format the content with bold text
            Phrase formattedContent = new Phrase();

            // Regular content
            formattedContent.Add(new Chunk($"a licensed agent under the Real Estate Agents Act 2008 (REAA 2008), " +
                "and all other franchisees of Harcourts Group Limited ", contentFont));




            // Bold text (Agent)

            Chunk agentChunk = new Chunk("(Agent)", boldFont);
            formattedContent.Add(agentChunk);

            // Continue with regular content
            formattedContent.Add(new Chunk(", as the Client’s agent to act in the " +
                "sale or other disposal of the described property ", contentFont));

            // Bold text (Property)
            Chunk propertyChunk = new Chunk("(Property)", boldFont);
            formattedContent.Add(propertyChunk);

            // Continue with regular content
            formattedContent.Add(new Chunk(", on the terms and conditions set out in this agency agreement ", contentFont));

            // Bold text (Agreement)

            Chunk agreementChunk = new Chunk("(Agreement)", boldFont);
            formattedContent.Add(agreementChunk);

            Phrase formattedContent2 = new Phrase();

            formattedContent2.Add(new Chunk("2.  AGENCY", titleFont));
            Chunk agencyChunk = new Chunk(" (Choose either sole agency or general agency. Delete clause 2.1 or 2.2 as applicable.)", colorFont);
            formattedContent2.Add(agencyChunk);

            Phrase formattedContent3 = new Phrase();

            formattedContent3.Add(new Chunk("2.1  Sole Agency", colorFont));


            Phrase formattedContent4 = new Phrase();
            formattedContent4.Add(new Chunk("The Client appoints the Agent as sole agent. The agency commences on", contentFont));
            Chunk CommentedChunk = new Chunk("                                                 ", boldFont);
            TextField tf213 = new TextField(writer, new Rectangle(440, 590, 330, 610), "formattedContent4");
            tf213.BorderColor = new BaseColor(232, 232, 232);
            tf213.BackgroundColor = new BaseColor(232, 232, 232);
            tf213.Options = TextField.READ_ONLY;

            tf213.Text = isSoleSelected == true && contractDetails[0].AuthorityStartDate != null ? ((DateTime)contractDetails[0].AuthorityStartDate).ToString("dd-MM-yyyy") : string.Empty;

           // tf213.Text = isSoleSelected == true ? ((DateTime)contractDetails[0].AuthorityStartDate).ToString("dd-MM-yyyy") : string.Empty;
            tf213.FontSize = 8;
            tf213.TextColor = BaseColor.BLACK;
            writer.AddAnnotation(tf213.GetTextField());
            //AddTextFieldToPDF(writer, doc, "formattedContent4", "", 440, 593, 330, 610);
            Chunk Commented1Chunk = new Chunk("(Commencement Date)", boldFont);
            Chunk Commented2Chunk = new Chunk("and continues until midnight on ", contentFont);

            TextField tf214 = new TextField(writer, new Rectangle(250, 578, 170, 593), "formattedContent4");
            tf214.BorderColor = new BaseColor(232, 232, 232);
            tf214.BackgroundColor = new BaseColor(232, 232, 232);
            tf214.Options = TextField.READ_ONLY;
            tf214.Text = isSoleSelected == true && contractDetails[0].AuthorityEndDate != null ? ((DateTime)contractDetails[0].AuthorityEndDate).ToString("dd-MM-yyyy") : string.Empty;
           // tf214.Text = isSoleSelected == true ? ((DateTime)contractDetails[0].AuthorityEndDate).ToString("dd-MM-yyyy") : string.Empty;
            tf214.FontSize = 8;
            tf214.TextColor = BaseColor.BLACK;
            writer.AddAnnotation(tf214.GetTextField());
            //AddTextFieldToPDF(writer, doc, "Commented2Chunk", "", 250, 578, 170, 591);
            Chunk Commented3Chunk = new Chunk("                             " +
                "       or if no end date is provided," +
                " ninety (90) days from the Commencement Date.", contentFont);
            Chunk Commented4Chunk = new Chunk("This sole agency may be terminated by the Client, by written notice to the Agent by 5pm on the first working day after the day on which a copy of this Agreement is given to the Client.", contentFont);

            formattedContent4.Add(CommentedChunk);
            formattedContent4.Add(Commented1Chunk);
            formattedContent4.Add(Commented2Chunk);
            formattedContent4.Add(Commented3Chunk);
            formattedContent4.Add(Commented4Chunk);



            Phrase formattedContent5 = new Phrase();

            formattedContent5.Add(new Chunk("Note:", boldFont));
            Chunk c1 = new Chunk(" Any party to a sole agency agreement that relates to residential property" +
                " and is for a term longer than ninety (90) days may, at any time after the expiry of the" +
                "period of ninety (90) days after the Agreement is signed by the client, cancel the Agreement" +
                " by written notice to the other party or parties.", contentFont);


            formattedContent5.Add(c1);


            Phrase formattedContent6 = new Phrase();

            formattedContent6.Add(new Chunk("2.2.   General Agency \r\n", colorFont));
            Chunk c2 = new Chunk("The Client appoints the Agent as general agent. The agency commences on", contentFont);

            TextField tf215 = new TextField(writer, new Rectangle(400, 465, 340, 480), "formattedContent4");
            AddTextFieldToPDF(writer, doc, "c3", "", "", 563, 464, 525, 482);
            tf215.BorderColor = new BaseColor(232, 232, 232);
            tf215.BackgroundColor = new BaseColor(232, 232, 232);
            tf215.Options = TextField.READ_ONLY;
            tf215.Text = isGeneralSelected == true && contractDetails[0].AuthorityEndDate != null ? ((DateTime)contractDetails[0].AuthorityEndDate).ToString("dd-MM-yyyy") : string.Empty;
            // tf215.Text = isGeneralSelected == true ? ((DateTime)contractDetails[0].AuthorityEndDate).ToString("dd-MM-yyyy") : string.Empty;
            tf215.FontSize = 8;
            tf215.TextColor = BaseColor.BLACK;
            writer.AddAnnotation(tf215.GetTextField());

            //AddTextFieldToPDF(writer, doc, "c2", "", 400, 459, 340, 485);		

            TextField tf216 = new TextField(writer, new Rectangle(563, 465, 525, 480), "formattedContent4");
            tf216.BorderColor = new BaseColor(232, 232, 232);
            tf216.BackgroundColor = new BaseColor(232, 232, 232);
            tf216.Options = TextField.READ_ONLY;
            tf216.Text = isGeneralSelected == true && contractDetails[0].AuthorityEndDate != null ? ((DateTime)contractDetails[0].AuthorityEndDate).ToString("dd-MM-yyyy") : string.Empty;
           // tf216.Text = isGeneralSelected == true ? ((DateTime)contractDetails[0].AuthorityEndDate).ToString("dd-MM-yyyy") : string.Empty;
            tf216.FontSize = 8;
            tf216.TextColor = BaseColor.BLACK;
            writer.AddAnnotation(tf216.GetTextField());

            // AddTextFieldToPDF(writer, doc, "c2", "", 400, 464, 340, 482);
            // AddTextFieldToPDF(writer, doc, "c3", "", 563, 464, 525, 482);
            Chunk c3 = new Chunk("                         " +
                "and continues until midnight on" +
                "             \r\n" +
                "unless otherwise cancelled prior by either party giving seven (7) days’ written notice to the other party; or, if no end date is provided, until cancelled by either party giving seven (7) days’ written notice to the other party.", contentFont);

                formattedContent6.Add(c2);
            formattedContent6.Add(c3);



            Phrase formattedContent7 = new Phrase();

            formattedContent7.Add(new Chunk("3. PRIOR AGENCY", titleFont));
            Chunk c4 = new Chunk(" (Delete clause 3.1 or 3.2 as applicable. If neither option is deleted then clause 3.1 applies.)\r\n", contentFont);
            //AddTextFieldToPDF(writer, doc, "Commented2Chunk", "", 400, 481, 340, 500);
            //AddTextFieldToPDF(writer, doc, "Commented2Chunk", "", 560, 481, 525, 500);
            Chunk c5 = new Chunk(" 3.1. ", colorFont);
            Chunk c6 = new Chunk("The Client has not appointed any other real estate agent to sell the Property prior to signing this Agreement; or\r\n", contentFont);
            Chunk c7 = new Chunk(" 3.2. ", colorFont);
            Chunk c8 = new Chunk("The Client has appointed the following real estate agent(s) prior to signing this Agreement and has provided a copy of the agreements: \r\n     " +
                "Agency Name:                         " +
                "                                       Agency Period:\r\n \r\n" +
                "     Agency Name:                                       " +
                "                         Agency Period:\r\n ", contentFont);

            Chunk c9 = new Chunk("3.3.", colorFont);
            Chunk c10 = new Chunk("The Client acknowledges that if the Client has entered into a sole agency with any other real estate agent, the Client may be liable to pay full commission to more than one agent if a sale is effected during the term of the prior sole agency, regardless of whether or not the sale is by or through the instrumentality of the sole agent. The Client should not sign this Agreement if there is a current sole agency held by another real estate agent.\r\n", contentFont);
            Chunk c11 = new Chunk("Note:", boldFont);
            Chunk c12 = new Chunk(" If a sale is effected under a general agency agreement, by or through the instrumentality of any other real estate agent authorised by the Client, then the Client may be liable to pay full commission to more than one agent. If the Client is entering into a sole agency under this Agreement, and there is a prior general agency still in effect, the Client should ensure that the prior general agency agreement is cancelled and provide written confirmation to the Agent that this has been done, or should give written authority to the Agent to do so on the Client’s behalf.\r\n\r\n", contentFont);

            TextField tf054 = new TextField(writer, new Rectangle(250, 350, 110, 370), "field1");
            tf054.BorderColor = new BaseColor(232, 232, 232);
            tf054.BackgroundColor = new BaseColor(232, 232, 232);

            if (priorAgencyMarketings.Count > 0)
            {
                tf054.Text = priorAgencyMarketings[0].AgencyName;
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf054.Text = ""; // Set a default value or handle it based on your requirements
            }

            tf054.FontSize = 8;
            tf054.Options = TextField.READ_ONLY | TextField.MULTILINE;
            writer.AddAnnotation(tf054.GetTextField());

            TextField tf055 = new TextField(writer, new Rectangle(520, 350, 330, 370), "field2");
            tf055.BorderColor = new BaseColor(232, 232, 232);
            tf055.BackgroundColor = new BaseColor(232, 232, 232);
            //tf055.Text = priorAgencyMarketings[0].AgencyExpiredDate.ToString();
            if (priorAgencyMarketings.Count > 0)
            {
               // tf055.Text = priorAgencyMarketings[0].AgencyExpiredDate.ToString();
                tf055.Text = priorAgencyMarketings[0].AgencyExpiredDate != null ? (DateTime.Parse(priorAgencyMarketings[0].AgencyExpiredDate)).ToString("dd-MM-yyyy") : " ";
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf055.Text = ""; // Set a default value or handle it based on your requirements
            }
            tf055.FontSize = 8;
            tf055.Options = TextField.READ_ONLY | TextField.MULTILINE;
            writer.AddAnnotation(tf055.GetTextField());

            TextField tf056 = new TextField(writer, new Rectangle(250, 320, 110, 340), "field3");
            tf056.BorderColor = new BaseColor(232, 232, 232);
            tf056.BackgroundColor = new BaseColor(232, 232, 232);
            //  tf056.Text = priorAgencyMarketings[0].AgencyName1;
            if (priorAgencyMarketings.Count > 0)
            {
                tf056.Text = priorAgencyMarketings[0].AgencyName1;

            }
            else
            {
                // Handle the case when othercommentList is empty
                tf056.Text = ""; // Set a default value or handle it based on your requirements
            }
            tf056.FontSize = 8;
            tf056.Options = TextField.READ_ONLY | TextField.MULTILINE;
            writer.AddAnnotation(tf056.GetTextField());

            TextField tf057 = new TextField(writer, new Rectangle(520, 320, 330, 340), "field4");
            tf057.BorderColor = new BaseColor(232, 232, 232);
            tf057.BackgroundColor = new BaseColor(232, 232, 232);
            //  tf057.Text = priorAgencyMarketings[0].AgencyExpiredDate1.ToString();
            if (priorAgencyMarketings.Count > 0)
            {
              //  tf057.Text = priorAgencyMarketings[0].AgencyExpiredDate1.ToString();
                tf057.Text = priorAgencyMarketings[0].AgencyExpiredDate1 != null ? (DateTime.Parse(priorAgencyMarketings[0].AgencyExpiredDate1)).ToString("dd-MM-yyyy") : " ";

            }
            else
            {
                // Handle the case when othercommentList is empty
                tf057.Text = ""; // Set a default value or handle it based on your requirements
            }
            tf057.FontSize = 8;
            tf057.Options = TextField.READ_ONLY | TextField.MULTILINE;
            writer.AddAnnotation(tf057.GetTextField());

            //AddTextFieldToPDF(writer, doc, "Agency Name", priorAgencyMarketings[0].AgencyName, 250, 350, 110, 370);
            //AddTextFieldToPDF(writer, doc, " Agency Period:", priorAgencyMarketings[0].AgencyExpiredDate, 520, 350, 330, 370);
            //AddTextFieldToPDF(writer, doc, "Agency Name", priorAgencyMarketings[0].AgencyName1, 250, 320, 110, 340);
            //AddTextFieldToPDF(writer, doc, " Agency Period:", priorAgencyMarketings[0].AgencyExpiredDate1, 520, 320, 330, 340);
            formattedContent7.Add(c4);
            formattedContent7.Add(c5);
            formattedContent7.Add(c6);
            formattedContent7.Add(c7);
            formattedContent7.Add(c8);
            formattedContent7.Add(c9);
            formattedContent7.Add(c10);
            formattedContent7.Add(c11);
            formattedContent7.Add(c12);

            Phrase formattedContent8 = new Phrase();

            Chunk c13 = new Chunk("4. ADDITIONAL AUTHORITIES – SALE METHOD", titleFont);
            formattedContent8.Add(c13);

            Chunk c14 = new Chunk("\r\n(Select if applicable/not applicable for clause 4.1 and/or 4.2.)", colorFont);
            formattedContent8.Add(c14);
            // Add the formatted content
            doc.Add(new Paragraph(formattedContent));
            doc.Add(new Paragraph(formattedContent2));
            doc.Add(new Paragraph(formattedContent3));
            doc.Add(new Paragraph(formattedContent4));
            doc.Add(new Paragraph(formattedContent5));
            doc.Add(new Paragraph(formattedContent6));
            doc.Add(new Paragraph(formattedContent7));
            doc.Add(new Paragraph(formattedContent8));

            Phrase formattedContent9 = new Phrase();

            Chunk c15 = new Chunk("(4.1. Auction Authority)", colorFont);
            formattedContent9.Add(c15);

            string logoPath1 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "RadioChecked.png");
            iTextSharp.text.Image logo1 = iTextSharp.text.Image.GetInstance(logoPath1);
            if (System.IO.File.Exists(logoPath1))
            {
                logo1.ScaleAbsolute(15f, 15f);
                logo1.SetAbsolutePosition(150, 113);


                doc.Add(logo1);
            }
            Chunk c16 = new Chunk("                 Applicable", colorFont);
            formattedContent9.Add(c16);


            string logoPath2 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "RadioUnChecked.png");
            iTextSharp.text.Image logo2 = iTextSharp.text.Image.GetInstance(logoPath2);
            if (System.IO.File.Exists(logoPath2))
            {
                logo2.ScaleAbsolute(15f, 15f);
                logo2.SetAbsolutePosition(240, 113);


                doc.Add(logo2);
            }
            Chunk c17 = new Chunk("                  Not Applicable", colorFont);
            formattedContent9.Add(c17);

            doc.Add(new Paragraph(formattedContent9));

            Phrase formattedContent10 = new Phrase();

            Chunk c18 = new Chunk("The Client instructs the Agent to offer the Property for sale by public auction on the Auction Date specified below or as otherwise agreed. If the Property for sale by auction is subject to a reserve price, this must be notified to the Agent in writing prior to the auction. The auction of the Property shall be conducted on the terms and conditions contained in the Agent’s standard Particulars and Conditions of Sale of Real Estate by Auction as updated or amended. If the Property is sold at auction the Client authorises the", contentFont);
            formattedContent10.Add(c18);
            doc.Add(new Paragraph(formattedContent10));
            doc.Add(new Paragraph(""));


            doc.NewPage();
            doc.Add(t1);

            AddTextFieldToPDF(writer, doc, "Propertyfield", "Property Address", propertyAddress, 570, 765, 120, 785);
            Chunk c19 = new Chunk("Agent to sign on the Client’s behalf the agreement which forms part of the Agent’s standard Particulars and Conditions of Sale of Real Estate by Auction.", contentFont);
            Phrase formattedContent11 = new Phrase();
            formattedContent11.Add(c19);
            doc.Add(new Paragraph(formattedContent11));

            Phrase formattedContent12 = new Phrase();
            Chunk c20 = new Chunk("Auction Date:                                    " + "  Auction Time:  " + "                                           am/pm" + "                                              On-Site", contentFont);
            //AddTextFieldToPDF(writer, doc, "Auction Date:", "", 180, 720, 90, 700);

            TextField tf060 = new TextField(writer, new Rectangle(180, 715, 90, 700), "field1");
            tf060.BorderColor = new BaseColor(232, 232, 232);
            tf060.BackgroundColor = new BaseColor(232, 232, 232);
            //var dateTime = Convert.ToDateTime(methodOfSales[0].AuctionDate.ToString());
            //var dateValue2 = methodOfSales[0].AuctionDate.ToString();
            //  tf060.Text = ((DateTime)methodOfSales[0].AuctionDate).ToString("dd-MM-yyyy");

            if (methodOfSales.Count > 0)
            {
                tf060.Text = methodOfSales[0].AuctionDate !=null ? ((DateTime)methodOfSales[0].AuctionDate).ToString("dd-MM-yyyy") : " ";
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf060.Text = ""; // Set a default value or handle it based on your requirements
            }

            tf060.FontSize = 8;
            tf060.Options = TextField.READ_ONLY | TextField.MULTILINE;
            writer.AddAnnotation(tf060.GetTextField());

            formattedContent12.Add(c20);


            TextField tf061 = new TextField(writer, new Rectangle(240, 715, 350, 700), "field1");
            tf061.BorderColor = new BaseColor(232, 232, 232);
            tf061.BackgroundColor = new BaseColor(232, 232, 232);
            if (methodOfSales.Count > 0)
            {
               
                tf061.Text = methodOfSales[0].AuctionTime != null ? (DateTimeOffset.Parse(methodOfSales[0].AuctionTime).ToString("hh:mm tt") ): " " ;
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf061.Text = ""; // Set a default value or handle it based on your requirements
            }
            //  tf061.Text = DateTimeOffset.Parse(methodOfSales[0].AuctionTime).ToString("hh:mm tt");
            tf061.FontSize = 8;
            tf061.Options = TextField.READ_ONLY | TextField.MULTILINE;
            writer.AddAnnotation(tf061.GetTextField());

            //AddTextFieldToPDF(writer, doc, " Auction Time:", "", 240, 720, 350, 700);


            TextField tf062 = new TextField(writer, new Rectangle(385, 715, 450, 700), "field1");
            tf062.BorderColor = new BaseColor(232, 232, 232);
            tf062.BackgroundColor = new BaseColor(232, 232, 232);
            //tf062.Text = DateTimeOffset.Parse(methodOfSales[0].AuctionTime).ToString("tt");
            if (methodOfSales.Count > 0)
            {
                tf062.Text = methodOfSales[0].AuctionTime != null ? (DateTimeOffset.Parse(methodOfSales[0].AuctionTime).ToString("tt")) : " ";
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf062.Text = ""; // Set a default value or handle it based on your requirements
            }
            tf062.FontSize = 8;
            tf062.Options = TextField.READ_ONLY | TextField.MULTILINE;
            writer.AddAnnotation(tf062.GetTextField());

            //AddTextFieldToPDF(writer, doc, "am/pm", "", 385, 720, 450, 700);
            // AddCheckboxField(doc, writer, "myCheckbox", new Rectangle(485, 720, 475, 705), true, 475, 705);
            AddCheckboxField(doc, writer, "myCheckbox", new Rectangle(485, 715, 475, 705), true, 475, 705);
            doc.Add(new Paragraph(formattedContent12));



            Phrase formattedContent13 = new Phrase();
            Chunk c22 = new Chunk("\n" + "Venue:  ", contentFont);
            formattedContent13.Add(c22);


            TextField tf063 = new TextField(writer, new Rectangle(80, 665, 230, 687), "field1");
            tf063.BorderColor = new BaseColor(232, 232, 232);
            tf063.BackgroundColor = new BaseColor(232, 232, 232);
            // tf063.Text = methodOfSales[0].AuctionVenue.ToString();
            if (methodOfSales.Count > 0)
            {
                tf063.Text = methodOfSales[0].AuctionVenue.ToString();
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf063.Text = ""; // Set a default value or handle it based on your requirements
            }
            tf063.FontSize = 8;
            tf063.Options = TextField.READ_ONLY | TextField.MULTILINE;
            writer.AddAnnotation(tf063.GetTextField());


            //AddTextFieldToPDF(writer, doc, "Venue:", "", 80, 670, 230, 690);
            doc.Add(new Paragraph(formattedContent13));
            // doc.Add(new Paragraph(" "));
            Phrase formattedContent14 = new Phrase();
            Chunk c21 = new Chunk(" \n 4.2. Tender Authority" + "                        Applicable" + "                    Not Applicable", colorFont);
            formattedContent14.Add(c21);
            AddRadioField(doc, writer, "myRadio", new Rectangle(150, 640, 170, 655), true, 156, 645);
            AddRadioField(doc, writer, "myRadio", new Rectangle(350, 640, 170, 655), false, 156, 645);
            //AddRadioField(doc,writer, new Rectangle(100, 700, 120, 720), "radiogroup", "Yes");
            doc.Add(new Paragraph(formattedContent14));

            Phrase formattedContent15 = new Phrase();
            Chunk c23 = new Chunk("The Client instructs the Agent to offer the Property for sale by public tender with the public tender closing on the Tender Date specified below or as otherwise agreed. The terms and conditions of offer for sale by tender shall be contained in the Agent’s standard Particulars and Conditions of Sale by Tender as updated or amended.", contentFont);
            formattedContent15.Add(c23);

            Chunk c24 = new Chunk("\n Tender closes on:" + "                                    Auction Time: " + "                                               am / pm", contentFont);
            formattedContent15.Add(c24);

            TextField tf064 = new TextField(writer, new Rectangle(115, 575, 190, 590), "field1");
            tf064.BorderColor = new BaseColor(232, 232, 232);
            tf064.BackgroundColor = new BaseColor(232, 232, 232);
            if (methodOfSales.Count > 0)
            {
                tf064.Text = methodOfSales[0].TenderDate != null ? (((DateTime)methodOfSales[0].TenderDate).ToString("dd-MM-yyyy")) : " ";
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf064.Text = ""; // Set a default value or handle it based on your requirements
            }
            // tf064.Text = ((DateTime)methodOfSales[0].TenderDate).ToString("dd-MM-yyyy");
            tf064.FontSize = 8;
            tf064.Options = TextField.READ_ONLY | TextField.MULTILINE;
            writer.AddAnnotation(tf064.GetTextField());

            //AddTextFieldToPDF(writer, doc, " Tender closes on:", "", 115, 575, 190, 590);

            TextField tf065 = new TextField(writer, new Rectangle(260, 575, 360, 590), "field1");
            tf065.BorderColor = new BaseColor(232, 232, 232);
            tf065.BackgroundColor = new BaseColor(232, 232, 232);
            if (methodOfSales.Count > 0)
            {

                tf065.Text = methodOfSales[0].TenderDate != null ? (DateTimeOffset.Parse(methodOfSales[0].TenderTime).ToString("hh:mm tt")) : " ";
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf065.Text = "";
            }
            // tf065.Text = DateTimeOffset.Parse(methodOfSales[0].TenderTime).ToString("hh:mm tt");
            tf065.FontSize = 8;
            tf065.Options = TextField.READ_ONLY | TextField.MULTILINE;
            writer.AddAnnotation(tf065.GetTextField());
            //AddTextFieldToPDF(writer, doc, " Auction Time::", "", 260, 575, 360, 590);

            Chunk c25 = new Chunk("\n \n " + " Venue:  ", contentFont);
            formattedContent15.Add(c25);

            TextField tf066 = new TextField(writer, new Rectangle(80, 545, 230, 560), "field1");
            tf066.BorderColor = new BaseColor(232, 232, 232);
            tf066.BackgroundColor = new BaseColor(232, 232, 232);
            // tf066.Text = methodOfSales[0].TenderVenue.ToString();
            if (methodOfSales.Count > 0)
            {
                tf066.Text = methodOfSales[0].TenderVenue.ToString();
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf066.Text = "";
            }
            tf066.FontSize = 8;
            tf066.Options = TextField.READ_ONLY | TextField.MULTILINE;
            writer.AddAnnotation(tf066.GetTextField());

            // AddTextFieldToPDF(writer, doc, "Venue:", "", 80, 545, 230, 560);
            //AddTextFieldToPDF(writer, doc, "am/pm 1", "", 385, 570, 570, 620);
            doc.Add(new Paragraph(formattedContent15));


            Phrase formattedContent16 = new Phrase();
            Chunk c26 = new Chunk(" \n 4.3.Other (e.g. Deadline Sale Authority) – please specify \n", colorFont);
            TextField tf067 = new TextField(writer, new Rectangle(40, 480, 560, 510), "Deadline Sale Authority:");
            tf067.BorderColor = new BaseColor(232, 232, 232);
            tf067.BackgroundColor = new BaseColor(232, 232, 232);
            //tf067.Text = methodOfSales[0].TenderVenue.ToString();		
            tf067.FontSize = 8;
            tf067.Options = TextField.READ_ONLY | TextField.MULTILINE;
            writer.AddAnnotation(tf067.GetTextField());
            //AddTextFieldToPDF(writer, doc, "Deadline Sale Authority:", "", 40, 480, 560, 510);
            formattedContent16.Add(c26);
            doc.Add(new Paragraph(formattedContent16));
            //doc.Add(new Paragraph(""));
     



            Phrase formattedContent17 = new Phrase();
            Chunk c27 = new Chunk(" \n \n \n 5." + "      " + " MARKETING", titleFont);
            formattedContent17.Add(c27);
            doc.Add(new Paragraph(formattedContent17));
            //5.1 items
            Phrase formattedContent18 = new Phrase();
            Chunk c28 = new Chunk("(5.1) ", colorFont);
            Chunk c29 = new Chunk("The Agent has explained to the Client, and the Client acknowledges, that they are not obliged to agree to any advertising and            marketing expenses, however the Client agrees to and authorises the following:", contentFont);
            formattedContent18.Add(c28);
            formattedContent18.Add(c29);
            doc.Add(new Paragraph(formattedContent18));


            //5.1 a,b,c etc
            //5.1(a)
            Phrase formattedContent19 = new Phrase();
            Chunk c30 = new Chunk("        (a)", colorFont);
            Chunk c31 = new Chunk("after listing, the Agent to undertake the marketing of the Property in accordance with the attached Marketing Schedule; and", contentFont);
            formattedContent19.Add(c30);
            formattedContent19.Add(c31);
            doc.Add(new Paragraph(formattedContent19));
            //5.1(b)
            Phrase formattedContent20 = new Phrase();
            Chunk c32 = new Chunk("        (b)", colorFont);
            Chunk c33 = new Chunk("the Agent to spend up to the sum of $", contentFont);

            TextField tf059 = new TextField(writer, new Rectangle(300, 383, 220, 398), "field1");
            tf059.BorderColor = new BaseColor(232, 232, 232);
            tf059.BackgroundColor = new BaseColor(232, 232, 232);
            // tf059.Text = priorAgencyMarketings[0].AgencySum.ToString();
            if (priorAgencyMarketings.Count > 0)
            {
                tf059.Text = priorAgencyMarketings[0].AgencySum.ToString();
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf059.Text = "";
            }
            tf059.FontSize = 8;
            tf059.Options = TextField.READ_ONLY | TextField.MULTILINE;
            writer.AddAnnotation(tf059.GetTextField());
            //AddTextFieldToPDF(writer, doc, "c25", "", 300, 481, 340, 500);
            Chunk c34 = new Chunk("                                    " + "including GST on advertising, marketing and promoting the                Property on the Client’s behalf; and", contentFont);
            formattedContent20.Add(c32);
            formattedContent20.Add(c33);
            formattedContent20.Add(c34);
            doc.Add(new Paragraph(formattedContent20));

            //5.1(c)
            Phrase formattedContent21 = new Phrase();
            Chunk c35 = new Chunk("        (c)", colorFont);
            Chunk c36 = new Chunk("to reimburse the Agent upon demand for the amount spent under clause 5.1(b) and any subsequent agreed amount.", contentFont);
            formattedContent21.Add(c35);
            formattedContent21.Add(c36);
            doc.Add(new Paragraph(formattedContent21));
            doc.Add(new Paragraph(" "));


            //adding point 6
            Phrase formattedContent22 = new Phrase();
            Chunk c37 = new Chunk("6." + "      " + " PAYMENT OF COMMISSION", titleFont);
            formattedContent22.Add(c37);
            doc.Add(new Paragraph(formattedContent22));

            //6.1 items
            Phrase formattedContent23 = new Phrase();
            Chunk c38 = new Chunk("(6.1)", colorFont);
            Chunk c39 = new Chunk("The Client must pay the Agent the commission, on the terms set out in this Agreement, if:", contentFont);
            formattedContent23.Add(c38);
            formattedContent23.Add(c39);
            doc.Add(new Paragraph(formattedContent23));
            //6.1 a,b,c etc

            //6.1(a)
            Phrase formattedContent24 = new Phrase();
            Chunk c40 = new Chunk("       (a)", colorFont);
            Chunk c41 = new Chunk(" in the case of a sole agency, the Client enters into an agreement to sell or exchange the Property (or part of it) at any time               during the term of the agency and the agreement is or becomes unconditional (whether during or after the term of the                         agency);or", contentFont);
            formattedContent24.Add(c40);
            formattedContent24.Add(c41);
            doc.Add(new Paragraph(formattedContent24));

            //6.1(b)

            Phrase formattedContent25 = new Phrase();
            Chunk c42 = new Chunk("       (b)", colorFont);
            Chunk c43 = new Chunk(" in the case of a general agency, the Client enters into an agreement to sell or exchange the Property (or part of it) at any                   time during the term of the agency,through the instrumentality of the Agent or to a purchaser introduced by the Agent and                  the agreement is or becomes unconditional (whether during or after the term of the agency);or", contentFont);
            formattedContent25.Add(c42);
            formattedContent25.Add(c43);
            doc.Add(new Paragraph(formattedContent25));

            //6.1(c)
            Phrase formattedContent26 = new Phrase();
            Chunk c44 = new Chunk("       (c)", colorFont);
            Chunk c45 = new Chunk("in the case of either a sole or general agency, the Client enters into a Private Agreement to sell or exchange the Property (or              spart of it) within a period of six months following the date of expiry, cancellation or termination of the agency, through the                 instrumentality of the Agent or to a purchaser introduced by the Agent, and the agreement is or becomes unconditional                      (whether during or after the six month period). In this sub-clause “Private Agreement” means any agreement to sell or                        exchange the Property (or part of it) in the absence of any agency agreement between the Client and a real estate agent                   holding a licence under the REAA 2008.", contentFont);
            formattedContent26.Add(c44);
            formattedContent26.Add(c45);
            doc.Add(new Paragraph(formattedContent26));

            //6.2 items
            Phrase formattedContent27 = new Phrase();
            Chunk c46 = new Chunk("(6.2)", colorFont);
            Chunk c47 = new Chunk("Unless otherwise stated the commission will become payable immediately upon the agreement for the sale or other                           disposal of the Property becoming unconditional. The Client shall instruct their solicitor to advise the Agent as soon as                       practicable on the agreement becoming unconditional.", contentFont);
            formattedContent27.Add(c46);
            formattedContent27.Add(c47);
            doc.Add(new Paragraph(formattedContent27));

            //doc.Add(new Paragraph(" "));

            doc.NewPage();

            doc.Add(t1);
            AddTextFieldToPDF(writer, doc, "Propertyfield", "Property Address", propertyAddress, 570, 765, 120, 785);

            //7th point
            Phrase formattedContent28 = new Phrase();
            Chunk c48 = new Chunk("7." + "      " + " CALCULATION OF COMMISSION", titleFont);
            formattedContent28.Add(c48);
            doc.Add(new Paragraph(formattedContent28));

            //7.1
            Phrase formattedContent29 = new Phrase();
            Chunk c49 = new Chunk("(7.1)", colorFont);
            Chunk c50 = new Chunk("The commission is calculated on the purchase price shown on the sale and purchase agreement as follows (all amounts plus              goods and services tax (GST)):", contentFont);
            formattedContent29.Add(c49);
            formattedContent29.Add(c50);
            doc.Add(new Paragraph(formattedContent29));

            //7.1(a)
            Phrase formattedContent30 = new Phrase();
            Chunk c51 = new Chunk("        (a)", colorFont);
            Chunk c52 = new Chunk("Firstly, a fee of $                                     ,", contentFont);
            formattedContent30.Add(c51);
            formattedContent30.Add(c52);
            AddTextFieldToPDF(writer, doc, " Firstly", "", "", 220, 690, 140, 705);
            doc.Add(new Paragraph(formattedContent30));

            Phrase formattedContent31 = new Phrase();
            Chunk c53 = new Chunk("            secondly on the first $" + "                                   of the purchase price " + "                                      %, ", contentFont);
            AddTextFieldToPDF(writer, doc, "secondly", "", "", 240, 670, 160, 685);
            formattedContent31.Add(c53);
            //formattedContent31.Add(c54);
            AddTextFieldToPDF(writer, doc, "secondly1", "", "", 420, 670, 330, 685);
            doc.Add(new Paragraph(formattedContent31));

            Phrase formattedContent32 = new Phrase();
            Chunk c54 = new Chunk("            thirdly on the balance of the purchase price" + "                               % with a minimum commission of $" + "                                    ,", contentFont);
            AddTextFieldToPDF(writer, doc, "thirdly", "", "", 310, 655, 240, 668);
            formattedContent32.Add(c54);
            //formattedContent31.Add(c54);
            AddTextFieldToPDF(writer, doc, "secondly1", "", "", 550, 655, 460, 668);
            doc.Add(new Paragraph(formattedContent32));


            Phrase formattedContent33 = new Phrase();
            Chunk c55 = new Chunk("            fourthly in the case of leasehold property, a further one third of the total commission. \n            The Client shall pay the applicable GST.", contentFont);
            formattedContent33.Add(c55);
            doc.Add(new Paragraph(formattedContent33));

            Phrase formattedContent34 = new Phrase();
            Chunk c56 = new Chunk("(7.2)", colorFont);
            Chunk c57 = new Chunk("    For example, based upon (tick one         ):", contentFont);
            AddCheckboxField(doc, writer, "myCheckbox", new Rectangle(210, 610, 220, 620), true, 210, 610);
            formattedContent34.Add(c56);
            formattedContent34.Add(c57);

            AddCheckboxField(doc, writer, "myCheckbox", new Rectangle(70, 605, 80, 595), true, 70, 595);
            Chunk c58 = new Chunk("  \n                     the appraised value, or", contentFont);
            formattedContent34.Add(c58);
            AddCheckboxField(doc, writer, "myCheckbox", new Rectangle(70, 590, 80, 580), true, 70, 580);
            Chunk c59 = new Chunk("  \n                     the Client’s asking price (where an appraisal was not possible to be given),", contentFont);
            formattedContent34.Add(c59);

            Chunk c60 = new Chunk("  \n                     a sale price of $ " + "                           would mean an estimated commission of $" + "                          inclusive of GST.", contentFont);
            formattedContent34.Add(c60);
            AddTextFieldToPDF(writer, doc, "sale", "", "", 160, 560, 220, 575);
            AddTextFieldToPDF(writer, doc, "sale", "", "", 400, 560, 450, 575);
            doc.Add(new Paragraph(formattedContent34));

            doc.Add(new Paragraph(" "));
            //8th point
            Phrase formattedContent35 = new Phrase();
            Chunk c61 = new Chunk("8." + "      " + " DEPOSIT", titleFont);
            formattedContent35.Add(c61);
            doc.Add(new Paragraph(formattedContent35));

            //8.1
            Phrase formattedContent36 = new Phrase();
            Chunk c62 = new Chunk("(8.1)", colorFont);
            Chunk c63 = new Chunk("  The Client acknowledges and agrees:", contentFont);
            formattedContent36.Add(c62);
            formattedContent36.Add(c63);
            doc.Add(new Paragraph(formattedContent36));


            //8.1(a)
            Phrase formattedContent37 = new Phrase();
            Chunk c64 = new Chunk("        (a)", colorFont);
            Chunk c65 = new Chunk(" the Agent is entitled to receive a deposit on the Client’s behalf, to be held by the agent as a stakeholder;", contentFont);
            Chunk c66 = new Chunk("     \n        (b)", colorFont);
            Chunk c67 = new Chunk("  the Client will specify in any agreement for sale and purchase that may be entered into in accordance with the authority                     under this agreement that the deposit is to be paid to the trust account of the Agent.", contentFont);
            Chunk c68 = new Chunk("     \n        (c)", colorFont);
            Chunk c69 = new Chunk("  the Agent is entitled to deduct its commission and expenses from the deposit, subject to the requirements of section 123                   of the Real Estate Agents Act 2008. This provision requires the Agent to hold the deposit for not less than 10 working                         days, and to continue to hold the deposit if the Agent receives a written notice of requisition or objection in respect of the                    title to any land affected by the transaction, in the absence of a Court order or written authority signed by all the parties to                  the transaction ordering or authorising the release of the deposit). Where the Property being sold is a unit title the Client                     agrees that this deduction will be delayed until completion of the obligations under sections 147 and 148 of the Unit                            Titles Act 2010; and ", contentFont);

            formattedContent37.Add(c64);
            formattedContent37.Add(c65);
            formattedContent37.Add(c66);
            formattedContent37.Add(c67);
            formattedContent37.Add(c68);
            formattedContent37.Add(c69);

            doc.Add(new Paragraph(formattedContent37));

            doc.Add(new Paragraph(" "));
            //9th point
            Phrase formattedContent38 = new Phrase();
            Chunk c70 = new Chunk("9." + "      " + " REFERRALS", titleFont);
            formattedContent38.Add(c70);
            doc.Add(new Paragraph(formattedContent38));

            //9.1
            Phrase formattedContent39 = new Phrase();
            Chunk c71 = new Chunk("(9.1)", colorFont);
            Chunk c72 = new Chunk("  The Client agrees that the Agent may receive a commission (as defined in section 4 of the REAA 2008) from the provider of               any related service (for example, but not limited to Mortgage Express/Insurance Express or a Harcourts Business Partner) in             the event of a referral.", contentFont);
            formattedContent39.Add(c71);
            formattedContent39.Add(c72);
            doc.Add(new Paragraph(formattedContent39));

            doc.Add(new Paragraph(" "));
            //10th point
            Phrase formattedContent40 = new Phrase();
            Chunk c73 = new Chunk("10." + "      " + " AGENT’S STATEMENT RELATING TO REBATES, DISCOUNTS, & COMMISSIONS", titleFont);
            Chunk c74 = new Chunk(" (Delete clause 10.1(a) or 10.1(b) as            applicable.)", colorFont);
            formattedContent40.Add(c73);
            formattedContent40.Add(c74);
            doc.Add(new Paragraph(formattedContent40));

            //10.1
            Phrase formattedContent41 = new Phrase();
            Chunk c75 = new Chunk("(10.1)", colorFont);
            Chunk c76 = new Chunk("   I, the Agent, confirm that, in relation to any expenses for, or in connection with, any real estate agency work carried out by                   me for the Client(s) in connection with the transaction covered by this Agreement:", contentFont);
            formattedContent41.Add(c75);
            formattedContent41.Add(c76);
            doc.Add(new Paragraph(formattedContent41));

            //10.1(a) (b)
            Phrase formattedContent42 = new Phrase();
            Chunk c77 = new Chunk("        (a)", colorFont);
            Chunk c78 = new Chunk(" I will not receive, and am not entitled to receive, any rebates, discounts, or commissions; or", contentFont);
            Chunk c79 = new Chunk("     \n        (b)", colorFont);
            Chunk c80 = new Chunk("  I will receive, or am entitled to receive, the rebates, discounts, and commissions specified below.", contentFont);
            formattedContent42.Add(c77);
            formattedContent42.Add(c78);
            formattedContent42.Add(c79);
            formattedContent42.Add(c80);
            doc.Add(new Paragraph(formattedContent42));

            //10.1
            Phrase formattedContent43 = new Phrase();
            Chunk c81 = new Chunk("(10.2)", colorFont);
            Chunk c82 = new Chunk("   If you selected clause (b) above, provide the specified details for each rebate, discount, or commission $ (including GST) in              the table below. ", contentFont);
            formattedContent43.Add(c81);
            formattedContent43.Add(c82);
            doc.Add(new Paragraph(formattedContent43));

            Phrase formattedContent44 = new Phrase();
            Chunk Commented11Chunk = new Chunk("              Estimates must be clearly marked as such. Estimates may change.", boldFont);
            formattedContent44.Add(Commented11Chunk);
            doc.Add(new Paragraph(formattedContent44));

            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.NewPage();
            doc.Add(t1);

            doc.Add(new Paragraph(" "));

            AddTextFieldToPDF(writer, doc, "Propertyfield", "Property Address", propertyAddress, 570, 765, 120, 785);

            PdfPTable tableEstimates = new PdfPTable(4);
            tableEstimates.WidthPercentage = 100;


            float[] widths = new float[] { 60f, 60f, 60f, 20f };
            tableEstimates.DefaultCell.BackgroundColor = new BaseColor(220, 220, 220);
            tableEstimates.DefaultCell.BorderColor = BaseColor.DARK_GRAY;


            tableEstimates.SetWidths(widths);
            BaseColor headerBackgroundColor = BaseColor.BLACK; // Light Gray
            BaseColor rowBackgroundColor = new BaseColor(240, 240, 240); // Lighter Gray
                                                                         // Add headers to the table with background color

            string[] headers = { "Expenses to be incurred", "Provider of rebate, discount or \r\ncommission $ (including GST)", "Provider of rebate, discount or \r\ncommission $ (including GST)", "Tick here\r\nif estimate" };
            foreach (string header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.WHITE)));
                cell.BackgroundColor = headerBackgroundColor;
                cell.FixedHeight = 30f;
                tableEstimates.AddCell(cell);
            }
            // Replace this with your database retrieval logic
            //List<TableRowData> dynamicDataList = GetDataFromDatabase();
            //string[,] data = {
            //                    { " ", " ", " ", " " },
            //                    { " ", " ", " ", " " },
            //                    { " ", " ", " ", " " },
            //                    { " ", " ", " ", " " },
            //                    { " ", " ", " ", " " },
            //                    { " ", " ", " ", " " },
            //               };

            int rows = 6; // Adjust this based on the number of rows you need
            int cols = 4;

            string[,] data3 = new string[rows, cols];

            //string[,] data = {
            //                    { estimates[0].ExpensesToBeIncurred, estimates[0].ProviderDiscountCommission.ToString(), estimates[0].AmountDiscountCommission.ToString(), estimates[0].TickHereIfEstimate.ToString() },
            //                    {  estimates[1].ExpensesToBeIncurred, estimates[1].ProviderDiscountCommission.ToString(), estimates[1].AmountDiscountCommission.ToString(), estimates[1].TickHereIfEstimate.ToString()  },
            //                    { estimates[2].ExpensesToBeIncurred, estimates[2].ProviderDiscountCommission.ToString(), estimates[2].AmountDiscountCommission.ToString(), estimates[2].TickHereIfEstimate.ToString()  },
            //                    { estimates[3].ExpensesToBeIncurred, " ", " ", " " },
            //                    { " ", " ", " ", " " },
            //                    { " ", " ", " ", " " },
            //               };

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    PdfPCell cell;

                    if (j == 3) // Check if it's the 4th column
                    {
                        bool ischecked;
                        // select file name on basis of boolean value
                        //string imageFileName = isSelected ? "RadioChecked.png" : "RadioUnchecked.png";
                        string value1 = "true";
                        if (i < estimates.Count)
                        {
                            Estimates estimate = estimates[i];
                            ischecked = estimate.TickHereIfEstimate.ToString().Contains("True") ? true : false;
                            //ischecked
                        }
                        else
                        {
                            ischecked = false;

                        }

                        //replace image with file name;
                        //string imagePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "imageFileName");

                        string imagePath1 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", ischecked ? "checked.png" : "unchecked.png");
                        Image checkboxImage = Image.GetInstance(imagePath1);

                        // Adjust the checkbox image size as needed
                        checkboxImage.ScaleAbsolute(10f, 10f);

                        // Add the checkbox image to the cell
                        cell = new PdfPCell(checkboxImage);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingTop = 8f;
                        cell.BackgroundColor = BaseColor.WHITE;
                    }
                    else
                    {
                        // Normal cell with text
                        string value = " ";

                        if (i < estimates.Count)
                        {
                            Estimates estimate = estimates[i];

                            if (j == 0) value = estimate.ExpensesToBeIncurred;
                            else if (j == 1) value = estimate.ProviderDiscountCommission.ToString();
                            else if (j == 2) value = estimate.AmountDiscountCommission.ToString();
                            // else if (j == 3) value = estimate.TickHereIfEstimate.ToString();
                        }

                        data3[i, j] = value;
                        cell = new PdfPCell(new Phrase(data3[i, j], new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.BLACK)));
                        cell.BackgroundColor = rowBackgroundColor;

                    }
                    cell.FixedHeight = 30f;
                    tableEstimates.AddCell(cell);

                }
            }

            //for (int i = 0; i < data.GetLength(0); i++)
            //{
            //    for (int j = 0; j < data.GetLength(1); j++)
            //    {

            //        PdfPCell cell = new PdfPCell(new Phrase(data[i, j], new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.BLACK)));
            //        cell.BackgroundColor = rowBackgroundColor;
            //        cell.FixedHeight = 30f;
            //        tableEstimates.AddCell(cell);


            //    }
            //}
            doc.Add(tableEstimates);
            //AddCheckboxField(doc, writer, "myCheckbox0", new Rectangle(520, 700, 530, 690), true, 520, 690);
            //AddCheckboxField(doc, writer, "myCheckbox1", new Rectangle(520, 670, 530, 660), true, 520, 660);
            //AddCheckboxField(doc, writer, "myCheckbox2", new Rectangle(520, 640, 530, 630), true, 520, 630);
            //AddCheckboxField(doc, writer, "myCheckbox3", new Rectangle(520, 610, 530, 600), true, 520, 600);
            doc.Add(new Paragraph(" "));

            Phrase formattedContent45 = new Phrase();
            Chunk c84 = new Chunk(" Date:" + "                                Agent to sign here:", contentFont);
            Chunk c85 = new Chunk("\n Note: Expenses means any sum or reimbursement for expenses or charges incurred in connection with services provided by an         agent in the capacity of agent.", contentFont);
            formattedContent45.Add(c84);
            formattedContent45.Add(c85);
            AddTextFieldToPDF(writer, doc, "Date", "", "", 60, 505, 135, 490);
            AddTextFieldToPDF(writer, doc, "Date", "", "", 270, 505, 215, 490);
            doc.Add(new Paragraph(formattedContent45));

            doc.Add(new Paragraph(" "));
            //11th point
            Phrase formattedContent46 = new Phrase();
            Chunk c86 = new Chunk("11." + "      " + " CLIENT WARRANTIES", titleFont);
            Chunk c87 = new Chunk(" (Delete any warranties that are not applicable.)", colorFont);
            formattedContent46.Add(c86);
            formattedContent46.Add(c87);
            doc.Add(new Paragraph(formattedContent46));

            doc.Add(new Paragraph(" "));

            PdfPTable tableClientWarranties = new PdfPTable(1);
            tableClientWarranties.WidthPercentage = 100;


            // float[] widths1 = new float[] { 100f };
            tableClientWarranties.DefaultCell.BackgroundColor = new BaseColor(220, 220, 220);
            tableClientWarranties.DefaultCell.BorderColor = BaseColor.DARK_GRAY;
            // tableClientWarranties.SetWidths(widths1);
            BaseColor headerBackgroundColor1 = BaseColor.BLACK; // Light Gray

            // Add headers to the table with background color


            string[] headers1 = { "The Client warrants that as at the date of this Agreement:" };
            foreach (string header in headers1)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.WHITE)));
                cell.BackgroundColor = headerBackgroundColor1;
                cell.FixedHeight = 30f;
                tableClientWarranties.AddCell(cell);
            }

            string gstno = null; // Default value is null

            if (clientDetails != null && clientDetails.Count > 0)
            {
                gstno = clientDetails[0].GSTNumber != null ? clientDetails[0].GSTNumber.ToString() : null;
            }

           



        //    var gstno = clientDetails[0].GSTNumber != null ? clientDetails[0].GSTNumber.ToString() : null;
         
           

            //checkbox in rows
           

            AddTextFieldToPDF(writer, doc, "gstno", "", gstno, 100, 335, 175, 325);
            string[,] data1 = {
                                { "they have made proper enquiries about the Property and the information provided in this Agreement and the Client/Property Information pages are complete, true and \r\ncorrect;  " },
                                { "they are registered under the Goods & Services Tax Act 1985 in respect of the Property (tick one      ).      YES         NO \n GST Number:  " },
                                { "to the best of their knowledge the Property is not “contaminated” as that term is used in the Resource Management Act 1991;  " },
                                { "the Property is not, and has not been, used for the manufacture of methamphetamine, to the Client’s knowledge, or been subject to methamphetamine contamination \r\nknown to the Client, other than as may be identified in this Agreement" },
                                { "the Property is not subject to any defects or hazards (including the use of asbestos and Dux Quest plumbing), requisitions, outstanding requirements or notices from \r\nany party (e.g. from any Council, territorial authority, government authority or any other party) other than those identified in this Agreement, if any " },
                                { "they have not given any consent or waiver to a neighbour in respect of any development or work proposed by that neighbour on a neighbouring property, nor is the \r\nClient aware of any application by a neighbour for a consent to develop a neighbouring property which would reasonably be expected to adversely affect the Property; " },
                                { "where the Property is sold subject to a residential tenancy, they have complied with the requirements of the Residential Tenancies (Smoke Alarms and Insulation) \r\nRegulations 2016 and the Residential Tenancies (Healthy Homes Standards) Regulations 2019, and the Property meets the requirements of that legislation; " },
                                { "the Property is not, and has not been, subject to any weathertightness issues known to the Client other than as may be identified in this Agreement; " },
                                { "they have not received funding assistance in the form of a loan from any territorial authority or other service provider in relation to the installation of a heating device \r\nand/or insulation on the Property and further warrants that if they become aware of any matter to the contrary the Client will immediately inform the Agent. The Client \r\nacknowledges that they have a legal obligation to repay the balance of any loan to that territorial authority or other service provider on or prior to settlement and will \r\ninstruct their solicitor accordingly; " },
                                { "the person(s) signing this Agreement has the full authority of the registered owner(s) of the Property to enter into and to sign this Agreement " },
                                { "in the event new information arises that may affect these warranties, the Client shall immediately advise the Agent in writing.\r\n " },


                           };

            for (int i = 0; i < data1.GetLength(0); i++)
            {
                for (int j = 0; j < data1.GetLength(1); j++)
                {

                    PdfPCell cell = new PdfPCell(new Phrase(data1[i, j], new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.BackgroundColor = BaseColor.WHITE;
                    cell.FixedHeight = 30f;
                    cell.Padding = 3f;
                    tableClientWarranties.AddCell(cell);


                }
            }
            doc.Add(tableClientWarranties);
            AddCheckboxField(doc, writer, "myCheckbox", new Rectangle(425, 345, 435, 335), false, 425, 335);
            AddCheckboxField(doc, writer, "myCheckbox", new Rectangle(445, 345, 455, 335), isRegistered, 445, 335);
            AddCheckboxField(doc, writer, "myCheckbox", new Rectangle(480, 345, 490, 335), isNotregistered, 480, 335);

            doc.Add(t1);
            //doc.Add(new Paragraph(" "));
            //doc.Add(new Paragraph(" "));
            //doc.Add(new Paragraph(" "));

            AddTextFieldToPDF(writer, doc, "Propertyfield", "Property Address", propertyAddress, 570, 765, 120, 785);

            //12th point
            Phrase formattedContent47 = new Phrase();
            Chunk c88 = new Chunk("12." + "      " + "HEALTH AND SAFETY", titleFont);
            formattedContent47.Add(c88);
            doc.Add(new Paragraph(formattedContent47));

            //12.1
            Phrase formattedContent48 = new Phrase();
            Chunk c89 = new Chunk("(12.1)", colorFont);
            Chunk c90 = new Chunk("   The Client acknowledges and understands that the Agent has obligations under the Health and Safety at Work Act 2015 (", contentFont);
            Chunk Commented48Chunk = new Chunk("                 HSWA 2015", boldFont);
            Chunk c91 = new Chunk(" ) to ensure the health and safetyof workers (including employees, contractors, and employees of contractors)                and the general public so far as is reasonably practicable while undertaking work in relation to the sale and purchase or                     other disposal of the Property in accordance with this Agreement.", contentFont);
            formattedContent48.Add(c89);
            formattedContent48.Add(c90);
            formattedContent48.Add(Commented48Chunk);
            formattedContent48.Add(c91);
            doc.Add(new Paragraph(formattedContent48));

            //12.2
            Phrase formattedContent49 = new Phrase();
            Chunk c92 = new Chunk("(12.2)", colorFont);
            Chunk c93 = new Chunk("  In circumstances where the Client is a ‘person conducting a business or undertaking’ (as that term is defined in the HSWA                2015) the client must:", contentFont);
            formattedContent49.Add(c92);
            formattedContent49.Add(c93);
            doc.Add(new Paragraph(formattedContent49));

            Phrase formattedContent50 = new Phrase();
            Chunk c95 = new Chunk("        (a)", colorFont);
            Chunk c96 = new Chunk(" comply with their obligations under the HSWA 2015 (and supporting regulations) at all times during the continuation of this                Agreement; and", contentFont);
            Chunk c97 = new Chunk("     \n        (b)", colorFont);
            Chunk c98 = new Chunk("  consult, cooperate, and coordinate activities with the Agent and any other relevant party in respect of any work undertaken                 in relation to the sale or other disposal of the Property so as to ensure that all parties understand the nature of the work,                   the risks arising from the work, and the controls to be implemented to mitigate those risks so far as is reasonably                                practicable, and to enable the Client and the Agent to verify that the risks are being controlled and the work is being                           performed safely and in accordance with this Agreement.", contentFont);
            formattedContent50.Add(c95);
            formattedContent50.Add(c96);
            formattedContent50.Add(c97);
            formattedContent50.Add(c98);
            doc.Add(new Paragraph(formattedContent50));


            //13th point
            Phrase formattedContent51 = new Phrase();
            Chunk c99 = new Chunk("13." + "      " + "LIST OF PROPERTY HAZARDS OR RISKS OR HAZARDOUS SUBSTANCES", titleFont);
            formattedContent51.Add(c99);
            doc.Add(new Paragraph(formattedContent51));

            //13.1
            Phrase formattedContent52 = new Phrase();
            Chunk c100 = new Chunk("(13.1)", colorFont);
            Chunk c101 = new Chunk("  The Client will assist the Agent in the preparation of a list of hazards or risks or hazardous substances that may be at the                   Property or affect the Property.", contentFont);
            Chunk c102 = new Chunk(" \n (13.2)", colorFont);
            Chunk c103 = new Chunk("  The client will comply with any reasonable instructions given by the Agent about actions required to be taken to address                     any identified hazards or risks at the Property in order to ensure the health and safety of people vising the Property at the                  request or invitation of the Agent. ", contentFont);
            Chunk c104 = new Chunk(" \n (13.3)", colorFont);
            Chunk c105 = new Chunk("  The Client acknowledges that the Agent will not be able to conduct any open homes or allow potential purchasers to view                   the Property until the list of hazards or risks or hazardous substances affecting the Property has been prepared by the                      Agent and agreed by the Client.", contentFont);
            formattedContent52.Add(c100);
            formattedContent52.Add(c101);
            formattedContent52.Add(c102);
            formattedContent52.Add(c103);
            formattedContent52.Add(c104);
            formattedContent52.Add(c105);

            formattedContent48.Add(c91);
            doc.Add(new Paragraph(formattedContent52));

            doc.Add(new Paragraph(" "));

            //14th point
            Phrase formattedContent53 = new Phrase();
            Chunk c106 = new Chunk("14." + "      " + "USE OF MATERIALS", titleFont);
            formattedContent53.Add(c106);
            doc.Add(new Paragraph(formattedContent53));

            //14.1 14.2
            Phrase formattedContent54 = new Phrase();
            Chunk c107 = new Chunk("(14.1)", colorFont);
            Chunk c108 = new Chunk("  Any photographs taken of the Property in accordance with the Marketing Schedule and used in any display materials are for              the purpose of advertising, marketing and promoting the Property. The Client authorises the Agent to use all such                              photographs and/or display materials for this purpose. The Client agrees that the photographs and display materials may be              subsequently used by the Agent for purposes relating to the promotion of the Agent or the Agency and the Client hereby                   provides a waiver to the Agent and the Agency in relation to the collection and use of them under the Privacy Act 2020.", contentFont);
            Chunk c109 = new Chunk(" \n (14.2)", colorFont);
            Chunk c110 = new Chunk(" The waiver given in clause 14.1 may be revoked by the Client giving the Agent written notice in accordance with clause 21.               Such revocation shall be effective immediately upon receipt of such notice by the Agent.", contentFont);
            formattedContent54.Add(c107);
            formattedContent54.Add(c108);
            formattedContent54.Add(c109);
            formattedContent54.Add(c110);
            doc.Add(new Paragraph(formattedContent54));

            doc.Add(new Paragraph(" "));

            //15th point
            Phrase formattedContent55 = new Phrase();
            Chunk c111 = new Chunk("15." + "      " + "DISCLOSURE OF INFORMATION", titleFont);
            formattedContent55.Add(c111);
            doc.Add(new Paragraph(formattedContent55));

            //15.1
            Phrase formattedContent56 = new Phrase();
            Chunk c112 = new Chunk("(15.1)", colorFont);
            Chunk c113 = new Chunk("  The Client acknowledges that the Agent is required under the Real Estate Agents Act (Professional Conduct and Client                      Care) Rules 2012 (", contentFont);
            Chunk commented56chunk = new Chunk(" Rules ", boldFont);
            Chunk c118 = new Chunk(" ) to disclose known defects affecting the Property to purchasers or potential purchasers and not                to withhold information that should by law or in fairness be provided to purchasers or potential purchasers. The Client also                  acknowledges that where it would appear likely that the Property may be subject to hidden or underlying defects, then the                 Agent is required to either:", contentFont);
            formattedContent56.Add(c112);
            formattedContent56.Add(c113);
            formattedContent56.Add(commented56chunk);
            formattedContent56.Add(c118);
            doc.Add(new Paragraph(formattedContent56));

            //15.1 (a) (b)
            Phrase formattedContent57 = new Phrase();
            Chunk c114 = new Chunk("        (a)", colorFont);
            Chunk c115 = new Chunk(" obtain confirmation from the Client, supported by evidence or expert advice, that the Property is not subject to defect; or", contentFont);
            Chunk c116 = new Chunk("     \n        (b)", colorFont);
            Chunk c117 = new Chunk(" ensure that purchasers or potential purchasers are informed of any significant potential risk so that they can seek expert                     advice if they so choose.", contentFont);
            formattedContent57.Add(c114);
            formattedContent57.Add(c115);
            formattedContent57.Add(c116);
            formattedContent57.Add(c117);
            doc.Add(new Paragraph(formattedContent57));

            doc.Add(new Paragraph(" "));
            doc.Add(t1);
            AddTextFieldToPDF(writer, doc, "Propertyfield", "Property Address", propertyAddress, 570, 765, 120, 785);
            doc.Add(new Paragraph(" "));

            Phrase formattedContent58 = new Phrase();
            Chunk c119 = new Chunk("(15.2)", colorFont);
            Chunk c120 = new Chunk("   If the Agent is unable to obtain confirmation under clause 15.1.(a), the Agent will inform purchasers and potential                               purchasers of any significant potential risk identified by the Agent consistent with rule 10.7(b) of the Rules. The Client                        authorises the Agent to make this disclosure.", contentFont);
            Chunk c121 = new Chunk(" \n (15.3)", colorFont);
            Chunk c122 = new Chunk("  If at any time during the term of the agency established by this Agreement the Client directs the Agent not to disclose to                    purchasers or potential purchasers any known defects or any significant potential risks for hidden or underlying defects                      identified by the Agent contrary to the terms of this Agreement or to the Rules, the Agent may then cancel this Agreement                  by written notice to the Client in accordance with clause 21. Cancellation shall be effective immediately upon receipt of such              notice.", contentFont);
            formattedContent58.Add(c119);
            formattedContent58.Add(c120);
            formattedContent58.Add(c121);
            formattedContent58.Add(c122);
            doc.Add(new Paragraph(formattedContent58));

            doc.Add(new Paragraph(" "));

            //16th point
            Phrase formattedContent59 = new Phrase();
            Chunk c123 = new Chunk("16." + "      " + "ADDITIONAL DISCLOSURES RELATING TO THE PROPERTY AND/OR THE LAND", titleFont);
            formattedContent59.Add(c123);
            doc.Add(new Paragraph(formattedContent59));

            TextField tf0054 = new TextField(writer, new Rectangle(40, 565, 555, 345), "field");
            tf0054.BorderColor = new BaseColor(232, 232, 232);
            tf0054.BackgroundColor = new BaseColor(232, 232, 232);
            //tf0054.Text = estimatesDetails[0].AdditionalDisclosures;

            if (estimatesDetails.Count > 0)
            {
                tf0054.Text = estimatesDetails[0].AdditionalDisclosures;
            }
            else
            {
                // Handle the case when othercommentList is empty
                tf0054.Text = ""; // Set a default value or handle it based on your requirements
            }


            tf0054.FontSize = 8;
            tf0054.Options = TextField.READ_ONLY | TextField.MULTILINE;
            writer.AddAnnotation(tf0054.GetTextField());

            //AddTextFieldToPDF(writer, doc, "field", additionaldisclosure[0].AdditionalDisclosures, 40, 565, 555, 345);


            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));

            //17th point
            Phrase formattedContent60 = new Phrase();
            Chunk c124 = new Chunk("17." + "      " + "AUTHORITY TO USE PROPERTY INFORMATION", titleFont);
            formattedContent60.Add(c124);
            doc.Add(new Paragraph(formattedContent60));

            //17.1
            Phrase formattedContent61 = new Phrase();
            Chunk c125 = new Chunk("(17.1)", colorFont);
            Chunk c126 = new Chunk("  The Agent is committed to compliance with all applicable laws, including privacy and copyright laws. The Client confirms                    that it has obtained all necessary authorisations (including under privacy law) to allow the collection, storage, use and                        disclosure of information (including information about an identifiable individual (", contentFont);
            Chunk commented61chunk = new Chunk("Personal Information", boldFont);
            Chunk c127 = new Chunk(") pertaining to the                       Property for the purposes of:", contentFont);
            formattedContent61.Add(c125);
            formattedContent61.Add(c126);
            formattedContent61.Add(commented61chunk);
            formattedContent61.Add(c127);
            doc.Add(new Paragraph(formattedContent61));


            //17.1 (a) (b)
            Phrase formattedContent62 = new Phrase();
            Chunk c128 = new Chunk("        (a)", colorFont);
            Chunk c129 = new Chunk(" the Agent’s marketing and promotional activities;", contentFont);
            Chunk c130 = new Chunk("     \n        (b)", colorFont);
            Chunk c131 = new Chunk(" listing the Property on real estate and property listing websites (including the Agent’s website and third party websites);", contentFont);
            Chunk c132 = new Chunk("     \n        (c)", colorFont);
            Chunk c133 = new Chunk(" collating and sharing property information for research, reports, statistical analysis, and other purposes, including in                            particular sharing listing and sales data with the Real Estate Institute of New Zealand Inc (", contentFont);
            Chunk commented62chunk = new Chunk("REINZ", boldFont);
            Chunk c134 = new Chunk(") for inclusion in the                          aggregated databases, reports and materials made available by REINZ to people in the real estate industry and others;", contentFont);
            Chunk c135 = new Chunk("     \n        (d)", colorFont);
            Chunk c136 = new Chunk(" generating and publishing sales and other reports (whether generated by the Agent, REINZ or by any third party accessing               such information); and", contentFont);
            Chunk c137 = new Chunk("     \n        (e)", colorFont);
            Chunk c138 = new Chunk(" any related purposes.", contentFont);
            formattedContent62.Add(c128);
            formattedContent62.Add(c129);
            formattedContent62.Add(c130);
            formattedContent62.Add(c131);
            formattedContent62.Add(c132);
            formattedContent62.Add(c133);
            formattedContent62.Add(commented62chunk);
            formattedContent62.Add(c134);
            formattedContent62.Add(c135);
            formattedContent62.Add(c136);
            formattedContent62.Add(c137);
            formattedContent62.Add(c138);
            doc.Add(new Paragraph(formattedContent62));


            doc.NewPage();
            doc.Add(t1);
            doc.Add(new Paragraph(" "));
            AddTextFieldToPDF(writer, doc, "Propertyfield", "Property Address", propertyAddress, 570, 765, 120, 785);


            //18th point
            Phrase formattedContent63 = new Phrase();
            Chunk c139 = new Chunk("18." + "      " + "CUSTOMER DUE DILIGENCE AND AML/CFT", titleFont);
            formattedContent63.Add(c139);
            doc.Add(new Paragraph(formattedContent63));

            //18.1
            Phrase formattedContent64 = new Phrase();
            Chunk c140 = new Chunk("(18.1)", colorFont);
            Chunk c141 = new Chunk("  The parties acknowledge and agree that:", contentFont);
            formattedContent64.Add(c140);
            formattedContent64.Add(c141);
            doc.Add(new Paragraph(formattedContent64));

            //18.1 (a),(b)
            Phrase formattedContent65 = new Phrase();
            Chunk c142 = new Chunk("        (a)", colorFont);
            Chunk c143 = new Chunk(" The Agent must collect information about the Client to undertake customer due diligence and take any other steps that                      may be necessary to comply with the Anti-Money Laundering and Countering Finance of Terrorism Act 2008 (", contentFont);
            Chunk commented65chunk = new Chunk("(AML/CFT                    Act);", boldFont);
            Chunk c144 = new Chunk("     \n        (b)", colorFont);
            Chunk c145 = new Chunk("  The Agent may use customer due diligence services (including electronic based services from a third party) to very the                       Client’s identity and conduct customer due diligence under the AML/CFT Act;", contentFont);
            Chunk c146 = new Chunk("     \n        (c)", colorFont);
            Chunk c147 = new Chunk(" The Agent cannot conduct real estate agency work for the Client under the REAA 2008 until the Agent:", contentFont);

            Chunk c148 = new Chunk(") for inclusion in the                aggregated databases, reports and materials made available by REINZ to people in the real estate industry and others;", contentFont);
            Chunk c149 = new Chunk("     \n              i. ", colorFont);
            Chunk c150 = new Chunk(" has completed the appropriate level of customer due diligence on the Client under the AML/CFT Act and has satisfied                        themselves that they can act; and ", contentFont);
            Chunk c151 = new Chunk("     \n              ii.", colorFont);
            Chunk c152 = new Chunk(" has completed the steps required under the REAA 2008 and the Real Estate Agents Act (Professional Conduct and                           Client Care) Rules 2012, including giving a copy of this Agreement, signed by both parties, to the Client; and", contentFont);
            Chunk c153 = new Chunk("     \n        (d)", colorFont);
            Chunk c154 = new Chunk("  The Agent will notify the Client when the above requirements have been satisfied.", contentFont);

            formattedContent65.Add(c142);
            formattedContent65.Add(c143);
            formattedContent65.Add(commented65chunk);
            formattedContent65.Add(c144);
            formattedContent65.Add(c145);
            formattedContent65.Add(c146);
            formattedContent65.Add(c147);
            formattedContent65.Add(c148);
            formattedContent65.Add(c149);
            formattedContent65.Add(c150);
            formattedContent65.Add(c151);
            formattedContent65.Add(c152);
            formattedContent65.Add(c153);
            formattedContent65.Add(c154);
            doc.Add(new Paragraph(formattedContent65));

            doc.Add(new Paragraph(" "));

            //19th point
            Phrase formattedContent66 = new Phrase();
            Chunk c155 = new Chunk("19." + "      " + "INDEMNITY", titleFont);
            formattedContent66.Add(c155);
            doc.Add(new Paragraph(formattedContent66));

            //19.1
            Phrase formattedContent67 = new Phrase();
            Chunk c156 = new Chunk("(19.1)", colorFont);
            Chunk c157 = new Chunk("  The Client (and if more than one, jointly and severally) indemnifies the Agent, the licensees, Harcourts Group Limited, and                  any of their respective employees, agents, contractors and advisors against all costs, expenses, losses, damages, claims                  or other liability arising from a breach of this Agreement by the Client, including without limitation, the Client providing                         inaccurate information about the Property, or the Client omitting any material information in this Agreement or regarding the               Property.", contentFont);
            formattedContent67.Add(c156);
            formattedContent67.Add(c157);
            doc.Add(new Paragraph(formattedContent67));
            doc.Add(new Paragraph(" "));


            //20th point
            Phrase formattedContent71 = new Phrase();
            Chunk c163 = new Chunk("20." + "      " + "CONFIDENTIALITY", titleFont);
            formattedContent71.Add(c163);
            doc.Add(new Paragraph(formattedContent71));

            //20.1
            Phrase formattedContent72 = new Phrase();
            Chunk c164 = new Chunk("(20.1)", colorFont);
            Chunk c165 = new Chunk("   Except as provided in this Agreement or as agreed between the parties in writing, neither party may disclose any                              information contained in this Agreement to a third party other than:", contentFont);
            formattedContent72.Add(c164);
            formattedContent72.Add(c165);
            doc.Add(new Paragraph(formattedContent72));


            //20.. (a),(b)
            Phrase formattedContent73 = new Phrase();
            Chunk c166 = new Chunk("        (a)", colorFont);
            Chunk c167 = new Chunk(" as required by law;", contentFont);
            Chunk c168 = new Chunk("     \n        (b)", colorFont);
            Chunk c169 = new Chunk(" in good faith and in proper furtherance of the objects of this Agreement;", contentFont);
            Chunk c170 = new Chunk("     \n        (c)", colorFont);
            Chunk c171 = new Chunk(" to those of their employees, officers, professional or financial advisers and bankers as reasonably necessary but only on a                  strictly confidential basis;", contentFont);
            Chunk c172 = new Chunk("     \n        (d)", colorFont);
            Chunk c173 = new Chunk(" to enforce a party’s rights or to defend any claim or action under this Agreement; or", contentFont);
            Chunk c174 = new Chunk("     \n        (e)", colorFont);
            Chunk c175 = new Chunk("  where the information is already in the public domain.", contentFont);

            formattedContent73.Add(c166);
            formattedContent73.Add(c167);
            formattedContent73.Add(c168);
            formattedContent73.Add(c169);
            formattedContent73.Add(c170);
            formattedContent73.Add(c171);
            formattedContent73.Add(c172);
            formattedContent73.Add(c173);
            formattedContent73.Add(c174);
            formattedContent73.Add(c175);

            doc.Add(new Paragraph(formattedContent73));

            doc.Add(new Paragraph(" "));
            //21st point
            Phrase formattedContent68 = new Phrase();
            Chunk c158 = new Chunk("21." + "      " + "NOTICES", titleFont);
            formattedContent68.Add(c158);
            doc.Add(new Paragraph(formattedContent68));

            //21.1
            Phrase formattedContent69 = new Phrase();
            Chunk c159 = new Chunk("(21.1)", colorFont);
            Chunk c160 = new Chunk("  Any notices given under or relating to this Agreement may be served or given by hand, mail, or email. If there is more than                  one set of contact details for the Client, then a copy of this Agreement and any notices may be sent to any one of them and               notice to any person that is listed as a Client will be notice to all of them. Notices to the Client may also be sent to the                         Client’s lawyer unless otherwise instructed.", contentFont);
            formattedContent69.Add(c159);
            formattedContent69.Add(c160);
            doc.Add(new Paragraph(formattedContent69));

            //21.2
            Phrase formattedContent70 = new Phrase();
            Chunk c161 = new Chunk("(21.2)", colorFont);
            Chunk c162 = new Chunk(" This Agreement and notices under it will be deemed to have been received:", contentFont);
            formattedContent70.Add(c161);
            formattedContent70.Add(c162);
            doc.Add(new Paragraph(formattedContent70));

            //21.. (a),(b)
            Phrase formattedContent75 = new Phrase();
            Chunk c176 = new Chunk("           (a)", colorFont);
            Chunk c177 = new Chunk(" when delivered in person, at the time of delivery;", contentFont);
            Chunk c178 = new Chunk("     \n           (b)", colorFont);
            Chunk c179 = new Chunk(" if sent by mail, five (5) working days after being mailed; or", contentFont);
            Chunk c180 = new Chunk("     \n           (c)", colorFont);
            Chunk c181 = new Chunk(" if sent by email, when acknowledged by the addressee orally or by return email or otherwise in writing except that return                     emails generated automatically shall not constitute an acknowledgement.", contentFont);
            formattedContent75.Add(c176);
            formattedContent75.Add(c177);
            formattedContent75.Add(c178);
            formattedContent75.Add(c179);
            formattedContent75.Add(c180);
            formattedContent75.Add(c181);
            doc.Add(new Paragraph(formattedContent75));

            doc.NewPage();
            doc.Add(t1);
            doc.Add(new Paragraph(" "));
            AddTextFieldToPDF(writer, doc, "Propertyfield", "Property Address", propertyAddress, 570, 765, 120, 785);

            //22nd point
            Phrase formattedContent76 = new Phrase();
            Chunk c182 = new Chunk("22." + "      " + "GENERAL", titleFont);
            formattedContent76.Add(c182);
            doc.Add(new Paragraph(formattedContent76));

            //22.1 22.2
            Phrase formattedContent77 = new Phrase();
            Chunk c183 = new Chunk("(22.1)", colorFont);
            Chunk c184 = new Chunk(" The termination of this Agreement for any reason is without prejudice to any rights, powers, authorities, or remedies of the                   parties including the Agent’s right to commission and reimbursement of the agreed marketing costs and/or expenses. ", contentFont);
            Chunk c185 = new Chunk("\n (22.2)", colorFont);
            Chunk c186 = new Chunk("  Any reference to ‘working day’ will have the meaning ascribed to it under the REINZ/ADLS Agreement for Sale and                            Purchase of Real Estate, as updated or amended.", contentFont);
            formattedContent77.Add(c183);
            formattedContent77.Add(c184);
            formattedContent77.Add(c185);
            formattedContent77.Add(c186);
            doc.Add(new Paragraph(formattedContent77));
            doc.Add(new Paragraph(" "));


            //23rd point
            Phrase formattedContent78 = new Phrase();
            Chunk c187 = new Chunk("23." + "      " + "CLIENT ACKNOWLEDGMENTS - PLEASE READ CAREFULLY", titleFont);
            Chunk c188 = new Chunk(" \n (This is a binding contract. Professional advice should be sought regarding the effect and consequences of clauses in this    Agreement)", contentFont);
            formattedContent78.Add(c187);
            formattedContent78.Add(c188);
            doc.Add(new Paragraph(formattedContent78));
            doc.Add(new Paragraph(" "));

            PdfPTable tableClientAcknowledgment = new PdfPTable(2);
            tableClientAcknowledgment.WidthPercentage = 100;


            // float[] widths1 = new float[] { 100f };
            tableClientAcknowledgment.DefaultCell.BackgroundColor = new BaseColor(220, 220, 220);
            tableClientAcknowledgment.DefaultCell.BorderColor = BaseColor.DARK_GRAY;
            float[] widths2 = new float[] { 8f, 70f };

            tableClientAcknowledgment.SetWidths(widths2);
            BaseColor headerBackgroundColor2 = BaseColor.BLACK; // Light Gray

            // Add headers to the table with background color


            string[] headers2 = { "", "The Client acknowledges and agrees that, prior to signing this Agreement, the Client has been:" };
            foreach (string header in headers2)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.WHITE)));
                cell.BackgroundColor = headerBackgroundColor1;
                cell.FixedHeight = 30f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                tableClientAcknowledgment.AddCell(cell);
            }

            string[,] data2 = {
                                { "        (a)" ," recommended to seek legal and professional advice and a reasonable opportunity to do so was provided by the Agent;  " },
                                { "        (b)" ," recommended that the Client can, and may need to, seek technical or other advice and information and a reasonable opportunity to do so was provided by the Agent;  " },
                                { "        (c)" ," given a copy of the Real Estate Authority’s (REA) Approved Guide as to Residential Property Agency Agreements and Sale and Purchase Agreements. Further information on agency agreements and contractual documents is available from the REA at www.rea.govt.nz;" },
                                { "        (d)" ," advised about Harcourts’ complaints and disputes resolution processes which can be found at www.harcourts.net/nz under the Consumer Advice tab;" },
                                { "        (e)" ," advised that the Client and customers may access the REA’s complaints process without first using Harcourts’ complaints process and that any use of Harcourts’ complaints process does not preclude a complaint to the Authority" },
                                { "        (f)" ," advised and has had an explanation of the circumstances in which the Client could be liable to pay full commission to more than one agent in the event a transaction is concluded; " },
                                { "        (g)" ," advised when this Agreement comes to an end; " },
                                { "        (h)" ," made aware of the various possible methods of sale and how the chosen method could impact on the individual benefits that the licensees may receive; " },
                                { "        (i)" ," made aware of the Agent’s disclosure obligations as set out in this Agreement; " },
                                { "        (j)" ," given an appraisal for the Property in writing and where no directly comparable or semi-comparable sales data exists, this has been explained in writing; " },
                                { "        (k)" ," recommended to seek professional advice (tax and/or legal advice) on the tax implications regarding GST treatment; " },
                                { "        (l)" ," recommended to seek professional advice (tax and/or legal advice) on the income tax implications of the purchase price allocation rules (where applicable); " },


                           };

            for (int i = 0; i < data2.GetLength(0); i++)
            {
                for (int j = 0; j < data2.GetLength(1); j++)
                {


                    PdfPCell cell = new PdfPCell(new Phrase(data2[i, j], new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.BackgroundColor = BaseColor.WHITE;
                    cell.FixedHeight = 30f;
                    cell.Padding = 3f;
                    tableClientAcknowledgment.AddCell(cell);


                }
            }
            doc.Add(tableClientAcknowledgment);

            doc.Add(new Paragraph(" "));

            //23rd point
            Phrase formattedContent79 = new Phrase();
            Chunk c189 = new Chunk("" + "  " + "EXECUTION", titleFont);
            Chunk c190 = new Chunk(" \n   I / we have read, understood, and agree to the above terms.", contentFont);
            Chunk c191 = new Chunk(" \n   I / we agree that the Agent may disclose the listing and sale details of this Property for the legitimate conduct of the Agent’s real        estate agency business.", contentFont);
            Chunk c192 = new Chunk(" \n   I / we agree that Harcourts may contact me/us to survey for client satisfaction.", contentFont);
            Chunk c193 = new Chunk(" \n   I / we agree that this Agreement may be signed in two or more counter parts (electronically or otherwise), each of which shall be        deemed original and all of which together will comprise one document .", contentFont);
            formattedContent79.Add(c189);
            formattedContent79.Add(c190);
            formattedContent79.Add(c191);
            formattedContent79.Add(c192);
            formattedContent79.Add(c193);
            doc.Add(new Paragraph(formattedContent79));

            doc.Add(new Paragraph(" "));
            doc.NewPage();

            doc.Add(t1);
            AddTextFieldToPDF(writer, doc, "Propertyfield", "Property Address", propertyAddress, 570, 765, 120, 785);

            doc.Add(new Paragraph(" "));
            if (signature.Count < 6)
            {
                for (int i = 0; i <= (6 - signature.Count); i++)
                {
                    signature.Add(new SignaturesOfClient());

                }
            }

            if (clientDetails.Count < 6)
            {
                for (int i = 0; i <= (6 - clientDetails.Count); i++)
                {
                    clientDetails.Add(new ClientDetail());

                }
            }

            //DownloadImage(doc, id);
            // Phrase formattedContent80 = new Phrase();
            // Chunk c194 = new Chunk("\nSignature of Client(s): " + "                                                                 Signature of Client(s):", contentFont);
            if (execution == null)
            {
                execution = new Execution();
            }



            //AddTextFieldToPDF(writer, doc, "Propertyfield", "", items[0], 125, 720, 220, 733);
            // AddTextFieldToPDF(writer, doc, "Propertyfield", "", items[1], 380, 720, 470, 733);
            // Chunk c195 = new Chunk("\n Position: Owner/Director/Trustee/Attorney/" + "                                Position: Owner/Director/Trustee/Attorney/", contentFont);
            //Chunk c196 = new Chunk("\n Authorised Signatory" + "                                                                  Authorised Signatory", contentFont);


            Phrase formattedContent80 = new Phrase();   
            Chunk c194 = new Chunk("\nSignature of Client(s): " + "                                                                 Signature of Client(s):", contentFont);

            CreatePdfWithImageInsideTextField(doc, writer, signature[0].SignatureOfClientName != null ? signature[0].SignatureOfClientName : "", "signature1", 125, 705, 220, 733);
            CreatePdfWithImageInsideTextField(doc, writer, signature[1].SignatureOfClientName != null ? signature[1].SignatureOfClientName : "", "signature2", 380, 705, 470, 733);
            //AddTextFieldToPDF(writer, doc, "Propertyfield", "", 125, 705, 220, 733);
            //AddTextFieldToPDF(writer, doc, "Propertyfield", "", 380, 705, 470, 733);

            Chunk c195 = new Chunk("\n\n Position: " + clientDetails[0].Position + "                                                                             Position: " + clientDetails[1].Position, contentFont);
            Chunk c196 = new Chunk("\n " + clientDetails[0].FirstName + " " + clientDetails[0].SurName + "                                                                              " + clientDetails[1].FirstName + " " + clientDetails[1].SurName, contentFont);


            formattedContent80.Add(c194);
            formattedContent80.Add(c195);
            formattedContent80.Add(c196);
            //formattedContent80.Add(c197);
            doc.Add(new Paragraph(formattedContent80));
            doc.Add(new Paragraph(" "));



            Phrase formattedContent81 = new Phrase();
            Chunk c197 = new Chunk("Signature of Client(s): " + "                                                                 Signature of Client(s):", contentFont);

            CreatePdfWithImageInsideTextField(doc, writer, signature[2].SignatureOfClientName != null ? signature[2].SignatureOfClientName : "", "signature1", 125, 635, 220, 660);
            CreatePdfWithImageInsideTextField(doc, writer, signature[3].SignatureOfClientName != null ? signature[3].SignatureOfClientName : "", "signature2", 380, 635, 470, 660);

            //AddTextFieldToPDF(writer, doc, "Propertyfield", "", "", 125, 635, 220, 660);
            //AddTextFieldToPDF(writer, doc, "Propertyfield", "", "", 380, 635, 470, 660);
            Chunk c198 = new Chunk("\n Position: " + clientDetails[2].Position + "                                                                                    Position: " + clientDetails[3].Position, contentFont);
            Chunk c199 = new Chunk("\n " + clientDetails[2].FirstName + " " + clientDetails[0].SurName + "                                                                                " + clientDetails[3].FirstName + " " + clientDetails[3].SurName, contentFont);
            formattedContent81.Add(c197);
            formattedContent81.Add(c198);
            formattedContent81.Add(c199);
            doc.Add(new Paragraph(formattedContent81));
            doc.Add(new Paragraph(" "));

            Phrase formattedContent82 = new Phrase();
            Chunk c200 = new Chunk("\n Signed on behalf of the Agent:" + "                                      Date:" + "                                   at" + "                                  am/pm", contentFont);

            if (execution.AgentToSignHere != null)
            {
                iTextSharp.text.Image logo00 = iTextSharp.text.Image.GetInstance(execution.AgentToSignHere);
                logo00.ScaleToFit(35f, 35f);
                logo00.SetAbsolutePosition(160, 530);

                PdfContentByte contentByte = writer.DirectContent;
                contentByte.AddImage(logo00);
            }
            //AddTextFieldToPDF(writer, doc, "Propertyfield", "", "", 160, 545, 250, 520);
            var executionDate = execution.AgentToSignHereDate != null ? ((DateTime)execution.AgentToSignHereDate).ToString("MM-yyyy") : "";
            var executionTIme = execution.AgentToSignHereDate != null ? (DateTimeOffset.Parse(execution.AgentToSignHereDate.ToString()).ToString("tt")) : "";
            AddTextFieldToPDF(writer, doc, "Propertyfield", "", executionDate, 280, 555, 360, 530);
            AddTextFieldToPDF(writer, doc, "Propertyfield", "", executionTIme, 372, 555, 450, 530);

            formattedContent82.Add(c200);
            doc.Add(new Paragraph(formattedContent82));
            doc.Add(new Paragraph(" "));

        }

        public void AddTextFieldToPDF(PdfWriter writer, Document doc, string fieldName, string labelText, string text, float x1, float y1, float x2, float y2, bool isCheckbox = false, bool isChecked = false, bool isRadio = false, bool isSelected = false)
        {
            if (!isCheckbox && !isRadio)
            {
                // Existing code to add a regular text field
                TextField textField = new TextField(writer, new iTextSharp.text.Rectangle(x1, y1, x2, y2), fieldName);
                textField.Options = TextField.READ_ONLY; // For edit text and Readonly, set here..

                // Set the background color to #f1f4ff
                int redValue = 241;
                int greenValue = 244;
                int blueValue = 255;
                BaseColor backgroundColor = new BaseColor(232, 232, 232);
                textField.BackgroundColor = backgroundColor;
                textField.Text = text;
                textField.FontSize = 8;
                textField.TextColor = BaseColor.BLACK;
                PdfFormField field = textField.GetTextField();
                writer.AddAnnotation(field);

                PdfContentByte overContent = writer.DirectContent;
                BaseFont baseFont = BaseFont.CreateFont();
                overContent.SetFontAndSize(baseFont, 10);
                overContent.BeginText();
                overContent.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, labelText, x2 - 5, y1 + 5, 0);
                overContent.EndText();
            }
            else if (isCheckbox)
            {
                //                // Code to add a checkbox using the new style
                //                iTextSharp.text.Rectangle checkboxRect = new iTextSharp.text.Rectangle(x1, y1, x1 + 15, y1 + 15);
                //                PdfFormField checkbox = PdfFormField.CreateCheckBox(writer);

                //                checkbox.SetWidget(checkboxRect, PdfAnnotation.HIGHLIGHT_INVERT);

                //                checkbox.FieldName = fieldName;

                //                checkbox.ValueAsName = isChecked ? "Yes" : "Off"; // Set the checkbox value based on the isChecked parameter

                //                // Add the read-only flag to the checkbox field
                //                checkbox.SetFieldFlags(PdfAnnotation.FLAGS_READONLY | PdfFormField.FF_READ_ONLY);

                //                // Add the checkbox to the document
                //                writer.AddAnnotation(checkbox)
                //;

                //                // Add the checkbox image based on the checkbox state (checked or unchecked)
                //                string imagesFolder = "Pictures";
                //                string imageFileName = isChecked ? "Checked1.png" : "Unchecked1.png";
                //                string imagePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images", "Checked1.png");
                //                iTextSharp.text.Image checkboxImage = iTextSharp.text.Image.GetInstance(imagePath);

                //                // Set the position and size of the image
                //                checkboxImage.SetAbsolutePosition(x1, y1);
                //                checkboxImage.ScaleAbsolute(15, 15); // Adjust the size as needed

                //                // Add the image to the document
                //                doc.Add(checkboxImage);

                //                // Add the label text for the checkbox
                //                PdfContentByte overContent = writer.DirectContent;
                //                BaseFont baseFont = BaseFont.CreateFont();
                //                overContent.SetFontAndSize(baseFont, 10);
                //                overContent.BeginText();
                //                overContent.ShowTextAligned(PdfContentByte.ALIGN_LEFT, labelText, x1 + 20, y1, 0);
                //                overContent.EndText();
            }
            else if (isRadio)
            {
                // Code to add a radio button
                iTextSharp.text.Rectangle radioRect = new iTextSharp.text.Rectangle(x1, y1, x1 + 15, y1 + 15);
                PdfFormField radio = PdfFormField.CreateRadioButton(writer, true);

                radio.SetWidget(radioRect, PdfAnnotation.HIGHLIGHT_INVERT);

                radio.FieldName = fieldName;

                radio.ValueAsName = isSelected ? "Yes" : "Off"; // Set the radio button value based on the isSelected parameter

                // Add the read-only flag to the radio button field
                radio.SetFieldFlags(PdfAnnotation.FLAGS_READONLY | PdfFormField.FF_READ_ONLY);

                // Add the radio button to the document
                writer.AddAnnotation(radio)
        ;

                // Add the radio button image based on the radio button state (selected or unselected)
                string imagesFolder = "Pictures";
                string imageFileName = isSelected ? "RadioChecked.png" : "RadioUnchecked.png";
                string imagePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "RadioChecked.png");
                iTextSharp.text.Image radioImage = iTextSharp.text.Image.GetInstance(imagePath);

                // Set the position and size of the image
                radioImage.SetAbsolutePosition(x1, y1);
                radioImage.ScaleAbsolute(15, 15); // Adjust the size as needed

                // Add the image to the document
                doc.Add(radioImage);

                // Add the label text for the radio button
                PdfContentByte overContent = writer.DirectContent;
                BaseFont baseFont = BaseFont.CreateFont();
                overContent.SetFontAndSize(baseFont, 10);
                overContent.BeginText();
                overContent.ShowTextAligned(PdfContentByte.ALIGN_LEFT, labelText, x1 + 20, y1, 0);
                overContent.EndText();
            }
        }
        public static void AddRadioField(Document document, PdfWriter writer, string fieldName, Rectangle position, bool isChecked, int x, int y)
        {

            // Create a checkbox field
            PdfFormField radioField = PdfFormField.CreateRadioButton(writer, isChecked);
            radioField.FieldName = fieldName;

            // radioField.BorderStyle = PdfBorderDictionary.STYLE_SOLID;

            //radioField.bord = BaseField.BORDER_WIDTH_THIN;

            if (isChecked)
            {

                string checkmarkImagePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "TestCheck.png");
                iTextSharp.text.Image checkmarkImage = iTextSharp.text.Image.GetInstance(checkmarkImagePath);
                // Image checkmarkImage = Image.GetInstance(checkmarkImagePath);
                checkmarkImage.SetAbsolutePosition(x, y);
                checkmarkImage.ScaleToFit(7, 7); // Adjust the size of the image as needed

                PdfContentByte cb = writer.DirectContent;
                cb.AddImage(checkmarkImage);
                Phrase phrase = new Phrase();
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, phrase, x, y, 0);
            }
            //else
            //{
            //    string checkmarkImagePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images", "RadioUnchecked.png");
            //    iTextSharp.text.Image checkmarkImage = iTextSharp.text.Image.GetInstance(checkmarkImagePath);
            //    checkmarkImage.SetAbsolutePosition(x, y);
            //    checkmarkImage.ScaleToFit(10, 10); // Adjust the size of the image as needed

            //    PdfContentByte cb = writer.DirectContent;
            //    cb.AddImage(checkmarkImage);
            //    Phrase phrase = new Phrase();

            //    ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, phrase, x, y, 0);
            //}

            radioField.MKBorderColor = BaseColor.WHITE;
            radioField.SetWidget(position, PdfAnnotation.APPEARANCE_NORMAL);
            int fieldFlags = PdfAnnotation.FLAGS_READONLY;
            radioField.SetFieldFlags(fieldFlags);
            radioField.SetFieldFlags(PdfFormField.FF_READ_ONLY);
            // checkboxField.MKBackgroundColor = new  BaseColor(232, 232, 232);
            radioField.MKBorderColor = new BaseColor(135, 206, 235);
            writer.AddAnnotation(radioField);



        }

        private void AddContentToPDF(Document doc, PdfWriter writer, ListingAddress listingAddresses,ApplicationUser user)
        {
            if (listingAddresses == null)
            {

                //List<ListingAddress> listings = new List<ListingAddress>();		
                ListingAddress obj = new ListingAddress();
                listingAddresses = obj;
            }
            else
            {
                listingAddresses = listingAddresses;
            }
            BaseFont baseF14 = BaseFont.CreateFont();
            var blueColor = new BaseColor(43, 145, 175);
            PdfPTable headertable = new PdfPTable(1);
            headertable.WidthPercentage = 108;
            headertable.DefaultCell.Border = 0;

            PdfPTable table = new PdfPTable(3);
            table.WidthPercentage = 108;
            BaseColor backgroundColor = new BaseColor(232, 232, 232);
            doc.Add(table);

            // Add the fieldset box with legend
            PdfPTable fieldsetTable = new PdfPTable(1);
            fieldsetTable.WidthPercentage = 100;

            PdfPCell legendCell = new PdfPCell();
            legendCell.Border = iTextSharp.text.Rectangle.BOX;
            legendCell.BorderWidth = 2;
            legendCell.Padding = 2;
            legendCell.BorderColor = BaseColor.DARK_GRAY;
            legendCell.BorderWidth = 1f;


            fieldsetTable.AddCell(legendCell);

            Phrase legaltypePhrase = new Phrase();
            Chunk legaltypeChunk = new Chunk("Location");
            var legaltypeParagraph = new Paragraph(legaltypeChunk);
            legaltypeParagraph.PaddingTop = 2f;
            legaltypeParagraph.Font.Size = 10;
            legaltypeParagraph.Alignment = Element.ALIGN_TOP;
            legaltypeParagraph.Font.Color = blueColor;
            legaltypeParagraph.Font.SetStyle(Font.BOLD);
            headertable.AddCell(legaltypeParagraph);

            Phrase typePhrase1 = new Phrase();
            Chunk typeChunk1 = new Chunk(" Address");
            var typeParagraph1 = new Paragraph(typeChunk1);
            typeParagraph1.PaddingTop = 2f;
            typeParagraph1.Alignment = Element.ALIGN_TOP;
            typeParagraph1.Font.Color = BaseColor.BLACK;
            typeChunk1.Font.Size = 9f;
            typeChunk1.Font.SetStyle(Font.BOLD);


            PdfPTable dataTable1 = new PdfPTable(14);
            dataTable1.WidthPercentage = 100f;
            dataTable1.DefaultCell.Border = 0;

            float[] arr1 = new float[] { 0.5f, 0.8f, 0.2f, 1.3f, 0.8f, 0.2f, 0.7f, 4.4f, 0.2f, 0.8f, 1.1f, 0.2f, 1f, 0.6f };
            dataTable1.SetWidths(arr1);

            Chunk UnitChunk = new Chunk("Unit");
            var UnitParagraph = new Paragraph(UnitChunk);
            UnitParagraph.Alignment = Element.ALIGN_TOP;
            UnitParagraph.Alignment = Element.ALIGN_TOP;
            UnitParagraph.Font.Size = 8;
            dataTable1.AddCell(UnitParagraph);
            PdfPCell textcell = new PdfPCell();
            textcell.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", listingAddresses.Unit, baseF14, 7);
            textcell.Border = 0;
            textcell.BackgroundColor = backgroundColor;
            dataTable1.AddCell(textcell);


            dataTable1.AddCell("");

            Chunk StreetnumberChunk = new Chunk("Street number");
            StreetnumberChunk.Font.Size = 8;
            var StreetnumberParagraph = new Paragraph(StreetnumberChunk);
            StreetnumberParagraph.Alignment = Element.ALIGN_TOP;
            StreetnumberParagraph.Font.Color = BaseColor.BLACK;
            dataTable1.AddCell(StreetnumberParagraph);
            PdfPCell textcell1 = new PdfPCell();
            textcell1.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", listingAddresses.StreetNumber, baseF14, 7);
            textcell1.Border = 0;
            textcell1.BackgroundColor = backgroundColor;
            dataTable1.AddCell(textcell1);

            //added for empty space between textbox and label 
            dataTable1.AddCell("");

            Chunk StreetChunk = new Chunk("Street");
            var StreetParagraph = new Paragraph(StreetChunk);
            StreetParagraph.Alignment = Element.ALIGN_TOP;
            StreetParagraph.Font.Size = 8;
            StreetParagraph.Font.Color = BaseColor.BLACK;
            dataTable1.AddCell(StreetParagraph);
            PdfPCell textcell2 = new PdfPCell();
            textcell2.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", listingAddresses.StreetName, baseF14, 7);
            textcell2.Border = 0;
            textcell2.BackgroundColor = backgroundColor;
            dataTable1.AddCell(textcell2);
            //Phrase StreetPhrase2 = new Phrase();
            //Chunk StreetChunk2 = new Chunk("__________");
            //StreetChunk2.SetBackground(backgroundColor);
            //var StreetParagraph2 = new Paragraph(StreetChunk2);
            //dataTable1.AddCell(StreetParagraph2);


            //ADDED FOR EMPTY SPACE BETWEEN TEXTBOX AND LABEL 
            dataTable1.AddCell("");

            Chunk SuburbChunk = new Chunk("Suburb");
            var SuburbParagraph = new Paragraph(SuburbChunk);
            SuburbParagraph.Alignment = Element.ALIGN_TOP;
            SuburbParagraph.Font.Color = BaseColor.BLACK;
            SuburbParagraph.Font.Size = 8;
            dataTable1.AddCell(SuburbParagraph);
            PdfPCell textcell3 = new PdfPCell();
            textcell3.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", listingAddresses.Suburb, baseF14, 7);
            textcell3.Border = 0;
            textcell3.BackgroundColor = backgroundColor;
            dataTable1.AddCell(textcell3);


            dataTable1.AddCell("");

            Chunk codeChunk = new Chunk("Post code");
            var codeParagraph = new Paragraph(codeChunk);
            codeParagraph.Alignment = Element.ALIGN_TOP;
            codeParagraph.Font.Size = 8;
            codeParagraph.Font.Color = BaseColor.BLACK;
            dataTable1.AddCell(codeParagraph);
            PdfPCell textcell4 = new PdfPCell();
            textcell4.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", listingAddresses.PostCode, baseF14, 7);
            textcell4.Border = 0;
            textcell4.BackgroundColor = backgroundColor;
            dataTable1.AddCell(textcell4);



            PdfPTable dataTable3 = new PdfPTable(1);
            dataTable3.WidthPercentage = 100f;
            dataTable3.DefaultCell.Border = 0;
            dataTable3.DefaultCell.FixedHeight = 7f;
            dataTable3.AddCell(" ");


            PdfPTable dataTable2 = new PdfPTable(8);
            dataTable2.WidthPercentage = 100f;
            dataTable2.DefaultCell.Border = 0;

            float[] arr2 = new float[] { 0.9f, 3.4f, 0.1f, 1.1f, 1.8f, 0.1f, 0.7f, 1.9f };
            dataTable2.SetWidths(arr2);

            Chunk AgentChunk = new Chunk("Agent name ");
            var AgentParagraph = new Paragraph(AgentChunk);
            AgentParagraph.Alignment = Element.ALIGN_TOP;
            AgentParagraph.Font.Color = BaseColor.BLACK;
            AgentParagraph.Font.Size = 8;
            dataTable2.AddCell(AgentParagraph);
            PdfPCell textcell5 = new PdfPCell();
            textcell5.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", user.FirstName+ " " + user.LastName, baseF14, 7);
            textcell5.Border = 0;
            textcell5.BackgroundColor = backgroundColor;
            dataTable2.AddCell(textcell5);
 

            dataTable2.AddCell("");

            Chunk numberChunk = new Chunk("Listing number");
            var numberParagraph = new Paragraph(numberChunk);
            numberParagraph.Alignment = Element.ALIGN_TOP;
            numberParagraph.Font.Color = BaseColor.BLACK;
            numberParagraph.Font.Size = 8;
            dataTable2.AddCell(numberParagraph);
            PdfPCell textcell6 = new PdfPCell();
            textcell6.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", "", baseF14, 7);
            textcell6.Border = 0;
            textcell6.BackgroundColor = backgroundColor;
            dataTable2.AddCell(textcell6);

        

            dataTable2.AddCell("");
            Chunk LifecycleChunk = new Chunk("Lifecycle");
            var LifecycleParagraph = new Paragraph(LifecycleChunk);
            LifecycleParagraph.Alignment = Element.ALIGN_TOP;
            LifecycleParagraph.Font.Size = 8;
            LifecycleParagraph.Font.Color = BaseColor.BLACK;
            dataTable2.AddCell(LifecycleParagraph);
            PdfPCell textcell7 = new PdfPCell();
            textcell7.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", "  ", baseF14, 7);
            textcell7.Border = 0;
            textcell7.BackgroundColor = backgroundColor;
            dataTable2.AddCell(textcell7);



            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable dataTable4 = new PdfPTable(1);
            dataTable4.WidthPercentage = 100f;
            dataTable4.DefaultCell.Border = 0;
            dataTable4.DefaultCell.FixedHeight = 7f;
            dataTable4.AddCell(" ");

            legendCell.AddElement(typeParagraph1);
            legendCell.AddElement(dataTable1);
            legendCell.AddElement(dataTable3);
            legendCell.AddElement(dataTable2);
            legendCell.AddElement(dataTable4);

            fieldsetTable.AddCell(legendCell);
            headertable.AddCell(fieldsetTable);
            doc.Add(headertable);

            //ADDED FOR ADDING SPACE AFTER TABE
            //doc.Add(new Paragraph(" "));
        }

        public void AddClientTables(Document doc, PdfWriter writer, List<ClientDetail> clientlist)
        {
            if (clientlist.Count == 0)
            {
                ClientDetail obj = new ClientDetail();
                clientlist.Add(obj);
            }
            BaseFont baseF14 = BaseFont.CreateFont();
            var blueColor = new BaseColor(43, 145, 175);
            PdfPTable headertable = new PdfPTable(1);
            headertable.WidthPercentage = 108;
            headertable.DefaultCell.Border = 0;

            BaseColor backgroundColor = new BaseColor(232, 232, 232);
            PdfPTable parentTable = new PdfPTable(2);
            parentTable.WidthPercentage = 108f;
            parentTable.DefaultCell.Border = 0;

            PdfPTable table1 = new PdfPTable(1);
            table1.WidthPercentage = 100f;

            table1.DefaultCell.Border = 0;
            table1.HorizontalAlignment = Element.ALIGN_LEFT;

            // Add the fieldset box with legend
            PdfPTable fieldsetTable1 = new PdfPTable(1);
            fieldsetTable1.WidthPercentage = 100f;

            PdfPCell legendCell1 = new PdfPCell();
            legendCell1.Border = iTextSharp.text.Rectangle.BOX;
            legendCell1.BorderWidth = 2;
            legendCell1.Padding = 5;
            legendCell1.BorderColor = BaseColor.DARK_GRAY;
            legendCell1.BorderWidth = 1f;
            // legendCell1.PaddingBottom = 275;
            fieldsetTable1.AddCell(legendCell1);


            Chunk clientChunk1 = new Chunk("ClientDetails\r\n");
            var clientParagraph1 = new Paragraph(clientChunk1);
            clientParagraph1.PaddingTop = 1f;
            clientParagraph1.Alignment = Element.ALIGN_TOP;
            clientParagraph1.Font.Color = blueColor;
            clientParagraph1.Font.Size = 10;
            clientParagraph1.Font.SetStyle(Font.BOLD);

            headertable.AddCell(clientParagraph1);
            doc.Add(new Paragraph("\n"));

            Phrase typePhrase1 = new Phrase();
            Chunk typeChunk1 = new Chunk(" Address");
            var typeParagraph1 = new Paragraph(typeChunk1);
            typeParagraph1.PaddingTop = 2f;
            typeParagraph1.Alignment = Element.ALIGN_TOP;
            typeParagraph1.Font.Color = BaseColor.BLACK;
            typeChunk1.Font.Size = 9f;
            typeChunk1.Font.SetStyle(Font.BOLD);

            PdfPTable dataTable1 = new PdfPTable(6);
            dataTable1.WidthPercentage = 100f;
            dataTable1.DefaultCell.Border = 0;

            float[] arr2 = new float[] { 1.4f, 2.5f, 0.2f, 2.2f, 6f, 0.2f };
            dataTable1.SetWidths(arr2);

            Chunk titleChunk1 = new Chunk("Title");
            var titleParagraph1 = new Paragraph(titleChunk1);
            titleParagraph1.Alignment = Element.ALIGN_TOP;
            titleParagraph1.Font.Size = 8;
            titleParagraph1.Font.Color = BaseColor.BLACK;
            dataTable1.AddCell(titleParagraph1);
            PdfPCell textcell = new PdfPCell();
            textcell.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[0].Title, baseF14, 7);
            textcell.Border = 0;
            textcell.BackgroundColor = backgroundColor;
            dataTable1.AddCell(textcell);




            dataTable1.AddCell("");


            Chunk surChunk1 = new Chunk("Surname(s)");
            var surParagraph1 = new Paragraph(surChunk1);
            surParagraph1.Alignment = Element.ALIGN_TOP;
            surParagraph1.Font.Size = 8;
            surParagraph1.Font.Color = BaseColor.BLACK;
            dataTable1.AddCell(surParagraph1);
            PdfPCell textcell1 = new PdfPCell();
            textcell1.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[0].SurName, baseF14, 7);
            textcell1.Border = 0;
            textcell1.BackgroundColor = backgroundColor;
            dataTable1.AddCell(textcell1);



            dataTable1.AddCell("");

            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable1 = new PdfPTable(1);
            emptydataTable1.WidthPercentage = 100f;
            emptydataTable1.DefaultCell.Border = 0;
            emptydataTable1.DefaultCell.FixedHeight = 7f;
            emptydataTable1.AddCell(" ");


            PdfPTable dataTable3 = new PdfPTable(2);
            dataTable3.WidthPercentage = 100f;
            dataTable3.DefaultCell.Border = 0;

            float[] arr5 = new float[] { 2, 4 };
            dataTable3.SetWidths(arr5);

          



            /*Change in code*/

            if (string.IsNullOrEmpty(clientlist[0].FirstName)) // Check if FirstName is null or empty
            {
                Chunk firChunk1 = new Chunk("Company/trust Name");
                var firParagraph1 = new Paragraph(firChunk1);
                firParagraph1.Alignment = Element.ALIGN_TOP;
                firParagraph1.Font.Size = 8;
                firParagraph1.Font.Color = BaseColor.BLACK;
                dataTable3.AddCell(firParagraph1);
                PdfPCell textcell2 = new PdfPCell();
                textcell2.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[0].CompanyTrustName, baseF14, 7);
                textcell2.Border = 0;
                textcell2.BackgroundColor = backgroundColor;
                dataTable3.AddCell(textcell2);
                dataTable3.AddCell("");


            }
            else
            {
                Chunk firChunk1 = new Chunk("First name(s)"); // Use First name(s) label
                var firParagraph1 = new Paragraph(firChunk1);
                firParagraph1.Alignment = Element.ALIGN_TOP;
                firParagraph1.Font.Size = 8;
                firParagraph1.Font.Color = BaseColor.BLACK;
                dataTable3.AddCell(firParagraph1);

                PdfPCell textcell2 = new PdfPCell();
                textcell2.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[0].FirstName, baseF14, 7);
                textcell2.Border = 0;
                textcell2.BackgroundColor = backgroundColor;
                dataTable3.AddCell(textcell2);
                dataTable3.AddCell("");
            }


            /*Change in code*/




            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable2 = new PdfPTable(1);
            emptydataTable2.WidthPercentage = 100f;
            emptydataTable2.DefaultCell.Border = 0;
            emptydataTable2.DefaultCell.FixedHeight = 7f;
            emptydataTable2.AddCell(" ");

            PdfPTable dataTable4 = new PdfPTable(1);
            dataTable4.WidthPercentage = 100f;
            dataTable4.DefaultCell.Border = 0;

            Chunk addChunk1 = new Chunk("Address");
            var addParagraph1 = new Paragraph(addChunk1);
            addParagraph1.Alignment = Element.ALIGN_TOP;
            addParagraph1.Font.Size = 8;
            addParagraph1.Font.Color = BaseColor.BLACK;
            dataTable4.AddCell(addParagraph1);

            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable3 = new PdfPTable(1);
            emptydataTable3.WidthPercentage = 100f;
            emptydataTable3.DefaultCell.Border = 0;
            emptydataTable3.DefaultCell.FixedHeight = 6f;
            emptydataTable3.AddCell(" ");

            PdfPTable dataTable18 = new PdfPTable(3);
            dataTable18.WidthPercentage = 100f;
            dataTable18.DefaultCell.Border = 0;
            dataTable18.DefaultCell.FixedHeight = 30f;

            float[] arr15 = new float[] { 0.1f, 7f, 0.1f };
            dataTable18.SetWidths(arr15);
            dataTable18.AddCell("");
            PdfPCell textcell3 = new PdfPCell();
            textcell3.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[0].Address, baseF14, 7);
            textcell3.Border = 0;
            textcell3.FixedHeight = 30f;
            textcell3.BackgroundColor = backgroundColor;
            dataTable18.AddCell(textcell3);

            dataTable18.AddCell("");



            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable4 = new PdfPTable(1);
            emptydataTable4.WidthPercentage = 100f;
            emptydataTable4.DefaultCell.Border = 0;
            emptydataTable4.DefaultCell.FixedHeight = 7f;
            emptydataTable4.AddCell(" ");

            PdfPTable dataTable5 = new PdfPTable(3);
            dataTable5.WidthPercentage = 100f;
            dataTable5.DefaultCell.Border = 0;

            float[] arr7 = new float[] { 0.5f, 0.9f, 1.8f };
            dataTable5.SetWidths(arr7);

            Chunk postChunk1 = new Chunk("Post code");
            var postParagraph1 = new Paragraph(postChunk1);
            postParagraph1.Alignment = Element.ALIGN_TOP;
            postParagraph1.Font.Size = 8;
            postParagraph1.Font.Color = BaseColor.BLACK;
            dataTable5.AddCell(postParagraph1);
            PdfPCell textcell4 = new PdfPCell();
            textcell4.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[0].PostCode, baseF14, 7);
            textcell4.Border = 0;
            textcell4.BackgroundColor = backgroundColor;
            dataTable5.AddCell(textcell4);

            dataTable5.AddCell("");


            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable5 = new PdfPTable(1);
            emptydataTable5.WidthPercentage = 100f;
            emptydataTable5.DefaultCell.Border = 0;
            emptydataTable5.DefaultCell.FixedHeight = 7f;
            emptydataTable5.AddCell(" ");

            PdfPTable dataTable6 = new PdfPTable(6);
            dataTable6.WidthPercentage = 100f;
            dataTable6.DefaultCell.Border = 0;

            float[] arr8 = new float[] { 1.9f, 5.5f, 0.3f, 2f, 6f, 0.2f };
            dataTable6.SetWidths(arr8);

            Chunk homeChunk1 = new Chunk("Home");
            var homeParagraph1 = new Paragraph(homeChunk1);
            homeParagraph1.Alignment = Element.ALIGN_TOP;
            homeParagraph1.Font.Size = 8;
            homeParagraph1.Font.Color = BaseColor.BLACK;
            dataTable6.AddCell(homeParagraph1);
            PdfPCell textcell5 = new PdfPCell();
            textcell5.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[0].Home, baseF14, 7);
            textcell5.Border = 0;
            textcell5.BackgroundColor = backgroundColor;
            dataTable6.AddCell(textcell5);
            dataTable6.AddCell("");


            Chunk mobileChunk1 = new Chunk("Mobile");
            var mobileParagraph1 = new Paragraph(mobileChunk1);
            mobileParagraph1.Alignment = Element.ALIGN_TOP;
            mobileParagraph1.Font.Size = 8;
            mobileParagraph1.Font.Color = BaseColor.BLACK;
            dataTable6.AddCell(mobileParagraph1);
            PdfPCell textcell6 = new PdfPCell();
            textcell6.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[0].Mobile, baseF14, 7);
            textcell6.Border = 0;
            textcell6.BackgroundColor = backgroundColor;
            dataTable6.AddCell(textcell6);

            dataTable6.AddCell("");



            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable6 = new PdfPTable(1);
            emptydataTable6.WidthPercentage = 100f;
            emptydataTable6.DefaultCell.Border = 0;
            emptydataTable6.DefaultCell.FixedHeight = 7f;
            emptydataTable6.AddCell(" ");

            PdfPTable dataTable11 = new PdfPTable(3);
            dataTable11.WidthPercentage = 100f;
            dataTable11.DefaultCell.Border = 0;

            float[] arr12 = new float[] { 0.8f, 4.9f, 0.1f };
            dataTable11.SetWidths(arr12);

            Chunk busChunk1 = new Chunk("Business");
            var busParagraph1 = new Paragraph(busChunk1);
            busParagraph1.Alignment = Element.ALIGN_TOP;
            busParagraph1.Font.Size = 8;
            busParagraph1.Font.Color = BaseColor.BLACK;
            dataTable11.AddCell(busParagraph1);
            PdfPCell textcell7 = new PdfPCell();
            textcell7.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[0].Business, baseF14, 7);
            textcell7.Border = 0;
            textcell7.BackgroundColor = backgroundColor;
            dataTable11.AddCell(textcell7);
            dataTable11.AddCell("");


            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable7 = new PdfPTable(1);
            emptydataTable7.WidthPercentage = 100f;
            emptydataTable7.DefaultCell.Border = 0;
            emptydataTable7.DefaultCell.FixedHeight = 7f;
            emptydataTable7.AddCell(" ");

            PdfPTable dataTable12 = new PdfPTable(3);
            dataTable12.WidthPercentage = 100f;
            dataTable12.DefaultCell.Border = 0;

            float[] arr13 = new float[] { 0.6f, 4.9f, 0.1f };
            dataTable12.SetWidths(arr13);

            Chunk emailChunk1 = new Chunk("Email");
            var emailParagraph1 = new Paragraph(emailChunk1);
            emailParagraph1.Alignment = Element.ALIGN_TOP;
            emailParagraph1.Font.Size = 8;
            emailParagraph1.Font.Color = BaseColor.BLACK;
            dataTable12.AddCell(emailParagraph1);
            PdfPCell textcell8 = new PdfPCell();
            textcell8.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[0].Email, baseF14, 7);
            textcell8.Border = 0;
            textcell8.BackgroundColor = backgroundColor;
            dataTable12.AddCell(textcell8);
            dataTable12.AddCell("");




            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable8 = new PdfPTable(1);
            emptydataTable8.WidthPercentage = 100f;
            emptydataTable8.DefaultCell.Border = 0;
            emptydataTable8.DefaultCell.FixedHeight = 7f;
            emptydataTable8.AddCell(" ");

            PdfPTable dataTable13 = new PdfPTable(6);
            dataTable13.WidthPercentage = 100f;
            dataTable13.DefaultCell.Border = 0;

            float[] arr14 = new float[] { 3.6f, 4.2f, 0.4f, 2f, 4f, 0.2f };
            dataTable13.SetWidths(arr14);

          


            Chunk compChunk1;
            if (!string.IsNullOrEmpty(clientlist[0].CompanyTrustName))
            {
                compChunk1 = new Chunk("Contact Person");
            }
            else
            {
                compChunk1 = new Chunk("Company/trust");
            }

            var compParagraph1 = new Paragraph(compChunk1);
            compParagraph1.Alignment = Element.ALIGN_TOP;
            compParagraph1.Font.Size = 8;
            compParagraph1.Font.Color = BaseColor.BLACK;
            dataTable13.AddCell(compParagraph1);

            PdfPCell textcell9 = new PdfPCell();
            textcell9.Border = 0;
            textcell9.BackgroundColor = backgroundColor;

            if (!string.IsNullOrEmpty(clientlist[0].CompanyTrustName))
            {
                textcell9.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[0].ContactPerson, baseF14, 7);
            }
            else
            {
                textcell9.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[0].CompanyTrustName, baseF14, 7);
            }

            dataTable13.AddCell(textcell9);
            dataTable13.AddCell("");




            Chunk positionChunk1 = new Chunk("Position");
            var positionParagraph1 = new Paragraph(positionChunk1);
            positionParagraph1.Alignment = Element.ALIGN_TOP;
            positionParagraph1.Font.Size = 8;
            positionParagraph1.Font.Color = BaseColor.BLACK;
            dataTable13.AddCell(positionParagraph1);
            PdfPCell textcell10 = new PdfPCell();
            textcell10.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[0].Position, baseF14, 7);
            textcell10.Border = 0;
            textcell10.BackgroundColor = backgroundColor;
            dataTable13.AddCell(textcell10);
            dataTable13.AddCell("");



            /*Adding Checkbox here so keep in mind*/

            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable9 = new PdfPTable(1);
            emptydataTable9.WidthPercentage = 100f;
            emptydataTable9.DefaultCell.Border = 0;
            emptydataTable9.DefaultCell.FixedHeight = 7f;
            emptydataTable9.AddCell(" ");

            PdfPTable dataTable17 = new PdfPTable(6);
            dataTable17.WidthPercentage = 100f;
            dataTable17.DefaultCell.Border = 0;

            float[] arr017 = new float[] { 1.9f, 0.9f, 0.9f, 2.1f, 1.8f, 0.1f };
            dataTable17.SetWidths(arr017);

           



            Chunk gstChunk1 = new Chunk("GST registered");
            var gstParagraph1 = new Paragraph(gstChunk1);
            gstParagraph1.Alignment = Element.ALIGN_TOP;
            gstParagraph1.Font.Size = 8;
            gstParagraph1.Font.Color = BaseColor.BLACK;
            dataTable17.AddCell(gstParagraph1);

            if (clientlist[0].ID != 0)
            {
                if (clientlist[0].IsGSTRegistered)
                {

                    isRegistered = true;
                    isNotregistered = false;

                }
                else
                {
                    isRegistered = false;
                    isNotregistered = true;
                }


            }
            else
            {
                isRegistered = false;
                isNotregistered = false;
            }

            Chunk noChunk1 = new Chunk(" No");
            noChunk1.Font.Size = 8;
            var noParagraph1 = new Paragraph();

            // Add unchecked checkbox image to the paragraph
            Image uncheckedImage = GetCheckboxImage(isNotregistered);
            uncheckedImage.ScaleAbsolute(8, 8); // Adjust the size as needed
            noParagraph1.Add(new Chunk(uncheckedImage, 0, 0));


            // Add the "No" text to the paragraph
            noParagraph1.Add(noChunk1);
            noParagraph1.Alignment = Element.ALIGN_TOP;
            noParagraph1.Font.Size = 8;
            noParagraph1.Font.Color = BaseColor.BLACK;

            dataTable17.AddCell(noParagraph1);



            Chunk yesChunk1 = new Chunk(" Yes");
            yesChunk1.Font.Size = 8;
            var yesParagraph1 = new Paragraph();
            Image checkedImage = GetCheckboxImage(isRegistered);
            checkedImage.ScaleAbsolute(8, 8);
            yesParagraph1.Add(new Chunk(checkedImage, 0, 0));


            yesParagraph1.Add(yesChunk1);
            yesParagraph1.Alignment = Element.ALIGN_TOP;
            yesParagraph1.Font.Size = 8;
            yesParagraph1.Font.Color = BaseColor.BLACK;
            dataTable17.AddCell(yesParagraph1);


            Chunk gstyesChunk1 = new Chunk("If yes,GST number");
            var gstyesParagraph1 = new Paragraph(gstyesChunk1);
            gstyesParagraph1.Alignment = Element.ALIGN_TOP;
            gstyesParagraph1.Font.Color = BaseColor.BLACK;
            gstyesParagraph1.Font.Size = 8;
            dataTable17.AddCell(gstyesParagraph1);
            PdfPCell textcell11 = new PdfPCell();
            textcell11.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[0].GSTNumber, baseF14, 7);
            textcell11.Border = 0;
            textcell11.BackgroundColor = backgroundColor;
            dataTable17.AddCell(textcell11);

            dataTable17.AddCell("");


            // Add two empty cells to fill the remaining columns

            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable10 = new PdfPTable(1);
            emptydataTable10.WidthPercentage = 100f;
            emptydataTable10.DefaultCell.Border = 0;
            emptydataTable10.DefaultCell.FixedHeight = 7f;
            emptydataTable10.AddCell(" ");

            legendCell1.AddElement(typeParagraph1);
            legendCell1.AddElement(dataTable1);
            legendCell1.AddElement(emptydataTable1);
            legendCell1.AddElement(dataTable3);
            legendCell1.AddElement(emptydataTable2);
            legendCell1.AddElement(dataTable4);
            legendCell1.AddElement(emptydataTable3);
            legendCell1.AddElement(dataTable18);
            legendCell1.AddElement(emptydataTable4);
            legendCell1.AddElement(dataTable5);
            legendCell1.AddElement(emptydataTable5);
            legendCell1.AddElement(dataTable6);
            legendCell1.AddElement(emptydataTable6);
            legendCell1.AddElement(dataTable11);
            legendCell1.AddElement(emptydataTable7);
            legendCell1.AddElement(dataTable12);
            legendCell1.AddElement(emptydataTable8);
            legendCell1.AddElement(dataTable13);
            legendCell1.AddElement(emptydataTable9);
            legendCell1.AddElement(dataTable17);
            legendCell1.AddElement(emptydataTable10);


            fieldsetTable1.AddCell(legendCell1);
            headertable.AddCell(fieldsetTable1);
            //table1.AddCell(fieldsetTable1);
            table1.AddCell(headertable);

            // Add table 1 to the parent table cell
            PdfPCell cell1 = new PdfPCell(table1);
            cell1.Border = PdfPCell.NO_BORDER;
            parentTable.AddCell(cell1);


            // Table 2
            PdfPTable headertable1 = new PdfPTable(1);
            headertable1.WidthPercentage = 100;
            headertable1.DefaultCell.Border = 0;

            PdfPTable table2 = new PdfPTable(1);
            table2.WidthPercentage = 100f;

            table2.DefaultCell.Border = 0;
            table2.HorizontalAlignment = Element.ALIGN_RIGHT;

            // Add the fieldset box with legend
            PdfPTable fieldsetTable2 = new PdfPTable(1);
            fieldsetTable2.WidthPercentage = 100f;

            PdfPCell legendCell2 = new PdfPCell();
            legendCell2.Border = iTextSharp.text.Rectangle.BOX;
            legendCell2.BorderWidth = 2;
            legendCell2.Padding = 5;

            legendCell2.BorderColor = BaseColor.DARK_GRAY;
            legendCell2.BorderWidth = 1f;
            fieldsetTable2.AddCell(legendCell2);


            Chunk clientChunk2 = new Chunk("ClientDetails");
            var clientParagraph2 = new Paragraph(clientChunk2);
            clientParagraph2.PaddingTop = 2f;
            clientParagraph2.Alignment = Element.ALIGN_TOP;
            clientParagraph2.Font.Color = blueColor;
            clientParagraph2.Font.SetStyle(Font.BOLD);
            clientParagraph2.Font.Size = 10;
            //clientParagraph2.Font.BaseFont.FontType = Font.BOLD;
            headertable1.AddCell(clientParagraph2);

            Phrase typePhrase2 = new Phrase();
            Chunk typeChunk2 = new Chunk(" Address");
            var typeParagraph2 = new Paragraph(typeChunk1);
            typeParagraph2.PaddingTop = 2f;
            typeParagraph2.Alignment = Element.ALIGN_TOP;
            typeParagraph2.Font.Color = BaseColor.BLACK;
            typeChunk2.Font.Size = 9f;
            typeChunk2.Font.SetStyle(Font.BOLD);

            PdfPTable dataTable2 = new PdfPTable(6);
            dataTable2.WidthPercentage = 100f;
            dataTable2.DefaultCell.Border = 0;




            float[] arr3 = new float[] { 1.4f, 2.5f, 0.2f, 2.2f, 6f, 0.2f };
            dataTable2.SetWidths(arr3);

            Chunk titleChunk = new Chunk("Title");
            var titleParagraph = new Paragraph(titleChunk);
            titleParagraph.Alignment = Element.ALIGN_TOP;
            titleParagraph.Font.Color = BaseColor.BLACK;
            titleParagraph.Font.Size = 8;
            dataTable2.AddCell(titleParagraph);
            PdfPCell textcell12 = new PdfPCell();
            textcell12.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[1].Title, baseF14, 7);
            textcell12.Border = 0;
            textcell12.BackgroundColor = backgroundColor;
            dataTable2.AddCell(textcell12);

            dataTable2.AddCell("");



            Chunk surChunk = new Chunk("Surneme(s)");
            var surParagraph = new Paragraph(surChunk);
            surParagraph.Alignment = Element.ALIGN_TOP;
            surParagraph.Font.Color = BaseColor.BLACK;
            surParagraph.Font.Size = 8;
            dataTable2.AddCell(surParagraph);
            PdfPCell textcell13 = new PdfPCell();
            textcell13.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[1].SurName, baseF14, 7);
            textcell13.Border = 0;
            textcell13.BackgroundColor = backgroundColor;
            dataTable2.AddCell(textcell13);

            dataTable2.AddCell("");



            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable11 = new PdfPTable(1);
            emptydataTable11.WidthPercentage = 100f;
            emptydataTable11.DefaultCell.Border = 0;
            emptydataTable11.DefaultCell.FixedHeight = 7f;
            emptydataTable11.AddCell(" ");


            PdfPTable dataTable7 = new PdfPTable(2);
            dataTable7.WidthPercentage = 100f;
            dataTable7.DefaultCell.Border = 0;

            float[] arr9 = new float[] { 2, 4 };
            dataTable7.SetWidths(arr9);

           




            if (string.IsNullOrEmpty(clientlist[1].FirstName)) // Check if FirstName is null or empty
            {
                Chunk firChunk12 = new Chunk("Company/trust Name");
                var firParagraph12 = new Paragraph(firChunk12);
                firParagraph12.Alignment = Element.ALIGN_TOP;
                firParagraph12.Font.Size = 8;
                firParagraph12.Font.Color = BaseColor.BLACK;
                dataTable7.AddCell(firParagraph12);
                PdfPCell textcell14 = new PdfPCell();
                textcell14.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[1].CompanyTrustName, baseF14, 7);
                textcell14.Border = 0;
                textcell14.BackgroundColor = backgroundColor;
                dataTable7.AddCell(textcell14);
                dataTable7.AddCell("");


            }
            else
            {
                Chunk firChunk12 = new Chunk("First name(s)"); // Use First name(s) label
                var firParagraph12 = new Paragraph(firChunk12);
                firParagraph12.Alignment = Element.ALIGN_TOP;
                firParagraph12.Font.Size = 8;
                firParagraph12.Font.Color = BaseColor.BLACK;
                dataTable7.AddCell(firParagraph12);

                PdfPCell textcell14 = new PdfPCell();
                textcell14.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[1].FirstName, baseF14, 7);
                textcell14.Border = 0;
                textcell14.BackgroundColor = backgroundColor;
                dataTable7.AddCell(textcell14);
                dataTable7.AddCell("");
            }




            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable12 = new PdfPTable(1);
            emptydataTable12.WidthPercentage = 100f;
            emptydataTable12.DefaultCell.Border = 0;
            emptydataTable12.DefaultCell.FixedHeight = 7f;
            emptydataTable12.AddCell(" ");

            PdfPTable dataTable8 = new PdfPTable(1);
            dataTable8.WidthPercentage = 100f;
            dataTable8.DefaultCell.Border = 0;


            Chunk addChunk12 = new Chunk("Address");
            var addParagraph12 = new Paragraph(addChunk12);
            addParagraph12.Alignment = Element.ALIGN_TOP;
            addParagraph12.Font.Color = BaseColor.BLACK;
            addParagraph12.Font.Size = 8;
            dataTable8.AddCell(addParagraph12);

            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable13 = new PdfPTable(1);
            emptydataTable13.WidthPercentage = 100f;
            emptydataTable13.DefaultCell.Border = 0;
            emptydataTable13.DefaultCell.FixedHeight = 7f;
            emptydataTable13.AddCell(" ");

            PdfPTable dataTable181 = new PdfPTable(3);
            dataTable181.WidthPercentage = 100f;
            dataTable181.DefaultCell.Border = 0;
            dataTable181.DefaultCell.FixedHeight = 30f;

            float[] arr151 = new float[] { 0.1f, 7f, 0.1f };
            dataTable181.SetWidths(arr151);
            dataTable181.AddCell("");
            PdfPCell textcell15 = new PdfPCell();
            textcell15.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[1].Address, baseF14, 7);
            textcell15.Border = 0;
            textcell15.FixedHeight = 30f;
            textcell15.BackgroundColor = backgroundColor;
            dataTable181.AddCell(textcell15);

            dataTable181.AddCell("");





            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable14 = new PdfPTable(1);
            emptydataTable14.WidthPercentage = 100f;
            emptydataTable14.DefaultCell.Border = 0;
            emptydataTable14.DefaultCell.FixedHeight = 7f;
            emptydataTable14.AddCell(" ");


            PdfPTable dataTable9 = new PdfPTable(3);
            dataTable9.WidthPercentage = 100f;
            dataTable9.DefaultCell.Border = 0;

            float[] arr11 = new float[] { 0.5f, 0.9f, 1.8f };
            dataTable9.SetWidths(arr11);

            Chunk postChunk12 = new Chunk("Post code");
            var postParagraph12 = new Paragraph(postChunk12);
            postParagraph12.Alignment = Element.ALIGN_TOP;
            postParagraph12.Font.Color = BaseColor.BLACK;
            postParagraph12.Font.Size = 8;
            dataTable9.AddCell(postParagraph12);
            PdfPCell textcell16 = new PdfPCell();
            textcell16.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[1].PostCode, baseF14, 7);
            textcell16.Border = 0;
            textcell16.BackgroundColor = backgroundColor;
            dataTable9.AddCell(textcell16);

            dataTable9.AddCell("");



            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable15 = new PdfPTable(1);
            emptydataTable15.WidthPercentage = 100f;
            emptydataTable15.DefaultCell.Border = 0;
            emptydataTable15.DefaultCell.FixedHeight = 7f;
            emptydataTable15.AddCell(" ");


            PdfPTable dataTable10 = new PdfPTable(6);
            dataTable10.WidthPercentage = 100f;
            dataTable10.DefaultCell.Border = 0;

            float[] arr115 = new float[] { 1.9f, 5.5f, 0.3f, 2f, 6f, 0.2f };
            dataTable10.SetWidths(arr115);

            Chunk homeChunk12 = new Chunk("Home");
            var homeParagraph12 = new Paragraph(homeChunk12);
            homeParagraph12.Alignment = Element.ALIGN_TOP;
            homeParagraph12.Font.Size = 8;
            homeParagraph12.Font.Color = BaseColor.BLACK;
            dataTable10.AddCell(homeParagraph12);
            PdfPCell textcell17 = new PdfPCell();
            textcell17.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[1].Home, baseF14, 7);
            textcell17.Border = 0;
            textcell17.BackgroundColor = backgroundColor;
            dataTable10.AddCell(textcell17);
            dataTable10.AddCell("");



            Chunk mobileChunk12 = new Chunk("Mobile");
            var mobileParagraph12 = new Paragraph(mobileChunk12);
            mobileParagraph12.Alignment = Element.ALIGN_TOP;
            mobileParagraph12.Font.Size = 8;
            mobileParagraph12.Font.Color = BaseColor.BLACK;
            dataTable10.AddCell(mobileParagraph12);
            PdfPCell textcell18 = new PdfPCell();
            textcell18.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[1].Mobile, baseF14, 7);
            textcell18.Border = 0;
            textcell18.BackgroundColor = backgroundColor;
            dataTable10.AddCell(textcell18);
            dataTable10.AddCell("");



            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable16 = new PdfPTable(1);
            emptydataTable16.WidthPercentage = 100f;
            emptydataTable16.DefaultCell.Border = 0;
            emptydataTable16.DefaultCell.FixedHeight = 7f;
            emptydataTable16.AddCell(" ");

            PdfPTable dataTable14 = new PdfPTable(3);
            dataTable14.WidthPercentage = 100f;
            dataTable14.DefaultCell.Border = 0;

            float[] arr16 = new float[] { 0.8f, 4.9f, 0.1f };
            dataTable14.SetWidths(arr16);

            Chunk busChunk12 = new Chunk("Business");
            var busParagraph12 = new Paragraph(busChunk12);
            busParagraph12.Alignment = Element.ALIGN_TOP;
            busParagraph12.Font.Size = 8;
            busParagraph12.Font.Color = BaseColor.BLACK;
            dataTable14.AddCell(busParagraph12);
            PdfPCell textcell19 = new PdfPCell();
            textcell19.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[1].Business, baseF14, 7);
            textcell19.Border = 0;
            textcell19.BackgroundColor = backgroundColor;
            dataTable14.AddCell(textcell19);
            dataTable14.AddCell("");




            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable17 = new PdfPTable(1);
            emptydataTable17.WidthPercentage = 100f;
            emptydataTable17.DefaultCell.Border = 0;
            emptydataTable17.DefaultCell.FixedHeight = 7f;
            emptydataTable17.AddCell(" ");

            PdfPTable dataTable15 = new PdfPTable(3);
            dataTable15.WidthPercentage = 100f;
            dataTable15.DefaultCell.Border = 0;

            float[] arr17 = new float[] { 0.6f, 4.9f, 0.1f };
            dataTable15.SetWidths(arr17);

            Chunk emailChunk12 = new Chunk("Email");
            var emailParagraph12 = new Paragraph(emailChunk12);
            emailParagraph12.Alignment = Element.ALIGN_TOP;
            emailParagraph12.Font.Size = 8;
            emailParagraph12.Font.Color = BaseColor.BLACK;
            dataTable15.AddCell(emailParagraph12);
            PdfPCell textcell20 = new PdfPCell();
            textcell20.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[1].Email, baseF14, 7);
            textcell20.Border = 0;
            textcell20.BackgroundColor = backgroundColor;
            dataTable15.AddCell(textcell20);
            dataTable15.AddCell("");

            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable18 = new PdfPTable(1);
            emptydataTable18.WidthPercentage = 100f;
            emptydataTable18.DefaultCell.Border = 0;
            emptydataTable18.DefaultCell.FixedHeight = 7f;
            emptydataTable18.AddCell(" ");


            PdfPTable dataTable16 = new PdfPTable(6);
            dataTable16.WidthPercentage = 100f;
            dataTable16.DefaultCell.Border = 0;

            float[] arr18 = new float[] { 3.6f, 4.2f, 0.4f, 2f, 4f, 0.2f };
            dataTable16.SetWidths(arr18);

            





            Chunk compChunk12;
            if (!string.IsNullOrEmpty(clientlist[1].CompanyTrustName))
            {
                compChunk12 = new Chunk("Contact Person");
            }
            else
            {
                compChunk12 = new Chunk("Company/trust");
            }

            var compParagraph12 = new Paragraph(compChunk12);
            compParagraph12.Alignment = Element.ALIGN_TOP;
            compParagraph12.Font.Size = 8;
            compParagraph12.Font.Color = BaseColor.BLACK;
            dataTable16.AddCell(compParagraph12);

            PdfPCell textcell21 = new PdfPCell();
            textcell21.Border = 0;
            textcell21.BackgroundColor = backgroundColor;

            if (!string.IsNullOrEmpty(clientlist[1].CompanyTrustName))
            {
                textcell21.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[1].ContactPerson, baseF14, 7);
            }
            else
            {
                textcell21.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[1].CompanyTrustName, baseF14, 7);
            }

            dataTable16.AddCell(textcell21);
            dataTable16.AddCell("");




            Chunk positionChunk12 = new Chunk("Position");
            var positionParagraph12 = new Paragraph(positionChunk12);
            positionParagraph12.Alignment = Element.ALIGN_TOP;
            positionParagraph12.Font.Size = 8;
            positionParagraph12.Font.Color = BaseColor.BLACK;
            dataTable16.AddCell(positionParagraph12);
            PdfPCell textcell22 = new PdfPCell();
            textcell22.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[1].Position, baseF14, 7);
            textcell22.Border = 0;
            textcell22.BackgroundColor = backgroundColor;
            dataTable16.AddCell(textcell22);
            dataTable16.AddCell("");



            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable19 = new PdfPTable(1);
            emptydataTable19.WidthPercentage = 100f;
            emptydataTable19.DefaultCell.Border = 0;
            emptydataTable19.DefaultCell.FixedHeight = 7f;
            emptydataTable19.AddCell(" ");
            /*Adding Checkbox here so keep in mind*/


            PdfPTable dataTable018 = new PdfPTable(6);
            dataTable018.WidthPercentage = 100f;
            dataTable018.DefaultCell.Border = 0;

            float[] arr018 = new float[] { 1.9f, 0.9f, 0.9f, 2.1f, 1.8f, 0.1f };
            dataTable018.SetWidths(arr018);

           



            Chunk gstChunk12 = new Chunk("GST registered");
            var gstParagraph12 = new Paragraph(gstChunk12);
            gstParagraph12.Alignment = Element.ALIGN_TOP;
            gstParagraph12.Font.Size = 8;
            gstParagraph12.Font.Color = BaseColor.BLACK;
            dataTable018.AddCell(gstParagraph12);

            if (clientlist[1].ID != 0)
            {
                if (clientlist[1].IsGSTRegistered)
                {

                    isRegistered = true;
                    isNotregistered = false;

                }
                else
                {
                    isRegistered = false;
                    isNotregistered = true;
                }


            }
            else
            {
                isRegistered = false;
                isNotregistered = false;
            }

            Chunk noChunk2 = new Chunk(" No");
            noChunk2.Font.Size = 8;
            var noParagraph2 = new Paragraph();

            // Add unchecked checkbox image to the paragraph
            Image uncheckedImage2 = GetCheckboxImage(isNotregistered); // Negating the condition
            uncheckedImage2.ScaleAbsolute(8, 8); // Adjust the size as needed
            noParagraph2.Add(new Chunk(uncheckedImage2, 0, 0));

            // Add the "No" text to the paragraph
            noParagraph2.Add(noChunk2);
            noParagraph2.Alignment = Element.ALIGN_TOP;
            noParagraph2.Font.Size = 8;
            noParagraph2.Font.Color = BaseColor.BLACK;
            dataTable018.AddCell(noParagraph2);

            Chunk yesChunk2 = new Chunk(" Yes");
            yesChunk2.Font.Size = 8;
            var yesParagraph2 = new Paragraph();
            Image checkedImage2 = GetCheckboxImage(isRegistered); // Using the original condition
            checkedImage2.ScaleAbsolute(8, 8);
            yesParagraph2.Add(new Chunk(checkedImage2, 0, 0));
            yesParagraph2.Add(yesChunk2);
            yesParagraph2.Alignment = Element.ALIGN_TOP;
            yesParagraph2.Font.Size = 8;
            yesParagraph2.Font.Color = BaseColor.BLACK;
            dataTable018.AddCell(yesParagraph2);






            Chunk gstyesChunk12 = new Chunk("If yes,GST number");
            var gstyesParagraph12 = new Paragraph(gstyesChunk1);
            gstyesParagraph12.Alignment = Element.ALIGN_TOP;
            gstyesParagraph12.Font.Size = 8;
            gstyesParagraph12.Font.Color = BaseColor.BLACK;
            dataTable018.AddCell(gstyesParagraph12);
            PdfPCell textcell23 = new PdfPCell();
            textcell23.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", clientlist[1].GSTNumber, baseF14, 7);
            textcell23.Border = 0;
            textcell23.BackgroundColor = backgroundColor;
            dataTable018.AddCell(textcell23);
            dataTable018.AddCell("");



            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable20 = new PdfPTable(1);
            emptydataTable20.WidthPercentage = 100f;
            emptydataTable20.DefaultCell.Border = 0;
            emptydataTable20.DefaultCell.FixedHeight = 7f;
            emptydataTable20.AddCell(" ");

            legendCell2.AddElement(typeParagraph2);
            legendCell2.AddElement(dataTable2);
            legendCell2.AddElement(emptydataTable11);
            legendCell2.AddElement(dataTable7);
            legendCell2.AddElement(emptydataTable12);
            legendCell2.AddElement(dataTable8);
            legendCell2.AddElement(emptydataTable13);
            legendCell2.AddElement(dataTable181);
            legendCell2.AddElement(emptydataTable14);
            legendCell2.AddElement(dataTable9);
            legendCell2.AddElement(emptydataTable15);
            legendCell2.AddElement(dataTable10);
            legendCell2.AddElement(emptydataTable16);
            legendCell2.AddElement(dataTable14);
            legendCell2.AddElement(emptydataTable17);
            legendCell2.AddElement(dataTable15);
            legendCell2.AddElement(emptydataTable18);
            legendCell2.AddElement(dataTable16);
            legendCell2.AddElement(emptydataTable19);
            legendCell2.AddElement(dataTable018);
            legendCell2.AddElement(emptydataTable20);

            fieldsetTable2.AddCell(legendCell2);

            headertable1.AddCell(fieldsetTable2);
            table2.AddCell(headertable1);

            // Add table 2 to the parent table cell
            PdfPCell cell2 = new PdfPCell(table2);
            cell2.Border = PdfPCell.NO_BORDER;
            parentTable.AddCell(cell2);

            // Add the parent table to the document
            doc.Add(parentTable);


            //doc.Add(new Paragraph(" "));



        }


        public void AddContentToSolicitordetails(Document doc, PdfWriter writer, SolicitorDetail solicitorDetail)
        {

            BaseFont baseF14 = BaseFont.CreateFont();
            var blueColor = new BaseColor(43, 145, 175);
            PdfPTable headertable = new PdfPTable(1);
            headertable.WidthPercentage = 108;
            headertable.DefaultCell.Border = 0;

            Phrase solicitorPhrase = new Phrase();
            Chunk solicitorChunk = new Chunk("Solicitor details");
            var solicitorParagraph = new Paragraph(solicitorChunk);
            solicitorParagraph.PaddingTop = 2f;
            solicitorParagraph.Font.Size = 10; ;
            solicitorParagraph.Alignment = Element.ALIGN_TOP;
            solicitorParagraph.Font.Color = blueColor;
            solicitorParagraph.Font.SetStyle(Font.BOLD);

            headertable.AddCell(solicitorParagraph);

            BaseColor backgroundColor = new BaseColor(232, 232, 232);
            PdfPTable table = new PdfPTable(3);
            table.WidthPercentage = 100;


            // Add the fieldset box with legend
            PdfPTable fieldsetTable = new PdfPTable(1);
            fieldsetTable.WidthPercentage = 100;

            PdfPCell legendCell = new PdfPCell();
            legendCell.Border = iTextSharp.text.Rectangle.BOX;
            legendCell.BorderWidth = 2;
            legendCell.Padding = 5;

            // Set the event to draw text over the border

            legendCell.BorderColor = BaseColor.DARK_GRAY;
            legendCell.BorderWidth = 1f;
            fieldsetTable.AddCell(legendCell);

            //table for inside elements
            PdfPTable dataTable3 = new PdfPTable(5);
            dataTable3.WidthPercentage = 100f;
            dataTable3.DefaultCell.Border = 0;

            float[] arr5 = new float[] { 0.5f, 4f, 0.2f, 1.2f, 4f };
            dataTable3.SetWidths(arr5);

            Chunk firmChunk1 = new Chunk("Firm");
            firmChunk1.Font.Size = 8;
            var firmParagraph1 = new Paragraph(firmChunk1);
            firmParagraph1.Alignment = Element.ALIGN_TOP;
            firmParagraph1.Font.Size = 8;
            firmParagraph1.Font.Color = BaseColor.BLACK;
            dataTable3.AddCell(firmParagraph1);
            //add textfield
            PdfPCell textcell1 = new PdfPCell();
            textcell1.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", solicitorDetail.Firm, baseF14, 7);
            textcell1.Border = 0;
            textcell1.BackgroundColor = backgroundColor;
            dataTable3.AddCell(textcell1);

            //ADDED FOR EMPTY SPACE BETWEEN TEXTBOX AND LABEL 
            dataTable3.AddCell("");

            Chunk indiChunk1 = new Chunk("Individual acting");
            var indiParagraph1 = new Paragraph(indiChunk1);
            indiParagraph1.Alignment = Element.ALIGN_TOP;
            indiParagraph1.Font.Size = 8;
            indiParagraph1.Font.Color = BaseColor.BLACK;
            dataTable3.AddCell(indiParagraph1);
            //add textfield
            PdfPCell textcell2 = new PdfPCell();
            textcell2.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", solicitorDetail.IndividualActing, baseF14, 7);
            textcell2.Border = 0;
            textcell2.BackgroundColor = backgroundColor;
            dataTable3.AddCell(textcell2);

            //ADD 1 columnn table between  2 rows
            PdfPTable dataTableForRow1 = new PdfPTable(1);
            dataTableForRow1.WidthPercentage = 100f;
            dataTableForRow1.DefaultCell.Border = 0;
            dataTableForRow1.DefaultCell.FixedHeight = 7f;
            dataTableForRow1.AddCell(" ");

            PdfPTable dataTable4 = new PdfPTable(5);
            dataTable4.WidthPercentage = 100f;
            dataTable4.DefaultCell.Border = 0;

            float[] arr4 = new float[] { 0.5f, 4f, 0.2f, 0.5f, 4 };
            dataTable4.SetWidths(arr4);

            Chunk phoneChunk1 = new Chunk("Phone");
            var phoneParagraph1 = new Paragraph(phoneChunk1);
            phoneParagraph1.Alignment = Element.ALIGN_TOP;
            phoneParagraph1.Font.Size = 8;
            phoneParagraph1.Font.Color = BaseColor.BLACK;
            dataTable4.AddCell(phoneParagraph1);
            //add textfiled
            PdfPCell textcell3 = new PdfPCell();
            textcell3.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", solicitorDetail.Phone, baseF14, 7);
            textcell3.Border = 0;
            textcell3.BackgroundColor = backgroundColor;
            dataTable4.AddCell(textcell3);

            //ADDED FOR EMPTY SPACE BETWEEN TEXTBOX AND LABEL 
            dataTable4.AddCell("");

            Chunk emailChunk1 = new Chunk("Email");
            var emailParagraph1 = new Paragraph(emailChunk1);
            emailParagraph1.Alignment = Element.ALIGN_TOP;
            emailParagraph1.Font.Size = 8;
            emailParagraph1.Font.Color = BaseColor.BLACK;
            dataTable4.AddCell(emailParagraph1);
            //add textfiled
            PdfPCell textcell4 = new PdfPCell();
            textcell4.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", solicitorDetail.EmailID, baseF14, 7);
            textcell4.Border = 0;
            textcell4.BackgroundColor = backgroundColor;
            dataTable4.AddCell(textcell4);

            //ADD 1 columnn table between  2 rows
            PdfPTable dataTableForRow2 = new PdfPTable(1);
            dataTableForRow2.WidthPercentage = 100f;
            dataTableForRow2.DefaultCell.Border = 0;
            dataTableForRow2.DefaultCell.FixedHeight = 7f;
            dataTableForRow2.AddCell(" ");

            PdfPTable dataTable5 = new PdfPTable(2);
            dataTable5.WidthPercentage = 100f;
            dataTable5.DefaultCell.Border = 0;

            float[] arr0 = new float[] { 0.5f, 6.9f };
            dataTable5.SetWidths(arr0);

            Chunk addressChunk1 = new Chunk("Address");
            var addressParagraph1 = new Paragraph(addressChunk1);
            addressParagraph1.Alignment = Element.ALIGN_TOP;
            addressParagraph1.Font.Size = 8;
            addressParagraph1.Font.Color = BaseColor.BLACK;
            dataTable5.AddCell(addressParagraph1);

            //add textfiled
            PdfPCell textcell5 = new PdfPCell();
            textcell5.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", solicitorDetail.Address, baseF14, 7);
            textcell5.Border = 0;
            textcell5.BackgroundColor = backgroundColor;
            dataTable5.AddCell(textcell5);

            // legendCell.AddElement(solicitorParagraph);
            //legendCell.AddElement(solicitorPhrase);
            legendCell.AddElement(dataTable3);
            legendCell.AddElement(dataTableForRow1); //table for emply row
            legendCell.AddElement(dataTable4);
            legendCell.AddElement(dataTableForRow2); //table for emply row
            legendCell.AddElement(dataTable5);

            fieldsetTable.AddCell(legendCell);

            headertable.AddCell(fieldsetTable);
            doc.Add(headertable);
            //doc.Add(new Paragraph("\r\n"));
            //doc.Add(fieldsetTable);
        }
        public void AddContentToParticulars(Document doc, PdfWriter writer, List<ParticularDetail> particularDetail, List<PropertyAttributeTypeWithAllDataViewModel> data, List<LegalDetail> legalDetails, int id)
        {

            var particularCheckboxList = data.Where(w => w.Name == "Particulars Type").ToList();
            int propertyID = _context.PropertyAttributeType.Where(x => x.Name == "Particulars Type").Select(x => x.ID).FirstOrDefault();
            var chkboxlistForParticularsDetails = new List<PropertyAttributeTypeWithLegalParticularDetailModel>();
            using (var connection = new SqlConnection(_configuration["ConnectionStrings:ConnStr"].ToString()))
            {
                var sql = string.Format("SELECT PAT.ID as PropertyAttributeTypeID,PA.Name as PropertyAttributeName,PA.ID as PropertyAttributeID, PD.ParticularTypeID,PD.PID, case when ISNULL(PD.ParticularTypeID,0)=0 THEN 0 ELSE 1 END as Checkbox FROM PropertyAttributeType as PAT INNER JOIN PropertyAttribute as PA  ON PAT.ID = PA.PropertyAttributeTypeID LEFT JOIN ParticularDetail as PD on PA.id = PD.ParticularTypeID and  PD.PID={0}", id + " WHERE  PA.PropertyAttributeTypeID=" + propertyID);
                chkboxlistForParticularsDetails = connection.Query<PropertyAttributeTypeWithLegalParticularDetailModel>(sql).ToList();
            }

            BaseFont baseF14 = BaseFont.CreateFont();
            var blueColor = new BaseColor(43, 145, 175);
            PdfPTable headertable = new PdfPTable(1);
            headertable.WidthPercentage = 108;
            headertable.DefaultCell.Border = 0;

            Phrase typePhrase = new Phrase();
            Chunk typeChunk = new Chunk("Particulars");
            var typeParagraph = new Paragraph(typeChunk);
            typeParagraph.PaddingTop = 2f;
            typeParagraph.Alignment = Element.ALIGN_TOP;
            typeParagraph.Font.Color = blueColor;
            typeParagraph.Font.Size = 10;
            typeParagraph.Font.SetStyle(Font.BOLD);


            headertable.AddCell(typeParagraph);



            PdfPTable table = new PdfPTable(3);
            table.WidthPercentage = 100;
            BaseColor backgroundColor = new BaseColor(232, 232, 232);

            doc.Add(table);

            // Add the fieldset box with legend
            PdfPTable fieldsetTable = new PdfPTable(1);
            fieldsetTable.WidthPercentage = 100;

            PdfPCell legendCell = new PdfPCell();
            legendCell.Border = iTextSharp.text.Rectangle.BOX;
            legendCell.BorderWidth = 2;
            legendCell.Padding = 2;



            legendCell.BorderColor = BaseColor.DARK_GRAY;
            legendCell.BorderWidth = 1f;


            fieldsetTable.AddCell(legendCell);


            Phrase typePhrase1 = new Phrase();
            Chunk typeChunk1 = new Chunk("  Type");
            var typeParagraph1 = new Paragraph(typeChunk1);
            typeParagraph1.PaddingTop = 2f;
            typeParagraph1.Alignment = Element.ALIGN_TOP;
            typeParagraph1.Font.Color = BaseColor.BLACK;
            typeChunk1.Font.Size = 9f;
            typeChunk1.Font.SetStyle(Font.BOLD);



            PdfPTable dataTable1 = new PdfPTable(18);
            dataTable1.WidthPercentage = 100f;
            dataTable1.DefaultCell.Border = 0;

            float[] arr4 = new float[] { 0.3f, 3.2f, 4.7f, 1.2f, 2f, 0.5f, 1.3f, 2.3f, 0.5f, 2.1f, 2.2f, 0.5f, 1.6f, 2.1f, 0.5f, 2.9f, 2.4f, 0.1f };
            dataTable1.SetWidths(arr4);

            dataTable1.AddCell(" ");

            Chunk noChunk1 = new Chunk(chkboxlistForParticularsDetails[0].PropertyAttributeName);
            noChunk1.Font.Size = 8;
            var noParagraph1 = new Paragraph();


            Image uncheckedImage = GetCheckboxImage(chkboxlistForParticularsDetails[0].Checkbox);
            uncheckedImage.ScaleAbsolute(8, 8);
            noParagraph1.Add("\t");
            noParagraph1.Add(new Chunk(uncheckedImage, 0, 0));


            noParagraph1.Add(noChunk1);
            noParagraph1.Alignment = Element.ALIGN_TOP;
            noParagraph1.Font.Size = 8;
            noParagraph1.Font.Color = BaseColor.BLACK;

            dataTable1.AddCell(noParagraph1);


            Chunk yesChunk1 = new Chunk(chkboxlistForParticularsDetails[7].PropertyAttributeName);
            yesChunk1.Font.Size = 8;
            var yesParagraph1 = new Paragraph();


            Image checkedImage = GetCheckboxImage(chkboxlistForParticularsDetails[7].Checkbox);
            checkedImage.ScaleAbsolute(8, 8);
            yesParagraph1.Add(new Chunk(checkedImage, 0, 0));

            // Add the "Boat shed" text to the paragraph
            yesParagraph1.Add(yesChunk1);
            yesParagraph1.Alignment = Element.ALIGN_TOP;
            yesParagraph1.Font.Size = 8;
            yesParagraph1.Font.Color = BaseColor.BLACK;

            dataTable1.AddCell(yesParagraph1);



            Chunk unitChunk = new Chunk("Bed");
            var unitParagraph = new Paragraph(unitChunk);
            unitParagraph.Alignment = Element.ALIGN_TOP;
            unitParagraph.Font.Size = 8;
            unitParagraph.Font.Color = BaseColor.BLACK;
            dataTable1.AddCell(unitParagraph);
            //add textfiled
            PdfPCell textcell1 = new PdfPCell();
            textcell1.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", particularDetail[0].Bed, baseF14, 7);
            textcell1.Border = 0;
            textcell1.BackgroundColor = backgroundColor;
            dataTable1.AddCell(textcell1);

            dataTable1.AddCell(" ");


            Chunk streetChunk = new Chunk("Bath");
            var streetParagraph = new Paragraph(streetChunk);
            streetParagraph.Alignment = Element.ALIGN_TOP;
            streetParagraph.Font.Size = 8;
            streetParagraph.Font.Color = BaseColor.BLACK;
            dataTable1.AddCell(streetParagraph);
            PdfPCell textcell2 = new PdfPCell();
            textcell2.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", particularDetail[0].Bath, baseF14, 7);
            textcell2.Border = 0;
            textcell2.BackgroundColor = backgroundColor;
            dataTable1.AddCell(textcell2);

            dataTable1.AddCell(" ");


            Chunk strChunk = new Chunk("Ensuites");
            var strParagraph = new Paragraph(strChunk);
            strParagraph.Alignment = Element.ALIGN_TOP;
            strParagraph.Font.Size = 8;
            strParagraph.Font.Color = BaseColor.BLACK;
            dataTable1.AddCell(strParagraph);
            PdfPCell textcell3 = new PdfPCell();
            textcell3.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", particularDetail[0].Ensuites, baseF14, 7);
            textcell3.Border = 0;
            textcell3.BackgroundColor = backgroundColor;
            dataTable1.AddCell(textcell3);

            dataTable1.AddCell(" ");

            Chunk subhurbChunk = new Chunk("Toilets");
            var subhurbParagraph = new Paragraph(subhurbChunk);
            subhurbParagraph.Alignment = Element.ALIGN_TOP;
            subhurbParagraph.Font.Size = 8;
            subhurbParagraph.Font.Color = BaseColor.BLACK;
            dataTable1.AddCell(subhurbParagraph);
            PdfPCell textcell4 = new PdfPCell();
            textcell4.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", particularDetail[0].Toilets, baseF14, 7);
            textcell4.Border = 0;
            textcell4.BackgroundColor = backgroundColor;
            dataTable1.AddCell(textcell4);

            dataTable1.AddCell(" ");


            Chunk postChunk = new Chunk("Living rooms");
            var postParagraph = new Paragraph(postChunk);
            postParagraph.Alignment = Element.ALIGN_TOP;
            postParagraph.Font.Size = 8;
            postParagraph.Font.Color = BaseColor.BLACK;
            dataTable1.AddCell(postParagraph);
            PdfPCell textcell5 = new PdfPCell();
            textcell5.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", particularDetail[0].LivingRooms, baseF14, 7);
            textcell5.Border = 0;
            textcell5.BackgroundColor = backgroundColor;
            dataTable1.AddCell(textcell5);

            dataTable1.AddCell(" ");

            //ADDED EMPTY TABLE WITH EMPTY CELL TO GIVE SPACE BETWEEN TWO TABLES
            PdfPTable emptydataTable1 = new PdfPTable(1);
            emptydataTable1.WidthPercentage = 100f;
            emptydataTable1.DefaultCell.Border = 0;
            emptydataTable1.DefaultCell.FixedHeight = 5f;
            emptydataTable1.AddCell(" ");


            PdfPTable dataTable2 = new PdfPTable(18);
            dataTable2.WidthPercentage = 100f;
            dataTable2.DefaultCell.Border = 0;

            float[] arr5 = new float[] { 0.2f, 2.5f, 3.6f, 2.1f, 1.1f, 0.1f, 1.2f, 1.2f, 0.1f, 1.6f, 1.2f, 0.1f, 1.6f, 1.2f, 0.1f, 3.4f, 2f, 0.1f };
            dataTable2.SetWidths(arr5);

            dataTable2.AddCell(" ");
            Chunk noChunk12 = new Chunk(chkboxlistForParticularsDetails[1].PropertyAttributeName);
            noChunk12.Font.Size = 8;
            var noParagraph12 = new Paragraph();


            Image uncheckedImage2 = GetCheckboxImage(chkboxlistForParticularsDetails[1].Checkbox);
            uncheckedImage2.ScaleAbsolute(8, 8);
            noParagraph12.Add(new Chunk(uncheckedImage2, 0, 0));

            // Add the "Carpark" text to the paragraph
            noParagraph12.Add(noChunk12);
            noParagraph12.Alignment = Element.ALIGN_TOP;
            noParagraph12.Font.Size = 8;
            noParagraph12.Font.Color = BaseColor.BLACK;

            dataTable2.AddCell(noParagraph12);

            // Home & income section
            Chunk yesChunk12 = new Chunk(chkboxlistForParticularsDetails[8].PropertyAttributeName);
            yesChunk12.Font.Size = 8;
            var yesParagraph12 = new Paragraph();

            // Add checked checkbox image to the paragraph
            Image checkedImage2 = GetCheckboxImage(chkboxlistForParticularsDetails[8].Checkbox);
            checkedImage2.ScaleAbsolute(8, 8);
            yesParagraph12.Add(new Chunk(checkedImage2, 0, 0));

            // Add the "Home & income" text to the paragraph
            yesParagraph12.Add(yesChunk12);
            yesParagraph12.Alignment = Element.ALIGN_TOP;
            yesParagraph12.Font.Size = 8;
            yesParagraph12.Font.Color = BaseColor.BLACK;

            dataTable2.AddCell(yesParagraph12);


            Chunk unitChunk2 = new Chunk("Study room");
            var unitParagraph2 = new Paragraph(unitChunk2);
            unitParagraph2.Alignment = Element.ALIGN_TOP;
            unitParagraph2.Font.Size = 8;
            unitParagraph2.Font.Color = BaseColor.BLACK;
            dataTable2.AddCell(unitParagraph2);
            PdfPCell textcell6 = new PdfPCell();
            textcell6.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", particularDetail[0].StudyRooms, baseF14, 7);
            textcell6.Border = 0;
            textcell6.BackgroundColor = backgroundColor;
            dataTable2.AddCell(textcell6);

            dataTable2.AddCell(" ");


            Chunk streetChunk2 = new Chunk("Dining");
            var streetParagraph2 = new Paragraph(streetChunk2);
            streetParagraph2.Alignment = Element.ALIGN_TOP;
            streetParagraph2.Font.Size = 8;
            streetParagraph2.Font.Color = BaseColor.BLACK;
            dataTable2.AddCell(streetParagraph2);
            PdfPCell textcell7 = new PdfPCell();
            textcell7.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", particularDetail[0].Dining, baseF14, 7);
            textcell7.Border = 0;
            textcell7.BackgroundColor = backgroundColor;
            dataTable2.AddCell(textcell7);

            dataTable2.AddCell(" ");


            Chunk strChunk2 = new Chunk("Garages");
            var strParagraph2 = new Paragraph(strChunk2);
            strParagraph2.Alignment = Element.ALIGN_TOP;
            strParagraph2.Font.Size = 8;
            strParagraph2.Font.Color = BaseColor.BLACK;
            dataTable2.AddCell(strParagraph2);
            PdfPCell textcell8 = new PdfPCell();
            textcell8.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", particularDetail[0].Garages, baseF14, 7);
            textcell8.Border = 0;
            textcell8.BackgroundColor = backgroundColor;
            dataTable2.AddCell(textcell8);

            dataTable2.AddCell(" ");


            Chunk subhurbChunk2 = new Chunk("Carports");
            var subhurbParagraph2 = new Paragraph(subhurbChunk2);
            subhurbParagraph2.Alignment = Element.ALIGN_TOP;
            subhurbParagraph2.Font.Size = 8;
            subhurbParagraph2.Font.Color = BaseColor.BLACK;
            dataTable2.AddCell(subhurbParagraph2);
            PdfPCell textcell9 = new PdfPCell();
            textcell9.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", particularDetail[0].Carports, baseF14, 7);
            textcell9.Border = 0;
            textcell9.BackgroundColor = backgroundColor;
            dataTable2.AddCell(textcell9);

            dataTable2.AddCell(" ");


            Chunk postChunk2 = new Chunk("Open parking spaces");
            var postParagraph2 = new Paragraph(postChunk2);
            postParagraph2.Alignment = Element.ALIGN_TOP;
            postParagraph2.Font.Size = 8;
            postParagraph2.Font.Color = BaseColor.BLACK;
            dataTable2.AddCell(postParagraph2);
            PdfPCell textcell10 = new PdfPCell();
            textcell10.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", particularDetail[0].OpenParkingSpaces, baseF14, 7);
            textcell10.Border = 0;
            textcell10.BackgroundColor = backgroundColor;
            dataTable2.AddCell(textcell10);

            dataTable2.AddCell(" ");



            PdfPTable emptydataTable2 = new PdfPTable(1);
            emptydataTable2.WidthPercentage = 100f;
            emptydataTable2.DefaultCell.Border = 0;
            emptydataTable2.DefaultCell.FixedHeight = 5f;
            emptydataTable2.AddCell(" ");

            PdfPTable dataTable3 = new PdfPTable(5);
            dataTable3.WidthPercentage = 100f;
            dataTable3.DefaultCell.Border = 0;

            float[] arr6 = new float[] { 0.1f, 1.31f, 2f, 3f, 6f };
            dataTable3.SetWidths(arr6);

            dataTable3.AddCell(" ");

            Chunk noChunk120 = new Chunk(chkboxlistForParticularsDetails[2].PropertyAttributeName);
            noChunk120.Font.Size = 8;
            var noParagraph120 = new Paragraph();


            Image uncheckedImage20 = GetCheckboxImage(chkboxlistForParticularsDetails[2].Checkbox);
            uncheckedImage20.ScaleAbsolute(8, 8);
            noParagraph120.Add(new Chunk(uncheckedImage20, 0, 0));


            noParagraph120.Add(noChunk120);
            noParagraph120.Alignment = Element.ALIGN_TOP;
            noParagraph120.Font.Size = 8;
            noParagraph120.Font.Color = BaseColor.BLACK;

            dataTable3.AddCell(noParagraph120);





            Chunk yesChunk120 = new Chunk(chkboxlistForParticularsDetails[9].PropertyAttributeName);
            yesChunk120.Font.Size = 8;
            var yesParagraph120 = new Paragraph();


            Image checkedImage20 = GetCheckboxImage(chkboxlistForParticularsDetails[9].Checkbox);
            checkedImage20.ScaleAbsolute(8, 8);
            yesParagraph120.Add(new Chunk(checkedImage20, 0, 0));


            yesParagraph120.Add(yesChunk120);
            yesParagraph120.Alignment = Element.ALIGN_TOP;
            yesParagraph120.Font.Size = 8;
            yesParagraph120.Font.Color = BaseColor.BLACK;

            dataTable3.AddCell(yesParagraph120);


            Chunk homeChunk120 = new Chunk("  Home & land package");
            homeChunk120.Font.Size = 8;
            var homeParagraph120 = new Paragraph();


            Image uncheckedImage3 = GetCheckboxImage(particularDetail[0].IsHomeLandPackage);
            uncheckedImage3.ScaleAbsolute(8, 8);
            homeParagraph120.Add(new Chunk(uncheckedImage3, 0, 0));


            homeParagraph120.Add(homeChunk120);
            homeParagraph120.Alignment = Element.ALIGN_TOP;
            homeParagraph120.Font.Size = 8;
            homeParagraph120.Font.Color = BaseColor.BLACK;

            dataTable3.AddCell(homeParagraph120);


            Chunk newChunk120 = new Chunk("  New construction");
            newChunk120.Font.Size = 8;
            var newParagraph120 = new Paragraph();


            Image checkedImage3 = GetCheckboxImage(particularDetail[0].IsNewConstruction);
            checkedImage3.ScaleAbsolute(8, 8);
            newParagraph120.Add(new Chunk(checkedImage3, 0, 0));


            newParagraph120.Add(newChunk120);
            newParagraph120.Alignment = Element.ALIGN_TOP;
            newParagraph120.Font.Size = 8;
            newParagraph120.Font.Color = BaseColor.BLACK;

            dataTable3.AddCell(newParagraph120);



            PdfPTable emptydataTable3 = new PdfPTable(1);
            emptydataTable3.WidthPercentage = 100f;
            emptydataTable3.DefaultCell.Border = 0;
            emptydataTable3.DefaultCell.FixedHeight = 5f;
            emptydataTable3.AddCell(" ");


            PdfPTable dataTable4 = new PdfPTable(2);
            dataTable4.WidthPercentage = 100f;
            dataTable4.DefaultCell.Border = 0;

            float[] arr7 = new float[] { 0.05f, 6f };
            dataTable4.SetWidths(arr7);

            dataTable4.AddCell(" ");
            Chunk multipleChunk120 = new Chunk(chkboxlistForParticularsDetails[3].PropertyAttributeName);
            multipleChunk120.Font.Size = 8;
            var multipleParagraph120 = new Paragraph();


            Image uncheckedImage202 = GetCheckboxImage(chkboxlistForParticularsDetails[3].Checkbox);
            uncheckedImage202.ScaleAbsolute(8, 8);
            multipleParagraph120.Add(new Chunk(uncheckedImage202, 0, 0));

            // Add the "Multiple properties" text to the paragraph
            multipleParagraph120.Add(multipleChunk120);
            multipleParagraph120.Alignment = Element.ALIGN_TOP;
            multipleParagraph120.Font.Size = 8;
            multipleParagraph120.Font.Color = BaseColor.BLACK;

            dataTable4.AddCell(multipleParagraph120);

            PdfPTable dataTable5 = new PdfPTable(10);
            dataTable5.WidthPercentage = 100f;
            dataTable5.DefaultCell.Border = 0;

            float[] arr8 = new float[] { 0.28f, 7.7f, 3.5f, 3f, 1.2f, 2.5f, 3.5f, 3.4f, 2.6f, 0.1f };
            dataTable5.SetWidths(arr8);
            dataTable5.AddCell(" ");

            Chunk reChunk12 = new Chunk(chkboxlistForParticularsDetails[4].PropertyAttributeName);
            reChunk12.Font.Size = 8;
            var reParagraph12 = new Paragraph();


            Image uncheckedImage12 = GetCheckboxImage(chkboxlistForParticularsDetails[4].Checkbox);
            uncheckedImage12.ScaleAbsolute(8, 8);
            reParagraph12.Add(new Chunk(uncheckedImage12, 0, 0));


            reParagraph12.Add(reChunk12);
            reParagraph12.Alignment = Element.ALIGN_TOP;
            reParagraph12.Font.Size = 8;
            reParagraph12.Font.Color = BaseColor.BLACK;

            dataTable5.AddCell(reParagraph12);


            Chunk floorChunk2 = new Chunk("Approx. floor area");
            var floorParagraph2 = new Paragraph(floorChunk2);
            floorParagraph2.Alignment = Element.ALIGN_TOP;
            floorParagraph2.Font.Size = 8;
            floorParagraph2.Font.Color = BaseColor.BLACK;
            dataTable5.AddCell(floorParagraph2);
            PdfPCell textcell11 = new PdfPCell();
            textcell11.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", particularDetail[0].AprxFloorArea.ToString(), baseF14, 7);
            textcell11.Border = 0;
            textcell11.BackgroundColor = backgroundColor;
            dataTable5.AddCell(textcell11);



            Chunk sqmChunk2 = new Chunk("sqm");
            var sqmParagraph2 = new Paragraph(sqmChunk2);
            sqmParagraph2.Alignment = Element.ALIGN_TOP;
            sqmParagraph2.Font.Size = 8;
            sqmParagraph2.Font.Color = BaseColor.BLACK;
            dataTable5.AddCell(sqmParagraph2);


            Chunk verifiedChunk12 = new Chunk("  Verified");
            verifiedChunk12.Font.Size = 8;
            var verifiedParagraph12 = new Paragraph();


            Image checkedImage12 = GetCheckboxImage(particularDetail[0].IsVerified);
            checkedImage12.ScaleAbsolute(8, 8);
            verifiedParagraph12.Add(new Chunk(checkedImage12, 0, 0));


            verifiedParagraph12.Add(verifiedChunk12);
            verifiedParagraph12.Alignment = Element.ALIGN_TOP;
            verifiedParagraph12.Font.Size = 8;
            verifiedParagraph12.Font.Color = BaseColor.BLACK;

            dataTable5.AddCell(verifiedParagraph12);


            Chunk nonverifiedChunk12 = new Chunk("  Non Verified");
            nonverifiedChunk12.Font.Size = 8;
            var nonverifiedParagraph12 = new Paragraph();


            Image checkedImageNonverified = GetCheckboxImage(particularDetail[0].IsNonVerified);
            checkedImageNonverified.ScaleAbsolute(8, 8);
            nonverifiedParagraph12.Add(new Chunk(checkedImageNonverified, 0, 0));


            nonverifiedParagraph12.Add(nonverifiedChunk12);
            nonverifiedParagraph12.Alignment = Element.ALIGN_TOP;
            nonverifiedParagraph12.Font.Size = 8;
            nonverifiedParagraph12.Font.Color = BaseColor.BLACK;

            dataTable5.AddCell(nonverifiedParagraph12);


            Chunk yearChunk2 = new Chunk("Approx. year built ");
            var yearParagraph2 = new Paragraph(yearChunk2);
            yearParagraph2.Alignment = Element.ALIGN_TOP;
            yearParagraph2.Font.Size = 8;
            yearParagraph2.Font.Color = BaseColor.BLACK;
            dataTable5.AddCell(yearParagraph2);

            PdfPCell textcell12 = new PdfPCell();
            textcell12.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", (particularDetail[0].AprxYearBuilt == null ? " " : particularDetail[0].AprxYearBuilt.ToString()), baseF14, 7);
            textcell12.Border = 0;
            textcell12.BackgroundColor = backgroundColor;
            dataTable5.AddCell(textcell12);
            dataTable5.AddCell(" ");

            PdfPTable emptydataTable4 = new PdfPTable(1);
            emptydataTable4.WidthPercentage = 100f;
            emptydataTable4.DefaultCell.Border = 0;
            emptydataTable4.DefaultCell.FixedHeight = 5f;
            emptydataTable4.AddCell(" ");


            PdfPTable dataTable9 = new PdfPTable(10);
            dataTable9.WidthPercentage = 100f;
            dataTable9.DefaultCell.Border = 0;

            float[] arr9 = new float[] { 0.14f, 1.2f, 2f, 0.9f, 1.8f, 1f, 1.5f, 0.7f, 2.2f, 0.1f };
            dataTable9.SetWidths(arr9);

            dataTable9.AddCell(" ");


            Chunk multipleChunk = new Chunk(chkboxlistForParticularsDetails[5].PropertyAttributeName);
            multipleChunk.Font.Size = 8;
            var multipleParagraph = new Paragraph();


            Image uncheckedImage9 = GetCheckboxImage(chkboxlistForParticularsDetails[5].Checkbox);
            uncheckedImage9.ScaleAbsolute(8, 8);
            multipleParagraph.Add(new Chunk(uncheckedImage9, 0, 0));


            multipleParagraph.Add(multipleChunk);
            multipleParagraph.Alignment = Element.ALIGN_TOP;
            multipleParagraph.Font.Size = 8;
            multipleParagraph.Font.Color = BaseColor.BLACK;
            dataTable9.AddCell(multipleParagraph);

            Chunk studioChunk = new Chunk(chkboxlistForParticularsDetails[10].PropertyAttributeName);
            studioChunk.Font.Size = 8;
            var studioParagraph = new Paragraph();
            Image uncheckedImagestudio = GetCheckboxImage(chkboxlistForParticularsDetails[10].Checkbox);
            uncheckedImagestudio.ScaleAbsolute(8, 8);
            studioParagraph.Add(new Chunk(uncheckedImagestudio, 0, 0));
            studioParagraph.Add(studioChunk);
            studioParagraph.Alignment = Element.ALIGN_TOP;
            studioParagraph.Font.Size = 8;
            studioParagraph.Font.Color = BaseColor.BLACK;
            dataTable9.AddCell(studioParagraph);


            Chunk landChunk2 = new Chunk("Land area");
            var landParagraph2 = new Paragraph(landChunk2);
            landParagraph2.Alignment = Element.ALIGN_TOP;
            landParagraph2.Font.Size = 8;
            landParagraph2.Font.Color = BaseColor.BLACK;
            dataTable9.AddCell(landParagraph2);
            PdfPCell textcell13 = new PdfPCell();
            textcell13.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", legalDetails[0].LandArea, baseF14, 7);
            // textcell13.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", (particularDetail[0]. == null ? " " : particularDetail[0].LandArea.ToString()), baseF14, 7);
            textcell13.Border = 0;
            textcell13.Border = 0;
            textcell13.BackgroundColor = backgroundColor;
            dataTable9.AddCell(textcell13);



            Chunk sqmChunk12 = new Chunk(" sqm");
            sqmChunk12.Font.Size = 8;
            var sqmParagraph12 = new Paragraph();


            Image checkedImage12sqm = GetCheckboxImage(legalDetails[0].IsSqm);
            checkedImage12sqm.ScaleAbsolute(8, 8);
            sqmParagraph12.Add(new Chunk(checkedImage12sqm, 0, 0));


            sqmParagraph12.Add(sqmChunk12);
            sqmParagraph12.Alignment = Element.ALIGN_TOP;
            sqmParagraph12.Font.Size = 8;
            sqmParagraph12.Font.Color = BaseColor.BLACK;

            dataTable9.AddCell(sqmParagraph12);



            Chunk hectareChunk12 = new Chunk(" Hectare");
            hectareChunk12.Font.Size = 8;
            var hectareParagraph12 = new Paragraph();


            Image checkedImage12hectare = GetCheckboxImage(legalDetails[0].IsHectare);
            checkedImage12hectare.ScaleAbsolute(8, 8);
            hectareParagraph12.Add(new Chunk(checkedImage12hectare, 0, 0));


            hectareParagraph12.Add(hectareChunk12);
            hectareParagraph12.Alignment = Element.ALIGN_TOP;
            hectareParagraph12.Font.Size = 8;
            hectareParagraph12.Font.Color = BaseColor.BLACK;

            dataTable9.AddCell(hectareParagraph12);


            Chunk zoningChunk2 = new Chunk("Zoning");
            var zoningParagraph2 = new Paragraph(zoningChunk2);
            zoningParagraph2.Alignment = Element.ALIGN_TOP;
            zoningParagraph2.Font.Size = 8;
            zoningParagraph2.Font.Color = BaseColor.BLACK;
            dataTable9.AddCell(zoningParagraph2);
            PdfPCell textcell14 = new PdfPCell();
            textcell14.CellEvent = new SingleCellFieldPositioningEvent(writer, "cellTextBox", particularDetail[0].Zoning, baseF14, 7);
            textcell14.Border = 0;
            textcell14.BackgroundColor = backgroundColor;
            dataTable9.AddCell(textcell14);
            dataTable9.AddCell(" ");




            PdfPTable dataTable10 = new PdfPTable(3);
            dataTable10.WidthPercentage = 100f;
            dataTable10.DefaultCell.Border = 0;

            float[] arr10 = new float[] { 0.035f, 0.33f, 2.7f };
            dataTable10.SetWidths(arr10);

            dataTable10.AddCell(" ");

            Chunk townhouseChunk = new Chunk(chkboxlistForParticularsDetails[6].PropertyAttributeName);
            townhouseChunk.Font.Size = 8;
            var townhouseParagraph = new Paragraph();


            Image uncheckedImage10 = GetCheckboxImage(chkboxlistForParticularsDetails[6].Checkbox);
            uncheckedImage10.ScaleAbsolute(8, 8);
            townhouseParagraph.Add(new Chunk(uncheckedImage10, 0, 0));
            townhouseParagraph.Add(townhouseChunk);
            townhouseParagraph.Alignment = Element.ALIGN_TOP;
            townhouseParagraph.Font.Size = 8;
            townhouseParagraph.Font.Color = BaseColor.BLACK;
            dataTable10.AddCell(townhouseParagraph);

            Chunk unitChunk101 = new Chunk(chkboxlistForParticularsDetails[11].PropertyAttributeName);
            unitChunk101.Font.Size = 8;
            var unitParagraph101 = new Paragraph();


            Image uncheckedImage101 = GetCheckboxImage(chkboxlistForParticularsDetails[11].Checkbox);
            uncheckedImage101.ScaleAbsolute(8, 8);
            unitParagraph101.Add(new Chunk(uncheckedImage101, 0, 0));


            unitParagraph101.Add(unitChunk101);
            unitParagraph101.Alignment = Element.ALIGN_TOP;
            unitParagraph101.Font.Color = BaseColor.BLACK;
            dataTable10.AddCell(unitParagraph101);

            PdfPTable emptydataTable6 = new PdfPTable(1);
            emptydataTable6.WidthPercentage = 100f;
            emptydataTable6.DefaultCell.Border = 0;
            emptydataTable6.DefaultCell.FixedHeight = 5f;
            emptydataTable6.AddCell(" ");

            // legendCell.AddElement(typeParagraph);
            legendCell.AddElement(typePhrase);
            legendCell.AddElement(typeParagraph1);
            legendCell.AddElement(typePhrase1);
            legendCell.AddElement(dataTable1);
            legendCell.AddElement(emptydataTable1);
            legendCell.AddElement(dataTable2);
            legendCell.AddElement(emptydataTable2);
            legendCell.AddElement(dataTable3);
            legendCell.AddElement(emptydataTable3);
            legendCell.AddElement(dataTable4);
            legendCell.AddElement(dataTable5);
            legendCell.AddElement(emptydataTable4);
            legendCell.AddElement(dataTable9);
            // legendCell.AddElement(emptydataTable5);
            legendCell.AddElement(dataTable10);
            legendCell.AddElement(emptydataTable6);


            fieldsetTable.AddCell(legendCell);

            //doc.Add(fieldsetTable);
            headertable.AddCell(fieldsetTable);
            doc.Add(headertable);

            //// Add text fields to the PDF

            // doc.Add(new Paragraph(" "));
        }

        private static Image GetCheckboxImage(bool isChecked)
        {
            string imagePath = isChecked ? "Pictures/checked.png" : "Pictures/unchecked.png";
            return Image.GetInstance(imagePath);
        }

        private static Image GetClockImage(bool isChecked)
        {
            string imagePath = isChecked ? "Pictures/Calender.png" : "Pictures/Clock.png";
            return Image.GetInstance(imagePath);
        }

        // Helper method to get the correct value for each cell
        public static string GetEstimateCellValue(EstimateForPDF estimate, int columnIndex)
        {
            switch (columnIndex)
            {
                case 0: return estimate.ExpensesToBeIncurred;
                case 1: return estimate.ProviderDiscountCommission.ToString();
                case 2: return estimate.AmountDiscountCommission.ToString();
                default: return "";
            }
        }

        public static void CreatePdfWithImageInsideTextField(Document doc, PdfWriter writer, string imagePath, string textFieldName, int width, int height, int x, int y)
        {

            Rectangle imageTextFieldRectangle = new Rectangle(width, x, height, y);
            BaseColor backgroundColor = new BaseColor(232, 232, 232);
            imageTextFieldRectangle.BackgroundColor = backgroundColor;
            AddImageInsideTextField(doc, writer, imagePath, imageTextFieldRectangle, width, x, height, y);

        }

        public static void AddImageInsideTextField(Document doc, PdfWriter writer, string imagePath, Rectangle textFieldRectangle, int width, int x, int height, int y)
        {
            try
            {
              
                if (imagePath != null && imagePath != "")
                {
                    iTextSharp.text.Image logo1 = iTextSharp.text.Image.GetInstance(imagePath);
                    logo1.ScaleToFit(50f, 50f);
                    logo1.SetAbsolutePosition(width + 5, y - 50);

                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.AddImage(logo1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding image inside text field: " + ex.Message);
            }
        }

        public class PdfPCellEvent : IPdfPCellEvent
        {
            public PdfFormField FormField { get; set; }
            private string FieldName { get; }

            public PdfPCellEvent(string fieldName)
            {
                FieldName = fieldName;
            }

            public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
            {
                PdfWriter writer = canvases[0].PdfWriter;
                writer.AddAnnotation(FormField);


                Rectangle rect = new Rectangle(position.Left, position.Bottom, position.Right, position.Top);
                FormField.SetWidget(rect, PdfAnnotation.HIGHLIGHT_INVERT);
            }
        }

        public class RoundedCellEvent : IPdfPCellEvent
        {
            private float radius;
            private BaseColor color;
            private float width = 150;
            private float height = 30;

            public RoundedCellEvent(float radius, BaseColor color)
            {
                this.radius = radius;
                this.color = color;

            }

            public void CellLayout(
                PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
            {
                PdfContentByte canvas = canvases[PdfPTable.BACKGROUNDCANVAS];
                canvas.RoundRectangle(
                    position.Left, position.Bottom, this.width, this.height, radius);
                canvas.SetColorFill(color);
                canvas.Fill();
            }

        }

        public class RectangleCellEvent : IPdfPCellEvent
        {

            private readonly BaseColor fillColor;
            private readonly BaseColor textColor;
            private readonly string text;
            private readonly float width;
            private readonly float height;
            private readonly float positionX;
            private readonly float positionY;
            //BaseColor.YELLOW, BaseColor.BLUE, "Hello, world!", 100f, 20f, 50f, 360f
            public RectangleCellEvent(BaseColor fillColor, BaseColor textColor, string text, float positionX,
                              float height, float width, float positionY)
            {
                this.fillColor = fillColor;
                this.textColor = textColor;
                this.text = text;
                this.width = width;
                this.height = height;
                this.positionX = positionX;
                this.positionY = positionY;
            }

            public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
            {
            

                PdfContentByte canvas = canvases[PdfPTable.LINECANVAS];

              

                float thickness = 0.5f;
                //float x = 50;  // Adjust as needed
                //float y = 360;   // Adjust as needed
                //float width = 100;  // Adjust as needed
                //float height = 20;  // Adjust as needed
                // Draw the rectangle
                canvas.SaveState();
                canvas.SetColorFill(BaseColor.WHITE);
                //canvas.Rectangle(x, y, width, height);
                canvas.Rectangle(positionX, positionY, width, height);

                canvas.Fill();
                canvas.RestoreState();
                //canvas.SetColorStroke(color);
                canvas.SetLineWidth(thickness);

                PdfContentByte canvasText = canvases[PdfPTable.TEXTCANVAS];
                var blueColor = new BaseColor(43, 145, 175);
                //ColumnText.ShowTextAligned(canvasText, Element.ALIGN_LEFT,
                //    new Phrase(text, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, blueColor)),
                //    170 - 115, 357 + 20 / 2, 0);
                ColumnText.ShowTextAligned(canvasText, Element.ALIGN_LEFT,
                    new Phrase(text, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, blueColor)),
                    positionX + width - 70, positionY + height - 10, 0);

            }


        }

        public class PdfFooter : PdfPageEventHelper
        {
            // Override the OnEndPage method to add the footer
            private readonly ApplicationUser _user;
            private readonly Execution _execution;
            public PdfFooter(ApplicationUser user, Execution execution)
            {
                _user = user;
                _execution = execution;
            }
            public override void OnEndPage(PdfWriter writer, Document document)
            {




                // Set the font and size for the footer
                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Font font = new Font(baseFont, 6, Font.NORMAL, BaseColor.BLACK);

                // Create a Phrase with the footer content
                Phrase footerText1 = new Phrase(_user.CompanyName + " Listing Agency Agreement NZ " + DateTime.Now.Year.ToString() + " - Page " + writer.PageNumber + " of " + writer.PageNumber, font);
                Phrase footerText2 = new Phrase("© " + DateTime.Now.Year.ToString() + " " + _user.CompanyName, font);

                // Get the PdfContentByte object to write to the PDF
                PdfContentByte cb = writer.DirectContent;

                // Set the position of the footer
                float x1 = document.Left - 10;
                float y1 = document.Bottom - 10; // You can adjust this value to change the position of the footer

                // Add the footer to the PDF
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, footerText1, x1, y1, 0);

                float x2 = document.Left - 10;
                float y2 = document.Bottom - 20;
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, footerText2, x2, y2, 0);

                float boxWidth = 90; // Width of the signature box
                float boxHeight = 20; // Height of the signature box
                                      //float boxX = document.Right - boxWidth - 36; // Adjust the 36 to set the distance from the right margin
                float boxX = document.Right - boxWidth; // Adjust the 36 to set the distance from the right margin
                float boxY = y1 - 10; // Adjust the 10 to set the vertical position of the signature box

                ////iTextSharp.text.Image logo1 = iTextSharp.text.Image.GetInstance(_execution.AgentToSignHere);
                ////logo1.ScaleToFit(35f, 35f);
                ////logo1.SetAbsolutePosition(boxX + 10, boxY + boxHeight - 20);
                ////if (_execution != null)
                ////{
                ////    iTextSharp.text.Image logo1 = iTextSharp.text.Image.GetInstance(_execution.AgentToSignHere);
                ////    logo1.ScaleToFit(35f, 35f);
                ////    logo1.SetAbsolutePosition(boxX + 10, boxY + boxHeight - 20);
                ////}
                ////PdfContentByte contentByte = writer.DirectContent;
                ////contentByte.AddImage(logo1);
                //// Set the background color of the signature box
                ////BaseColor backgroundColor = new BaseColor(43, 145, 175); // Replace with your desired background color  
                //BaseColor backgroundColor = new BaseColor(232, 232, 232); // Replace with your desired background color  
                //PdfGState state = new PdfGState();
                ////state.FillOpacity = 0.3f;
                //cb.SetGState(state);
                //cb.SetColorFill(backgroundColor);
                ////cb.Rectangle(boxX, boxY, boxWidth, boxHeight);
                //cb.AddImage(logo1);
                //cb.Fill();
                if (_execution != null)
                {
                    iTextSharp.text.Image logo1 = iTextSharp.text.Image.GetInstance(_execution.AgentToSignHere);
                    logo1.ScaleToFit(35f, 35f);
                    logo1.SetAbsolutePosition(boxX + 10, boxY + boxHeight - 20);

                    // Set the background color of the signature box
                    BaseColor backgroundColor = new BaseColor(232, 232, 232); // Replace with your desired background color  
                    PdfGState state = new PdfGState();
                    cb.SetGState(state);
                    cb.SetColorFill(backgroundColor);
                    cb.AddImage(logo1);
                    cb.Fill();

                }

              

                Font labelFont = new Font(baseFont, 10, Font.NORMAL, BaseColor.BLACK);
                Phrase label = new Phrase("Initial Here:", labelFont);
                float labelX = boxX - 60; // Adjust the 5 to set the distance between the label and the box
                float labelY = boxY + boxHeight - 12; // Adjust the 12 to set the vertical position of the label
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, label, labelX, labelY, 0);

            }
        }

        public class SingleCellFieldPositioningEvent : IPdfPCellEvent
        {

            public TextField Field { get; set; }
            public PdfWriter Writer { get; set; }
            public float Padding { get; set; }

            public SingleCellFieldPositioningEvent(PdfWriter writer, TextField field)
            {
                this.Field = field;
                this.Writer = writer;
            }

            public SingleCellFieldPositioningEvent(PdfWriter writer, string fieldName, string text = "", BaseFont font = null, float fontSize = 8)
            {
                //The rectangle gets changed later so it doesn't matter what we use
                var rect = new iTextSharp.text.Rectangle(1, 1);

                //Create the field and set various properties
                this.Field = new TextField(writer, rect, fieldName);
                this.Field.Text = text;
                this.Field.Options = TextField.READ_ONLY;
                //this.Field.Options = TextField.READ_ONLY;
                if (null == font)
                {
                    font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                }
                this.Field.Font = font;
                this.Field.FontSize = fontSize;

                this.Writer = writer;
            }

            public void CellLayout(PdfPCell cell, iTextSharp.text.Rectangle rect, PdfContentByte[] canvases)
            {
                //Create the field's rectangle based on the current cell and requested padded
                var newRect = new PdfRectangle(rect.GetLeft(Padding), rect.GetBottom(Padding), rect.GetRight(Padding), rect.GetTop(Padding));


                //Set the appearance's rectangle to the same as the box
                this.Field.Box = newRect.Rectangle;
                //Get the raw field

                var tf = this.Field.GetTextField();

                //Change the field's rectangle
                tf.Put(PdfName.RECT, newRect);

                //Add the annotation to the writer
                Writer.AddAnnotation(tf);
            }
        }
        public class RectangleOverPdfPCellBorder : IPdfPCellEvent
        {
            private string innerText;
            private float x1;
            private float y1;
            private float w1;
            private float h1;
            private float xText;
            private float yText;



            public RectangleOverPdfPCellBorder(string text, float x, float y, float w, float h, float textx, float texty)
            {
                innerText = text;
                x1 = x;
                y1 = y;
                w1 = w;
                h1 = h;
                xText = textx;
                yText = texty;
            }
            public void CellLayout(PdfPCell cell, Rectangle rect, PdfContentByte[] canvases)
            {
                PdfContentByte canvas = canvases[PdfPTable.LINECANVAS];

                var blueColor = new BaseColor(43, 145, 175);
                // Calculate the width of the text


                // Draw a rectangle over the cell's border with text width
                canvas.Rectangle(x1, y1, w1, h1);
                canvas.SetColorFill(BaseColor.WHITE);
                canvas.Fill();
                canvas.Stroke();

                ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(innerText, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, blueColor)), xText, yText, 0);

            }
        }

        public class EstimateForPDF
        {
            public string ExpensesToBeIncurred { get; set; }
            public int ProviderDiscountCommission { get; set; }
            public int AmountDiscountCommission { get; set; }
            public bool TickHereIfEstimate { get; set; }
        }

    }
}



