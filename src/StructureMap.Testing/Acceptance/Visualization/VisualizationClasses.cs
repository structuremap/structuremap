using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap.Testing.Acceptance.Visualization
{
    // SAMPLE: VisualizationRegistry
    public class VisualizationRegistry : Registry
    {
        public VisualizationRegistry()
        {
            // The main ILogVisualizer service
            For<ILogVisualizer>().Use<LogVisualizer>();

            // A default, fallback visualizer
            For(typeof(IVisualizer<>)).Use(typeof(DefaultVisualizer<>));

            // Auto-register all concrete types that "close"
            // IVisualizer<TLog>
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.ConnectImplementationsToTypesClosing(typeof(IVisualizer<>));
            });

        }
    }
    // ENDSAMPLE

    // SAMPLE: IVisualizer<T>
    public interface IVisualizer<TLog>
    {
        string ToHtml(TLog log);
    }
    // ENDSAMPLE

    // SAMPLE: ILogVisualizer
    public interface ILogVisualizer
    {
        // If we already know what the type of log we have
        string ToHtml<TLog>(TLog log);

        // If we only know that we have a log object
        string ToHtml(object log);
    }
    // ENDSAMPLE

    // SAMPLE: DefaultVisualizer
    public class DefaultVisualizer<TLog> : IVisualizer<TLog>
    {
        public string ToHtml(TLog log)
        {
            return string.Format("<div>{0}</div>", log);
        }
    }
    // ENDSAMPLE


    public class SpecialLog { }

    public class TypicalLog { }
    public class TaskAssigned { }
    public class IssueResolved { }
    public class Comment { }


    public class IssueCreated { }

    // SAMPLE: specific-visualizers
    public class IssueCreatedVisualizer : IVisualizer<IssueCreated>
    {
        public string ToHtml(IssueCreated log)
        {
            return "special html for an issue being created";
        }
    }

    public class IssueResolvedVisualizer : IVisualizer<IssueResolved>
    {
        public string ToHtml(IssueResolved log)
        {
            return "special html for issue resolved";
        }
    }
    // ENDSAMPLE


    public class Visualizer
    {
        private readonly IContainer _container;

        public Visualizer(IContainer container)
        {
            _container = container;
        }

        // SAMPLE: to-html-already-knowning-the-log-type
        public string ToHtml<TLog>(TLog log)
        {
            // _container is a reference to an IContainer object
            return _container.GetInstance<IVisualizer<TLog>>().ToHtml(log);
        }
        // ENDSAMPLE
    }

    // SAMPLE: LogVisualizer
    public class LogVisualizer : ILogVisualizer
    {
        private readonly IContainer _container;

        // Take in the IContainer directly so that
        // yes, you can use it as a service locator
        public LogVisualizer(IContainer container)
        {
            _container = container;
        }

        // It's easy if you already know what the log
        // type is
        public string ToHtml<TLog>(TLog log)
        {
            return _container.GetInstance<IVisualizer<TLog>>()
                .ToHtml(log);
        }

        public string ToHtml(object log)
        {
            // The ForObject() method uses the 
            // log.GetType() as the parameter to the open
            // type Writer<T>, and then resolves that
            // closed type from the container and
            // casts it to IWriter for you
            return _container.ForObject(log)
                .GetClosedTypeOf(typeof (Writer<>))
                .As<IWriter>()
                .Write(log);
        }

        public string ToHtml2(object log)
        {
            // The ForGenericType() method is again creating
            // a closed type of Writer<T> from the Container
            // and casting it to IWriter
            return _container.ForGenericType(typeof (Writer<>))
                .WithParameters(log.GetType())
                .GetInstanceAs<IWriter>()
                .Write(log);
        }

        // The IWriter and Writer<T> class below are
        // adapters to go from "object" to <T>() signatures
        public interface IWriter
        {
            string Write(object log);
        }

        public class Writer<T> : IWriter
        {
            private readonly IVisualizer<T> _visualizer;

            public Writer(IVisualizer<T> visualizer)
            {
                _visualizer = visualizer;
            }

            public string Write(object log)
            {
                return _visualizer.ToHtml((T) log);
            }
        }
    }
    // ENDSAMPLE

}