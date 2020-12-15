// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class VerbosityLevelInvalidException : DotNetUninstallException
    {
        public VerbosityLevelInvalidException() :
            base(LocalizableStrings.VerbosityLevelInvalidExceptionMessage)
        { }
    }
}
