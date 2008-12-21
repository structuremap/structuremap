using System;

namespace StructureMap.Pipeline
{
    public sealed class BuildPolicy : IBuildPolicy
    {
        #region IBuildPolicy Members

        public object Build(BuildSession buildSession, Type pluginType, Instance instance)
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

        public void EjectAll()
        {
            // no-op.  Unlike other Container's, StructureMap doesn't hang on to 
            // objects it created as "Transients"
        }

        #endregion

        public bool Equals(BuildPolicy obj)
        {
            return !ReferenceEquals(null, obj);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is BuildPolicy;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}