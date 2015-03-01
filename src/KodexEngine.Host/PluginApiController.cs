using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace KodexEngine.Host
{
    public class PluginApiController : ApiController
    {
        Plugin _plugin;
        public Plugin Plugin
        {
            get
            {
                return _plugin ?? (_plugin = PluginLoader.Instance[this.GetType().Assembly]);
            }
        }
    }

    public class PluginApiController<TConfig> : PluginApiController
    {
        bool _loaded = false;
        TConfig _config;
        public TConfig Config
        {
            get
            {
                if (!_loaded)
                {
                    _loaded = true;
                    _config = Plugin.Config<TConfig>();
                }
                return _config;
            }
        }
    }
}
