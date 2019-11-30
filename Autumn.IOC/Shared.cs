namespace Autumn.IOC
{
    public interface IShared
    {
        Container Container { get; }
        void Init();
        void Dispose();
    }
    
    public class SharedObject : IShared
    {
        public Container Container { get; protected set; }

        public virtual void Init()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}