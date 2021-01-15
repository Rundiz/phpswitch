using phpswitch.SubPrograms;
using System;
using System.IO;
using System.Text.Json;

namespace phpswitch.Libraries
{
    /// <summary>
    /// File system class.
    /// </summary>
    class FileSystem
    {


        private Models.PHPSwitchConfig MPHPSwitchConfig;


        /// <summary>
        /// File system class constructor.
        /// </summary>
        /// <param name="PHPSwitchConfig"></param>
        public FileSystem(Models.PHPSwitchConfig PHPSwitchConfig)
        {
            this.MPHPSwitchConfig = PHPSwitchConfig;
        }


        /// <summary>
        /// Copy additional folders and files.
        /// </summary>
        /// <param name="copyFrom">Copy from directory.</param>
        /// <param name="searchPattern">
        /// Search pattern. 
        /// See more at https://docs.microsoft.com/en-us/dotnet/api/system.io.directory.getdirectories?view=net-5.0#System_IO_Directory_GetDirectories_System_String_System_String_System_IO_SearchOption_
        /// </param>
        /// <param name="copyTo">Copy to directory.</param>
        public int CopyAdditional(string copyFrom, string searchPattern, string copyTo)
        {
            int copyCount = 0;
            ConsoleStyle ConsoleStyle = new ConsoleStyle(this.MPHPSwitchConfig);

            if (Directory.Exists(copyFrom))
            {
                // create folders on target.
                foreach (string dirPath in Directory.GetDirectories(copyFrom, searchPattern, SearchOption.AllDirectories))
                {
                    string relDirPath = dirPath.Replace(copyFrom, "");
                    char[] charsToTrim = { ' ', '"', '\'', '/', '\\' };
                    relDirPath = relDirPath.Trim(charsToTrim);
                    Directory.CreateDirectory(NormalizePath(copyTo) + Path.DirectorySeparatorChar + relDirPath);
                    ConsoleStyle.WriteVerbose("      --{0}", NormalizePath(copyTo) + Path.DirectorySeparatorChar + relDirPath);
                    copyCount++;
                }

                // copy all files to target.
                foreach (string filePath in Directory.GetFiles(copyFrom, searchPattern, SearchOption.AllDirectories))
                {
                    string relFilePath = filePath.Replace(copyFrom, "");
                    char[] charsToTrim = { ' ', '"', '\'', '/', '\\' };
                    relFilePath = relFilePath.Trim(charsToTrim);
                    File.Copy(filePath, NormalizePath(copyTo) + Path.DirectorySeparatorChar + relFilePath, false);
                    ConsoleStyle.WriteVerbose("      --Copied {0}", NormalizePath(copyTo) + Path.DirectorySeparatorChar + relFilePath);
                    copyCount++;
                }
            }

            return copyCount;
        }


        /// <summary>
        /// Copy selected PHP version directory to "php-running" directory.
        /// 
        /// https://stackoverflow.com/a/3822913/128761 Original source code.
        /// </summary>
        /// <param name="phpVersion">The PHP version number. (7.3, 7.4, x.x)</param>
        public void CopyPHPFolder(string phpVersion)
        {
            string sourcePath = FileSystem.NormalizePath(this.MPHPSwitchConfig.PHPSwitchJSO.phpVersionsDir + "/php" + phpVersion);
            string destinationPath = FileSystem.NormalizePath(this.MPHPSwitchConfig.PHPSwitchJSO.phpRunningDir);

            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            }
            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
            }
        }


        /// <summary>
        /// Delete files and directories inside "php-running" directory.
        /// </summary>
        /// <returns>Return true on success, false on failure.</returns>
        public bool DeletePHPRunning()
        {
            if (Directory.Exists(this.MPHPSwitchConfig.PHPSwitchJSO.phpRunningDir) == false)
            {
                return false;
            }

            DirectoryInfo Di = new DirectoryInfo(FileSystem.NormalizePath(this.MPHPSwitchConfig.PHPSwitchJSO.phpRunningDir));

            foreach (FileInfo file in Di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in Di.EnumerateDirectories())
            {
                dir.Delete(true);
            }

            return true;
        }


        /// <summary>
        /// Load JSON file to object.
        /// </summary>
        /// <param name="jsonFile">Full path to .json file.</param>
        /// <returns>Return PHPSwitchJSO object</returns>
        public Models.PHPSwitchJSO LoadJSON(string jsonFile)
        {
            string json = File.ReadAllText(jsonFile);
            Models.PHPSwitchJSO deserialized = JsonSerializer.Deserialize<Models.PHPSwitchJSO>(json);
            return deserialized;
        }


        /// <summary>
        /// Normalize path.
        /// https://stackoverflow.com/a/21058121/128761 Original source code.
        /// </summary>
        /// <param name="path">The path to be normalize</param>
        /// <returns>Return normalized path</returns>
        public static string NormalizePath(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return path;
            }

            char[] charsToTrim = { ' ', '"', '\'' };
            path = path.Trim(charsToTrim);

            return Path
                .GetFullPath(
                    new Uri(path).LocalPath
                )
                .TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                );
        }


    }
}
