using System.Collections.Generic;
using System.Linq;

namespace StructureMap.Diagnostics.TreeView
{
    public class Section
    {
        protected readonly List<object> items = new List<object>();

        public virtual int TabWidth
        {
            get
            {
                return 0;
            }
        }

        public void Write(string format, params object[] parameters)
        {
            items.Add(new Line(format.ToFormat(parameters)));
        }

        public void ChildSection(Section section)
        {
            items.Add(section);
        }

        public virtual IEnumerable<Line> ToLines()
        {
            int index = 0;
            foreach (var item in items)
            {
                var line = item as Line;

                if (line != null)
                {
                    applyBuffer(item.As<Line>(), index);
                    yield return line;
                }
                else
                {
                    var child = item.As<Section>();

                    var childLines = child.ToLines().ToArray();
                    applyBufferingToChildSectionLines(childLines, index);

                    foreach (var childLine in childLines)
                    {
                        yield return childLine;
                    }
                }

                index++;
            }
        }

        protected bool isSubsequentLine(int index)
        {
            return items.Skip(index + 1).Any(x => x is Line);
        }

        protected virtual void applyBufferingToChildSectionLines(IEnumerable<Line> sectionLines, int index)
        {
            sectionLines.Each(x => applyBuffer(x, 0));
        }

        protected virtual void applyBuffer(Line line, int index)
        {
            // nothing
        }
    }
}