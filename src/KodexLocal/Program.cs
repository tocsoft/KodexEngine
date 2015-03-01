using KodexLocal.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KodexLocal
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var appHost = new AppHost()
                .Init()
                .Start("http://127.0.0.1:15535/");


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
