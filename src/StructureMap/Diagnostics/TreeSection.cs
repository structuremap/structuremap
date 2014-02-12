using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StructureMap.Diagnostics
{
    public class TreeSection : ITabbedLines
    {
        private readonly ILeftPadding _padding;
        public IBulletStyle BulletStyle = new NulloBulletStyle();
        private readonly IList<ITabbedLines> _lines = new List<ITabbedLines>();

        public TreeSection(ILeftPadding padding)
        {
            _padding = padding;
        }

        public int LineCount
        {
            get
            {
                return _lines.Sum(x => x.LineCount);
            }
        }

        public ILeftPadding Padding
        {
            get { return _padding; }
        }

        public void Write(ILeftPadding padding, TextWriter writer)
        {
            applyBullets();

            var childIndent = padding.ToChild(_padding);
            _lines.Each(x => x.Write(childIndent, writer));
        }

        private void applyBullets()
        {
            BulletStyle.ApplyBullets(_lines.OfType<TabbedLine>());
        }

        public void WriteLine(string format, params object[] parameters)
        {
            _lines.Add(new TabbedLine(format, parameters));
        }

        public TreeSection ChildSection(ILeftPadding padding, IBulletStyle bullets = null)
        {
            var section = new TreeSection(padding);
            if (bullets != null)
            {
                section.BulletStyle = bullets;
            }

            _lines.Add(section);

            return section;
        }

        public int MaxLength()
        {
            applyBullets();

            var max = _lines.Max(x => x.MaxLength());
            if (_lines.OfType<TreeSection>().Any())
            {
                var sectionMax = _lines.OfType<TreeSection>().Max(x => x.MaxLength() + x.Padding.Create().Length);
                if (sectionMax > max)
                    max = sectionMax;
            }

            return max;
        }

        public void Separator(char character)
        {
            _lines.Add(new SeparatorLine(character, this));
        }
    }
}