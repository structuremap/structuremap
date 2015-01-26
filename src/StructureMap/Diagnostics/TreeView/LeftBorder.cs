namespace StructureMap.Diagnostics.TreeView
{
    public class LeftBorder : Section
    {
        private readonly string _buffer;

        public LeftBorder(int tabWidth = 5)
        {
            _buffer = "| ".PadLeft(tabWidth);
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