using System;
using NUnit.Framework;
using Shouldly;
using StructureMap.Graph;
using StructureMap.Web;
using StructureMap.Web.Pipeline;

namespace StructureMap.Testing.Web
{
    [TestFixture]
    public class PluginFamilyTester
    {
        [Test]
        public void SetScopeToHttpContext()
        {
            var family = new PluginFamily(typeof(IServiceProvider));
            family.Lifecycle.ShouldBeNull();

            family.SetLifecycleTo(WebLifecycles.HttpContext);
            family.Lifecycle.ShouldBeOfType<HttpContextLifecycle>();
        }

        [Test]
        public void SetScopeToHybrid()
        {
            var family = new PluginFamily(typeof(IServiceProvider));

            family.SetLifecycleTo(WebLifecycles.Hybrid);
            family.Lifecycle.ShouldBeOfType<HybridLifecycle>();
        }

        [Test]
        public void set_the_scope_to_session()
        {
            var family = new PluginFamily(typeof(IServiceProvider));
            family.SetLifecycleTo(WebLifecycles.HttpSession);

            family.Lifecycle.ShouldBeOfType<HttpSessionLifecycle>();
        }

        [Test]
        public void set_the_scope_to_session_hybrid()
        {
            var family = new PluginFamily(typeof(IServiceProvider));
            family.SetLifecycleTo(WebLifecycles.HybridSession);

            family.Lifecycle.ShouldBeOfType<HybridSessionLifecycle>();
        }
    }
}
