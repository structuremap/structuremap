using System;
using System.Text;

namespace StructureMap.DataAccess.JSON
{
    public class JSONProperty : Part
    {
        private readonly string _name;
        private readonly string _text;

        public static Part Number(string name, object number)
        {
            return new JSONProperty(name, number.ToString());
        }

        public static Part String(string name, string stringValue)
        {
            string text = string.Format("'{0}'", stringValue);
            return new JSONProperty(name, text);
        }

        public static Part DateTime(string name, DateTime date)
        {
            string dateString = string.Format("new Date({0}, {1}, {2}, {3}, {4}, {5})",
                                              date.Year,
                                              date.Month,
                                              date.Day,
                                              date.Hour,
                                              date.Minute,
                                              date.Second
                );

            return new JSONProperty(name, dateString);
        }

        public static Part Null(string name)
        {
            return new JSONProperty(name, "null");
        }

        private JSONProperty(string name, string text)
        {
            _name = name;
            _text = text;
        }

        public override void Write(StringBuilder sb)
        {
            sb.AppendFormat("{0}: {1}", _name, _text);
        }
    }
}