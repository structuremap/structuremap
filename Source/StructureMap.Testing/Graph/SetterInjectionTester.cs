using System.Reflection;
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

        private PluginGraph getPluginGraph()
        {
            return DataMother.GetDiagnosticPluginGraph("SetterInjectionTesting.xml");
        }

        [Test]
        public void AutoFillDeterminationWithSetterPropertiesIsFalse()
        {
            Plugin plugin = new Plugin(typeof (CannotBeAutoFilledGridColumn));
            Assert.IsFalse(plugin.CanBeAutoFilled);
        }

        [Test]
        public void AutoFillDeterminationWithSetterPropertiesIsTrue()
        {
            Plugin plugin = new Plugin(typeof (AutoFilledGridColumn));
            Assert.IsTrue(plugin.CanBeAutoFilled);
        }

        [Test]
        public void CanFindMarkedProperties()
        {
            PropertyInfo[] properties = SetterPropertyAttribute.FindMarkedProperties(typeof (BasicGridColumn));
            Assert.AreEqual(5, properties.Length);
        }


        [Test]
        public void CreateSetterPropertyCollectionFromExplicitPlugin()
        {
            PluginGraph pluginGraph = getPluginGraph();
            PluginFamily family = pluginGraph.FindFamily(typeof (IGridColumn));
            Plugin plugin = family.Plugins["Other"];

            Assert.AreEqual(5, plugin.Setters.Count);
            Assert.IsTrue(plugin.Setters.Contains("Widget"));
            Assert.IsTrue(plugin.Setters.Contains("FontStyle"));
            Assert.IsTrue(plugin.Setters.Contains("ColumnName"));
            Assert.IsTrue(plugin.Setters.Contains("Rules"));
            Assert.IsTrue(plugin.Setters.Contains("WrapLines"));
        }

        [Test]
        public void CreateSetterPropertyCollectionFromImplicitPlugin()
        {
            /*    The BasicGridColumn class has 5 [SetterProperty] marked properties
			 *    Widget
			 *    FontStyle
			 *    ColumnName
			 *    Rules
			 *    WrapLines
			 */

            Plugin plugin = new Plugin(typeof (BasicGridColumn));

            Assert.AreEqual(5, plugin.Setters.Count);
            Assert.IsTrue(plugin.Setters.Contains("Widget"));
            Assert.IsTrue(plugin.Setters.Contains("FontStyle"));
            Assert.IsTrue(plugin.Setters.Contains("ColumnName"));
            Assert.IsTrue(plugin.Setters.Contains("Rules"));
            Assert.IsTrue(plugin.Setters.Contains("WrapLines"));
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToAddANonExistentSetterProperty()
        {
            Plugin plugin = new Plugin(typeof (BasicGridColumn), "Basic");
            plugin.Setters.Add("NonExistentPropertyName");
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToAddASetterPropertyThatDoesNotHaveASetter()
        {
            Plugin plugin = new Plugin(typeof (BasicGridColumn), "Basic");
            plugin.Setters.Add("HeaderText");
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToCreateAnImplicitPluginWithASetterPropertyThatDoesNotHaveASetMethod()
        {
            Plugin plugin = new Plugin(typeof (BadSetterClass));
        }
    }
}