using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LPD.Compiler.Helpers
{
    public static class FileHelper
    {
        private const string OutputDirectoryName = "output";
        private const string SyntaxHighlightFilePath = @"Properties\LPDSyntaxHighlighting.xshd";

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
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SyntaxHighlightFilePath);
            
            return new XmlTextReader(path);
        }

        public static string GetOutputFilePath(string fileName)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var dirPath = Path.Combine(currentDir, OutputDirectoryName);

            if (!Directory.Exists(dirPath))
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (SystemException)
                {
                    dirPath = Path.GetTempPath();
                }
            }

            return Path.Combine(dirPath, fileName);
        }
    }
}
