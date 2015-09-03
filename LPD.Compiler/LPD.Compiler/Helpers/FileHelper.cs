using System.IO;
using System.Threading.Tasks;

namespace LPD.Compiler.Helpers
{
    public static class FileHelper
    {
        public static async Task<string> GetFileContentAsStringAsync(string path)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}
