using System;

namespace StructureMap.Pipeline
{
    public class DefaultInstance : Instance
    {
        public DefaultInstance()
        {
            CopyAsIsWhenClosingInstance = true;
        }

        protected override object build(Type pluginType, BuildSession session)
        {
            if (EnumerableInstance.IsEnumerable(pluginType))
            {
                var enumerable = new EnumerableInstance(pluginType, null);
                return enumerable.Build(pluginType, session);
            }

            return session.GetInstance(pluginType);
        }

        protected override string getDescription()
        {
            return "Default";
        }
    }
}