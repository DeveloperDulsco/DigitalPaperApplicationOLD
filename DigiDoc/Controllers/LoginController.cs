using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigiDoc.Models;
using DigiDoc.DataAccess;
using DigiDoc.DataAccess.Models;
using DigiDoc.Helper;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DigiDoc.Controllers
{
    public class LoginController : BaseController
    {
        // GET: Login
        public ActionResult Index()
        {
           

            ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
            return View();
        }

        [HttpPost]
       
        //[RequestSizeLimit(100_000_000)]
        public ActionResult ValidateLogin(LoginRequestModel loginRequest)
        {
            if (ModelState.IsValid)
            {
               
                try
                {
                    if(string.IsNullOrEmpty(loginRequest.UserPassword))
                    {
                        LogHelper.Instance.Debug($"Password is not entered", "Validate Login", "Portal", "Login");
                        ModelState.AddModelError("UserName", $"Password can not be blank");
                        return View("Index");
                    }
                    var loginStatus = new DapperHelper().ExecuteSP<LoginUserDetailsModel>("usp_ValidateLogin", ConfigurationModel.ConnectionString, new { UserName = loginRequest.UserName, Password = loginRequest.UserPassword }).ToList();
                    if (loginStatus != null && loginStatus.Count > 0 && !string.IsNullOrEmpty(loginStatus.First().Result) && loginStatus.First().Result.Equals("200"))
                    {
                        if(loginStatus.First().UpdatepasswordFlag != null && loginStatus.First().UpdatepasswordFlag.Value)
                        {
                            LogHelper.Instance.Debug($"Update password flag is enabled routing to reset password page", "Validate Login", "Portal", "Login");
                            ViewBag.ResetPassword = "true";
                            loginRequest.UserID = loginStatus.First().UserID.ToString();
                            ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                            return View("Index", loginRequest);
                        }
                        var timezoneid = "Singapore Standard Time";
                        var sppropertresponse = UtilityHelper.getPropertyDetails(loginStatus.First().PropertyID.ToString());
                        if (sppropertresponse != null)
                        {
                            var propertlist = (List<PropertyMasterModel>)sppropertresponse.ResponseData;
                            timezoneid = propertlist.FirstOrDefault().TimeZone;

                        }
                        Session["DigiDocData"] = new SessionDataModel()
                        {
                            ProfileID = loginStatus.First().UserProfileID.ToString(),
                            ProfileName = loginStatus.First().ProfileName,
                            UserID = loginStatus.First().UserID,
                            UserName = loginStatus.First().UserName,
                            RealName = loginStatus.First().RealName,
                            PropertyID = loginStatus.First().PropertyID.ToString(),
                            TimeZoneId = timezoneid,
                            Email=loginStatus.First().Email
                        };
                        AuditHelper.InsertAuditLog("Login", loginRequest.UserName, "Succesfully logged-in");
                        return RedirectToAction("Index", "Home");
                    }
                    //User Disabled
                    else if (loginStatus != null && loginStatus.Count > 0 && !string.IsNullOrEmpty(loginStatus.First().Message) && !string.IsNullOrEmpty(loginStatus.First().Result)
                        && loginStatus.First().Result.Equals("204"))
                    {
                        LogHelper.Instance.Debug($"{loginStatus.First().Message} - (Error code : {loginStatus.First().Result} with user name- {loginRequest.UserName})", "Validate Login", "Portal", "Login");
                        ModelState.AddModelError("UserName", $"{loginStatus.First().Message} - (Error code : {loginStatus.First().Result} ). Please contact administrator");
                    }
                    //Invalid User name
                    else if(loginStatus != null && loginStatus.Count > 0 && !string.IsNullOrEmpty(loginStatus.First().Message) && !string.IsNullOrEmpty(loginStatus.First().Result)
                        && loginStatus.First().Result.Equals("203"))
                    {
                        LogHelper.Instance.Debug($"{loginStatus.First().Message} - (Error code : {loginStatus.First().Result} with user name- {loginRequest.UserName})", "Validate Login", "Portal", "Login");
                        ModelState.AddModelError("UserName", $"{loginStatus.First().Message} - (Error code : {loginStatus.First().Result} )");
                    }
                    //Invalid password
                    else if (loginStatus != null && loginStatus.Count > 0 && !string.IsNullOrEmpty(loginStatus.First().Message) && !string.IsNullOrEmpty(loginStatus.First().Result)
                        && loginStatus.First().Result.Equals("205"))
                    {
                        var BalanceLoginAttemptResponse = UtilityHelper.VerifyBalanceLoginAttemptCount(loginStatus.First().TryCount);
                        if (BalanceLoginAttemptResponse != null 
                            && BalanceLoginAttemptResponse.result
                            && BalanceLoginAttemptResponse.ResponseData != null
                            && (Int32)BalanceLoginAttemptResponse.ResponseData >= 0)
                        {
                            if ((Int32)BalanceLoginAttemptResponse.ResponseData == 0)
                            {
                                LogHelper.Instance.Debug($"{loginStatus.First().Message} - (Error code : {loginStatus.First().Result} with user name- {loginRequest.UserName})", "Validate Login", "Portal", "Login");
                                ModelState.AddModelError("UserName", $"{loginStatus.First().Message} - (Error code : {loginStatus.First().Result}, one more wrong try will block the account)");
                            }
                            else
                            {
                                LogHelper.Instance.Debug($"{loginStatus.First().Message} - (Error code : {loginStatus.First().Result} with user name- {loginRequest.UserName})", "Validate Login", "Portal", "Login");
                                ModelState.AddModelError("UserName", $"{loginStatus.First().Message} - (Error code : {loginStatus.First().Result} )");
                            }
                        }
                        //Disable User
                        else if (BalanceLoginAttemptResponse != null
                            && BalanceLoginAttemptResponse.result
                            && BalanceLoginAttemptResponse.ResponseData != null
                            && (Int32)BalanceLoginAttemptResponse.ResponseData < 0)
                        {
                            LogHelper.Instance.Debug($"{loginStatus.First().Message} - (Error code : {loginStatus.First().Result} with user name- {loginRequest.UserName})", "Validate Login", "Portal", "Login");

                            var updateUserAccountResponse = UtilityHelper.updateUserAccount(new UpdateUserDetailsModel()
                            {
                                IsActive = false,
                                UserName = loginRequest.UserName
                            });
                            if (updateUserAccountResponse != null && updateUserAccountResponse.result)
                            {
                                AuditHelper.InsertAuditLog("Login", loginRequest.UserName, "User account got locked due to more than allowed un successfull try");
                                LogHelper.Instance.Debug($"Account disabled", "Validate Login", "Portal", "Login");
                                ModelState.AddModelError("UserName", $"Account Got Disabled (Please contact administrator)!");
                            }
                            else if(updateUserAccountResponse != null)
                            {
                                LogHelper.Instance.Debug($"{updateUserAccountResponse.ResponseMessage} error code : {updateUserAccountResponse.ResultCode}", "Validate Login", "Portal", "Login");
                                ModelState.AddModelError("UserName", $"{updateUserAccountResponse.ResponseMessage} error code : {updateUserAccountResponse.ResultCode}");
                            }
                            else
                            {
                                LogHelper.Instance.Debug($"Failled to disable user account", "Validate Login", "Portal", "Login");
                                ModelState.AddModelError("UserName", $"{loginStatus.First().Message} - (Error code : {loginStatus.First().Result} )");
                            }
                        }
                        else
                        {
                            LogHelper.Instance.Debug($"{loginStatus.First().Message} - (Error code : {loginStatus.First().Result} with user name- {loginRequest.UserName})", "Validate Login", "Portal", "Login");
                            ModelState.AddModelError("UserName", $"{loginStatus.First().Message} - (Error code : {loginStatus.First().Result} )");
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug($"Unsuccefull login atempt, invalid username or password with user name- {loginRequest.UserName})", "Validate Login", "Portal", "Login");
                        ModelState.AddModelError("UserName", $"Invalid Username or Password");
                    }
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("UserName", "Generic error (Error code: -1)");
                    LogHelper.Instance.Error(ex, "Validate Login", "Portal", "Login");
                }
                
                
            }
            else
            {
                //if (ModelState.Count > 0)
                {
                    //foreach (var Mstate in ModelState.Values)
                    {
                        //if()
                        LogHelper.Instance.Debug("Failled to login with validation error in fields", "Validate Login", "Portal", "Login");
                        //ModelState.AddModelError("UserName", "Please enter a valid Username or Password");
                    }
                }
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult ValidateLoginCredentials(LoginRequestModel loginRequest)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    if (!string.IsNullOrEmpty(loginRequest.UserPassword))
                    {
                        var loginStatus = new DapperHelper().ExecuteSP<LoginUserDetailsModel>("usp_ValidateLogin", ConfigurationModel.ConnectionString, new { UserName = loginRequest.UserName, Password = loginRequest.UserPassword }).ToList();
                        if (loginStatus != null && loginStatus.Count > 0 && !string.IsNullOrEmpty(loginStatus.First().Result) && loginStatus.First().Result.Equals("200"))
                        {
                            LogHelper.Instance.Debug($"Forgot user got authenticated with user name- {loginRequest.UserName}", "Validate Login Credentials", "Portal", "Login");
                            ViewBag.ResetPassword = "true";
                            ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                            loginRequest.UserID = loginStatus.First().UserID;
                            return View("Index", loginRequest);
                        }
                        else
                        {
                            LogHelper.Instance.Debug($"invalid username or password with user name- {loginRequest.UserName} can not reset password", "Validate Login Credentials", "Portal", "Login");
                            ViewBag.ShowForgotPasswordWindow = "true";
                            ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                            ModelState.AddModelError("UserPassword", $"Invalid Username or Password");
                            return View("Index");
                        }
                    }
                    else if(!string.IsNullOrEmpty(loginRequest.SecuirityQuestion) && !string.IsNullOrEmpty(loginRequest.SecurityAnswer))
                    {
                        int secuirityQuestionID;
                        if(Int32.TryParse(loginRequest.SecuirityQuestion,out secuirityQuestionID))
                        {
                            var validateUser = UtilityHelper.ValidateUserWithSecurityQuestion(loginRequest);
                            if(validateUser != null && validateUser.result && validateUser.ResponseData != null)
                            {
                                LogHelper.Instance.Debug($"Forgot user got authenticated with user name- {loginRequest.UserName}", "Validate Login Credentials", "Portal", "Login");
                                ViewBag.ResetPassword = "true";
                                loginRequest.UserID = ((LoginUserDetailsModel)validateUser.ResponseData).UserID.ToString();
                                ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                                return View("Index", loginRequest);
                            }
                            else if(validateUser != null)
                            {
                                LogHelper.Instance.Debug($"User can not be authenticated with reason {validateUser.ResponseMessage} and error code {validateUser.ResultCode}", "Validate Login Credentials", "Portal", "Login");
                                ViewBag.ShowForgotPasswordWindow = "true";
                                ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                                ModelState.AddModelError("UserPassword", $"Please enter a valid old password or security answer to authenticate");
                                return View("Index");
                            }
                            else
                            {
                                LogHelper.Instance.Debug($"User can not be authenticated", "Validate Login Credentials", "Portal", "Login");
                                ViewBag.ShowForgotPasswordWindow = "true";
                                ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                                ModelState.AddModelError("UserPassword", $"Please enter a valid old password or security answer to authenticate");
                                return View("Index");
                            }
                        }
                        else
                        {
                            LogHelper.Instance.Debug($"invalid secuirty question selected with user name- {loginRequest.UserName} can not reset password", "Validate Login Credentials", "Portal", "Login");
                            ViewBag.ShowForgotPasswordWindow = "true";
                            ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                            ModelState.AddModelError("UserPassword", $"Please enter a valid security question and answer to authenticate");
                            return View("Index");
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug($"invalid username or password with user name- {loginRequest.UserName} can not reset password", "Validate Login Credentials", "Portal", "Login");
                        ViewBag.ShowForgotPasswordWindow = "true";
                        ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                        ModelState.AddModelError("UserPassword", $"Please enter a valid old password or security answer to authenticate");
                        return View("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("UserPassword", "Generic error (Error code: -1)");
                    LogHelper.Instance.Error(ex, "Validate Login Credentials", "Portal", "Login");
                    ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                    ViewBag.ShowForgotPasswordWindow = "true";
                    return View("Index");
                }
            }
            else
            {
                LogHelper.Instance.Debug("Failled to authenticate user because username is not provided", "Validate Login Credentials", "Portal", "Login");
                ModelState.AddModelError("UserPassword", "Please enter a valid Username and old password or security answer");
                ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                ViewBag.ShowForgotPasswordWindow = "true";
                return View("Index");
               // return RedirectToAction("Index", "Login");
            }

            
        }

     
        [HttpPost]
      
        public async Task<ActionResult> ResetPassword(ResetPasswordModel resetPassword)
        {
            if (ModelState.IsValid)
            {
                LogHelper.Instance.Debug($"To reset password", "resetPassword", "Portal", "resetPassword");
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
                                LogHelper.Instance.Debug($"user name- {resetPassword.UserName}, password got resetted", "Reset Password", "Portal", "Login");
                                AuditHelper.InsertAuditLog("Login", resetPassword.UserName, "Succesfully reseted the password");
                                ViewBag.ResetPassword = "false";
                                ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                                return Json(new { Result = true, Message = "Password changed succesfully!", Success = true });
                            }
                            else if (resetPasswordResponse != null)
                            {
                                LogHelper.Instance.Debug($"user name- {resetPassword.UserName}, password reset failled with reason {resetPasswordResponse.ResponseMessage} (Error code - {resetPasswordResponse.ResultCode})", "Reset Password", "Portal", "Login");
                                ModelState.AddModelError("UserPassword", $"{resetPasswordResponse.ResponseMessage} (Error code - {resetPasswordResponse.ResultCode}), please contact administrator!");
                                ViewBag.ResetPassword = "true";
                                ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                                //  return View("Index", new LoginRequestModel() { UserID = resetPassword.UserID });
                                return Json(new { Result = false, Message = $"{resetPasswordResponse.ResponseMessage} (Error code - {resetPasswordResponse.ResultCode}), please contact administrator!" });

                            }
                            else
                            {
                                LogHelper.Instance.Debug($"user name- {resetPassword.UserName}, password reset failled", "Reset Password", "Portal", "Login");
                                ModelState.AddModelError("UserPassword", "Failed to update the password, please contact administrator!");
                                ViewBag.ResetPassword = "true";
                                ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                                return Json(new { Result = false, Message = "Failed to update the password, please contact administrator!" });
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("UserPassword", "Password does not match!");
                            ViewBag.ResetPassword = "true";
                            ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                            //  return View("Index", new LoginRequestModel() { UserID = resetPassword.UserID });
                            return Json(new { Result = false, Message = "Password does not match!" });
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
                ModelState.AddModelError("UserPassword", "Please enter valid old password or security answer to authenticate");
                ViewBag.ResetPassword = "true";
                ViewBag.SecurityQuestionList = UtilityHelper.fetchSecuirityQuestionMaster();
                //  return View("Index", new LoginRequestModel() { UserID = resetPassword.UserID });
                return Json(new { Result = false, Message = "Please enter valid old password or security answer to authenticate" });
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Index");
        }

    }
}