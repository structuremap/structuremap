using System;
using NUnit.Framework;
using StructureMap.Graph;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class GenericsPluginGraphTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        private void assertCanBeCast(Type pluginType, Type pluggedType)
        {
            Assert.IsTrue(GenericsPluginGraph.CanBeCast(pluginType, pluggedType));
        }

        private void assertCanNotBeCast(Type pluginType, Type pluggedType)
        {
            Assert.IsFalse(GenericsPluginGraph.CanBeCast(pluginType, pluggedType));
        }

        [Test]
        public void BuildAnInstanceManagerFromTemplatedPluginFamily()
        {
            PluginGraph pluginGraph = new PluginGraph();
            PluginFamily family = pluginGraph.PluginFamilies.Add(typeof (IGenericService<>));
            family.DefaultInstanceKey = "Default";
            family.Plugins.Add(typeof (GenericService<>), "Default");
            family.Plugins.Add(typeof (SecondGenericService<>), "Second");
            family.Plugins.Add(typeof (ThirdGenericService<>), "Third");

            InstanceManager manager = new InstanceManager(pluginGraph);

            GenericService<int> intService = (GenericService<int>) manager.CreateInstance<IGenericService<int>>();
            Assert.AreEqual(typeof (int), intService.GetT());

            Assert.IsInstanceOfType(typeof(SecondGenericService<int>), manager.CreateInstance<IGenericService<int>>("Second"));

            GenericService<string> stringService = (GenericService<string>) manager.CreateInstance<IGenericService<string>>();
            Assert.AreEqual(typeof (string), stringService.GetT());
        }

        [Test]
        public void BuildTemplatedFamilyWithOnlyOneTemplateParameter()
        {
            PluginGraph pluginGraph = new PluginGraph();
            PluginFamily family = pluginGraph.PluginFamilies.Add(typeof(IGenericService<>));
            family.Plugins.Add(typeof (GenericService<>), "Default");
            family.Plugins.Add(typeof (SecondGenericService<>), "Second");
            family.Plugins.Add(typeof (ThirdGenericService<>), "Third");

            PluginFamily templatedFamily = family.CreateTemplatedClone(typeof (int));

            Assert.IsNotNull(templatedFamily);
            Assert.AreEqual(typeof (IGenericService<int>), templatedFamily.PluginType);

            Assert.AreEqual(3, templatedFamily.Plugins.Count);
            Assert.AreEqual(typeof (GenericService<int>), templatedFamily.Plugins["Default"].PluggedType);
            Assert.AreEqual(typeof (SecondGenericService<int>), templatedFamily.Plugins["Second"].PluggedType);
            Assert.AreEqual(typeof (ThirdGenericService<int>), templatedFamily.Plugins["Third"].PluggedType);
        }

        [Test]
        public void BuildTemplatedFamilyWithThreeTemplateParameters()
        {
            PluginGraph pluginGraph = new PluginGraph();
            PluginFamily family = pluginGraph.PluginFamilies.Add(typeof(IGenericService3<,,>));
            family.Plugins.Add(typeof (GenericService3<,,>), "Default");
            family.Plugins.Add(typeof (SecondGenericService3<,,>), "Second");
            family.Plugins.Add(typeof (ThirdGenericService3<,,>), "Third");

            PluginFamily templatedFamily = family.CreateTemplatedClone(typeof (int), typeof (bool), typeof (string));

            Assert.IsNotNull(templatedFamily);
            Assert.AreEqual(typeof (IGenericService3<int, bool, string>), templatedFamily.PluginType);

            Assert.AreEqual(3, templatedFamily.Plugins.Count);
            Assert.AreEqual(typeof (GenericService3<int, bool, string>), templatedFamily.Plugins["Default"].PluggedType);
            Assert.AreEqual(typeof (SecondGenericService3<int, bool, string>),
                            templatedFamily.Plugins["Second"].PluggedType);
            Assert.AreEqual(typeof (ThirdGenericService3<int, bool, string>),
                            templatedFamily.Plugins["Third"].PluggedType);
        }


        [Test]
        public void DirectImplementationOfInterfaceCanBeCast()
        {
            assertCanBeCast(typeof (IGenericService<>), typeof (GenericService<>));
            assertCanNotBeCast(typeof (IGenericService<>), typeof (SpecificService<>));
        }

        [Test]
        public void DirectInheritanceOfAbstractClassCanBeCast()
        {
            assertCanBeCast(typeof (BaseSpecificService<>), typeof (SpecificService<>));
        }

        [Test]
        public void GetTemplatedFamily()
        {
            PluginGraph pluginGraph = new PluginGraph();
            PluginFamily family = pluginGraph.PluginFamilies.Add(typeof(IGenericService<>));
            family.Plugins.Add(typeof (GenericService<>), "Default");
            family.Plugins.Add(typeof (SecondGenericService<>), "Second");
            family.Plugins.Add(typeof (ThirdGenericService<>), "Third");

            GenericsPluginGraph genericsGraph = new GenericsPluginGraph();
            genericsGraph.AddFamily(family);

            PluginFamily templatedFamily = genericsGraph.CreateTemplatedFamily(typeof (IGenericService<int>));

            Assert.IsNotNull(templatedFamily);
            Assert.AreEqual(typeof (IGenericService<int>), templatedFamily.PluginType);
        }

        [Test]
        public void ImplementationOfInterfaceFromBaseType()
        {
            assertCanBeCast(typeof (ISomething<>), typeof (SpecificService<>));
        }

        [Test]
        public void RecursiveImplementation()
        {
            assertCanBeCast(typeof (ISomething<>), typeof (SpecificService<>));
            assertCanBeCast(typeof (ISomething<>), typeof (GrandChildSpecificService<>));
        }

        [Test]
        public void RecursiveInheritance()
        {
            assertCanBeCast(typeof (BaseSpecificService<>), typeof (ChildSpecificService<>));
            assertCanBeCast(typeof (BaseSpecificService<>), typeof (GrandChildSpecificService<>));
        }
    }

    public interface IGenericService<T>
    {
    }

    public class GenericService<T> : IGenericService<T>
    {
        public Type GetT()
        {
            return typeof (T);
        }
    }

    public class SecondGenericService<T> : IGenericService<T>
    {
    }

    public class ThirdGenericService<T> : IGenericService<T>
    {
    }

    public interface ISomething<T>
    {
    }

    public abstract class BaseSpecificService<T> : ISomething<T>
    {
    }

    public class SpecificService<T> : BaseSpecificService<T>
    {
    }

    public class ChildSpecificService<T> : SpecificService<T>
    {
    }

    public class GrandChildSpecificService<T> : ChildSpecificService<T>
    {
    }


    public interface IGenericService3<T, U, V>
    {
    }

    public class GenericService3<T, U, V> : IGenericService3<T, U, V>
    {
        public Type GetT()
        {
            return typeof (T);
        }
    }

    public class SecondGenericService3<T, U, V> : IGenericService3<T, U, V>
    {
    }

    public class ThirdGenericService3<T, U, V> : IGenericService3<T, U, V>
    {
    }
}