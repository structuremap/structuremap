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

    public class ConcretePlug<T> : IPlug<T>
    {
        #region IPlug<T> Members

        public Type PlugType { get { return typeof (T); } }

        #endregion
    }

    public interface IConcept<T>
    {
    }

    public class GenericConcept<T> : IConcept<T>
    {
    }

    public class SpecificConcept : IConcept<object>
    {
    }

    public interface IThing<T, U>
    {
    }

    public class ColorThing<T, U> : IThing<T, U>
    {
        public ColorThing(string color)
        {
            Color = color;
        }

        public string Color { get; }
    }

    public class ComplexThing<T, U> : IThing<T, U>
    {
        public ComplexThing(string name, int age, bool ready)
        {
            Name = name;
            Age = age;
            Ready = ready;
        }

        public string Name { get; }

        public int Age { get; }

        public bool Ready { get; }
    }

    public abstract class AbstractClass<T>
    {
        public Guid Id { get; } = Guid.NewGuid();

        public Type GetT()
        {
            return typeof (T);
        }
    }

    public class ConcreteClass<T> : AbstractClass<T>
    {
    }

    public interface ILotsOfTemplatedTypes<T, U, V>
    {
    }

    public class LotsOfTemplatedTypes<T, U, V> : ILotsOfTemplatedTypes<T, U, V>
    {
    }
}