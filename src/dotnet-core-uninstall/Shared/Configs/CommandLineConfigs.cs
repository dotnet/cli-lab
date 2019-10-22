using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Commands;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Configs
{
    internal static class CommandLineConfigs
    {
        private static readonly string ListCommandName = "list";
        private static readonly string DryRunCommandName = "dry-run";
        private static readonly string WhatIfCommandName = "whatif";
        private static readonly string RemoveCommandName = "remove";

        public static readonly RootCommand UninstallRootCommand = new RootCommand(
            RuntimeInfo.RunningOnWindows ? LocalizableStrings.UninstallNoOptionDescriptionWindows 
            : LocalizableStrings.UninstallNoOptionDescriptionMac);

        public static readonly Command ListCommand = new Command(
            ListCommandName,
            LocalizableStrings.ListCommandDescription);

        public static readonly Command DryRunCommand = new Command(
            DryRunCommandName,
            LocalizableStrings.DryRunCommandDescription);

        public static readonly Command RemoveCommand = new Command(
           RemoveCommandName,
            LocalizableStrings.RemoveCommandDescription);


        public static readonly string SdkOptionName = "sdk";
        public static readonly string RuntimeOptionName = "runtime";
        public static readonly string AspNetRuntimeOptionName = "aspnet-runtime";
        public static readonly string HostingBundleOptionName = "hosting-bundle";
        public static readonly string X64OptionName = "x64";
        public static readonly string X86OptionName = "x86";

        public static readonly Option UninstallAllOption = new Option(
            "--all",
            LocalizableStrings.UninstallAllOptionDescription);

        public static readonly Option UninstallAllLowerPatchesOption = new Option(
            "--all-lower-patches",
            LocalizableStrings.UninstallAllLowerPatchesOptionDescription);

        public static readonly Option UninstallAllButLatestOption = new Option(
            "--all-but-latest",
            LocalizableStrings.UninstallAllButLatestOptionDescription);

        public static readonly Option UninstallAllButOption = new Option(
            "--all-but",
            LocalizableStrings.UninstallAllButOptionDescription)
        {
            Argument = new Argument<IEnumerable<string>>
            {
                Name = LocalizableStrings.UninstallAllButOptionArgumentName
            }
        };

        public static readonly Option UninstallAllBelowOption = new Option(
            "--all-below",
            LocalizableStrings.UninstallAllBelowOptionDescription)
        {
            Argument = new Argument<string>
            {
                Name = LocalizableStrings.UninstallAllBelowOptionArgumentName
            }
        };

        public static readonly Option UninstallAllPreviewsOption = new Option(
            "--all-previews",
            LocalizableStrings.UninstallAllPreviewsOptionDescription);

        public static readonly Option UninstallAllPreviewsButLatestOption = new Option(
            "--all-previews-but-latest",
            LocalizableStrings.UninstallAllPreviewsButLatestOptionDescription);

        public static readonly Option UninstallMajorMinorOption = new Option(
            "--major-minor",
            LocalizableStrings.UninstallMajorMinorOptionDescription)
        {
            Argument = new Argument<string>
            {
                Name = LocalizableStrings.UninstallMajorMinorOptionArgumentName
            }
        };

        public static readonly Option VerbosityOption = new Option(
            new[] { "--verbosity", "-v" },
            LocalizableStrings.VerbosityOptionDescription)
        {
            Argument = new Argument<string>
            {
                Name = LocalizableStrings.VerbosityOptionArgumentName
            }
        };

        public static readonly Option ListX64Option = new Option(
            $"--{X64OptionName}",
            LocalizableStrings.ListX64OptionDescription);

        public static readonly Option ListX86Option = new Option(
            $"--{X86OptionName}",
            LocalizableStrings.ListX86OptionDescription);

        public static readonly Option VersionOption = new Option("--version")
        {
            IsHidden = true
        };

        public static readonly Option YesOption = new Option(
            new[] { "--yes", "-y" },
            LocalizableStrings.YesOptionDescription);

        public static readonly Option ForceOption = new Option(
            "--force",
            RuntimeInfo.RunningOnWindows ? LocalizableStrings.ForceOptionDescriptionWindows
            : LocalizableStrings.ForceOptionDescriptionMac);

        public static readonly Option[] UninstallFilterBundlesOptions = new Option[]
        {
            UninstallAllOption,
            UninstallAllLowerPatchesOption,
            UninstallAllButLatestOption,
            UninstallAllButOption,
            UninstallAllBelowOption,
            UninstallAllPreviewsOption,
            UninstallAllPreviewsButLatestOption,
            UninstallMajorMinorOption
        };

        public static readonly Option[] ListBundleTypeOptions = new Option[]
        {
            new Option($"--{SdkOptionName}", LocalizableStrings.ListSdkOptionDescription),
            new Option($"--{RuntimeOptionName}", LocalizableStrings.ListRuntimeOptionDescription),
            new Option($"--{AspNetRuntimeOptionName}", LocalizableStrings.ListAspNetRuntimeOptionDescription),
            new Option($"--{HostingBundleOptionName}", LocalizableStrings.ListHostingBundleOptionDescription)
        };

        public static readonly Option[] UninstallBundleTypeOptions = new Option[]
        {
            new Option($"--{SdkOptionName}", LocalizableStrings.UninstallSdkOptionDescription),
            new Option($"--{RuntimeOptionName}", LocalizableStrings.UninstallRuntimeOptionDescription),
            new Option($"--{AspNetRuntimeOptionName}", LocalizableStrings.UninstallAspNetRuntimeOptionDescription),
            new Option($"--{HostingBundleOptionName}", LocalizableStrings.UninstallHostingBundleOptionDescription)
        };

        public static readonly Option[] ArchUninstallOptions = new Option[]
        {
            new Option($"--{X64OptionName}", LocalizableStrings.UninstallX64OptionDescription),
            new Option($"--{X86OptionName}", LocalizableStrings.UninstallX86OptionDescription)
        };

        public static readonly Option[] AdditionalUninstallOptions = new Option[]
        {
            VerbosityOption,
            VersionOption, 
            ForceOption
        };

        public static readonly Dictionary<string, VerbosityLevel> VerbosityLevels = new Dictionary<string, VerbosityLevel> 
        {
            { "q", VerbosityLevel.Quiet }, { "quiet", VerbosityLevel.Quiet },
            { "m", VerbosityLevel.Minimal }, { "minimal", VerbosityLevel.Minimal },
            { "n", VerbosityLevel.Normal }, { "normal", VerbosityLevel.Normal },
            { "d", VerbosityLevel.Detailed }, { "detailed", VerbosityLevel.Detailed },
            { "diag", VerbosityLevel.Diagnostic }, { "diagnostic", VerbosityLevel.Diagnostic }
        };

        public static readonly ParseResult CommandLineParseResult;
        public static readonly IEnumerable<Option> RemoveAuxOptions;
        public static readonly IEnumerable<Option> DryRunAuxOptions;
        public static readonly IEnumerable<Option> WhatIfAuxOptions;
        public static readonly IEnumerable<Option> ListAuxOptions;

        static CommandLineConfigs() 
        {
            UninstallRootCommand.AddOption(new Option("--version", LocalizableStrings.VersionOptionDescription));
            DryRunCommand.AddAlias(WhatIfCommandName);

            UninstallRootCommand.AddCommand(ListCommand);
            UninstallRootCommand.AddCommand(DryRunCommand);
            UninstallRootCommand.AddCommand(RemoveCommand);

            var supportedBundleTypeNames = SupportedBundleTypeConfigs.GetSupportedBundleTypes().Select(type => type.OptionName);

            RemoveAuxOptions = UninstallBundleTypeOptions
                .Where(option => supportedBundleTypeNames.Contains(option.Name))
                .Concat(AdditionalUninstallOptions)
                .Append(YesOption);
            if (RuntimeInfo.RunningOnWindows)
            {
                RemoveAuxOptions = RemoveAuxOptions.Concat(ArchUninstallOptions);
            }
            AssignOptionsToCommand(RemoveCommand, RemoveAuxOptions
                .Concat(UninstallFilterBundlesOptions), true);

            DryRunAuxOptions = UninstallBundleTypeOptions
                .Where(option => supportedBundleTypeNames.Contains(option.Name))
                .Concat(AdditionalUninstallOptions);
            if (RuntimeInfo.RunningOnWindows)
            {
                DryRunAuxOptions = DryRunAuxOptions.Concat(ArchUninstallOptions);
            }
            AssignOptionsToCommand(DryRunCommand, DryRunAuxOptions
                .Concat(UninstallFilterBundlesOptions), true);

            ListAuxOptions = ListBundleTypeOptions
                .Where(option => supportedBundleTypeNames.Contains(option.Name))
                .Append(VerbosityOption);
            if (RuntimeInfo.RunningOnWindows)
            {
                ListAuxOptions = ListAuxOptions
                    .Append(ListX64Option)
                    .Append(ListX86Option);
            }
            AssignOptionsToCommand(ListCommand, ListAuxOptions);

            ListCommand.Handler = CommandHandler.Create(ExceptionHandler.HandleException(() => ListCommandExec.Execute()));
            DryRunCommand.Handler = CommandHandler.Create(ExceptionHandler.HandleException(() => DryRunCommandExec.Execute()));
            RemoveCommand.Handler = CommandHandler.Create(ExceptionHandler.HandleException(() => UninstallCommandExec.Execute()));

            CommandLineParseResult = UninstallRootCommand.Parse(Environment.GetCommandLineArgs());
        }

        public static Option GetUninstallMainOption(this CommandResult commandResult)
        {
            var specified = UninstallFilterBundlesOptions
                .Where(option => commandResult.OptionResult(option.Name) != null);

            if (specified.Count() > 1)
            {
                throw new OptionsConflictException(specified);
            }

            var specifiedOption = specified.FirstOrDefault();

            if (specifiedOption != null && commandResult.Tokens.Count > 0)
            {
                var optionName = $"--{specifiedOption.Name}";

                if (specifiedOption.Name.Equals(UninstallAllButOption.Name))
                {
                    throw new VersionBeforeOptionException(optionName);
                }
                else if (specifiedOption.Name.Equals(UninstallAllBelowOption.Name) || specifiedOption.Name.Equals(UninstallMajorMinorOption.Name))
                {
                    throw new MoreThanOneVersionSpecifiedException(optionName);
                }
                else
                {
                    throw new MoreThanZeroVersionSpecifiedException(optionName);
                }
            }

            return specifiedOption;
        }

        public static BundleType GetTypeSelection(this CommandResult commandResult)
        {
            var supportedBundleTypes = SupportedBundleTypeConfigs.GetSupportedBundleTypes();

            var typeSelection = supportedBundleTypes
                .Where(type => commandResult.OptionResult(type.OptionName) != null)
                .Select(type => type.Type)
                .Aggregate((BundleType)0, (orSum, next) => orSum | next);

            return typeSelection == 0 ?
                supportedBundleTypes.Select(type => type.Type).Aggregate((BundleType)0, (orSum, next) => orSum | next) :
                typeSelection;
        }

        public static BundleArch GetArchSelection(this CommandResult commandResult)
        {
            var archSelection = new[]
            {
                (OptionName: X64OptionName, Arch: BundleArch.X64),
                (OptionName: X86OptionName, Arch: BundleArch.X86)
            }
            .Where(tuple => commandResult.OptionResult(tuple.OptionName) != null)
            .Select(tuple => tuple.Arch)
            .Aggregate((BundleArch)0, (orSum, next) => orSum | next);

            return archSelection == 0 ?
                archSelection = Enum.GetValues(typeof(BundleArch)).OfType<BundleArch>().Aggregate((BundleArch)0, (orSum, next) => orSum | next) :
                archSelection;
        }

        public static VerbosityLevel GetVerbosityLevel(this CommandResult commandResult)
        {
            var optionResult = commandResult.OptionResult(VerbosityOption.Name);

            if (optionResult == null)
            {
                return VerbosityLevel.Normal;
            }

            var levelString = optionResult.GetValueOrDefault<string>();

            if (VerbosityLevels.TryGetValue(levelString, out var level))
            {
                return level;
            }
            else
            {
                throw new VerbosityLevelInvalidException();
            }
        }

        private static void AssignOptionsToCommand(Command command, IEnumerable<Option> options, bool addVersionArgument = false)
        {
            foreach (var option in options
                .OrderBy(option => option.Name))
            {
                command.AddOption(option);
            }
            if (addVersionArgument)
            {
                command.AddArgument(new Argument<IEnumerable<string>>
                {
                    Name = LocalizableStrings.UninstallNoOptionArgumentName,
                    Description = LocalizableStrings.UninstallNoOptionArgumentDescription
                });
            }
        }
    }
}
