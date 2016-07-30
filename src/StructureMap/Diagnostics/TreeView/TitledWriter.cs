using System;
using System.IO;
using System.Linq;

namespace StructureMap.Diagnostics.TreeView
{
    public class TitledWriter : IDisposable
    {
        private readonly string _title;
        private readonly TreeWriter _writer;
        private bool _hasWrittenTitle;

        public TitledWriter(string title, TreeWriter writer)
        {
            _title = title;
            _writer = writer;
        }

        public TreeWriter Writer
        {
            get { return _writer; }
        }

        public void Line(string format, params object[] parameters)
        {
            if (_hasWrittenTitle)
            {
                _writer.Line("".PadRight(_title.Length) + format, parameters);
            }
            else
            {
                _writer.Line(_title + format, parameters);
                _hasWrittenTitle = true;
                _writer.StartSection(_title.Length);
            }
        }

        public void Dispose()
        {
            if (_hasWrittenTitle)
            {
                _writer.EndSection();
            }
        }
    }
}