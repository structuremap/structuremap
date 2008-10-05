using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace StructureMap.Diagnostics
{
    public class TextReportWriter
    {
        private readonly int _columnCount;
        private readonly List<Line> _lines = new List<Line>();

        public TextReportWriter(int columnCount)
        {
            _columnCount = columnCount;
        }

        public void AddDivider(char character)
        {
            _lines.Add(new DividerLine(character));
        }

        public void AddText(params string[] contents)
        {
            _lines.Add(new TextLine(contents));
        }

        public void AddContent(string contents)
        {
            _lines.Add(new PlainLine(contents));
        }

        public void Write(StringWriter writer)
        {
            CharacterWidth[] widths = CharacterWidth.For(_columnCount);

            foreach (Line line in _lines)
            {
                line.OverwriteCounts(widths);
            }

            for (int i = 0; i < widths.Length - 1; i++)
            {
                CharacterWidth width = widths[i];
                width.Add(5);
            }

            foreach (Line line in _lines)
            {
                writer.WriteLine();
                line.Write(writer, widths);
            }
        }

        public string Write()
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            Write(writer);

            return sb.ToString();
        }

        public void DumpToConsole()
        {
            Console.WriteLine(Write());
        }

        public void DumpToDebug()
        {
            Debug.WriteLine(Write());
        }
    }

    internal class PlainLine : Line
    {
        public PlainLine(string contents)
        {
            Contents = contents;
        }

        public string Contents { get; set; }

        #region Line Members

        public void OverwriteCounts(CharacterWidth[] widths)
        {
            // no-op
        }

        public void Write(TextWriter writer, CharacterWidth[] widths)
        {
            writer.WriteLine(Contents);
        }

        #endregion
    }
}