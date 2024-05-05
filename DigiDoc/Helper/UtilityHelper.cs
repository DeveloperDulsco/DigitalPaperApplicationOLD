using DigiDoc.DataAccess;
using DigiDoc.DataAccess.Models;
using DigiDoc.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigiDoc.Helper
{
    public class UtilityHelper
    {
        public static UtilityResponseModel VerifyBalanceLoginAttemptCount(int Attemptedcount)
        {
            if (Attemptedcount > 0)
            {
                if (ConfigurationModel.Settings != null && ConfigurationModel.Settings.Count > 0)
                {
                    var settings = ConfigurationModel.Settings.Find(x => x.SettingName.Equals("LoginAttemptThresholdValue"));
                    if (settings != null && !string.IsNullOrEmpty(settings.SettingValue))
                    {
                        int tryCountThreshold;
                        if (Int32.TryParse(settings.SettingValue, out tryCountThreshold))
                        {
                            return new UtilityResponseModel()
                            {
                                result = true,
                                ResponseData = tryCountThreshold - Attemptedcount,
                                ResultCode = "200",
                                ResponseMessage = "Success"
                            };
                        }
                        else
                        {
                            LogHelper.Instance.Debug($"Settings value for LoginAttemptThresholdValue is not a valid integer value", "Verify Login Attempt Count", "Portal", "Login");
                            return new UtilityResponseModel()
                            {
                                result = false,
                                ResultCode = "-3",
                                ResponseMessage = "Settings value for LoginAttemptThresholdValue is not a valid integer value"
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug($"Settings value for LoginAttemptThresholdValue is not configured", "Verify Login Attempt Count", "Portal", "Login");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResultCode = "-3",
                            ResponseMessage = "Settings value for LoginAttemptThresholdValue is not configured"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug($"Settings values are empty", "Verify Login Attempt Count", "Portal", "Login");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResultCode = "-3",
                        ResponseMessage = "Settings values are empty"
                    };
                }
            }
            else
            {
                LogHelper.Instance.Debug($"Invalid attempted count passed it should be greater that 0", "Verify Login Attempt Count", "Portal", "Login");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResultCode = "-3",
                    ResponseMessage = "Invalid attempted count passed it should be greater that 0"
                };
            }
        }

        public static UtilityResponseModel VerifyDocumentApprovalThreshold(int ApprovalRecordCount)
        {
            if (ConfigurationModel.Settings != null && ConfigurationModel.Settings.Count > 0)
            {
                var settings = ConfigurationModel.Settings.Find(x => x.SettingName.Equals("DocumentApprovalThreshold"));
                LogHelper.Instance.Debug($"Settings value for ThresholdValue is not a valid integer value"+settings.SettingValue+""+ApprovalRecordCount, "maximum approval count", "Portal", "Document");

                int ApprovalThreshold;
                if (settings != null && !string.IsNullOrEmpty(settings.SettingValue) && Int32.TryParse(settings.SettingValue, out ApprovalThreshold))
                {
                    if (ApprovalRecordCount >= ApprovalThreshold)
                    {
                        return new UtilityResponseModel()
                        {
                            result = true,
                            ResponseData = true
                        };
                    }
                    else
                    {
                        return new UtilityResponseModel()
                        {
                            result = true,
                            ResponseData = false
                        };
                    }
                }
                else
                {
                    return new UtilityResponseModel()
                    {
                        result = true,
                        ResponseData = false
                    };
                }
            }
            else
            {
                return new UtilityResponseModel()
                {
                    result = true,
                    ResponseData = false
                };
            }
        }

        public static UtilityResponseModel updateUserAccount(UpdateUserDetailsModel updateUser)
        {
            try
            {
                var spResponse = updateUser.UserID != null ?
                                    new DapperHelper().ExecuteSP<SPResponseModel>("Usp_UpdateUserDetails", ConfigurationModel.ConnectionString,
                                        new { UserID = updateUser.UserID.Value, TryCount = updateUser.TryCount, IsActive = updateUser.IsActive, UpdatepasswordFlag = updateUser.UpdatepasswordFlag,
                                        
                                            update= updateUser.update,
                                            RealName = updateUser.RealName,
                                           
                                            Email = updateUser.Email,
                                            PhoneNumber = updateUser.PhoneNumber,
                                            UserName = updateUser.UserName,
                                            UserProfileID = updateUser.UserProfileID,
                                        })
                                    : new DapperHelper().ExecuteSP<SPResponseModel>("Usp_UpdateUserDetails", ConfigurationModel.ConnectionString,
                                        new { UserName = updateUser.UserName, TryCount = updateUser.TryCount, IsActive = updateUser.IsActive, UpdatepasswordFlag = updateUser.UpdatepasswordFlag,
                                        

                                            update = updateUser.update,
                                            RealName = updateUser.RealName,

                                            Email = updateUser.Email,
                                            PhoneNumber = updateUser.PhoneNumber,
                                           
                                            UserProfileID = updateUser.UserProfileID,
                                        });
                if (spResponse != null)
                {
                    if (spResponse.First().Result.Equals("200"))
                    {
                        return new UtilityResponseModel()
                        {
                            result = true,
                            ResultCode = "200",
                            ResponseMessage = spResponse.First().Message
                        };
                    }
                    else
                    {
                        LogHelper.Instance.Debug($"SP Usp_UpdateUserDetails returned Error Code : {spResponse.First().Result} and Message : {spResponse.First().Message}", "Disable User Account", "Portal", "Login");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResultCode = spResponse.First().Result,
                            ResponseMessage = spResponse.First().Message
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP Usp_UpdateUserDetails returned null", "Reset Password By UserID", "Portal", "Login");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResultCode = "-2",
                        ResponseMessage = "DB Error"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Disable User Account", "Portal", "Login");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = "Generic exception"
                };
            }
        }

        public static UtilityResponseModel updateDocumentTypeMaster(DocumentTypeMaster documentType)
        {
            try
            {
                var spResponse = documentType.DocumentID != null ?
                                    new DapperHelper().ExecuteSP<SPResponseModel>("Usp_InsertDocumentMaster", ConfigurationModel.ConnectionString,
                                        new { DocumentCode = documentType.DocumentCode, DocumentName = documentType.DocumentName, DocumentID = Int32.Parse(documentType.DocumentID),IsActive=documentType.IsActive})
                                    : new DapperHelper().ExecuteSP<SPResponseModel>("Usp_InsertDocumentMaster", ConfigurationModel.ConnectionString,
                                        new { DocumentCode = documentType.DocumentCode, DocumentName = documentType.DocumentName,IsActive=true });
                if (spResponse != null)
                {
                    if (spResponse.First().Result.Equals("200"))
                    {
                        return new UtilityResponseModel()
                        {
                            result = true,
                            ResultCode = "200",
                            ResponseMessage = spResponse.First().Message
                        };
                    }
                    if (spResponse.First().Result.Equals("201"))
                    {
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResultCode = "201",
                            ResponseMessage = spResponse.First().Message
                        };
                    }
                    else if (spResponse.First().Result.Equals("214"))
                    {
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResultCode = "214",
                            ResponseMessage = spResponse.First().Message
                        };
                    }
                    else
                    {
                        LogHelper.Instance.Debug($"SP Usp_InsertDocumentMaster returned Error Code : {spResponse.First().Result} and Message : {spResponse.First().Message}", "Update Document Type Master", "Portal", "Login");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResultCode = spResponse.First().Result,
                            ResponseMessage = spResponse.First().Message
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP Usp_InsertDocumentMaster returned null", "Update Document Type Master", "Portal", "Login");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResultCode = "-2",
                        ResponseMessage = "DB Error"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Update Document Type Master", "Portal", "Login");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = "Generic exception"
                };
            }
        }

        public static UtilityResponseModel createUserAccount(UserFormModel userModel,bool val=true)
        {
            try
            {
                int PropertyID;
                int UserProfileID;
                if (!string.IsNullOrEmpty(userModel.PropertyID) && Int32.TryParse(userModel.PropertyID, out PropertyID))
                {
                    if (!string.IsNullOrEmpty(userModel.UserProfileID) && Int32.TryParse(userModel.UserProfileID, out UserProfileID))
                    {
                        var spResponse = new DapperHelper().ExecuteSP<SPUserResponseModel>("Usp_InsertUserDetails", ConfigurationModel.ConnectionString,
                                            new
                                            {
                                                UserName = userModel.UserName,
                                                RealName = userModel.RealName,
                                                UserPassword = userModel.UserPassword,
                                                Email = userModel.Email,
                                                PhoneNumber = userModel.PhoneNumber,
                                                PropertyID = PropertyID,
                                                UserProfileID = UserProfileID,
                                                IsActive = bool.Parse(userModel.IsActive),
                                                UpdatepasswordFlag = bool.Parse(userModel.UpdatePassword),
                                                Insert=val

                                            });
                        if (spResponse != null)
                        {
                            if (spResponse.First().Result.Equals("200"))
                            {
                                return new UtilityResponseModel()
                                {
                                    result = true,
                                    ResultCode = "200",
                                    ResponseMessage = spResponse.First().Message,
                                    ResponseData=spResponse.First().UserId

                                };
                            }
                            else
                            {
                                LogHelper.Instance.Log($"SP Usp_InsertUserDetails returned Error Code : {spResponse.First().Result} and Message : {spResponse.First().Message}", "Create User Account", "Portal", "Login");
                                return new UtilityResponseModel()
                                {
                                    result = false,
                                    ResultCode = spResponse.First().Result,
                                    ResponseMessage = spResponse.First().Message
                                };
                            }
                        }
                        else
                        {
                            LogHelper.Instance.Log("SP Usp_InsertUserDetails returned null", "Create User Account", "Portal", "Login");
                            return new UtilityResponseModel()
                            {
                                result = false,
                                ResultCode = "-2",
                                ResponseMessage = "DB Error"
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Log("Profile ID can not be blank", "Create User Account", "Portal", "Login");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResultCode = "-2",
                            ResponseMessage = "DB Error"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Log("Property ID can not be blank", "Create User Account", "Portal", "Login");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResultCode = "-2",
                        ResponseMessage = "DB Error"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Create User Account", "Portal", "Login");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = "Generic exception"
                };
            }
        }

        public static UtilityResponseModel resetPasswordByUserID(ResetPasswordModel resetPassword)
        {
            try
            {
                var spResponse = new DapperHelper().ExecuteSP<SPResponseModel>("usp_ChangeUserPassword", ConfigurationModel.ConnectionString, new { UserID = Int32.Parse(resetPassword.UserID),
                    Newpassword = resetPassword.NewPassword,
                    SecurityQuestionID = Int32.Parse(resetPassword.SecuirityQuestion),
                    SecurityAnswer = resetPassword.SecurityAnswer }).ToList();
                if (spResponse != null && spResponse.Count > 0)
                {
                    if (spResponse.First().Result.Equals("200"))
                    {
                        if (updateUserAccount(new UpdateUserDetailsModel()
                        {
                            IsActive = true,
                            UpdatepasswordFlag = false,
                            TryCount = 0,
                            UserID = Int32.Parse(resetPassword.UserID)
                        }).result)
                        {
                            return new UtilityResponseModel()
                            {
                                result = true,
                                ResponseMessage = spResponse.First().Message,
                                ResultCode = spResponse.First().Result
                            };
                        }
                        else
                        {
                            LogHelper.Instance.Debug($"Failled to update the user details", "Reset Password By UserID", "Portal", "Login");
                            return new UtilityResponseModel()
                            {
                                result = true,
                                ResponseMessage = spResponse.First().Message,
                                ResultCode = spResponse.First().Result
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug($"SP usp_ChangeUserPassword returned Error Code : {spResponse.First().Result} and Message : {spResponse.First().Message}", "Reset Password By UserID", "Portal", "Login");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = spResponse.First().Message,
                            ResultCode = spResponse.First().Result
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP usp_ChangeUserPassword returned null", "Reset Password By UserID", "Portal", "Login");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Reset Password By UserID", "Portal", "Login");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
        }

        public static List<SecurityQuestionModel> fetchSecuirityQuestionMaster()
        {
            try
            {
                return new DapperHelper().ExecuteSP<SecurityQuestionModel>("usp_GetSecurityQuestionList", ConfigurationModel.ConnectionString).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Fetch Secuirity Question Master", "Portal", "Login");
                return null;
            }
        }

        



        public static UtilityResponseModel ValidateUserWithSecurityQuestion(LoginRequestModel loginRequest)
        {
            try
            {
                var ValidateUser = new DapperHelper().ExecuteSP<LoginUserDetailsModel>("Usp_ValidateUserSecurityQuestions", ConfigurationModel.ConnectionString,
                                        new { UserName = loginRequest.UserName, SecurityQuestionID = loginRequest.SecuirityQuestion, SecurityAnswer = loginRequest.SecurityAnswer }).ToList();
                if (ValidateUser != null && ValidateUser.Count > 0)
                {
                    if (ValidateUser.First().Result.Equals("200"))
                    {
                        return new UtilityResponseModel()
                        {
                            result = true,
                            ResponseData = ValidateUser.First(),
                            ResultCode = "200"
                        };
                    }
                    else
                    {
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = ValidateUser.First().Message,
                            ResultCode = ValidateUser.First().Result
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP Usp_ValidateUserSecurityQuestions returned null", "Validate User With Security Question", "Portal", "Login");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Validate User With Security Question", "Portal", "Login");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
        }

        public static UtilityResponseModel getDocumentGroupList()
        {
            try
            {
                var spResponse = new DapperHelper().ExecuteSP<Models.DocumentTypeMaster>("Usp_GetDocumentMaster", ConfigurationModel.ConnectionString).ToList();
                if (spResponse != null && spResponse.Count > 0)
                {
                    if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                    {
                        return new UtilityResponseModel()
                        {
                            ResponseData = spResponse,
                            result = true,
                            ResultCode = "200"
                        };
                    }
                    else
                    {
                        return new UtilityResponseModel()
                        {
                            ResponseMessage = spResponse.First().Message,
                            result = false,
                            ResultCode = spResponse.First().Result
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP Usp_GetDocumentMaster returned null", "Get Document Group List", "Portal", "Document");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Get Document Group List", "Portal", "Document");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
        }

        public static UtilityResponseModel getDocumentDetails(int documentDetailID,string UserID)
        {
            try
            {
                if (string.IsNullOrEmpty(UserID))
                {
                    var spResponse = new DapperHelper().ExecuteSP<DocumentModel>("Usp_GetDocumentDetails", ConfigurationModel.ConnectionString, new { DocumentDetailID = documentDetailID }).ToList();
                    if (spResponse != null && spResponse.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseData = spResponse,
                                result = true,
                                ResultCode = "200"
                            };
                        }
                        else
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseMessage = spResponse.First().Message,
                                result = false,
                                ResultCode = spResponse.First().Result
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug("SP Usp_GetDocumentDetails returned null", "Get Document Details", "Portal", "Document");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = "DB Error",
                            ResultCode = "-2"
                        };
                    }
                }
               else
                {
                    var spResponse = new DapperHelper().ExecuteSP<DocumentModel>("Usp_GetDocumentDetails", ConfigurationModel.ConnectionString, new { DocumentDetailID = documentDetailID, userID = Int32.Parse(UserID) }).ToList();
                    if (spResponse != null && spResponse.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseData = spResponse,
                                result = true,
                                ResultCode = "200"
                            };
                        }
                        else
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseMessage = spResponse.First().Message,
                                result = false,
                                ResultCode = spResponse.First().Result
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug("SP Usp_GetDocumentDetails returned null", "Get Document Details", "Portal", "Document");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = "DB Error",
                            ResultCode = "-2"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Get Document Details", "Portal", "Document");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
        }

        public static UtilityResponseModel insertComments(int DocumentHeaderID, int DocumentDetailID, string Comments, string UserID, int? ApprovalID)
        {
            try
            {
                if (!string.IsNullOrEmpty(Comments))
                {
                    var spResponse = new DapperHelper().ExecuteSP<SPResponseModel>("Usp_InsertCommentsDetails", ConfigurationModel.ConnectionString, new
                    {
                        DocumentHeaderID = DocumentHeaderID,
                        DocumentDetailID = DocumentDetailID,
                        Comments = Comments,
                        UserID = UserID,
                        ApprovalID = ApprovalID
                    }).ToList();
                    if (spResponse != null && spResponse.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                        {
                            return new UtilityResponseModel()
                            {

                                result = true,
                                ResultCode = "200"
                            };
                        }
                        else
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseMessage = spResponse.First().Message,
                                result = false,
                                ResultCode = spResponse.First().Result
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug("SP Usp_InsertCommentsDetails returned null", "Insert Comments", "Portal", "Document");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = "DB Error",
                            ResultCode = "-2"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("comment field is empty", "Insert Comments", "Portal", "Document");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Insert Comments", "Portal", "Document");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
        }

        public static UtilityResponseModel saveDocumentInfo(string documentType, string documentName, string DocumentHeaderID, string DocumentDetailID)
        {
            try
            {
                int documentHeaderID;
                int documentDetailID;
                if (!string.IsNullOrEmpty(DocumentDetailID) && !string.IsNullOrEmpty(DocumentHeaderID) && Int32.TryParse(DocumentDetailID, out documentDetailID) && Int32.TryParse(DocumentHeaderID, out documentHeaderID))
                {
                    var spResponse = new DapperHelper().ExecuteSP<Models.DocumentTypeMaster>("Usp_UpdateDocumentDetails", ConfigurationModel.ConnectionString, new
                    {
                        DocHeaderID = documentHeaderID,
                        DocumentDetailID = documentDetailID,
                        DocumentName = documentName,
                        DocumentType = documentType,
                        IsEnabled = true
                    }).ToList();
                    if (spResponse != null && spResponse.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                        {
                            return new UtilityResponseModel()
                            {

                                result = true,
                                ResultCode = "200"
                            };
                        }
                        else
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseMessage = spResponse.First().Message,
                                result = false,
                                ResultCode = spResponse.First().Result
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug("SP Usp_UpdateDocumentDetails returned null", "Save Document Info", "Portal", "Document");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = "DB Error",
                            ResultCode = "-2"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("Document header and detailid is NULL", "Save Document Info", "Portal", "Document");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Save Document Info", "Portal", "Document");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
        }
        
        public static UtilityResponseModel saveDocumentSignature(byte[] documentSignature, string DocumentHeaderID, string DocumentDetailID, string documentType)
        {
            try
            {
                int documentHeaderID;
                int documentDetailID;
                if (!string.IsNullOrEmpty(DocumentDetailID) && !string.IsNullOrEmpty(DocumentHeaderID) && Int32.TryParse(DocumentDetailID, out documentDetailID) && Int32.TryParse(DocumentHeaderID, out documentHeaderID))
                {
                    var spResponse = new DapperHelper().ExecuteSP<Models.DocumentTypeMaster>("Usp_UpdateDocumentDetails", ConfigurationModel.ConnectionString, new
                    {
                        DocHeaderID = documentHeaderID,
                        DocumentDetailID = documentDetailID,
                        SignatureFile = documentSignature,
                        DocumentType = documentType,
                        IsEnabled = true
                    }).ToList();
                    if (spResponse != null && spResponse.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                        {
                            return new UtilityResponseModel()
                            {
                                result = true,
                                ResultCode = "200"
                            };
                        }
                        else
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseMessage = spResponse.First().Message,
                                result = false,
                                ResultCode = spResponse.First().Result
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug("SP Usp_UpdateDocumentDetails returned null", "Save Document Signature", "Portal", "Document");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = "DB Error",
                            ResultCode = "-2"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("Document header and detailid is NULL", "Save Document Signature", "Portal", "Document");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Save Document Signature", "Portal", "Document");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
        }

        public static UtilityResponseModel updateDocument(byte[] document, string DocumentHeaderID, string DocumentDetailID, string documentType)
        {
            try
            {
                int documentHeaderID;
                int documentDetailID;
                if (!string.IsNullOrEmpty(DocumentDetailID) && !string.IsNullOrEmpty(DocumentHeaderID) && Int32.TryParse(DocumentDetailID, out documentDetailID) && Int32.TryParse(DocumentHeaderID, out documentHeaderID))
                {
                    var spResponse = new DapperHelper().ExecuteSP<Models.DocumentTypeMaster>("Usp_UpdateDocumentDetails", ConfigurationModel.ConnectionString, new
                    {
                        DocHeaderID = documentHeaderID,
                        DocumentDetailID = documentDetailID,
                        DocumentFile = document,
                        DocumentType = documentType,
                        IsEnabled = true
                    }).ToList();
                    if (spResponse != null && spResponse.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                        {
                            return new UtilityResponseModel()
                            {
                                result = true,
                                ResultCode = "200"
                            };
                        }
                        else
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseMessage = spResponse.First().Message,
                                result = false,
                                ResultCode = spResponse.First().Result
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug("SP Usp_UpdateDocumentDetails returned null", "Update Document", "Portal", "Document");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = "DB Error",
                            ResultCode = "-2"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("Document header and detailid is NULL", "Update Document", "Portal", "Document");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Update Document", "Portal", "Document");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
        }

        public static UtilityResponseModel getUserProfileDetails(string userProfileID = null)
        {
            int ProfileID;
            if (!string.IsNullOrEmpty(userProfileID) && Int32.TryParse(userProfileID,out ProfileID))
            {
                try
                {
                    var spResponse = new DapperHelper().ExecuteSP<UserProfileModel>("usp_GetUserProfileModuleList", ConfigurationModel.ConnectionString, new { UserProfileID = ProfileID }).ToList();
                    if (spResponse != null && spResponse.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseData = spResponse,
                                result = true,
                                ResultCode = "200"
                            };
                        }
                        else
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseMessage = spResponse.First().Message,
                                result = false,
                                ResultCode = spResponse.First().Result
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug("SP usp_GetUserProfileModuleList returned null", "Get User Profile Details", "Portal", "Master");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = "DB Error",
                            ResultCode = "-2"
                        };
                    }
                }
                catch(Exception ex)
                {
                    LogHelper.Instance.Error(ex, "Get User Profile Details", "Portal", "Master");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "Generic exception",
                        ResultCode = "-1"
                    };
                }
            }
            else
            {
                try
                {
                    var spResponse = new DapperHelper().ExecuteSP<UserProfileModel>("usp_GetUserProfileModuleList", ConfigurationModel.ConnectionString).ToList();
                    if (spResponse != null && spResponse.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseData = spResponse,
                                result = true,
                                ResultCode = "200"
                            };
                        }
                        else
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseMessage = spResponse.First().Message,
                                result = false,
                                ResultCode = spResponse.First().Result
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug("SP usp_GetUserProfileModuleList returned null", "Get User Profile Details", "Portal", "Master");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = "DB Error",
                            ResultCode = "-2"
                        };
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Error(ex, "Get User Profile Details", "Portal", "Master");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "Generic exception",
                        ResultCode = "-1"
                    };
                }
            }
        }


        public static UtilityResponseModel getUserProfileList(string userProfileID = null)
        {
            int ProfileID;
            if (!string.IsNullOrEmpty(userProfileID) && Int32.TryParse(userProfileID, out ProfileID))
            {
                try
                {
                    var spResponse = new DapperHelper().ExecuteSP<UserProfileModel>("usp_GetUserProfileMasterList", ConfigurationModel.ConnectionString, new { UserProfileID = ProfileID }).ToList();
                    if (spResponse != null && spResponse.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseData = spResponse,
                                result = true,
                                ResultCode = "200"
                            };
                        }
                        else
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseMessage = spResponse.First().Message,
                                result = false,
                                ResultCode = spResponse.First().Result
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug("SP usp_GetUserProfileMasterList returned null", "Get User Profile List", "Portal", "Master");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = "DB Error",
                            ResultCode = "-2"
                        };
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Error(ex, "Get User Profile List", "Portal", "Master");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "Generic exception",
                        ResultCode = "-1"
                    };
                }
            }
            else
            {
                try
                {
                    var spResponse = new DapperHelper().ExecuteSP<UserProfileModel>("usp_GetUserProfileMasterList", ConfigurationModel.ConnectionString).ToList();
                    if (spResponse != null && spResponse.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseData = spResponse,
                                result = true,
                                ResultCode = "200"
                            };
                        }
                        else
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseMessage = spResponse.First().Message,
                                result = false,
                                ResultCode = spResponse.First().Result
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug("SP usp_GetUserProfileModuleList returned null", "Get User Profile List", "Portal", "Master");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = "DB Error",
                            ResultCode = "-2"
                        };
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Error(ex, "Get User Profile List", "Portal", "Master");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "Generic exception",
                        ResultCode = "-1"
                    };
                }
            }
        }
        public static UtilityResponseModel getPropertyDetails(string PropertyID = null)
        {
            int propertyID;
            if (!string.IsNullOrEmpty(PropertyID) && Int32.TryParse(PropertyID, out propertyID))
            {
                try
                {
                    var spResponse = new DapperHelper().ExecuteSP<PropertyMasterModel>("usp_GetPropertyList", ConfigurationModel.ConnectionString, new { PropertyID = propertyID }).ToList();
                    if (spResponse != null && spResponse.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseData = spResponse,
                                result = true,
                                ResultCode = "200"
                            };
                        }
                        else
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseMessage = spResponse.First().Message,
                                result = false,
                                ResultCode = spResponse.First().Result
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug("SP usp_GetPropertyList returned null", "Get Property Details", "Portal", "Master");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = "DB Error",
                            ResultCode = "-2"
                        };
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Error(ex, "Get Property Details", "Portal", "Master");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "Generic exception",
                        ResultCode = "-1"
                    };
                }
            }
            else
            {
                try
                {
                    var spResponse = new DapperHelper().ExecuteSP<PropertyMasterModel>("usp_GetPropertyList", ConfigurationModel.ConnectionString).ToList();
                    if (spResponse != null && spResponse.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseData = spResponse,
                                result = true,
                                ResultCode = "200"
                            };
                        }
                        else
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseMessage = spResponse.First().Message,
                                result = false,
                                ResultCode = spResponse.First().Result
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug("SP usp_GetPropertyList returned null", "Get User Profile Details", "Portal", "Master");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = "DB Error",
                            ResultCode = "-2"
                        };
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Error(ex, "Get Property Details", "Portal", "Master");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "Generic exception",
                        ResultCode = "-1"
                    };
                }
            }
        }

        public static UtilityResponseModel getUserListDetails(int? userID = null)
        {
            try
            {
                //var spResponse =  new DapperHelper().ExecuteSP<UserModel>("usp_GetUserList", ConfigurationModel.ConnectionString, new { UserID = userID }).ToList();



                var spResponse = userID != null ? (new DapperHelper().ExecuteSP<UserModel>(
                                                    "usp_GetUserList",
                                                    ConfigurationModel.ConnectionString,
                                                    new { UserID = userID.Value }).ToList())
                                                : (new DapperHelper().ExecuteSP<UserModel>(
                                                    "usp_GetUserList",
                                                    ConfigurationModel.ConnectionString).ToList());
                if (spResponse != null && spResponse.Count > 0)
                {
                    {
                        return new UtilityResponseModel()
                        {
                            ResponseData = spResponse,
                            result = true,
                            ResultCode = "200"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP usp_GetUserList returned null", "Get User List Details", "Portal", "Master");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Get User List Details", "Portal", "Master");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
                        
        }

        public static UtilityResponseModel fetchAuditHeaderDetails(int PageNumber,int PageSize,string StartDate, string EndDate,string search,string Sort,string SortBy,string Timezoneid)
        {
            try
            {

                var spResponse =  new DapperHelper().ExecuteSP<AuditHeaderModel>(
                                                    "Usp_GetAuditHeaderDetails",
                                                    ConfigurationModel.ConnectionString,new {PageNumber,PageSize,StartDate,EndDate,search,Sort,SortBy,Timezoneid }).ToList();
                if (spResponse != null && spResponse.Count > 0)
                {
                    {
                        return new UtilityResponseModel()
                        {
                            ResponseData = spResponse,
                            result = true,
                            ResultCode = "200"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP Usp_GetAuditHeaderDetails returned null", "Fetch Audit Header Details", "Portal", "Reports");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Fetch Audit Header Details", "Portal", "Reports");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }

        }

        public static UtilityResponseModel fetchApprovalDocumentList(int PageNumber, int PageSize, bool ApprovalStatus, int UserID,string Sort,string SortBy,string search)
        {
            try
            {
                var spResponse = new DapperHelper().ExecuteSP<DocumentFileSummaryModel>(
                                                    "Usp_FetchDocumentApprovals",
                                                    ConfigurationModel.ConnectionString, new { PageNumber, PageSize, ApprovalStatus, UserID,Sort,SortBy,search }).ToList();
                if (spResponse != null && spResponse.Count > 0 && !string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                {
                    {
                        return new UtilityResponseModel()
                        {
                            ResponseData = spResponse,
                            result = true,
                            ResultCode = "200"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP Usp_FetchDocumentApprovals returned null", "Fetch Approval Document List", "Portal", "Document");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Fetch Approval Document List", "Portal", "Document");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }

        }


        public static UtilityResponseModel getAuditDetails(int auditHeaderID)
        {
            try
            {
                var spResponse = new DapperHelper().ExecuteSP<AuditDetailsModel>(
                                                    "Usp_FetchAuditDetailsChanges",
                                                    ConfigurationModel.ConnectionString,
                                                    new { AuditHeaderID = auditHeaderID }).ToList();
                if (spResponse != null && spResponse.Count > 0)
                {
                    {
                        return new UtilityResponseModel()
                        {
                            ResponseData = spResponse,
                            result = true,
                            ResultCode = "200"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP Usp_FetchAuditDetailsChanges returned null", "Get Audit Details", "Portal", "Reports");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Get Audit Details", "Portal", "Reports");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }

        }

        public static UtilityResponseModel getAuditDetailsForReport(string StartDate,string EndDate,string Timezoneid )
        {
            try
            {
               /// Timezoneid = "'"+ Timezoneid + "'";
                var spResponse = new DapperHelper().ExecuteSPForDataSet(
                                                    "Usp_RPT_AuditDetails ",
                                                    ConfigurationModel.ConnectionString,
                                                    new { StartDate = StartDate, EndDate = EndDate, TimeZone = Timezoneid });
                if (spResponse != null && spResponse.Rows.Count > 0)
                {
                    return new UtilityResponseModel()
                    {
                        DataTable = spResponse,
                        ResponseMessage = "Success",
                        result = true,
                        ResultCode = "200"
                    };
                }
                else
                {
                    return new UtilityResponseModel()
                    {
                        ResponseData = null,
                        ResponseMessage = "DBError",
                        result = false,
                        ResultCode = "-2"
                    };
                }
                
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Get Audit Details For Report", "Portal", "Reports");
                return new UtilityResponseModel()
                {
                    ResponseData = null,
                    ResponseMessage = "General Exception",
                    result = false,
                    ResultCode = "-1"
                };
            }

        }

        public static UtilityResponseModel getApprovalDetailsForDocID(string DocumentHeaderID)
        {
            try
            {
                var spResponse = new DapperHelper().ExecuteSP<DocumentApprovalDetails>(
                                                    "Usp_GetUserApprovalStatus",
                                                    ConfigurationModel.ConnectionString,
                                                    new { DocumentHeaderID = DocumentHeaderID }).ToList();
                if (spResponse != null && spResponse.Count > 0)
                {
                    return new UtilityResponseModel()
                    {
                        ResponseData = spResponse,
                        ResponseMessage = "Success",
                        result = true,
                        ResultCode = "200"
                    };
                }
                else
                {
                    return new UtilityResponseModel()
                    {
                        ResponseData = null,
                        ResponseMessage = "DBError",
                        result = false,
                        ResultCode = "-2"
                    };
                }

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Get Audit Details For Report", "Portal", "Reports");
                return new UtilityResponseModel()
                {
                    ResponseData = null,
                    ResponseMessage = "General Exception",
                    result = false,
                    ResultCode = "-1"
                };
            }

        }

        public static UtilityResponseModel insertApprovalStatus(int DocumentHeaderID, string UserSignature, string UserID,bool ApprovalStatus,bool RejectStatus,int userid)
        {
            try
            {
                if (!string.IsNullOrEmpty(UserID))
                {
                    var spResponse = new DapperHelper().ExecuteSP<ApprovalResponseModel>("Usp_InsertApprovalDetails", ConfigurationModel.ConnectionString, new
                    {
                        DocumentHeaderID = DocumentHeaderID,
                        UserID = Int32.Parse(UserID),
                        SignatureFile = !string.IsNullOrEmpty(UserSignature) ? Convert.FromBase64String(UserSignature) : null,
                        ApprovalStatus = ApprovalStatus,
                        RejectStatus = RejectStatus,
                        SenderId = userid
                    }).ToList();
                    if (spResponse != null && spResponse.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseData = spResponse.First().ModuleID,
                                result = true,
                                ResultCode = "200"
                            };
                        }
                        else
                        {
                            return new UtilityResponseModel()
                            {
                                ResponseMessage = spResponse.First().Message,
                                result = false,
                                ResultCode = spResponse.First().Result
                            };
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug("SP Usp_InsertApprovalDetails returned null", "Insert Approval Status", "Portal", "Document");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = "DB Error",
                            ResultCode = "-2"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("comment field is empty", "Insert Approval Status", "Portal", "Document");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Insert Approval Status", "Portal", "Document");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
        }
        public static UtilityResponseModel VerifyDocumentApprovalCount(int ApprovalRecordCount)
        {
            if (ConfigurationModel.Settings != null && ConfigurationModel.Settings.Count > 0)
            {
                var settings = ConfigurationModel.Settings.Find(x => x.SettingName.Equals("DocumentApprovalLimit"));
                int ApprovalThreshold;
                if (settings != null && !string.IsNullOrEmpty(settings.SettingValue) && Int32.TryParse(settings.SettingValue, out ApprovalThreshold))
                {
                    if (ApprovalRecordCount >= ApprovalThreshold)
                    {
                        return new UtilityResponseModel()
                        {
                            result = true,
                            ResponseData = true
                        };
                    }
                    else
                    {
                        return new UtilityResponseModel()
                        {
                            result = true,
                            ResponseData = false
                        };
                    }
                }
                else
                {
                    return new UtilityResponseModel()
                    {
                        result = true,
                        ResponseData = false
                    };
                }
            }
            else
            {
                return new UtilityResponseModel()
                {
                    result = true,
                    ResponseData = false
                };
            }
        }

        public static UtilityResponseModel GetDocumentPerUserCount(string UserID,int ApprovalStatus)
        {

            try
            {
                var spResponse = new DapperHelper().ExecuteSP<DocumentCount>(
                                                    "Usp_GetDocumentPerUserCount",
                                                    ConfigurationModel.ConnectionString,
                                                    new { UserID = UserID,ApprovalStatus=ApprovalStatus }).ToList();
                if (spResponse != null && spResponse.Count > 0)
                {
                    {
                        return new UtilityResponseModel()
                        {
                            ResponseData = spResponse.SingleOrDefault().Counts,
                            result = true,
                            ResultCode = "200"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("Usp_GetDocumentPerUserCount returned null", "GetDocumentPerUserCount", "Portal", "Document");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "GetDocumentPerUserCount", "Portal", "Document");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
        }

        public static UtilityResponseModel FetchUserListDetails(int PageNumber, int PageSize,string search, string Sort, string SortBy)
        {
            try
            {
                //var spResponse =  new DapperHelper().ExecuteSP<UserModel>("usp_GetUserList", ConfigurationModel.ConnectionString, new { UserID = userID }).ToList();



                var spResponse =new DapperHelper().ExecuteSP<UserModel>(
                                                    "usp_GetUserPageList",
                                                    ConfigurationModel.ConnectionString, new { PageNumber, PageSize, search, Sort, SortBy }).ToList();
                if (spResponse != null && spResponse.Count > 0)
                {
                    {
                        return new UtilityResponseModel()
                        {
                            ResponseData = spResponse,
                            result = true,
                            ResultCode = "200"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP usp_GetUserList returned null", "Get User List Details", "Portal", "Master");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Get User List Details", "Portal", "Master");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }

        }

        public static UtilityResponseModel getDocumentMasterList(int PageNumber, int PageSize,string search, string Sort, string SortBy)
        {
            try
            {
                var spResponse = new DapperHelper().ExecuteSP<Models.DocumentTypeMaster>("usp_GetDocumentMasterPageList", ConfigurationModel.ConnectionString,new { PageNumber, PageSize, search, Sort, SortBy }).ToList();
                if (spResponse != null && spResponse.Count > 0)
                {
                    if (!string.IsNullOrEmpty(spResponse.First().Result) && spResponse.First().Result.Equals("200"))
                    {
                        return new UtilityResponseModel()
                        {
                            ResponseData = spResponse,
                            result = true,
                            ResultCode = "200"
                        };
                    }
                    else
                    {
                        return new UtilityResponseModel()
                        {
                            ResponseMessage = spResponse.First().Message,
                            result = false,
                            ResultCode = spResponse.First().Result
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP Usp_GetDocumentMaster returned null", "Get Document Group List", "Portal", "Document");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Get Document Group List", "Portal", "Document");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
        }
        public static UtilityResponseModel getUserModeListDetails(int? userID = null)
        {
            try
            {
                //var spResponse =  new DapperHelper().ExecuteSP<UserModel>("usp_GetUserList", ConfigurationModel.ConnectionString, new { UserID = userID }).ToList();



                var spResponse = userID != null ? (new DapperHelper().ExecuteSP<UpdateUserModel>(
                                                    "usp_GetUserList",
                                                    ConfigurationModel.ConnectionString,
                                                    new { UserID = userID.Value }).ToList())
                                                : (new DapperHelper().ExecuteSP<UpdateUserModel>(
                                                    "usp_GetUserList",
                                                    ConfigurationModel.ConnectionString).ToList());
                if (spResponse != null && spResponse.Count > 0)
                {
                    {
                        return new UtilityResponseModel()
                        {
                            ResponseData = spResponse,
                            result = true,
                            ResultCode = "200"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP usp_GetUserList returned null", "Get User List Details", "Portal", "Master");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Get User List Details", "Portal", "Master");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }

        }

        public static UtilityResponseModel GetApprovalDetails(int DocumentHeaderID)
        {
            try
            {
                var spResponse = new DapperHelper().ExecuteSP<ApproverDetails>(
                                                    "Usp_GetApproverDetails",
                                                    ConfigurationModel.ConnectionString,
                                                    new { DocHeaderID = DocumentHeaderID }).ToList();
                if (spResponse != null && spResponse.Count > 0)
                {
                    {
                        return new UtilityResponseModel()
                        {
                            ResponseData = spResponse,
                            result = true,
                            ResultCode = "200"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP GetApprovalDetails returned null", "GetApprovalDetails", "Portal", "Document");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "GetApprovalDetails", "Portal", "Document");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }

        }
        public static UtilityResponseModel GetApprovalDetailsForPDF(int DocumentHeaderID)
        {
            try
            {
                /// Timezoneid = "'"+ Timezoneid + "'";
                var spResponse = new DapperHelper().ExecuteSPForDataSet(
                                                    "Usp_GetApproverDetails ",
                                                    ConfigurationModel.ConnectionString,
                                                     new { DocHeaderID = DocumentHeaderID });
                if (spResponse != null && spResponse.Rows.Count > 0)
                {
                    return new UtilityResponseModel()
                    {
                        DataTable = spResponse,
                        ResponseMessage = "Success",
                        result = true,
                        ResultCode = "200"
                    };
                }
                else
                {
                    return new UtilityResponseModel()
                    {
                        ResponseData = null,
                        ResponseMessage = "DBError",
                        result = false,
                        ResultCode = "-2"
                    };
                }

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "GetApprovalDetailsForPDF", "Portal", "Document");
                return new UtilityResponseModel()
                {
                    ResponseData = null,
                    ResponseMessage = "General Exception",
                    result = false,
                    ResultCode = "-1"
                };
            }

        }
        public static UtilityResponseModel getUserById(int userID)
        {
            try
            {
              


                var spResponse = new DapperHelper().ExecuteSP<UserModel>(
                                                    "usp_GetUserById",
                                                    ConfigurationModel.ConnectionString,
                                                    new { UserID = userID }).ToList();
                                    
                if (spResponse != null && spResponse.Count > 0)
                {
                    {
                        return new UtilityResponseModel()
                        {
                            ResponseData = spResponse,
                            result = true,
                            ResultCode = "200"
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP usp_GetUserList returned null", "Get User List Details", "Portal", "Master");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Get User List Details", "Portal", "Master");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }

        }
        public static UtilityResponseModel getDocumentFile(int documentDetailID)
        {
            try
            {
                var spResponse = new DapperHelper().ExecuteSP<DocumentModel>("Usp_GetDocumentfile", ConfigurationModel.ConnectionString, new { DocumentDetailID = documentDetailID

                }) 
                .ToList();
                if (spResponse != null && spResponse.Count > 0)
                {
                   
                        return new UtilityResponseModel()
                        {
                            ResponseData = spResponse,
                            result = true,
                            ResultCode = "200"
                        };
                    
                }
                else
                {
                    LogHelper.Instance.Debug("SP Usp_GetDocumentDetails returned null", "Get Document Details", "Portal", "Document");
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResponseMessage = "DB Error",
                        ResultCode = "-2"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Get Document Details", "Portal", "Document");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
        }
        public static UtilityResponseModel UserSecurityQuestion(UserFormModel user)
        {
            try
            {
                var spResponse = new DapperHelper().ExecuteSP<SPResponseModel>("Usp_InsertUserSecurityAnswers", ConfigurationModel.ConnectionString, new
                {
                    UserID = user.UserID,
                   
                    SecurityQuestionID = Int32.Parse(user.SecuirityQuestion),
                    SecurityAnswer = user.SecurityAnswer
                }).ToList();
                if (spResponse != null && spResponse.Count > 0)
                {
                   
                            return new UtilityResponseModel()
                            {
                                result = true,
                                ResponseMessage = spResponse.First().Message,
                                ResultCode = spResponse.First().Result
                            };
                      
                    }
                    else
                    {
                        LogHelper.Instance.Debug($"SP UserSecurityQuestion returned Error Code : {spResponse.First().Result} and Message : {spResponse.First().Message}", "UserSecurityQuestion By UserID", "Portal", "Login");
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResponseMessage = spResponse.First().Message,
                            ResultCode = spResponse.First().Result
                        };
                    }
                
                
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "UserSecurityQuestion By UserID", "Portal", "Login");
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
        }
        public static UtilityResponseModel ProcessDocument(DocumentRequestModel documentRequest)
        {
            try
            {

                LogHelper.Instance.Debug($"document starting processing named" + documentRequest.DocumentName, "ProcessDocument", "PortalAPI", "ProcessDocument");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
                byte[] documentBytes = Convert.FromBase64String(documentRequest.DocumentBase64);

               


                var result = dapper.ExecuteSP<Models.DocumentResponseModel>("Usp_UploadDocumentMobile", connectionstring, new 
                {
                    UserName = documentRequest.Username,
                    DocumentFile = documentBytes,
                    DocumentFileName = documentRequest.DocumentName,
                    DocumentType = documentRequest.DocumentType,
                  
                    ReservationNumber = documentRequest.DocumentName,
                   
                    RoomNo = documentRequest.RoomNo
                   
                });
                //string ConnectionString = ConfigurationManager.AppSettings["CloudConnectionString"];

                //BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);


                //await new BlobStorage().UploadFileBlobAsync(documentBytes, "document" + result.FirstOrDefault().DocumentDetailID + ".pdf", blobServiceClient);

                LogHelper.Instance.Debug($"document inserted successfully to blob" + documentRequest.DocumentName, "ProcessDocument", "PortalAPI", "ProcessDocument");

                return new UtilityResponseModel()
                {
                    result = true,
                    ResponseMessage = result.First().Message,
                    ResultCode = result.First().Result,
                    ResponseData = result.First().DocumentDetailID
                };
            }
            catch (Exception ex)
            {
                return new UtilityResponseModel()
                {
                    result = false,
                    ResponseMessage = "Generic exception",
                    ResultCode = "-1"
                };
            }
        }

    }
}