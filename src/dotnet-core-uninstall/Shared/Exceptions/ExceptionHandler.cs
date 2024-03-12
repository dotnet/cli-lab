﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal static class ExceptionHandler
    {
        public static Action<T> HandleException<T>(Action<T> action)
        {
            return (x) =>
            {
                try
                {
                    action.Invoke(x);
                }
                catch (DotNetUninstallException e)
                {
                    PrintExceptionMessage(e.Message);
                    Environment.Exit(1);
                }
            };
        }

        public static void PrintExceptionMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
