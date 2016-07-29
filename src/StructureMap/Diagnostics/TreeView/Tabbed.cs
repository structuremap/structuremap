namespace StructureMap.Diagnostics.TreeView
{
    public class Tabbed : Section
    {
        private readonly string _buffer;

        public override int TabWidth
        {
            get { return _buffer.Length; }
        }

        public Tabbed() : this(4)
        {
        }

        public Tabbed(int tabWidth)
        {
            _buffer = "".PadRight(tabWidth);
        }

        protected override void applyBuffer(Line line, int index)
        {
            line.Prepend(_buffer);
        }
    }
}