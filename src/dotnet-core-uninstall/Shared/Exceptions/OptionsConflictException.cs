// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.CommandLine;
using System.Linq;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal class OptionsConflictException : DotNetUninstallException
    {
        public OptionsConflictException(IEnumerable<Option> options) :
            base(string.Format(LocalizableStrings.OptionsConflictExceptionMessageFormat, string.Join(", ", options.Select(option => $"--{option.Name}"))))
        { }
    }
}
