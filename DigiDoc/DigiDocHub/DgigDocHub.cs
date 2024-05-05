using DigiDoc.Helper;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigiDoc.DigiDocHib
{
    public class DigiDocHub : Hub
    {

        public static ConcurrentDictionary<string, List<string>> ConnectedUsers = new ConcurrentDictionary<string, List<string>>();
        public void SendNotification()
        {
            Clients.All.broadcastNotification();
        }
        /// <summary>
        /// To Send Files to IPAD
        /// </summary>
        /// <param name="name"></param>
        /// <param name="TempId"></param>
        /// <param name="ClientID"></param>
        public void Send(string name, string TempId, string ClientID)
        {
            LogHelper.Instance.Log($"Started- Document Send to IPAd " + Context.ConnectionId, "Send", "DigiPortal", "Send");

            var connection = string.Empty;
            var connectionlist = ConnectedUsers.Where(x => x.Key == name).SelectMany(x => x.Value).ToList();

            if (connectionlist.Any())
            {

             

                connection = connectionlist.LastOrDefault(); 
                var s = Clients.Client(connection);
                LogHelper.Instance.Debug($"Send - Message To IPAd" + name, "Send", "DigiPortal", "Send");
                Clients.Client(connection).MessageToApp(TempId, ClientID);
            }
            else

                if (ClientID != null || ClientID != "")
            {
                var activeconnection = MyDictionaryToJson(ConnectedUsers);
                LogHelper.Instance.Log($"Active Connections " + activeconnection, "Send", "DigiPortal", "Send");

                LogHelper.Instance.Log($"Failed -  SignalRConnection Not Found " + name, "Send", "DigiPortal", "Send");
                Notification(ClientID, "103", name);
                LogHelper.Instance.Debug($"Notification to WEBAPP Regarding SignalRConnection Not Found " + Context.ConnectionId, "Send", "DigiPortal", "Send");
            }
        }
        /// <summary>
        /// Cancel By the Client
        /// </summary>
        /// <param name="name"></param>
        /// <param name="TempId"></param>
        /// <param name="DocumentId"></param>
        public void Cancel(string name, string TempId, string DocumentId)
        {
            LogHelper.Instance.Log($"Cancel from WebAPP" + name, "Cancel", "DigiPortal", "Cancel");

            var connection = string.Empty;
            var connectionlist = ConnectedUsers.Where(x => x.Key == name).SelectMany(x => x.Value).ToList();

            if (connectionlist.Any())
            {
                connection = connectionlist.LastOrDefault();
                Clients.Client(connection).CancelOpera(TempId, DocumentId);

            }
            else
            {
                LogHelper.Instance.Log($"Failed -  SignalRConnection Not Found " + name, "Cancel", "DigiPortal", "Cancel");

            }
        }
        /// <summary>
        /// Calling WebAPP to Update RegCard
        /// </summary>
        /// <param name="name"></param>
        /// <param name="TempId"></param>
        /// <param name="DocumentId"></param>
            public void CallOpera(string name, string TempId,string DocumentId)
        {
            LogHelper.Instance.Log($"Calling WebAPP to Update RegCard" + name, "Cancel", "DigiPortal", "Cancel");

            var connection = string.Empty;
            var connectionlist = ConnectedUsers.Where(x => x.Key == name).SelectMany(x => x.Value).ToList();

            if (connectionlist.Any())
            {
                connection = connectionlist.LastOrDefault();
                Clients.Client(connection).MessageToOpera(TempId, DocumentId);
            }
            else
            {
                LogHelper.Instance.Log($"Failed -  SignalRConnection Not Found " + name, "Cancel", "DigiPortal", "Cancel");

            }


        }
        /// <summary>
        /// ALL Notification to and from IPAd
        /// </summary>
        /// <param name="name"></param>
        /// <param name="status"></param>
        /// <param name="device"></param>
        public void Notification(string name, string status,string device)
        {
            LogHelper.Instance.Log($"Notification to device started -" + name, "Notification", "DigiPortal", "Notification");

            var connection = string.Empty;
            var connectionlist = ConnectedUsers.Where(x => x.Key == name).SelectMany(x => x.Value).ToList();

            if (connectionlist.Any())
            {
                var message = "";
                var spResponse = DigiDocMobileHelper.GetStatus(status);
               if(spResponse != null)
                {
                    if (spResponse.result)
                    {
                        var result = (DigiDoc.Models.Ereg.NotificationStatus)spResponse.ResponseData;
                        if (result != null)
                        {
                            LogHelper.Instance.Debug($"Notification to device started -" + name, "Notification", "DigiPortal", "Notification");

                            message = result.Message;
                        }
                        else
                        {
                            LogHelper.Instance.Log($"Notification status response data is null -" + name, "Notification", "DigiPortal", "Notification");

                        }
                    }
                    else
                    {
                        LogHelper.Instance.Log($"Notification status result is false -" + name, "Notification", "DigiPortal", "Notification");

                    }
                }
               else
                {
                    LogHelper.Instance.Log($"Notification status for status not found - -" + status, "Notification", "DigiPortal", "Notification");

                }
                connection = connectionlist.LastOrDefault();
                Clients.Client(connection).NotificationToOpera(status,device,message);
            }
            else
            {
                LogHelper.Instance.Log($"Failed -  SignalRConnection Not Found " + name, "Notification", "DigiPortal", "Notification");

            }


        }
        public void FolioNotification(string name, string status,string device,string TempId)
        {
            LogHelper.Instance.Log($"Folio Notification to device started -" + name, "FolioNotification", "DigiPortal", "FolioNotification");

            var connection = string.Empty;
            var connectionlist = ConnectedUsers.Where(x => x.Key == name).SelectMany(x => x.Value).ToList();

            if (connectionlist.Any())
            {
                connection = connectionlist.LastOrDefault();
                Clients.Client(connection).FolioNotification(status,device,TempId);
            }
            else
            {
                LogHelper.Instance.Log($"Failed -  SignalRConnection Not Found " + name, "FolioNotification", "DigiPortal", "FolioNotification");

            }


        }
        /// <summary>
        /// Not Used
        /// </summary>
        /// <param name="name"></param>
        /// <param name="status"></param>
        public void ApproveNotification(string name, string status)
        {
            var connection = string.Empty;
            var connectionlist = ConnectedUsers.Where(x => x.Key == name).SelectMany(x => x.Value).ToList();

            if (connectionlist.Any())
            {
                connection = connectionlist.LastOrDefault();
                Clients.Client(connection).ApproveNotification(status);
            }


        }
        /// <summary>
        /// Geting User Connection from Dictionary
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public string GetUserConnection(string test)
        {
            var connectionlist = ConnectedUsers.Where(x => x.Key == test).SelectMany(x => x.Value).ToList();
            string connection=string.Empty;
            if (connectionlist.Any())
            {
                connection = connectionlist.LastOrDefault();
            }
            return connection;
        }
        /// <summary>
        /// Connection to SignalR
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            Task con = null;
          
            try
            {
                if (Context.QueryString.GetValues("username") == null)
                {
                    LogHelper.Instance.Log($"Context.QueryString.GetValues(username) is null", "OnConnected", "DigiPortal", "OnConnected");
                }
                else
                {
                    LogHelper.Instance.Log($"Context.QueryString.GetValues(username) value is "+ Context.QueryString.GetValues("username").FirstOrDefault(), "OnConnected", "DigiPortal", "OnConnected");
                }
                var username = Context.QueryString.GetValues("username").FirstOrDefault();
                username = username != null ? username = username.Replace("\"", string.Empty).Trim() : username;
                bool writelog = Context.QueryString.GetValues("writelog")!=null? Context.QueryString.GetValues("writelog").FirstOrDefault() != null ? Convert.ToBoolean(Context.QueryString.GetValues("writelog").FirstOrDefault()) : false:false;
                LogHelper.Instance.Log($"Ipad name and connection used for creating connection are "+username+ "and "+ Context.ConnectionId, "OnConnected", "DigiPortal", "OnConnected");
                List<string> existingUserConnectionIds;
                ConnectedUsers.TryGetValue(username, out existingUserConnectionIds);

                if (existingUserConnectionIds == null)
                {
                    existingUserConnectionIds = new List<string>();
                }
                else
                {
                    existingUserConnectionIds.Clear();
                }
                LogHelper.Instance.Log($"existingUserConnectionIds Ipad name used for creating connection" + username, "OnConnected", "DigiPortal", "OnConnected");
                existingUserConnectionIds.Add(Context.ConnectionId);
                LogHelper.Instance.Log($"Ipad name used for creating connection" + Context.ConnectionId, "OnConnected", "DigiPortal", "OnConnected");
                if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(Context.ConnectionId))
                {
                    var assignedval = "";


                    var spResponse = DigiDocMobileHelper.UpdateAppDetails(new Models.Ereg.AppDetails()
                    {
                        AppName = username,
                        connectionId = Context.ConnectionId,
                        IsAssinged = assignedval == "1" ? true : false,
                    });

                    if (spResponse == null)
                    {
                        LogHelper.Instance.Debug($"Connection details not inserted into db", "OnConnected", "DigiPortal", "OnConnected");
                    }
                    else
                    {
                        if (spResponse.result)
                        {
                            LogHelper.Instance.Debug($"Connection details inserted into db", "OnConnected", "DigiPortal", "OnConnected");
                        }
                        else
                        {
                            LogHelper.Instance.Debug($"Connection details not inserted into db" + spResponse.ResponseMessage, "OnConnected", "DigiPortal", "OnConnected");
                        }

                    }
                }
                LogHelper.Instance.Debug($"Connection started for connection" + Context.ConnectionId, "OnConnected", "DigiPortal", "OnConnected");

                ConnectedUsers.TryAdd(username, existingUserConnectionIds);
                con = base.OnConnected();
                if (con.IsCompleted)
                {
                    if (writelog)
                    {
                        LogHelper.Instance.Debug($"Reset connection for " + username + " is completed", "OnConnected", "DigiPortal", "OnConnected");
                    }
                    }
                else
                {

                    if (writelog)
                    {
                        LogHelper.Instance.Debug($"Reset connection for " + username + " is not completed", "OnConnected", "DigiPortal", "OnConnected");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log($"Inner Exception while creating connection" + ex.InnerException, "OnConnected", "DigiPortal", "OnConnected");
                LogHelper.Instance.Log($"Exception while creating connection" + ex.Message, "OnConnected", "DigiPortal", "OnConnected");
            }
            return con;
        }
        string MyDictionaryToJson(ConcurrentDictionary<string, List<string>> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\"{0}\": [{1}]", d.Key, string.Join(",", d.Value)));
            return "{" + string.Join(",", entries) + "}";
        }
    }
}