using System;

namespace StructureMap.Testing.Widget3
{
    public interface IGateway
    {
        string WhoAmI { get; }
        void DoSomething();
    }

    public class DefaultGateway : IGateway
    {
        public string Name { get; set; }

        public string Color { get; set; }

        #region IGateway Members

        public void DoSomething()
        {
        }

        public string WhoAmI
        {
            get { return "Default"; }
        }

        #endregion
    }

    public class DecoratedGateway : IGateway
    {
        public DecoratedGateway(IGateway innerGateway)
        {
            InnerGateway = innerGateway;
        }

        public IGateway InnerGateway { get; }

        #region IGateway Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        public string WhoAmI
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }


    public class StubbedGateway : IGateway
    {
        #region IGateway Members

        public void DoSomething()
        {
        }

        public string WhoAmI
        {
            get { return "Stubbed"; }
        }

        #endregion
    }
}