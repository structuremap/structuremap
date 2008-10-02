using System;
using System.Data;

namespace StructureMap.DataAccess.JSON
{
    public class Field : IField
    {
        private readonly int _index;
        private readonly string _name;

        public Field(int index, string name)
        {
            _index = index;
            _name = name;
        }

        #region IField Members

        public void Write(JSONObject target, DataRow row)
        {
            if (row.IsNull(_index))
            {
                target.AddNull(_name);
            }
            else
            {
                writeProperty(_name, row[_index], target);
            }
        }

        #endregion

        public static Field[] GetFields(DataTable table)
        {
            var returnValue = new Field[table.Columns.Count];


            for (int i = 0; i < table.Columns.Count; i++)
            {
                DataColumn column = table.Columns[i];
                string name = column.ColumnName;
                Type type = column.DataType;
                returnValue[i] = CreateField(i, name, type);
            }

            return returnValue;
        }

        public static Field CreateField(int index, string name, Type type)
        {
            switch (type.Name)
            {
                case "String":
                    return new Field(index, name);

                case "DateTime":
                    return new DateTimeField(index, name);

                default:
                    return new NumericField(index, name);
            }
        }

        protected virtual void writeProperty(string name, object rawValue, JSONObject target)
        {
            target.AddString(name, rawValue.ToString().Replace("\n", "~~~").Replace("\r", ""));
        }
    }
}