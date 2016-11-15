using Cellnet;
using System;

namespace UnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var ed = new EventDispatcher();

            var peer = new Connector(ed.Queue).Start("127.0.0.1:7010");
            Subscribe.RegisterMessage<gamedef.SessionConnected>(ed, (msg, ses) =>
            {
                Console.WriteLine("connected");
            });




            ed.Start();


            ed.Wait();
        }
    }
}
