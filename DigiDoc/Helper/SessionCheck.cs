using DigiDoc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigiDoc.Helper
{
    public class SessionCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool IsSessionCheck = true;
            var controllerType = filterContext.Controller.GetType();
            if (controllerType == typeof(GuestDocumentController))
            {
                // Get the action name
                string actionName = filterContext.ActionDescriptor.ActionName;
                if(actionName== "Index")
                {
                    IsSessionCheck = false;
                }
                // Apply your filter logic here using actionName
            }
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session != null && session["GuestDigiDocData"] == null && IsSessionCheck)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "Controller", "Login" },
                { "Action", "Index" }
            });
            }
        }
    }
}