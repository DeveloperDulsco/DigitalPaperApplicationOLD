
using iTextSharp.text.pdf;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using DigiDoc.PdfHelper.Model;
using iTextSharp.text.pdf.parser;
using System.Globalization;

namespace DigiDoc.PdfHelper
{
    public class PdfHelper
    {
        public static ResponseModel stampImageInPDF(string PDFBase64,string ImageBase64,float ImageScaleWidth,float ImageScaleHeight,float ImagePosX,float ImagePosY,string realName)
        {
            try
            {
                if (!string.IsNullOrEmpty(ImageBase64) && !string.IsNullOrEmpty(PDFBase64))
                {
                    byte[] imageByte = Convert.FromBase64String(ImageBase64);
                    byte[] pdfBytes = Convert.FromBase64String(PDFBase64);
                    string output = null;
                    using (PdfReader pdfReader = new PdfReader(pdfBytes))
                    {
                        Rectangle pageSize = pdfReader.GetPageSize(1);
                        float varx = pageSize.Width - (ImageScaleWidth + 20);
                        float vary = pageSize.Height - (pageSize.Height - ImageScaleHeight) - 20;
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (PdfStamper stamper = new PdfStamper(pdfReader, memoryStream))
                            {
                                PdfContentByte content = stamper.GetUnderContent(pdfReader.NumberOfPages);
                                Image image = Image.GetInstance(imageByte);
                                if(ImageScaleHeight != 0 && ImageScaleWidth != 0)
                                    image.ScaleAbsolute(ImageScaleWidth, ImageScaleHeight);
                                image.SetAbsolutePosition(varx, vary);
                                content.AddImage(image);
                                ColumnText.ShowTextAligned(content, Element.ALIGN_LEFT, new Phrase(realName), varx - 15, vary - 10, 0);
                                ColumnText.ShowTextAligned(content, Element.ALIGN_LEFT, new Phrase(DateTime.Now.ToString("dd-MM-yyyy HH:mm tt")), varx - 15, vary - 20, 0);
                                stamper.Close();
                            }
                            output = Convert.ToBase64String(memoryStream.ToArray());
                        }
                    }
                    return new ResponseModel()
                    {
                        Data = output,
                        Result = true,
                        ResponseCode = "200"
                    };
                }
                else
                {
                    return new ResponseModel()
                    {
                        Result = false,
                        Message = "Image or pdf document can not be blank",
                        ResponseCode = "-4"
                    };
                }
            }
            catch(Exception ex)
            {
                return new ResponseModel()
                {
                    ResponseCode = "-1",
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        public static ResponseModel aproveDocumentWithCoverletter(string PDFBase64, string ImageBase64, float ImageScaleWidth, float ImageScaleHeight,string RealName,DateTime dtproperty,string comment)
        {
            try
            {

               
                byte[] imageByte = Convert.FromBase64String(ImageBase64);
                byte[] pdfBytes = Convert.FromBase64String(PDFBase64);
                using (PdfReader pdfReader = new PdfReader(pdfBytes))
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentPageText = PdfTextExtractor.GetTextFromPage(pdfReader, pdfReader.NumberOfPages, strategy);
                    string resultBase64 = null;
                    if (currentPageText.Contains("Cover Letter"))
                    {
                        var parser = new PdfReaderContentParser(pdfReader);

                        var Locationstrategy = parser.ProcessContent(pdfReader.NumberOfPages, new LocationTextExtractionStrategyWithPosition());

                        var res = Locationstrategy.GetLocations();

                        var searchResult = res.Where(p => p.Text.Contains("Signature")).OrderBy(p => p.Y).Reverse().ToList();

                        using (MemoryStream ms = new MemoryStream())
                        {

                            using (PdfStamper stamper = new PdfStamper(pdfReader, ms))
                            {

                                iTextSharp.text.Rectangle rectangle = pdfReader.GetPageSizeWithRotation(pdfReader.NumberOfPages);


                                BaseFont font = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                BaseFont subfont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                PdfContentByte overContent = stamper.GetOverContent((pdfReader.NumberOfPages));

                                overContent.BeginText();
                                overContent.SetFontAndSize(subfont, 11.0f);
                                overContent.SetTextMatrix(10, searchResult.Last().Y - 40);
                                overContent.ShowText("This document is verfied and approved by ");
                                overContent.ShowTextAligned(Element.ALIGN_LEFT, $"Name : {RealName}", 10, searchResult.Last().Y - 90, 0);
                                overContent.ShowTextAligned(Element.ALIGN_LEFT, $"Date   : {dtproperty.ToString("dd/MM/yyyy hh:mm tt")}", 10, searchResult.Last().Y - 110, 0);
                                overContent.ShowTextAligned(Element.ALIGN_LEFT, $"Signature:", rectangle.Width - 200, searchResult.Last().Y - 110, 0);
                                if (!string.IsNullOrEmpty(comment))
                                {
                                    ColumnText ct = new ColumnText(overContent);
                                    ct.SetSimpleColumn(new Phrase(new Chunk($"Comment : {comment}")),
                             46, 190, 530, 36, 25, Element.ALIGN_LEFT); ct.Go();

                                    //overContent.ShowTextAligned(Element.ALIGN_LEFT, $"Comment  :{comment}", 10, searchResult.Last().Y - 130, 0);
                                }
                                    overContent.EndText();
                                Image image = Image.GetInstance(imageByte);
                                if (ImageScaleHeight != 0 && ImageScaleWidth != 0)
                                    image.ScaleAbsolute(ImageScaleWidth, ImageScaleHeight);
                                image.SetAbsolutePosition(rectangle.Width - 160, searchResult.Last().Y - 100);
                                overContent.AddImage(image);
                                
                                stamper.Close();
                            }
                            resultBase64 = Convert.ToBase64String(ms.ToArray());
                        }

                        return new ResponseModel()
                        {
                            Data = resultBase64,
                            ResponseCode = "200",
                            Result = true
                        };
                    }
                    else
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {

                            using (PdfStamper stamper = new PdfStamper(pdfReader, ms))
                            {

                                iTextSharp.text.Rectangle rectangle = pdfReader.GetPageSizeWithRotation(pdfReader.NumberOfPages);

                                stamper.InsertPage((pdfReader.NumberOfPages + 1), rectangle);
                                BaseFont font = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                BaseFont subfont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                PdfContentByte overContent = stamper.GetOverContent((pdfReader.NumberOfPages));

                                overContent.SaveState();
                                overContent.BeginText();
                                overContent.SetFontAndSize(font, 15.0f);
                                overContent.SetTextMatrix((rectangle.Width / 2) - 50, rectangle.Height - 50);
                                overContent.ShowText("Cover Letter");
                                overContent.EndText();
                                overContent.BeginText();
                                overContent.SetFontAndSize(subfont, 11.0f);
                                overContent.SetTextMatrix(10, rectangle.Height - 80);
                                overContent.ShowText("This document is verfied and approved by ");
                                overContent.ShowTextAligned(Element.ALIGN_LEFT, $"Name : {RealName}", 10, rectangle.Height - 140, 0);
                                overContent.ShowTextAligned(Element.ALIGN_LEFT, $"Date   : {dtproperty.ToString("dd/MM/yyyy hh:mm tt")}", 10, rectangle.Height - 160, 0);
                                overContent.ShowTextAligned(Element.ALIGN_LEFT, $"Applicants Signature:", rectangle.Width - 200, rectangle.Height - 160, 0);
                                if (!string.IsNullOrEmpty(comment))
                                {
                                    ColumnText ct = new ColumnText(overContent);
                                    ct.SetSimpleColumn(new Phrase(new Chunk($"Comment : {comment}")),
                   10, rectangle.Height - 180,30, 2, 160, Element.ALIGN_LEFT); ct.Go();

                                    overContent.ShowTextAligned(Element.ALIGN_LEFT, $"Comment  :{comment}", 10, rectangle.Height - 180, 0);
                                }
                                overContent.EndText();
                                Image image = Image.GetInstance(imageByte);
                                if (ImageScaleHeight != 0 && ImageScaleWidth != 0)
                                    image.ScaleAbsolute(ImageScaleWidth, ImageScaleHeight);
                                image.SetAbsolutePosition(rectangle.Width - 160, rectangle.Height - 150);
                                overContent.AddImage(image);

                                
                                overContent.RestoreState();

                                stamper.Close();
                            }
                            resultBase64 = Convert.ToBase64String(ms.ToArray());
                            
                        }
                        return new ResponseModel()
                        {
                            Data = resultBase64,
                            ResponseCode = "200",
                            Result = true
                        };
                    }
                }
               
            }
            catch(Exception ex)
            {
                return new ResponseModel()
                {
                    Result = false,
                    Message = ex.Message,
                    ResponseCode = "-1"
                };
            }
        }
        public static ResponseModel FolioTemplate(string PDFBase64, string ImageBase64, float ImageScaleWidth, float ImageScaleHeight, string RealName, DateTime dtproperty, string comment,string SignatureName,string SignatureXAxis,string SignatureYAxis)
        {
            try
            {


                byte[] imageByte = Convert.FromBase64String(ImageBase64);
                byte[] pdfBytes = Convert.FromBase64String(PDFBase64);
                using (PdfReader pdfReader = new PdfReader(pdfBytes))
                {

                    //string strText = string.Empty;
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    //for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                    //{
                    //    ITextExtractionStrategy its = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();
                    //    String s = PdfTextExtractor.GetTextFromPage(pdfReader, page, its);
                    //    s = PdfTextExtractor.GetTextFromPage(pdfReader, 1);
                    //    s = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(s)));
                    //    strText = strText + s;

                    //}
                    string resultBase64 = null;
                    for (int page = pdfReader.NumberOfPages; page >= 1; page--)
                    {
                        string currentPageText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                     

                        if (SignatureName != null && currentPageText.Contains(SignatureName))
                        {
                            var parser = new PdfReaderContentParser(pdfReader);

                            var Locationstrategy = parser.ProcessContent(page, new LocationTextExtractionStrategyWithPosition());

                            var res = Locationstrategy.GetLocations();

                            var searchResult = res.Where(p => p.Text.Contains(SignatureName)).OrderBy(p => p.Y).Reverse().ToList();


                            using (MemoryStream ms = new MemoryStream())
                            {

                                using (PdfStamper stamper = new PdfStamper(pdfReader, ms))
                                {

                                    iTextSharp.text.Rectangle rectangle = pdfReader.GetPageSizeWithRotation(1);


                                    BaseFont font = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                    BaseFont subfont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                    PdfContentByte overContent = stamper.GetOverContent((page));


                                    iTextSharp.text.Image image2 = null;
                                    System.Drawing.Bitmap bmp;
                                    using (var ms1 = new MemoryStream(imageByte))
                                    {
                                        bmp = new System.Drawing.Bitmap(ms1);
                                        bmp.MakeTransparent(System.Drawing.Color.White);
                                        image2 = iTextSharp.text.Image.GetInstance(bmp, System.Drawing.Imaging.ImageFormat.Png);
                                    }

                                    if (ImageScaleHeight != 0 && ImageScaleWidth != 0)
                                        image2.ScaleAbsolute(ImageScaleWidth, ImageScaleHeight);
                                    float.Parse(SignatureXAxis, CultureInfo.InvariantCulture.NumberFormat);
                                    float sigx = float.Parse(SignatureXAxis, CultureInfo.InvariantCulture.NumberFormat);
                                    float sigy = float.Parse(SignatureYAxis, CultureInfo.InvariantCulture.NumberFormat);
                                    float x = searchResult.Last().X + sigx;
                                    float y = searchResult.Last().Y + sigy;

                                    image2.SetAbsolutePosition(x, y);

                                    overContent.AddImage(image2);
                                    stamper.Close();
                                }
                                resultBase64 = Convert.ToBase64String(ms.ToArray());
                            }

                            return new ResponseModel()
                            {
                                Data = resultBase64,
                                ResponseCode = "200",
                                Result = true
                            };
                        }
                    }
                    if (SignatureName != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {

                            using (PdfStamper stamper = new PdfStamper(pdfReader, ms))
                            {

                                iTextSharp.text.Rectangle rectangle = pdfReader.GetPageSizeWithRotation(pdfReader.NumberOfPages);

                                stamper.InsertPage((pdfReader.NumberOfPages + 1), rectangle);
                                BaseFont font = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                BaseFont subfont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                PdfContentByte overContent = stamper.GetOverContent((pdfReader.NumberOfPages));

                                overContent.SaveState();
                                overContent.BeginText();
                                overContent.SetFontAndSize(font, 15.0f);
                                overContent.SetTextMatrix((rectangle.Width / 2) - 50, rectangle.Height - 50);
                                //overContent.ShowText("Cover Letter");
                                //overContent.EndText();
                                //overContent.BeginText();
                                //overContent.SetFontAndSize(subfont, 11.0f);
                                //overContent.SetTextMatrix(10, rectangle.Height - 80);
                                //overContent.ShowText("This document is verfied and approved by ");
                                //overContent.ShowTextAligned(Element.ALIGN_LEFT, $"Name : {RealName}", 10, rectangle.Height - 140, 0);
                                //overContent.ShowTextAligned(Element.ALIGN_LEFT, $"Date   : {dtproperty.ToString("dd/MM/yyyy hh:mm tt")}", 10, rectangle.Height - 160, 0);
                                overContent.ShowTextAligned(Element.ALIGN_LEFT, $"Signature:", rectangle.Width - 200, rectangle.Height - 160, 0);
                   //             if (!string.IsNullOrEmpty(comment))
                   //             {
                   //                 ColumnText ct = new ColumnText(overContent);
                   //                 ct.SetSimpleColumn(new Phrase(new Chunk($"Comment : {comment}")),
                   //10, rectangle.Height - 180, 30, 2, 160, Element.ALIGN_LEFT); ct.Go();

                   //                 overContent.ShowTextAligned(Element.ALIGN_LEFT, $"Comment  :{comment}", 10, rectangle.Height - 180, 0);
                   //             }
                                overContent.EndText();
                                Image image = Image.GetInstance(imageByte);
                                if (ImageScaleHeight != 0 && ImageScaleWidth != 0)
                                    image.ScaleAbsolute(ImageScaleWidth, ImageScaleHeight);
                                image.SetAbsolutePosition(rectangle.Width - 160, rectangle.Height - 150);
                                overContent.AddImage(image);


                                overContent.RestoreState();

                                stamper.Close();
                            }
                            resultBase64 = Convert.ToBase64String(ms.ToArray());

                        }
                        return new ResponseModel()
                        {
                            Data = resultBase64,
                            ResponseCode = "200",
                            Result = true
                        };
                    }
                }
                return new ResponseModel()
                {
                    Result = false,
                    Message = "PDF Reader Exit",
                    ResponseCode = "-1"
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel()
                {
                    Result = false,
                    Message = ex.Message,
                    ResponseCode = "-1"
                };
            }
        }
        public static ResponseModel RegCardTemplate(string PDFBase64, string ImageBase64, float ImageScaleWidth, float ImageScaleHeight, string RealName, DateTime dtproperty, string comment, string SignatureName, string SignatureXAxis, string SignatureYAxis, List<DocumentPdfModel> documentmodel)
        {
            try
            {
                #region Variables
                DocumentPdfModel pdf = new DocumentPdfModel();
                pdf = documentmodel.FirstOrDefault();
                string resultBase64 = null;
                iTextSharp.text.pdf.BaseFont bf = null;
                byte[] imageByte = Convert.FromBase64String(ImageBase64);
                byte[] pdfBytes = Convert.FromBase64String(PDFBase64);
                byte[] pdfBytesTemp = Convert.FromBase64String(PDFBase64);
                #endregion
                using (PdfReader pdfReaders = new PdfReader(pdfBytes))
                {
                    #region readerregion
                    BaseFont fonts = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    //string strText = string.Empty;
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    #endregion
                    #region Stamping
                    for (int page = 1; page <= pdfReaders.NumberOfPages; page++)
                    {
                        pdfBytesTemp = !String.IsNullOrEmpty(resultBase64) ? Convert.FromBase64String(resultBase64) : pdfBytes;
                        PdfReader pdfReader = new PdfReader(pdfBytesTemp);
                        var parser = new PdfReaderContentParser(pdfReader);
                        myLocationTextExtractionStrategy strategys = new myLocationTextExtractionStrategy();

                        var Locationstrategy = parser.ProcessContent(page, new LocationTextExtractionStrategyWithPosition());

                        var res = Locationstrategy.GetLocations();
                       
                        var searchResult = !string.IsNullOrEmpty(SignatureName) ? res.Where(p => p.Text.Contains(SignatureName)).OrderBy(p => p.Y).Reverse().ToList() : null;
                        var EmailAdress = !string.IsNullOrEmpty(pdf.Email) ? res.Where(p => p.Text.Contains(pdf.Email)).OrderBy(p => p.Y).Reverse().ToList() : null;
                        var phone = !string.IsNullOrEmpty(pdf.Phone) ? res.Where(p => p.Text.Contains(pdf.Phone)).OrderBy(p => p.Y).Reverse().ToList() : null;
                        var Address = !string.IsNullOrEmpty(pdf.Address) ? res.Where(p => p.Text.Contains(pdf.Address)).OrderBy(p => p.Y).Reverse().ToList() : null;
                        var state = !string.IsNullOrEmpty(pdf.State) ? res.Where(p => p.Text.Contains(pdf.State)).OrderBy(p => p.Y).Reverse().ToList() : null;
                        var country = !string.IsNullOrEmpty(pdf.Country) ? res.Where(p => p.Text.Contains(pdf.Country)).OrderBy(p => p.Y).Reverse().ToList() : null;
                        var city = !string.IsNullOrEmpty(pdf.City) ? res.Where(p => p.Text.Contains(pdf.City)).OrderBy(p => p.Y).Reverse().ToList() : null;
                        var pincode = !string.IsNullOrEmpty(pdf.PinCode) ? res.Where(p => p.Text.Contains(pdf.PinCode)).OrderBy(p => p.Y).Reverse().ToList() : null;
                        var Email2Adress = !string.IsNullOrEmpty(pdf.Email2) ? res.Where(p => p.Text.Contains(pdf.Email2)).OrderBy(p => p.Y).Reverse().ToList() : null;
                        using (MemoryStream ms = new MemoryStream())
                        {

                            using (PdfStamper stamper = new PdfStamper(pdfReader, ms))
                            {
                                #region Common Binding
                                iTextSharp.text.pdf.PdfContentByte cb2 = null;
                                cb2 = stamper.GetOverContent(page);
                                strategys.UndercontentCharacterSpacing = (int)cb2.CharacterSpacing;
                                strategys.UndercontentHorizontalScaling = (int)cb2.HorizontalScaling;


                                string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategys);
                                List<iTextSharp.text.Rectangle> MatchesFound = null;
                                int Xaxis = 0;
                                int Yaxis = 0;
                                int width = 60;
                                int height = 0;
                                int size = 9;
                                string value = "";
                                #endregion
                                #region EmailAddress PDF Merging

                                if (EmailAdress != null && EmailAdress.Count() > 0)
                                {
                                    Xaxis = !String.IsNullOrEmpty(pdf.EmailAddressxaxis) ? Convert.ToInt32(pdf.EmailAddressxaxis) : 0;
                                    Yaxis = !String.IsNullOrEmpty(pdf.EmailAddressyaxis) ? Convert.ToInt32(pdf.EmailAddressyaxis) : 0;
                                    value = !String.IsNullOrEmpty(pdf.EmailValue) ? pdf.EmailValue : "";
                                    width= !String.IsNullOrEmpty(pdf.EmailAddresswidth) ? Convert.ToInt32(pdf.EmailAddresswidth) : 60;
                                    height = !String.IsNullOrEmpty(pdf.EmailAddressheight) ? Convert.ToInt32(pdf.EmailAddressheight) : 0;
                                    size = !String.IsNullOrEmpty(pdf.EmailAddresssize) ? Convert.ToInt32(pdf.EmailAddresssize) : 0;
                                    if (!String.IsNullOrEmpty(value))
                                    {
                                        MatchesFound = strategys.GetTextLocations(pdf.Email, StringComparison.CurrentCultureIgnoreCase);


                                        cb2.SetColorFill(BaseColor.WHITE);
                                        foreach (iTextSharp.text.Rectangle rect in MatchesFound)
                                        {

                                            //width
                                            cb2.Rectangle(rect.Left + Xaxis, rect.Bottom + Yaxis, width, height==0?rect.Height:height);


                                            cb2.Fill();
                                            cb2.SetColorFill(BaseColor.BLACK);
                                            bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                                            cb2.SetFontAndSize(bf, size);

                                            cb2.BeginText();
                                            cb2.ShowTextAligned(Element.ALIGN_LEFT, pdf.EmailValue, rect.Left + Xaxis, rect.Bottom+Yaxis, 0);
                                            cb2.EndText();
                                            cb2.Fill();
                                            //Font LineBreak = FontFactory.GetFont("Arial", size);
                                            //Paragraph paragraph = new Paragraph("\n\n", LineBreak);
                                            // cb2.RestoreState();

                                        }
                                    }
                                }
                                #endregion
                                #region Phonenumber PDF Merging

                                if (phone != null && phone.Count() > 0)
                                {
                                    Xaxis = !String.IsNullOrEmpty(pdf.PhoneNumberxaxis) ? Convert.ToInt32(pdf.PhoneNumberxaxis) : 0;
                                    Yaxis = !String.IsNullOrEmpty(pdf.PhoneNumberyaxis) ? Convert.ToInt32(pdf.PhoneNumberyaxis) : 0;
                                    value = !String.IsNullOrEmpty(pdf.PhoneValue) ? pdf.PhoneValue : "";
                                   
                                    width = !String.IsNullOrEmpty(pdf.Phonewidth) ? Convert.ToInt32(pdf.Phonewidth) : 60;
                                    height = !String.IsNullOrEmpty(pdf.Phoneheight) ? Convert.ToInt32(pdf.Phoneheight) : 0;
                                    size = !String.IsNullOrEmpty(pdf.Phonesize) ? Convert.ToInt32(pdf.Phonesize) : 9;
                                    MatchesFound = strategys.GetTextLocations(pdf.Phone, StringComparison.CurrentCultureIgnoreCase);

                                    if (!String.IsNullOrEmpty(value))
                                    {
                                        cb2.SetColorFill(BaseColor.WHITE);
                                        foreach (iTextSharp.text.Rectangle rect in MatchesFound)
                                        {

                                            //width
                                            
                                            cb2.Rectangle(rect.Left + Xaxis, rect.Bottom + Yaxis, width, height==0?rect.Height:height);


                                            cb2.Fill();
                                            cb2.SetColorFill(BaseColor.BLACK);
                                            bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                                            cb2.SetFontAndSize(bf, size);

                                            cb2.BeginText();
                                            cb2.ShowTextAligned(Element.ALIGN_LEFT, pdf.PhoneValue, rect.Left + Xaxis, rect.Bottom + Yaxis, 0);
                                            cb2.EndText();
                                            cb2.Fill();
                                           
                                            //cb2.RestoreState();

                                        }
                                    }
                                }
                                #endregion
                                #region Address PDF Merging

                                if (Address != null && Address.Count() > 0)
                                {
                                    Xaxis = !String.IsNullOrEmpty(pdf.Addressxaxis) ? Convert.ToInt32(pdf.Addressxaxis) : 0;
                                    Yaxis = !String.IsNullOrEmpty(pdf.Addressyaxis) ? Convert.ToInt32(pdf.Addressyaxis) : 0;
                                    value = !String.IsNullOrEmpty(pdf.AddressValue) ? pdf.AddressValue : "";
                                    width = !String.IsNullOrEmpty(pdf.Addresswidth) ? Convert.ToInt32(pdf.Addresswidth) : 60;
                                    height = !String.IsNullOrEmpty(pdf.Addressheight) ? Convert.ToInt32(pdf.Addressheight) : 0;
                                    size = !String.IsNullOrEmpty(pdf.Addresssize) ? Convert.ToInt32(pdf.Addresssize) : 9;
                                    MatchesFound = strategys.GetTextLocations(pdf.Address, StringComparison.CurrentCultureIgnoreCase);

                                    if (!String.IsNullOrEmpty(value))
                                    {
                                        cb2.SetColorFill(BaseColor.WHITE);
                                        foreach (iTextSharp.text.Rectangle rect in MatchesFound)
                                        {

                                            //width
                                            cb2.Rectangle(rect.Left + Xaxis, rect.Bottom + Yaxis, width, height==0?rect.Height:height);


                                            cb2.Fill();
                                            cb2.SetColorFill(BaseColor.BLACK);
                                            bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                                            cb2.SetFontAndSize(bf, size);

                                            cb2.BeginText();
                                            cb2.ShowTextAligned(Element.ALIGN_LEFT, pdf.AddressValue, rect.Left + Xaxis, rect.Bottom + Yaxis, 0);
                                            cb2.EndText();
                                            cb2.Fill();
                                            //cb2.RestoreState();

                                        }
                                    }
                                }
                                #endregion
                                #region country PDF Merging

                                if (country != null && country.Count() > 0)
                                {
                                    Xaxis = !String.IsNullOrEmpty(pdf.Countryxaxis) ? Convert.ToInt32(pdf.Countryxaxis) : 0;
                                    Yaxis = !String.IsNullOrEmpty(pdf.Countryyaxis) ? Convert.ToInt32(pdf.Countryyaxis) : 0;
                                    value = !String.IsNullOrEmpty(pdf.CountryValue) ? pdf.CountryValue : "";
                                    width = !String.IsNullOrEmpty(pdf.Countrywidth) ? Convert.ToInt32(pdf.Countrywidth) : 60;
                                    height = !String.IsNullOrEmpty(pdf.Countryheight) ? Convert.ToInt32(pdf.Countryheight) : 0;
                                    size = !String.IsNullOrEmpty(pdf.Countrysize) ? Convert.ToInt32(pdf.Countrysize) : 9;
                                    MatchesFound = strategys.GetTextLocations(pdf.Country, StringComparison.CurrentCultureIgnoreCase);

                                    if (!String.IsNullOrEmpty(value))
                                    {
                                        cb2.SetColorFill(BaseColor.WHITE);
                                        foreach (iTextSharp.text.Rectangle rect in MatchesFound)
                                        {

                                            //width
                                            cb2.Rectangle(rect.Left + Xaxis, rect.Bottom + Yaxis, width,height==0?rect.Height:height);


                                            cb2.Fill();
                                            cb2.SetColorFill(BaseColor.BLACK);
                                            bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                                            cb2.SetFontAndSize(bf, size);

                                            cb2.BeginText();
                                            cb2.ShowTextAligned(Element.ALIGN_LEFT, pdf.CountryValue, rect.Left + Xaxis, rect.Bottom + Yaxis, 0);
                                            cb2.EndText();
                                            cb2.Fill();
                                            //cb2.RestoreState();

                                        }
                                    }
                                }
                                #endregion
                                #region state PDF Merging

                                if (state != null && state.Count() > 0)
                                {
                                    Xaxis = !String.IsNullOrEmpty(pdf.Statexaxis) ? Convert.ToInt32(pdf.Statexaxis) : 0;
                                    Yaxis = !String.IsNullOrEmpty(pdf.Stateyaxis) ? Convert.ToInt32(pdf.Stateyaxis) : 0;
                                    value = !String.IsNullOrEmpty(pdf.StateValue) ? pdf.StateValue : "";
                                    width = !String.IsNullOrEmpty(pdf.Statewidth) ? Convert.ToInt32(pdf.Statewidth) : 60;
                                    height = !String.IsNullOrEmpty(pdf.Stateheight) ? Convert.ToInt32(pdf.Stateheight) : 0;
                                    size = !String.IsNullOrEmpty(pdf.Statesize) ? Convert.ToInt32(pdf.Statesize) : 9;
                                    MatchesFound = strategys.GetTextLocations(pdf.State, StringComparison.CurrentCultureIgnoreCase);

                                    if (!String.IsNullOrEmpty(value))
                                    {
                                        cb2.SetColorFill(BaseColor.WHITE);
                                        foreach (iTextSharp.text.Rectangle rect in MatchesFound)
                                        {

                                            //width
                                            cb2.Rectangle(rect.Left + Xaxis, rect.Bottom + Yaxis, width,height==0?rect.Height:height);


                                            cb2.Fill();
                                            cb2.SetColorFill(BaseColor.BLACK);
                                            bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                                            cb2.SetFontAndSize(bf, size);

                                            cb2.BeginText();
                                            cb2.ShowTextAligned(Element.ALIGN_LEFT, pdf.StateValue, rect.Left + Xaxis, rect.Bottom + Yaxis, 0);
                                            cb2.EndText();
                                            cb2.Fill();
                                            //cb2.RestoreState();

                                        }
                                    }
                                }
                                #endregion
                                #region city PDF Merging

                                if (city != null && city.Count() > 0)
                                {
                                    Xaxis = !String.IsNullOrEmpty(pdf.Cityxaxis) ? Convert.ToInt32(pdf.Cityxaxis) : 0;
                                    Yaxis = !String.IsNullOrEmpty(pdf.Cityyaxis) ? Convert.ToInt32(pdf.Cityyaxis) : 0;
                                    value = !String.IsNullOrEmpty(pdf.CityValue) ? pdf.CityValue : "";
                                    width = !String.IsNullOrEmpty(pdf.Citywidth) ? Convert.ToInt32(pdf.Citywidth) : 60;
                                    height = !String.IsNullOrEmpty(pdf.Cityheight) ? Convert.ToInt32(pdf.Cityheight) : 0;
                                    size = !String.IsNullOrEmpty(pdf.Citysize) ? Convert.ToInt32(pdf.Citysize) : 9;
                                    MatchesFound = strategys.GetTextLocations(pdf.City, StringComparison.CurrentCultureIgnoreCase);

                                    if (!String.IsNullOrEmpty(value))
                                    {
                                        cb2.SetColorFill(BaseColor.WHITE);
                                        foreach (iTextSharp.text.Rectangle rect in MatchesFound)
                                        {

                                            //width
                                            cb2.Rectangle(rect.Left + Xaxis, rect.Bottom + Yaxis, width, height==0?rect.Height:height);


                                            cb2.Fill();
                                            cb2.SetColorFill(BaseColor.BLACK);
                                            bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                                            cb2.SetFontAndSize(bf, size);

                                            cb2.BeginText();
                                            cb2.ShowTextAligned(Element.ALIGN_LEFT, pdf.CityValue, rect.Left + Xaxis, rect.Bottom + Yaxis, 0);
                                            cb2.EndText();
                                            cb2.Fill();
                                            //cb2.RestoreState();

                                        }
                                    }
                                }
                                #endregion
                                #region pincode PDF Merging

                                if (pincode != null && pincode.Count() > 0)
                                {
                                    Xaxis = !String.IsNullOrEmpty(pdf.PinCodexaxis) ? Convert.ToInt32(pdf.PinCodexaxis) : 0;
                                    Yaxis = !String.IsNullOrEmpty(pdf.PinCodeyaxis) ? Convert.ToInt32(pdf.PinCodeyaxis) : 0;
                                    value = !String.IsNullOrEmpty(pdf.PinCodeValue) ? pdf.PinCodeValue : "";
                                    width = !String.IsNullOrEmpty(pdf.PinCodewidth) ? Convert.ToInt32(pdf.PinCodewidth) : 60;
                                    height = !String.IsNullOrEmpty(pdf.PinCodeheight) ? Convert.ToInt32(pdf.PinCodeheight) : 0;
                                    size = !String.IsNullOrEmpty(pdf.PinCodesize) ? Convert.ToInt32(pdf.PinCodesize) : 9;
                                    MatchesFound = strategys.GetTextLocations(pdf.PinCode, StringComparison.CurrentCultureIgnoreCase);

                                    if (!String.IsNullOrEmpty(value))
                                    {
                                        cb2.SetColorFill(BaseColor.WHITE);
                                        foreach (iTextSharp.text.Rectangle rect in MatchesFound)
                                        {

                                            //width
                                            cb2.Rectangle(rect.Left + Xaxis, rect.Bottom + Yaxis, width,height==0?rect.Height:height);


                                            cb2.Fill();
                                            cb2.SetColorFill(BaseColor.BLACK);
                                            bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                                            cb2.SetFontAndSize(bf, size);

                                            cb2.BeginText();
                                            cb2.ShowTextAligned(Element.ALIGN_LEFT, pdf.PinCodeValue, rect.Left + Xaxis, rect.Bottom + Yaxis, 0);
                                            cb2.EndText();
                                            cb2.Fill();
                                            //cb2.RestoreState();

                                        }
                                    }
                                }
                                #endregion
                                #region Email2Address PDF Merging

                                if (Email2Adress != null && Email2Adress.Count() > 0)
                                {
                                    Xaxis = !String.IsNullOrEmpty(pdf.Email2Addressxaxis) ? Convert.ToInt32(pdf.Email2Addressxaxis) : 0;
                                    Yaxis = !String.IsNullOrEmpty(pdf.Email2Addressyaxis) ? Convert.ToInt32(pdf.Email2Addressyaxis) : 0;
                                    value = !String.IsNullOrEmpty(pdf.EmailValue) ? pdf.EmailValue : "";
                                    width = !String.IsNullOrEmpty(pdf.Email2Addresswidth) ? Convert.ToInt32(pdf.Email2Addresswidth) : 60;
                                    height = !String.IsNullOrEmpty(pdf.Email2Addressheight) ? Convert.ToInt32(pdf.Email2Addressheight) : 0;
                                    size = !String.IsNullOrEmpty(pdf.Email2Addresssize) ? Convert.ToInt32(pdf.Email2Addresssize) : 0;
                                    if (!String.IsNullOrEmpty(value))
                                    {
                                        string[] split = pdf.Email2.Split();
                                        MatchesFound = strategys.GetTextLocations(split[0], StringComparison.CurrentCultureIgnoreCase);


                                        cb2.SetColorFill(BaseColor.WHITE);
                                        foreach (iTextSharp.text.Rectangle rect in MatchesFound)
                                        {

                                            //width
                                            cb2.Rectangle(rect.Left + Xaxis, rect.Bottom + Yaxis, width, height == 0 ? rect.Height : height);


                                            cb2.Fill();
                                            cb2.SetColorFill(BaseColor.BLACK);
                                            bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                                            cb2.SetFontAndSize(bf, size);

                                            cb2.BeginText();
                                            cb2.ShowTextAligned(Element.ALIGN_LEFT, pdf.EmailValue, rect.Left + Xaxis, rect.Bottom + Yaxis, 0);
                                            cb2.EndText();
                                            cb2.Fill();
                                            // cb2.RestoreState();

                                        }
                                    }
                                }
                                #endregion
                                stamper.Close();
                            }
                            resultBase64 = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    #endregion
                    #region Signature Binding
                    bool checksignature = false;
                    for (int page = pdfReaders.NumberOfPages; page >= 1; page--)
                    {
                        pdfBytesTemp = !String.IsNullOrEmpty(resultBase64) ? Convert.FromBase64String(resultBase64) : pdfBytes;
                        PdfReader pdfReadersignature = new PdfReader(pdfBytesTemp);
                        string currentPageText = PdfTextExtractor.GetTextFromPage(pdfReadersignature, page, strategy);


                        if (SignatureName != null && currentPageText.Contains(SignatureName))
                        {
                            checksignature = true;
                            var parser = new PdfReaderContentParser(pdfReadersignature);

                            var Locationstrategy = parser.ProcessContent(page, new LocationTextExtractionStrategyWithPosition());

                            var res = Locationstrategy.GetLocations();

                            var searchResult = res.Where(p => p.Text.Contains(SignatureName)).OrderBy(p => p.Y).Reverse().ToList();

                            using (MemoryStream ms = new MemoryStream())
                            {

                                using (PdfStamper stamper = new PdfStamper(pdfReadersignature, ms))
                                {

                                    iTextSharp.text.Rectangle rectangle = pdfReaders.GetPageSizeWithRotation(1);



                                    BaseFont font = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                    BaseFont subfont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                    PdfContentByte overContent = stamper.GetOverContent((page));


                                    iTextSharp.text.Image image2 = null;
                                    System.Drawing.Bitmap bmp;
                                    using (var ms1 = new MemoryStream(imageByte))
                                    {
                                        bmp = new System.Drawing.Bitmap(ms1);
                                        bmp.MakeTransparent(System.Drawing.Color.White);
                                        image2 = iTextSharp.text.Image.GetInstance(bmp, System.Drawing.Imaging.ImageFormat.Png);
                                    }

                                    if (ImageScaleHeight != 0 && ImageScaleWidth != 0)
                                        image2.ScaleAbsolute(ImageScaleWidth, ImageScaleHeight);
                                    float.Parse(SignatureXAxis, CultureInfo.InvariantCulture.NumberFormat);
                                    float sigx = float.Parse(SignatureXAxis, CultureInfo.InvariantCulture.NumberFormat);
                                    float sigy = float.Parse(SignatureYAxis, CultureInfo.InvariantCulture.NumberFormat);
                                    float x = searchResult.Last().X + sigx;
                                    float y = searchResult.Last().Y + sigy;

                                    image2.SetAbsolutePosition(x, y);

                                    overContent.AddImage(image2);

                                    stamper.Close();
                                }
                                resultBase64 = Convert.ToBase64String(ms.ToArray());
                            }

                            return new ResponseModel()
                            {
                                Data = resultBase64,
                                ResponseCode = "200",
                                Result = true
                            };
                        }
                    }
                    #endregion
                    return new ResponseModel()
                    {
                        Data = resultBase64,
                        ResponseCode = "200",
                        Result = true
                    };
                }

            }
            catch (Exception ex)
            {
                return new ResponseModel()
                {
                    Result = false,
                    Message = ex.Message,
                    ResponseCode = "-1"
                };
            }
        }

    }
}
