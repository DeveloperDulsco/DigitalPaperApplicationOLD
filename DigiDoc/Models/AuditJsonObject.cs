using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.Models
{
    public class AuditJsonObject
    {
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string FieldName { get; set; }
    }
}