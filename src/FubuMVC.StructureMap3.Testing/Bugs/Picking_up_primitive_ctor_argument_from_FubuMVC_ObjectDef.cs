using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using NUnit.Framework;

namespace FubuMVC.StructureMap3.Testing.Bugs
{
    [TestFixture]
    public class Picking_up_primitive_ctor_argument_from_FubuMVC_ObjectDef
    {

    }

    public interface IAssetRequirements
    {
        void UseAssetIfExists(string[] assets);
    }

    public class AssetRequirements : IAssetRequirements
    {
        public void UseAssetIfExists(string[] assets)
        {
            
        }
    }

    public class OptionalAssetsNode : BehaviorNode
    {
        readonly string _assetName;

        public OptionalAssetsNode(string assetName)
        {
            _assetName = assetName;
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Wrapper; }
        }

        protected override ObjectDef buildObjectDef()
        {
            var def = new ObjectDef(typeof(OptionalAssetsBehavior));
            def.DependencyByValue(_assetName);
            return def;
        }
    }

    public class OptionalAssetsBehavior : BasicBehavior
    {
        readonly MimeType[] _mimeTypes = new[]
        {
            MimeType.Javascript,
            MimeType.Css
        };

        readonly string _assetName;
        readonly IAssetRequirements _assetRequirements;

        public OptionalAssetsBehavior(string assetName, IAssetRequirements assetRequirements)
            : base(PartialBehavior.Ignored)
        {
            _assetName = assetName;
            _assetRequirements = assetRequirements;
        }

        protected override DoNext performInvoke()
        {
            var assets = _mimeTypes.SelectMany(x => x.Extensions).Select(x => _assetName + x).ToArray();
            IncludeAssets(_assetRequirements, assets);
            return DoNext.Continue;
        }

        protected virtual void IncludeAssets(IAssetRequirements assetRequirements, string[] assets)
        {
            _assetRequirements.UseAssetIfExists(assets);
        }
    }
}