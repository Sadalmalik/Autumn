using System;

namespace Autumn.Test.IOC
{
    public class MyClass0 : Autumn.IOC.SharedObject
    {
        public override void Init()
        {
            Console.WriteLine($"MyClass0.Init() in {Container.Name}");
        }

        public override void Dispose()
        {
            Console.WriteLine($"MyClass0.Dispose() in {Container.Name}");
        }
    }
}