using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigiDoc.App_Start
{
    public class AntiForgeryTokenFilter : IFilterProvider
    {
        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            List<Filter> result = new List<Filter>();

            string incomingVerb = controllerContext.HttpContext.Request.HttpMethod;
            if (actionDescriptor.ActionName == "SaveRegCardSignature" || actionDescriptor.ActionName == "SaveFolioSignature" || actionDescriptor.ActionName == "RejectReservation" ||
actionDescriptor.ActionName == "SaveRegCardSignatureWOI" ||  actionDescriptor.ActionName == "SaveAnyPDfSignature" ||  actionDescriptor.ActionName == "GetGuestDocumentListAjax")
            {
                return result;
            }
            if (String.Equals(incomingVerb, "POST", StringComparison.OrdinalIgnoreCase))
            {
                result.Add(new Filter(new ValidateAntiForgeryTokenAttribute(), FilterScope.Global, null));
            }

            return result;
        }
    }
}