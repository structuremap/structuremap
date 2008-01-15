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
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (CannotBeAutoFilledGridColumn));
            Assert.IsFalse(plugin.CanBeAutoFilled);
        }

        [Test]
        public void AutoFillDeterminationWithSetterPropertiesIsTrue()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (AutoFilledGridColumn));
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
            Plugin plugin = pluginGraph.PluginFamilies[typeof (IGridColumn)].Plugins["Other"];

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

            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (BasicGridColumn));

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
            Plugin plugin = Plugin.CreateExplicitPlugin(typeof (BasicGridColumn), "Basic", string.Empty);
            plugin.Setters.Add("NonExistentPropertyName");
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToAddASetterPropertyThatDoesNotHaveASetter()
        {
            Plugin plugin = Plugin.CreateExplicitPlugin(typeof (BasicGridColumn), "Basic", string.Empty);
            plugin.Setters.Add("HeaderText");
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToCreateAnImplicitPluginWithASetterPropertyThatDoesNotHaveASetMethod()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (BadSetterClass));
        }
    }
}