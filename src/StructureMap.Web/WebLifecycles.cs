using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap.Web.Pipeline;

namespace StructureMap.Web
{
    public static class WebLifecycles
    {
        public static readonly HybridLifecycle Hybrid = new HybridLifecycle();
        public static readonly HttpSessionLifecycle HttpSession = new HttpSessionLifecycle();
        public static readonly HybridSessionLifecycle HybridSession = new HybridSessionLifecycle();
        public static readonly HttpContextLifecycle HttpContext = new HttpContextLifecycle();
    }
}
