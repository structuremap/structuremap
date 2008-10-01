using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture]
    public class StructureMapConfigurationDefensiveChecksTester
    {
        private void shouldThrowExceptionWhenSealed(Action action)
        {
            StructureMapConfiguration.ResetAll();
            StructureMapConfiguration.Seal();

            try
            {
                action();
                Assert.Fail("Should have thrown exception");
            }
            catch (StructureMapException ex)
            {
                ex.ErrorCode.ShouldEqual(50);
            }
        }

        [Test]
        public void Ensure_defensive_check_is_always_thrown_when_StructureMapConfiguration_is_sealed()
        {
            shouldThrowExceptionWhenSealed(() => StructureMapConfiguration.AddRegistry(null));
            shouldThrowExceptionWhenSealed(() => StructureMapConfiguration.BuildInstancesOf<IGateway>());
            shouldThrowExceptionWhenSealed(() => StructureMapConfiguration.CreateProfile("something"));
            shouldThrowExceptionWhenSealed(() => StructureMapConfiguration.ForRequestedType(typeof(IGateway)));
            shouldThrowExceptionWhenSealed(() => StructureMapConfiguration.IgnoreStructureMapConfig = true);
            shouldThrowExceptionWhenSealed(() => StructureMapConfiguration.IncludeConfigurationFromFile("something"));
            shouldThrowExceptionWhenSealed(() => StructureMapConfiguration.IncludeConfigurationFromNode(null, null));
            shouldThrowExceptionWhenSealed(() => StructureMapConfiguration.PullConfigurationFromAppConfig = true);
            shouldThrowExceptionWhenSealed(() => StructureMapConfiguration.RegisterInterceptor(null));
            shouldThrowExceptionWhenSealed(() => StructureMapConfiguration.Scan(x => {}));
            shouldThrowExceptionWhenSealed(() => StructureMapConfiguration.TheDefaultProfileIs("something"));
            shouldThrowExceptionWhenSealed(() => StructureMapConfiguration.UseDefaultStructureMapConfigFile = true);
        }
    }
}
