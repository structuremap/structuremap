using System;

namespace StructureMap.DataAccess.JSON
{
    public class DateTimeField : Field
    {
        public DateTimeField(int index, string name) : base(index, name)
        {
        }

        protected override void writeProperty(string name, object rawValue, JSONObject target)
        {
            target.AddDate(name, (DateTime) rawValue);
        }
    }
}