using System.IO;

namespace StructureMap.Diagnostics
{
    internal class TextLine : Line
    {
        private readonly string[] _contents;

        internal TextLine(string[] contents)
        {
            _contents = contents;
            for (var i = 0; i < contents.Length; i++)
            {
                if (contents[i] == null) contents[i] = string.Empty;
            }
        }

        #region Line Members

        public void OverwriteCounts(CharacterWidth[] widths)
        {
            for (var i = 0; i < widths.Length; i++)
            {
                var width = widths[i];
                width.SetWidth(_contents[i].Length);
            }
        }

        public void Write(TextWriter writer, CharacterWidth[] widths)
        {
            for (var i = 0; i < widths.Length; i++)
            {
                var width = widths[i];
                writer.Write(_contents[i].PadRight(width.Width));
            }
        }

        #endregion
    }
}