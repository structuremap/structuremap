using System;
using NUnit.Framework;
using StructureMap.Diagnostics.TreeView;

namespace StructureMap.Testing.Diagnostics.TreeView
{
    [TestFixture]
    public class TreeWriter_Smoke_Tester
    {
        [Test]
        public void write_single_section_of_text()
        {
            var writer = new TreeWriter();
            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));

            writer.WriteAll(Console.Out);
        }

        [Test]
        public void write_single_section_of_text_with_astericks()
        {
            var writer = new TreeWriter(new Astericks());
            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));

            writer.WriteAll(Console.Out);
        }

        [Test]
        public void write_single_section_of_text_with_numbers()
        {
            var writer = new TreeWriter(new Numbered());
            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));

            writer.WriteAll(Console.Out);
        }

        [Test]
        public void write_single_section_of_text_with_outline_with_bars()
        {
            var writer = new TreeWriter(new Outline());
            "abcdefghijk".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));


            writer.StartSection(5);
            "LMNOPQRS".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));
            writer.EndSection();

            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));

            writer.WriteAll(Console.Out);
        }

        [Test]
        public void nested_bullets()
        {
            var writer = new TreeWriter();
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

//        [Test]
//        public void separator_line_up_top_and_bottom()
//        {
//            var writer = new TreeWriter();
//            writer.Separator('=');
//            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));
//            writer.Separator('=');
//
//            writer.WriteAll(Console.Out);
//        }
//
//        [Test]
//        public void separator_line_up_top_and_bottom_with_bullets()
//        {
//            var writer = new TreeWriter(new Astericks());
//            writer.Separator('=');
//            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));
//            writer.Separator('=');
//
//            writer.WriteAll(Console.Out);
//        }

//        [Test]
//        public void separator_within_a_nested_section()
//        {
//            var writer = new TreeWriter();
//            writer.Line("First List");
//            writer.StartSection<Astericks>();
//
//            writer.Separator('=');
//            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));
//            writer.Separator('=');
//            writer.EndSection();
//
//            writer.Line("More text");
//
//            writer.StartSection<Numbered>();
//            writer.Separator('~');
//            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(10, c)));
//            writer.Separator('~');
//
//            writer.EndSection();
//
//            writer.WriteAll(Console.Out);
//        }
//
//        [Test]
//        public void deep_nested_sections_and_separators()
//        {
//            var writer = new TreeWriter();
//            writer.Separator('*');
//            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));
//
//            writer.StartSection<Astericks>();
//            writer.Separator('&');
//            "LMNOPQRS".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));
//
//            writer.StartSection<Numbered>();
//            writer.Separator('~');
//            "TUVWXYZ".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));
//            writer.Separator('~');
//            writer.EndSection();
//
//            "1234567".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));
//            writer.Separator('&');
//            writer.EndSection();
//
//            "GHIJKLMNO".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));
//
//            writer.Separator('*');
//
//            writer.WriteAll(Console.Out);
//        }

        [Test]
        public void deep_nested_sections_with_left_border()
        {
            var writer = new TreeWriter();
            "ABCDEFGHIJK".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));

            writer.StartSection<Astericks>();
            "LMNOPQRS".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));

            writer.StartSection<Numbered>();
            "TUVWXYZ".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));
            writer.EndSection();

            "1234567".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));
            writer.EndSection();

            "GHIJKLMNO".ToCharArray().Each(c => writer.Line("".PadRight(20, c)));


            writer.WriteAll(Console.Out);
        }
    }
}