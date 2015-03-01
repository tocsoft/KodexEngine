using KodexEngine;
using KodexEngine.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DocumentSearcher.Controllers
{
    public class DocumentController : PluginApiController<Dictionary<string, Config>>
    {
        DocumentService documents = new DocumentService();

        Dictionary<string, Task<string>> _tasks = new Dictionary<string, Task<string>>();

        [HttpGet, PluginRoute("hello")]
        public string Hello()
        {
            return "Hello World";
        }

        [HttpGet, PluginRoute("StartProcessing")]
        public string StartProcessing(string caseRefernece, string configName)
        {
            var taskId = Guid.NewGuid().ToString();

            Task<string> t = Task.Run(() =>
            {
                var config = this.Config[configName];

                var pdfPath = documents.GetDocument(caseRefernece, config.Document);



                return "";

            });

            _tasks.Add(taskId, t);


            return taskId;
        }
    }
}
