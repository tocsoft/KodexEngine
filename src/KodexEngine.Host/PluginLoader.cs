using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KodexEngine
{
    public class PluginLoader
    {
        private List<Plugin> _plugins;
        public static PluginLoader Instance { get; internal set; }

        static PluginLoader()
        {
            Instance = new PluginLoader();
        }

        public PluginLoader()
        {
            _plugins = new List<Plugin>();
        }


        public IEnumerable<Plugin> Load(IEnumerable<string> directories)
        {
            foreach (var d in directories)
            {
                var p = Plugin.Load(d, this);
                if (p != null)
                {
                    _plugins.Add(p);
                }
            }

            return _plugins;
        }

        public IEnumerable<Plugin> Plugins { get { return _plugins; } }
        public Plugin this[string key]
        {
            get
            {
                return _plugins.SingleOrDefault(x => x.Name.ToUpperInvariant() == key.ToUpperInvariant());
            }
        }
        public Plugin this[Assembly key]
        {
            get
            {
                return _plugins.SingleOrDefault(x => x.PluginAssembly == key);
            }
        }
    }

    public class Plugin
    {
        private pluginManifest _manifest;
        internal static Plugin Load(string dir, PluginLoader loader)
        {
            var jsonPath = Path.Combine(dir, "plugin.json");
            if (File.Exists(jsonPath))
            {
                var config = Newtonsoft.Json.JsonConvert.DeserializeObject<pluginManifest>(File.ReadAllText(jsonPath));
                var plugin = loader[config.name];
                if (plugin == null) //ensure we haven't already loaded it up
                {
                    var dllPath = Path.Combine(dir, config.main);
                    if (File.Exists(dllPath))
                    {
                        plugin = new Plugin(config, Path.GetFullPath(dir), Assembly.LoadFrom(dllPath));

                        AppDomain.CurrentDomain.SetupInformation.PrivateBinPath += "," + dir;
                    }
                }

                return plugin;
            }
            return null;
        }

        private class pluginManifest
        {
            public string name { get; set; }
            public string description { get; set; }
            public string main { get; set; }
            public string content { get; set; }
        }

        private Plugin(pluginManifest manifest, string directory, Assembly pluginAssembly)
        {
            _manifest = manifest;
            Directory = directory;
            PluginAssembly = pluginAssembly;
        }
        public string Name { get { return _manifest.name; } }
        public string Description { get { return _manifest.description; } }

        public string Directory { get; private set; }

        public string ContentPath { get { return _manifest.content; } }

        public Assembly PluginAssembly { get; private set; }

        JObject _config = null;
        Dictionary<Type, object> _configCache = new Dictionary<Type, object>();
        public TConfig Config<TConfig>()
        {
            Type configType = typeof(TConfig);
            if (!_configCache.ContainsKey(configType))
            {
                lock (_configCache)
                {
                    if (!_configCache.ContainsKey(configType))
                    {
                        TConfig result = default(TConfig);
                        var configPath = Path.Combine(Directory, "config.json");
                        if (File.Exists(configPath))
                        {
                            var json = File.ReadAllText(configPath);
                            result = Newtonsoft.Json.JsonConvert.DeserializeObject<TConfig>(json);
                        }

                        _configCache.Add(configType, result);
                    }
                }
            }

            return (TConfig)_configCache[configType];
        }

        public TConfig UpdateConfig<TConfig>(TConfig settings)
        {
            object settingsAsObject = settings as object;

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(settingsAsObject ?? new object(), Newtonsoft.Json.Formatting.Indented);

            var configPath = Path.Combine(Directory, "config.json");

            File.WriteAllText(configPath, json);

            return settings;
        }

        public PluginContentFile LoadContentFile(string path, ContentAreas area)
        {
            return PluginContentFile.Load(path, this, area);
        }

        public enum ContentAreas
        {
            Client = 0,
            Admin = 1
        }
    }

    public class PluginContentFile
    {
        public static PluginContentFile Load(string path, Plugin plugin, KodexEngine.Plugin.ContentAreas area)
        {
            var folders = new[]{
                area.ToString(),
                "Shared"
            };

            foreach (var part in folders)
            {

                //lets check r
                var diskPath = Path.Combine(plugin.Directory, plugin.ContentPath, part, path);


                Func<Stream> open = null;

                if (File.Exists(diskPath))
                {
                    open = () => File.OpenRead(path);
                }
                else
                {
                    var resources = plugin.PluginAssembly.GetManifestResourceNames();
                    var resourcePath = Path.Combine(plugin.Name, plugin.ContentPath, part, path).Replace(Path.DirectorySeparatorChar, '.');

                    var resName = resources.SingleOrDefault(x => x.ToUpperInvariant() == resourcePath.ToUpperInvariant());
                    if (!string.IsNullOrWhiteSpace(resName))
                    {
                        open = () =>
                        {
                            return plugin.PluginAssembly.GetManifestResourceStream(resName);
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
                var filename = Path.GetFileName(path);

                return new PluginContentFile()
                {
                    FileName = filename,
                    _open = open,
                    Extension = Path.GetExtension(filename)
                };
            }
            return null;
        }

        private static string CommonPrefix(string[] ss)
        {
            if (ss.Length == 0)
            {
                return "";
            }

            if (ss.Length == 1)
            {
                return ss[0];
            }

            int prefixLength = 0;

            foreach (char c in ss[0])
            {
                foreach (string s in ss)
                {
                    if (s.Length <= prefixLength || s[prefixLength] != c)
                    {
                        return ss[0].Substring(0, prefixLength);
                    }
                }
                prefixLength++;
            }

            return ss[0]; // all strings identical
        }


        public string FileName { get; private set; }
        public string Extension { get; private set; }

        private Func<Stream> _open;
        public Stream OpenStream()
        {
            return _open();
        }
    }
}
