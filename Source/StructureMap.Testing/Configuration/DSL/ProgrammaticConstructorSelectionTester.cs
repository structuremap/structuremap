using NUnit.Framework;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class ProgrammaticConstructorSelectionTester
    {
        [Test]
        public void override_constructor_selection_in_code()
        {
            var container = new Container(x =>
            {
                x.SelectConstructor<ClassWithTwoConstructors>(()=>new ClassWithTwoConstructors(0));
                x.ForConcreteType<ClassWithTwoConstructors>().Configure
                    .WithCtorArg("age").EqualTo(34);
            });

            container.GetInstance<ClassWithTwoConstructors>().WasConstructedWithNarrowCtor.ShouldBeTrue();
        }
    }

    public class ClassWithTwoConstructors
    {
        private int _age;
        private string _name;

        public ClassWithTwoConstructors(int age, string name)
        {
            Assert.Fail("Should not be called");

            _age = age;
            _name = name;
        }

        public bool WasConstructedWithNarrowCtor = false;
        public ClassWithTwoConstructors(int age)
        {
            _age = age;
            WasConstructedWithNarrowCtor = true;
        }
    }
}