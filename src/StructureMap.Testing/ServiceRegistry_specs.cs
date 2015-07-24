using NUnit.Framework;
using Shouldly;

namespace StructureMap.Testing
{
    [TestFixture]
    public class ServiceRegistry_specs
    {
        [Test]
        public void set_service_if_none()
        {
            Container.ForServices(_ => _.SetServiceIfNone<Color, Blue>())
                .GetInstance<Color>().ShouldBeOfType<Blue>();
        }


        public interface Color { }

        public class Red : Color { }
        public class Blue : Color { }
        public class Yellow : Color { }
        public class Green : Color { }
        public class Purple : Color { }
    }
}