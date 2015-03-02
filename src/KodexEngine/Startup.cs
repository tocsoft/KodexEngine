using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

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

            //config.MapHttpAttributeRoutes();




            //config.Routes.MapHttpRoute(
            //    name: "default",
            //    routeTemplate: "modules/{plugin}/api/{controller}/{action}",
            //    defaults: new
            //    {
            //    },
            //    constraints: null,
            //    handler: new PluginHttpControllerDispatcher(config));

            var controllers = PluginLoader.Instance.Plugins
                .SelectMany(x => x.PluginAssembly.GetTypes()
                    .Select(t => new { plugin = x, type = t }))
                .Where(x => typeof(IHttpController).IsAssignableFrom(x.type))
                .Where(x => !x.type.IsAbstract)
                .Where(x => x.type.Name.EndsWith("Controller"));

            foreach (var map in controllers)
            {
                var controllerName = map.type.Name.Substring(0, map.type.Name.Length - "Controller".Length);
                var r = config.Routes.MapHttpRoute(
                    name: map.plugin.Name + "." + map.type.Name,
                    routeTemplate: "modules/" + map.plugin.Name + "/api/" + controllerName + "/{action}",
                    defaults: new { controller = controllerName }
                );

                r.DataTokens["Namespaces"] = new string[] { map.type.Namespace };
            }



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
