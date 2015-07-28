using System.IO;

namespace StructureMap.Diagnostics
{
    public static class WriterExtensions
    {
        public static void WriteLine(this TextWriter writer, int spaces, string text)
        {
            writer.WriteLine("".PadRight(spaces) + text);
        }

        public static string Line(this int length, char character)
        {
            return "".PadRight(length, character);
        }
    }
}