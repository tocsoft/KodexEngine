using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSearcher
{
    public static class pdf2htmlEX
    {
        static string _exePath;
        private static string ExePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_exePath))
                {
                    var ass = typeof(pdf2htmlEX).Assembly;

                    var path = typeof(pdf2htmlEX).Namespace + ".pdf2htmlEX.zip";

                    var folder = Path.Combine(Path.GetTempPath(), "pdf2htmlEX");
                    using (var stream = ass.GetManifestResourceStream(path))
                    using (var z = Ionic.Zip.ZipFile.Read(stream))
                    {
                        z.ExtractAll(folder, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                    }

                    _exePath = Path.Combine(folder, "pdf2htmlEX.exe");
                }

                return _exePath;
            }
        }

    }


}
