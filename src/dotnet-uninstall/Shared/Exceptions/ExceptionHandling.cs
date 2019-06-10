using System;
using static Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal static class ExceptionHandling
    {
        public static Action HandleException(Action action)
        {
            return () =>
            {
                try
                {
                    action.Invoke();
                }
                catch (LinuxNotSupportedException)
                {
                    PrintExceptionMessage(Messages.LinuxNotSupportedExceptionMessage);
                }
                catch (OptionsConflictException)
                {
                    PrintExceptionMessage(Messages.OptionsConflictExceptionMessage);
                }
                catch (InvalidVersionStringException e)
                {
                    PrintExceptionMessage(string.Format(Messages.InvalidVersionStringExceptionMessage, e.Message));
                }
                catch (SpecifiedVersionNotFoundException e)
                {
                    PrintExceptionMessage(string.Format(Messages.SpecifiedVersionNotFoundExceptionMessage, e.Message));
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
