using StructureMap.Diagnostics;
using System;
using Xunit;

namespace StructureMap.Testing.Diagnostics
{
    public class TextReportWriterSmokeTester
    {
        [Fact]
        public void TryWithTwoColumnsAndSomeDividers()
        {
            var writer = new TextReportWriter(2);
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

            Console.WriteLine(writer.Write());
        }
    }
}