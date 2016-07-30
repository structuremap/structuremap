using System;
using System.Collections.Generic;

namespace StructureMap.Diagnostics.TreeView
{
    public class Outline : Section
    {
        private readonly int _tabWidth;

        public Outline() : this(4)
        {
        }

        public Outline(int tabWidth)
        {
            _tabWidth = tabWidth;
        }

        public override int TabWidth
        {
            get { return _tabWidth; }
        }

        protected override void applyBufferingToChildSectionLines(IEnumerable<Line> sectionLines, int index)
        {
            if (isSubsequentLine(index))
            {
                var buffer = (Convert.ToChar(9475).ToString() + " ").PadLeft(_tabWidth);
                sectionLines.Each(x => x.Prepend(buffer));
            }
        }

        protected override void applyBuffer(Line line, int index)
        {
            var buffer = isSubsequentLine(index)
                ? (Convert.ToChar(9507).ToString() + " ").PadLeft(_tabWidth)
                : (Convert.ToChar(9495).ToString() + " ").PadLeft(_tabWidth);

            line.Prepend(buffer);
        }
    }
}