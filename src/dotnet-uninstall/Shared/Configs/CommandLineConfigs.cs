using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.DotNet.Tools.Uninstall.Shared.Commands;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Configs
{
    internal static class CommandLineConfigs
    {
        public static readonly Option UninstallAllOption = new Option(
            "--all",
            Messages.UninstallAllOptionDescription);

        public static readonly Option UninstallAllLowerPatchesOption = new Option(
            "--all-lower-patches",
            Messages.UninstallAllLowerPatchesOptionDescription);

        public static readonly Option UninstallAllButLatestOption = new Option(
            "--all-but-latest",
            Messages.UninstallAllButLatestOptionDescription);

        public static readonly Option UninstallAllButOption = new Option(
            "--all-but",
            Messages.UninstallAllButOptionDescription,
            new Argument<IEnumerable<string>>
            {
                Name = Messages.UninstallAllButOptionArgumentName,
                Description = Messages.UninstallAllButOptionArgumentDescription
            });

        public static readonly Option UninstallAllBelowOption = new Option(
            "--all-below",
            Messages.UninstallAllBelowOptionDescription,
            new Argument<string>
            {
                Name = Messages.UninstallAllBelowOptionArgumentName,
                Description = Messages.UninstallAllBelowOptionArgumentDescription
            });

        public static readonly Option UninstallAllPreviewsOption = new Option(
            "--all-previews",
            Messages.UninstallAllPreviewsOptionDescription);

        public static readonly Option UninstallAllPreviewsButLatestOption = new Option(
            "--all-previews-but-latest",
            Messages.UninstallAllPreviewsButLatestOptionDescription);

        public static readonly Option UninstallMajorMinorOption = new Option(
            "--major-minor",
            Messages.UninstallMajorMinorOptionDescription,
            new Argument<string>
            {
                Name = Messages.UninstallMajorMinorOptionArgumentName,
                Description = Messages.UninstallMajorMinorOptionArgumentDescription
            });

        public static readonly Option UninstallVerbosityOption = new Option(
            new[] { "--verbosity", "-v" },
            Messages.UninstallVerbosityOptionDescription,
            new Argument<string>
            {
                Name = Messages.UninstallVerbosityOptionArgumentName,
                Description = Messages.UninstallVerbosityOptionArgumentDescription
            });

        public static readonly IEnumerable<Option> Options = new Option[]
        {
            UninstallAllOption,
            UninstallAllLowerPatchesOption,
            UninstallAllButLatestOption,
            UninstallAllButOption,
            UninstallAllBelowOption,
            UninstallAllPreviewsOption,
            UninstallAllPreviewsButLatestOption,
            UninstallMajorMinorOption,
            UninstallVerbosityOption
        };

        public static readonly RootCommand UninstallRootCommand = new RootCommand(
            Messages.UninstallNoOptionDescription,
            argument: new Argument<IEnumerable<string>>
            {
                Name = Messages.UninstallNoOptionArgumentName,
                Description = Messages.UninstallNoOptionArgumentDescription
            });

        public static readonly Command ListCommand = new Command(
            "list",
            Messages.ListCommandDescription);

        static CommandLineConfigs()
        {
            UninstallRootCommand.Add(ListCommand);

            foreach (var option in Options)
            {
                UninstallRootCommand.AddOption(option);
            }

            ListCommand.Handler = CommandHandler.Create(ExceptionHandling.HandleException(() => ListCommandExec.Execute()));
            UninstallRootCommand.Handler = CommandHandler.Create(ExceptionHandling.HandleException(() => UninstallCommandExec.Execute()));
        }

        public static Option GetUniqueOption(this CommandResult commandResult)
        {
            Option specifiedOption = null;

            foreach (var option in Options)
            {
                if (commandResult.OptionResult(option.Name) != null)
                {
                    if (specifiedOption == null)
                    {
                        specifiedOption = option;
                    }
                    else
                    {
                        throw new OptionsConflictException(specifiedOption, option);
                    }
                }
            }

            if (specifiedOption != null && commandResult.Arguments.Count > 0)
            {
                throw new CommandArgOptionConflictException(specifiedOption);
            }

            return specifiedOption;
        }
    }
}
