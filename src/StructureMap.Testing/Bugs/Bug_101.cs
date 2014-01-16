using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    public interface ISomeInterface<in T>
    {
    }

    public class Base
    {
    }

    public class Derived : Base
    {
    }

    public class Foo : ISomeInterface<Base>
    {
    }


    [TestFixture, Ignore("til 1/17/2014")]
    public class Bug_101
    {
        [Test]
        public void open_generic_scanning()
        {
            var container = new Container(i => i.Scan(s => {
                s.AssemblyContainingType<Bug_101>();
                s.WithDefaultConventions();
                s.AddAllTypesOf(typeof (ISomeInterface<>));
            }));

            container.GetInstance<ISomeInterface<Derived>>()
                .ShouldNotBeNull();
        }
    }
}