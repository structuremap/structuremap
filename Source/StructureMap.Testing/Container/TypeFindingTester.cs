using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class TypeFindingTester
    {
        private IInstanceManager _manager;

        [SetUp]
        public void SetUp()
        {
            Registry registry = new Registry();
            registry.BuildInstancesOf<INormalType>();
            registry.ScanAssemblies().IncludeTheCallingAssembly()
                .AddAllTypesOf<TypeIWantToFind>()
                .AddAllTypesOf<OtherType>();

            _manager = registry.BuildInstanceManager();
        }

        [Test]
        public void FoundTheRightNumberOfInstancesForATypeWithNoPlugins()
        {
            Assert.AreEqual(3, _manager.GetAllInstances<TypeIWantToFind>().Count);
        }

        [Test]
        public void FoundTheRightNumberOfInstancesForATypeWithNoPlugins2()
        {
            Assert.AreEqual(2, _manager.GetAllInstances<OtherType>().Count);
        }

        [Test]
        public void ScanAssembliesForAPluginAndOnlyGetExplicitlyAttributedClassesWithPluginAttributes()
        {
            IList<INormalType> instances = _manager.GetAllInstances<INormalType>();
            Assert.AreEqual(1, instances.Count);

            Assert.IsInstanceOfType(typeof(NormalTypeWithPluggableAttribute), instances[0]);
        }

        [Test]
        public void DoNotFindPluginWithNoPublicCTOR()
        {
            Assert.IsFalse(Plugin.CanBeCast(typeof(TypeIWantToFind), typeof(GreenType)));
        }




        [Test]
        public void FindTypes()
        {
            AssemblyGraph assemblyGraph = new AssemblyGraph(Assembly.GetExecutingAssembly());
            Type[] types = assemblyGraph.FindTypes(
                delegate(Type type) { return type.Equals(typeof(BlueType)); });

            Assert.AreEqual(new Type[] { typeof(BlueType) }, types);
        }
    }


    public interface TypeIWantToFind
    {

    }

    public class RedType { }

    public class BlueType : TypeIWantToFind { }
    public class PurpleType : TypeIWantToFind { }
    public class YellowType : TypeIWantToFind { }
    public class GreenType : TypeIWantToFind
    {
        private GreenType() { }
    }

    public abstract class OrangeType : TypeIWantToFind { }

    public class OtherType{}
    public class DifferentOtherType : OtherType{}

    public interface INormalType{}
    [Pluggable("First")] public class NormalTypeWithPluggableAttribute : INormalType{}
    public class SecondNormalType : INormalType{}

}
