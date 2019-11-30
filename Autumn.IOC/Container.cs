using System;
using System.Collections.Generic;
using System.Reflection;

namespace Autumn.IOC
{
    public class Container
    {
        private static int _counter = 0;
        private Dictionary<Type, IShared> _shareds;

        public string Name { get; private set; }
        public bool Active { get; private set; }
        public Container Parent { get; private set; }
        public List<Container> Childs { get; private set; }

        public Container(Container parent = null, string name = null)
        {
            Name = name != null ? Name : $"Container#{(_counter++).ToString("0000")}";

            Active = false;
            Parent = parent;
            Childs = new List<Container>();
            _shareds = new Dictionary<Type, IShared>();

            if (Parent != null)
                Parent.Childs.Add(this);
        }

        public T Add<T>() where T : IShared, new()
        {
            Type type = typeof(T);
            if (_shareds.ContainsKey(type))
                throw new ContainerTypeException($"Container already contains element of type '{type.Name}'");

            T shared = new T();
            _shareds.Add(type, shared);
            return shared;
        }

        public T Add<T>(T shared) where T : IShared
        {
            Type type = typeof(T);
            if (_shareds.ContainsKey(type))
                throw new ContainerTypeException($"Container already contains element of type '{type.Name}'");
            _shareds.Add(type, shared);
            return shared;
        }

        public T Get<T>() where T : IShared
        {
            Type type = typeof(T);
            if (!_shareds.TryGetValue(type, out var shared))
            {
                if (Parent == null)
                    throw new ContainerTypeException($"Container doesn't contains element of type '{type.Name}'");
                return Parent.Get<T>();
            }
            return (T)shared;
        }

        public object Get(Type type)
        {
            if (!_shareds.TryGetValue(type, out var shared))
            {
                if (Parent == null)
                    throw new ContainerTypeException($"Container doesn't contains element of type '{type.Name}'");
                return Parent.Get(type);
            }
            return shared;
        }

        public void Init()
        {
            if (Parent != null && !Parent.Active)
                throw new ContainerInitializationException($"Can't initialize '{Name}': Parent container '{Parent.Name}' is not active!");

            foreach (var pair in _shareds)
            {
                IShared sharedObject = pair.Value;

                Type type = pair.Key;
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var field in fields)
                {
                    Attribute attr = Attribute.GetCustomAttribute(field, typeof(Inject));
                    if (attr != null)
                    {
                        object other = Get(field.FieldType);
                        field.SetValue(sharedObject, other);
                    }
                }

                {
                    var field = type.GetProperty("Container",
                        BindingFlags.GetProperty | BindingFlags.SetProperty |
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field != null)
                        field.SetValue(sharedObject, this);
                }
            }

            foreach (var pair in _shareds)
            {
                var shared = pair.Value;
                shared.Init();
            }

            Active = true;
        }

        public void Dispose()
        {
            foreach (var child in Childs)
                if (!child.Active)
                    throw new ContainerInitializationException($"Can't dispose '{Name}': Child container '{child.Name}' is active!");

            Active = false;

            foreach (var pair in _shareds)
            {
                var shared = pair.Value;
                shared.Dispose();
            }

            if (Parent != null)
                Parent.Childs.Remove(this);
        }
    }
}