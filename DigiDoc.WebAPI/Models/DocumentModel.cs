using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.WebAPI.Models
{
    public class DocumentRequestModel
    {
        public string DocumentBase64 { get; set; }

        public string Username { get; set; }

        public string DocumentName { get; set; }

        public string DocumentType { get; set; }
    }
}