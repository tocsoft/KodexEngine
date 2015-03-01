using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodexLocal.Soap
{

    //Define the Web Services AppHost
    public class AppHost : AppSelfHostBase
    {
        public AppHost()
            : base("HttpListener Self-Host", typeof(AppHost).Assembly) { }

        public override void Configure(Funq.Container container) { }
    }
}
