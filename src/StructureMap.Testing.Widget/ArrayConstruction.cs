namespace StructureMap.Testing.Widget
{
    public interface IList
    {
        int Count { get; }
    }

    [Pluggable("String")]
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

    [Pluggable("Integer")]
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