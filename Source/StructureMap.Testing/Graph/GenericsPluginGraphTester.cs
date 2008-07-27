using System;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;

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
        public void Check_the_generic_plugin_family_expression()
        {
            Container container = new Container(r =>
            {
                r.ForRequestedType(typeof (IGenericService<>)).TheDefaultIsConcreteType(typeof (GenericService<>));
            });

            container.GetInstance<IGenericService<string>>().ShouldBeOfType(typeof(GenericService<string>));
        }

        [Test]
        public void BuildAnInstanceManagerFromTemplatedPluginFamily()
        {
            PluginGraph pluginGraph = new PluginGraph();
            PluginFamily family = pluginGraph.FindFamily(typeof (IGenericService<>));
            family.DefaultInstanceKey = "Default";
            family.AddPlugin(typeof (GenericService<>), "Default");
            family.AddPlugin(typeof (SecondGenericService<>), "Second");
            family.AddPlugin(typeof (ThirdGenericService<>), "Third");

            Container manager = new Container(pluginGraph);

            GenericService<int> intService = (GenericService<int>) manager.GetInstance<IGenericService<int>>();
            Assert.AreEqual(typeof (int), intService.GetT());

            Assert.IsInstanceOfType(typeof (SecondGenericService<int>),
                                    manager.GetInstance<IGenericService<int>>("Second"));

            GenericService<string> stringService =
                (GenericService<string>) manager.GetInstance<IGenericService<string>>();
            Assert.AreEqual(typeof (string), stringService.GetT());
        }

        [Test]
        public void BuildTemplatedFamilyWithOnlyOneTemplateParameter()
        {
            PluginGraph pluginGraph = new PluginGraph();
            PluginFamily family = pluginGraph.FindFamily(typeof (IGenericService<>));
            family.AddPlugin(typeof (GenericService<>), "Default");
            family.AddPlugin(typeof (SecondGenericService<>), "Second");
            family.AddPlugin(typeof (ThirdGenericService<>), "Third");

            PluginFamily templatedFamily = GenericsPluginGraph.CreateTemplatedClone(family, typeof (int));

            Assert.IsNotNull(templatedFamily);
            Assert.AreEqual(typeof (IGenericService<int>), templatedFamily.PluginType);

            Assert.AreEqual(3, templatedFamily.Plugins.Count);
            Assert.IsNotNull(templatedFamily.Plugins[typeof (GenericService<int>)]);
            Assert.IsNotNull(templatedFamily.Plugins[typeof (SecondGenericService<int>)]);
            Assert.IsNotNull(templatedFamily.Plugins[typeof (ThirdGenericService<int>)]);
        }

        [Test]
        public void BuildTemplatedFamilyWithThreeTemplateParameters()
        {
            PluginGraph pluginGraph = new PluginGraph();
            PluginFamily family = pluginGraph.FindFamily(typeof (IGenericService3<,,>));
            family.AddPlugin(typeof (GenericService3<,,>), "Default");
            family.AddPlugin(typeof (SecondGenericService3<,,>), "Second");
            family.AddPlugin(typeof (ThirdGenericService3<,,>), "Third");

            PluginFamily templatedFamily = GenericsPluginGraph.CreateTemplatedClone(family, typeof (int), typeof (bool),
                                                                                    typeof (string));

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
            PluginFamily family = pluginGraph.FindFamily(typeof (IGenericService<>));
            family.AddPlugin(typeof (GenericService<>), "Default");
            family.AddPlugin(typeof (SecondGenericService<>), "Second");
            family.AddPlugin(typeof (ThirdGenericService<>), "Third");

            GenericsPluginGraph genericsGraph = new GenericsPluginGraph();
            genericsGraph.AddFamily(family);

            PluginFamily templatedFamily = genericsGraph.CreateTemplatedFamily(typeof (IGenericService<int>),
                                                                               new ProfileManager());

            Assert.IsNotNull(templatedFamily);
            Assert.AreEqual(typeof (IGenericService<int>), templatedFamily.PluginType);
        }

        [Test]
        public void ImplementationOfInterfaceFromBaseType()
        {
            assertCanBeCast(typeof (ISomething<>), typeof (SpecificService<>));
        }

        [Test]
        public void Import_from_adds_all_new_PluginFamily_from_source()
        {
            PluginFamily sourceFamily = new PluginFamily(typeof (ISomething<>));
            PluginFamily sourceFamily2 = new PluginFamily(typeof (ISomething2<>));
            PluginFamily sourceFamily3 = new PluginFamily(typeof (ISomething3<>));
            GenericsPluginGraph source = new GenericsPluginGraph();
            source.AddFamily(sourceFamily);
            source.AddFamily(sourceFamily2);
            source.AddFamily(sourceFamily3);

            GenericsPluginGraph destination = new GenericsPluginGraph();
            destination.ImportFrom(source);

            Assert.AreEqual(3, destination.FamilyCount);

            Assert.AreNotSame(sourceFamily, destination.FindFamily(typeof (ISomething<>)));
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

    public interface ISomething2<T>
    {
    }

    public interface ISomething3<T>
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