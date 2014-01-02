using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Testing.Widget5;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
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

        [Test]
        public void AutoFillDeterminationWithSetterPropertiesIsFalse()
        {
            new Policies().CanBeAutoFilled(typeof (CannotBeAutoFilledGridColumn))
                .ShouldBeFalse();
        }

        [Test]
        public void AutoFillDeterminationWithSetterPropertiesIsTrue()
        {
            new Policies().CanBeAutoFilled(typeof (AutoFilledGridColumn))
                .ShouldBeTrue();
        }
    }
}