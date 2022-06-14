using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dalProgHost
{
    class HostError : Exception
    {
        public string msg = "";
        public override string ToString()
        {
            return msg;
        }
    }
    class EXCantExtractWebsite : HostError
    {
        public EXCantExtractWebsite()
        {
            msg = "Can't extract website name from url!";
        }
    }


    class EXSocketException : Exception
    {
        public string msg = "";
    }
    class EXCantGetCmdId : EXSocketException
    {
        public EXCantGetCmdId()
        {
            msg = "Can't get Command Id!";
        }
    }



    class AppletException : Exception
    {
        public string msg = "";

        public override string ToString()
        {
            return msg;
        }
    }
    class EXNotSignedIn : AppletException
    {
        public EXNotSignedIn()
        {
            msg = "User not signed in!";
        }
    }
    //class EXNotRegistered : AppletException
    //{
    //    public EXNotRegistered() //thrown at sign in...
    //    {
    //        msg = "No User registered!";
    //    }
    //}

}
