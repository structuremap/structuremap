using System.Diagnostics;
using System.Windows.Forms;
using StructureMap.Configuration.DSL;

namespace StructureMap.DebuggerVisualizers.Testing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = new Container(x =>
            {
                x.Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.WithDefaultConventions();
                });
                x.ForRequestedType<IDoMore<string>>().TheDefaultIsConcreteType<DoForStrings>();
                x.ForRequestedType<IDoThat>().AddInstances(i =>
                {
                    i.OfConcreteType<DoThat>().WithName("Red");
                    i.OfConcreteType<DoThat>().WithName("Blue");
                });
            });

            ObjectFactory.Initialize(i => i.ForRequestedType<IDoThat>().TheDefaultIsConcreteType<DoThat>());
            Debug.WriteLine(container.WhatDoIHave());

            ContainerDetail details = ContainerVisualizerObjectSource.BuildContainerDetails(container);
            Application.Run(new ContainerForm(details));
        }
    }
}