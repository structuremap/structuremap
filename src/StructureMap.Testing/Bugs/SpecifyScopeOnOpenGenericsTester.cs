using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class SpecifyScopeOnOpenGenericsTester
    {
        [Test]
        public void should_obey_scope_set_on_open_type()
        {
            var container =
                new Container(x => { x.ForSingletonOf(typeof (IOpenType<>)).Use(typeof (OpenType<>)); });

            var o1 = container.GetInstance<IOpenType<string>>();
            var o2 = container.GetInstance<IOpenType<string>>();

            o1.ShouldBeTheSameAs(o2);
        }
    }

    public interface IOpenType<T>
    {
    }

    public class OpenType<T> : IOpenType<T>
    {
    }
}