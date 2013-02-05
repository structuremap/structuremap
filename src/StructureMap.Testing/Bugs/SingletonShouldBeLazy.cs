using System;
using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Bugs
{
    public class SingletonShouldBeLazy : IGateway
    {
        public SingletonShouldBeLazy()
        {
            Assert.Fail("I should not be called");
        }

        #region IGateway Members

        public string WhoAmI { get { throw new NotImplementedException(); } }

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    public class CrmService
    {
        private readonly ICrm _crm;

        public CrmService()
            : this(ObjectFactory.GetInstance<ICrm>())
        {
        }

        public CrmService(ICrm crm)
        {
            _crm = crm;
        }

        public static void Main(string[] args)
        {
            Initialise();

            var svc = new CrmService();
            svc.Run(args);
        }

        public void Run(string[] args)
        {
            Trace.WriteLine("Starting service");
            _crm.Message("Running CRM");
        }

        private static void Initialise()
        {
            ObjectFactory.Initialize(registry => { });
            ObjectFactory.Configure(registry =>
                                    registry.ForSingletonOf<ICrm>().Use<Crm>());
        }
    }


    public interface ICrm
    {
        void Message(string arg);
    }


    public class Crm : ICrm
    {
        public Crm()
        {
            Trace.WriteLine("Initialising CRM");
        }

        public void Message(string message)
        {
            Trace.WriteLine(message);
        }
    }
}