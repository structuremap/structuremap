using System.Data;

namespace StructureMap.DataAccess.JSON
{
    public class JSONSerializer
    {
        private readonly DataTable _table;
        private Field[] _fields;

        public JSONSerializer(DataTable table)
        {
            _table = table;
        }

        public string CreateJSON()
        {
            _fields = Field.GetFields(_table);

            JSONArray array = new JSONArray(true);


            foreach (DataRow row in _table.Rows)
            {
                writeObject(array, row);
            }

            return array.ToJSON();
        }

        private void writeObject(JSONArray array, DataRow row)
        {
            JSONObject obj = new JSONObject();

            foreach (Field field in _fields)
            {
                field.Write(obj, row);
            }

            array.AddObject(obj);
        }
    }
}