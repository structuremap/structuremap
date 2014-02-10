using System;
using NUnit.Framework;
using Rhino.Mocks.Constraints;
using StructureMap.Diagnostics;

namespace StructureMap.Testing.Diagnostics
{
    [TestFixture]
    public class TextTreeWriter_Smoke_Tester
    {
        [Test]
        public void write_single_section_of_text()
        {
            var writer = new TextTreeWriter();
            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));

            writer.WriteAll(Console.Out);
        }

        [Test]
        public void write_single_section_of_text_with_astericks()
        {
            var writer = new TextTreeWriter(new Astericks());
            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));

            writer.WriteAll(Console.Out);
        }

        [Test]
        public void write_single_section_of_text_with_numbers()
        {
            var writer = new TextTreeWriter(new Numbered());
            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));

            writer.WriteAll(Console.Out);
        }

        [Test]
        public void write_single_section_of_text_with_outline_with_bars()
        {
            var writer = new TextTreeWriter(new OutlineWithBars());
            "abcdefghijk".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));

            writer.WriteAll(Console.Out);
        }

        [Test]
        public void nested_bullets()
        {
            var writer = new TextTreeWriter();
            writer.Line("First List");
            writer.StartSection<Astericks>();

            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));

            writer.EndSection();

            writer.Line("More text");

            writer.StartSection<Numbered>();
            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));


            writer.EndSection();

            writer.WriteAll(Console.Out);

        }

        [Test]
        public void separator_line_up_top_and_bottom()
        {
            var writer = new TextTreeWriter();
            writer.Separator('=');
            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));
            writer.Separator('=');

            writer.WriteAll(Console.Out);
        }

        [Test]
        public void separator_line_up_top_and_bottom_with_bullets()
        {
            var writer = new TextTreeWriter(new Astericks());
            writer.Separator('=');
            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));
            writer.Separator('=');

            writer.WriteAll(Console.Out);
        }

        [Test]
        public void separator_within_a_nested_section()
        {
            var writer = new TextTreeWriter();
            writer.Line("First List");
            writer.StartSection<Astericks>();

            writer.Separator('=');
            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));
            writer.Separator('=');
            writer.EndSection();

            writer.Line("More text");

            writer.StartSection<Numbered>();
            writer.Separator('~');
            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));
            writer.Separator('~');

            writer.EndSection();

            writer.WriteAll(Console.Out);
        }

        [Test]
        public void deep_nested_sections_and_separators()
        {
            var writer = new TextTreeWriter();
            writer.Separator('*');
            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));

            writer.StartSection<Astericks>();
            writer.Separator('&');
            "LMNOPQRS".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));

            writer.StartSection<Numbered>();
            writer.Separator('~');
            "TUVWXYZ".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));
            writer.Separator('~');
            writer.EndSection();

            "1234567".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));
            writer.Separator('&');
            writer.EndSection();

            "GHIJKLMNO".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));

            writer.Separator('*');

            writer.WriteAll(Console.Out);
        }
    }
}