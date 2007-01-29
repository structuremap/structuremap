using System;

namespace StructureMap.Testing.Widget3
{
    [PluginFamily("Default")]
    public interface IGateway
    {
        void DoSomething();
        string WhoAmI { get; }
    }

    [Pluggable("Default", "")]
    public class DefaultGateway : IGateway
    {
        private string _name;
        private string _color;

        #region IGateway Members

        public void DoSomething()
        {
            // TODO:  Add DefaultGateway.DoSomething implementation
        }

        public string WhoAmI
        {
            get { return "Default"; }
        }

        #endregion

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Color
        {
            get { return _color; }
            set { _color = value; }
        }
    }

    public class DecoratedGateway : IGateway
    {
        private IGateway _innerGateway;

        public DecoratedGateway(IGateway innerGateway)
        {
            _innerGateway = innerGateway;
        }

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        public string WhoAmI
        {
            get { throw new NotImplementedException(); }
        }
    }


    [Pluggable("Stubbed", "")]
    public class StubbedGateway : IGateway
    {
        #region IGateway Members

        public void DoSomething()
        {
            // TODO:  Add StubbedGateway.DoSomething implementation
        }

        public string WhoAmI
        {
            get { return "Stubbed"; }
        }

        #endregion
    }
}