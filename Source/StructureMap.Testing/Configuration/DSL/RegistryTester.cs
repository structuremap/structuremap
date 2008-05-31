using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class RegistryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion
    }

    public class TestRegistry : Registry
    {
    }

    public class FakeGateway : IGateway
    {
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

    public class Fake2Gateway : IGateway
    {
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

    public class Fake3Gateway : IGateway
    {
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
}