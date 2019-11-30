using System;

namespace Autumn.Test.IOC
{
    public class MyClass2 : Autumn.IOC.SharedObject
    {
        [Autumn.IOC.Inject] private MyClass0 _myClass0;
        [Autumn.IOC.Inject] private MyClass1 _myClass1;

        public override void Init()
        {
            Console.WriteLine($"MyClass2.Init() in {Container.Name}\n\t_myClass0: {_myClass0.ToString()}\n\t_myClass1: {_myClass1.ToString()}");
        }

        public override void Dispose()
        {
            Console.WriteLine($"MyClass2.Dispose() in {Container.Name}");
        }
    }
}