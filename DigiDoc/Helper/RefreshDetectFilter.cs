using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigiDoc.App_Start
{
    public class RefreshDetectFilter : ActionFilterAttribute, IActionFilter
    {
        //void  IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies["RefreshFilter"];
            filterContext.RouteData.Values["IsRefreshed"] = cookie != null &&
                                                            cookie.Value == filterContext.HttpContext.Request.Url.ToString();

            filterContext.HttpContext.Response.SetCookie(new HttpCookie("RefreshFilter", filterContext.HttpContext.Request.Url.ToString()));
        }
    }
}