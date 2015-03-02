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
            var client = new kodex.apiSoapClient();

            string payload = "Scott Williams\n" +
                             "SCOTTTEST1\n" +
                             "correspondent updater\n";

            string secret = "thisisthesecretkey";

            var secretOffset = new Random().Next(255);


            string encryptedPayload = "" + (char)secretOffset;
            var i = 0;
            while (i < payload.Length)
            {
                i++;

                var secretI = ((i - 1) + secretOffset) % secret.Length;
                var payLetter = payload.Substring(i - 1, 1)[0];
                var secretLetter = secret.Substring(secretI, 1)[0];

                var encodedchar = ((int)payLetter + (int)secretLetter) % 255;

                encryptedPayload = encryptedPayload + (char)encodedchar;

            }

            var shift = (int)encryptedPayload[0];
            var fixedPayload = encryptedPayload
                .Skip(1)
                .Select((x, j) => (x - secret[(j + shift) % secret.Length]))
                //.Select(x=> x < 0? x+255:x)
                .Select(x => x % 255)
                .Select(x => (char)x)
                .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c)).ToString();

            var name = "Scott Williams - " + Guid.NewGuid().ToString();

            var application = "DocumentSearcher";

            if (client.launch(application, name, "SCOTTTEST1", Newtonsoft.Json.JsonConvert.SerializeObject(new { documentType = "medical_report" })))
            {
                //we accepted the launch
                bool isbusy = true;
                while (isbusy)
                {
                    isbusy = client.isbusy(application, name);
                }

                var response = client.results(application, name);

                Console.WriteLine(response);

                Console.ReadLine();
            }
        }
    }
}
