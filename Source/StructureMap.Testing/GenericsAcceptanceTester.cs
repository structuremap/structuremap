using System;
using System.Reflection;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.GenericWidgets;

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
            PluginGraph graph = new PluginGraph();
            graph.Assemblies.Add(Assembly.GetExecutingAssembly());
            PluginFamily family = graph.FindFamily(typeof (IGenericService<>));
            family.DefaultInstanceKey = "Default";
            family.Plugins.Add(typeof (GenericService<>), "Default");

            graph.Seal();

            InstanceManager manager = new InstanceManager(graph);
        }

        [Test]
        public void CanBuildAGenericObjectThatHasAnotherGenericObjectAsAChild()
        {
            Type serviceType = typeof (IService<double>);
            PluginGraph pluginGraph = PluginGraph.BuildGraphFromAssembly(serviceType.Assembly);
            InstanceManager manager = new InstanceManager(pluginGraph);

            Type doubleServiceType = typeof (IService<double>);

            ServiceWithPlug<double> service =
                (ServiceWithPlug<double>) manager.CreateInstance(doubleServiceType, "Plugged");
            Assert.AreEqual(typeof (double), service.Plug.PlugType);
        }

        [Test]
        public void CanCreatePluginFamilyForGenericTypeWithGenericParameter()
        {
            PluginFamily family = new PluginFamily(typeof (IGenericService<int>));
        }

        [Test]
        public void CanCreatePluginFamilyForGenericTypeWithoutGenericParameter()
        {
            PluginFamily family = new PluginFamily(typeof (IGenericService<>));
        }

        [Test]
        public void CanCreatePluginForGenericTypeWithGenericParameter()
        {
            Plugin plugin = new Plugin(typeof (GenericService<int>), "key");
        }

        [Test]
        public void CanCreatePluginForGenericTypeWithoutGenericParameter()
        {
            Plugin plugin = new Plugin(typeof (GenericService<>), "key");
        }

        [Test, Ignore("Generics with more than 2 parameters")]
        public void CanEmitForATemplateWithThreeTemplates()
        {
            PluginFamily family = new PluginFamily(typeof (ITarget2<int, string, bool>));
            family.Plugins.Add(typeof (SpecificTarget2<int, string, bool>), "specific");

            InstanceFactory factory = new InstanceFactory(family);
        }

        [Test]
        public void CanEmitForATemplateWithTwoTemplates()
        {
            PluginFamily family = new PluginFamily(typeof (ITarget<int, string>));
            family.Plugins.Add(typeof (SpecificTarget<int, string>), "specific");

            InstanceFactory factory = new InstanceFactory(family);
        }

        [Test]
        public void CanEmitInstanceBuilderForATypeWithConstructorArguments()
        {
            PluginGraph graph = new PluginGraph();
            PluginFamily family = graph.FindFamily(typeof (ComplexType<int>));
            family.Plugins.Add(typeof (ComplexType<int>), "complex");

            InstanceManager manager = new InstanceManager(graph);

            ConfiguredInstance instance = new ConfiguredInstance();
            instance.ConcreteKey = "complex";
            instance.SetProperty("name", "Jeremy");
            instance.SetProperty("age", "32");

            ComplexType<int> com = manager.CreateInstance<ComplexType<int>>(instance);
            Assert.AreEqual("Jeremy", com.Name);
            Assert.AreEqual(32, com.Age);
        }

        [Test]
        public void CanGetPluginFamilyFromPluginGraphWithNoParameters()
        {
            PluginGraph graph = new PluginGraph();
            graph.Assemblies.Add(Assembly.GetExecutingAssembly());
            PluginFamily family1 = graph.FindFamily(typeof (IGenericService<int>));
            PluginFamily family2 = graph.FindFamily(typeof (IGenericService<string>));
            PluginFamily family3 = graph.FindFamily(typeof (IGenericService<>));

            Assert.AreSame(graph.FindFamily(typeof(IGenericService<int>)), family1);
            Assert.AreSame(graph.FindFamily(typeof(IGenericService<string>)), family2);
            Assert.AreSame(graph.FindFamily(typeof(IGenericService<>)), family3);
        }

        [Test]
        public void CanGetPluginFamilyFromPluginGraphWithParameters()
        {
            PluginGraph graph = new PluginGraph();
            graph.Assemblies.Add(Assembly.GetExecutingAssembly());
            PluginFamily family1 = graph.FindFamily(typeof (IGenericService<int>));
            PluginFamily family2 = graph.FindFamily(typeof (IGenericService<string>));

            Assert.AreSame(graph.FindFamily(typeof (IGenericService<int>)), family1);
            Assert.AreSame(graph.FindFamily(typeof (IGenericService<string>)), family2);
        }


        [Test]
        public void CanPlugGenericConcreteClassIntoGenericInterfaceWithNoGenericParametersSpecified()
        {
            bool canPlug = Plugin.CanBeCast(typeof (IGenericService<>), typeof (GenericService<>));
            Assert.IsTrue(canPlug);
        }

        [Test]
        public void GenericsTypeAndProfileOrMachine()
        {
            PluginGraph pluginGraph = PluginGraph.BuildGraphFromAssembly(typeof(IService<>).Assembly);
            pluginGraph.ProfileManager.SetDefault("1", typeof(IService<>),new ReferencedInstance("Default"));
            pluginGraph.ProfileManager.SetDefault("2", typeof(IService<>),new ReferencedInstance("Plugged"));
            

            InstanceManager manager = new InstanceManager(pluginGraph);

            IPlug<string> plug = manager.CreateInstance<IPlug<string>>();

            manager.SetDefaultsToProfile("1");
            Assert.IsInstanceOfType(typeof(Service<string>), manager.CreateInstance(typeof(IService<string>)));

            manager.SetDefaultsToProfile("2");
            Assert.IsInstanceOfType(typeof(ServiceWithPlug<string>), manager.CreateInstance(typeof(IService<string>)));

            manager.SetDefaultsToProfile("1");
            Assert.IsInstanceOfType(typeof(Service<string>), manager.CreateInstance(typeof(IService<string>)));
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

        public string Name
        {
            get { return _name; }
        }

        public int Age
        {
            get { return _age; }
        }

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
        public DisposableTarget()
        {
        }

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
}