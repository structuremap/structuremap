using System.IO;

namespace StructureMap.Diagnostics
{
    internal interface Line
    {
        void OverwriteCounts(CharacterWidth[] widths);
        void Write(TextWriter writer, CharacterWidth[] widths);
    }
}