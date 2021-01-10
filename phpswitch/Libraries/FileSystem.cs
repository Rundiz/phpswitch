using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace phpswitch.Libraries
{
    class FileSystem
    {


        private string apacheDir;


        private string phpDir;


        /**
         * <summary>Get or set Apache folder</summary>
         */
        public string ApacheDir
        {
            get
            {
                if (apacheDir == null)
                {
                    // Current path was copied from https://stackoverflow.com/a/32339322/128761 .
                    string parentDir = Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).ToString();
                    return this.NormalizePath(parentDir + "/apache/Apache24");
                } else
                {
                    return this.NormalizePath(this.apacheDir);
                }
            }
            set
            {
                this.apacheDir = value;
            }
        }


        /**
         * <summary>Get or set PHP folder</summary>
         */
        public string PhpDir
        {
            get {
                if (phpDir == null)
                {
                    // Current path was copied from https://stackoverflow.com/a/32339322/128761 .
                    string parentDir = Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).ToString();
                    return this.NormalizePath(parentDir + "/php");
                } else
                {
                    return this.NormalizePath(this.phpDir);
                }
            }
            set {
                this.phpDir = value;
            }
        }


        /**
         * <summary>Copy files and folders from source to target.</summary>
         * <remarks>Copied from https://stackoverflow.com/a/3822913/128761 .</remarks>
         * <param name="phpVersion">The PHP version number. (5.5, 5.6, x.x)</param>
         */
        public void CopyFolder(string phpVersion)
        {
            string sourcePath = this.NormalizePath(this.PhpDir + "/php" + phpVersion);
            string destinationPath = this.NormalizePath(this.PhpDir + "/php-running");

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
        public bool DeletePhpRunning()
        {
            if (this.IsPhpRunningFolderExists() == false)
            {
                return false;
            }

            DirectoryInfo Di = new DirectoryInfo(this.NormalizePath(this.PhpDir + "/php-running"));

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
            if (this.ApacheDir == null)
            {
                return false;
            }

            if (File.Exists(this.NormalizePath(this.ApacheDir + "/conf/extra/httpd-php.conf")) == false)
            {
                return false;
            }

            if (File.Exists(this.NormalizePath(this.ApacheDir + "/conf/extra/httpd-php-" + phpVersion + ".conf")) == false)
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
            if (this.ApacheDir == null)
            {
                return false;
            }

            if (Directory.Exists(this.NormalizePath(this.ApacheDir + "/conf")) == false)
            {
                return false;
            }

            if (Directory.Exists(this.NormalizePath(this.ApacheDir + "/conf/extra")) == false)
            {
                return false;
            }

            if (File.Exists(this.NormalizePath(this.ApacheDir + "/conf/extra/httpd-php.conf")) == false)
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
            if (this.PhpDir == null)
            {
                return false;
            }

            return Directory.Exists(this.NormalizePath(this.PhpDir + "/php-running"));
        }


        /**
         * <summary>Check that "phpx.x" folder exists. The "x.x" is version number.</summary>
         */
        public bool IsPhpVersionExists(string version)
        {
            if (this.PhpDir == null)
            {
                return false;
            }

            return Directory.Exists(this.NormalizePath(this.PhpDir + "/php" + version));
        }


        /**
         * <summary>Load JSON file to object.</summary>
         */
        public Libraries.PhpSwitchJSO loadJSON(string jsonFile)
        {
            string json = File.ReadAllText(jsonFile);
            Libraries.PhpSwitchJSO deserialized = JsonConvert.DeserializeObject<Libraries.PhpSwitchJSO>(json);
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
