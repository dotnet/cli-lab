// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class UninstallationNotAllowedException : DotNetUninstallException
    {
        public UninstallationNotAllowedException() :
            base(string.Format(LocalizableStrings.UninstallNotAllowedExceptionFormat, VisualStudioSafeVersionsExtractor.UpperLimit.ToNormalizedString()))
        { }
    }
}
