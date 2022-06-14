using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using WebSocketSharp;

namespace dalProgHost
{
    //Web socket Behaviour - For Applet
    class WSBehav_Applet : WebSocketBehavior
    {
        AppletAPI myAppletAPI = new AppletAPI();
        const int CMD_ID_LENGTH = 1;

        protected override void OnOpen()
        {
            base.OnOpen();
            Console.WriteLine("Client opened a socket!");
            sendMsgToClient(FromServerOperation.SUCCESS, "successfully connected to Server!");
        }

        protected override void OnMessage(MessageEventArgs e) //upon received message
        {
            string receivedMsg = e.Data;
            string data = receivedMsg.Substring(CMD_ID_LENGTH);
            int commandId = getCommandId(receivedMsg);

            Console.WriteLine("<- Received message from Echo client: " + (ToServerOperation)commandId + " " + data);

            switch (commandId)
            {
                case (int)ToServerOperation.REGISTER:
                    {
                        // on register, the data is the new password
                        myAppletAPI.ResetMemory();
                        int responseCode = myAppletAPI.Register(data);
                        if (responseCode == (int)FromAppletOperation.RES_SUCCESS)
                            sendMsgToClient(FromServerOperation.SUCCESS, "successfully registered!");
                        break;
                    }
                case (int)ToServerOperation.SIGN_IN:
                    {
                        // on signin, the data is the password
                        int responseCode = myAppletAPI.SignIn(data);
                        if (responseCode == (int)FromAppletOperation.RES_WRONG_PASSWORD)
                            sendMsgToClient(FromServerOperation.ER_WRONG_PASSWORD, "wrong password!");

                        else if (responseCode == (int)FromAppletOperation.RES_NOT_REGISTERED)
                            sendMsgToClient(FromServerOperation.ER_NOT_REGISTERED, "you must register!");

                        else if (responseCode == (int)FromAppletOperation.RES_SUCCESS)
                            sendMsgToClient(FromServerOperation.SUCCESS, "successfully signed in!");
                        else
                            sendMsgToClient(FromServerOperation.ER_UNKOWN_ERROR, "unknown error");
                        break;
                    }
                case (int)ToServerOperation.GET_PASSWORD:
                    {
                        // on get password, the data is the url
                        try
                        {
                            bool generated;

                            string msg = myAppletAPI.GetPassword(data, out generated);
                            if (generated)
                                msg += " generated pswd!";
                            else
                                msg += "password retrieved";
                            sendMsgToClient(FromServerOperation.HERE_IS_PASSWORD, msg);
                        }
                        catch (EXNotSignedIn ex)
                        {
                            sendMsgToClient(FromServerOperation.MUST_LOG_IN, ex.msg);
                        }

                        break;
                    }
                case (int)ToServerOperation.GET_ALL_PASSWORDS:
                    {
                        try
                        {
                            string msg = myAppletAPI.GetAllPasswords();
                            sendMsgToClient(FromServerOperation.HERE_ARE_ALL_PASSWORDS, msg);
                        }
                        catch (EXNotSignedIn ex)
                        {
                            sendMsgToClient(FromServerOperation.MUST_LOG_IN, ex.msg);
                        }

                        break;
                    }
                case (int)ToServerOperation.RESET_MEMORY:
                    {
                        myAppletAPI.ResetMemory();
                        sendMsgToClient(FromServerOperation.SUCCESS, "memory reset!");
                        break;
                    }
                default:
                    Send("9" + "did not recognize your code!");
                    break;
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            myAppletAPI.Close(); //<<-- change this to allow server to stay alive..
        }

        private int getCommandId(string msg)
        {
            int res;
            bool success = Int32.TryParse(msg.Substring(0, CMD_ID_LENGTH), out res);
            if (!success)
                throw new EXCantGetCmdId();
            return res;
        }

        private void sendMsgToClient(FromServerOperation cmd, string msg)
        {
            string msgToSend = ((int)cmd + msg);
            Send(msgToSend);
            Console.WriteLine("-> Message sent to client: " + cmd + " " + msg);
        }

        public static string GetPortName()
        {
            return typeof(WSBehav_Applet).Name;
        }
    }
}
