using phpswitch.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace phpswitch.SubPrograms
{
    /// <summary>
    /// Services class that do start and stop tasks.
    /// </summary>
    class Services
    {


        protected ConsoleStyle ConsoleStyle;


        protected PHPSwitchConfig MPHPSwitchConfig;


        protected List<string> AllServices;


        protected List<string> SearchedServices;


        /// <summary>
        /// Service class constructor.
        /// </summary>
        /// <param name="PHPSwitchConfigClass">The PHPSwitchConfig class.</param>
        public Services(PHPSwitchConfig PHPSwitchConfigClass)
        {
            this.MPHPSwitchConfig = PHPSwitchConfigClass;
            this.ConsoleStyle = new ConsoleStyle(this.MPHPSwitchConfig);

            this.GetServices();
        }


        /// <summary>
        /// Get all services.
        /// </summary>
        /// <returns>Return list of all services.</returns>
        protected List<string> GetServices()
        {
            List<string> serviceList = new List<string>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ServiceController[] scServices = ServiceController.GetServices();
                foreach (ServiceController eachService in scServices)
                {
                    serviceList.Add(eachService.ServiceName);
                }
            }
            else
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \" service --status-all \"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,// prevent error echo out.
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                while (!proc.StandardOutput.EndOfStream)
                {
                    string line = proc.StandardOutput.ReadLine();
                    string pattern = @"\[(.+)\](\s+)([\w-]+)";
                    RegexOptions options = RegexOptions.IgnoreCase;
                    MatchCollection matches = Regex.Matches(line, pattern, options);

                    foreach (Match match in matches)
                    {
                        /*Console.WriteLine(" >> [value{0}] [1{1}] [2{2}] [3{3}]",
                            match.Value,
                            match.Groups[1].Value,
                            match.Groups[2].Value,
                            match.Groups[3].Value);*/
                        serviceList.Add(match.Groups[3].Value);
                    }
                }
            }

            this.AllServices = serviceList;
            return serviceList;
        }


        /// <summary>
        /// Check if services exists.
        /// You must check with this method before start/stop because it will be use the list generated from this method.
        /// </summary>
        /// <returns>Return true if found at least one of them, return false if not found at all.</returns>
        public bool IsServiceExists()
        {
            if (this.MPHPSwitchConfig.PHPSwitchJSO.webserverServiceName == null)
            {
                return false;
            }
            if (this.MPHPSwitchConfig.PHPSwitchJSO.webserverServiceName.Length == 0)
            {
                return false;
            }

            IEnumerable<string> searchedServices = this.AllServices.Intersect(this.MPHPSwitchConfig.PHPSwitchJSO.webserverServiceName, StringComparer.OrdinalIgnoreCase);
            this.SearchedServices = searchedServices.ToList();

            if (this.MPHPSwitchConfig.Verbose)
            {
                Console.WriteLine("--Search service exists");
                foreach (string eachService in searchedServices)
                {
                    Console.WriteLine("  --" + eachService);
                }
                Console.WriteLine("  --Found: " + searchedServices.Any());
                Console.WriteLine("--Finish search service exists");
                Console.WriteLine();
            }

            return searchedServices.Any();
        }


        /// <summary>
        /// Start the services that specified in JSON config file.
        /// </summary>
        public void StartServices()
        {
            bool foundService = false;

            foreach (string eachService in this.SearchedServices)
            {
                if (foundService == false)
                {
                    foundService = true;
                }

                Console.WriteLine("Found service: " + eachService);
                Console.WriteLine("  Trying to start, please wait...");

                try
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        // if Windows OS.
                        var proc = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "cmd.exe",
                                Arguments = $"/C net start \"{eachService}\"",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,// prevent error echo out.
                                CreateNoWindow = true
                            }
                        };
                        proc.Start();
                        proc.WaitForExit();// wait for response.

                        ConsoleStyle.ClearCurrentConsoleLine();
                        Console.WriteLine("  The " + eachService + " service is now started.");
                    }
                    else
                    {
                        // if other OS.
                        var proc = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "/bin/bash",
                                Arguments = $"-c \" service {eachService} start \"",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,// prevent error echo out.
                                CreateNoWindow = true
                            }
                        };
                        proc.Start();
                        proc.WaitForExit();// wait for response.

                        ConsoleStyle.ClearCurrentConsoleLine();
                        Console.WriteLine("  The " + eachService + " service is now started.");
                    }
                }
                catch (InvalidOperationException)
                {
                    ConsoleStyle.ClearCurrentConsoleLine();
                    ConsoleStyle.ErrorExit("Could not start the " + eachService + " service.");
                }
            }// endforeach;

            if (foundService == true)
            {
                Console.WriteLine();
            }
        }


        /// <summary>
        /// Stop the services that specified in JSON config file.
        /// </summary>
        public void StopServices()
        {
            bool foundService = false;

            foreach (string eachService in this.SearchedServices)
            {
                if (foundService == false)
                {
                    foundService = true;
                }

                Console.WriteLine("Found service: " + eachService);
                Console.WriteLine("  Trying to stop, please wait...");

                try
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        // if Windows OS.
                        var proc = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "cmd.exe",
                                Arguments = $"/C net stop \"{eachService}\"",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,// prevent error echo out.
                                CreateNoWindow = true
                            }
                        };
                        proc.Start();
                        proc.WaitForExit();// wait for response.

                        ConsoleStyle.ClearCurrentConsoleLine();
                        Console.WriteLine("  The " + eachService + " service is now stopped.");
                    }
                    else
                    {
                        // if other OS.
                        var proc = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "/bin/bash",
                                Arguments = $"-c \" service {eachService} stop \"",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,// prevent error echo out.
                                CreateNoWindow = true
                            }
                        };
                        proc.Start();
                        proc.WaitForExit();// wait for response.

                        ConsoleStyle.ClearCurrentConsoleLine();
                        Console.WriteLine("  The " + eachService + " service is now stopped.");
                    }
                }
                catch (InvalidOperationException)
                {
                    ConsoleStyle.ClearCurrentConsoleLine();
                    ConsoleStyle.ErrorExit("Could not stop the " + eachService + " service.");
                }
            }// endforeach;

            if (foundService == true)
            {
                Console.WriteLine();
            }
        }


    }
}
