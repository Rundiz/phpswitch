using System;
using System.IO;
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


        public static void Main(string[] args)
        {
            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
            if (args.Length == 0 || (args.Length >= 1 && HelpRequired(args[0])))
            {
                // if displaying help.
                AppConsole.DisplayHelp();
            } else
            {
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
                // if running the code.
                FileSystem Fs = new FileSystem(args[0], phpDir, apacheDir);

                // validate required folders.
                FileSystem.ValidateRequiredPath();

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
    }
}
