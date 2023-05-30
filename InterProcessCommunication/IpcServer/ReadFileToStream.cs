using System.IO;

namespace IpcServer
{
    /// <summary>
    /// Contains the method executed in the context of the impersonated user
    /// </summary>
    public class ReadFileToStream
    {
        private readonly string fn;
        private readonly StreamString ss;

        public ReadFileToStream(StreamString str, string filename)
        {
            fn = filename;
            ss = str;
        }

        public void Start()
        {
            string contents = File.ReadAllText(fn);
            ss.WriteString(contents);
        }
    }
}
