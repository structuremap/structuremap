using System;

namespace StructureMap.Testing.GenericWidgets
{
    public interface ISimpleThing<T>
    {
    }

    public class SimpleThing<T> : ISimpleThing<T>
    {
    }

    public interface IService<T>
    {
    }

    [Pluggable("Default")]
    public class Service<T> : IService<T>
    {
        public Type GetT()
        {
            return typeof (T);
        }
    }

    public class Service2<T> : IService<T>
    {
        public Type GetT()
        {
            return typeof (T);
        }
    }

    [Pluggable("Plugged")]
    public class ServiceWithPlug<T> : IService<T>
    {
        private readonly IPlug<T> _plug;

        public ServiceWithPlug(IPlug<T> plug)
        {
            _plug = plug;
        }


        public IPlug<T> Plug { get { return _plug; } }
    }

    public interface IPlug<T>
    {
        Type PlugType { get; }
    }

    [Pluggable("Default")]
    public class ConcretePlug<T> : IPlug<T>
    {
        #region IPlug<T> Members

        public Type PlugType { get { return typeof (T); } }

        #endregion
    }

    public interface IConcept<T>
    {
    }

    [Pluggable("Default")]
    public class GenericConcept<T> : IConcept<T>
    {
    }

    [Pluggable("Specific")]
    public class SpecificConcept : IConcept<object>
    {
    }

    public interface IThing<T, U>
    {
    }

    public class ColorThing<T, U> : IThing<T, U>
    {
        private readonly string _color;

        public ColorThing(string color)
        {
            _color = color;
        }

        public string Color { get { return _color; } }
    }

    public class ComplexThing<T, U> : IThing<T, U>
    {
        private readonly int _age;
        private readonly string _name;
        private readonly bool _ready;

        public ComplexThing(string name, int age, bool ready)
        {
            _name = name;
            _age = age;
            _ready = ready;
        }

        public string Name { get { return _name; } }

        public int Age { get { return _age; } }

        public bool Ready { get { return _ready; } }
    }

    public abstract class AbstractClass<T>
    {
        private readonly Guid _id = Guid.NewGuid();

        public Guid Id { get { return _id; } }

        public Type GetT()
        {
            return typeof (T);
        }
    }

    [Pluggable("Default")]
    public class ConcreteClass<T> : AbstractClass<T>
    {
    }

    public interface ILotsOfTemplatedTypes<T, U, V>
    {
    }

    [Pluggable("Default")]
    public class LotsOfTemplatedTypes<T, U, V> : ILotsOfTemplatedTypes<T, U, V>
    {
    }
}