using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
[assembly: OwinStartup(typeof(DigiDoc.StartUp))]
namespace DigiDoc
{
    public class StartUp
    {
        public void Configuration(IAppBuilder app)
        {         
            var config = new HubConfiguration();
            config.EnableJSONP = true;
            app.MapSignalR(config);
        }
    }
}