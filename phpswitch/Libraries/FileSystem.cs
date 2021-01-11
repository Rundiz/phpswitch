using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace phpswitch.Libraries
{
    class FileSystem
    {


        private Models.PhpSwitchConfig MPhpSwitchConfig;


        /**
         * <summary>File system class constructor</summary>
         */
        public FileSystem(Models.PhpSwitchConfig PhpSwitchConfig)
        {
            MPhpSwitchConfig = PhpSwitchConfig;
        }


        /**
         * <summary>Copy additional folders and files.</summary>
         */
        public void CopyAdditional(string copyFrom, string searchPattern, string copyTo)
        {
            int copyCount = 0;
            if (Directory.Exists(copyFrom))
            {
                // create folders on target.
                foreach (string dirPath in Directory.GetDirectories(copyFrom, searchPattern, SearchOption.AllDirectories))
                {
                    string relDirPath = dirPath.Replace(copyFrom, "");
                    char[] charsToTrim = { ' ', '"', '\'', '/', '\\' };
                    relDirPath = relDirPath.Trim(charsToTrim);
                    Directory.CreateDirectory(NormalizePath(copyTo) + Path.DirectorySeparatorChar + relDirPath);
                    copyCount++;
                }

                // copy all files to target.
                foreach (string filePath in Directory.GetFiles(copyFrom, searchPattern, SearchOption.AllDirectories))
                {
                    string relFilePath = filePath.Replace(copyFrom, "");
                    char[] charsToTrim = { ' ', '"', '\'', '/', '\\' };
                    relFilePath = relFilePath.Trim(charsToTrim);
                    File.Copy(filePath, NormalizePath(copyTo) + Path.DirectorySeparatorChar + relFilePath, false);
                    copyCount++;
                }
            }

            Console.WriteLine("  Total {0} folders and files were copied.", copyCount);
        }


        /**
         * <summary>Copy files and folders from source to target.</summary>
         * <remarks>Copied from https://stackoverflow.com/a/3822913/128761 .</remarks>
         * <param name="phpVersion">The PHP version number. (7.3, 7.4, x.x)</param>
         */
        public void CopyPHPFolder(string phpVersion)
        {
            string sourcePath = this.NormalizePath(MPhpSwitchConfig.phpDir + "/php" + phpVersion);
            string destinationPath = this.NormalizePath(MPhpSwitchConfig.phpDir + "/php-running");

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


        /**
         * <summary>Delete contents in "php-running" folder.</summary>
         * <remarks>Copied from https://stackoverflow.com/a/1288747/128761 .</remarks>
         */
        public bool DeletePHPRunning()
        {
            if (this.IsPhpRunningFolderExists() == false)
            {
                return false;
            }

            DirectoryInfo Di = new DirectoryInfo(this.NormalizePath(MPhpSwitchConfig.phpDir + "/php-running"));

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


        /**
         * <summary>Check if apache config file for specific PHP version exists.</summary>
         */
        public bool IsApacheConfigFileExists(string phpVersion)
        {
            if (MPhpSwitchConfig.apacheDir == null)
            {
                return false;
            }

            if (File.Exists(this.NormalizePath(MPhpSwitchConfig.apacheDir + "/conf/extra/httpd-php.conf")) == false)
            {
                return false;
            }

            if (File.Exists(this.NormalizePath(MPhpSwitchConfig.apacheDir + "/conf/extra/httpd-php-" + phpVersion + ".conf")) == false)
            {
                return false;
            }

            return true;
        }


        /**
         * <summary>Check if apache folder and config file exists.</summary>
         */
        public bool IsApacheFolderExists()
        {
            if (MPhpSwitchConfig.apacheDir == null)
            {
                return false;
            }

            if (Directory.Exists(this.NormalizePath(MPhpSwitchConfig.apacheDir + "/conf")) == false)
            {
                return false;
            }

            if (Directory.Exists(this.NormalizePath(MPhpSwitchConfig.apacheDir + "/conf/extra")) == false)
            {
                return false;
            }

            return true;
        }


        /**
         * <summary>Check that "php-running" folder exists in the php versions folder path.</summary>
         */
        public bool IsPhpRunningFolderExists()
        {
            if (MPhpSwitchConfig.phpDir == null)
            {
                return false;
            }

            return Directory.Exists(this.NormalizePath(MPhpSwitchConfig.phpDir + "/php-running"));
        }


        /**
         * <summary>Check that "phpx.x" folder exists, where "x.x" is version number.</summary>
         */
        public bool IsPhpVersionExists(string version)
        {
            if (MPhpSwitchConfig.phpDir == null)
            {
                return false;
            }

            return Directory.Exists(this.NormalizePath(MPhpSwitchConfig.phpDir + "/php" + version));
        }


        /**
         * <summary>Load JSON file to object.</summary>
         */
        public Models.PhpSwitchJSO loadJSON(string jsonFile)
        {
            string json = File.ReadAllText(jsonFile);
            Models.PhpSwitchJSO deserialized = JsonConvert.DeserializeObject<Models.PhpSwitchJSO>(json);
            return deserialized;
        }


        /**
         * <summary>Normalize path.</summary>
         * <remarks>Copied from https://stackoverflow.com/a/21058121/128761 .</remarks>
         */
        public string NormalizePath(string path)
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
