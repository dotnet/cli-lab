// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class BundleTypeMissingException : DotNetUninstallException
    {
        public BundleTypeMissingException(IEnumerable<string> options) :
            base(string.Format(
                LocalizableStrings.BundleTypeMissingExceptionMessage,
                string.Join(", ", options)))
        { }
    }
}
