using phpswitch.SubPrograms;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

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
            var rootCommand = new RootCommand
            {
                new Argument<string>(
                    "phpversion",
                    "The PHP version to switch to."),
                new Option<FileInfo>(
                    "--config-json",
                    "The path to phpswitch.json configuration file."),
            };

            // add option with alias.
            var option = new Option(
                "--verbose",
                "Show verbose output");
            option.AddAlias("-v");
            rootCommand.Add(option);

            rootCommand.Description = "The PHP switcher command line that will work on these tasks for you."
                + "\n"
                + "  Switch PHP version for CLI.\n"
                + "  Rewrite Apache configuration to use selected PHP version. (Optional)\n"
                + "  Start and stop web server service(s). (Optional)\n"
                + "  Copy files depend on PHP version in JSON config file. (Optional)";

            var method = typeof(App).GetMethod("Run");
            rootCommand.Handler = CommandHandler.Create(method);

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
