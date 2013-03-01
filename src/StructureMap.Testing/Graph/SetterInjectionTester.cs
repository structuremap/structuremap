using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget5;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class SetterInjectionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            DataMother.WriteDocument("FullTesting.XML");
        }

        #endregion

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
            var plugin = new Plugin(typeof (CannotBeAutoFilledGridColumn));
            Assert.IsFalse(plugin.CanBeAutoFilled);
        }

        [Test]
        public void AutoFillDeterminationWithSetterPropertiesIsTrue()
        {
            var plugin = new Plugin(typeof (AutoFilledGridColumn));
            Assert.IsTrue(plugin.CanBeAutoFilled);
        }


        [Test]
        public void got_the_right_number_of_mandatory_and_optional_properties()
        {
            var plugin = new Plugin(typeof (SetterTarget));
            plugin.Setters.IsMandatory("Name1").ShouldBeFalse();
            plugin.Setters.IsMandatory("Name2").ShouldBeTrue();
            plugin.Setters.IsMandatory("Name3").ShouldBeFalse();
            plugin.Setters.IsMandatory("Name4").ShouldBeTrue();
        }

        [Test]
        public void SetterProperty_picks_up_IsMandatory_from_setter_attribute()
        {
            var setter1 = new SetterProperty(ReflectionHelper.GetProperty<SetterTarget>(x => x.Name1));
            setter1.IsMandatory.ShouldBeFalse();

            var setter2 = new SetterProperty(ReflectionHelper.GetProperty<SetterTarget>(x => x.Name2));
            setter2.IsMandatory.ShouldBeTrue();
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
    }
}