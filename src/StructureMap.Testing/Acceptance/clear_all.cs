using System.Diagnostics;
using NUnit.Framework;
using Shouldly;
using StructureMap.Configuration.DSL;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class clear_all
    {
        // SAMPLE: ImportantClientWidget
        public class ImportantClientWidget : IWidget { }

        public class ImportantClientServices : Registry
        {
            public ImportantClientServices()
            {
                For<IWidget>().ClearAll().Use<ImportantClientWidget>();
            }
        }
        // ENDSAMPLE

        // SAMPLE: clear_all_in_action
        [Test]
        public void clear_all_in_action()
        {
            var container = new Container(_ =>
            {
                _.For<IWidget>().Use<AWidget>();

                
                _.IncludeRegistry<ImportantClientServices>();
            });

            container.GetInstance<IWidget>()
                .ShouldBeOfType<ImportantClientWidget>();

            Debug.WriteLine(container.WhatDoIHave(pluginType:typeof(IWidget)));
        }
        // ENDSAMPLE
    }

    
}