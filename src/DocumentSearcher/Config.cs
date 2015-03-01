using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSearcher
{

    public class Config
    {
        public IDictionary<string, string> SearchTerms { get; set; }
        public string Document { get; set; }
    }

}
