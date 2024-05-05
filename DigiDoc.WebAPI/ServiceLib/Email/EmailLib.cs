
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;

namespace DigiDoc.WebAPI.Controllers
{
    public class EmailLib
    {
      
        public async Task<EmailResponse> SendEmail(string ToEmail, string FromEmail, EmailType emailType, string Subject, string Username, string Password, string Host, int port, bool enableSsl,string SMTPDefaultCredentials,string displayFromEmail,string DocumentName,string UserName,string ApprovedUser,string base64Attchement, string attachmentname)
        {
            try
            {
              
                if (string.IsNullOrEmpty(FromEmail))
                    return new EmailResponse()
                    {
                        result = false,
                        responseMessage = "From email can not be blank"
                    };
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                {
                    return true;
                };
                using (MailMessage message = createMailMessage(emailType, Subject, FromEmail, displayFromEmail, DocumentName, UserName, ApprovedUser))
                {




                    if (ToEmail != null)
                    {

                        message.To.Add(new MailAddress(ToEmail));
                    }
                    else
                    {
                        return new EmailResponse()
                        {
                            result = false,
                            responseMessage = "To email address is missing"
                        };
                    }
                    if (string.IsNullOrEmpty(FromEmail))
                    {
                        return new EmailResponse()
                        {
                            result = false,
                            responseMessage = "from email address is missing"
                        };
                    }
                    using (var smtp = new SmtpClient())
                    {
                        if (!string.IsNullOrEmpty(base64Attchement))
                        {
                            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(base64Attchement)))
                            {
                                Attachment att = new Attachment(stream, attachmentname);//System.Web.HttpContext.Current.Request.MapPath("~\\Resources\\Images\\Temp\\" + Confirmation_no + "_folio.pdf"));
                                att.ContentDisposition.Inline = true;
                                message.Attachments.Add(att);

                                smtp.UseDefaultCredentials = string.IsNullOrEmpty(SMTPDefaultCredentials) ? false : SMTPDefaultCredentials.ToUpper().Equals("TRUE") ? true : false;
                                var credentials = new NetworkCredential
                                {
                                    UserName = Username,
                                    Password = Password
                                };
                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = credentials;
                                smtp.Host = Host;
                                smtp.Port = port;
                                smtp.EnableSsl = enableSsl;
                                smtp.Timeout = 10000;
                                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;


                                await smtp.SendMailAsync(message);
                                message.Dispose();
                                smtp.Dispose();
                                return new EmailResponse()
                                {
                                    result = true,
                                };
                            }
                        }
                        smtp.UseDefaultCredentials = string.IsNullOrEmpty(SMTPDefaultCredentials) ? false : SMTPDefaultCredentials.ToUpper().Equals("TRUE") ? true : false;
                        var credential = new NetworkCredential
                        {
                            UserName = Username,
                            Password = Password
                        };
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = credential;
                        smtp.Host = Host;
                        smtp.Port = port;
                        smtp.EnableSsl = enableSsl;
                        smtp.Timeout = 10000;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;


                        await smtp.SendMailAsync(message);
                        message.Dispose();
                        smtp.Dispose();
                        return new EmailResponse()
                        {
                            result = true,
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new EmailResponse()
            {
                result = false,
            };
        }


        public MailMessage createMailMessage(EmailType emailType,string Subject,string FromEmail,string displayFromEmail,string DocumentName,string UserName,string ApprovedUser)
        {
            try
            {
                if (emailType == EmailType.Accept)
                {
                    MailMessage message = new MailMessage();
                    string header_content_id = Guid.NewGuid().ToString();

                    string header_image_path = System.Web.HttpContext.Current.Request.MapPath("~\\Resources\\Images\\Logo_login.jpg");

                    string htmlBody = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Request.MapPath("~\\Resources\\HTML\\AcceptRequest.html"));
                    htmlBody = htmlBody.Replace("$$HEADER_IMAGE$$", header_content_id);

                    if (!string.IsNullOrEmpty(DocumentName))
                        htmlBody = htmlBody.Replace("$$Document_Name$$", DocumentName);
                    if (!string.IsNullOrEmpty(UserName))
                        htmlBody = htmlBody.Replace("$$USER_NAME$$", UserName);
                    if (!string.IsNullOrEmpty(ApprovedUser))
                        htmlBody = htmlBody.Replace("$$APPROVED_USER$$", ApprovedUser);
                    AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                    LinkedResource inline = new LinkedResource(header_image_path, MediaTypeNames.Image.Jpeg);
                    inline.ContentId = header_content_id;
                    avHtml.LinkedResources.Add(inline);
                 
                  
                    message.Subject = Subject;
                    message.IsBodyHtml = true;
                    message.AlternateViews.Add(avHtml);
                    message.From = new MailAddress(FromEmail, displayFromEmail);
                    return message;
                }
                else
                     if (emailType == EmailType.Reject)
                {
                    MailMessage message = new MailMessage();
                    string header_content_id = Guid.NewGuid().ToString();
                    string buton_content_id = Guid.NewGuid().ToString();
                    string header_image_path = System.Web.HttpContext.Current.Request.MapPath("~\\Resources\\Images\\Logo_login.jpg");

                    string htmlBody = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Request.MapPath("~\\Resources\\HTML\\RejectRequest.html"));

                    htmlBody = htmlBody.Replace("$$HEADER_IMAGE$$", header_content_id);
                    if (!string.IsNullOrEmpty(DocumentName))
                        htmlBody = htmlBody.Replace("$$Document_Name$$", DocumentName);
                    if (!string.IsNullOrEmpty(UserName))
                        htmlBody = htmlBody.Replace("$$USER_NAME$$", UserName);
                    if (!string.IsNullOrEmpty(ApprovedUser))
                        htmlBody = htmlBody.Replace("$$APPROVED_USER$$", ApprovedUser);

                    AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                    LinkedResource inline = new LinkedResource(header_image_path, MediaTypeNames.Image.Jpeg);
                    inline.ContentId = header_content_id;
                    avHtml.LinkedResources.Add(inline);

                   
                    message.Subject = Subject;
                    message.IsBodyHtml = true;
                    message.AlternateViews.Add(avHtml);
                    message.From = new MailAddress(FromEmail, displayFromEmail);
                    return message;
                }
                else
                if (emailType == EmailType.SendToApproval)
                {
                    MailMessage message = new MailMessage();
                    string header_content_id = Guid.NewGuid().ToString();
                    string buton_content_id = Guid.NewGuid().ToString();
                    string header_image_path = System.Web.HttpContext.Current.Request.MapPath("~\\Resources\\Images\\Logo_login.jpg");
                    //string button_image_path = System.Web.HttpContext.Current.Request.MapPath("~\\Resources\\Images\\checkout\\chkout.png");
                    string htmlBody = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Request.MapPath("~\\Resources\\HTML\\ApprovalRequest.html"));
                    //htmlBody = htmlBody.Replace("$$BUTTON_IMAGE$$", buton_content_id);
                    htmlBody = htmlBody.Replace("$$HEADER_IMAGE$$", header_content_id);
                    if (!string.IsNullOrEmpty(DocumentName))
                        htmlBody = htmlBody.Replace("$$Document_Name$$", DocumentName);
                    if (!string.IsNullOrEmpty(UserName))
                        htmlBody = htmlBody.Replace("$$USER_NAME$$", UserName);
                    if (!string.IsNullOrEmpty(ApprovedUser))
                        htmlBody = htmlBody.Replace("$$Sender_Name$$", ApprovedUser);


                    AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                    LinkedResource inline = new LinkedResource(header_image_path, MediaTypeNames.Image.Jpeg);
                    inline.ContentId = header_content_id;
                    avHtml.LinkedResources.Add(inline);
                    //inline = new LinkedResource(button_image_path, MediaTypeNames.Image.Jpeg);
                    //inline.ContentId = buton_content_id;
                    //avHtml.LinkedResources.Add(inline);

                    message.Subject = Subject;
                    message.IsBodyHtml = true;
                    message.AlternateViews.Add(avHtml);
                    message.From = new MailAddress(FromEmail, displayFromEmail);
                    return message;
                }
                else if (emailType == EmailType.GuestFolio)
                {
                    MailMessage message = new MailMessage();
                    string header_content_id = Guid.NewGuid().ToString();
                    string buton_content_id = Guid.NewGuid().ToString();
                    string header_image_path = System.Web.HttpContext.Current.Request.MapPath("~\\Resources\\Images\\Logo_login.jpg");
                    //string button_image_path = System.Web.HttpContext.Current.Request.MapPath("~\\Resources\\Images\\checkout\\chkout.png");
                    string htmlBody = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Request.MapPath("~\\Resources\\HTML\\GuestFolio.html"));
                    htmlBody = htmlBody.Replace("$$HEADER_IMAGE$$", header_content_id);
                   
                    if (!string.IsNullOrEmpty(UserName))
                        htmlBody = htmlBody.Replace("$$$$GUEST_NAME$$$$", UserName);
                  

                    AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                    LinkedResource inline = new LinkedResource(header_image_path, MediaTypeNames.Image.Jpeg);
                    inline.ContentId = header_content_id;
                    avHtml.LinkedResources.Add(inline);
                    //inline = new LinkedResource(button_image_path, MediaTypeNames.Image.Jpeg);
                    //inline.ContentId = buton_content_id;
                    //avHtml.LinkedResources.Add(inline);

                    message.Subject = Subject;
                    message.IsBodyHtml = true;
                    message.AlternateViews.Add(avHtml);
                    message.From = new MailAddress(FromEmail, displayFromEmail);
                    return message;
                }
                else
                {
                    
                }
                return null;
               
            }
            catch(Exception ex)
            {
                System.IO.File.WriteAllText(System.Web.HttpContext.Current.Request.MapPath("~\\Resources\\log.txt"),ex.ToString());
                throw ex;
            }
        }

       
    }
}