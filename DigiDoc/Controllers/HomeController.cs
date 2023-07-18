
using DigiDoc.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using DigiDoc.DataAccess.Models;
using DigiDoc.Helper;
using DigiDoc.Models;

namespace DigiDoc.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("DocumentGroups", "Document");
        }

        public ActionResult PropertyMaster()
        {
            
            return View();
        }

        public ActionResult ProfileMaster()
        {
            
            return View();
        }

        public ActionResult UserMaster()
        {
            
            return RedirectToAction("UserList", "User");
        }

        
    }
}