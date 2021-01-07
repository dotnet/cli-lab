// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Filterers
{
    internal class NoOptionFilterer : ArgFilterer<IEnumerable<string>>
    {
        public override IEnumerable<Bundle<TBundleVersion>> Filter<TBundleVersion>(IEnumerable<string> argValue, IEnumerable<Bundle<TBundleVersion>> bundles)
        {
            var versions = argValue
                .Select(value => BundleVersion.FromInput<TBundleVersion>(value))
                .OrderBy(version => version);

            if (versions.Any(version => !bundles.Select(bundle => bundle.Version).ToList().Contains(version)))
            {
                throw new SpecifiedVersionNotFoundException(versions
                    .Where(version => !bundles.Select(bundle => bundle.Version).ToList().Contains(version)));
            }

            return bundles
                .Where(bundle => versions.Contains(bundle.Version));
        }
    }
}
