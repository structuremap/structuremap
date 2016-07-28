using Shouldly;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;
using StructureMap.Testing.Samples;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Acceptance
{
    public class iconfigured_instance_behavior
    {
        [Fact]
        public void set_to_singleton()
        {
            // SAMPLE: set-iconfigured-instance-to-SingletonThing
            IConfiguredInstance instance
                = new ConfiguredInstance(typeof(WidgetHolder));

            instance.Singleton();

            instance.Lifecycle.ShouldBeOfType<SingletonLifecycle>();
            // ENDSAMPLE
        }

        [Fact]
        public void set_to_default()
        {
            // SAMPLE: iconfiguredinstance-lifecycle
            IConfiguredInstance instance
                = new ConfiguredInstance(typeof(WidgetHolder));

            // Use the SingletonThing lifecycle
            instance.Singleton();

            // or supply an ILifecycle type
            instance.SetLifecycleTo<ThreadLocalStorageLifecycle>();

            // or supply an ILifecycle object
            instance.SetLifecycleTo(new Lifecycles_Samples.MyCustomLifecycle());

            // or override to the default "transient" lifecycle
            instance.DefaultLifecycle();
            // ENDSAMPLE

            instance.Lifecycle
                .ShouldBeOfType<TransientLifecycle>();
        }

        // SAMPLE: reflecting-over-parameters
        public class GuyWithArguments
        {
            public GuyWithArguments(IWidget widget, Rule rule)
            {
            }
        }

        [Fact]
        public void reflecting_over_constructor_args()
        {
            IConfiguredInstance instance = new SmartInstance<GuyWithArguments>()
                // I'm just forcing it to assign the constructor function
                .SelectConstructor(() => new GuyWithArguments(null, null));

            instance.Constructor.GetParameters().Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("widget", "rule");
        }

        // ENDSAMPLE

        // SAMPLE: iconfiguredinstance-getsettableproperties
        public class GuyWithProperties
        {
            public IWidget Widget { get; set; }
            public Rule Rule { get; private set; }
        }

        [Fact]
        public void get_settable_properties()
        {
            IConfiguredInstance instance
                = new ConfiguredInstance(typeof(GuyWithProperties));

            instance.SettableProperties()
                .Single().Name.ShouldBe("Widget");
        }

        // ENDSAMPLE

        // SAMPLE: add-dependency-by-property-info
        [Fact]
        public void dependency_with_setter_with_value()
        {
            var instance
                = new ConfiguredInstance(typeof(GuyWithProperties));
            var prop = instance.PluggedType.GetProperty("Widget");

            var myWidget = new ColorWidget("red");
            instance.Dependencies.AddForProperty(prop, myWidget);

            var container = new Container();

            container.GetInstance<GuyWithProperties>(instance)
                .Widget.ShouldBeTheSameAs(myWidget);
        }

        // ENDSAMPLE

        // SAMPLE: add-dependency-by-property-info-with-instance
        [Fact]
        public void dependency_with_setter_with_instance()
        {
            var instance
                = new ConfiguredInstance(typeof(GuyWithProperties));
            var prop = instance.PluggedType.GetProperty("Widget");

            var dependency = new SmartInstance<AWidget>();
            instance.Dependencies.AddForProperty(prop, dependency);

            var container = new Container();

            container.GetInstance<GuyWithProperties>(instance)
                .Widget.ShouldBeOfType<AWidget>();
        }

        // ENDSAMPLE

        // SAMPLE: add-dependency-by-constructor-parameter
        public class GuyWithDatabaseConnection
        {
            public string ConnectionString { get; set; }

            public GuyWithDatabaseConnection(string connectionString)
            {
                ConnectionString = connectionString;
            }
        }

        [Fact]
        public void specify_dependency_by_constructor_parameter()
        {
            var instance = ConstructorInstance
                .For<GuyWithDatabaseConnection>();

            var parameter = instance.Constructor.GetParameters().Single();
            parameter.Name.ShouldBe("connectionString");

            var connString =
                "I haven't used sql server in years and I don't remember what connection strings look like";

            instance.Dependencies.AddForConstructorParameter(parameter, connString);

            var guy = new Container().GetInstance<GuyWithDatabaseConnection>(instance);

            guy.ConnectionString.ShouldBe(connString);
        }

        // ENDSAMPLE

        // SAMPLE: add-interceptor-to-iconfigured-instance

        public class SimpleWidget
        {
            public bool WasIntercepted = false;

            public void Intercept()
            {
                WasIntercepted = true;
            }
        }

        [Fact]
        public void add_interceptor()
        {
            var interceptor =
                new ActivatorInterceptor<SimpleWidget>(w => w.Intercept());
            var instance = new SmartInstance<SimpleWidget>();

            instance.AddInterceptor(interceptor);

            new Container().GetInstance<SimpleWidget>(instance)
                .WasIntercepted.ShouldBeTrue();
        }

        // ENDSAMPLE
    }
}