using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**
 * Class that contains configuration from file or input and use between classes.
 */


namespace phpswitch.Models
{
    class PhpSwitchConfig
    {


        /**
         * <summary>Additional copy tasks.</summary>
         */
        public object additionalCopy
        {
            get;
            set;
        }


        /**
         * <summary>Apache main folder path.</summary>
         */
        public string apacheDir
        {
            get;
            set;
        }


        /**
         * <summary>Update Apache config file.</summary>
         */
        public bool apacheUpdateConfig
        {
            get;
            set;
        } = true;


        /**
         * <summary>PHP versions folder path.</summary>
         */
        public string phpDir
        {
            get;
            set;
        }


        /**
         * <summary>PHP version number.</summary>
         */
        public string phpVersion
        {
            get;
            set;
        }


        /**
         * <summary>The current path that this command is running. Example: run `phpswitch` in C:\Inetpub then this path will be return.</summary>
         */
        public string runningDir
        {
            get;
            set;
        }


        /**
         * <summary>Run service tasks or not.</summary>
         */
        public bool runServiceTask
        {
            get;
            set;
        } = false;


        /**
         * <summary>Web server service name.</summary>
         */
        public string webserverServiceName
        {
            get;
            set;
        } = "apache";


    }
}
