using System.IO;

namespace StructureMap.Diagnostics
{
    public class SeparatorLine : ITabbedLines
    {
        private readonly char _character;
        private readonly TreeSection _parent;

        public SeparatorLine(char character, TreeSection parent)
        {
            _character = character;
            _parent = parent;
        }

        public void Write(ILeftPadding padding, TextWriter writer)
        {
            writer.WriteLine(padding.Create() + _parent.MaxLength().Line(_character));
        }

        public char Character
        {
            get { return _character; }
        }

        public int MaxLength()
        {
            return 1;
        }

        public int LineCount { get { return 1; } }
    }
}