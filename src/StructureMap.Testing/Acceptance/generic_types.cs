using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Testing.Acceptance.Visualization;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class generic_types
    {
        // SAMPLE: register_open_generic_type
        [Test]
        public void register_open_generic_type()
        {
            var container = new Container(_ =>
            {
                _.For(typeof (IVisualizer<>)).Use(typeof (DefaultVisualizer<>));
            });

            
            Debug.WriteLine(container.WhatDoIHave(@namespace:"StructureMap.Testing.Acceptance.Visualization"));
            

            container.GetInstance<IVisualizer<IssueCreated>>()
                .ShouldBeOfType<DefaultVisualizer<IssueCreated>>();

            Debug.WriteLine(container.WhatDoIHave(@namespace: "StructureMap.Testing.Acceptance.Visualization"));
            


            container.GetInstance<IVisualizer<IssueResolved>>()
                .ShouldBeOfType<DefaultVisualizer<IssueResolved>>();
        }
        // ENDSAMPLE

        [Test]
        public void using_visualizer()
        {
            // SAMPLE: using-visualizer-knowning-the-type   
            // Just setting up a Container and ILogVisualizer
            var container = Container.For<VisualizationRegistry>();
            var visualizer = container.GetInstance<ILogVisualizer>();

            // If I have an IssueCreated lob object...
            var created = new IssueCreated();

            // I can get the html representation:
            var html = visualizer.ToHtml(created);
            // ENDSAMPLE
        }

        [Test]
        public void a_bunch_of_logs()
        {
            // SAMPLE: using-visualizer-not-knowing-the-type
            var logs = new object[]
            {
                new IssueCreated(), 
                new TaskAssigned(), 
                new Comment(), 
                new IssueResolved()
            };

            // SAMPLE: using-visualizer-knowning-the-type   
            // Just setting up a Container and ILogVisualizer
            var container = Container.For<VisualizationRegistry>();
            var visualizer = container.GetInstance<ILogVisualizer>();

            var items = logs.Select(visualizer.ToHtml);
            var html = string.Join("<hr />", items);
            // ENDSAMPLE
        }

        // SAMPLE: generic-defaults-with-fallback
        [Test]
        public void generic_defaults()
        {
            var container = new Container(_ =>
            {
                // The default visualizer just like we did above
                _.For(typeof(IVisualizer<>)).Use(typeof(DefaultVisualizer<>));

                // Register a specific visualizer for IssueCreated
                _.For<IVisualizer<IssueCreated>>().Use<IssueCreatedVisualizer>();
            });


            // We have a specific visualizer for IssueCreated
            container.GetInstance<IVisualizer<IssueCreated>>()
                .ShouldBeOfType<IssueCreatedVisualizer>();

            // We do not have any special visualizer for TaskAssigned,
            // so fall back to the DefaultVisualizer<T>
            container.GetInstance<IVisualizer<TaskAssigned>>()
                .ShouldBeOfType<DefaultVisualizer<TaskAssigned>>();
        }
        // ENDSAMPLE

        // SAMPLE: visualization-registry-in-action
        [Test]
        public void visualization_registry()
        {
            var container = Container.For<VisualizationRegistry>();

            Debug.WriteLine(container.WhatDoIHave(@namespace: "StructureMap.Testing.Acceptance.Visualization"));

            container.GetInstance<IVisualizer<IssueCreated>>()
                .ShouldBeOfType<IssueCreatedVisualizer>();

            container.GetInstance<IVisualizer<IssueResolved>>()
                .ShouldBeOfType<IssueResolvedVisualizer>();

            // We have no special registration for TaskAssigned,
            // so fallback to the default visualizer
            container.GetInstance<IVisualizer<TaskAssigned>>()
                .ShouldBeOfType<DefaultVisualizer<TaskAssigned>>();
        }
        // ENDSAMPLE
    }


   
}

