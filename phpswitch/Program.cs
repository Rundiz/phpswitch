using phpswitch.SubPrograms;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text.Json;

namespace phpswitch
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
            var rootCommand = new RootCommand("The PHP switcher command line that will work on these tasks for you.");
            var phpversion = new Argument<string>(
                    name: "phpversion",
                    description: "The PHP version to switch to."
                );
            rootCommand.AddArgument(phpversion);

            var configjson = new Option<FileInfo>(
                    name: "--config-json",
                    description: "The path to phpswitch.json configuration file."
                );
            rootCommand.AddOption(configjson);
            var verbose = new Option<bool>(
                    name: "--verbose",
                    description: "Show verbose output"
                );
            verbose.AddAlias("-v");
            rootCommand.Add(verbose);

            rootCommand.Description = "The PHP switcher command line that will work on these tasks for you."
                + "\n"
                + "  Switch PHP version for CLI.\n"
                + "  Rewrite Apache configuration to use selected PHP version. (Optional)\n"
                + "  Start and stop web server service(s). (Optional)\n"
                + "  Copy files depend on PHP version in JSON config file. (Optional)";

            rootCommand.SetHandler((phpversion, configJson, verbose) => {
                var app = new App();
                app.Run(phpversion, configJson, verbose);
            },
            phpversion, configjson, verbose);

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
