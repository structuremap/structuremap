namespace StructureMap.Diagnostics.TreeView
{
    public class Astericks : Section
    {
        private readonly string _buffer;

        public Astericks() : this(5)
        {
        }

        public Astericks(int tabWidth)
        {
            _buffer = "* ".PadLeft(tabWidth);
        }

        public override int TabWidth
        {
            get { return _buffer.Length; }
        }

        protected override void applyBuffer(Line line, int index)
        {
            line.Prepend(_buffer);
        }
    }
}