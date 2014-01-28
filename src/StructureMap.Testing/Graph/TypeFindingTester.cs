using System.Linq;
using NUnit.Framework;
using StructureMap.Graph;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class TypeFindingTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(registry => {
                registry.For<INormalType>();
                registry.Scan(x => {
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
            container.GetAllInstances<TypeIWantToFind>().Count()
                .ShouldEqual(3);
        }

        [Test]
        public void FoundTheRightNumberOfInstancesForATypeWithNoPlugins2()
        {
            container.GetAllInstances<OtherType>().Count().ShouldEqual(2);
        }

        [TestFixture]
        public class when_finding_all_types_implementing_and_open_generic_interface
        {
            [Test]
            public void it_can_find_all_implementations()
            {
                using (var container = new Container(c => c.Scan(s => {
                    s.AddAllTypesOf(typeof (IOpenGeneric<>));
                    s.TheCallingAssembly();
                })))
                {
                    var redTypes = container.GetAllInstances<IOpenGeneric<string>>();

                    redTypes.Count().ShouldEqual(1);
                }
            }

            [Test]
            public void it_can_override_generic_implementation_with_specific()
            {
                var container = new Container(c => c.Scan(s => {
                    s.AddAllTypesOf(typeof (IOpenGeneric<>));
                    s.TheCallingAssembly();
                }));

                using (container)
                {
                    var redType = container.GetInstance<IOpenGeneric<string>>();
                    Assert.That(redType, Is.InstanceOfType(typeof (StringOpenGeneric)));
                }
            }
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

    //[Pluggable("First")]
    public class NormalTypeWithPluggableAttribute : INormalType
    {
    }

    public class SecondNormalType : INormalType
    {
    }
}