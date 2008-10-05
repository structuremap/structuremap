using System.IO;

namespace StructureMap.Diagnostics
{
    internal class DividerLine : Line
    {
        private readonly char _character;

        internal DividerLine(char character)
        {
            _character = character;
        }

        #region Line Members

        public void OverwriteCounts(CharacterWidth[] widths)
        {
            // no-op
        }

        public void Write(TextWriter writer, CharacterWidth[] widths)
        {
            foreach (CharacterWidth width in widths)
            {
                writer.Write(string.Empty.PadRight(width.Width, _character));
            }
        }

        #endregion
    }
}