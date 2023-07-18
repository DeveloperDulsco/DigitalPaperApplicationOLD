using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace DigiDoc.WebAPI.Controllers
{
    public class EmailRequest
    {
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
       
        public string Subject { get; set; }
       
        public EmailType EmailType { get; set; }
        public string displayFromEmail { get; set; }
        public string DocumentName { get; set; }
        public string UserName { get; set; }
        public string ApproverName { get; set; }
        public string AttchmentBase64 { get; set; }
        public string AttachmentFileName { get; set; }
        
    }

    public enum EmailType
    {
        Reject,
        Accept,
        SendToApproval
    }


    public class EmailResponse
    {
        public object responseData { get; set; }
        public bool result { get; set; }
        public string responseMessage { get; set; }
        public int statusCode { get; set; }
    }
}