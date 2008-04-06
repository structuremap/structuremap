using StructureMap.Pipeline;

namespace StructureMap.Testing.Widget
{
    public interface IList
    {
        int Count { get; }
    }

    [Pluggable("String", "")]
    public class StringList : IList
    {
        public string[] values;

        public StringList(string[] Values)
        {
            values = Values;
        }

        #region IList Members

        public int Count
        {
            get { return values.Length; }
        }

        #endregion
    }

    [Pluggable("Integer", "")]
    public class IntegerList : IList
    {
        public int[] values;

        public IntegerList(int[] Values)
        {
            values = Values;
        }

        #region IList Members

        public int Count
        {
            get { return values.Length; }
        }

        #endregion
    }


    public class StringListBuilder : InstanceBuilder
    {
        public override string ConcreteTypeKey
        {
            get { return null; }
        }


        public override string PluggedType
        {
            get { return null; }
        }


        public override string PluginType
        {
            get { return null; }
        }


        public override object BuildInstance(InstanceMemento memento)
        {
            return null;
        }
    }
}