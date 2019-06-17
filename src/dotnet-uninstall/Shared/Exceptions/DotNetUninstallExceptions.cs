using System;
using System.CommandLine;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions
{
    internal abstract class DotNetUninstallException : Exception
    {
        public DotNetUninstallException(string message) : base(message) { }
    }

    internal class LinuxNotSupportedException : DotNetUninstallException
    {
        public LinuxNotSupportedException() :
            base(Messages.LinuxNotSupportedExceptionMessage)
        { }
    }

    internal class OptionsConflictException : DotNetUninstallException
    {
        public OptionsConflictException(Option option1, Option option2) :
            base(string.Format(Messages.OptionsConflictExceptionMessageFormat, $"--{option1.Name}", $"--{option2.Name}"))
        { }
    }

    internal class CommandArgOptionConflictException : DotNetUninstallException
    {
        public CommandArgOptionConflictException(Option option) :
            base(string.Format(Messages.CommandArgOptionConflictExceptionMessageFormat, $"--{option.Name}"))
        { }
    }

    internal class InvalidInputVersionStringException : DotNetUninstallException
    {
        public InvalidInputVersionStringException(string versionString) :
            base(string.Format(Messages.InvalidInputVersionStringExceptionMessageFormat, versionString))
        { }
    }

    internal class SpecifiedVersionNotFoundException : DotNetUninstallException
    {
        public SpecifiedVersionNotFoundException(string versionString) :
            base(string.Format(Messages.SpecifiedVersionNotFoundExceptionMessageFormat, versionString))
        { }
    }

    internal class BundleTypeNotSpecifiedException : DotNetUninstallException
    {
        public BundleTypeNotSpecifiedException() :
            base(Messages.BundleTypeNotSpecifiedExceptionMessage)
        { }
    }

    internal class RequiredArgMissingForUninstallCommandException : DotNetUninstallException
    {
        public RequiredArgMissingForUninstallCommandException() :
            base(Messages.RequiredArgMissingForUninstallCommandExceptionMessage)
        { }
    }

    internal class UninstallationFailedException : DotNetUninstallException
    {
        public UninstallationFailedException(string command) :
            base(string.Format(Messages.UninstallationFailedExceptionMessageFormat, command))
        { }

        public UninstallationFailedException(string command, int exitCode) :
            base(string.Format(Messages.UninstallationFailedExceptionWithExitCodeMessageFormat, command, exitCode))
        { }
    }
}
