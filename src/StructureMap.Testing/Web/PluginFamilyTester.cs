using Shouldly;
using StructureMap.Graph;
using StructureMap.Web;
using StructureMap.Web.Pipeline;
using System;
using Xunit;

namespace StructureMap.Testing.Web
{
    public class PluginFamilyTester
    {
        [Fact]
        public void SetScopeToHttpContext()
        {
            var family = new PluginFamily(typeof(IServiceProvider));
            family.Lifecycle.ShouldBeNull();

            family.SetLifecycleTo(WebLifecycles.HttpContext);
            family.Lifecycle.ShouldBeOfType<HttpContextLifecycle>();
        }

        [Fact]
        public void SetScopeToHybrid()
        {
            var family = new PluginFamily(typeof(IServiceProvider));

            family.SetLifecycleTo(WebLifecycles.Hybrid);
            family.Lifecycle.ShouldBeOfType<HybridLifecycle>();
        }

        [Fact]
        public void set_the_scope_to_session()
        {
            var family = new PluginFamily(typeof(IServiceProvider));
            family.SetLifecycleTo(WebLifecycles.HttpSession);

            family.Lifecycle.ShouldBeOfType<HttpSessionLifecycle>();
        }

        [Fact]
        public void set_the_scope_to_session_hybrid()
        {
            var family = new PluginFamily(typeof(IServiceProvider));
            family.SetLifecycleTo(WebLifecycles.HybridSession);

            family.Lifecycle.ShouldBeOfType<HybridSessionLifecycle>();
        }
    }
}