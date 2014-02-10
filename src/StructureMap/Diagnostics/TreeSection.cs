using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StructureMap.Diagnostics
{
    public class TreeSection : ITabbedLines
    {
        private readonly int _indention;
        public IBulletStyle BulletStyle = new NulloBulletStyle();
        private readonly IList<ITabbedLines> _lines = new List<ITabbedLines>();

        public TreeSection(int indention = 4)
        {
            _indention = indention;
        }

        public int Indention
        {
            get { return _indention; }
        }

        public void Write(int spaces, TextWriter writer)
        {
            applyBullets();

            var childIndent = spaces + _indention;
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

        public TreeSection ChildSection(int indention = 4, IBulletStyle bullets = null)
        {
            var section = new TreeSection(indention);
            if (bullets != null)
            {
                section.BulletStyle = bullets;
            }

            _lines.Add(section);

            return section;
        }


        public TreeSection ChildSection<T>(int indention = 4) where T : IBulletStyle, new()
        {
            return ChildSection(indention, new T());
        }

        public int MaxLength()
        {
            applyBullets();

            var max = _lines.Max(x => x.MaxLength());
            if (_lines.OfType<TreeSection>().Any())
            {
                var sectionMax = _lines.OfType<TreeSection>().Max(x => x.MaxLength() + x.Indention);
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