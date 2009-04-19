using System;
using System.Collections;
using System.Web;
using StructureMap.Attributes;

namespace StructureMap.Pipeline
{
    public class HttpSessionBuildPolicy : HttpContextBuildPolicy
    {
        protected override IDictionary findHttpDictionary()
        {
            return new SessionWrapper(HttpContext.Current.Session);
        }

        public override string ToString()
        {
            return InstanceScope.HttpSession.ToString();
        }
    }


    public class HttpSessionLifecycle : HttpContextLifecycle
    {
        protected override IDictionary findHttpDictionary()
        {
            return new SessionWrapper(HttpContext.Current.Session);
        }
    }

    public class HybridSessionLifecycle : HttpLifecycleBase<HttpSessionLifecycle, ThreadLocalStorageLifecycle>
    {
        
    }

    public class HybridSessionBuildPolicy : HttpBuildPolicyBase<HttpSessionBuildPolicy, ThreadLocalStoragePolicy>
    {
        public override IBuildPolicy Clone()
        {
            return new HybridSessionBuildPolicy() { InnerPolicy = InnerPolicy.Clone() };
        }

        public override string ToString()
        {
            return InstanceScope.HybridHttpSession.ToString();
        }
    }
}