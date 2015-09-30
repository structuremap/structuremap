using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing
{
    [TestFixture]
    public class WhatDidIScan_smoke_tester
    {
        [Test]
        public void what_did_i_scan_usage()
        {
            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();

                    x.WithDefaultConventions();
                    x.RegisterConcreteTypesAgainstTheFirstInterface();
                    x.SingleImplementationsOfInterface();
                });

                _.Scan(x =>
                {
                    x.Description = "Second Scanner";

                    x.AssembliesFromApplicationBaseDirectory(assem => assem.FullName.Contains("Widget"));
                    x.ConnectImplementationsToTypesClosing(typeof(IService<>));
                    x.AddAllTypesOf<IWidget>();
                });
            });

            Debug.WriteLine(container.WhatDidIScan());
        }
    }


}