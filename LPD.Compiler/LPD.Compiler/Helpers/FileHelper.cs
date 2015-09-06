using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LPD.Compiler.Helpers
{
    public static class FileHelper
    {
        public static async Task<string> GetFileContentAsStringAsync(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static async Task SaveFileStringContentsAsync(string path, string contents)
        {
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                await writer.WriteAsync(contents);
                await writer.FlushAsync();
            }
        }

        public static XmlTextReader GetSyntaxHighlighting()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Properties\LPDSyntaxHighlighting.xshd");
            
            return new XmlTextReader(path);
        }
    }
}
