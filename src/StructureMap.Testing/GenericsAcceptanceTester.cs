using System;
using System.Reflection;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.GenericWidgets;
using StructureMap.TypeRules;

namespace StructureMap.Testing
{
    [TestFixture]
    public class GenericsAcceptanceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void BuildFamilyAndPluginThenSealAndCreateInstanceManagerWithGenericTypeWithOpenGenericParameters()
        {
            var graph = new PluginGraph();
            graph.Scan(x => x.TheCallingAssembly());


            PluginFamily family = graph.FindFamily(typeof (IGenericService<>));
            family.DefaultInstanceKey = "Default";
            family.AddPlugin(typeof (GenericService<>), "Default");

            graph.Seal();

            var manager = new Container(graph);
        }

        [Test]
        public void CanBuildAGenericObjectThatHasAnotherGenericObjectAsAChild()
        {
            Type serviceType = typeof (IService<double>);
            PluginGraph pluginGraph = PluginGraph.BuildGraphFromAssembly(serviceType.Assembly);
            var manager = new Container(pluginGraph);

            Type doubleServiceType = typeof (IService<double>);

            var service =
                (ServiceWithPlug<double>) manager.GetInstance(doubleServiceType, "Plugged");
            Assert.AreEqual(typeof (double), service.Plug.PlugType);
        }

        [Test]
        public void CanCreatePluginFamilyForGenericTypeWithGenericParameter()
        {
            var family = new PluginFamily(typeof (IGenericService<int>));
        }

        [Test]
        public void CanCreatePluginFamilyForGenericTypeWithoutGenericParameter()
        {
            var family = new PluginFamily(typeof (IGenericService<>));
        }

        [Test]
        public void CanCreatePluginForGenericTypeWithGenericParameter()
        {
            var plugin = new Plugin(typeof (GenericService<int>));
        }

        [Test]
        public void CanCreatePluginForGenericTypeWithoutGenericParameter()
        {
            var plugin = new Plugin(typeof (GenericService<>));
        }


        [Test]
        public void CanEmitForATemplateWithTwoTemplates()
        {
            var family = new PluginFamily(typeof (ITarget<int, string>));


            family.AddPlugin(typeof (SpecificTarget<int, string>), "specific");

            var factory = new InstanceFactory(family);
        }

        [Test]
        public void CanEmitInstanceBuilderForATypeWithConstructorArguments()
        {
            var graph = new PluginGraph();
            PluginFamily family = graph.FindFamily(typeof (ComplexType<int>));
            family.AddPlugin(typeof (ComplexType<int>), "complex");

            var manager = new Container(graph);

            ConfiguredInstance instance = new ConfiguredInstance(typeof (ComplexType<int>))
                .WithProperty("name").EqualTo("Jeremy")
                .WithProperty("age").EqualTo(32);

            var com = manager.GetInstance<ComplexType<int>>(instance);
            Assert.AreEqual("Jeremy", com.Name);
            Assert.AreEqual(32, com.Age);
        }

        [Test]
        public void CanGetPluginFamilyFromPluginGraphWithNoParameters()
        {
            var graph = new PluginGraph();
            graph.Scan(x => x.TheCallingAssembly());

            PluginFamily family1 = graph.FindFamily(typeof (IGenericService<int>));
            PluginFamily family2 = graph.FindFamily(typeof (IGenericService<string>));
            PluginFamily family3 = graph.FindFamily(typeof (IGenericService<>));

            Assert.AreSame(graph.FindFamily(typeof (IGenericService<int>)), family1);
            Assert.AreSame(graph.FindFamily(typeof (IGenericService<string>)), family2);
            Assert.AreSame(graph.FindFamily(typeof (IGenericService<>)), family3);
        }

        [Test]
        public void CanGetPluginFamilyFromPluginGraphWithParameters()
        {
            var graph = new PluginGraph();
            graph.Scan(x => x.TheCallingAssembly());

            PluginFamily family1 = graph.FindFamily(typeof (IGenericService<int>));
            PluginFamily family2 = graph.FindFamily(typeof (IGenericService<string>));

            Assert.AreSame(graph.FindFamily(typeof (IGenericService<int>)), family1);
            Assert.AreSame(graph.FindFamily(typeof (IGenericService<string>)), family2);
        }

        [Test]
        public void CanGetTheSameInstanceOfGenericInterfaceWithSingletonScope()
        {
            var con = new Container(x =>
            {
                x.ForRequestedType(typeof (IService<>))
                    .CacheBy(InstanceScope.Singleton)
                    .TheDefaultIsConcreteType(typeof (Service<>));
            });

            var first = con.GetInstance<IService<string>>();
            var second = con.GetInstance<IService<string>>();

            Assert.AreSame(first, second, "The objects are not the same instance");
        }


        [Test]
        public void CanPlugGenericConcreteClassIntoGenericInterfaceWithNoGenericParametersSpecified()
        {
            bool canPlug = typeof (GenericService<>).CanBeCastTo(typeof (IGenericService<>));
            Assert.IsTrue(canPlug);
        }

        [Test]
        public void CanPlugConcreteNonGenericClassIntoGenericInterface()
        {
            typeof(NotSoGenericService).CanBeCastTo(typeof(IGenericService<>))
                .ShouldBeTrue();
        }

        [Test]
        public void Define_profile_with_generics_and_concrete_type()
        {
            var container = new Container(registry =>
            {
                registry.Profile("1").For(typeof (IService<>)).UseConcreteType(typeof (Service<>));
                registry.Profile("2").For(typeof (IService<>)).UseConcreteType(typeof (Service2<>));
            });

            container.SetDefaultsToProfile("1");

            container.GetInstance<IService<string>>().ShouldBeOfType<Service<string>>();

            container.SetDefaultsToProfile("2");

            container.GetInstance<IService<string>>().ShouldBeOfType<Service2<string>>();
        }

        [Test]
        public void Define_profile_with_generics_with_named_instance()
        {
            IContainer container = new Container(r =>
            {
                r.InstanceOf(typeof (IService<>)).Is(typeof (Service<>)).WithName("Service1");
                r.InstanceOf(typeof (IService<>)).Is(typeof (Service2<>)).WithName("Service2");

                r.Profile("1").For(typeof (IService<>)).UseNamedInstance("Service1");
                r.Profile("2").For(typeof (IService<>)).UseNamedInstance("Service2");
            });

            container.SetDefaultsToProfile("1");

            container.GetInstance<IService<string>>().ShouldBeOfType<Service<string>>();


            container.SetDefaultsToProfile("2");
            container.GetInstance<IService<int>>().ShouldBeOfType<Service2<int>>();
        }

        [Test]
        public void GenericsTypeAndProfileOrMachine()
        {
            PluginGraph pluginGraph = PluginGraph.BuildGraphFromAssembly(typeof (IService<>).Assembly);
            pluginGraph.SetDefault("1", typeof (IService<>), new ReferencedInstance("Default"));
            pluginGraph.SetDefault("2", typeof (IService<>), new ReferencedInstance("Plugged"));


            var container = new Container(pluginGraph);


            container.SetDefaultsToProfile("1");
            container.GetInstance(typeof (IService<string>)).ShouldBeOfType<Service<string>>();

            container.SetDefaultsToProfile("2");
            container.GetInstance(typeof (IService<string>))
                                           .ShouldBeOfType<ServiceWithPlug<string>>();

            container.SetDefaultsToProfile("1");
            container.GetInstance(typeof (IService<string>)).ShouldBeOfType < Service<string>>();
        }


        [Test]
        public void GetGenericTypeByString()
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            Type type = assem.GetType("StructureMap.Testing.ITarget`2");

            Type genericType = type.GetGenericTypeDefinition();
            Assert.AreEqual(typeof (ITarget<,>), genericType);
        }


        [Test]
        public void SmokeTestCanBeCaseWithImplementationOfANonGenericInterface()
        {
            Assert.IsTrue(GenericsPluginGraph.CanBeCast(typeof (ITarget<,>), typeof (DisposableTarget<,>)));
        }
    }


    public class ComplexType<T>
    {
        private readonly int _age;
        private readonly string _name;

        public ComplexType(string name, int age)
        {
            _name = name;
            _age = age;
        }

        public string Name { get { return _name; } }

        public int Age { get { return _age; } }

        [ValidationMethod]
        public void Validate()
        {
            throw new ApplicationException("Break!");
        }
    }

    public interface ITarget<T, U>
    {
    }

    public class SpecificTarget<T, U> : ITarget<T, U>
    {
    }

    public class DisposableTarget<T, U> : ITarget<T, U>, IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }

    public interface ITarget2<T, U, V>
    {
    }

    public class SpecificTarget2<T, U, V> : ITarget2<T, U, V>
    {
    }

    public interface IGenericService<T>
    {
        void DoSomething(T thing);
    }

    public class GenericService<T> : IGenericService<T>
    {
        #region IGenericService<T> Members

        public void DoSomething(T thing)
        {
            throw new NotImplementedException();
        }

        #endregion

        public Type GetGenericType()
        {
            return typeof (T);
        }
    }

    public class NotSoGenericService : IGenericService<string>
    {
        public void DoSomething(string thing) { }
    }
}
