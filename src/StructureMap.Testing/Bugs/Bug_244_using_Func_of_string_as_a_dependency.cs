using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_244_using_Func_of_string_as_a_dependency
    {
        [Test]
        public void use_a_simple_func_for_string_dependency()
        {
            var container = new Container(x => {
                x.For<Rule>().Use<ColorRule>().Ctor<string>().Is(() => "blue");
            });

            container.GetInstance<Rule>()
                .ShouldBeOfType<ColorRule>().Color.ShouldEqual("blue");
        }

        [Test]
        public void use_a_func_of_context_for_string_dependency()
        {
            var container = new Container(x => {
                x.ForConcreteType<ColorRule>().Configure.Ctor<string>().Is("fuschia");
                x.ForConcreteType<StringHolder>().Configure.Ctor<string>()
                    .Is(c => c.GetInstance<ColorRule>().Color);
            });

            container.GetInstance<StringHolder>().Name.ShouldEqual("fuschia");
        }
    }

    public class StringHolder
    {
        private readonly string _name;

        public StringHolder(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}   