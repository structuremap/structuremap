using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Hosting;
using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.DocumentationExamples;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class HybridBuildPolicyTester
    {
        [Test]
        public void run_without_an_httpcontext()
        {
            var policy = new HybridBuildPolicy(){InnerPolicy = new BuildPolicy()};
            var instance = new SmartInstance<RemoteService>();
            var object1 = policy.Build(new BuildSession(), typeof (IService), instance);
            var object2 = policy.Build(new BuildSession(), typeof(IService), instance);
            var object3 = policy.Build(new BuildSession(), typeof(IService), instance);
        
            object1.ShouldNotBeNull();
            object2.ShouldNotBeNull();
            object3.ShouldNotBeNull();

            object1.ShouldBeTheSameAs(object2).ShouldBeTheSameAs(object3);
        }

    }

    
}