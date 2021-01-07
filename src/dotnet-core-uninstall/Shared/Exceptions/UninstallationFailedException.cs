// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class UninstallationFailedException : DotNetUninstallException
    {
        public UninstallationFailedException(string command) :
            base(string.Format(LocalizableStrings.UninstallationFailedExceptionMessageFormat, command))
        { }

        public UninstallationFailedException(string command, int exitCode) :
            base(string.Format(LocalizableStrings.UninstallationFailedExceptionWithExitCodeMessageFormat, command, exitCode))
        { }
    }
}
