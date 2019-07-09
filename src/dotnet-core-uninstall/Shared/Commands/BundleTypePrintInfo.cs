using System;
using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Commands
{
    internal abstract class BundleTypePrintInfo
    {
        public abstract BundleType Type { get; }

        public string Header { get; }

        protected BundleTypePrintInfo(string header)
        {
            Header = header ?? throw new ArgumentNullException();
        }

        public abstract IEnumerable<Bundle> Filter(IEnumerable<Bundle> bundles);
    }

    internal class BundleTypePrintInfo<TBundleVersion> : BundleTypePrintInfo
        where TBundleVersion : BundleVersion, IComparable<TBundleVersion>, new()
    {
        public override BundleType Type => new TBundleVersion().Type;

        public BundleTypePrintInfo(string header) : base(header) { }

        public override IEnumerable<Bundle> Filter(IEnumerable<Bundle> bundles)
        {
            return Bundle<TBundleVersion>.FilterWithSameBundleType(bundles);
        }
    }
}
