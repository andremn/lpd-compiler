using System.Diagnostics;

namespace LPD.Compiler.Helpers
{
    public static class ProcessHelper
    {
        public static void StartProcess(string exePath, string arguments)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo(exePath, arguments)
            };
            
            process.Start();
        }
    }
}
