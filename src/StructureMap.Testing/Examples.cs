using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.DocumentationExamples
{
    public interface IDataProvider
    {
    }

    public class Repository
    {
        private IDataProvider _provider;

        // Adding the SetterProperty to a setter directs
        // StructureMap to use this property when
        // constructing a Repository instance
        [SetterProperty]
        public IDataProvider Provider { set { _provider = value; } }

        [SetterProperty]
        public bool ShouldCache { get; set; }
    }

    public interface IShippingService
    {
        void ShipIt();
    }

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

    public class ClassThatUsesShippingService
    {
        public ClassThatUsesShippingService()
        {
            // Accessing the IShippingService Instance's by name
            var internationalService = ObjectFactory.GetNamedInstance<IShippingService>("International");
            var domesticService = ObjectFactory.GetNamedInstance<IShippingService>("Domestic");
            var internalService = ObjectFactory.GetNamedInstance<IShippingService>("Internal");

            // Without generics
            var internationalService2 =
                (IShippingService) ObjectFactory.GetNamedInstance(typeof (IShippingService), "International");


            internationalService.ShipIt();
            domesticService.ShipIt();
            internationalService2.ShipIt();

            string serviceName = determineShippingService();
            var service = ObjectFactory.GetNamedInstance<IShippingService>(serviceName);


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

            IList<InvoiceValidator> validators = ObjectFactory.GetAllInstances<InvoiceValidator>();
            foreach (InvoiceValidator validator in validators)
            {
                validator.Validate(invoice, result);
            }

            return result;
        }

        // Without Generics
        public ValidationResult RunRulesWithoutGenerics(Invoice invoice)
        {
            var result = new ValidationResult();

            IList validators = ObjectFactory.GetAllInstances(typeof (InvoiceValidator));
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


            // Adding configuration from an extension Assembly
            // after ObjectFactory is already configured
            ObjectFactory.Configure(x =>
            {
                x.Scan(scan =>
                {
                    scan.Assembly("MyCompany.MyApp.ExtensionAssembly");
                    scan.LookForRegistries();
                });
            });
        }
    }

    public class Invoice
    {
    }

    public interface IRepository
    {
    }

    public interface IPresenter
    {
        void Activate();
    }

    public class ShippingScreenPresenter : IPresenter
    {
        private readonly IRepository _repository;
        private readonly IShippingService _service;


        // This is the way to write a Constructor Function with an IoC tool
        // Let the IoC container "inject" services from outside, and keep
        // ShippingScreenPresenter ignorant of the IoC infrastructure
        public ShippingScreenPresenter(IShippingService service, IRepository repository)
        {
            _service = service;
            _repository = repository;
        }

        // FAIL!
        // This is the wrong way to use an IoC container.  Do NOT invoke the container from
        // the constructor function.  This tightly couples the ShippingScreenPresenter to
        // the IoC container in a harmful way.  This class cannot be used in either
        // production or testing without a valid IoC configuration.  Plus, you're writing more
        // code
        public ShippingScreenPresenter()
        {
            _service = ObjectFactory.GetInstance<IShippingService>();
            _repository = ObjectFactory.GetInstance<IRepository>();
        }

        #region IPresenter Members

        public void Activate()
        {
        }

        #endregion
    }

    public class ApplicationController
    {
        public void ActivateScreenFor<T>() where T : IPresenter
        {
            IPresenter presenter = ObjectFactory.GetInstance<T>();
            presenter.Activate();
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
            var controller = ObjectFactory.GetInstance<ApplicationController>();
            controller.ActivateScreenFor<ShippingScreenPresenter>();
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
            var presenter = ObjectFactory.With(invoice).GetInstance<EditInvoicePresenter>();
            controller.ActivateScreen(presenter);
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
        public IQueryToolBar QueryToolBar { get { return null; } }

        public IExplorerPane ExplorerPane { get { return null; } }
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
            ObjectFactory.Inject<IApplicationShell>(shell);
            ObjectFactory.Inject(shell.QueryToolBar);
            ObjectFactory.Inject(shell.ExplorerPane);


            Application.Run(shell);
        }
    }

    public class Bootstrapper : IBootstrapper
    {
        private static bool _hasStarted;

        public void BootstrapStructureMap()
        {
            ObjectFactory.Initialize(x =>
            {
                // initialization
            });
        }

        public static void Restart()
        {
            if (_hasStarted)
            {
                ObjectFactory.ResetDefaults();
            }
            else
            {
                Bootstrap();
                _hasStarted = true;
            }
        }

        public static void Bootstrap()
        {
            new Bootstrapper().BootstrapStructureMap();
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