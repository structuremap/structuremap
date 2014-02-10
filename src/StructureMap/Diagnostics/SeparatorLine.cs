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

        public void Write(int spaces, TextWriter writer)
        {
            writer.WriteLine(spaces, _parent.MaxLength().Line(_character));
        }

        public char Character
        {
            get { return _character; }
        }

        public int MaxLength()
        {
            return 1;
        }
    }
}