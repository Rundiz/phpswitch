using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace sharedLibraries
{
    /// <summary>
    /// Windows privilege user class.
    /// </summary>
    public class Privilege
    {


        /// <summary>
        /// Get user ID on Linux.
        /// For admin, check if `getuid() == 0`.
        /// https://stackoverflow.com/a/58567692/128761 Original source code.
        /// </summary>
        /// <returns></returns>
        [DllImport("libc")]
        public static extern uint getuid();


        /// <summary>
        /// Check if running as administrator
        /// https://stackoverflow.com/a/58567692/128761 Original source code.
        /// </summary>
        /// <returns>Return true if yes, false if no.</returns>
        public static bool IsAdmin()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // if Windows.
                    WindowsIdentity user = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(user);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
                else
                {
                    // if not Windows.
                    return getuid() == 0;
                }
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Re-launch as admin.
        /// https://stackoverflow.com/a/46080075/128761 Original source code.
        /// </summary>
        /// <param name="phpversion">The PHP version to switch to.</param>
        /// <param name="configJson">Path to phpswitch.json config file.</param>
        /// <param name="verbose">Verbose value will be true or false.</param>
        public static void RelaunchAsAdmin(string phpversion = "", FileInfo configJson = null, bool verbose = false)
        {
            string argumentString = "";
            if (String.IsNullOrEmpty(phpversion) == false)
            {
                argumentString += phpversion;
            }
            if (String.IsNullOrEmpty(configJson?.FullName) == false)
            {
                argumentString += " --config-json=\"" + configJson + "\"";
            }
            if (verbose == true)
            {
                argumentString += " --verbose";
            }

            ProcessStartInfo Proc = new ProcessStartInfo();
            Proc.Arguments = argumentString;
            // https://stackoverflow.com/a/980225/128761 Get executing app .exe original source code.
            Proc.FileName = Process.GetCurrentProcess().MainModule.FileName;
            Proc.UseShellExecute = true;
            Proc.Verb = "runas";
            Proc.WorkingDirectory = Environment.CurrentDirectory;

            try
            {
                Process.Start(Proc);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to raise the administrator privilege.");
            }
        }


    }
}
