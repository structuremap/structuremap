using System.Collections;
using System.Web;
using StructureMap.Pipeline;

namespace StructureMap.Web.Pipeline
{
    public class HttpSessionLifecycle : HttpContextLifecycle
    {
        protected override IDictionary findHttpDictionary()
        {
            return new SessionWrapper(HttpContext.Current.Session);
        }
    }
}