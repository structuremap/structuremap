using Xunit;

namespace StructureMap.Testing.Configuration.DSL
{
    public class ProgrammaticConstructorSelectionTester
    {
        [Fact]
        public void override_constructor_selection_in_code()
        {
            var container = new Container(x =>
            {
                x.ForConcreteType<ClassWithTwoConstructors>().Configure
                    .SelectConstructor(() => new ClassWithTwoConstructors(34))
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
            Assert.True(false, "Should not be called");

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