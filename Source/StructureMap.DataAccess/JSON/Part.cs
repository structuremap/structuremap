using System.Text;

namespace StructureMap.DataAccess.JSON
{
    public abstract class Part
    {
        public abstract void Write(StringBuilder sb);

        public string ToJSON()
        {
            StringBuilder sb = new StringBuilder();
            Write(sb);
            return sb.ToString().Trim();
        }
    }
}