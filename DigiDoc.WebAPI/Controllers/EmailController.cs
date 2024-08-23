using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DigiDoc.WebAPI.Controllers
{
    public class EmailController : ApiController
    {
        [HttpPost]
        [ActionName("SendEmail")]
        public async Task<EmailResponse> SendEmail(EmailRequest emailRequest)
        {
            try
            {
               EmailLib emailLib1 = new EmailLib();
                EmailResponse emailResponse1 = await emailLib1.SendEmail(emailRequest.ToEmail, emailRequest.FromEmail, emailRequest.EmailType,  emailRequest.Subject, ConfigurationManager.AppSettings["SMTPUsername"], ConfigurationManager.AppSettings["SMTPPassword"], ConfigurationManager.AppSettings["SMTPHOST"], Int32.Parse(ConfigurationManager.AppSettings["PORT"]), bool.Parse(ConfigurationManager.AppSettings["SslEnabled"]),ConfigurationManager.AppSettings["SMTPDefaultCredentials"],emailRequest.displayFromEmail,emailRequest.DocumentName,emailRequest.UserName,emailRequest.ApproverName,emailRequest.AttchmentBase64,emailRequest.AttachmentFileName,emailRequest.RealName,emailRequest.Password);
                return emailResponse1;
            }
            catch (Exception ex)
            {
                return new EmailResponse()
                {
                    result = false,
                    responseMessage = ex.Message,
                    statusCode = -1
                };
            }

        }
    }
}
