using System;

namespace StructureMap.Docs.samples
{
    public interface IBlah { }

    public interface IService
    {
        void DoSomething();
    }

    #region FooBar Model

// SAMPLE: foobar-model
    public interface IBar
    {
    }

    public class Bar : IBar
    {
    }

    public interface IFoo
    {
    }

    public class Foo : IFoo
    {
        public IBar Bar { get; private set; }

        public Foo(IBar bar)
        {
            if (bar == null) throw new ArgumentNullException("bar");
            Bar = bar;
        }
    }

// ENDSAMPLE

    public class SomeOtherFoo : IFoo
    {
    }

    #endregion
  
    #region Concrete Weather Model

// SAMPLE: concrete-weather-model
    public class Weather
    {
        public Location Location { get; set; }
        public Atmosphere Atmosphere { get; set; }
        public Wind Wind { get; set; }
        public Condition Condition { get; set; }

        public Weather(Location location, Atmosphere atmosphere, Wind wind, Condition condition)
        {
            Location = location;
            Atmosphere = atmosphere;
            Wind = wind;
            Condition = condition;
        }
    }

    public class Location
    {
        //some properties
    }

    public class Atmosphere
    {
        //some properties
    }

    public class Wind
    {
        //some properties        
    }

    public class Condition
    {
        //some properties        
    }

// ENDSAMPLE

    #endregion

}