using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using FubuMVC.Core;
using StructureMap.Web.Pipeline;

namespace AspNetHarness
{
    public class Global : System.Web.HttpApplication
    {
        private FubuRuntime _runtime;

        protected void Application_Start(object sender, EventArgs e)
        {
            _runtime = FubuApplication.BootstrapApplication<AspNetApplication>();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            HttpContextLifecycle.DisposeAndClearAll();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            _runtime.Dispose();
        }
    }
}