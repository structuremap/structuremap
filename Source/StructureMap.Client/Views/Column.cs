using System;
using System.Reflection;
using StructureMap.Configuration;

namespace StructureMap.Client.Views
{
    [Pluggable("Label")]
    public class Column : IColumn
    {
        private readonly string _propertyName;
        private readonly string _headerText;
        private PropertyInfo _property;

        public Column(string propertyName, string headerText)
        {
            _propertyName = propertyName;
            _headerText = headerText;
        }

        public void Initialize(Type subjectType)
        {
            _property = subjectType.GetProperty(_propertyName);
        }

        public string HeaderText
        {
            get { return _headerText; }
        }

        public void CreateCell(TableMaker maker, GraphObject subject)
        {
            object rawValue = _property.GetValue(subject, null);

            string cellText = string.Empty;
            if (rawValue is string[])
            {
                string[] contents = (string[]) rawValue;
                cellText = string.Join(", ", contents);
            }
            else
            {
                cellText = rawValue.ToString();
            }

            createCellContents(maker, cellText, subject);
        }

        protected virtual void createCellContents(TableMaker maker, string cellText, GraphObject subject)
        {
            maker.AddCell(cellText);
        }
    }
}