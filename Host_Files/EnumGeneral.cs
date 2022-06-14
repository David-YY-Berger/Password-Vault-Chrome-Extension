using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dalProgHost
{
    enum FromAppletOperation //applet -> host
    {
        RES_FAIL = 0,
        RES_SUCCESS = 1,
        RES_PSWD_GENERATED = 2,
        RES_PSWD_RETRIEVED = 3,
        RES_NOT_SIGNED_IN = 4,
        RES_WRONG_PASSWORD = 5,
        RES_NOT_REGISTERED = 6
    }

    enum ToAppletOperation //host -> applet
    {
        RESET_MEMORY = 0, //Applet erases any existing memory
        RETRIEVE_PASSWORD = 1, //Host sends URL, if Applet has no password saved,

        //generates password and returns
        RETURN_ALL_PASSWORDS = 2, //Applet returns all URL's and passwords
        SIGN_IN = 3,
        REGISTER = 4
    }

    enum ToServerOperation // client ->server
    {
        REGISTER = 0, //client sets up his account password
        SIGN_IN = 2, //when client sends server a password to get into vault
        GET_PASSWORD = 3, //client wants password (from vault) for this URL
        GET_ALL_PASSWORDS = 4, //client wants all urls and passwords from vault
        RESET_MEMORY = 5
    }

    enum FromServerOperation //server -> client
    {
        MUST_LOG_IN = 0,
        SEND_ME_PASSWORD = 1,
        SUCCESS = 2,
        ER_WRONG_PASSWORD = 3, //when client entered wrong password
        HERE_IS_PASSWORD = 4, //when server sends password to client
        ER_NOT_REGISTERED = 5,
        ER_UNKOWN_ERROR = 6,
        HERE_ARE_ALL_PASSWORDS = 7,
    }
}
