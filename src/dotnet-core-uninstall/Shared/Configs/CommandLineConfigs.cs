using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Commands;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Configs
{
    internal static class CommandLineConfigs
    {
        public static readonly string SdkOptionName = "sdk";
        public static readonly string RuntimeOptionName = "runtime";
        public static readonly string AspNetRuntimeOptionName = "aspnet-runtime";
        public static readonly string HostingBundleOptionName = "hosting-bundle";
        public static readonly string X64OptionName = "x64";
        public static readonly string X86OptionName = "x86";
        public static readonly string ListCommandName = "list";

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

        public static readonly Option UninstallSdkOption = new Option(
            $"--{SdkOptionName}",
            LocalizableStrings.UninstallSdkOptionDescription);

        public static readonly Option UninstallRuntimeOption = new Option(
            $"--{RuntimeOptionName}",
            LocalizableStrings.UninstallRuntimeOptionDescription);

        public static readonly Option UninstallAspNetRuntimeOption = new Option(
            $"--{AspNetRuntimeOptionName}",
            LocalizableStrings.UninstallAspNetRuntimeOptionDescription);

        public static readonly Option UninstallHostingBundleOption = new Option(
            $"--{HostingBundleOptionName}",
            LocalizableStrings.UninstallHostingBundleOptionDescription);

        public static readonly Option ListSdkOption = new Option(
            $"--{SdkOptionName}",
            LocalizableStrings.ListSdkOptionDescription);

        public static readonly Option ListRuntimeOption = new Option(
            $"--{RuntimeOptionName}",
            LocalizableStrings.ListRuntimeOptionDescription);

        public static readonly Option ListAspNetRuntimeOption = new Option(
            $"--{AspNetRuntimeOptionName}",
            LocalizableStrings.ListAspNetRuntimeOptionDescription);

        public static readonly Option ListHostingBundleOption = new Option(
            $"--{HostingBundleOptionName}",
            LocalizableStrings.ListHostingBundleOptionDescription);

        public static readonly Option UninstallX64Option = new Option(
            $"--{X64OptionName}",
            LocalizableStrings.UninstallX64OptionDescription);

        public static readonly Option UninstallX86Option = new Option(
            $"--{X86OptionName}",
            LocalizableStrings.UninstallX86OptionDescription);

        public static readonly Option ListX64Option = new Option(
            $"--{X64OptionName}",
            LocalizableStrings.ListX64OptionDescription);

        public static readonly Option ListX86Option = new Option(
            $"--{X86OptionName}",
            LocalizableStrings.ListX86OptionDescription);

        public static readonly Option VersionOption = new Option(
            "--version")
        {
            IsHidden = true
        };

        public static readonly Option DryRunOption = new Option(
            "--dry-run",
            LocalizableStrings.DryRunOptionDescription);

        public static readonly Option YesOption = new Option(
            new[] { "--yes", "-y" },
            LocalizableStrings.YesOptionDescription);

        public static readonly IEnumerable<Option> UninstallMainOptions = new Option[]
        {
            UninstallAllOption,
            UninstallAllLowerPatchesOption,
            UninstallAllButLatestOption,
            UninstallAllButOption,
            UninstallAllBelowOption,
            UninstallAllPreviewsOption,
            UninstallAllPreviewsButLatestOption,
            UninstallMajorMinorOption,
        };

        public static readonly RootCommand UninstallRootCommand = new RootCommand(
            LocalizableStrings.UninstallNoOptionDescription);

        public static readonly Command ListCommand = new Command(
            ListCommandName,
            LocalizableStrings.ListCommandDescription);

        public static readonly Dictionary<string, VerbosityLevel> VerbosityLevels = new Dictionary<string, VerbosityLevel>
        {
            { "q", VerbosityLevel.Quiet }, { "quiet", VerbosityLevel.Quiet },
            { "m", VerbosityLevel.Minimal }, { "minimal", VerbosityLevel.Minimal },
            { "n", VerbosityLevel.Normal }, { "normal", VerbosityLevel.Normal },
            { "d", VerbosityLevel.Detailed }, { "detailed", VerbosityLevel.Detailed },
            { "diag", VerbosityLevel.Diagnostic }, { "diagnostic", VerbosityLevel.Diagnostic }
        };

        public static readonly ParseResult CommandLineParseResult;
        public static readonly IEnumerable<Option> UninstallAuxOptions;
        public static readonly IEnumerable<Option> ListAuxOptions;

        static CommandLineConfigs()
        {
            UninstallRootCommand.AddArgument(new Argument<IEnumerable<string>>
            {
                Name = LocalizableStrings.UninstallNoOptionArgumentName,
                Description = LocalizableStrings.UninstallNoOptionArgumentDescription
            });

            UninstallRootCommand.AddCommand(ListCommand);

            var supportedBundleTypeNames = SupportedBundleTypeConfigs.GetSupportedBundleTypes().Select(type => type.OptionName);

            var supportedUninstallBundleTypeOptions = new Option[]
            {
                UninstallSdkOption,
                UninstallRuntimeOption,
                UninstallAspNetRuntimeOption,
                UninstallHostingBundleOption
            }
            .Where(option => supportedBundleTypeNames.Contains(option.Name));

            var supportedListBundleTypeOptions = new Option[]
            {
                ListSdkOption,
                ListRuntimeOption,
                ListAspNetRuntimeOption,
                ListHostingBundleOption
            }
            .Where(option => supportedBundleTypeNames.Contains(option.Name));

            UninstallAuxOptions = supportedUninstallBundleTypeOptions
                .Append(VerbosityOption)
                .Append(UninstallX64Option)
                .Append(UninstallX86Option)
                .Append(VersionOption)
                .Append(DryRunOption)
                .Append(YesOption);

            ListAuxOptions = supportedListBundleTypeOptions
                .Append(VerbosityOption)
                .Append(ListX64Option)
                .Append(ListX86Option);

            foreach (var option in UninstallMainOptions
                .Concat(UninstallAuxOptions)
                .OrderBy(option => option.Name))
            {
                UninstallRootCommand.AddOption(option);
            }

            foreach (var option in ListAuxOptions
                .OrderBy(option => option.Name))
            {
                ListCommand.AddOption(option);
            }

            ListCommand.Handler = CommandHandler.Create(ExceptionHandler.HandleException(() => ListCommandExec.Execute()));
            UninstallRootCommand.Handler = CommandHandler.Create(ExceptionHandler.HandleException(() => UninstallCommandExec.Execute()));

            CommandLineParseResult = UninstallRootCommand.Parse(Environment.GetCommandLineArgs());
        }

        public static Option GetUninstallMainOption(this CommandResult commandResult)
        {
            var specified = UninstallMainOptions
                .Where(option => commandResult.OptionResult(option.Name) != null);

            if (specified.Count() > 1)
            {
                throw new OptionsConflictException(specified);
            }

            var specifiedOption = specified.FirstOrDefault();

            if (specifiedOption != null && commandResult.Tokens.Count > 0)
            {
                var optionName = $"--{specifiedOption.Name}";

                if (specifiedOption.Equals(UninstallAllButOption))
                {
                    throw new VersionBeforeOptionException(optionName);
                }
                else if (specifiedOption.Equals(UninstallAllBelowOption) || specifiedOption.Equals(UninstallMajorMinorOption))
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
    }
}
