using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigiDoc.Controllers
{
    public class UserProfileController : Controller
    {
        // GET: UserProfile
        public ActionResult NewUserProfile()
        {
            ViewBag.ActiveMenuName = "ProfileMaster";
            return View();
        }
    }
}