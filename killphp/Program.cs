using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace killphp
{
    class Program
    {


        /// <summary>
        /// Main entry for the program.
        /// https://dotnetdevaddict.co.za/2020/09/25/getting-started-with-system-commandline/ Tutorial for System.CommandLine
        /// https://github.com/dotnet/command-line-api System.CommandLine Git repository and its document.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand ("Kills PHP execution files that are currently running.");

            rootCommand.Description = "Kills PHP execution files that are currently running.";

            rootCommand.SetHandler(() =>
            {
                var app = new App();
                app.Run();
            });

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }
    }
}