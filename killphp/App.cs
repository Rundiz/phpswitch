using sharedLibraries;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace killphp
{
    internal class App
    {


        /// <summary>
        /// Application class constructor.
        /// </summary>
        public App()
        {

        }


        /// <summary>
        /// Run the application.
        /// </summary>
        public void Run()
        {
            Privilege.IsAdmin();
            if (Privilege.IsAdmin() == false)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    try
                    {
                        Privilege.RelaunchAsAdmin("");
                    } catch (Exception ex)
                    {
                        ConsoleStyle.ErrorMessage(ex.Message);
                        Environment.Exit(5);
                    }
                }
                else
                {
                    ConsoleStyle.ErrorMessage("Administrator privileges are required!");
                    Environment.Exit(403);
                }
            }

            // display program header.
            ConsoleStyle.ProgramHeader();

            // start to kill php processes.
            var processTask = new ProcessTask();
            processTask.killPHP();

            // success message and exit.
            ConsoleStyle.SuccessExit();
        }


    }
}
