using KodexEngine;
using KodexEngine.Host;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DocumentSearcher.Controllers
{
    public class DocumentController : PluginApiController<Dictionary<string, Config>>
    {
        DocumentService documents = new DocumentService();

        static MemoryCache _cache = new MemoryCache("processingJobs");
        [HttpGet]
        public string Hello()
        {
            return "Hello World";
        }

        [HttpGet]
        public string StartProcessing(string caseReference, string documentType)
        {
            var taskId = Guid.NewGuid().ToString();

            Task<Stream> t = Task.Run(async () =>
            {
                var config = this.Config[documentType];

                var pdfPath = documents.GetDocument(caseReference, config.Document);

                return await pdf2htmlEX.ConvertToHtml(pdfPath);
            });

            _cache.Add(new CacheItem(taskId, t), new CacheItemPolicy()
            {
                SlidingExpiration = new TimeSpan(0, 10, 0)
            });


            return taskId;
        }

        [HttpGet]
        public bool IsProcessing(string taskId)
        {
            var t = _cache.Get(taskId) as Task<Stream>;

            if (t != null && t.IsCompleted)
            {
                return false;
            }

            return true;
        }

        [HttpGet]
        public HttpResponseMessage Load(string taskId)
        {
            var t = _cache.Get(taskId) as Task<Stream>;

            if (t == null || t.IsCompleted)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

                var ms = new MemoryStream();
                t.Result.CopyTo(ms);
                ms.Position = 0;
                result.Content = new StreamContent(ms);
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("text/html");
                return result;
            }

            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }


    }
}
