using System;
using System.ServiceProcess;
using phpswitch.SubPrograms;

namespace phpswitch.SubPrograms
{
    class Service
    {


        /**
         * <summary>Check if service exists. Must check with this method before call other methods.</summary>
         * <param name="searchService">The part of service name to search. Lower case only.</param>
         * <returns>Return true if service exists, false for otherwise.</returns>
         */
        public static bool IsServiceExists(string searchService)
        {
            if (String.IsNullOrEmpty(searchService) == true)
            {
                return false;
            }

            searchService = searchService.ToLower();
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();
            foreach (ServiceController eachService in scServices)
            {
                string serviceName = eachService.ServiceName.ToLower();
                if (serviceName.Contains(searchService)) {
                    // if search found service name (at least part of name or the whole name to allow stop/start multiple services at once).
                    return true;
                }
            }

            return false;
        }


        /**
         * <summary>Start the web server service(s).</summary>
         * <remarks>Copied from https://docs.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicecontroller.getservices?view=netframework-4.7.2 .</remarks>
         * <param name="searchService">The part of service name to search. Lower case only.</param>
         */
        public static void StartWebServerService(string searchService)
        {
            searchService = searchService.ToLower();
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();
            bool foundServices = false;

            foreach (ServiceController eachService in scServices)
            {
                string serviceName = eachService.ServiceName.ToLower();
                if (serviceName.Contains(searchService))
                {
                    // if found service contains search name. example search for apache, found Apachex86 and Apachex64 then start them both.
                    foundServices = true;

                    if (eachService.Status != ServiceControllerStatus.Running)
                    {
                        // if service is NOT running.
                        Console.WriteLine("Trying to start the service, please wait...");
                        try
                        {
                            eachService.Start();
                            eachService.WaitForStatus(ServiceControllerStatus.Running);
                            AppConsole.ClearCurrentConsoleLine();
                            Console.WriteLine("The " + eachService.ServiceName + " service is now {0}.", eachService.Status.ToString().ToLower());
                        } catch (InvalidOperationException)
                        {
                            AppConsole.ClearCurrentConsoleLine();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Could not start the " + eachService.ServiceName + " service.");
                            Console.Error.Close();
                            Console.ResetColor();
                            System.Threading.Thread.Sleep(5000);
                            Environment.Exit(1);
                        }
                    }
                }
            }// end foreach

            if (foundServices == true)
            {
                Console.WriteLine();
            }
        }


        /**
         * <summary>Stop the web server service(s).</summary>
         * <remarks>Copied from https://docs.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicecontroller.getservices?view=netframework-4.7.2 .</remarks>
         * <param name="searchService">The part of service name to search. Lower case only.</param>
         */
        public static void StopWebServerService(string searchService)
        {
            searchService = searchService.ToLower();
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();
            bool foundServices = false;

            foreach (ServiceController eachService in scServices)
            {
                string serviceName = eachService.ServiceName.ToLower();
                if (serviceName.Contains(searchService))
                {
                    // if found service contains search name. example search for apache, found Apachex86 and Apachex64 then stop them both.
                    Console.WriteLine("Found service: " + eachService.ServiceName + " (" + eachService.DisplayName + ")");
                    Console.WriteLine("  The " + eachService.ServiceName + " service is currently " + (eachService.Status == ServiceControllerStatus.Running ? "running" : "stopped") + ".");
                    foundServices = true;

                    if (eachService.Status == ServiceControllerStatus.Running)
                    {
                        // if service is running.
                        Console.WriteLine("  Trying to stop, please wait...");
                        try
                        {
                            eachService.Stop();
                            eachService.WaitForStatus(ServiceControllerStatus.Stopped);
                            AppConsole.ClearCurrentConsoleLine();
                            Console.WriteLine("  The " + eachService.ServiceName + " service is now {0}.", eachService.Status.ToString().ToLower());
                        } catch (InvalidOperationException)
                        {
                            AppConsole.ClearCurrentConsoleLine();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Error.WriteLine("Could not stop the " + eachService.ServiceName + " service.");
                            Console.Error.Close();
                            Console.ResetColor();
                            System.Threading.Thread.Sleep(5000);
                            Environment.Exit(1);
                        }
                    }
                }
            }// end foreach;

            if (foundServices == true)
            {
                Console.WriteLine();
            }
        }


    }
}
