using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intel.Dal;

namespace dalProgHost
{
    //this class alllows the host to interact with the Applet
    class AppletAPI
    {

        static public JhiSession session;
        static public Jhi jhi;

        string appletID = "2c3f685b-00e4-4fd9-8dfd-58a102dd4fac";

        //for eyal:
        //string appletPath = @"C:\Users\seyal\Documents\dal\AppletFinalProg\bin\AppletFinalProg.dalp";
        //for david:
        string appletPath = @"C:\Users\dyyb1\OneDrive\Documentos\A_Mechon_Lev_Programs\V7_BEH\V7_Applet\V7_passwordVault\bin\V7_passwordVault.dalp";
        //david - for debugging:
        //string appletPath = @"C:\Users\dyyb1\OneDrive\Documentos\A_Mechon_Lev_Programs\V7_BEH\V7_Applet\V7_passwordVault\bin\V7_passwordVault-debug.dalp";

        public AppletAPI() //assigns two fields - session and jhi
        {
#if AMULET
            Jhi.DisableDllValidation = true;
#endif

            jhi = Jhi.Instance;

            // Install the Trusted Application
            Console.WriteLine("\nInstalling the applet.");
            jhi.Install(appletID, appletPath);

            // Start a session with the Trusted Application
            byte[] initBuffer = new byte[] { }; // Data to send to the applet onInit function
            Console.WriteLine("Opening a session.");
            jhi.CreateSession(appletID, JHI_SESSION_FLAGS.None, initBuffer, out session);
        }

        public void Close()
        {
            // Close the session
            Console.WriteLine("Closing the session.");
            jhi.CloseSession(session);

            //Uninstall the Trusted Application
            Console.WriteLine("Uninstalling the applet.\n");
            jhi.Uninstall(appletID);
        }

        public void ResetMemory()
        {
            byte[] recvBuff = new byte[0];
            int responseCode;
            int cmdId = (int)dalProgHost.ToAppletOperation.RESET_MEMORY;
            jhi.SendAndRecv2(session, cmdId, null, ref recvBuff, out responseCode);
            if (responseCode == (int)dalProgHost.FromAppletOperation.RES_SUCCESS)
                Console.WriteLine("memory reset successfully!");
        }


        public string GetPassword(string url, out bool generated)
        {
            byte[] recvBuff = new byte[100];
            int responseCode;
            byte[] bytesUrl = Encoding.ASCII.GetBytes(url);

            int cmdId = (int)dalProgHost.ToAppletOperation.RETRIEVE_PASSWORD;
            jhi.SendAndRecv2(session, cmdId, bytesUrl, ref recvBuff, out responseCode);
            //for debugging:
            generated = false;
            if (responseCode == (int)dalProgHost.FromAppletOperation.RES_NOT_SIGNED_IN)
            {
                Console.WriteLine("failed to get password - not signed in!");
                throw new EXNotSignedIn();
            }

            if (responseCode == (int)dalProgHost.FromAppletOperation.RES_PSWD_GENERATED)
            {
                Console.WriteLine("password generated...");
                generated = true;
            }
            else if (responseCode == (int)dalProgHost.FromAppletOperation.RES_PSWD_RETRIEVED)
                Console.WriteLine("password retrieved...");

            return ConvertByteArrToString(recvBuff);
        }

        public string GetAllPasswords()
        {
            byte[] recvBuff = new byte[2000];
            int responseCode;

            int cmdId = (int)dalProgHost.ToAppletOperation.RETURN_ALL_PASSWORDS;
            //receive bytes:
            jhi.SendAndRecv2(session, cmdId, null, ref recvBuff, out responseCode);
            if (responseCode == (int)dalProgHost.FromAppletOperation.RES_NOT_SIGNED_IN)
            {
                Console.WriteLine("failed to get password - not signed in!");
                throw new EXNotSignedIn();
            }

            int urlLength = 20;
            int passwordLength = 20;
            string recvBufferString = ConvertByteArrToString(recvBuff);
            //trim response code:
            //string responseCodeString = recvBufferString.Substring(0, LENGTH_OF_RESPONSE_CODE);
            //recvBufferString = recvBufferString.Substring(LENGTH_OF_RESPONSE_CODE);

            //add "\n" to the res
            string res = "";
            for (int cursor = 0; cursor < recvBufferString.Length;)
            {
                res += "\n";
                res += recvBufferString.Substring(cursor, urlLength) + ":";
                res += "\n";
                cursor += urlLength;
                res += recvBufferString.Substring(cursor, passwordLength);
                res += "\n";
                cursor += passwordLength;
            }


            return res;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="password"></param>
        /// <returns>response code</returns>
        public int SignIn(string password)
        {
            byte[] recvBuff = new byte[100];
            int responseCode;
            byte[] bytespassword = Encoding.ASCII.GetBytes(password);

            int cmdId = (int)dalProgHost.ToAppletOperation.SIGN_IN;
            jhi.SendAndRecv2(session, cmdId, bytespassword, ref recvBuff, out responseCode);

            return responseCode;
        }

        public int Register(string newPassword)
        {
            byte[] recvBuff = new byte[100];
            int responseCode;
            byte[] bytespassword = Encoding.ASCII.GetBytes(newPassword);

            int cmdId = (int)dalProgHost.ToAppletOperation.REGISTER;
            jhi.SendAndRecv2(session, cmdId, bytespassword, ref recvBuff, out responseCode);

            return responseCode;
        }


        private string ConvertByteArrToString(byte[] bytes)
        {
            //byte values can be from 0 till 255
            //only accept char values from 32 till 126!
            int lowestNum = 32;
            int highestNum = 126;
            int num;
            byte[] newArr = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                num = (int)bytes[i];
                if (num > highestNum)
                    num -= 129; //to get from 255 to 126..
                while (num < lowestNum)
                    num += lowestNum;
                newArr[i] = (byte)num;
            }

            return Encoding.ASCII.GetString(newArr);
        }
    }
}
