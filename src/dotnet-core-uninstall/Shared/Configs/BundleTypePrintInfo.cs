using System;
using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Configs
{
    internal abstract class BundleTypePrintInfo
    {
        public abstract BundleType Type { get; }

        public string Header { get; }
        public Func<IList<Bundle>, GridView> GridViewGenerator { get; }

        protected BundleTypePrintInfo(string header, Func<IList<Bundle>, GridView> gridViewGenerator)
        {
            Header = header ?? throw new ArgumentNullException();
            GridViewGenerator = gridViewGenerator ?? throw new ArgumentNullException();
        }

        public abstract IEnumerable<Bundle> Filter(IEnumerable<Bundle> bundles);
    }

    internal class BundleTypePrintInfo<TBundleVersion> : BundleTypePrintInfo
        where TBundleVersion : BundleVersion, IComparable<TBundleVersion>, new()
    {
        public override BundleType Type => new TBundleVersion().Type;

        public BundleTypePrintInfo(string header, Func<IList<Bundle>, GridView> gridViewGenerator) : base(header, gridViewGenerator) { }

        public override IEnumerable<Bundle> Filter(IEnumerable<Bundle> bundles)
        {
            return Bundle<TBundleVersion>.FilterWithSameBundleType(bundles);
        }
    }
}
