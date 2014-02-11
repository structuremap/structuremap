using System.Collections.Generic;
using System.IO;

namespace StructureMap.Diagnostics
{
    public class TextTreeWriter
    {
        private readonly TreeSection _top = new TreeSection(0);
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

        public void StartSection(int indention = 4, IBulletStyle bulletStyle = null)
        {
            var section = _sections.Peek().ChildSection(indention, bulletStyle);
            _sections.Push(section);
        }

        public void StartSection<T>(int indention = 4) where T : IBulletStyle, new()
        {
            StartSection(indention, new T());
        }


        public void EndSection()
        {
            _sections.Pop();
        }

        public void Line(string text, params object[] parameters)
        {
            _sections.Peek().WriteLine(text, parameters);
        }

        public void WriteAll(TextWriter writer)
        {
            _top.Write(0, writer);
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