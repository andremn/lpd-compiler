using Microsoft.Win32;
using System.Linq;

namespace LPD.Compiler.Helpers
{
    public static class RegistryHelper
    {
        public static string GetProgramInstallationPath(string programName)
        {
            string displayName;
            string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);

            if (key != null)
            {
                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;

                    if (displayName != null && displayName.Contains(programName))
                    {
                        return subkey.GetValue("InstallLocation") as string;
                    }
                }

                key.Close();
            }

            registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            key = Registry.LocalMachine.OpenSubKey(registryKey);

            if (key != null)
            {
                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;

                    if (displayName != null && displayName.Contains(programName))
                    {
                        return subkey.GetValue("InstallLocation") as string;
                    }
                }

                key.Close();
            }

            return null;
        }
    }
}
