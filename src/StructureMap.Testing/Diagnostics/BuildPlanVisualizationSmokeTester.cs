using StructureMap.Query;
using StructureMap.Testing.Widget;
using System.Diagnostics;
using Xunit;

namespace StructureMap.Testing.Diagnostics
{
    public class when_building_a_build_plan_for_a_single_instance
    {
        private string theDescription;

        public when_building_a_build_plan_for_a_single_instance()
        {
            var container = Container.For<VisualizationRegistry>();
            theDescription = container.Model.For<IDevice>()
                .Default.DescribeBuildPlan();
        }

        [Fact]
        public void should_specify_the_plugin_type()
        {
            theDescription.ShouldContain("PluginType: StructureMap.Testing.Diagnostics.IDevice");
        }

        [Fact]
        public void should_display_the_lifecycle()
        {
            theDescription.ShouldContain("Lifecycle: Transient");
        }
    }

    public class Examples
    {
        [Fact]
        public void deep_build_plan_for_default_of_type()
        {
            // SAMPLE: build-plan-deep-for-default
            var container = Container.For<VisualizationRegistry>();

            var description = container.Model.For<IDevice>().Default
                .DescribeBuildPlan();

            Debug.WriteLine(description);
            // ENDSAMPLE
        }
    }

    public class BuildPlanVisualizationSmokeTester
    {
        private readonly IContainer theContainer = Container.For<VisualizationRegistry>();

        [Fact]
        public void no_arg_constructor()
        {
            var description = theContainer.Model.For<IDevice>()
                .Default.DescribeBuildPlan();

            Debug.WriteLine(description);

            description.ShouldContain("new DefaultDevice()");
        }

        [Fact]
        public void simple_build_by_lambda()
        {
            // SAMPLE: build-plan-by-name
            var description = theContainer.Model.For<IDevice>()
                .Find("A")
                .DescribeBuildPlan();
            // ENDSAMPLE

            Debug.WriteLine(description);

            description.ShouldContain("Lambda: new ADevice()");
        }

        [Fact]
        public void simple_build_by_object()
        {
            var description = theContainer.Model.For<IDevice>()
                .Find("B")
                .DescribeBuildPlan();

            description.ShouldContain("Value: StructureMap.Testing.Diagnostics.BDevice");
        }

        [Fact]
        public void single_ctor_arg_with_constant()
        {
            var description = theContainer.Model
                .Find<Rule>("Red")
                .DescribeBuildPlan();

            Debug.WriteLine(description);
            description.ShouldContain("┗ String color = Value: Red");
        }

        [Fact]
        public void multiple_ctor_args_with_constants()
        {
            var description = theContainer.Model
                .Find<IDevice>("GoodSimpleArgs")
                .DescribeBuildPlan();

            Debug.WriteLine(description);

            description.ShouldContain("┣ String color = Value: Blue");
            description.ShouldContain("┣ String direction = Value: North");
            description.ShouldContain("┗ String name = Value: Declan");
        }

        [Fact]
        public void multiple_ctor_args_and_setters_with_constants()
        {
            var description = theContainer.Model
                .Find<IDevice>("MixedCtorAndSetter")
                .DescribeBuildPlan();

            Debug.WriteLine(description);

            description.ShouldContain("┣ String color = Value: Blue");
            description.ShouldContain("┣ String direction = Value: North");
            description.ShouldContain("┗ String name = Value: Declan");
        }

        [Fact]
        public void activator_interceptor()
        {
            var description = theContainer.Model
                .Find<IDevice>("Activated")
                .DescribeBuildPlan();

            Debug.WriteLine(description);

            description.ShouldContain(
                "ActivatorInterceptor of StructureMap.Testing.Acceptance.Activateable: Activateable.Activate()");
        }

        [Fact]
        public void inline_default_with_flat_visualization()
        {
            var description = theContainer.Model
                .Find<DeviceDecorator>("UsesDefault")
                .DescribeBuildPlan();

            Debug.WriteLine(description);

            description.ShouldContain("┗ IDevice = **Default**");
        }

        [Fact]
        public void show_problems_in_flat_visualization()
        {
            var ex = Exception<StructureMapBuildPlanException>.ShouldBeThrownBy(() =>
            {
                theContainer.Model
                    .Find<IDevice>("MixedCtorAndSetterWithProblems")
                    .DescribeBuildPlan();
            });

            Debug.WriteLine(ex.Context);

            ex.Context.ShouldContain("┣ String color = Required primitive dependency is not explicitly defined");

            ex.Context.ShouldContain("Set Int64 Age = Required primitive dependency is not explicitly defined");
        }

        [Fact]
        public void inline_referenced_dependency()
        {
            var description = theContainer.Model
                .Find<DeviceDecorator>("UsesA")
                .DescribeBuildPlan();

            Debug.WriteLine(description);

            description.ShouldContain("┗ IDevice = Instance named 'A'");
        }

        [Fact]
        public void inline_all_possible_enumeration()
        {
            var description = theContainer.Model
                .Find<CompositeDevice>("AllPossible")
                .DescribeBuildPlan();

            Debug.WriteLine(description);

            description.ShouldContain(
                "┗ IEnumerable<IDevice> = All registered Instances of StructureMap.Testing.Diagnostics.IDevice");
        }

        [Fact]
        public void inlined_constructor_dependency_simple()
        {
            var description = theContainer.Model
                .Find<DeviceDecorator>("InlineDevice")
                .DescribeBuildPlan();

            Debug.WriteLine(description);
        }

        [Fact]
        public void inlined_constructor_dependency_complex()
        {
            var description = theContainer.Model
                .Find<DeviceDecorator>("DeepInlineDevice")
                .DescribeBuildPlan();

            Debug.WriteLine(description);
        }

        [Fact]
        public void multiple_inlined_constructor_dependencies()
        {
            var description = theContainer.Model
                .For<DeviceUser>().Default
                .DescribeBuildPlan();

            Debug.WriteLine(description);
        }

        [Fact]
        public void inline_enumerable_instance_dependency()
        {
            var description = theContainer.Model
                .Find<CompositeDevice>("InlineEnumerable")
                .DescribeBuildPlan();

            Debug.WriteLine(description);
        }

        [Fact]
        public void deep_visualization_with_a_default_that_is_not_registered()
        {
            var description = theContainer.Model
                .For<ClassThatHoldsINotRegistered>()
                .Default
                .DescribeBuildPlan(1);

            description.ShouldContain("NO DEFAULT IS SPECIFIED AND CANNOT BE AUTOMATICALLY DETERMINED");

            Debug.WriteLine(description);
        }

        [Fact]
        public void deep_visualization_with_a_default_that_can_be_found()
        {
            var description = theContainer.Model
                .Find<DeviceDecorator>("UsesDefault")
                .DescribeBuildPlan(1);

            description.ShouldNotContain("NO DEFAULT IS SPECIFIED AND CANNOT BE AUTOMATICALLY DETERMINED");

            Debug.WriteLine(description);
        }

        [Fact]
        public void deep_visualization_with_a_reference_that_cannot_be_found()
        {
            var description = theContainer.Model
                .Find<DeviceDecorator>("UsesNonExistent")
                .DescribeBuildPlan(1);

            description.ShouldContain("NO SUCH INSTANCE IS REGISTERED FOR THIS NAME");

            Debug.WriteLine(description);
        }
    }
}