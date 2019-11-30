using System;

namespace Autumn.Test.IOC
{
    public class MyClass1 : Autumn.IOC.SharedObject
    {
        [Autumn.IOC.Inject] private MyClass0 _myClass0;
        [Autumn.IOC.Inject] private MyClass2 _myClass2;

        public override void Init()
        {
            Console.WriteLine($"MyClass1.Init() in {Container.Name}\n\t_myClass0: {_myClass0.ToString()}\n\t_myClass2: {_myClass2.ToString()}");
        }

        public override void Dispose()
        {
            Console.WriteLine($"MyClass1.Dispose() in {Container.Name}");
        }
    }
}