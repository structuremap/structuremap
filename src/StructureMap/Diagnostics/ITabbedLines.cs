using System.IO;

namespace StructureMap.Diagnostics
{
    public interface ITabbedLines
    {
        void Write(ILeftPadding padding, TextWriter writer);
        int MaxLength();
        int LineCount { get; }
    }
}