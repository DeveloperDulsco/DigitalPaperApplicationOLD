using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigiDoc.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var action = filterContext.ActionDescriptor.ActionName;
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (controller == "Login")
            {
                //if its session controller, if user is already logged in redirect to home page                
                if (Session["DigiDocData"] != null)
                {
                    if (action == "Logout")
                    {
                        //if logout call do nothing.
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                        {
                            { "Controller", "Home" },
                            { "Action", "Index" }
                        });
                    }

                }
            }
            else
            {

                if (session.IsNewSession || Session["DigiDocData"] == null)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = Json("Session Timeout", "appliation/json");
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", string.Empty);
                        ModelState.AddModelError("Password", "Session expired");
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                            { "Controller", "Login" },
                            { "Action", "Index" }
                        });
                    }
                }
            }
        }
    }
}