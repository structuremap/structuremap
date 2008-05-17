using System.IO;

namespace StructureMap.Diagnostics
{
    internal class TextLine : Line
    {
        private readonly string[] _contents;

        internal TextLine(string[] contents)
        {
            _contents = contents;
        }

        public void OverwriteCounts(CharacterWidth[] widths)
        {
            for (int i = 0; i < widths.Length; i++)
            {
                CharacterWidth width = widths[i];
                width.SetWidth(_contents[i].Length);
            }
        }

        public void Write(TextWriter writer, CharacterWidth[] widths)
        {
            for (int i = 0; i < widths.Length; i++)
            {
                CharacterWidth width = widths[i];
                writer.Write(_contents[i].PadRight(width.Width));
            }
        }
    }
}