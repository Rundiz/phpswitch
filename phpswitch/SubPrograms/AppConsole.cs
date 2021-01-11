using System;

namespace phpswitch.SubPrograms
{
    class AppConsole
    {


        /**
         * <summary>Clear current console line.</summary>
         * <remarks>Copied from https://stackoverflow.com/a/5027364/128761 .</remarks>
         */
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


        /**
         * <summary>Display help message.</summary>
         */
        public static void DisplayHelp()
        {
            Console.WriteLine("Usage: phpswitch <PHP version> [<PHP versions folder>] [<Apache folder>]");
            Console.WriteLine("");
            Console.WriteLine("The <PHP version> is version number of PHP that exists in the folder. Example: 8.0 for PHP v8.0.");
            Console.WriteLine("");
            Console.WriteLine("Available options:");
            Console.WriteLine("[<PHP versions folder>] The folder that contain PHP versions for switch to. This folder must contain \"php-running\" folder in it.");
            Console.WriteLine("[<Apache folder>] The folder of installed Apache. This folder must contain \"conf\", \"bin\" folders in it. Leave this option empty if you don't use web server.");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }


        /**
         * <summary>Display program header.</summary>
         */
        public static void DisplayProgramHeader()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("PHPSwitch.");
            Console.ResetColor();
        }


        /**
         * <summary>Write error message with red text. Reset color after completed.</summary>
         */
        public static void ErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.Error.Close();
            Console.ResetColor();
        }


        /**
         * <summary>Display success message and exit.</summary>
         */
        public static void SuccessExit()
        {
            // success message.
            SuccessMessage("Completed. Please reload the web page or try \"php -v\" in the command line.");
            Console.WriteLine();

            // press any key to exit or wait to exit automatically.
            // Copied from https://stackoverflow.com/questions/11512821/how-to-stop-c-sharp-console-applications-from-closing-automatically .
            // use Press any key to exit ------------.
            //Console.WriteLine("Press enter to exit.");
            //Console.ReadLine();
            // use auto exit ------------------------.
            System.Threading.Thread.Sleep(5000);
        }


        /**
         * <summary>Write success message with green text. Reset color after completed.</summary>
         */
        public static void SuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }


    }
}
