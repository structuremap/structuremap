using System;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// Makes sure that every request for this object returns a unique object
    /// </summary>
    public class UniquePerRequestLifecycle : ILifecycle
    {
        public void EjectAll()
        {
            
        }

        public IObjectCache FindCache()
        {
            return new NulloObjectCache();
        }

        //#region IBuildInterceptor Members
        
        //private IBuildPolicy _innerPolicy = new BuildPolicy();
        
        //public IBuildPolicy InnerPolicy
        //{
        //    get { return _innerPolicy; }
        //    set { _innerPolicy = value; }
        //}

        //#endregion

        //#region IBuildPolicy Members

        //public object Build(BuildSession buildSession, Type pluginType, Instance instance)
        //{
        //    //insert a default object creator
        //    buildSession.RegisterDefault(pluginType, () => InnerPolicy.Build(buildSession, pluginType, instance));

        //    //build this object for the first time
        //    return buildSession.CreateInstance(pluginType);
        //}

        //public IBuildPolicy Clone()
        //{
        //    return new UniquePerRequestInterceptor();
        //}

        //public void EjectAll()
        //{
        //    InnerPolicy.EjectAll();
        //}

        //#endregion
    }
}
