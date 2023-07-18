using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.Models
{
    public class CommentsRequestModel
    {
        public object DapperRow { get; set; }
        public int DocumentDetailID { get; set; }
        public int DocumentHeaderID { get; set; }
        public int DocumentID { get; set; }
        public string UserID { get; set; }
        public string Comments { get; set; }
    }
}