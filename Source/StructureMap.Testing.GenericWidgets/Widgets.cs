using System;
using StructureMap.Attributes;

namespace StructureMap.Testing.GenericWidgets
{
    public interface ISimpleThing<T>{}
    public class SimpleThing<T> : ISimpleThing<T>{}
    
    [PluginFamily]
    public interface IService<T>{}
    
    [Pluggable("Default")]
    public class Service<T> : IService<T>
    {
        public Type GetT()
        {
            return typeof (T);
        }
    }
    
    public interface IThing<T, U, V>{}
    
    public class ColorThing<T, U, V> : IThing<T, U, V>
    {
        private readonly string _color;

        public ColorThing(string color)
        {
            _color = color;
        }

        public string Color
        {
            get { return _color; }
        }
    }
    
    public class ComplexThing<T, U, V> : IThing<T, U, V>
    {
        private readonly string _name;
        private readonly int _age;
        private readonly bool _ready;

        public ComplexThing(string name, int age, bool ready)
        {
            this._name = name;
            this._age = age;
            this._ready = ready;
        }

        public string Name
        {
            get { return _name; }
        }

        public int Age
        {
            get { return _age; }
        }

        public bool Ready
        {
            get { return _ready; }
        }
    }
    
    [PluginFamily("Default", Scope = InstanceScope.Singleton)]
    public abstract class AbstractClass<T>
    {
        private Guid _id = Guid.NewGuid();

        public Guid Id
        {
            get { return _id; }
        }
        
        public Type GetT()
        {
            return typeof (T);
        }
    }
    
    [Pluggable("Default")]
    public class ConcreteClass<T> : AbstractClass<T>{}    
    
    [PluginFamily("Default")]
    public interface ILotsOfTemplatedTypes<T,U,V>{}
    
    [Pluggable("Default")]
    public class LotsOfTemplatedTypes<T,U,V> : ILotsOfTemplatedTypes<T, U, V>{}
}
