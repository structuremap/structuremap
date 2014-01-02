using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Graph;
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
            new Policies().CanBeAutoFilled(typeof(CannotBeAutoFilledGridColumn))
                .ShouldBeFalse();
        }

        [Test]
        public void AutoFillDeterminationWithSetterPropertiesIsTrue()
        {
            new Policies().CanBeAutoFilled(typeof (AutoFilledGridColumn))
                .ShouldBeTrue();
        }


        [Test]
        public void SetterPropertyCollection_builds_the_correct_number_of_properties()
        {
            var plugin = new Plugin(typeof (OtherGridColumn));
            plugin.Setters.OptionalCount.ShouldEqual(7);

            plugin.Setters.MarkSetterAsMandatory("Widget");
            plugin.Setters.MarkSetterAsMandatory("FontStyle");
            plugin.Setters.MarkSetterAsMandatory("ColumnName");
            plugin.Setters.MarkSetterAsMandatory("Rules");
            plugin.Setters.MarkSetterAsMandatory("WrapLines");

            Assert.AreEqual(2, plugin.Setters.OptionalCount);
            Assert.AreEqual(5, plugin.Setters.MandatoryCount);
        }

        [Test]
        public void SetterProperty_picks_up_IsMandatory_from_setter_attribute()
        {
            var plugin = new Plugin(typeof (SetterTarget));
            new SetterRules().Configure(plugin);

            plugin.Setters.IsMandatory("Name1").ShouldBeFalse();
            plugin.Setters.IsMandatory("Name2").ShouldBeTrue();
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToAddANonExistentSetterProperty()
        {
            var plugin = new Plugin(typeof (BasicGridColumn));
            plugin.Setters.MarkSetterAsMandatory("NonExistentPropertyName");
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToAddASetterPropertyThatDoesNotHaveASetter()
        {
            var plugin = new Plugin(typeof (BasicGridColumn));
            plugin.Setters.MarkSetterAsMandatory("HeaderText");
        }

        [Test]
        public void got_the_right_number_of_mandatory_and_optional_properties()
        {
            var plugin = new Plugin(typeof (SetterTarget));
            new SetterRules().Configure(plugin);

            plugin.Setters.IsMandatory("Name1").ShouldBeFalse();
            plugin.Setters.IsMandatory("Name2").ShouldBeTrue();
            plugin.Setters.IsMandatory("Name3").ShouldBeFalse();
            plugin.Setters.IsMandatory("Name4").ShouldBeTrue();
        }
    }
}