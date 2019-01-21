using System;
using System.IO;
using phpswitch.SubPrograms;

namespace phpswitch.SubPrograms.WebServer
{
    class Apache
    {


        private static Libraries.FileSystem Fs;


        private static string phpVersion;


        public Apache(string phpVersion, string apacheDir = "")
        {
            Fs = new Libraries.FileSystem();

            Apache.phpVersion = phpVersion;

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
            if (Fs.IsApacheFolderExists() == false)
            {
                AppConsole.ErrorMessage("Error! The Apache config folder or required folders, file for Apache configuration is not exists. (" + Fs.ApacheDir + ")");
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(1);
                return;
            }

            if (Fs.IsApacheConfigFileExists(Apache.phpVersion) == false)
            {
                AppConsole.ErrorMessage("Error! The Apache config file for specific PHP version is not exists. (conf/extra/httpd-php-" + Apache.phpVersion + ".conf)");
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(1);
                return;
            }
        }


        /**
         * <summary>Write Apache config file.</summary>
         */
        public static void WriteConfig()
        {
            Console.WriteLine("Changing PHP version in Apache.");

            string configPath = Fs.NormalizePath(Fs.ApacheDir + "/conf/extra/httpd-php.conf");
            string configPhpVersion = "Include conf/extra/httpd-php-" + Apache.phpVersion + ".conf" + Environment.NewLine;

            File.WriteAllText(@configPath, configPhpVersion);
            Console.WriteLine("PHP version in Apache config file has been changed. (" + configPath + ")");
            Console.WriteLine();
        }


    }
}
