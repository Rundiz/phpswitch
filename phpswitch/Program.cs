using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using phpswitch.SubPrograms;

namespace phpswitch
{
    class Program
    {


        /**
         * <summary>Detect help argument.</summary>
         */
        private static bool HelpRequired(string param)
        {
            return param == "" || param == "-h" || param == "--help" || param == "/?";
        }


        /**
         * <summary>Check if app is running as admin.</summary>
         * <remarks>Copied from https://stackoverflow.com/questions/2818179/how-do-i-force-my-net-application-to-run-as-administrator/50186997#50186997 .</remarks>
         */
        private static bool IsRunAsAdmin()
        {
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            } catch
            {
                return false;
            }
        }


        public static void Main(string[] args)
        {
            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
            if (args.Length == 0 || (args.Length >= 1 && HelpRequired(args[0])))
            {
                // if displaying help.
                AppConsole.DisplayHelp();
            } else
            {
                // if running the code.
                string runningDir = Directory.GetCurrentDirectory();// directory where this application is running from.
                string phpDir = "";
                string apacheDir = "";
                if (args.Length >= 2)
                {
                    // if php versions folder was specified.
                    // set php folder path.
                    phpDir = args[1];
                }
                if (args.Length >= 3)
                {
                    // if apache folder was specified.
                    // set apache folder path.
                    apacheDir = args[2];
                }
                FileSystem Fs = new FileSystem(args[0], phpDir, apacheDir);

                // check that this is running as admin.
                if (IsRunAsAdmin() == false)
                {
                    // if not run as admin.
                    try
                    {
                        RelaunchAsAdmin();
                    } catch
                    {
                        AppConsole.ErrorMessage("Please run this command as administrator privilege.");
                        System.Threading.Thread.Sleep(5000);
                        Environment.Exit(1);
                    }
                }
                // end check if running as admin.

                // validate required folders.
                Fs.ValidateRequiredPath();

                // stop web server service.
                Service.StopWebServerService();

                // copy files (also remove running version before copy).
                FileSystem.StartCopyFiles();

                // start web server service again.
                Service.StartWebServerService();

                // success message and exit.
                AppConsole.SuccessExit();
            }
        }


        /**
         * <summary>Raise administrator privilege.</summary>
         * <remarks>Copied from https://stackoverflow.com/a/46080075/128761 .</remarks>
         */
        private static void RelaunchAsAdmin()
        {
            // get current command arguments.
            // Copied from https://docs.microsoft.com/en-us/dotnet/api/system.environment.getcommandlineargs?view=netframework-4.7.2 .
            String[] arguments = Environment.GetCommandLineArgs();
            // remove first key which is the exe itself.
            // Copied from https://social.msdn.microsoft.com/Forums/vstudio/en-US/77ed73c3-fb56-414a-bacf-0cdcd41afff4/remove-element-from-an-array-of-string?forum=csharpgeneral#4242f96f-74b1-4422-bb86-f4b98ee91154
            arguments = arguments.Where(w => w != arguments[0]).ToArray();
            // loop wrap second or more arguments with double quote. "..."
            // Copied from https://stackoverflow.com/a/60089/128761 .
            for (int key = 0; key < arguments.Length; ++key)
            {
                if (key >= 1)
                {
                    // if argument contain path (key >= 1)
                    // wrap with ".."
                    arguments[key] = '"' + arguments[key] + '"';
                }
            }
            string argumentString = String.Join(" ", arguments);

            ProcessStartInfo Proc = new ProcessStartInfo();
            Proc.Arguments = argumentString;
            Proc.FileName = Assembly.GetEntryAssembly().CodeBase;
            Proc.UseShellExecute = true;
            Proc.Verb = "runas";
            Proc.WorkingDirectory = Environment.CurrentDirectory;

            try
            {
                Process.Start(Proc);
                Environment.Exit(0);
            } catch
            {
                throw new Exception("Unable to raise the administrator privilege.");
            }
        }
    }
}
