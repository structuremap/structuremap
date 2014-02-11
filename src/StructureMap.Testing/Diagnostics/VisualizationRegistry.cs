using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.Acceptance;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Diagnostics
{
    public class VisualizationRegistry : Registry
    {
        public VisualizationRegistry()
        {
            For<IDevice>().Use<DefaultDevice>();

            For<IDevice>().Add(() => new ADevice()).Named("A");
            For<IDevice>().Add(new BDevice()).Named("B");

            For<IDevice>().Add<DeviceWithArgs>()
                .Named("GoodSimpleArgs")
                .Ctor<string>("color").Is("Blue")
                .Ctor<string>("direction").Is("North")
                .Ctor<string>("name").Is("Declan");

            For<IDevice>().Add<DeviceWithArgsAndSetters>()
                .Named("MixedCtorAndSetter")
                .Ctor<string>("color").Is("Blue")
                .Ctor<string>("direction").Is("North")
                .Ctor<string>("name").Is("Declan")
                .Setter<int>("Age").Is(40)
                .Setter<int>("Order").Is(2);


            For<Rule>().Use<ColorRule>().Ctor<string>("color").Is("Red").Named("Red");
        }
    }

    public interface IDevice{}

    public class DefaultDevice : IDevice { }

    public class ADevice : Activateable, IDevice
    {

    }

    public class BDevice : Activateable, IDevice
    {

    }

    public class CDevice : Activateable, IDevice
    {

    }

    public class DeviceWithArgs : IDevice
    {
        public DeviceWithArgs(string color, string direction, string name)
        {
        }
    }

    public class DeviceWithArgsAndSetters : DeviceWithArgs
    {
        public DeviceWithArgsAndSetters(string color, string direction, string name) : base(color, direction, name)
        {
        }

        [SetterProperty]
        public int Age { get;set; }
    
        [SetterProperty]
        public int Order { get; set; }
    }



    public class DeviceWrapper
    {
        public IDevice Wrap(IDevice Device)
        {
            return new DeviceDecorator(Device);
        }
    }

    public class DeviceDecorator : IDevice
    {
        private readonly IDevice _inner;

        public DeviceDecorator(IDevice inner)
        {
            _inner = inner;
        }

        public IDevice Inner
        {
            get { return _inner; }
        }
    }
}