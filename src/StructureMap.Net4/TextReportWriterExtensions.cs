using System;
using StructureMap.Diagnostics;

namespace StructureMap
{
    public static class TextReportWriterExtensions
    {
        public static void DumpToConsole(this TextReportWriter writer)
        {
            Console.WriteLine(writer.Write());
        }
    }
}