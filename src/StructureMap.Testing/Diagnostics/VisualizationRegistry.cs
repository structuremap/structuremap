using StructureMap.Configuration.DSL;
using StructureMap.Testing.Acceptance;

namespace StructureMap.Testing.Diagnostics
{
    public class VisualizationRegistry : Registry
    {
        public VisualizationRegistry()
        {
            For<IDevice>().Use<DefaultDevice>();

            For<IDevice>().Add(() => new ADevice()).Named("A");
            For<IDevice>().Add(new BDevice()).Named("B");


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