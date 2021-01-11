using System;
using System.IO;
using phpswitch.SubPrograms;

namespace phpswitch.SubPrograms.WebServer
{
    class Apache
    {


        private Models.PhpSwitchConfig MPhpSwitchConfig;


        private static Libraries.FileSystem LibFs;


        public Apache(Models.PhpSwitchConfig PhpSwitchConfig)
        {
            LibFs = new Libraries.FileSystem(PhpSwitchConfig);

            MPhpSwitchConfig = PhpSwitchConfig;
        }


        /**
         * <summary>Validate required folders. Exit the program if not found.</summary>
         */
        public void ValidateRequiredPath()
        {
            // validate that selected apache folder is existing. -------------------------
            if (String.IsNullOrEmpty(MPhpSwitchConfig.apacheDir) == true)
            {
                // if apacheDir config value is empty.
                // set it based from running directory.
                MPhpSwitchConfig.apacheDir = MPhpSwitchConfig.runningDir + Path.DirectorySeparatorChar + "apache";
            }

            if (String.IsNullOrEmpty(MPhpSwitchConfig.apacheDir) == false)
            {
                string lastApacheDirChar = MPhpSwitchConfig.apacheDir.Substring(MPhpSwitchConfig.apacheDir.Length - 1);
                if (lastApacheDirChar == "\"")
                {
                    AppConsole.ErrorMessage("Do not enter Apache folder with trailing slash.");
                    System.Threading.Thread.Sleep(5000);
                    Environment.Exit(1);
                    return;
                }

                if (Directory.Exists(MPhpSwitchConfig.apacheDir) == false)
                {
                    AppConsole.ErrorMessage("The Apache folder is not exists. (" + MPhpSwitchConfig.apacheDir + ")");
                    System.Threading.Thread.Sleep(5000);
                    Environment.Exit(1);
                    return;
                }
            }
            // end validate that selected apache folder is existing. ---------------------

            if (LibFs.IsApacheFolderExists() == false)
            {
                AppConsole.ErrorMessage("Error! The Apache required folders such as 'conf' is not exists. (" + MPhpSwitchConfig.apacheDir + ")");
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(1);
                return;
            }

            if (LibFs.IsApacheConfigFileExists(MPhpSwitchConfig.phpVersion) == false)
            {
                AppConsole.ErrorMessage("Error! The Apache config file for specific PHP version is not exists. (conf/extra/httpd-php-" + MPhpSwitchConfig.phpVersion + ".conf)");
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(1);
                return;
            }
        }


        /**
         * <summary>Write Apache config file.</summary>
         */
        public void WriteConfig()
        {
            Console.WriteLine("Changing PHP version in Apache.");

            string configPath = LibFs.NormalizePath(MPhpSwitchConfig.apacheDir + "/conf/extra/httpd-php.conf");
            string configPhpVersion = "Include conf/extra/httpd-php-" + MPhpSwitchConfig.phpVersion + ".conf" + Environment.NewLine;

            File.WriteAllText(@configPath, configPhpVersion);
            Console.WriteLine("  PHP version in Apache config file has been changed. (" + configPath + ")");
            Console.WriteLine();
        }


    }
}
