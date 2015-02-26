using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scratch
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new kodex.apiSoapClient();
            if (c.launch("corupd", "Scott Williams", "SCOTTTEST1", new kodex.ArrayOfString() { "tp1" }))
            {
                //we accepted the launch
                bool isbusy = true;
                while (isbusy)
                {
                    isbusy = c.isbusy("corupd", "Scott Williams");
                }

                var response = c.results("corupd", "Scott Williams");

                Console.WriteLine(response);
            }
        }
    }
}
