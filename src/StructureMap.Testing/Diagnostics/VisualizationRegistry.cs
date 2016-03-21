using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.Acceptance;
using StructureMap.Testing.Widget;
using System.Collections.Generic;

namespace StructureMap.Testing.Diagnostics
{
    public class VisualizationRegistry : Registry
    {
        public VisualizationRegistry()
        {
            For<IDevice>().DecorateAllWith(x => new DeviceDecorator(x));

            For<IDevice>().DecorateAllWith<CrazyDecorator>();

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
                .Setter<long>("Age").Is(40)
                .Setter<int>("Order").Is(2);

            For<IDevice>().Add<DeviceWithArgsAndSetters>()
                .Named("MixedCtorAndSetterWithProblems")
                //.Ctor<string>("color").Is("Blue")
                .Ctor<string>("direction").Is("North")
                .Ctor<string>("name").Is("Declan")
                //.Setter<int>("Age").Is(40.01)
                .Setter<int>("Order").Is(2);

            For<IDevice>().Add<CDevice>().Named("Activated");

            //            For<IDevice>().Add<CDevice>().Named("DecoratedWithFunc")
            //                .DecorateWith(x => new DeviceDecorator(x));

            For<Activateable>().OnCreationForAll(x => x.Activate());

            For<Rule>().Use<ColorRule>().Ctor<string>("color").Is("Red").Named("Red");

            For<DeviceDecorator>().Add<DeviceDecorator>().Named("UsesDefault");

            For<DeviceDecorator>().Add<DeviceDecorator>().Named("InlineDevice")
                .Ctor<IDevice>().Is<ADevice>(x => x.Singleton());

            For<DeviceDecorator>().Add<DeviceDecorator>().Named("DeepInlineDevice")
                .Ctor<IDevice>().Is<DeviceWithArgs>(x =>
                {
                    x.Singleton();
                    x.Ctor<string>("color").Is("Blue");
                    x.Ctor<string>("direction").Is("North");
                    x.Ctor<string>("name").Is("Declan");
                });

            For<DeviceDecorator>().Add<DeviceDecorator>().Named("UsesA")
                .Ctor<IDevice>().IsNamedInstance("A");

            For<CompositeDevice>().Add<CompositeDevice>().Named("AllPossible");

            For<CompositeDevice>().Add<CompositeDevice>().Named("InlineEnumerable")
                .EnumerableOf<IDevice>().Contains(x =>
                {
                    x.Type<ADevice>();
                    x.Type<BDevice>();
                    x.Type<CDevice>();
                });

            ForConcreteType<DeviceUser>()
                .Configure
                .Ctor<IDevice>("one").Is<ADevice>()
                .Ctor<IDevice>("two").Is<BDevice>()
                .Ctor<IDevice>("three").Is<CDevice>();

            For<ClassThatHoldsINotRegistered>().Use<ClassThatHoldsINotRegistered>();

            For<DeviceDecorator>().Add<DeviceDecorator>().Named("UsesDefault");
            For<DeviceDecorator>().Add<DeviceDecorator>().Named("UsesA")
                .Ctor<IDevice>().IsNamedInstance("A");

            For<DeviceDecorator>().Add<DeviceDecorator>().Named("UsesNonExistent")
                .Ctor<IDevice>().IsNamedInstance("NonExistent");
        }
    }

    public interface IDevice
    {
    }

    public class CrazyDecorator : IDevice
    {
        public CrazyDecorator(IDevice inner, IEngine engine, IFoo foo)
        {
        }
    }

    public class DefaultDevice : IDevice
    {
    }

    public class ADevice : Activateable, IDevice
    {
    }

    public class BDevice : Activateable, IDevice
    {
    }

    public class CDevice : Activateable, IDevice
    {
    }

    public class CompositeDevice
    {
        public CompositeDevice(IEnumerable<IDevice> devices)
        {
        }
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
        public long Age { get; set; }

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

    public class DeviceUser
    {
        public DeviceUser(IDevice one, IDevice two, IDevice three)
        {
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

    public interface INotRegistered
    {
    }

    public class ClassThatHoldsINotRegistered
    {
        public ClassThatHoldsINotRegistered(INotRegistered registered)
        {
        }
    }

    public class DeviceHandler
    {
        public DeviceHandler(IDevice device)
        {
        }
    }
}