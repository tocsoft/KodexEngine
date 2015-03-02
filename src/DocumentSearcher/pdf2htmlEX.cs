using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static Task<Stream> ConvertToHtml(string path)
        {
            return Task.Run<Stream>(() =>
            {
                var folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                Directory.CreateDirectory(folder);

                var tempPath = Path.Combine(folder, "document.pdf");
                var htmlPath = Path.Combine(folder, "document.html");

                File.Copy(path, tempPath, true);

                var proc = Process.Start(new ProcessStartInfo(ExePath, BuildCommandLineArgs("document.pdf", "document.html", "--fit-width", "1024"))
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = folder
                });

                var error = proc.StandardError.ReadToEnd();
                var output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();

                return new ProxyStream(File.OpenRead(htmlPath), () =>
                {
                    File.Delete(tempPath);
                    File.Delete(htmlPath);
                });
            });
        }

        private static string BuildCommandLineArgs(params string[] argsList)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (string arg in argsList)
            {
                sb.Append("\"\"" + arg.Replace("\"", @"\" + "\"") + "\"\" ");
            }

            if (sb.Length > 0)
            {
                sb = sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        private class ProxyStream : Stream
        {
            private Stream _stream;
            private Action _disposed;
            public ProxyStream(Stream stream, Action disposed)
            {
                _stream = stream;
                _disposed = disposed;
            }
            public override bool CanRead
            {
                get { return _stream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return _stream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return _stream.CanWrite; }
            }

            public override void Flush()
            {
                _stream.Flush();
            }

            public override long Length
            {
                get { return _stream.Length; }
            }

            public override long Position
            {
                get
                {
                    return _stream.Position;
                }
                set
                {
                    _stream.Position = value;
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _stream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _stream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _stream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _stream.Write(buffer, offset, count);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _stream.Dispose();
                }
                _disposed();
                base.Dispose(disposing);
            }
        }
    }
}
