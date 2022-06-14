//bsiyata d'shmaya!

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace dalProgHost
{

    class Program
    {

        static void Main(string[] args)
        {
            string msgToStop = "stop";

            string portNumber = "6130";
            WebSocketServer wssv = new WebSocketServer("ws://127.0.0.1:" + portNumber);
            wssv.AddWebSocketService<WSBehav_Applet>("/" + WSBehav_Applet.GetPortName());
            wssv.Start();
            Console.WriteLine("SERVER CONSOLE (echos all messages sent to and from the Chrome Extension:");
            Console.WriteLine("Enter \"" + msgToStop + "\" to stop");
            Console.WriteLine("WS server started on ws://127.0.0.1:" + portNumber + WSBehav_Applet.GetPortName());


            //to keep console alive..
            string x = Console.ReadLine();
            while (x != msgToStop)
                x = Console.ReadLine();
           wssv.Stop();

           Console.WriteLine("Press Enter to finish.");
           Console.Read();
        }

        static string convertByteToString(byte[] bytes)
        {
            var sb = new StringBuilder("");

            foreach (var b in bytes)
                sb.Append(b + ", ");

            return sb.ToString();
        }



    }
}