using phpswitch.Libraries;
using phpswitch.Models;
using System;
using System.IO;

namespace phpswitch.SubPrograms
{
    /// <summary>
    /// File copier.
    /// </summary>
    class FileCopier
    {


        protected ConsoleStyle ConsoleStyle;


        protected FileSystem LibFS;


        protected PHPSwitchConfig MPHPSwitchConfig;


        protected PHPSwitchJSO MPHPSwitchJSO;


        protected WebServers.Apache WSApache;


        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="PHPSwitchConfigClass">The PHPSwitchConfig class.</param>
        public FileCopier(PHPSwitchConfig PHPSwitchConfigClass)
        {
            this.MPHPSwitchConfig = PHPSwitchConfigClass;
            this.LibFS = new FileSystem(this.MPHPSwitchConfig);
            this.WSApache = new WebServers.Apache(this.MPHPSwitchConfig);
            this.ConsoleStyle = new ConsoleStyle(this.MPHPSwitchConfig);
        }


        /// <summary>
        /// Load JSON config file and set to PHPSwitchJSO (as reference), PHPSwitchConfig.
        /// </summary>
        /// <param name="filePath">Full path to .json file.</param>
        protected void LoadJSONConfigFile(string filePath)
        {
            this.MPHPSwitchJSO = this.LibFS.LoadJSON(filePath);
            this.MPHPSwitchConfig.PHPSwitchJSO = this.MPHPSwitchJSO;
        }


        /// <summary>
        /// <para>* Remove contents in "php-running" folder and start copy from selected version into it.</para>
        /// <para>* Re-write Apache config file if JSON config `apacheUpdateConfig` was set to `true`.</para>
        /// <para>* Copy additional files and or directories to selected target in JSON config `additionalCopy` property.</para>
        /// </summary>
        public void StartCopyFiles()
        {
            Console.WriteLine("It will be switching to PHP" + this.MPHPSwitchConfig.PhpVersion + ".");

            // remove currently running php in "php-running" folder.
            Console.WriteLine("  Removing current version of PHP.");
            this.LibFS.DeletePHPRunning();
            this.ConsoleStyle.WriteVerbose("  --Delete everything in {0}.", this.MPHPSwitchConfig.PHPSwitchJSO.phpRunningDir);
            Console.WriteLine("  Current version of PHP has been removed.");

            // starting to copy files and folders.
            Console.WriteLine("  Starting to copy PHP" + this.MPHPSwitchConfig.PhpVersion + ".");
            this.LibFS.CopyPHPFolder(this.MPHPSwitchConfig.PhpVersion);
            this.ConsoleStyle.WriteVerbose(
                " --Copy everything from {0} to {1}.",
                this.MPHPSwitchConfig.PHPSwitchJSO.phpVersionsDir + Path.DirectorySeparatorChar + "php" + this.MPHPSwitchConfig.PhpVersion,
                this.MPHPSwitchConfig.PHPSwitchJSO.phpRunningDir
            );
            Console.WriteLine("  Copy complete.");
            Console.WriteLine();

            // re-write Apache config file if JSON config `apacheUpdateConfig` was set to `true`.
            this.WSApache.WriteConfig();

            // copy additional files if there are JSON config `additionalCopy` property.
            AdditionalCopier AdditionalCopier = new AdditionalCopier(this.MPHPSwitchConfig);
            AdditionalCopier.StartCopyFiles();
        }


        /// <summary>
        /// Validate required things such as folders, files.
        /// Write the error and exit program if failed to validate.
        /// </summary>
        /// <param name="configJson">Path to phpswitch.json config file.</param>
        public void ValidateRequired(FileInfo configJson)
        {
            this.ValidateJSONFile(configJson);

            // check property values in json config must exists. --------------
            this.ConsoleStyle.WriteVerbose("--Checking PHP directories.");

            // check php versions directory. (the directory wher contain PHP multiple version in directories.)
            if (
                String.IsNullOrEmpty(this.MPHPSwitchConfig.PHPSwitchJSO.phpVersionsDir) ||
                Directory.Exists(this.MPHPSwitchConfig.PHPSwitchJSO.phpVersionsDir) == false
            )
            {
                ConsoleStyle.ErrorExit("You did not specify `phpVersionsDir` property in json file or the directory you specified does not exists. (" + this.MPHPSwitchConfig.PHPSwitchJSO.phpVersionsDir + ")");
            }
            else
            {
                this.ConsoleStyle.WriteVerbose("  --phpVersionsDir: {0} OK.", this.MPHPSwitchConfig.PHPSwitchJSO.phpVersionsDir);
            }

            // check "php-running" directory. (where php will be execute in CLI.)
            if (
                String.IsNullOrEmpty(this.MPHPSwitchConfig.PHPSwitchJSO.phpRunningDir) ||
                Directory.Exists(this.MPHPSwitchConfig.PHPSwitchJSO.phpRunningDir) == false
            )
            {
                ConsoleStyle.ErrorExit("You did not specify `phpRunningDir` property in json file or the \"php-running\" directory does not exists. (" + this.MPHPSwitchConfig.PHPSwitchJSO.phpRunningDir + ")");
            }
            else
            {
                this.ConsoleStyle.WriteVerbose("  --phpRunningDir: {0} OK.", this.MPHPSwitchConfig.PHPSwitchJSO.phpRunningDir);
            }

            // check selected php version number directory which is inside php versions directory.
            if (
                Directory.Exists(this.MPHPSwitchConfig.PHPSwitchJSO.phpVersionsDir + Path.DirectorySeparatorChar + "php" + this.MPHPSwitchConfig.PhpVersion) == false
            )
            {
                ConsoleStyle.ErrorExit("The selected PHP version does not exists. (" +
                    this.MPHPSwitchConfig.PHPSwitchJSO.phpVersionsDir + Path.DirectorySeparatorChar + "php" + this.MPHPSwitchConfig.PhpVersion +
                    ")");
            }
            else
            {
                this.ConsoleStyle.WriteVerbose(
                    "  --selected PHP version directory: {0} OK.", 
                    FileSystem.NormalizePath(this.MPHPSwitchConfig.PHPSwitchJSO.phpVersionsDir + Path.DirectorySeparatorChar + "php" + this.MPHPSwitchConfig.PhpVersion)
                );
            }

            this.ConsoleStyle.WriteVerbose("--Finish checking PHP directories.");
            // end check property values in json config must exists. ----------

            if (this.MPHPSwitchConfig.PHPSwitchJSO.apacheUpdateConfig == true)
            {
                this.WSApache.ValidateRequired();
            }

            this.ConsoleStyle.WriteVerbose();
        }


        /// <summary>
        /// Validate JSON config file.
        /// </summary>
        /// <param name="configJson">Path to phpswitch.json config file.</param>
        protected void ValidateJSONFile(FileInfo configJson)
        {
            // check json file exists. ----------------------------------------
            this.ConsoleStyle.WriteVerbose("--Validating JSON file.");

            // check phpswitch.json config file specified or exists.
            string jsonFilePath = "";
            if (
                String.IsNullOrEmpty(configJson?.FullName) == false &&
                File.Exists(configJson.FullName)
            )
            {
                // if config json is not empty and file was found.
                jsonFilePath = configJson.FullName;
                Console.WriteLine("Using custom json config file. ({0})", configJson.FullName);
            }
            else if (
              String.IsNullOrEmpty(configJson?.FullName) == false &&
              File.Exists(configJson?.FullName) == false
            )
            {
                // if config json is not empty but file not found.
                ConsoleStyle.ErrorExit("The path to custom json config file could not be found. (" + configJson?.FullName + ")");
            }
            else
            {
                // anything else.
                // use phpswitch.json that stay aside with this program.
                string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                string runningDir = FileSystem.NormalizePath(Path.Combine(exePath, "../"));
                jsonFilePath = runningDir + Path.DirectorySeparatorChar + "phpswitch.json";// just for prevent build error.
                if (
                    File.Exists(runningDir + Path.DirectorySeparatorChar + "phpswitch.json") == false &&
                    File.Exists(runningDir + Path.DirectorySeparatorChar + "phpswitch.example.json") == false
                )
                {
                    // if config json that should be aside with this program was not found.
                    ConsoleStyle.ErrorExit("The path to json config file could not be found. (" + jsonFilePath + ")");
                }
                else if (File.Exists(runningDir + Path.DirectorySeparatorChar + "phpswitch.json"))
                {
                    jsonFilePath = runningDir + Path.DirectorySeparatorChar + "phpswitch.json";
                }
                else if (File.Exists(runningDir + Path.DirectorySeparatorChar + "phpswitch.example.json"))
                {
                    jsonFilePath = runningDir + Path.DirectorySeparatorChar + "phpswitch.example.json";
                }
                Console.WriteLine("Using json config file. ({0})", jsonFilePath);
            }

            this.ConsoleStyle.WriteVerbose("--Finish validating JSON file.");
            // end check json file exists. ------------------------------------

            Console.WriteLine();

            // load json file to object.
            this.LoadJSONConfigFile(jsonFilePath);
        }


    }
}
