using StructureMap.Pipeline;

namespace StructureMap
{
    public interface IExplicitProperty
    {
        ExplicitArgsExpression EqualTo(object value);
    }

    public class ExplicitArgsExpression : IExplicitProperty
    {
        private readonly ExplicitArguments _args = new ExplicitArguments();
        private readonly IInstanceManager _manager;
        private string _lastArgName;

        internal ExplicitArgsExpression(IInstanceManager manager)
        {
            _manager = manager;
        }

        #region IExplicitProperty Members

        ExplicitArgsExpression IExplicitProperty.EqualTo(object value)
        {
            _args.SetArg(_lastArgName, value);
            return this;
        }

        #endregion

        public ExplicitArgsExpression With<T>(T arg)
        {
            _args.Set<T>(arg);
            return this;
        }

        public IExplicitProperty With(string argName)
        {
            _lastArgName = argName;
            return this;
        }


        public T GetInstance<T>()
        {
            return _manager.CreateInstance<T>(_args);
        }
    }
}