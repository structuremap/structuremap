using Shouldly;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget2;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_244_using_Func_of_string_as_a_dependency
    {
        [Fact]
        public void use_a_simple_func_for_string_dependency()
        {
            var container = new Container(x => { x.For<Rule>().Use<ColorRule>().Ctor<string>().Is(() => "blue"); });

            container.GetInstance<Rule>()
                .ShouldBeOfType<ColorRule>().Color.ShouldBe("blue");
        }

        [Fact]
        public void use_a_func_of_context_for_string_dependency()
        {
            var container = new Container(x =>
            {
                x.ForConcreteType<ColorRule>().Configure.Ctor<string>().Is("fuschia");
                x.ForConcreteType<StringHolder>().Configure.Ctor<string>()
                    .Is(c => c.GetInstance<ColorRule>().Color);
            });

            container.GetInstance<StringHolder>().Name.ShouldBe("fuschia");
        }

        [Fact]
        public void use_a_func_for_a_simple_type()
        {
            var container = new Container(x => { x.For<IntHolder>().Use<IntHolder>().Ctor<int>().Is(() => 5); });

            container.GetInstance<IntHolder>()
                .Number.ShouldBe(5);
        }

        [Fact]
        public void use_a_func_for_enums()
        {
            var container =
                new Container(
                    x => { x.For<EnumHolder>().Use<EnumHolder>().Ctor<BreedEnum>().Is(() => BreedEnum.Beefmaster); });

            // My father raises Beefmasters and there'd be
            // hell to pay if he caught me using Angus as
            // test data
            container.GetInstance<EnumHolder>()
                .Breed.ShouldBe(BreedEnum.Beefmaster);
        }
    }

    public class EnumHolder
    {
        private readonly BreedEnum _breed;

        public EnumHolder(BreedEnum breed)
        {
            _breed = breed;
        }

        public BreedEnum Breed
        {
            get { return _breed; }
        }
    }

    public class IntHolder
    {
        private readonly int _number;

        public IntHolder(int number)
        {
            _number = number;
        }

        public int Number
        {
            get { return _number; }
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