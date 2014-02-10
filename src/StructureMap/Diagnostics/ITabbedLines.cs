using System.IO;

namespace StructureMap.Diagnostics
{
    public interface ITabbedLines
    {
        void Write(int spaces, TextWriter writer);
        int MaxLength();
    }
}