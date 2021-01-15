using phpswitch.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace phpswitch.SubPrograms
{
    /// <summary>
    /// Application class start here after Program.Main();
    /// </summary>
    class App
    {


        protected ConsoleStyle ConsoleStyle;


        protected PHPSwitchConfig MPHPSwitchConfig;


        /// <summary>
        /// Application class constructor.
        /// </summary>
        public App()
        {

        }


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
        private static bool IsAdmin()
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
        private static void RelaunchAsAdmin(string phpversion, FileInfo configJson, bool verbose)
        {
            string argumentString = phpversion;
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
            catch
            {
                throw new Exception("Unable to raise the administrator privilege.");
            }
        }


        /// <summary>
        /// Run the application.
        /// </summary>
        /// <param name="phpversion">The PHP version to switch to.</param>
        /// <param name="configJson">Path to phpswitch.json config file.</param>
        /// <param name="verbose">Verbose value will be true or false.</param>
        public void Run(string phpversion, FileInfo configJson, bool verbose)
        {
            if (IsAdmin() == false)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    RelaunchAsAdmin(phpversion, configJson, verbose);
                }
                else
                {
                    ConsoleStyle.ErrorMessage("Administrator privileges are required!");
                    Environment.Exit(403);
                }
            }

            // display program header.
            ConsoleStyle.ProgramHeader();

            this.MPHPSwitchConfig = new PHPSwitchConfig();
            this.MPHPSwitchConfig.PhpVersion = phpversion;
            this.MPHPSwitchConfig.Verbose = verbose;

            this.ConsoleStyle = new ConsoleStyle(this.MPHPSwitchConfig);

            FileCopier FileCopierClass = new FileCopier(this.MPHPSwitchConfig);

            // validate required folders and files.
            FileCopierClass.ValidateRequired(configJson);

            // services tasks (stop services). -----------------
            Services ServicesClass = new Services(this.MPHPSwitchConfig);
            if (ServicesClass.IsServiceExists())
            {
                this.MPHPSwitchConfig.RunServiceTask = true;
                ServicesClass.StopServices();
            } else
            {
                this.ConsoleStyle.WriteVerbose("--The services are not specify." + Environment.NewLine);
                this.MPHPSwitchConfig.RunServiceTask = false;
            }
            // end services tasks (stop services). -------------

            // remove php-running files and copy from selected version.
            FileCopierClass.StartCopyFiles();

            // services tasks (start services). ----------------
            if (this.MPHPSwitchConfig.RunServiceTask == true)
            {
                ServicesClass.StartServices();
            }
            // end services tasks (start services). ------------

            // success message and exit.
            ConsoleStyle.SuccessExit();
        }


    }
}
