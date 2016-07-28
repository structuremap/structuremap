
namespace StructureMap.Testing.Widget
{
    public interface IList
    {
        int Count { get; }
    }

    public class StringList : IList
    {
        public string[] values;

        public StringList(string[] Values)
        {
            values = Values;
        }

        #region IList Members

        public int Count { get { return values.Length; } }

        #endregion
    }

    public class IntegerList : IList
    {
        public int[] values;

        public IntegerList(int[] Values)
        {
            values = Values;
        }

        #region IList Members

        public int Count { get { return values.Length; } }

        #endregion
    }
}