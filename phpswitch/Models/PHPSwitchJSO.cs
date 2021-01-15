using System;
using System.Collections.Generic;

namespace phpswitch.Models
{
    /// <summary>
    /// Class that create as reference for **phpswitch.json** config file.
    /// </summary>
    class PHPSwitchJSO
    {

        /// <summary>
        /// Additional copy tasks.
        /// </summary>
        public Dictionary<string, object> additionalCopy
        {
            get;
            set;
        }


        ///
        /// <summary>
        /// Apache main folder path.
        /// </summary>
        public string apacheDir
        {
            get;
            set;
        }


        /// <summary>
        /// Update Apache config file.
        /// </summary>
        public bool apacheUpdateConfig
        {
            get;
            set;
        } = true;


        /// <summary>
        /// PHP running folder path.
        /// This is where PHP executable will be run from CLI.
        /// </summary>
        public string phpRunningDir
        {
            get;
            set;
        }


        /// <summary>
        /// PHP versions folder path.
        /// This folder must contain multiple PHP versions in each sub folder.
        /// </summary>
        public string phpVersionsDir
        {
            get;
            set;
        }


        /// <summary>
        /// Web server service name.
        /// </summary>
        public string[] webserverServiceName
        {
            get;
            set;
        }


    }
}
