using NUnit.Framework;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class nested_containers
    {
        [Test]
        public void nested_container_from_profile_container()
        {
            var container = new Container(x => {
                x.For<IColor>().Use<Red>();

                x.Profile("Blue", _ => _.For<IColor>().Use<Blue>());
                x.Profile("Green", _ => _.For<IColor>().Use<Green>());
            });

            using (var nested = container.GetProfile("Blue").GetNestedContainer())
            {
                nested.GetInstance<IColor>().ShouldBeOfType<Blue>();
            }

            using (var nested = container.GetNestedContainer("Green"))
            {
                nested.GetInstance<IColor>().ShouldBeOfType<Green>();
            }
        }
    }

    public interface IColor{}
    public class Red : IColor{} 
    public class Blue : IColor{} 
    public class Green : IColor{} 
}