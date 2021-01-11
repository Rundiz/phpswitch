using System;
using System.IO;
using phpswitch.SubPrograms;

namespace phpswitch.SubPrograms
{
    class FileCopier
    {


        private Libraries.FileSystem LibFs;


        private Models.PhpSwitchConfig MPhpSwitchConfig;


        private WebServer.Apache WsApache;


        /**
         * <summary>File copier program class constructor.</summary>
         */
        public FileCopier(Models.PhpSwitchConfig PhpSwitchConfig)
        {
            LibFs = new Libraries.FileSystem(PhpSwitchConfig);
            WsApache = new WebServer.Apache(PhpSwitchConfig);

            MPhpSwitchConfig = PhpSwitchConfig;
        }


        /**
         * <summary>Validate required folders. Exit the program if not found.</summary>
         */
        public void ValidateRequiredPath()
        {
            // validate that selected php folder is existing. -------------
            if (String.IsNullOrEmpty(MPhpSwitchConfig.phpDir) == true)
            {
                // if phpDir config value is empty.
                // set it based from running directory.
                MPhpSwitchConfig.phpDir = MPhpSwitchConfig.runningDir + Path.DirectorySeparatorChar + "php";
            }

            if (String.IsNullOrEmpty(MPhpSwitchConfig.phpDir) == false)
            {
                string lastPhpDirChar = MPhpSwitchConfig.phpDir.Substring(MPhpSwitchConfig.phpDir.Length - 1);
                if (lastPhpDirChar == "\"")
                {
                    AppConsole.ErrorMessage("Do not enter <PHP versions folder> with trailing slash.");
                    System.Threading.Thread.Sleep(5000);
                    Environment.Exit(1);
                    return;
                }

                if (Directory.Exists(MPhpSwitchConfig.phpDir) == false)
                {
                    AppConsole.ErrorMessage("The <PHP versions folder> is not exists. (" + MPhpSwitchConfig.phpDir + ")");
                    System.Threading.Thread.Sleep(5000);
                    Environment.Exit(1);
                    return;
                }
            }
            // end validate that selected php folder is existing. ---------

            if (LibFs.IsPhpRunningFolderExists() == false)
            {
                AppConsole.ErrorMessage("Error! The \"php-running\" folder is not exists in the <PHP versions folder>. (" + MPhpSwitchConfig.phpDir + ")");
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(1);
                return;
            }

            if (LibFs.IsPhpVersionExists(MPhpSwitchConfig.phpVersion) == false)
            {
                AppConsole.ErrorMessage("Error! The selected PHP version does not exists. (" + MPhpSwitchConfig.phpDir + Path.DirectorySeparatorChar + "php" + MPhpSwitchConfig.phpVersion + ")");
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(1);
                return;
            }
            

            if (MPhpSwitchConfig.apacheUpdateConfig == true)
            {
                WsApache.ValidateRequiredPath();
            }
        }


        /**
         * <summary>Remove contents in "php-running" folder and start copy from specified folder into it.</summary>
         */
        public void StartCopyFiles()
        {
            Console.WriteLine("It will be switching to PHP" + MPhpSwitchConfig.phpVersion + ".");

            // remove currently running php in "php-running" folder.
            Console.WriteLine("  Removing current version of PHP.");
            LibFs.DeletePHPRunning();
            Console.WriteLine("  Current version of PHP has been removed.");

            // starting to copy files and folders.
            // Copied from https://stackoverflow.com/a/3822913/128761 .
            Console.WriteLine("  Starting to copy PHP" + MPhpSwitchConfig.phpVersion + ".");
            LibFs.CopyPHPFolder(MPhpSwitchConfig.phpVersion);
            Console.WriteLine("  Copy complete.");
            Console.WriteLine();

            if (MPhpSwitchConfig.apacheUpdateConfig == true)
            {
                WsApache.WriteConfig();
            }

            if (MPhpSwitchConfig.additionalCopy != null)
            {
                AdditionalCopier AdditionalCopierClass = new AdditionalCopier(MPhpSwitchConfig);

                AdditionalCopierClass.StartCopyFiles();
            }

        }


    }
}
