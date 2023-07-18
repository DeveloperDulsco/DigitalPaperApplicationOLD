using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.WebAPI.Models
{
    public class ServiceResponseModel
    {
        public bool Result { get; set; }
        public object Data { get; set; }
        public string ResponseMessage { get; set; }
        public string ResponseCode { get; set; }
    }
    public class EregResponseModel
    {

        public object responseData { get; set; }
        public bool result { get; set; }
        public string responseMessage { get; set; }
        public int statusCode { get; set; }


    }
}