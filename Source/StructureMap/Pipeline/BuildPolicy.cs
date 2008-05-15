using System;
using System.Data;

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
            
            // TODO:  error handling around the interception
            return buildSession.ApplyInterception(pluginType, builtObject);
        }

        public IBuildPolicy Clone()
        {
            return this;
        }

        #endregion
    }
}