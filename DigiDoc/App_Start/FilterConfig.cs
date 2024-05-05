using DigiDoc.App_Start;
using DigiDoc.Helper;
using System.Web;
using System.Web.Mvc;

namespace DigiDoc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RefreshDetectFilter());
            //filters.Add(new NoDirectAccessAttribute());
            FilterProviders.Providers.Add(new AntiForgeryTokenFilter());
            //filters.Add(new SessionCheck());
        }
    }
}
