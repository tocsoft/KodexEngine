using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace KodexEngine
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            //config.Services.Replace(typeof(IAssembliesResolver), new PluginAssemblyResolver());

            config.MapHttpAttributeRoutes();


            config.Routes.MapHttpRoute(
                name: "content",
                routeTemplate: "modules/{plugin}/{contentArea}/{*path}",
                defaults: new
                {
                    controller = "KodexModuleContent",
                    action = "File"
                });

            appBuilder.UseWebApi(config);
        }
    }
}
