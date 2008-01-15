using System;
using StructureMap.Configuration;

namespace StructureMap.Client.Views
{
    [Pluggable("Index")]
    public class IndexColumn : IColumn
    {
        private int _index = 0;

        public IndexColumn()
        {
        }

        #region IColumn Members

        public void Initialize(Type subjectType)
        {
            // no-op;
        }

        public string HeaderText
        {
            get { return "Index"; }
        }

        public void CreateCell(TableMaker maker, GraphObject subject)
        {
            _index++;

            maker.AddHeader(_index.ToString());
        }

        #endregion
    }
}