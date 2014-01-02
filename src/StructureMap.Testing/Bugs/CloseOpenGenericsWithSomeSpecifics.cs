using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class CloseOpenGenericsWithSomeSpecifics
    {
        [Test]
        public void should_handle_default_closed_and_specific_closed()
        {
            var container = new Container(x => {
                x.Scan(y => {
                    y.TheCallingAssembly();
                    y.ConnectImplementationsToTypesClosing(typeof (IAmOpenGeneric<>));
                });

                x.For(typeof (IAmOpenGeneric<>)).Use(typeof (TheClosedGeneric<>));
            });

            container.GetInstance<IAmOpenGeneric<int>>().ShouldBeOfType<TheClosedGeneric<int>>();
            container.GetInstance<IAmOpenGeneric<string>>().ShouldBeOfType<SpecificClosedGeneric>();
        }
    }

    public interface IAmOpenGeneric<T>
    {
    }

    public class TheClosedGeneric<T> : IAmOpenGeneric<T>
    {
    }

    public class SpecificClosedGeneric : TheClosedGeneric<string>
    {
    }
}