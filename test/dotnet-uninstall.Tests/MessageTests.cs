// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Xunit;

namespace Microsoft.DotNet.Tools.Uninstall
{
    public class MessageTests 
    {
        [Fact]
        public void It_should_have_correct_message()
        {
            Messages.message.Should().Be("Hello World!");
        }
    }
}
