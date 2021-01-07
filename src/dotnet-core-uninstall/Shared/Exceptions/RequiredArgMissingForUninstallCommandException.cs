// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class RequiredArgMissingForUninstallCommandException : DotNetUninstallException
    {
        public RequiredArgMissingForUninstallCommandException() :
            base(LocalizableStrings.RequiredArgMissingForUninstallCommandExceptionMessage)
        { }
    }
}
