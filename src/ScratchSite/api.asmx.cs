using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace ScratchSite
{
    /// <summary>
    /// Summary description for api
    /// </summary>
    [WebService(Namespace = "http://kodexengine.client/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class api : System.Web.Services.WebService
    {

        [WebMethod]
        public bool launch(string app, string user, string caseref, string[] args)
        {
            return true;
        }

        [WebMethod]
        public bool isbusy(string app, string user)
        {
            return true;
        }

        [WebMethod]
        public string results(string app, string user)
        {
            return "";
        }
    }
}
