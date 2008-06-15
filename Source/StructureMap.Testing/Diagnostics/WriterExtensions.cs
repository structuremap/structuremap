using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StructureMap.Testing.Diagnostics
{
    public static class WriterExtensions
    {
        public static void WriteLine(this StringWriter writer, string format, params string[] args)
        {
            string message = string.Format(format, args);
            writer.WriteLine(message);
        }

        public static void WriteSeparator(this StringWriter writer)
        {
            writer.WriteLine("-----------------------------------------------------------------------------------------------------");
        }
    }
}
