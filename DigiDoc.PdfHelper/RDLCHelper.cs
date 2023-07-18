using DigiDoc.PdfHelper.Model;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace DigiDoc.PdfHelper
{
    public class RDLCHelper
    {
        public static ResponseModel getAuditReportAsBase64(DataTable ReportDataSet,AuditReportParameterModel auditReportParameter)
        {
            try
            {
                
                if (System.IO.File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/RDLC/AuditReport/AuditReport.rdlc")))
                {
                    List<ReportParameter> reportParameters = new List<ReportParameter>();
                    if (!string.IsNullOrEmpty(auditReportParameter.FromDate))
                        reportParameters.Add(new ReportParameter("FromDate", auditReportParameter.FromDate));
                    if (!string.IsNullOrEmpty(auditReportParameter.ToDate))
                        reportParameters.Add(new ReportParameter("ToDate", auditReportParameter.ToDate));
                    if (!string.IsNullOrEmpty(auditReportParameter.ReportName))
                        reportParameters.Add(new ReportParameter("ReportName", auditReportParameter.ReportName));
                    ReportViewer rv = new Microsoft.Reporting.WebForms.ReportViewer();
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = System.Web.Hosting.HostingEnvironment.MapPath("~/RDLC/AuditReport/AuditReport.rdlc");

                    ReportDataSource reportParameterDatasource = new ReportDataSource();
                    reportParameterDatasource.Name = "AuditDataSet";
                    reportParameterDatasource.Value = ReportDataSet;
                    rv.LocalReport.DataSources.Add(reportParameterDatasource);
                    rv.LocalReport.SetParameters(reportParameters);

                    //System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                    //ps.Landscape = true;
                    //ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 297, 210);
                    //ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
                    //rv.SetPageSettings(ps);

                    rv.LocalReport.Refresh();
                    byte[] streamBytes = null;
                    string mimeType = "";
                    string encoding = "";
                    string filenameExtension = "";
                    string[] streamids = null;
                    Warning[] warnings = null;
                    streamBytes = rv.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                    var response = Convert.ToBase64String(streamBytes);
                    return new ResponseModel()
                    {
                        Data = response,
                        Message = "Success",
                        ResponseCode = "200",
                        Result = true
                    };
                }
                else
                {
                    return new ResponseModel()
                    {
                        Data = null,
                        Message = "RDLC file can not be found",
                        ResponseCode = "-5",
                        Result = false
                    }; 
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel()
                {
                    Data = null,
                    Message = ex.Message,
                    ResponseCode = "-1",
                    Result = false
                };
            }
        }

        public static ResponseModel getCoverLetterBase64(DataTable CoverLetterDataSet, CoverLetterParameterModel coverLetterParameterModel)
        {
            try
            {

                if (System.IO.File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/RDLC/CoverLetter/CoverLetter.rdlc")))
                {
                    List<ReportParameter> reportParameters = new List<ReportParameter>();

                    if (!string.IsNullOrEmpty(coverLetterParameterModel.ReportName))
                        reportParameters.Add(new ReportParameter("ReportName", coverLetterParameterModel.ReportName));
                    ReportViewer rv = new Microsoft.Reporting.WebForms.ReportViewer();
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = System.Web.Hosting.HostingEnvironment.MapPath("~/RDLC/CoverLetter/CoverLetter.rdlc");

                    ReportDataSource reportParameterDatasource = new ReportDataSource();
                    reportParameterDatasource.Name = "CoverLetterReportDataSet";
                    reportParameterDatasource.Value = CoverLetterDataSet;
                    rv.LocalReport.DataSources.Add(reportParameterDatasource);
                    rv.LocalReport.SetParameters(reportParameters);


                    rv.LocalReport.Refresh();
                    byte[] streamBytes = null;
                    string mimeType = "";
                    string encoding = "";
                    string filenameExtension = "";
                    string[] streamids = null;
                    Warning[] warnings = null;
                    streamBytes = rv.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                    var response = Convert.ToBase64String(streamBytes);
                    return new ResponseModel()
                    {
                        Data = response,
                        Message = "Success",
                        ResponseCode = "200",
                        Result = true
                    };
                }
                else
                {
                    return new ResponseModel()
                    {
                        Data = null,
                        Message = "RDLC file can not be found",
                        ResponseCode = "-5",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel()
                {
                    Data = null,
                    Message = ex.Message,
                    ResponseCode = "-1",
                    Result = false
                };
            }
        }


    }
}
