using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.StructureMap3.Testing.Internals
{
    public class StubActivator : IActivator
    {
        private IEnumerable<IPackageInfo> _packages;
        private IPackageLog _log;

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _packages = packages;
            _log = log;


        }

        public IEnumerable<IPackageInfo> Packages
        {
            get { return _packages; }
        }

        public IPackageLog Log
        {
            get { return _log; }
        }
    }
}