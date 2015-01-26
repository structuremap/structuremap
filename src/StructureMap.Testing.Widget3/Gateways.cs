using System;
using StructureMap.LegacyAttributeSupport;

namespace StructureMap.Testing.Widget3
{
    public interface IGateway
    {
        string WhoAmI { get; }
        void DoSomething();
    }

    [Pluggable("Default")]
    public class DefaultGateway : IGateway
    {
        public string Name { get; set; }

        public string Color { get; set; }

        #region IGateway Members

        public void DoSomething()
        {
        }

        public string WhoAmI { get { return "Default"; } }

        #endregion
    }

    public class DecoratedGateway : IGateway
    {
        private IGateway _innerGateway;

        public DecoratedGateway(IGateway innerGateway)
        {
            _innerGateway = innerGateway;
        }

        public IGateway InnerGateway
        {
            get { return _innerGateway; }
        }

        #region IGateway Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        public string WhoAmI { get { throw new NotImplementedException(); } }

        #endregion
    }


    [Pluggable("Stubbed")]
    public class StubbedGateway : IGateway
    {
        #region IGateway Members

        public void DoSomething()
        {
        }

        public string WhoAmI { get { return "Stubbed"; } }

        #endregion
    }
}