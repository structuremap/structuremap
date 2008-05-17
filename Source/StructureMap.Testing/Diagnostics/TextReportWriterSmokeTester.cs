using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Diagnostics;

namespace StructureMap.Testing.Diagnostics
{
    [TestFixture]
    public class TextReportWriterSmokeTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void TryWithTwoColumnsAndSomeDividers()
        {
            TextReportWriter writer = new TextReportWriter(2);
            writer.AddDivider('=');
            writer.AddText("Name", "City");
            writer.AddDivider('=');
            writer.AddText("Jeremy", "Austin");
            writer.AddText("Jessica", "Little Rock");
            writer.AddText("Natalie", "Bentonville");
            writer.AddDivider('-');
            writer.AddText("Monte", "Joplin");
            writer.AddText("aaaaaaaaaaaaaaaaaaaaaaaaaa", "Joplin");
            writer.AddText("aaaaaaaaaaa", "Joplin adsf asdf asdf asdf");
            writer.AddDivider('=');

            writer.DumpToConsole();
        }
    }
}
