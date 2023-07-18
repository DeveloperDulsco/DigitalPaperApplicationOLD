using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.Models.DPO
{
    public class UserDetailRequestDTO
    {
        public int UserID { get; set; }
        public string Message { get; set; } = null;
        public bool Success { get; set; } = false;
    }
}