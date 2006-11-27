using System;

namespace StructureMap.DataAccess.JSON
{
	public class NumericField : Field
	{
		public NumericField(int index, string name) : base(index, name)
		{
		}

		protected override void writeProperty(string name, object rawValue, JSONObject target)
		{
			target.AddNumber(name, rawValue);
		}
	}
}
