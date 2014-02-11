using System.IO;

namespace StructureMap.Diagnostics
{
    public class TabbedLine : ITabbedLines
    {
        private readonly string _text;

        public TabbedLine(string text, params object[] parameters)
        {
            _text = text.ToFormat(parameters);
            Bullet = string.Empty;
        }

        public string Bullet { get; set; }

        public void Write(int spaces, TextWriter writer)
        {
            writer.WriteLine(spaces, Bullet + _text);
        }

        public int MaxLength()
        {
            return _text.Length + Bullet.Length;
        }

        public int LineCount {
            get { return 1; }
        }
    }
}