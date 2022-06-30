using sharedLibraries;
using phpswitch.Models;
using System;
using System.IO;
using System.Runtime.InteropServices;

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
        /// Run the application.
        /// </summary>
        /// <param name="phpversion">The PHP version to switch to.</param>
        /// <param name="configJson">Path to phpswitch.json config file.</param>
        /// <param name="verbose">Verbose value will be true or false.</param>
        public void Run(string phpversion, FileInfo configJson, bool verbose)
        {
            if (Privilege.IsAdmin() == false)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Privilege.RelaunchAsAdmin(phpversion, configJson, verbose);
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

            // start to kill php processes.
            var processTask = new ProcessTask();
            processTask.killPHP();

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
