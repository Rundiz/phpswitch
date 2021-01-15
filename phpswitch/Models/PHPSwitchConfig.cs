using System;

namespace phpswitch.Models
{
    class PHPSwitchConfig
    {


        /// <summary>
        /// PHP Switch JSON object.
        /// </summary>
        public PHPSwitchJSO PHPSwitchJSO
        {
            get;
            set;
        }


        /// <summary>
        /// PHP version.
        /// </summary>
        public string PhpVersion
        {
            get;
            set;
        }


        /// <summary>
        /// Are there any running service task. The result is true if there is, false for not.
        /// </summary>
        public bool RunServiceTask
        {
            get;
            set;
        } = false;


        /// <summary>
        /// Show verbose or not.
        /// </summary>
        public bool Verbose
        {
            get;
            set;
        }


    }
}
