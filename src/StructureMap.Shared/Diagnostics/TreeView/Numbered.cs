using System.Collections.Generic;

namespace StructureMap.Diagnostics.TreeView
{
    public class Numbered : Section
    {
        private readonly int _tabWidth;
        private int _count;

        public Numbered() : this(5)
        {
        }

        public Numbered(int tabWidth)
        {
            _tabWidth = tabWidth;
        }

        public override int TabWidth
        {
            get { return _tabWidth; }
        }

        protected override void applyBufferingToChildSectionLines(IEnumerable<Line> sectionLines, int index)
        {
            var buffer = "".PadLeft(_tabWidth);
            sectionLines.Each(x => x.Prepend(buffer));
        }

        protected override void applyBuffer(Line line, int index)
        {
            _count++;
            var buffer = (_count + ".) ").PadLeft(_tabWidth);

            line.Prepend(buffer);
        }
    }
}