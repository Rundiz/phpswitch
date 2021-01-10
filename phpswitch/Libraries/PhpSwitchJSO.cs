using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**
 * Class that create as reference for **phpswitch.json** config file.
 */


namespace phpswitch.Libraries
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


        public string phpVersionsDir
        {
            get;
            set;
        }


    }
}
