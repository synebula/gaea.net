using System;
using Gaea;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            IStartable startable = new Startable();
            startable.Start();
        }
    }

    class Startable : IStartable
    {

        public void Start()
        {
            Console.WriteLine("Hello World!");
        }

    }
}
