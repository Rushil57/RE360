
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml;
using System.Text;

namespace RE360.API.Common
{
    public class PdfFooter : PdfPageEventHelper
    {
        // Override the OnEndPage method to add the footer
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            // Set the font and size for the footer
            BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font font = new Font(baseFont, 6, Font.NORMAL, BaseColor.BLACK);

            // Create a Phrase with the footer content
            Phrase footerText1 = new Phrase("Harcourts Residential Listing Agency Agreement NZ V2 June 2023 - Page " + writer.PageNumber + " of 8", font);
            Phrase footerText2 = new Phrase("© 2023 Harcourts Internationa", font);

            // Get the PdfContentByte object to write to the PDF
            PdfContentByte cb = writer.DirectContent;

            // Set the position of the footer
            float x1 = document.Left;
            float y1 = document.Bottom - 10; // You can adjust this value to change the position of the footer

            // Add the footer to the PDF
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, footerText1, x1, y1, 0);

            float x2 = document.Left;
            float y2 = document.Bottom - 20;
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, footerText2, x2, y2, 0);

            float boxWidth = 90; // Width of the signature box
            float boxHeight = 20; // Height of the signature box
                                  //float boxX = document.Right - boxWidth - 36; // Adjust the 36 to set the distance from the right margin
            float boxX = document.Right - boxWidth; // Adjust the 36 to set the distance from the right margin
            float boxY = y1 - 10; // Adjust the 10 to set the vertical position of the signature box


            // Set the background color of the signature box
            //BaseColor backgroundColor = new BaseColor(43, 145, 175); // Replace with your desired background color  
            BaseColor backgroundColor = new BaseColor(232, 232, 232); // Replace with your desired background color  
            PdfGState state = new PdfGState();
            //state.FillOpacity = 0.3f;
            cb.SetGState(state);
            cb.SetColorFill(backgroundColor);
            cb.Rectangle(boxX, boxY, boxWidth, boxHeight);
            cb.Fill();

            // Draw the signature box
            //cb.SetLineWidth(0f);
            //cb.Rectangle(boxX, boxY, boxWidth, boxHeight); 
            //cb.Stroke();

            Font labelFont = new Font(baseFont, 10, Font.NORMAL, BaseColor.BLACK);
            Phrase label = new Phrase("Initial Here:", labelFont);
            float labelX = boxX - 60; // Adjust the 5 to set the distance between the label and the box
            float labelY = boxY + boxHeight - 12; // Adjust the 12 to set the vertical position of the label
            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, label, labelX, labelY, 0);

        }
    }
    public class PDFHelper
    {
        public void GeneratePDF(string html, string CSS)
        {
            try
            {
                string filePath = @"D:\Projects\RE360\RE360\RE360.API\Document\test.pdf";
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                using (var doc = new Document(PageSize.A4))
                {
                    var writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                    writer.PageEvent = new PdfFooter();
                    doc.Open();

                    var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                    tagProcessors.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
                    tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor()); // use our new processor
                    CssFilesImpl cssFiles = new CssFilesImpl();
                    cssFiles.Add(XMLWorkerHelper.GetInstance().GetDefaultCSS());
                    var cssResolver = new StyleAttrCSSResolver(cssFiles);
                    cssResolver.AddCss(CSS, "utf-8", true);
                    var charset = Encoding.UTF8;
                    var hpc = new HtmlPipelineContext(new CssAppliersImpl(new XMLWorkerFontProvider()));
                    hpc.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors); // inject the tagProcessors
                    var htmlPipeline = new HtmlPipeline(hpc, new PdfWriterPipeline(doc, writer));
                    var pipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
                    var worker = new XMLWorker(pipeline, true);
                    var xmlParser = new XMLParser(true, worker, charset);
                    xmlParser.Parse(new StringReader(html));
                }
                //Process.Start("test.pdf");

                //var p = new Process();
                //p.StartInfo = new ProcessStartInfo(@"test.pdf")
                //{
                //    UseShellExecute = true
                //};
                //p.Start();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
