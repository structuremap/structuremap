using System.Collections.Generic;
using System.IO;

namespace StructureMap.Diagnostics.TreeView
{
    public class TreeWriter
    {
        private readonly Section _top;
        private readonly Stack<Section> _sections = new Stack<Section>();

        public TreeWriter(Section top = null)
        {
            _top = top ?? new Section();

            _sections.Push(_top);
        }

        public void StartSection(Section section)
        {
            _sections.Peek().ChildSection(section);
            _sections.Push(section);
        }


        public void StartSection<T>() where T : Section, new()
        {
            StartSection(new T());
        }


        public void EndSection()
        {
            _sections.Pop();
        }

        public virtual void Line(string text, params object[] parameters)
        {
            _sections.Peek().Write(text, parameters);
        }

        public void WriteAll(TextWriter writer)
        {
            _top.ToLines().Each(x => writer.WriteLine(x.Text));
        }

        public void BlankLines(int count)
        {

            for (int i = 0; i < count; i++)
            {
                Line("");
            }
        }

        public void StartSection(int tabWidth)
        {
            StartSection(new Tabbed(tabWidth));
        }
    }
}