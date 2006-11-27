using System;
using System.Collections;
using System.Text;

namespace StructureMap.DataAccess.JSON
{
	public class JSONObject : Part
	{
		private ArrayList _parts = new ArrayList();
		
		public JSONObject()
		{

		}
		
		public JSONObject AddString(string propertyName, string propertyValue)
		{
			_parts.Add(JSONProperty.String(propertyName, propertyValue));
			return this;
		}
		
		public JSONObject AddNumber(string propertyName, object number)
		{
			_parts.Add(JSONProperty.Number(propertyName, number));
			return this;
		}
		
		public JSONObject AddDate(string propertyName, DateTime date)
		{
			_parts.Add(JSONProperty.DateTime(propertyName, date));
			return this;
		}
		
		public JSONObject AddNull(string propertyName)
		{
			_parts.Add(JSONProperty.Null(propertyName));
			return this;
		}

		public override void Write(StringBuilder sb)
		{
			if (_parts.Count == 0)
			{
				sb.Append("{}");
				return;
			}
			
			sb.Append("{");

			foreach (Part part in _parts)
			{
				part.Write(sb);
				sb.Append(", ");
			}
			
			sb.Remove(sb.Length - 2, 2);
			sb.Append("}");
		}
	}
}
