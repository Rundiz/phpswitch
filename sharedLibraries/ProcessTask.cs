using System;
using System.Diagnostics;

namespace sharedLibraries
{
    /// <summary>
    /// Process task.
    /// </summary>
    public class ProcessTask
    {


        // <summary>
        /// Kill PHP execution file processes.
        /// </summary>
        public void killPHP()
        {
            string[] phpfiles =
            {
                "php",
                "php-cgi",
                "phpdbg",
                "php-win"
            };

            foreach (string exefile in phpfiles)
            {
                Console.WriteLine("Searching for process name " + exefile + " to terminate.");

                var allProcesses = Process.GetProcessesByName(exefile);
                foreach (var process in allProcesses)
                {
                    Console.WriteLine("  Found " + process.ProcessName + " (" + process.Id + ").");
                    try
                    {
                        process.Kill(true);
                        Console.WriteLine("    Process terminated.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("    {0} Exception caught.", e);
                    }

                }

                if (allProcesses.Length <= 0)
                {
                    Console.WriteLine("  Not found.");
                }
            }
            Console.WriteLine();
        }


    }
}