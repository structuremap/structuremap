using NUnit.Framework;
using Shouldly;
using StructureMap.Graph;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class TypePathTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void can_parse_assembly_qualified_generics()
        {
            var sampleGenericType = typeof (IConcept<AWidget>);
            var genericAssemblyQualifiedName = sampleGenericType.AssemblyQualifiedName;

            var path = new TypePath(genericAssemblyQualifiedName);
            path.FindType().ShouldBe(sampleGenericType);
        }

        [Test]
        public void CanBuildTypeCreatedFromType()
        {
            var path = new TypePath(GetType());
            path.FindType();
        }
    }
}