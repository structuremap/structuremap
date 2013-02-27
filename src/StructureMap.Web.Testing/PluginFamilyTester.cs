﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing;
using StructureMap.Web.Pipeline;

namespace StructureMap.Web.Testing
{
    [TestFixture]
    public class PluginFamilyTester
    {
        [Test]
        public void SetScopeToHttpContext()
        {
            var family = new PluginFamily(typeof(IServiceProvider));
            family.Lifecycle.ShouldBeOfType<TransientLifecycle>();

            family.SetScopeTo(WebLifecycles.HttpContext);
            family.Lifecycle.ShouldBeOfType<HttpContextLifecycle>();
        }

        [Test]
        public void SetScopeToHybrid()
        {
            var family = new PluginFamily(typeof(IServiceProvider));

            family.SetScopeTo(WebLifecycles.Hybrid);
            family.Lifecycle.ShouldBeOfType<HybridLifecycle>();
        }

        [Test]
        public void set_the_scope_to_session()
        {
            var family = new PluginFamily(typeof(IServiceProvider));
            family.SetScopeTo(WebLifecycles.HttpSession);

            family.Lifecycle.ShouldBeOfType<HttpSessionLifecycle>();
        }

        [Test]
        public void set_the_scope_to_session_hybrid()
        {
            var family = new PluginFamily(typeof(IServiceProvider));
            family.SetScopeTo(WebLifecycles.HybridSession);

            family.Lifecycle.ShouldBeOfType<HybridSessionLifecycle>();
        }
    }
}
