using System.Diagnostics;
using System.Windows.Forms;

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
                x.For<IDoMore<string>>().TheDefaultIsConcreteType<DoForStrings>();
                x.For<IDoThat>().AddInstances(i =>
                {
                    i.OfConcreteType<DoThat>().WithName("Red");
                    i.OfConcreteType<DoThat>().WithName("Blue");
                });
            });

            ObjectFactory.Initialize(i => i.For<IDoThat>().TheDefaultIsConcreteType<DoThat>());
            Debug.WriteLine(container.WhatDoIHave());

            ContainerDetail details = ContainerVisualizerObjectSource.BuildContainerDetails(container);
            Application.Run(new ContainerForm(details));
        }
    }
}