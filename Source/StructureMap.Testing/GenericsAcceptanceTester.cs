using System;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using StructureMap.Graph;

namespace StructureMap.Testing
{
    [TestFixture]
    public class GenericsAcceptanceTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void CanCreatePluginFamilyForGenericTypeWithGenericParameter()
        {
            PluginFamily family = new PluginFamily(typeof(IGenericService<int>));
        }

        [Test]
        public void CanCreatePluginFamilyForGenericTypeWithoutGenericParameter()
        {
            PluginFamily family = new PluginFamily(typeof(IGenericService<>));
        }

        [Test]
        public void CanCreatePluginForGenericTypeWithGenericParameter()
        {
            Plugin plugin = Plugin.CreateExplicitPlugin(typeof (GenericService<int>), "key", string.Empty);            
        }

        [Test]
        public void CanCreatePluginForGenericTypeWithoutGenericParameter()
        {
            Plugin plugin = Plugin.CreateExplicitPlugin(typeof(GenericService<>), "key", string.Empty);
        }

        [Test]
        public void CanGetPluginFamilyFromPluginGraphWithParameters()
        {
            PluginGraph graph = new PluginGraph();
            graph.Assemblies.Add(Assembly.GetExecutingAssembly());
            PluginFamily family1 = graph.PluginFamilies.Add(typeof(IGenericService<int>), string.Empty);
            PluginFamily family2 = graph.PluginFamilies.Add(typeof(IGenericService<string>), string.Empty);
            
            Assert.AreSame( graph.PluginFamilies[typeof(IGenericService<int>)], family1);
            Assert.AreSame( graph.PluginFamilies[typeof(IGenericService<string>)], family2);
        }

        [Test]
        public void CanGetPluginFamilyFromPluginGraphWithNoParameters()
        {
            PluginGraph graph = new PluginGraph();
            graph.Assemblies.Add(Assembly.GetExecutingAssembly());
            PluginFamily family1 = graph.PluginFamilies.Add(typeof(IGenericService<int>), string.Empty);
            PluginFamily family2 = graph.PluginFamilies.Add(typeof(IGenericService<string>), string.Empty);
            PluginFamily family3 = graph.PluginFamilies.Add(typeof(IGenericService<>), string.Empty);

            Assert.AreSame(graph.PluginFamilies[typeof(IGenericService<int>)], family1);
            Assert.AreSame(graph.PluginFamilies[typeof(IGenericService<string>)], family2);
            Assert.AreSame(graph.PluginFamilies[typeof(IGenericService<>)], family3);
        }


        [Test]
        public void CanPlugGenericConcreteClassIntoGenericInterfaceWithNoGenericParametersSpecified()
        {
            bool canPlug = Plugin.CanBeCast(typeof (IGenericService<>), typeof (GenericService<>));        
            Assert.IsTrue(canPlug);
        }
        
        [Test]
        public void BuildFamilyAndPluginThenSealAndCreateInstanceManagerWithGenericTypeWithOpenGenericParameters()
        {
            PluginGraph graph = new PluginGraph();
            graph.Assemblies.Add(Assembly.GetExecutingAssembly());
            PluginFamily family = graph.PluginFamilies.Add(typeof(IGenericService<>), "Default");
            family.Plugins.Add(typeof(GenericService<>), "Default");

            graph.Seal();

            InstanceManager manager = new InstanceManager(graph);
        }

        [Test]
        public void GetGenericTypeByString()
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            Type type = assem.GetType("StructureMap.Testing.ITarget`2");

            Type genericType = type.GetGenericTypeDefinition();
            Assert.AreEqual(typeof(ITarget<,>), genericType);
        }


        [Test]
        public void GetGenericTypeWithParametersByString()
        {
            Type type1 = typeof (ITarget<int, string>);
            string name = type1.FullName;
            Debug.WriteLine(name);
            Type type2 = Assembly.GetExecutingAssembly().GetType(name);
            
            Assert.AreEqual(type1, type2);
        }

        [Test]
        public void CanEmitInstanceBuilderForATypeWithConstructorArguments()
        {
            PluginFamily family = new PluginFamily(typeof(ComplexType<int>));
            family.Plugins.Add(typeof(ComplexType<int>), "complex");

            InstanceFactory factory = new InstanceFactory(family, true);
            
            MemoryInstanceMemento memento = new MemoryInstanceMemento("complex", "Me");
            memento.SetProperty("name", "Jeremy");
            memento.SetProperty("age", "32");

            ComplexType<int> com = (ComplexType<int>) factory.GetInstance(memento);
            Assert.AreEqual("Jeremy", com.Name);
            Assert.AreEqual(32, com.Age);
        }

        [Test]
        public void CanEmitForATemplateWithTwoTemplates()
        {
            PluginFamily family = new PluginFamily(typeof(ITarget<int, string>));
            family.Plugins.Add(typeof(SpecificTarget<int,string>), "specific");
            
            InstanceFactory factory = new InstanceFactory(family, true);
        }



        [Test, Ignore("Generics with more than 2 parameters")]
        public void CanEmitForATemplateWithThreeTemplates()
        {
            PluginFamily family = new PluginFamily(typeof(ITarget2<int, string, bool>));
            family.Plugins.Add(typeof(SpecificTarget2<int, string, bool>), "specific");

            InstanceFactory factory = new InstanceFactory(family, true);
        }
    }
    
    public class ComplexType<T>
    {
        private readonly string _name;
        private readonly int _age;

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
    }
    
    public interface ITarget<T, U>{}
    
    public class SpecificTarget<T, U> : ITarget<T, U>{}

    public interface ITarget2<T, U, V> { }

    public class SpecificTarget2<T, U, V> : ITarget2<T, U, V> { }

    public interface IGenericService<T>
    {
        void DoSomething(T thing);
    }

    public class GenericService<T> : IGenericService<T>
    {
        public void DoSomething(T thing)
        {
            throw new NotImplementedException();
        }

        public Type GetGenericType()
        {
            return typeof(T);
        }
    }
}
