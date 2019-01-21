using System;
using System.IO;
using phpswitch.SubPrograms;

namespace phpswitch.SubPrograms
{
    class FileSystem
    {


        private static Libraries.FileSystem Fs;


        private static WebServer.Apache WsApache;


        private static string phpVersion;


        /**
         * <summary>File system program class constructor.</summary>
         */
        public FileSystem(string phpVersion, string phpDir = "", string apacheDir = "")
        {
            Fs = new Libraries.FileSystem();

            FileSystem.phpVersion = phpVersion;

            if (phpDir != "")
            {
                Fs.PhpDir = phpDir;
            }

            if (apacheDir != "")
            {
                Fs.ApacheDir = apacheDir;
            }
        }


        /**
         * <summary>Validate required folders. Exit the program if not found.</summary>
         */
        public static void ValidateRequiredPath()
        {
            if (Fs.IsPhpRunningFolderExists() == false)
            {
                AppConsole.ErrorMessage("Error! The \"php-running\" folder is not exists in the <PHP versions folder>. (" + Fs.PhpDir + ")");
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(1);
                return;
            }

            if (Fs.IsPhpVersionExists(FileSystem.phpVersion) == false)
            {
                AppConsole.ErrorMessage("Error! The selected PHP version does not exists. (" + FileSystem.phpVersion + ")");
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(1);
                return;
            }

            if (Service.IsServiceExists("apache"))
            {
                WsApache = new WebServer.Apache(FileSystem.phpVersion, Fs.ApacheDir);
                WebServer.Apache.ValidateRequiredPath();
            }
        }


        /**
         * <summary>Remove contents in "php-running" folder and start copy from specified folder into it.</summary>
         */
        public static void StartCopyFiles()
        {
            if (Fs.IsPhpVersionExists(FileSystem.phpVersion) == false || Fs.IsPhpRunningFolderExists() == false)
            {
                return;
            }

            Console.WriteLine("It will be switching to PHP" + FileSystem.phpVersion + ".");
            Console.WriteLine();

            // remove currently running php in "php-running" folder.
            Console.WriteLine("Removing current version of PHP.");
            Fs.DeletePhpRunning();
            Console.WriteLine("Current version of PHP has been removed.");
            Console.WriteLine();

            // starting to copy files and folders.
            // Copied from https://stackoverflow.com/a/3822913/128761 .
            Console.WriteLine("Starting to copy PHP" + FileSystem.phpVersion + ".");
            Fs.CopyFolder(FileSystem.phpVersion);
            Console.WriteLine("Copy complete.");
            Console.WriteLine();

            if (Service.IsServiceExists("apache"))
            {
                WebServer.Apache.WriteConfig();
            }
        }


    }
}
