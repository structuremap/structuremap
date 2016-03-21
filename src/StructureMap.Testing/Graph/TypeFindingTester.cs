using Shouldly;
using StructureMap.Graph;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Graph
{
    public class TypeFindingTester
    {
        public TypeFindingTester()
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

        private readonly IContainer container;

        [Fact]
        public void FoundTheRightNumberOfInstancesForATypeWithNoPlugins()
        {
            container.GetAllInstances<TypeIWantToFind>().Count()
                .ShouldBe(3);
        }

        [Fact]
        public void FoundTheRightNumberOfInstancesForATypeWithNoPlugins2()
        {
            container.GetAllInstances<OtherType>().Count().ShouldBe(2);
        }

        public class when_finding_all_types_implementing_and_open_generic_interface
        {
            [Fact]
            public void it_can_find_all_implementations()
            {
                using (var container = new Container(c => c.Scan(s =>
                {
                    s.AddAllTypesOf(typeof(IOpenGeneric<>));
                    s.TheCallingAssembly();
                })))
                {
                    var redTypes = container.GetAllInstances<IOpenGeneric<string>>();

                    redTypes.Count().ShouldBe(1);
                }
            }

            [Fact]
            public void it_can_override_generic_implementation_with_specific()
            {
                var container = new Container(c => c.Scan(s =>
                {
                    s.AddAllTypesOf(typeof(IOpenGeneric<>));
                    s.TheCallingAssembly();
                }));

                using (container)
                {
                    var redType = container.GetInstance<IOpenGeneric<string>>();
                    redType.ShouldBeOfType<StringOpenGeneric>();
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