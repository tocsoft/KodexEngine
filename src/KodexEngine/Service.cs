using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace KodexEngine
{
    public partial class Service : ServiceBase
    {
        private IDisposable _app;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {

            var service = new Service();

            if (Environment.UserInteractive)
            {
                service.OnStart(args);
                Console.WriteLine("Press any key to stop program");
                Console.Read();
                service.OnStop();
            }
            else
            {
                ServiceBase.Run(service);
            }
        }

        public Service()
        {
            InitializeComponent();
        }


        protected override void OnStart(string[] args)
        {
            var pluginDirectories = args.Where(x => Directory.Exists(x)).Select(x => Path.GetFullPath(x)).ToList();
            if (Directory.Exists("Plugins"))
            {
                pluginDirectories.AddRange(Directory.EnumerateDirectories("Plugins"));
            }

            var plugins = PluginLoader.Instance.Load(pluginDirectories);


            string baseAddress = "http://localhost:9000/";


            _app = WebApp.Start<Startup>(url: baseAddress);


        }

        protected override void OnStop()
        {
            _app.Dispose();
        }
    }
}
