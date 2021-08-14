using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketLibrary
{
    class Program
    {
        private static bool isExit = false;
        static SokClient sokClient = new SokClient();
        static void Main(string[] args)
        {
            if (sokClient.Connect("127.0.0.1", "7001"))
            {
                Thread th = new Thread(() => Recive());
                th.IsBackground = true;
                th.Start();
                th = new Thread(() => Send());
                th.IsBackground = true;
                th.Start();
            }
            while (true)
            {
                if (isExit)
                {
                    sokClient.DisConnect();
                    break;
                }
            }
        }
        /// <summary>
        /// 循环接受
        /// </summary>
        private static void Recive()
        {
            while (true)
            {
                string msg = sokClient.ReciveMessage(1);
                Console.WriteLine(msg);
            }
        }
        /// <summary>
        /// 循环发送
        /// </summary>
        private static void Send() {
            while (true)
            {
                string msg = Console.ReadLine();
                if (msg == "EXIT")
                {
                    isExit = true;
                    break;
                }
                sokClient.SendMessage(msg, 1);
            }
        }
    }
}
