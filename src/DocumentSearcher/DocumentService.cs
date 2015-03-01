using KodexEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSearcher
{
    public class DocumentService
    {
        public string GetDocument(string caseReference, string field)
        {
            return Path.Combine(PluginLoader.Instance[typeof(DocumentService).Assembly].Directory, "sample.pdf");
        
        }
    }
}
