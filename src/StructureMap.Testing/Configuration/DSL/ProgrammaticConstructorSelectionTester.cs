using NUnit.Framework;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class ProgrammaticConstructorSelectionTester
    {
        [Test, Ignore("Pending a fix to GH-74")]
        public void override_constructor_selection_in_code()
        {
            var container = new Container(x => {
                x.ForConcreteType<ClassWithTwoConstructors>().Configure
                    .Ctor<int>("age").Is(34);
            });

            container.GetInstance<ClassWithTwoConstructors>().WasConstructedWithNarrowCtor.ShouldBeTrue();
        }
    }

    public class ClassWithTwoConstructors
    {
        private int _age;
        private string _name;

        public bool WasConstructedWithNarrowCtor;

        public ClassWithTwoConstructors(int age, string name)
        {
            Assert.Fail("Should not be called");

            _age = age;
            _name = name;
        }

        public ClassWithTwoConstructors(int age)
        {
            _age = age;
            WasConstructedWithNarrowCtor = true;
        }
    }
}