using KodexEngine;
using KodexEngine.Host;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DocumentSearcher.Controllers
{
    public class DocumentController : PluginApiController<Dictionary<string, Config>>
    {
        DocumentService documents = new DocumentService();

        MemoryCache _cache = new MemoryCache("processingJobs");
        [HttpGet, PluginRoute("hello")]
        public string Hello()
        {
            return "Hello World";
        }

        [HttpGet, PluginRoute("StartProcessing")]
        public string StartProcessing(string caseRefernece, string documentType)
        {
            var taskId = Guid.NewGuid().ToString();

            Task<Stream> t = Task.Run(async () =>
            {
                var config = this.Config[documentType];

                var pdfPath = documents.GetDocument(caseRefernece, config.Document);

                return await pdf2htmlEX.ConvertToHtml(pdfPath);
            });

            _cache.Add(new CacheItem(taskId, t), new CacheItemPolicy()
            {
                SlidingExpiration = new TimeSpan(0, 1, 0)
            });


            return taskId;
        }


        public bool IsBusy(string taskId)
        {
            var t = _cache.Get(taskId) as Task<Stream>;

            if (t == null || t.IsCompleted)
            {
                return false;
            }

            return true;
        }


    }
}
