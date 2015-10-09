using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using StructureMap.Testing.Acceptance;
using StructureMap.Testing.Widget3;
using IService = StructureMap.Testing.Widget3.IService;

namespace StructureMap.Testing.DocumentationExamples
{
    public interface IDataProvider
    {
    }

    // SAMPLE: setter-injection-with-SetterProperty
    public class Repository
    {
        private IDataProvider _provider;

        // Adding the SetterProperty to a setter directs
        // StructureMap to use this property when
        // constructing a Repository instance
        [SetterProperty]
        public IDataProvider Provider
        {
            set { _provider = value; }
        }

        [SetterProperty]
        public bool ShouldCache { get; set; }
    }
    // ENDSAMPLE

    public class DataProvider : IDataProvider { }

    public class BuildPlans
    {
        public void ShowRepositoryBuildPlan()
        {
            var container = new Container(_ =>
            {
                _.For<IDataProvider>().Use<DataProvider>();
                _.ForConcreteType<Repository>().Configure.Setter<bool>().Is(false);
            });

            Debug.WriteLine(container.Model.For<Repository>().Default.DescribeBuildPlan());
        }
    }

    // SAMPLE: IShippingService
    public interface IShippingService
    {
        void ShipIt();
    }

    // ENDSAMPLE

    public class ShippingWebService : IShippingService
    {
        private readonly string _url;

        public ShippingWebService(string url)
        {
            _url = url;
        }

        public void ShipIt()
        {
            throw new NotImplementedException();
        }
    }

    public class InternalShippingService : IShippingService
    {
        public void ShipIt()
        {
            throw new NotImplementedException();
        }
    }

    // SAMPLE: ShippingRegistry
    public class ShippingRegistry : Registry
    {
        public ShippingRegistry()
        {
            For<IShippingService>().AddInstances(x =>
            {
                x.Type<ShippingWebService>()
                    .Ctor<string>("url").Is("a url")
                    .Named("Domestic");

                x.Type<ShippingWebService>()
                    .Ctor<string>("url").Is("a different url")
                    .Named("International");

                x.Type<InternalShippingService>().Named("Internal");
            });
        }
    }

    // ENDSAMPLE

    public class ClassThatUsesShippingService
    {
        public ClassThatUsesShippingService()
        {
            // SAMPLE: getting-ishippingservice
            var container = new Container(new ShippingRegistry());

            // Accessing the IShippingService Instance's by name
            var internationalService = container.GetInstance<IShippingService>("International");
            var domesticService = container.GetInstance<IShippingService>("Domestic");
            var internalService = container.GetInstance<IShippingService>("Internal");

            // ENDSAMPLE

            // Without generics
            var internationalService2 =
                (IShippingService) container.GetInstance(typeof (IShippingService), "International");


            internationalService.ShipIt();
            domesticService.ShipIt();
            internationalService2.ShipIt();

            var serviceName = determineShippingService();
            var service = container.GetInstance<IShippingService>(serviceName);


            service.ShipIt();
        }


        private string determineShippingService()
        {
            throw new NotImplementedException();
        }

        // With Generics
        public ValidationResult RunRulesWithGenerics(Invoice invoice)
        {
            var result = new ValidationResult();

            var container = Container.For<ShippingRegistry>();
            var validators = container.GetAllInstances<InvoiceValidator>();
            foreach (var validator in validators)
            {
                validator.Validate(invoice, result);
            }

            return result;
        }

        // Without Generics
        public ValidationResult RunRulesWithoutGenerics(Invoice invoice)
        {
            var result = new ValidationResult();

            var container = Container.For<ShippingRegistry>();
            var validators = container.GetAllInstances(typeof (InvoiceValidator));
            foreach (InvoiceValidator validator in validators)
            {
                validator.Validate(invoice, result);
            }

            return result;
        }

        #region Nested type: InvoiceValidator

        public interface InvoiceValidator
        {
            void Validate(Invoice invoice, ValidationResult result);
        }

        #endregion

        #region Nested type: ValidationResult

        public class ValidationResult
        {
        }

        #endregion
    }

    public class ScanningRegistry : Registry
    {
        public ScanningRegistry()
        {
            Scan(x =>
            {
                // Add assembly by name.
                x.Assembly("StructureMap.Testing.Widget");

                // Add an assembly directly
                x.Assembly(Assembly.GetExecutingAssembly());

                // Add the assembly that contains a certain type
                x.AssemblyContainingType<IService>();
                // or
                x.AssemblyContainingType(typeof (IService));
            });


            Scan(x =>
            {
                // I'm telling StructureMap to sweep a folder called "Extensions" directly
                // underneath the application root folder for any assemblies
                x.AssembliesFromPath("Extensions");

                // I also direct StructureMap to add any Registries that it finds in these
                // assemblies.  I'm assuming that all the StructureMap directives are
                // contained in Registry classes -- and this is the recommended approach
                x.LookForRegistries();
            });

            Scan(x =>
            {
                // This time I'm going to specify a filter on the assembly such that 
                // only assemblies that have "Extension" in their name will be scanned
                x.AssembliesFromPath("Extensions", assembly => assembly.GetName().Name.Contains("Extension"));

                x.LookForRegistries();
            });
        }
    }

    // SAMPLE: BasicScanning
    public class BasicScanning : Registry
    {
        public BasicScanning()
        {
            Scan(_ =>
            {
                // Declare which assemblies to scan
                _.Assembly("StructureMap.Testing");
                _.AssemblyContainingType<IWidget>();

                // Filter types
                _.Exclude(type => type.Name.Contains("Bad"));

                // A custom registration convention
                _.Convention<MySpecialRegistrationConvention>();

                // Built in registration conventions
                _.AddAllTypesOf<IWidget>().NameBy(x => x.Name.Replace("Widget", ""));
                _.WithDefaultConventions();
            });
        }
    }
    // ENDSAMPLE

    public class MySpecialRegistrationConvention : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            throw new NotImplementedException();
        }
    }

    public class Invoice
    {
    }

    public interface IRepository
    {
    }

    public class SimpleRepository : IRepository { }

    public interface IPresenter
    {
        void Activate();
    }

 


    public class ShippingScreenPresenter : IPresenter
    {
        private readonly IRepository _repository;
        private readonly IShippingService _service;

        // SAMPLE: ShippingScreenPresenter-with-ctor-injection
        // This is the way to write a Constructor Function with an IoC tool
        // Let the IoC container "inject" services from outside, and keep
        // ShippingScreenPresenter ignorant of the IoC infrastructure
        public ShippingScreenPresenter(IShippingService service, IRepository repository)
        {
            _service = service;
            _repository = repository;
        }
        // ENDSAMPLE

        // SAMPLE: ShippingScreenPresenter-anti-pattern
        // This is the wrong way to use an IoC container.  Do NOT invoke the container from
        // the constructor function.  This tightly couples the ShippingScreenPresenter to
        // the IoC container in a harmful way.  This class cannot be used in either
        // production or testing without a valid IoC configuration.  Plus, you're writing more
        // code
        public ShippingScreenPresenter(IContainer container)
        {
            // It's even worse if you use a static facade to retrieve
            // a service locator!
            _service = container.GetInstance<IShippingService>();
            _repository = container.GetInstance<IRepository>();
        }
        // ENDSAMPLE



        #region IPresenter Members

        public void Activate()
        {
        }

        #endregion
    }

    public class ShowBuildPlanOfShippingPresenter
    {
        // SAMPLE: ShippingScreenPresenter-build-plan
        public void ShowBuildPlan()
        {
            var container = new Container(_ =>
            {
                _.For<IShippingService>().Use<InternalShippingService>();
                _.For<IRepository>().Use<SimpleRepository>();
            });

            // Just proving that we can build ShippingScreenPresenter;)
            container.GetInstance<ShippingScreenPresenter>().ShouldNotBeNull();

            var buildPlan = container.Model.For<ShippingScreenPresenter>().Default.DescribeBuildPlan(1);

            Debug.WriteLine(buildPlan);
        }
        // ENDSAMPLE
    }

    public class ApplicationController
    {
        public void ActivateScreenFor<T>() where T : IPresenter
        {
            //IPresenter presenter = ObjectFactory.GetInstance<T>();
            //presenter.Activate();
        }

        public void ActivateScreen(IPresenter presenter)
        {
        }
    }

    public class Navigates
    {
        public Navigates()
        {
            // You most certainly do NOT just new() up an ApplicationController
            //var controller = ObjectFactory.GetInstance<ApplicationController>();
            //controller.ActivateScreenFor<ShippingScreenPresenter>();
        }
    }

    public interface IEditInvoiceView
    {
    }


    public class EditInvoicePresenter : IPresenter
    {
        private readonly Invoice _invoice;
        private readonly IRepository _repository;
        private readonly IEditInvoiceView _view;

        public EditInvoicePresenter(IRepository repository, IEditInvoiceView view, Invoice invoice)
        {
            _repository = repository;
            _view = view;
            _invoice = invoice;
        }

        #region IPresenter Members

        public void Activate()
        {
        }

        #endregion

        private void editInvoice(Invoice invoice, ApplicationController controller)
        {
            //var presenter = ObjectFactory.Container.With(invoice).GetInstance<EditInvoicePresenter>();
            //controller.ActivateScreen(presenter);
        }
    }

    public interface IApplicationShell
    {
    }

    //IQueryToolBar or IExplorerPane
    public interface IQueryToolBar
    {
    }

    public interface IExplorerPane
    {
    }

    public class ApplicationShell : Form, IApplicationShell
    {
        public IQueryToolBar QueryToolBar
        {
            get { return null; }
        }

        public IExplorerPane ExplorerPane
        {
            get { return null; }
        }
    }


    public class QueryController
    {
        private IQueryToolBar _toolBar;

        public QueryController(IQueryToolBar toolBar)
        {
            _toolBar = toolBar;
        }
    }


    public class InjectionClass
    {
        public InjectionClass()
        {
            // Familiar stuff for the average WinForms or WPF developer
            // Create the main form
            var shell = new ApplicationShell();

            // Put the main form, and some of its children into StructureMap
            // where other Controllers and Commands can get to them
            // without being coupled to the main form
            //ObjectFactory.Container.Inject<IApplicationShell>(shell);
            //ObjectFactory.Container.Inject(shell.QueryToolBar);
            //ObjectFactory.Container.Inject(shell.ExplorerPane);


            Application.Run(shell);
        }
    }

    public static class Program
    {
        [STAThread]
        public static void Main(params string[] args)
        {
        }
    }

    public class RemoteService : IService
    {
        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }

    public class InstanceExampleRegistry : Registry
    {
        public InstanceExampleRegistry()
        {
            // Shortcut for just specifying "use this type -- with auto wiring"
            For<IService>().Use<RemoteService>();

            // Set the default Instance of a PluginType
            For<IService>().Use<RemoteService>();

            // Add an additional Instance of a PluginType
            For<IService>().Use<RemoteService>();

            // Add multiple additional Instances of a PluginType
            For<IService>().AddInstances(x =>
            {
                x.ConstructedBy(() => new ColorService("Red"));

                x.Type<RemoteService>();

                x.Object(new ColorService("Red"));
            });

            // Use the InstanceExpression to define the default Instance
            // of a PluginType within a Profile
            Profile("Connected", x => { x.For<IService>().Use<RemoteService>(); });
        }
    }
}