using DigiDoc.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


namespace DigiDoc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DigiDoc.Helper.ConfigurationReader.readandsetConfig();
            //AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
            //MvcHandler.DisableMvcResponseHeader = true;
            //PreSendRequestHeaders += Application_PreSendRequestHeaders;
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            //log the error!
            LogHelper.Instance.Error(ex, "Application_Error","Portal","Error");
        }
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
          //Response.Headers.Remove("Server");
          //  Response.Headers.Remove("Server");
          //  Response.Headers.Remove("X-AspNet-Version");
        }
    }
}
