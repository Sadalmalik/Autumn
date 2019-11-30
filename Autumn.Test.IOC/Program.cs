using System;
using Autumn.IOC;

namespace Autumn.Test.IOC
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var baseContainer = new Container();
            baseContainer.Add<MyClass0>();
            baseContainer.Init();

            CycleSubprocess(baseContainer);
            
            CycleSubprocess(baseContainer);
            
            CycleSubprocess(baseContainer);

            baseContainer.Dispose();
        }

        private static void CycleSubprocess(Container baseContainer)
        {
            var mainContainer = new Container(baseContainer);
            mainContainer.Add<MyClass1>();
            mainContainer.Add<MyClass2>();

            mainContainer.Init();
            Console.WriteLine("\nDone, now dispose!\n");
            mainContainer.Dispose();
        }
    }
}