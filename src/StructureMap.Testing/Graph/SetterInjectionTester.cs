using StructureMap.Attributes;
using StructureMap.Testing.Widget5;
using Xunit;

namespace StructureMap.Testing.Graph
{
    public class SetterInjectionTester
    {
        public class SetterTarget
        {
            public string Name1 { get; set; }

            [SetterProperty]
            public string Name2 { get; set; }

            public string Name3 { get; set; }

            [SetterProperty]
            public string Name4 { get; set; }
        }

        [Fact]
        public void AutoFillDeterminationWithSetterPropertiesIsFalse()
        {
            Policies.Default().CanBeAutoFilled(typeof(CannotBeAutoFilledGridColumn))
                .ShouldBeFalse();
        }

        [Fact]
        public void AutoFillDeterminationWithSetterPropertiesIsTrue()
        {
            Policies.Default().CanBeAutoFilled(typeof(AutoFilledGridColumn))
                .ShouldBeTrue();
        }
    }
}