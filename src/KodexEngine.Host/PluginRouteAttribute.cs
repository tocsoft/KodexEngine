using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;

namespace KodexEngine
{


    /// <summary>
    /// Place on a controller or action to expose it directly via a route. 
    /// When placed on a controller, it applies to actions that do not have any <see cref="RouteAttribute"/>s on them.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class PluginRouteAttribute : Attribute, IDirectRouteFactory, IHttpRouteInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteAttribute" /> class.
        /// </summary>
        public PluginRouteAttribute()
        {
            Template = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteAttribute" /> class.
        /// </summary>
        /// <param name="template">The route template describing the URI pattern to match against.</param>
        public PluginRouteAttribute(string template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }
            Template = template;
        }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public int Order { get; set; }

        /// <inheritdoc />
        public string Template { get; private set; }

        RouteEntry IDirectRouteFactory.CreateRoute(DirectRouteFactoryContext context)
        {
            var tmplt = "api" + "/" + Template;

            var assembly = context.Actions.FirstOrDefault().ControllerDescriptor.ControllerType.Assembly;
            var plugin = PluginLoader.Instance[assembly];
            if (plugin == null)
            {
                throw new Exception("has to have a matching plugin");
            }

            IDirectRouteBuilder builder = context.CreateBuilder("modules/" + plugin.Name + "/" + tmplt);

            builder.Name = Name;
            builder.Order = Order;
            return builder.Build();
        }
    }

    /// <summary>
    /// Annotates a controller with a route prefix that applies to actions that have any <see cref="RouteAttribute"/>s on them.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute is intended to be extended by the user.")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PluginRoutePrefixAttribute : Attribute, IRoutePrefix
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutePrefixAttribute" /> class without specifying any parameters.
        /// </summary>
        protected PluginRoutePrefixAttribute()
        {
        }

        /// <summary>
        /// Gets the route prefix.
        /// </summary>
        public virtual string Prefix { get; private set; }
    }
}
