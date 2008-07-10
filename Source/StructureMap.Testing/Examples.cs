using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StructureMap.Attributes;

namespace StructureMap.Testing
{
    
    public interface IDataProvider{}

    public class Repository
    {
        private IDataProvider _provider;

        // Adding the SetterProperty to a setter directs
        // StructureMap to use this property when
        // constructing a Repository instance
        [SetterProperty]
        public IDataProvider Provider
        {
            set
            {
                _provider = value;
            }
        }

        [SetterProperty]
        public bool ShouldCache { get; set; }
    }

    public interface IShippingService
    {
        void Go();
    }

    public class ClassThatUsesShippingService
    {
        public ClassThatUsesShippingService()
        {
            // With generics
            IShippingService internationalService = ObjectFactory.GetNamedInstance<IShippingService>("International");
            IShippingService domesticService = ObjectFactory.GetNamedInstance<IShippingService>("Domestic");

            // Without generics
            IShippingService internationalService2 = 
                (IShippingService) ObjectFactory.GetNamedInstance(typeof(IShippingService), "International");
        
        
            internationalService.Go();
            domesticService.Go();
            internationalService2.Go();

            string serviceName = determineShippingService();
            IShippingService service = ObjectFactory.GetNamedInstance<IShippingService>(serviceName);


            service.Go();

        }



        private string determineShippingService()
        {
            throw new NotImplementedException();
        }

        public class ValidationResult{}

        public interface InvoiceValidator
        {
            void Validate(Invoice invoice, ValidationResult result);
        }

        // With Generics
        public ValidationResult RunRulesWithGenerics(Invoice invoice)
        {
            ValidationResult result = new ValidationResult();

            IList<InvoiceValidator> validators = ObjectFactory.GetAllInstances<InvoiceValidator>();
            foreach (var validator in validators)
            {
                validator.Validate(invoice, result);
            }

            return result;
        }

        // Without Generics
        public ValidationResult RunRulesWithoutGenerics(Invoice invoice)
        {
            ValidationResult result = new ValidationResult();

            IList validators = ObjectFactory.GetAllInstances(typeof(InvoiceValidator));
            foreach (InvoiceValidator validator in validators)
            {
                validator.Validate(invoice, result);
            }

            return result;
        }
    }

    public class Invoice{}

    public interface IRepository{}

    public interface IPresenter
    {
        void Activate();
    }

    public class ShippingScreenPresenter : IPresenter
    {
        private readonly IShippingService _service;
        private readonly IRepository _repository;

        public ShippingScreenPresenter(IShippingService service, IRepository repository)
        {
            _service = service;
            _repository = repository;
        }

        public void Activate()
        {
        }
    }

    public class ApplicationController
    {
        public void ActivateScreenFor<T>() where T : IPresenter
        {
            IPresenter presenter = ObjectFactory.FillDependencies<T>();
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
            ApplicationController controller = ObjectFactory.GetInstance<ApplicationController>();
            controller.ActivateScreenFor<ShippingScreenPresenter>();
        }
    }

    public interface IEditInvoiceView{}


    public class EditInvoicePresenter : IPresenter
    {
        private readonly IRepository _repository;
        private readonly IEditInvoiceView _view;
        private readonly Invoice _invoice;

        public EditInvoicePresenter(IRepository repository, IEditInvoiceView view, Invoice invoice)
        {
            _repository = repository;
            _view = view;
            _invoice = invoice;
        }


        public void Activate()
        {
            
        }

        private void editInvoice(Invoice invoice, ApplicationController controller)
        {
            EditInvoicePresenter presenter = ObjectFactory.With<Invoice>(invoice).GetInstance<EditInvoicePresenter>();
            controller.ActivateScreen(presenter);
        }
    }
    
    public interface IApplicationShell
    {
        
    }
    //IQueryToolBar or IExplorerPane
    public interface IQueryToolBar{}
    public interface IExplorerPane{}

    public class ApplicationShell : Form, IApplicationShell
    {
        public IQueryToolBar QueryToolBar
        {
            get
            {
                return null;
            }
        }

        public IExplorerPane ExplorerPane
        {
            get
            {
                return null;
            }
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
            ApplicationShell shell = new ApplicationShell();

            // Put the main form, and some of its children into StructureMap
            // where other Controllers and Commands can get to them
            // without being coupled to the main form
            ObjectFactory.Inject<IApplicationShell>(shell);
            ObjectFactory.Inject<IQueryToolBar>(shell.QueryToolBar);
            ObjectFactory.Inject<IExplorerPane>(shell.ExplorerPane);


            Application.Run(shell);
        }
    }
}
