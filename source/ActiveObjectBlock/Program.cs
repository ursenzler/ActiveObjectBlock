using System;
using System.Linq;
using System.Text;

namespace ActiveObjectBlock
{
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            var w = new Worker(i => Console.WriteLine(i));


            Console.WriteLine("press key to 1) queue items  2) start  3) queue items  4) stop");
            Console.ReadLine();

            Console.WriteLine("posting items 0..9");
            for (int i = 0; i < 10; i++)
            {
                w.Post(i);
            }

            Console.WriteLine("starting................");
            w.Start();

            Console.WriteLine("posting items 10..19");
            for (int i = 10; i < 20; i++)
            {
                w.Post(i);
            }

            Thread.Sleep(18);
            Console.WriteLine("stopping...........");
            w.Stop().Wait();
            Console.WriteLine("stopped................");




            Console.WriteLine("press key to 1) start  2) queue items  3) wait 5ms  4) abort");
            Console.ReadLine();
            
            Console.WriteLine("starting................");
            w.Start();

            Console.WriteLine("posting items 100..999");
            for (int i = 100; i < 1000; i++)
            {
                w.Post(i);
            }

            Thread.Sleep(5);
            
            Console.WriteLine("aborting.......");

            w.Abort();
            Console.WriteLine("aborted................");




            Console.WriteLine("press key to 1) start  2) queue items  4) stop");
            Console.ReadLine();

            Console.WriteLine("starting................");
            w.Start();

            Console.WriteLine("posting items 2000..2009");
            for (int i = 2000; i < 2010; i++)
            {
                w.Post(i);
            }

            Console.WriteLine("press key");
            Console.ReadLine();
            
            w.Stop().Wait();
        }
    }
}
