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
         * <summary>
         * return a list of files that matches some wildcard pattern, e.g.
         * C:\p4\software\dotnet\tools\*\*.sln to get all tool solution files
         * </summary>
         * <remarks>https://stackoverflow.com/a/398522/128761 Original source code</remarks>
         * <param name="glob">pattern to match</param>
         * <returns>all matching paths</returns>
         */
        public IEnumerable<string> Glob(string glob)
        {
            foreach (string path in Glob(PathHead(glob) + Path.DirectorySeparatorChar, PathTail(glob)))
            {
                yield return path;
            }
        }

        /**
         * <summary>
         * uses 'head' and 'tail' -- 'head' has already been pattern-expanded
         * and 'tail' has not.
         * </summary>
         * <remarks>https://stackoverflow.com/a/398522/128761 Original source code</remarks>
         * <param name="head">wildcard-expanded</param>
         * <param name="tail">not yet wildcard-expanded</param>
         */
        public IEnumerable<string> Glob(string head, string tail)
        {
            if (PathTail(tail) == tail)
            {
                foreach (string path in Directory.GetFiles(head, tail).OrderBy(s => s))
                {
                    yield return path;
                }
            }
            else
            {
                foreach (string dir in Directory.GetDirectories(head, PathHead(tail)).OrderBy(s => s))
                {
                    foreach (string path in Glob(Path.Combine(head, dir), PathTail(tail)))
                    {
                        yield return path;
                    }
                }
            }
        }


        /**
         * <summary>
         * return the first element of a file path
         * </summary>
         * <remarks>https://stackoverflow.com/a/398522/128761 Original source code</remarks>
         * <param name="path">file path</param>
         * <returns>first logical unit</returns>
         */
        private string PathHead(string path)
        {
            // handle case of \\share\vol\foo\bar -- return \\share\vol as 'head'
            // because the dir stuff won't let you interrogate a server for its share list
            // FIXME check behavior on Linux to see if this blows up -- I don't think so
            if (path.StartsWith("" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar))
            {
                return path.Substring(0, 2) +
                    path.Substring(2).Split(Path.DirectorySeparatorChar)[0] +
                    Path.DirectorySeparatorChar +
                    path.Substring(2).Split(Path.DirectorySeparatorChar)[1];
            }

            return path.Split(Path.DirectorySeparatorChar)[0];
        }


        /**
         * <summary>
         * return everything but the first element of a file path
         * e.g. PathTail("C:\TEMP\foo.txt") = "TEMP\foo.txt"
         * </summary>
         * <remarks>https://stackoverflow.com/a/398522/128761 Original source code</remarks>
         * <param name="path">file path</param>
         * <returns>all but the first logical unit</returns>
         */
        private string PathTail(string path)
        {
            if (!path.Contains(Path.DirectorySeparatorChar))
            {
                return path;
            }

            return path.Substring(1 + PathHead(path).Length);
        }


        /**
         * <summary>Start copy additional files.</summary>
         */
        public void StartCopyFiles()
        {
            Console.WriteLine("Starting to copy additional files.");

            dynamic additionalCopy = MPhpSwitchConfig.additionalCopy;// convert to dynamic to easily access to its property.
            if (additionalCopy.ContainsKey(MPhpSwitchConfig.phpVersion))
            {
                // if additionalCopy has php version as property.
                JArray aCopyPhp = additionalCopy[MPhpSwitchConfig.phpVersion];

                foreach(JObject eachCopy in aCopyPhp)
                {
                    if (eachCopy.Count < 3)
                    {
                        continue;
                    }
                    string copyFrom = eachCopy["copyFromDir"].ToString();
                    string searchPattern = eachCopy["searchPattern"].ToString();
                    string copyTo = eachCopy["copyTo"].ToString();

                    if (copyTo.Contains("."))
                    {
                        // if copyTo contains dot(s), resolve relative path.
                        string newPath = Path.Combine(MPhpSwitchConfig.phpDir + Path.DirectorySeparatorChar + "php-running", copyTo);
                        copyTo = newPath;
                        newPath = default(string);
                    }

                    Console.WriteLine("  Copy from " + copyFrom + " (" + searchPattern + ")");
                    Console.WriteLine("    To " + LibFs.NormalizePath(copyTo));

                    LibFs.CopyAdditional(copyFrom, searchPattern, copyTo);
                }// end foreach
            }

            Console.WriteLine();
        }


    }
}
