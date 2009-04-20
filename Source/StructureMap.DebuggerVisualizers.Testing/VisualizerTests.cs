using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using StructureMap.Pipeline;

namespace StructureMap.DebuggerVisualizers.Testing
{
    [TestFixture]
    public class VisualizerTests
    {
        [Test]
        public void can_serialize_container_details()
        {
            var pluginTypeDetails = new[]{ new PluginTypeDetail(typeof(string), typeof(object), new[]{ new InstanceDetail("First", "First Instance", typeof(string)), }) };
            var wrapper = new ContainerDetail(new[]{"config"}, pluginTypeDetails);
            var binaryFormatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            binaryFormatter.Serialize(stream, wrapper);
            
            stream.Position = 0;
            var detailOnOtherSide = (ContainerDetail) binaryFormatter.Deserialize(stream);
            Assert.That(detailOnOtherSide, Is.Not.Null);
            Assert.That(detailOnOtherSide, Is.TypeOf(typeof(ContainerDetail)));
        }

        [Test]
        public void display_nongeneric_types()
        {
            Assert.That( typeof(IDoThat).AsCSharp(), Is.EqualTo("IDoThat"));
        }

        [Test]
        public void display_open_generics_using_c_sharp_syntax()
        {
            Assert.That(typeof (IHasTwoGenerics<,>).AsCSharp(), Is.EqualTo("IHasTwoGenerics<FIRST,SECOND>"));
        }

        [Test]
        public void display_closed_generics_using_c_sharp_syntax()
        {
            Assert.That(typeof(IHasTwoGenerics<string, int>).AsCSharp(), Is.EqualTo("IHasTwoGenerics<String,Int32>"));
        }

        [Test]
        public void display_open_generics_using_c_sharp_syntax_with_fullnames()
        {
            Assert.That(typeof(IHasTwoGenerics<,>).AsCSharp(t => t.FullName), Is.EqualTo("StructureMap.DebuggerVisualizers.Testing.IHasTwoGenerics<FIRST,SECOND>"));
        }

        [Test]
        public void display_closed_generics_using_c_sharp_syntax_with_fullnames()
        {
            Assert.That(typeof(IHasTwoGenerics<string, int>).AsCSharp(t => t.FullName), Is.EqualTo("StructureMap.DebuggerVisualizers.Testing.IHasTwoGenerics<System.String,System.Int32>"));
        }

    }
}