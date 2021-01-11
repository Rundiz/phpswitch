using DotNet.Globbing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace phpswitch.SubPrograms
{
    class AdditionalCopier
    {


        private Libraries.FileSystem LibFs;


        private Models.PhpSwitchConfig MPhpSwitchConfig;


        /**
         * <summary>Additional copier class constructor</summary>
         */
        public AdditionalCopier(Models.PhpSwitchConfig PhpSwitchConfig)
        {
            LibFs = new Libraries.FileSystem(PhpSwitchConfig);

            MPhpSwitchConfig = PhpSwitchConfig;
        }


        /**
         * <summary>Start copy additional files.</summary>
         */
        public void StartCopyFiles()
        {
            //Console.WriteLine("Starting to copy additional files.");

            dynamic additionalCopy = MPhpSwitchConfig.additionalCopy;// convert to dynamic to easily access to its property.
            if (additionalCopy.ContainsKey(MPhpSwitchConfig.phpVersion))
            {
                // if additionalCopy has php version as property.
                JArray aCopyPhp = additionalCopy[MPhpSwitchConfig.phpVersion];

                foreach(JArray eachCopy in aCopyPhp)
                {
                    if (eachCopy.Count < 2)
                    {
                        continue;
                    }
                    string copyFrom = eachCopy[0].ToString();
                    string copyTo = eachCopy[1].ToString();

                    if (copyTo.Contains("."))
                    {
                        // if copyTo contains dot(s), resolve relative path.
                        string newPath = Path.Combine(MPhpSwitchConfig.phpDir + Path.DirectorySeparatorChar + "php-running", copyTo);
                        copyTo = newPath;
                        newPath = default(string);
                    }

                    //Console.WriteLine("  Copy from " + copyFrom);
                    //Console.WriteLine("    To " + LibFs.NormalizePath(copyTo));

                    var glob = Glob.Parse(copyTo);
                    
                }// end foreach
            }

            //Console.WriteLine();
        }


    }
}
