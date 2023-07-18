using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiAPI.Models
{
    public class SliderImage
    {
        public int ImgId { get; set; }
        public string ImgPath { get; set; }
    }
    public class DocumentTypeMaster
    {
        public string DocumentID { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentName { get; set; }
        public bool? IsActive { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
        public string TotalRecords { get; set; }
    }
    public class GeneralSettingsModel
    {
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}