using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigiDoc.Controllers
{
    public class PropertiesController : Controller
    {
        // GET: Properties
        public ActionResult NewProperty()
        {
            ViewBag.ActiveMenuName = "PropertyMaster";
            return View();
        }


    }
}