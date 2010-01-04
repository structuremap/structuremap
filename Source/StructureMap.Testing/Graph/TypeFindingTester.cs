using System.Collections.Generic;
using NUnit.Framework;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class TypeFindingTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(registry =>
            {
                registry.For<INormalType>();
                registry.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.AddAllTypesOf<TypeIWantToFind>();
                    x.AddAllTypesOf<OtherType>();
                });
            });
        }

        #endregion

        private IContainer container;


        [Test]
        public void FoundTheRightNumberOfInstancesForATypeWithNoPlugins()
        {
            Assert.AreEqual(3, container.GetAllInstances<TypeIWantToFind>().Count);
        }

        [Test]
        public void FoundTheRightNumberOfInstancesForATypeWithNoPlugins2()
        {
            container.GetAllInstances<OtherType>().Count.ShouldEqual(2);
        }

        [Test]
        public void ScanAssembliesForAPluginAndOnlyGetExplicitlyAttributedClassesWithPluginAttributes()
        {
            IList<INormalType> instances = container.GetAllInstances<INormalType>();
            Assert.AreEqual(1, instances.Count);

            Assert.IsInstanceOfType(typeof (NormalTypeWithPluggableAttribute), instances[0]);
        }
    }


    public interface TypeIWantToFind
    {
    }

    public class RedType
    {
    }

    public class BlueType : TypeIWantToFind
    {
    }

    public class PurpleType : TypeIWantToFind
    {
    }

    public class YellowType : TypeIWantToFind
    {
    }

    public class GreenType : TypeIWantToFind
    {
        private GreenType()
        {
        }
    }

    public abstract class OrangeType : TypeIWantToFind
    {
    }

    public class OtherType
    {
    }

    public class DifferentOtherType : OtherType
    {
    }

    public interface INormalType
    {
    }

    [Pluggable("First")]
    public class NormalTypeWithPluggableAttribute : INormalType
    {
    }

    public class SecondNormalType : INormalType
    {
    }
}