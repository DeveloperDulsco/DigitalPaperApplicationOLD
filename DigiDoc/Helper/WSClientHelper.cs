using DigiDoc.Models;
using DigiDoc.Models.Ereg;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DigiDoc.Helper
{
    public class WSClientHelper
    {

        HttpClient getProxyClient(string groupName, string ProxyHost, string proxyUserName, string proxyPassword, string clientID)
        {
            new LogHelper().Debug("assigning proxy credentials :- (host:" + ProxyHost + ",UN:" + proxyUserName + ",Password:" + proxyPassword + ")", "getProxyClient", clientID, groupName);
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.UseDefaultCredentials = true;
                var proxy = new WebProxy
                {
                    Address = new Uri(ProxyHost),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = false,

                    Credentials = new NetworkCredential(
                    userName: proxyUserName,
                    password: proxyPassword)
                };

                var httpClientHandler = new HttpClientHandler
                {
                    Proxy = proxy,
                };
                new LogHelper().Debug("proxy credentials assigned", "getProxyClient", clientID, groupName);
                return new HttpClient(handler: httpClientHandler, disposeHandler: true);
            }
            catch (Exception ex)
            {
                new LogHelper().Error(ex,  "getProxyClient", clientID, groupName);
                return null;
            }

        }

        public async Task<EmailResponse> SendEmail(EmailRequest emailRequest, string groupName,ServiceParameters serviceParameters)
        {
            try
            {
                new LogHelper().Debug("Sending email using web api", "", "SendEmail", groupName);
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                {
                    return true;
                };

                HttpClient httpClient = serviceParameters.isProxyEnableForEmailAPI ? getProxyClient(groupName, serviceParameters.EmailAPIProxyHost, serviceParameters.EmailAPIProxyUN, serviceParameters.EmailAPIProxyPswd, serviceParameters.ClientID) : new HttpClient();
                if (httpClient == null)
                {
                    new LogHelper().Debug("Failled to send email using web api due to proxy error", "", "SendEmail", groupName);
                    return new EmailResponse()
                    {
                        result = false,
                        responseMessage = "Failled to generate the proxy http client"
                    };
                }
                httpClient.DefaultRequestHeaders.Clear();
                string requestString = JsonConvert.SerializeObject(emailRequest, Formatting.None);
                var requestContent = new StringContent(requestString, Encoding.UTF8, "application/json");
                new LogHelper().Debug("web api url :- " + serviceParameters.EmailURL + @"SendEmail", "", "SendEmail", groupName);
                new LogHelper().Debug("web api request :- " + requestString, "", "SendEmail",  groupName);
                HttpResponseMessage response = await httpClient.PostAsync(serviceParameters.EmailURL + @"SendEmail", requestContent);
                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        new LogHelper().Debug("web API response :- " + apiResponse, "", "SendEmail",  groupName);
                        EmailResponse emailResponse = JsonConvert.DeserializeObject<EmailResponse>(apiResponse);
                        return emailResponse;
                    }
                    else
                    {
                        new LogHelper().Debug("Failled to send email using web api due to HTTP error : " + response.ReasonPhrase, "", "SendEmail",groupName);
                        return new EmailResponse()
                        {
                            result = false,
                            responseMessage = response.ReasonPhrase
                        };
                    }
                }
                else
                {
                    new LogHelper().Debug("Failled to send email using web api due to null returned from the local web api", "", "SendEmail", groupName);
                    return new EmailResponse()
                    {
                        result = false,
                        responseMessage = "Email web API returned null"
                    };
                }
            }
            catch (Exception ex)
            {
                new LogHelper().Error(ex,  "SendEmail", serviceParameters.ClientID, groupName);
                return new EmailResponse()
                {
                    result = false,
                    responseMessage = "Generic Exception : " + ex.Message
                };
            }
        }
        public async Task<EregResponseModel> GetImages(string groupName, ServiceParameters serviceParameters)
        {
            try
            {
                new LogHelper().Debug("GetImages using web api", "GetImages", "DigiDocMobile", groupName);
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                {
                    return true;
                };

                HttpClient httpClient = serviceParameters.isProxyEnableForDigiDocAPI ? getProxyClient(groupName, serviceParameters.DigiDocAPIProxyHost, serviceParameters.DigiDocAPIProxyUN, serviceParameters.DigiDocAPIProxyHost, serviceParameters.ClientID) : new HttpClient();
                if (httpClient == null)
                {
                    new LogHelper().Debug("Failled to get images using web api due to proxy error", "GetImages", "DigiDocMobile", groupName);
                    return new EregResponseModel()
                    {
                        result = false,
                        responseMessage = "Failled to generate the proxy http client"
                    };
                }
                httpClient.DefaultRequestHeaders.Clear();

                new LogHelper().Debug("web api url :- " + serviceParameters.DigiDocURL + @"GetImages", "GetImages", "DigiDocMobile", groupName);

                HttpResponseMessage response = await httpClient.GetAsync(serviceParameters.DigiDocURL + @"GetImages");
                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        new LogHelper().Debug("web API response :- " + apiResponse, "GetImages", "DigiDocMobile", groupName);
                        EregResponseModel Response = JsonConvert.DeserializeObject<EregResponseModel>(apiResponse);
                        if (Response.responseData != null)
                            Response.responseData = JsonConvert.DeserializeObject<List<SliderImage>>(Response.responseData.ToString());

                        return Response;
                    }
                    else
                    {
                        new LogHelper().Debug("Failled to get images using web api due to HTTP error : " + response.ReasonPhrase, "GetImages", "DigiDocMobile", groupName);
                        return new EregResponseModel()
                        {
                            result = false,
                            responseMessage = response.ReasonPhrase
                        };
                    }
                }
                else
                {
                    new LogHelper().Debug("Failled to get images using web api due to null returned from the local web api", "GetImages", "DigiDocMobile", groupName);
                    return new EregResponseModel()
                    {
                        result = false,
                        responseMessage = "Email web API returned null"
                    };
                }
            }
            catch (Exception ex)
            {
                new LogHelper().Error(ex, "GetImages", serviceParameters.ClientID, groupName);
                return new EregResponseModel()
                {
                    result = false,
                    responseMessage = "Generic Exception : " + ex.Message
                };
            }
        }

    }
}