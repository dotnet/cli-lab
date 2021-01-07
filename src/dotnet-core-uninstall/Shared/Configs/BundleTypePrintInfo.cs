// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        public Func<IDictionary<Bundle, string>, bool, GridView> GridViewGenerator { get; }
        public string OptionName { get; }

        protected BundleTypePrintInfo(string header, Func<IDictionary<Bundle, string>, bool, GridView> gridViewGenerator, string optionName)
        {
            Header = header ?? throw new ArgumentNullException();
            GridViewGenerator = gridViewGenerator ?? throw new ArgumentNullException();
            OptionName = optionName ?? throw new ArgumentNullException();
        }

        public abstract IEnumerable<Bundle> Filter(IEnumerable<Bundle> bundles);
    }

    internal class BundleTypePrintInfo<TBundleVersion> : BundleTypePrintInfo
        where TBundleVersion : BundleVersion, IComparable<TBundleVersion>, new()
    {
        public override BundleType Type => new TBundleVersion().Type;

        public BundleTypePrintInfo(string header, Func<IDictionary<Bundle, string>, bool, GridView> gridViewGenerator, string optionName) :
            base(header, gridViewGenerator, optionName)
        { }

        public override IEnumerable<Bundle> Filter(IEnumerable<Bundle> bundles)
        {
            return Bundle<TBundleVersion>.FilterWithSameBundleType(bundles);
        }
    }
}
