using System;
using System.ComponentModel;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal static class ExceptionHandler
    {
        private const int NATIVE_ERROR_CODE_CANCELED = 1223;

        public static Action HandleException(Action action)
        {
            return () =>
            {
                try
                {
                    action.Invoke();
                }
                catch (DotNetUninstallException e)
                {
                    PrintExceptionMessage(e.Message);
                }
                catch (Win32Exception e)
                {
                    if (e.NativeErrorCode == NATIVE_ERROR_CODE_CANCELED)
                    {
                        PrintExceptionMessage(e.Message);
                    }
                    else
                    {
                        throw e;
                    }
                }
            };
        }

        private static void PrintExceptionMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
