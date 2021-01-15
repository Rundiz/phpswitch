using phpswitch.Models;
using System;

namespace phpswitch.SubPrograms
{
    /// <summary>
    /// Console style class for write out with custom styles.
    /// </summary>
    class ConsoleStyle
    {


        protected PHPSwitchConfig MPHPSwitchConfig;


        /// <summary>
        /// Console style class constructor.
        /// </summary>
        /// <param name="PHPSwitchConfigClass">The PHPSwitchConfig class.</param>
        public ConsoleStyle(PHPSwitchConfig PHPSwitchConfigClass)
        {
            this.MPHPSwitchConfig = PHPSwitchConfigClass;
        }


        /// <summary>
        /// Clear current console line. Example: You wrote "Loading..." and when load done, you want to delete that message and change to "Success!".
        /// 
        /// https://stackoverflow.com/a/5027364/128761 Original source code.
        /// </summary>
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }
            Console.SetCursorPosition(0, currentLineCursor - 1);
        }


        /// <summary>
        /// Write error message and exit program.
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="wait">Set to true to wait and exit, false to exit immediately.</param>
        public static void ErrorExit(string message, bool wait = true)
        {
            ErrorMessage(message);
            if (wait == true)
            {
                System.Threading.Thread.Sleep(5000);
            }
            Environment.Exit(1);
        }


        /// <summary>
        /// Write error message in red. Reset color after completed.
        /// </summary>
        /// <param name="message">Message to display</param>
        public static void ErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.Error.Close();
            Console.ResetColor();
        }


        /// <summary>
        /// Write program header in specific style. Reset color after completed.
        /// </summary>
        /// <param name="name">The program name. Leave blank to use default</param>
        public static void ProgramHeader(string name = "")
        {
            if (String.IsNullOrEmpty(name))
            {
                name = "PHPSwitch";
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(name);
            Console.ResetColor();
        }


        /// <summary>
        /// Write success message and wait before exit.
        /// </summary>
        /// <param name="message">Message to display. Leave blank to use default</param>
        public static void SuccessExit(string message = "")
        {
            if (String.IsNullOrEmpty(message))
            {
                message = "Completed.";
            }

            // success message.
            SuccessMessage(message);
            Console.WriteLine();

            // press any key to exit or wait to exit automatically.
            // Copied from https://stackoverflow.com/questions/11512821/how-to-stop-c-sharp-console-applications-from-closing-automatically .
            // use Press any key to exit ------------.
            //Console.WriteLine("Press enter to exit.");
            //Console.ReadLine();
            // use auto exit ------------------------.
            System.Threading.Thread.Sleep(5000);
            Environment.Exit(0);
        }


        /// <summary>
        /// Write success message in green. Reset color after completed.
        /// </summary>
        /// <param name="message">Message to display</param>
        public static void SuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }


        /// <summary>
        /// Write verbose message if command argument `--verbose` was set.
        /// </summary>
        /// <param name="message">A composite format string.</param>
        /// <param name="arg0">The object to write using format.</param>
        /// <param name="arg1" >The object to write using format.</param>
        /// <param name="arg2">The object to write using format.</param>
        #nullable enable
        public void WriteVerbose(string message = "", object? arg0 = null, object? arg1 = null, object? arg2 = null)
        {
            if (this.MPHPSwitchConfig.Verbose)
            {
                Console.WriteLine(message, arg0, arg1, arg2);
            }
        }


    }
}
