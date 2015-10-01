using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Shouldly;
using StructureMap.Pipeline;
using StructureMap.Testing.Diagnostics;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Examples
{
    [TestFixture]
    public class UsingContainerModel
    {
        [Test]
        public void finding_things()
        {
            var container = Container.For<VisualizationRegistry>();

            // SAMPLE: find-all-plugin-types-from-the-current-assembly
            container.Model.PluginTypes.Where(x => x.PluginType.Assembly == Assembly.GetExecutingAssembly())
                .Each(pluginType => Debug.WriteLine(pluginType.PluginType));
            // ENDSAMPLE


            // SAMPLE: find-default-of-plugintype
            // Finding the concrete type of the default
            // IDevice service
            container.Model.DefaultTypeFor<IDevice>()
                .ShouldBe(typeof (DefaultDevice));

            // Find the configuration model for the default
            // IDevice service
            container.Model.For<IDevice>().Default
                .ReturnedType.ShouldBe(typeof (DefaultDevice));
            // ENDSAMPLE

            // SAMPLE: find-named-instance-by-type-and-name
            var redRule = container.Model.Find<Rule>("Red");
            // ENDSAMPLE

            // SAMPLE: query-instances-of-plugintype
            container.Model.For<Rule>().Instances.Each(i =>
            {
                Debug.WriteLine(i.Instance.Description);
            });
            // ENDSAMPLE

            // SAMPLE: eject-and-remove-configuration
            // Removes all configurations and objects already built as singletons 
            // related to types in the current Assembly
            container.Model.EjectAndRemoveTypes(type => type.Assembly == Assembly.GetExecutingAssembly());

            // Removes all configurations and objects already built as singletons
            // that were registered to IDevice
            container.Model.EjectAndRemove(typeof(IDevice));
            // ENDSAMPLE

            // SAMPLE: eject-an-object
            // ONLY ejects any built object for this Instance from the singleton
            // cache
            container.Model.For<IDevice>().Default.EjectObject();
            // ENDSAMPLE


            // SAMPLE: testing-for-registrations
            // Is there a default instance for IDevice?
            container.Model.HasDefaultImplementationFor<IDevice>().ShouldBeTrue();

            // Are there any configured instances for IDevice?
            container.Model.HasImplementationsFor<IDevice>().ShouldBeTrue();
            // ENDSAMPLE

            // SAMPLE: working-with-single-instance-ref
            // First, find the model for a single Instance
            var instance = container.Model.For<IDevice>().Default;

            // build or resolve an object for this Instance cast to
            // the type specified to the Get() method
            instance.Get<IDevice>().ShouldBeOfType<DefaultDevice>();

            // if the instance is configured as a singleton, test
            // if the singleton object has already been created
            var hasSingletonBeenCreated = instance.ObjectHasBeenCreated();

            if (hasSingletonBeenCreated)
            {
                // remove the singleton object from the cache so that
                // StructureMap will be forced to rebuild this object
                instance.EjectObject();
            }

            // test the lifecycle of this instance
            instance.Lifecycle.ShouldBeOfType<SingletonLifecycle>();

            // Visualize the build plan no more than 3 levels deep
            Debug.WriteLine(instance.DescribeBuildPlan(3));

            // Get at the underlying Instance model that StructureMap itself
            // uses. Be cautious using this.
            var rawModel = instance.Instance;

            // ENDSAMPLE
        }
    }
}