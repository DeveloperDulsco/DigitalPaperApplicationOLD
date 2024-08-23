
using DigiDoc.DataAccess;
using DigiDoc.DataAccess.Models;
using DigiDoc.Helper;
using DigiDoc.Models;
using DigiDoc.Models.DPO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DigiDoc.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult NewUser(string Message = null)
        {
            NewUserModel newUser = new NewUserModel();
            newUser.UserForm = new UserFormModel();
            var spResponse = UtilityHelper.getUserProfileList();
            if (spResponse != null && spResponse.result)
            {
                newUser.userProfiles = (List<UserProfileModel>)spResponse.ResponseData;
                var securityQuestions = UtilityHelper.fetchSecuirityQuestionMaster();
                if (securityQuestions != null && securityQuestions.Count > 0)
                {

                    newUser.securityQuestions = securityQuestions;
                    spResponse = UtilityHelper.getPropertyDetails();
                    if (spResponse != null && spResponse.result)
                    {

                        newUser.propertyMasters = (List<PropertyMasterModel>)spResponse.ResponseData;

                        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                        var sessionData = (SessionDataModel)Session["DigiDocData"];
                        sessionData.MenuName = "Masters";
                        sessionData.SubMenu = "User Master";
                        Session["DigiDocData"] = sessionData;
                        AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", "Opened new user creation window");
                        ViewBag.Message = Message;
                        return View(newUser);
                    }
                    else
                    {
                        return RedirectToAction("UserList", "User", new { Message = $"Not able to fetch the property details" });
                    }
                }
                else
                {
                    return RedirectToAction("UserList", "User", new { Message = $"Not able to fetch the security questions" });
                }
            }
            else
            {
                return RedirectToAction("UserList", "User", new { Message = $"Not able to fetch the user profile list ({spResponse.ResponseMessage} code - {spResponse.ResultCode})" });
            }

        }

        [HttpPost]

        public async Task<ActionResult> NewUser(NewUserModel newUser)
        {
            ViewBag.Message = string.Empty;
            UserFormModel userModel = new UserFormModel();

            if (newUser != null)
            {
                userModel = newUser.UserForm;
                if (userModel != null && !string.IsNullOrEmpty(userModel.UpdateButon))
                {
                    if (userModel.UpdateButon.Equals("Save"))
                    {
                        if (ModelState.IsValid)
                        {
                            //if (!string.IsNullOrEmpty(userModel.UserName))
                            //{
                            //    if (!string.IsNullOrEmpty(userModel.RealName))
                            //    {
                            //        if (!string.IsNullOrEmpty(userModel.Email))
                            //        {

                            //            if (!string.IsNullOrEmpty(userModel.UserPassword))
                            //            {
                            //                if (!string.IsNullOrEmpty(userModel.ConfirmUserPassword))
                            //                {
                            //                    if (userModel.ConfirmUserPassword.Equals(userModel.UserPassword))
                            //                    {
                            //                        if (!string.IsNullOrEmpty(userModel.SecurityAnswer))
                            //                        {
                            //                            if (userModel.UserName.Length < 32)
                            //                            {
                            //                                var usernameval = new Regex(@"^[a-zA-Z]+$");

                            //                                if (usernameval.IsMatch(userModel.UserName))
                            //                                {

                            //                                    var phoneregex = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");
                            //                                    if (phoneregex.IsMatch(userModel.PhoneNumber.Trim()))
                            //                                    {
                            //                                        var password = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*]).{8,10}$");
                            //                                        if (password.IsMatch(userModel.UserPassword))
                            //                                        {
                            //                                            var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

                            //if (r.IsMatch(userModel.Email))
                            //{
                            bool IsActive = !string.IsNullOrEmpty(userModel.IsUserEnabled) ? (userModel.IsUserEnabled.ToLower().Equals("true") ? true : false) : false;
                            bool UpdatePassword = !string.IsNullOrEmpty(userModel.UpdatePassword) ? (userModel.UpdatePassword.ToLower().Equals("true") ? true : false) : false;
                            userModel.IsActive = IsActive.ToString();
                            userModel.UpdatePassword = UpdatePassword.ToString();
                            var spResponse = UtilityHelper.createUserAccount(userModel, true);
                            if (spResponse != null && spResponse.result)
                            {
                                var UserID = Convert.ToDecimal(spResponse.ResponseData);
                                userModel.UserID = Convert.ToInt32(spResponse.ResponseData);
                                spResponse = UtilityHelper.UserSecurityQuestion(userModel);
                                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                                var sessionData = (SessionDataModel)Session["DigiDocData"];
                                sessionData.MenuName = "Masters";
                                sessionData.SubMenu = "User Master";
                                Session["DigiDocData"] = sessionData;
                                AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", $"New user created - user name {userModel.UserName}");


                                //return RedirectToAction("UserList", "User", new { Message = $"User created successfully!", Success = true });
                                ViewBag.Message = "User created successfully!";
                                ViewBag.Success = true;
                                return View("UserList");
                            }
                            else if (spResponse != null)
                            {
                                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                                var sessionData = (SessionDataModel)Session["DigiDocData"];
                                sessionData.MenuName = "Masters";
                                sessionData.SubMenu = "User Master";
                                Session["DigiDocData"] = sessionData;
                                //return RedirectToAction("NewUser", "User", new { Message = $"User creation failled : {spResponse.ResponseMessage} (Error code - {spResponse.ResultCode})" });

                                ViewBag.Message = $"User creation failled : {spResponse.ResponseMessage} (Error code - {spResponse.ResultCode})";

                            }
                            else
                            {
                                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                                var sessionData = (SessionDataModel)Session["DigiDocData"];
                                sessionData.MenuName = "Masters";
                                sessionData.SubMenu = "User Master";
                                Session["DigiDocData"] = sessionData;
                                //return RedirectToAction("NewUser", "User", new { Message = $"User creation failled!" });
                                ViewBag.Message = $"User creation failled!";

                            }
                        }

                    }
                    else
                    {
                        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                        var sessionData = (SessionDataModel)Session["DigiDocData"];
                        sessionData.MenuName = "Masters";
                        sessionData.SubMenu = "User Master";
                        Session["DigiDocData"] = sessionData;
                        return View("UserList");
                    }
                }
                else
                {
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    var sessionData = (SessionDataModel)Session["DigiDocData"];
                    sessionData.MenuName = "Masters";
                    sessionData.SubMenu = "User Master";
                    Session["DigiDocData"] = sessionData;
                    return RedirectToAction("NewUser", "User", new { Message = $"Generic data validation error" });
                }

            }
            var spResponses = UtilityHelper.getUserProfileList();
            if (spResponses != null && spResponses.result)
            {
                newUser.userProfiles = (List<UserProfileModel>)spResponses.ResponseData;
                var securityQuestions = UtilityHelper.fetchSecuirityQuestionMaster();
                if (securityQuestions != null && securityQuestions.Count > 0)
                {

                    newUser.securityQuestions = securityQuestions;
                    spResponses = UtilityHelper.getPropertyDetails();
                    if (spResponses != null && spResponses.result)
                    {

                        newUser.propertyMasters = (List<PropertyMasterModel>)spResponses.ResponseData;



                        return View(newUser);
                    }
                    else
                    {
                        ViewBag.Message = "Not able to fetch the property details";
                        return View("UserList");
                    }
                }
                else
                {
                    //return RedirectToAction("UserList", "User", new { Message = $"Not able to fetch the security questions" });
                    ViewBag.Message = "Not able to fetch the security questions";
                    return View("UserList");
                }
            }
            else
            {
                //return RedirectToAction("UserList", "User", new { Message = $"Not able to fetch the user profile list ({spResponses.ResponseMessage} code - {spResponses.ResultCode})" });
                ViewBag.Message = $"Not able to fetch the user profile list ({spResponses.ResponseMessage} code - {spResponses.ResultCode})";
                return View("UserList");
            }

        }

        public ActionResult ViewUserDetails(UserDetailRequestDTO requestDTO)
        {
            int UserID = requestDTO.UserID; string Message = requestDTO.Message; bool Success = requestDTO.Success;
            var spResponse = UtilityHelper.getUserModeListDetails(UserID);
            if (spResponse != null && spResponse.result)
            {
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                var sessionData = (SessionDataModel)Session["DigiDocData"];
                sessionData.MenuName = "Masters";
                sessionData.SubMenu = "User Master";
                Session["DigiDocData"] = sessionData;
                AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", $"Viewed user details of user name : ");
                ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                ViewBag.Message = Message;
                ViewBag.Success = Success;

                var spmasterResponse = UtilityHelper.getUserProfileList();
                if (spmasterResponse != null && spmasterResponse.result)
                {
                    ViewBag.UserProfileList = (List<UserProfileModel>)spmasterResponse.ResponseData;
                }
                else
                {
                    ViewBag.UserProfileList = null;
                }


                spmasterResponse = UtilityHelper.getPropertyDetails();
                if (spmasterResponse != null && spResponse.result)
                {
                    ViewBag.PropertyDetails = (List<PropertyMasterModel>)spmasterResponse.ResponseData;
                }
                else
                {
                    ViewBag.PropertyDetails = null;
                }

                UpdateUserModel user = new UpdateUserModel();
                user = ((List<UpdateUserModel>)spResponse.ResponseData).FirstOrDefault();

                return View(user);
            }
            else if (spResponse != null)
            {

                return RedirectToAction("UserList", "User", new { Message = spResponse.ResponseMessage });
            }
            else
            {

                return RedirectToAction("UserList", "User", new { Message = "Not able to fetch the user details" });
            }
        }

        public ActionResult UserList(string Message = null, bool Success = false)
        {

            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            var sessionData = (SessionDataModel)Session["DigiDocData"];
            sessionData.MenuName = "Masters";
            sessionData.SubMenu = "User Master";
            Session["DigiDocData"] = sessionData;
            AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", "Viewed users list");

            ViewBag.Message = Message;
            ViewBag.Success = Success;

            return View();
        }

        public ActionResult GetUserListAjax(DocumentDataTableModel model, Search search)
        {

            int start = 0;

            if (model.Start > 0)
            {
                start = model.Start / model.Length;
            }

            start += 1;

            string filterby = string.Empty;
            string soryOrder = "DEC";
            string sortBy = "";
            string sortColumn = "";

            if (Request.Params["search[value]"] != null)
            {
                filterby = Request.Params["search[value]"].ToString();
            }



            if (Request.Params["order[0][column]"] != null)
            {
                sortBy = Request.Params["order[0][column]"].ToString();
            }


            if (sortBy == "0")
            {
                sortColumn = "Serial #";
            }
            else if (sortBy == "1")
            {
                sortColumn = "Name";
            }
            else if (sortBy == "2")
            {
                sortColumn = "User Name";
            }
            else if (sortBy == "3")
            {
                sortColumn = "User Profile";
            }
            else if (sortBy == "4")
            {
                sortColumn = "Status";
            }



            if (Request.Params["order[0][dir]"] != null)
            {
                soryOrder = Request.Params["order[0][dir]"].ToString();
            }
            var spResponse = UtilityHelper.FetchUserListDetails(start, model.Length, filterby, sortColumn, soryOrder);
            if (spResponse != null && spResponse.result)
            {
                var documentList = (List<UserModel>)spResponse.ResponseData;
                var TotalCount = documentList[0].TotalRecords;

                var response = new
                {
                    draw = model.draw,
                    data = documentList,
                    recordsFiltered = TotalCount,

                    recordsTotal = TotalCount
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var response = new
                {
                    draw = model.draw,

                    recordsFiltered = 0,
                    recordsTotal = 0
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }






        }
        [HttpPost]
        public ActionResult ViewUserDetails(UpdateUserModel userModel)
        {

            if (String.IsNullOrEmpty(userModel.UpdateButon))
            {
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                var sessionData = (SessionDataModel)Session["DigiDocData"];
                sessionData.MenuName = "Masters";
                sessionData.SubMenu = "User Master";
                Session["DigiDocData"] = sessionData;
                AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", "No changes made, pressed cancel buton");
                return RedirectToAction("UserList", "User");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var documentList = new List<UserModel>();
                    var spResponses = UtilityHelper.getUserListDetails();
                    if (spResponses != null && spResponses.result)
                    {
                        documentList = (List<UserModel>)spResponses.ResponseData;

                    }
                    bool IsActive = !string.IsNullOrEmpty(userModel.IsUserEnabled) ? (userModel.IsUserEnabled.ToLower().Equals("true") ? true : false) : false;
                    bool UpdatePassword = !string.IsNullOrEmpty(userModel.UpdatePassword) ? (userModel.UpdatePassword.ToLower().Equals("true") ? true : false) : false;
                    bool old_IsActive = !string.IsNullOrEmpty(userModel.IsActive) ? (userModel.IsActive.ToLower().Equals("true") ? true : false) : false;
                    bool old_UpdatePassword = !string.IsNullOrEmpty(userModel.UpdatedPasswordFlag) ? (userModel.UpdatedPasswordFlag.ToLower().Equals("true") ? true : false) : false;
                    bool checkfieldschanged = false;
                    if (!string.IsNullOrEmpty(userModel.RealName) && !string.IsNullOrEmpty(userModel.UserName) && !string.IsNullOrEmpty(userModel.UserProfileID))
                    {
                        if (IsActive != old_IsActive || UpdatePassword != old_UpdatePassword || userModel.Email != documentList[0].Email ||
                       userModel.RealName != documentList[0].RealName || userModel.UserName != documentList[0].UserName || userModel.UserProfileID != documentList[0].UserProfileID.ToString() || userModel.PhoneNumber != documentList[0].PhoneNumber)
                        {
                            checkfieldschanged = true;
                        }
                    }
                    else
                    {
                        if (IsActive != old_IsActive || UpdatePassword != old_UpdatePassword)
                        {

                            checkfieldschanged = true;
                        }

                    }

                    if (checkfieldschanged)
                    {
                        var spResponse = UtilityHelper.updateUserAccount(new UpdateUserDetailsModel()
                        {
                            IsActive = IsActive,
                            UpdatepasswordFlag = UpdatePassword,
                            UserID = userModel.UserID,
                            UserName = userModel.UserName,
                            RealName = userModel.RealName,
                            Email = userModel.Email,
                            UserProfileID = userModel.UserProfileID,
                            PhoneNumber = userModel.PhoneNumber,

                            update = true

          ,

                        });
                        if (spResponse != null && spResponse.result)
                        {
                            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                            var sessionData = (SessionDataModel)Session["DigiDocData"];
                            sessionData.MenuName = "Masters";
                            sessionData.SubMenu = "User Master";
                            Session["DigiDocData"] = sessionData;
                            AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", "User account details updated", new List<AuditJsonObject>() {
                        new AuditJsonObject()
                        {
                            FieldName = "IsActive",
                            NewValue = IsActive.ToString(),
                            OldValue = old_IsActive.ToString()
                        },
                        new AuditJsonObject()
                        {
                            FieldName = "UpdatePasswordFlag",
                            NewValue = UpdatePassword.ToString(),
                            OldValue = old_UpdatePassword.ToString()
                        }
                        });
                            //return RedirectToAction("ViewUserDetails", "User", new { Message = "Changes updated successfully!", UserID = userModel.UserID, Success = true });
                            ViewBag.Message = "Changes updated successfully!";
                            ViewBag.UserID = userModel.UserID;
                            ViewBag.Success = true;



                        }
                        else if (spResponse != null)
                        {
                            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                            var sessionData = (SessionDataModel)Session["DigiDocData"];
                            sessionData.MenuName = "Masters";
                            sessionData.SubMenu = "User Master";
                            Session["DigiDocData"] = sessionData;
                            AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", $"Failled to update user account details with reason : {spResponse.ResponseMessage}");
                            //return RedirectToAction("ViewUserDetails", "User", new { Message = $"Failled to update : {spResponse.ResponseMessage}", UserID = userModel.UserID });

                            ViewBag.Message = $"Failled to update : {spResponse.ResponseMessage}";
                            ViewBag.UserID = userModel.UserID;

                        }
                        else
                        {
                            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                            var sessionData = (SessionDataModel)Session["DigiDocData"];
                            sessionData.MenuName = "Masters";
                            sessionData.SubMenu = "User Master";
                            Session["DigiDocData"] = sessionData;
                            AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", $"Failled to update user account details ");
                            //return RedirectToAction("ViewUserDetails", "User", new { Message = "Failled to update!", UserID = userModel.UserID });
                            ViewBag.Message = $"Failled to update : {spResponse.ResponseMessage}";
                            ViewBag.UserID = userModel.UserID;
                        }
                    }
                    else
                    {
                        //return RedirectToAction("ViewUserDetails", "User", new { Message = "No changes are made to update!", UserID = userModel.UserID });


                        ViewBag.Message = "No changes are made to update!";
                        ViewBag.UserID = userModel.UserID;
                    }
                }

            }

            var spmasterResponse = UtilityHelper.getUserProfileList();
            if (spmasterResponse != null && spmasterResponse.result)
            {
                ViewBag.UserProfileList = (List<UserProfileModel>)spmasterResponse.ResponseData;
            }
            else
            {
                ViewBag.UserProfileList = null;
            }

            ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();

            spmasterResponse = UtilityHelper.getPropertyDetails();
            if (spmasterResponse != null)
            {
                ViewBag.PropertyDetails = (List<PropertyMasterModel>)spmasterResponse.ResponseData;
            }
            else
            {
                ViewBag.PropertyDetails = null;
            }

            return View(userModel);
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordModel resetPassword)
        {
            if (ModelState.IsValid)
            {
                var password = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*]).{8,16}$");
                if (password.IsMatch(resetPassword.NewPassword))
                {
                    if (password.IsMatch(resetPassword.RetypedNewPassword))
                    {
                        if (resetPassword.NewPassword.Equals(resetPassword.RetypedNewPassword))
                        {


                            var resetPasswordResponse = UtilityHelper.resetPasswordByUserID(resetPassword);
                            if (resetPasswordResponse != null
                                && resetPasswordResponse.result)
                            {
                                LogHelper.Instance.Debug($"user name- {resetPassword.UserName}, password got resetted", "Reset Password", "Portal", "Master");
                                AuditHelper.InsertAuditLog("Masters", resetPassword.UserName, "Succesfully reseted the password");
                                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                                var sessionData = (SessionDataModel)Session["DigiDocData"];
                                sessionData.MenuName = "Masters";
                                sessionData.SubMenu = "User Master";
                                Session["DigiDocData"] = sessionData;
                                return Json(new { Result = true, Message = "Password changed succesfully!", Success = true });
                            }
                            else if (resetPasswordResponse != null)
                            {
                                LogHelper.Instance.Debug($"user name- {resetPassword.UserName}, password reset failled with reason {resetPasswordResponse.ResponseMessage} (Error code - {resetPasswordResponse.ResultCode})", "Reset Password", "Portal", "Master");
                                ModelState.AddModelError("UserPassword", $"{resetPasswordResponse.ResponseMessage} (Error code - {resetPasswordResponse.ResultCode}), please contact administrator!");
                                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                                var sessionData = (SessionDataModel)Session["DigiDocData"];
                                sessionData.MenuName = "Masters";
                                sessionData.SubMenu = "User Master";
                                Session["DigiDocData"] = sessionData;
                                return Json(new { Result = false, Message = $"Reset password failled!, {resetPasswordResponse.ResponseMessage} (Error code - {resetPasswordResponse.ResultCode})", Success = false });
                            }
                            else
                            {
                                LogHelper.Instance.Debug($"user name- {resetPassword.UserName}, password reset failled", "Reset Password", "Portal", "Master");
                                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                                var sessionData = (SessionDataModel)Session["DigiDocData"];
                                sessionData.MenuName = "Masters";
                                sessionData.SubMenu = "User Master";
                                Session["DigiDocData"] = sessionData;
                                return Json(new { Result = false, Message = "Reset password failled!", Success = false });
                            }
                        }
                        else
                        {
                            LogHelper.Instance.Debug($"user name- {resetPassword.UserName}, password does not match", "Reset Password", "Portal", "Login");

                            return Json(new { Result = false, Message = "Password does not match!", Success = false });
                        }
                    }
                    else
                    {
                        return Json(new { Result = false, Message = "Password Must contain 8 to 16 characters with 1 lower case, 1 upper case, 1 number,1 special character.", Success = false });
                    }

                }
                else
                {
                    return Json(new { Result = false, Message = "Password must contain 8 to 16 characters with 1 lower case, 1 upper case, 1 number,1 special character.", Success = false });
                }

            }
            else
            {

                return Json(new { Result = false, Message = "Failled to reset password, mandatory fields are not supplied", Success = false });
            }
        }
        [HttpPost]
        public ActionResult SetUserActive(int? UserID, string UserName)
        {


            if (UserID != null)
            {
                var spResponse = UtilityHelper.updateUserAccount(new UpdateUserDetailsModel()
                {

                    IsActive = false,
                    UserID = UserID,
                    UserName = UserName
                });
                if (spResponse != null && spResponse.result)
                {
                    LogHelper.Instance.Debug($"user name- {UserName}, user got deleted", " Delete Password", "Portal", "Master");
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    var sessionData = (SessionDataModel)Session["DigiDocData"];
                    sessionData.MenuName = "Masters";
                    sessionData.SubMenu = "User Master";
                    Session["DigiDocData"] = sessionData;
                    AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", "User account details updated", new List<AuditJsonObject>() {
                        new AuditJsonObject()
                        {
                            FieldName = "IsActive",
                            NewValue = true.ToString(),
                            OldValue = false.ToString()
                        }

                        });


                    return RedirectToAction("UserList", "User", new { Message = $" User - {UserName} Successfully Deactivated", Success = true });
                }
                else if (spResponse != null)
                {
                    LogHelper.Instance.Debug($"user name- {UserName}, Delete user failled with reason {spResponse.ResponseMessage} (Error code - {spResponse.ResultCode})", "Delete User", "Portal", "Master");

                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    var sessionData = (SessionDataModel)Session["DigiDocData"];
                    sessionData.MenuName = "Masters";
                    sessionData.SubMenu = "User Master";
                    Session["DigiDocData"] = sessionData;
                    AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", $"Failled to update user ");

                    return RedirectToAction("UserList", "User", new { Message = $"User - {UserName} Failed to Deactivate" });
                }
                else
                {
                    LogHelper.Instance.Debug($"user name- {UserName}, user delete failed", "Delete User", "Portal", "Master");

                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    var sessionData = (SessionDataModel)Session["DigiDocData"];
                    sessionData.MenuName = "Masters";
                    sessionData.SubMenu = "User Master";
                    Session["DigiDocData"] = sessionData;
                    AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", $"Failled to update user ");


                    return RedirectToAction("UserList", "User", new { Message = $"Failed to Update" });
                }
            }
            else
            {
                LogHelper.Instance.Debug($"user name- {UserName}, userid is null", "Delete User", "Portal", "Master");
                return RedirectToAction("UserList", "User", new { Message = $"No User Found" });

            }


        }
        [HttpPost]
        public async Task<ActionResult> Upload()
        {

            //Write your code here

            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[3] { new DataColumn("SL.No",typeof(int)), new DataColumn("Username", typeof(string)),
                            new DataColumn("Error Message", typeof(string)),
                             });


            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0] as HttpPostedFileBase;
                UserFormModel usermodel = new UserFormModel();

                if (file != null && file.ContentLength > 0)
                {
                    if (file.FileName.EndsWith(".csv"))
                    {
                        string extension = System.IO.Path.GetExtension(Request.Files["File"].FileName).ToLower();


                        string path1 = string.Format("{0}/{1}", Server.MapPath("~/uploads"), Request.Files["File"].FileName + DateTime.Now.ToString("ddmmyyyyhhmmss"));
                        if (!Directory.Exists(path1))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/uploads"));
                        }

                        if (System.IO.File.Exists(path1))
                        { System.IO.File.Delete(path1); }
                        Request.Files["File"].SaveAs(path1);
                        if (extension == ".csv")
                        {
                            using (StreamReader sr = new StreamReader(path1))
                            {
                                string[] headers = sr.ReadLine().Split(',');
                                if (headers.Length == 8)
                                {
                                    int i = 0;

                                    while (!sr.EndOfStream)
                                    {
                                        i++;
                                        string[] rows = sr.ReadLine().Split(',');
                                        if (rows.Length == 8)
                                        {
                                            usermodel.IsActive = "true";
                                            usermodel.UserName = rows[0];
                                            usermodel.RealName = rows[1];
                                            usermodel.Email = rows[2];
                                            usermodel.PhoneNumber = rows[3];
                                            usermodel.UserPassword = rows[7];
                                            usermodel.SecuirityQuestion = "Mothers maiden Name";
                                            usermodel.SecurityAnswer = ConfigurationManager.AppSettings["DefaultAnswer"].ToString();

                                            var spResponse = UtilityHelper.getPropertyDetails();
                                            if (spResponse != null && spResponse.result)
                                            {

                                                var propertyMasters = (List<PropertyMasterModel>)spResponse.ResponseData;
                                                if (propertyMasters != null)
                                                {
                                                    var property = propertyMasters.Where(x => x.PropertyName.ToString().ToUpper() == rows[4].ToUpper());
                                                    if (property != null && property.Count() > 0)
                                                    {
                                                        usermodel.PropertyID = propertyMasters.Where(x => x.PropertyName.ToString().ToUpper() == rows[4].ToUpper()).FirstOrDefault().PropertyID;
                                                    }
                                                }
                                            }
                                            spResponse = UtilityHelper.getUserProfileDetails();
                                            if (spResponse != null && spResponse.result)
                                            {

                                                var UserProfile = (List<UserProfileModel>)spResponse.ResponseData;
                                                if (UserProfile != null)
                                                {
                                                    var profileid = UserProfile.Where(x => x.ProfileName.ToString().ToUpper() == rows[5].ToUpper());
                                                    if (profileid != null && profileid.Count() > 0)
                                                    {
                                                        usermodel.UserProfileID = UserProfile.Where(x => x.ProfileName.ToString().ToUpper() == rows[5].ToUpper()).FirstOrDefault().UserProfileID;
                                                    }

                                                }
                                            }
                                            var securityquestion = UtilityHelper.fetchSecuirityQuestionMaster();
                                            if (securityquestion != null)
                                            {
                                                if (securityquestion.Count() > 0)
                                                {
                                                    var securityquestionid = securityquestion.Where(x => x.SecurityQuestion.ToLower() == usermodel.SecuirityQuestion.ToLower());
                                                    if (securityquestionid != null && securityquestionid.Count() > 0)
                                                    {
                                                        usermodel.SecuirityQuestion = securityquestionid.FirstOrDefault().SecurityQuestionID;
                                                    }

                                                }

                                            }

                                            usermodel.UpdatePassword = rows[6].ToLower();
                                            if (usermodel.UserName != null && usermodel.RealName != null && usermodel.UserPassword != null && usermodel.Email != null && usermodel.UserProfileID != null && usermodel.PropertyID != null && usermodel.UpdatePassword != null && usermodel.SecuirityQuestion != null && usermodel.SecurityAnswer != null)
                                            {
                                                var usernameval = new Regex(@"^[a-zA-Z0-9 ]+$");
                                                var remailregex = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
                                                var password = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*]).{8,16}$");
                                                //  var phoneregex = new Regex(@"^(\+\d{1,3}[- ]?)?\d{10,15}$");
                                                var phoneregex = new Regex(@"^\(?([0-9]{2,3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4,9})$");

                                                if ((usermodel.UserName.Length < 32 && usermodel.UserName.Length >= 3))
                                                {
                                                    if (usernameval.IsMatch(usermodel.UserName.ToString()))
                                                    {
                                                        if (password.IsMatch(usermodel.UserPassword))
                                                        {
                                                            if (remailregex.IsMatch(usermodel.Email.ToString()))
                                                            {
                                                                if (string.IsNullOrEmpty(usermodel.PhoneNumber))
                                                                {
                                                                    spResponse = UtilityHelper.createUserAccount(usermodel, true);
                                                                    if (spResponse != null && !spResponse.result)
                                                                    {
                                                                        DataRow dr = dt.NewRow();
                                                                        dr[0] = dt.Rows.Count;
                                                                        dr[1] = rows[0].ToString();
                                                                        dr[2] = $"User creation failled : {spResponse.ResponseMessage} (Error code - {spResponse.ResultCode})";
                                                                        dt.Rows.Add(dr);
                                                                    }
                                                                    else
                                                                    {
                                                                        usermodel.UserID = Convert.ToInt32(spResponse.ResponseData);
                                                                        spResponse = UtilityHelper.UserSecurityQuestion(usermodel);

                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (!string.IsNullOrEmpty(usermodel.PhoneNumber) && phoneregex.IsMatch(usermodel.PhoneNumber))
                                                                    {

                                                                        spResponse = UtilityHelper.createUserAccount(usermodel, true);
                                                                        if (spResponse != null && !spResponse.result)
                                                                        {
                                                                            DataRow dr = dt.NewRow();
                                                                            dr[0] = dt.Rows.Count;
                                                                            dr[1] = rows[0].ToString();
                                                                            dr[2] = $"User creation failled : {spResponse.ResponseMessage} (Error code - {spResponse.ResultCode})";
                                                                            dt.Rows.Add(dr);
                                                                        }
                                                                        else
                                                                        {
                                                                            usermodel.UserID = Convert.ToInt32(spResponse.ResponseData);
                                                                            spResponse = UtilityHelper.UserSecurityQuestion(usermodel);

                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        DataRow dr = dt.NewRow();
                                                                        dr[0] = dt.Rows.Count;
                                                                        dr[1] = rows[0].ToString();
                                                                        dr[2] = "Phone Numver is not valid";
                                                                        dt.Rows.Add(dr);

                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                DataRow dr = dt.NewRow();
                                                                dr[0] = dt.Rows.Count;
                                                                dr[1] = rows[0].ToString();
                                                                dr[2] = "Email is not valid";
                                                                dt.Rows.Add(dr);

                                                            }
                                                        }
                                                        else
                                                        {
                                                            DataRow dr = dt.NewRow();
                                                            dr[0] = dt.Rows.Count;
                                                            dr[1] = rows[0].ToString();
                                                            dr[2] = "Password must contain 1 lower case 1 upper case 1 number 1 special character with 8 to 16 characters";
                                                            dt.Rows.Add(dr);

                                                        }
                                                    }
                                                    else
                                                    {
                                                        DataRow dr = dt.NewRow();
                                                        dr[0] = dt.Rows.Count;
                                                        dr[1] = rows[0].ToString();
                                                        dr[2] = "Username accepts only alphabets";
                                                        dt.Rows.Add(dr);

                                                    }

                                                }
                                                else
                                                {
                                                    DataRow dr = dt.NewRow();
                                                    dr[0] = dt.Rows.Count;
                                                    dr[1] = rows[0].ToString();
                                                    dr[2] = "UserName shoulbe greater than 2 and less than 32";
                                                    dt.Rows.Add(dr);


                                                }
                                            }


                                            else
                                            {
                                                DataRow dr = dt.NewRow();
                                                dr[0] = dt.Rows.Count;
                                                dr[1] = rows[0].ToString();
                                                dr[2] = "UserName,RealName,UserPassword,Email,UserProfileID,PropertyID columns should not be null";
                                                dt.Rows.Add(dr);

                                            }



                                        }

                                        else
                                        {
                                            DataRow dr = dt.NewRow();
                                            dr[0] = dt.Rows.Count;
                                            dr[1] = rows[0].ToString();
                                            dr[2] = "No  of column should be 7";
                                            dt.Rows.Add(dr);

                                        }

                                    }

                                    if (dt.Rows.Count == 0)
                                    {

                                        return Json(new { Result = true, Message = "Users Saved Successfully", Success = true });
                                    }
                                    else
                                    {

                                        //byte[] filecontent = new DatatabletoPdf().exportpdf(dt);
                                        //string filename = "Sample_PDF_" + DateTime.Now.ToString("MMddyyyyhhmmss") + ".pdf";
                                        //return File(filecontent, "application/pdf", filename);


                                        return Json(new { Result = true, Message = $"Users Success count-{i - dt.Rows.Count}=Fail count- {dt.Rows.Count}", Success = true });

                                    }
                                }
                                else
                                {

                                    return Json(new { Result = false, Message = "No of Column should be 8", Success = false });

                                }
                            }


                        }
                        else
                        {
                            return Json(new { Result = false, Message = "Upload a valid .csv file", Success = false });

                        }


                    }
                    else
                    {
                        return Json(new { Result = false, Message = "Upload a valid .csv file", Success = false });

                    }

                }
                else
                {
                    return Json(new { Result = false, Message = "Upload a valid .csv file", Success = false });



                }


            }
            else
            {

                return Json(new { Result = false, Message = "Upload a valid .csv file", Success = false });

            }


        }

        public FileResult DownloadFile()
        {
            if (System.IO.File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/Assets/Samples/") + "UserUploadSample.csv"))
            {
                var sDocument = System.Web.Hosting.HostingEnvironment.MapPath("~/Assets/Samples/") + "UserUploadSample.csv";
                byte[] fileBytes = System.IO.File.ReadAllBytes(sDocument);


                //Send the File to Download.
                string fileName = "SampleUploadFile.csv";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            return null;
        }

        [HttpPost]
        public async Task<ActionResult> NewUserEmailSend(UserEmailSend user)
        {
            string BaseURL = ConfigurationManager.AppSettings["EmailURL"].ToString();
            string FromEmail = ConfigurationManager.AppSettings["FromEmail"].ToString();
            string DisplayFromEmail = ConfigurationManager.AppSettings["DisplayFromEmail"].ToString();
            string NewUserEmailSubject = ConfigurationManager.AppSettings["NewUserEmailSubject"].ToString();
            var sessionData = (SessionDataModel)Session["DigiDocData"];
            sessionData.MenuName = "Masters";
            sessionData.SubMenu = "User Master";
            Session["DigiDocData"] = sessionData;
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            EmailResponse emailResponse = await new WSClientHelper().SendEmail(new EmailRequest()

            {
                FromEmail = FromEmail,
                displayFromEmail = DisplayFromEmail,
                ToEmail = user.Email,
                Subject = NewUserEmailSubject,

                UserName = user.UserName,
                RealName = user.RealName,
                Password=user.UserPassword,
                EmailType = EmailType.SendPassword,


            }, "NewUserEmailSend", new ServiceParameters { EmailAPIProxyHost = ConfigurationModel.EmailAPIProxyHost, EmailAPIProxyPswd = ConfigurationModel.EmailAPIProxyPswd, isProxyEnableForEmailAPI = ConfigurationModel.isProxyEnableForEmailAPI, EmailURL = ConfigurationModel.EmailURL });
            if (emailResponse != null && emailResponse.result)
            {
                AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", "Email regarding password send to user" + user.RealName + " to his email" + user.Email);
                return Json(new { Result = true, Message = "Success" });
            }
            else
            {
                AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", "Email regarding password Failed send to user" + user.RealName + " to his email" + user.Email);
                return Json(new { Result = false, Message = "Failed" });
            }
        }
    }
}