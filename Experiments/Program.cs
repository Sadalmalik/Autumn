using System;
using Autumn.MVC;

namespace Experiments
{
    class Program
    {
        public static void Main(string[] args)
        {
            var container = new MvcContainer();

            container.Add<TestController>();
            container.AddDefaultFilesComponent();
            
            container.Start();
            
            Console.WriteLine("Press any key to stop server...");
            Console.ReadKey();
            
            container.Stop();
        }
    }
}