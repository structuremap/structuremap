using System.Collections;
using System.Web;

namespace StructureMap.Pipeline
{
    public class HttpSessionLifecycle : HttpContextLifecycle
    {
        protected override IDictionary findHttpDictionary()
        {
            return new SessionWrapper(HttpContext.Current.Session);
        }
    }
}