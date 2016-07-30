using Shouldly;
using StructureMap.Attributes;
using StructureMap.Building;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Graph;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget2;
using StructureMap.Testing.Widget3;
using StructureMap.TypeRules;
using System.Collections.Generic;
using NSubstitute;
using Xunit;

namespace StructureMap.Testing.Building
{
    public class ConcreteTypeTester
    {
        [Fact]
        public void if_a_dependency_value_is_IDependencySource_just_use_that()
        {
            var source = Substitute.For<IDependencySource>();
            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeArg", typeof(IGateway), source)
                .ShouldBeTheSameAs(source);
        }

        [Fact]
        public void no_value_for_non_simple_resolves_to_default_source()
        {
            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(IGateway), null)
                .ShouldBeOfType<DefaultDependencySource>()
                .DependencyType.ShouldBe(typeof(IGateway));
        }

        [Fact]
        public void value_is_instance_for_non_simple_resolves_to_lifecycle_source()
        {
            var instance = new FakeInstance();
            instance.SetLifecycleTo(new SingletonLifecycle());

            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(IGateway), instance)
                .ShouldBeTheSameAs(instance.DependencySource);
        }

        [Fact]
        public void if_value_exists_and_it_is_the_right_type_return_constant()
        {
            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(string), "foo")
                .ShouldBe(Constant.For("foo"));

            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(int), 42)
                .ShouldBe(Constant.For(42));

            // My dad raises registered Beefmasters and he'd be disappointed
            // if the default here was anything else
            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(BreedEnum),
                BreedEnum.Beefmaster)
                .ShouldBe(Constant.For(BreedEnum.Beefmaster));

            var gateway = new StubbedGateway();
            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(IGateway), gateway)
                .ShouldBe(Constant.For<IGateway>(gateway));
        }

        [Fact]
        public void if_list_value_exists_use_that()
        {
            var list = new List<IGateway> { new StubbedGateway(), new StubbedGateway() };
            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(List<IGateway>), list)
                .ShouldBe(Constant.For(list));

            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(IList<IGateway>), list)
                .ShouldBe(Constant.For<IList<IGateway>>(list));
        }

        [Fact]
        public void coerce_simple_numbers()
        {
            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(int), "42")
                .ShouldBe(Constant.For(42));
        }

        [Fact]
        public void coerce_enum()
        {
            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(BreedEnum), "Angus")
                .ShouldBe(Constant.For(BreedEnum.Angus));
        }

        [Fact]
        public void array_can_be_coerced_to_concrete_list()
        {
            var array = new IGateway[] { new StubbedGateway(), new StubbedGateway() };
            var constant =
                ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(List<IGateway>), array)
                    .ShouldBeOfType<Constant>();

            constant.ReturnedType.ShouldBe(typeof(List<IGateway>));
            constant.Value.As<List<IGateway>>()
                .ShouldHaveTheSameElementsAs(array);
        }

        [Fact]
        public void array_can_be_coerced_to_concrete_ilist()
        {
            var array = new IGateway[] { new StubbedGateway(), new StubbedGateway() };
            var constant =
                ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(IList<IGateway>), array)
                    .ShouldBeOfType<Constant>();

            constant.ReturnedType.ShouldBe(typeof(IList<IGateway>));
            constant.Value.As<IList<IGateway>>()
                .ShouldHaveTheSameElementsAs(array);
        }

        [Fact]
        public void array_can_be_coerced_to_enumerable()
        {
            var list = new IGateway[] { new StubbedGateway(), new StubbedGateway() };
            var constant =
                ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(List<IGateway>), list)
                    .ShouldBeOfType<Constant>();

            constant.ReturnedType.ShouldBe(typeof(List<IGateway>));
            constant.Value.As<List<IGateway>>()
                .ShouldHaveTheSameElementsAs(list);
        }

        [Fact]
        public void list_can_be_coerced_to_array()
        {
            var list = new List<IGateway> { new StubbedGateway(), new StubbedGateway() };
            var constant =
                ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(IGateway[]), list)
                    .ShouldBeOfType<Constant>();

            constant.ReturnedType.ShouldBe(typeof(IGateway[]));
            constant.Value.As<IGateway[]>()
                .ShouldHaveTheSameElementsAs(list.ToArray());
        }

        [Fact]
        public void use_all_possible_for_array()
        {
            var enumerableType = typeof(IGateway[]);
            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", enumerableType, null)
                .ShouldBe(new AllPossibleValuesDependencySource(enumerableType));
        }

        [Fact]
        public void use_all_possible_for_ienumerable()
        {
            var enumerableType = typeof(IEnumerable<IGateway>);
            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", enumerableType, null)
                .ShouldBe(new AllPossibleValuesDependencySource(enumerableType));
        }

        [Fact]
        public void use_all_possible_for_ilist()
        {
            var enumerableType = typeof(IList<IGateway>);
            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", enumerableType, null)
                .ShouldBe(new AllPossibleValuesDependencySource(enumerableType));
        }

        [Fact]
        public void use_all_possible_for_list()
        {
            var enumerableType = typeof(List<IGateway>);
            ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", enumerableType, null)
                .ShouldBe(new AllPossibleValuesDependencySource(enumerableType));
        }

        [Fact]
        public void source_for_missing_string_constructor_arg()
        {
            var source = ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeProp", typeof(string), null)
                .ShouldBeOfType<DependencyProblem>();

            source.Name.ShouldBe("SomeProp");
            source.Type.ShouldBe(ConcreteType.ConstructorArgument);
            source.Message.ShouldBe("Required primitive dependency is not explicitly defined");
            source.ReturnedType.ShouldBe(typeof(string));
        }

        [Fact]
        public void source_for_missing_string_setter_arg()
        {
            var source = ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.SetterProperty, "SomeProp", typeof(string), null)
                .ShouldBeOfType<DependencyProblem>();

            source.Name.ShouldBe("SomeProp");
            source.Type.ShouldBe(ConcreteType.SetterProperty);
            source.Message.ShouldBe("Required primitive dependency is not explicitly defined");
            source.ReturnedType.ShouldBe(typeof(string));
        }

        [Fact]
        public void unable_to_determine_a_dependency()
        {
            var colorRule = new ColorRule("Red");
            var source = ConcreteType.SourceFor(
                new Policies(PluginGraph.CreateRoot()),
                ConcreteType.SetterProperty,
                "SomeProp",
                typeof(IGateway),
                colorRule)
                .ShouldBeOfType<DependencyProblem>();

            source.Name.ShouldBe("SomeProp");
            source.Type.ShouldBe(ConcreteType.SetterProperty);
            source.ReturnedType.ShouldBe(typeof(IGateway));
            source.Message.ShouldBe(ConcreteType.UnableToDetermineDependency.ToFormat(
                typeof(IGateway).GetFullName(), colorRule));
        }

        [Fact]
        public void source_for_conversion_problem()
        {
            var source = ConcreteType.SourceFor(new Policies(PluginGraph.CreateRoot()), ConcreteType.ConstructorArgument, "SomeArg", typeof(int), "foo")
                .ShouldBeOfType<DependencyProblem>();

            source.Name.ShouldBe("SomeArg");
            source.Type.ShouldBe(ConcreteType.ConstructorArgument);
            source.ReturnedType.ShouldBe(typeof(int));
            source.Message.ShouldBe(ConcreteType.CastingError.ToFormat("foo", typeof(string).GetFullName(),
                typeof(int).GetFullName()));
        }

        [Fact]
        public void throw_a_description_exception_when_no_suitable_ctor_can_be_found()
        {
            var ex = Exception<StructureMapConfigurationException>.ShouldBeThrownBy(() =>
            {
                ConcreteType.BuildConstructorStep(typeof(GuyWithNoSuitableCtor), null, new DependencyCollection(),
                    Policies.Default());
            });

            ex.Message.ShouldContain("No public constructor could be selected for concrete type " +
                                     typeof(GuyWithNoSuitableCtor).GetFullName());
        }

        [Fact]
        public void is_valid_happy_path_with_ctor_checks()
        {
            // This class needs all three of these things
            var dependencies = new DependencyCollection();
            dependencies.Add("name", "Jeremy");
            dependencies.Add("age", 40);
            dependencies.Add("isAwake", true);

            ConcreteType.BuildSource(typeof(GuyWithPrimitives), null, dependencies, Policies.Default())
                .IsValid().ShouldBeTrue();
        }

        [Fact]
        public void is_valid_sad_path_with_ctor_checks()
        {
            // This class needs all three of these things
            var dependencies = new DependencyCollection();
            dependencies.Add("name", "Jeremy");
            dependencies.Add("age", 40);
            //dependencies.Add("isAwake", true);

            ConcreteType.BuildSource(typeof(GuyWithPrimitives), null, dependencies, Policies.Default())
                .IsValid().ShouldBeFalse();
        }

        [Fact]
        public void is_valid_happy_path_with_setter_checks()
        {
            // This class needs all three of these things
            var dependencies = new DependencyCollection();
            dependencies.Add("Name", "Jeremy");
            dependencies.Add("Age", 40);
            dependencies.Add("IsAwake", true);

            ConcreteType.BuildSource(typeof(GuyWithPrimitiveSetters), null, dependencies, Policies.Default())
                .IsValid().ShouldBeTrue();
        }

        [Fact]
        public void is_valid_sad_path_with_setter_checks()
        {
            // This class needs all three of these things
            var dependencies = new DependencyCollection();
            dependencies.Add("Name", "Jeremy");
            dependencies.Add("Age", 40);
            //dependencies.Add("IsAwake", true);

            ConcreteType.BuildSource(typeof(GuyWithPrimitiveSetters), null, dependencies, Policies.Default())
                .IsValid().ShouldBeFalse();
        }
    }

    public class GuyWithNoSuitableCtor
    {
        private GuyWithNoSuitableCtor()
        {
        }
    }

    public class GuyWithPrimitives
    {
        private readonly string _name;
        private readonly int _age;
        private readonly bool _isAwake;

        public GuyWithPrimitives(string name, int age, bool isAwake)
        {
            _name = name;
            _age = age;
            _isAwake = isAwake;
        }
    }

    public class GuyWithPrimitiveEverything
    {
        private readonly string _name;
        private readonly int _age;
        private readonly bool _isAwake;

        public GuyWithPrimitiveEverything(string name, int age, bool isAwake)
        {
            _name = name;
            _age = age;
            _isAwake = isAwake;
        }

        [SetterProperty]
        public string Direction { get; set; }

        [SetterProperty]
        public string Description { get; set; }
    }

    public class GuyWithPrimitiveSetters
    {
        [SetterProperty]
        public string Name { get; set; }

        [SetterProperty]
        public int Age { get; set; }

        [SetterProperty]
        public bool IsAwake { get; set; }
    }
}