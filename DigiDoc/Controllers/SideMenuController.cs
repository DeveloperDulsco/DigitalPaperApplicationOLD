using DigiDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigiDoc.Helper;
using DigiDoc.DataAccess.Models;

namespace DigiDoc.Controllers
{
    public class SideMenuController : BaseController
    {
        // GET: SideMenu
        public ActionResult Index()
        {
            //get logged in user from session

            //get user permission from BD

            //Build Menu list

            var sessionData = (SessionDataModel)Session["DigiDocData"];
            if(sessionData != null)
            {
                var spResponse = UtilityHelper.getUserProfileDetails(sessionData.ProfileID);
                if(spResponse != null && spResponse.result && spResponse.ResponseData != null)
                {
                    var UserProfile = (List<UserProfileModel>)spResponse.ResponseData;
                    sessionData.IsEdit = UserProfile.FirstOrDefault().IsEdit;
                    sessionData.IsDelete = UserProfile.FirstOrDefault().IsDelete;
                    sessionData.IsPrint = UserProfile.FirstOrDefault().IsPrint;
                    sessionData.IsComment = UserProfile.FirstOrDefault().IsComment;
                    sessionData.IsEditable = UserProfile.FirstOrDefault().IsEditable;
                    Session["DigiDocData"] = sessionData;
                    return View((List<UserProfileModel>)spResponse.ResponseData);
                }
            }
            return View();
        }
    }
}