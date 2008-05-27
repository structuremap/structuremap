using System;

namespace StructureMap.Pipeline
{
    public sealed class BuildPolicy : IBuildPolicy
    {
        #region IBuildPolicy Members

        public object Build(IBuildSession buildSession, Type pluginType, Instance instance)
        {
            if (buildSession == null)
            {
                throw new ArgumentNullException("buildSession");
            }

            object builtObject = instance.Build(pluginType, buildSession);

            try
            {
                return buildSession.ApplyInterception(pluginType, builtObject);
            }
            catch (Exception e)
            {
                throw new StructureMapException(308, e, instance.Name, builtObject.GetType());
            }
        }

        public IBuildPolicy Clone()
        {
            return this;
        }

        #endregion
    }
}