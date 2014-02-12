using System.Collections.Generic;
using System.IO;

namespace StructureMap.Diagnostics
{
    public class TextTreeWriter
    {
        private readonly TreeSection _top = new TreeSection(new NulloPadding());
        private readonly Stack<TreeSection> _sections = new Stack<TreeSection>();

        public TextTreeWriter(IBulletStyle bullets = null)
        {
            if (bullets != null)
            {
                _top.BulletStyle = bullets;
            }
            _sections.Push(_top);
        }

        public int LineCount
        {
            get
            {
                return _top.LineCount;
            }
        }

        public ILeftPadding CurrentPadding
        {
            get
            {
                return _sections.Peek().Padding;
            }
        }

        public void StartSection(TreeSection section)
        {
            _sections.Peek().ChildSection(section);
            _sections.Push(section);
        }

        public void StartSection(ILeftPadding padding = null, IBulletStyle bulletStyle = null)
        {
            var section = _sections.Peek().ChildSection(padding ?? new LeftPadding(4), bulletStyle);
            _sections.Push(section);
        }

        public void StartSection<T>(ILeftPadding padding = null) where T : IBulletStyle, new()
        {
            StartSection(padding, new T());
        }


        public void EndSection()
        {
            _sections.Pop();
        }

        public virtual void Line(string text, params object[] parameters)
        {
            _sections.Peek().WriteLine(text, parameters);
        }

        public void WriteAll(TextWriter writer)
        {
            _top.Write(new NulloPadding(), writer);
        }

        public void Separator(char character)
        {
            _sections.Peek().Separator(character);
        }

        public int MaxLength()
        {
            return _top.MaxLength();
        }

        public void BlankLines(int count)
        {

            for (int i = 0; i < count; i++)
            {
                Line("");                
            }
        }
    }

}