using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**
 * Class that create as reference for **phpswitch.json** config file.
 */


namespace phpswitch.Models
{
    class PhpSwitchJSO
    {


        public object additionalCopy
        {
            get;
            set;
        }


        public string apacheDir
        {
            get;
            set;
        }


        public bool apacheUpdateConfig
        {
            get;
            set;
        } = true;


        public string phpVersionsDir
        {
            get;
            set;
        }


        public string webserverServiceName
        {
            get;
            set;
        } = "apache";


    }
}
