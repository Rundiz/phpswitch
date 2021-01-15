using phpswitch.Libraries;
using phpswitch.Models;
using System;
using System.IO;

namespace phpswitch.SubPrograms.WebServers
{
    /// <summary>
    /// Class that do the tasks about Apache web server.
    /// </summary>
    class Apache
    {


        protected ConsoleStyle ConsoleStyle;


        protected FileSystem LibFS;


        protected PHPSwitchConfig MPHPSwitchConfig;


        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="PHPSwitchConfigClass">The PHPSwitchConfig class.</param>
        public Apache(PHPSwitchConfig PHPSwitchConfigClass)
        {
            this.MPHPSwitchConfig = PHPSwitchConfigClass;
            this.ConsoleStyle = new ConsoleStyle(this.MPHPSwitchConfig);
            this.LibFS = new FileSystem(this.MPHPSwitchConfig);
        }


        /// <summary>
        /// Validate required things such as folders, files.
        /// Write the error and exit program if failed to validate.
        /// </summary>
        public void ValidateRequired()
        {
            // check for apache required folders and files. ---------------------------
            this.ConsoleStyle.WriteVerbose("--Checking Apache directories and files.");

            // check Apache directory.
            if (
                String.IsNullOrEmpty(this.MPHPSwitchConfig.PHPSwitchJSO.apacheDir) ||
                Directory.Exists(this.MPHPSwitchConfig.PHPSwitchJSO.apacheDir) == false
            )
            {
                ConsoleStyle.ErrorExit("You did not specify `apacheDir` property in json file or the directory you specified does not exists. (" + this.MPHPSwitchConfig.PHPSwitchJSO.apacheDir + ")");
            } else
            {
                this.ConsoleStyle.WriteVerbose("  --apacheDir: {0} OK.", this.MPHPSwitchConfig.PHPSwitchJSO.apacheDir);
            }

            // check Apache /conf/extra directory.
            if (
                Directory.Exists(FileSystem.NormalizePath(this.MPHPSwitchConfig.PHPSwitchJSO.apacheDir + "/conf/extra")) == false
            )
            {
                ConsoleStyle.ErrorExit("The Apache config directory does not exists. (" + FileSystem.NormalizePath(this.MPHPSwitchConfig.PHPSwitchJSO.apacheDir + "/conf/extra") + ")");
            } else
            {
                this.ConsoleStyle.WriteVerbose(
                    "  --Apache config directory: {0} OK.", 
                    FileSystem.NormalizePath(this.MPHPSwitchConfig.PHPSwitchJSO.apacheDir + "/conf/extra")
                );
            }

            // check Apache config files for PHP must exists.
            if (
                File.Exists(FileSystem.NormalizePath(this.MPHPSwitchConfig.PHPSwitchJSO.apacheDir + "/conf/extra/httpd-php.conf")) == false ||
                File.Exists(FileSystem.NormalizePath(this.MPHPSwitchConfig.PHPSwitchJSO.apacheDir + "/conf/extra/httpd-php-" + this.MPHPSwitchConfig.PhpVersion + ".conf")) == false
            )
            {
                ConsoleStyle.ErrorExit("The Apache config files for specific PHP version is not exists.\n" +
                    "  " + FileSystem.NormalizePath(this.MPHPSwitchConfig.PHPSwitchJSO.apacheDir + "/conf/extra/httpd-php.conf") + "\n" +
                    "  " + FileSystem.NormalizePath(this.MPHPSwitchConfig.PHPSwitchJSO.apacheDir + "/conf/extra/httpd-php-" + this.MPHPSwitchConfig.PhpVersion + ".conf"));
            } else
            {
                this.ConsoleStyle.WriteVerbose(
                    "  --Apache config files:\n" +
                    "    --{0} OK.\n" +
                    "    --{1} OK.",
                    FileSystem.NormalizePath(this.MPHPSwitchConfig.PHPSwitchJSO.apacheDir + "/conf/extra/httpd-php.conf"),
                    FileSystem.NormalizePath(this.MPHPSwitchConfig.PHPSwitchJSO.apacheDir + "/conf/extra/httpd-php-" + this.MPHPSwitchConfig.PhpVersion + ".conf")
                );
            }
            
            this.ConsoleStyle.WriteVerbose("--Finish checking Apache directories and files.");
            // end check for apache required folders and files. -----------------------
        }



        /// <summary>
        /// Write config to Apache file if JSON config `apacheUpdateConfig` was set to `true`.
        /// </summary>
        public void WriteConfig()
        {
            if (this.MPHPSwitchConfig.PHPSwitchJSO.apacheUpdateConfig == false)
            {
                return;
            }

            Console.WriteLine("Changing PHP version in Apache.");

            string configPath = FileSystem.NormalizePath(this.MPHPSwitchConfig.PHPSwitchJSO.apacheDir + "/conf/extra/httpd-php.conf");
            string configPhpVersion = "Include conf/extra/httpd-php-" + this.MPHPSwitchConfig.PhpVersion+ ".conf" + Environment.NewLine;

            File.WriteAllText(@configPath, configPhpVersion);
            this.ConsoleStyle.WriteVerbose(
                "  --The configuration in a file {0} was changed to \"{1}\"",
                configPath,
                configPhpVersion.Trim()
            );
            Console.WriteLine("  PHP version in Apache config file has been changed. ({0})", configPath);
            Console.WriteLine();
        }


    }
}
