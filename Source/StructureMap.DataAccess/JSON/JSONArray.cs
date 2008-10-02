using System;
using System.Collections;
using System.Text;

namespace StructureMap.DataAccess.JSON
{
    public class JSONArray : Part
    {
        private readonly ArrayList _parts = new ArrayList();
        private readonly bool _pretty;

        public JSONArray(bool pretty)
        {
            _pretty = pretty;
        }

        public void AddObject(Object obj)
        {
            _parts.Add(obj);
        }

        public override void Write(StringBuilder sb)
        {
            if (_parts.Count == 0)
            {
                sb.Append("[]");
                return;
            }

            string header = _pretty ? "[\n" : "[";
            string separator = _pretty ? ",\n" : ", ";
            string footer = _pretty ? "\n]" : "] ";

            sb.Append(header);
            foreach (Part part in _parts)
            {
                part.Write(sb);
                sb.Append(separator);
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append(footer);
        }
    }
}