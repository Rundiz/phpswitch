using phpswitch.Libraries;
using phpswitch.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace phpswitch.SubPrograms
{
    /// <summary>
    /// Additional copy files and/or directories from JSON config `additionalCopy` property.
    /// </summary>
    class AdditionalCopier
    {


        protected ConsoleStyle ConsoleStyle;


        protected FileSystem LibFS;


        protected PHPSwitchConfig MPHPSwitchConfig;


        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="PHPSwitchConfigClass">The PHPSwitchConfig class.</param>
        public AdditionalCopier(PHPSwitchConfig PHPSwitchConfigClass)
        {
            this.MPHPSwitchConfig = PHPSwitchConfigClass;
            this.LibFS = new FileSystem(this.MPHPSwitchConfig);
            this.ConsoleStyle = new ConsoleStyle(this.MPHPSwitchConfig);
        }


        /// <summary>
        /// Start copy files.
        /// <para>https://marcroussy.com/2020/08/17/deserialization-with-system-text-json/ JSON .parse example code.</para>
        /// </summary>
        public void StartCopyFiles()
        {
            Console.WriteLine("Starting to copy additional files.");

            Dictionary<string, object> additionalCopyJSO = this.MPHPSwitchConfig.PHPSwitchJSO.additionalCopy;

            if (additionalCopyJSO.ContainsKey(this.MPHPSwitchConfig.PhpVersion))
            {
                // if additionalCopy has php version as property.
                this.ConsoleStyle.WriteVerbose("  --Additional copy was set in JSON config file.");
                dynamic selctedPHPVersionObject = additionalCopyJSO[this.MPHPSwitchConfig.PhpVersion];
                dynamic copyList = selctedPHPVersionObject.EnumerateArray();
                foreach (JsonElement eachTask in copyList)
                {
                    string copyTo = eachTask.GetProperty("copyTo").ToString();
                    string searchPattern = eachTask.GetProperty("searchPattern").ToString();
                    string copyFromDir = eachTask.GetProperty("copyFromDir").ToString();
                    
                    if (copyTo.Contains("."))
                    {
                        // if copyTo contains dot(s), resolve relative path.
                        string newPath = Path.Combine(this.MPHPSwitchConfig.PHPSwitchJSO.phpRunningDir, copyTo);
                        copyTo = newPath;
                        newPath = default(string);
                    }

                    Console.WriteLine("  Copy from {0} (search pattern: {1})", copyFromDir, searchPattern);
                    Console.WriteLine("    To {0}", FileSystem.NormalizePath(copyTo));

                    int totalCopied = this.LibFS.CopyAdditional(copyFromDir, searchPattern, copyTo);

                    Console.WriteLine("  Total {0} items were copied.", totalCopied);
                }// end foreach;
            } else
            {
                this.ConsoleStyle.WriteVerbose(" --Could not found additional copy task for selected PHP version. ({0})", this.MPHPSwitchConfig.PhpVersion);
            }

            Console.WriteLine();
        }


    }
}
