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
            LocalizableStrings.UninstallAllButOptionDescription,
            new Argument<IEnumerable<string>>
            {
                Name = LocalizableStrings.UninstallAllButOptionArgumentName
            });

        public static readonly Option UninstallAllBelowOption = new Option(
            "--all-below",
            LocalizableStrings.UninstallAllBelowOptionDescription,
            new Argument<string>
            {
                Name = LocalizableStrings.UninstallAllBelowOptionArgumentName
            });

        public static readonly Option UninstallAllPreviewsOption = new Option(
            "--all-previews",
            LocalizableStrings.UninstallAllPreviewsOptionDescription);

        public static readonly Option UninstallAllPreviewsButLatestOption = new Option(
            "--all-previews-but-latest",
            LocalizableStrings.UninstallAllPreviewsButLatestOptionDescription);

        public static readonly Option UninstallMajorMinorOption = new Option(
            "--major-minor",
            LocalizableStrings.UninstallMajorMinorOptionDescription,
            new Argument<string>
            {
                Name = LocalizableStrings.UninstallMajorMinorOptionArgumentName
            });

        public static readonly Option UninstallVerbosityOption = new Option(
            new[] { "--verbosity", "-v" },
            LocalizableStrings.UninstallVerbosityOptionDescription,
            new Argument<string>
            {
                Name = LocalizableStrings.UninstallVerbosityOptionArgumentName
            });

        public static readonly Option SdkOption = new Option(
            "--sdk",
            LocalizableStrings.SdkOptionDescription);

        public static readonly Option RuntimeOption = new Option(
            "--runtime",
            LocalizableStrings.RuntimeOptionDescription);

        public static readonly Option AspNetRuntimeOption = new Option(
            "--aspnet-runtime",
            LocalizableStrings.AspNetRuntimeOptionDescription);

        public static readonly Option HostingBundleOption = new Option(
            "--hosting-bundle",
            LocalizableStrings.HostingBundleOptionDescription);

        public static readonly Option X86Option = new Option(
            "--x86",
            LocalizableStrings.X86OptionDescription);

        public static readonly Option X64Option = new Option(
            "--x64",
            LocalizableStrings.X64OptionDescription);

        public static readonly Option VersionOption = new Option(
            "--version",
            isHidden: true);

        public static readonly Option DoItOption = new Option(
            "--do-it",
            LocalizableStrings.DoItOptionDescription);

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

        public static readonly IEnumerable<Option> AuxOptions = new Option[]
        {
            UninstallVerbosityOption,
            SdkOption,
            RuntimeOption,
            AspNetRuntimeOption,
            HostingBundleOption,
            X86Option,
            X64Option
        };

        public static readonly RootCommand UninstallRootCommand = new RootCommand(
            LocalizableStrings.UninstallNoOptionDescription,
            argument: new Argument<IEnumerable<string>>
            {
                Name = LocalizableStrings.UninstallNoOptionArgumentName,
                Description = LocalizableStrings.UninstallNoOptionArgumentDescription
            });

        public static readonly Command ListCommand = new Command(
            "list",
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

        static CommandLineConfigs()
        {
            UninstallRootCommand.Add(ListCommand);

            foreach (var option in UninstallMainOptions
                .Concat(AuxOptions)
                .Append(VersionOption)
                .Append(DoItOption)
                .OrderBy(option => option.Name))
            {
                UninstallRootCommand.AddOption(option);
            }

            foreach (var option in AuxOptions
                .OrderBy(option => option.Name))
            {
                ListCommand.Add(option);
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

            if (specifiedOption != null && commandResult.Arguments.Count > 0)
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
            var typeSelection = (BundleType)0;

            if (commandResult.OptionResult(SdkOption.Name) != null)
            {
                typeSelection |= BundleType.Sdk;
            }

            if (commandResult.OptionResult(RuntimeOption.Name) != null)
            {
                typeSelection |= BundleType.Runtime;
            }

            if (commandResult.OptionResult(AspNetRuntimeOption.Name) != null)
            {
                typeSelection |= BundleType.AspNetRuntime;
            }

            if (commandResult.OptionResult(HostingBundleOption.Name) != null)
            {
                typeSelection |= BundleType.HostingBundle;
            }

            if (typeSelection == 0)
            {
                typeSelection = Enum.GetValues(typeof(BundleType)).OfType<BundleType>().Aggregate((BundleType)0, (orSum, next) => orSum | next);
            }

            return typeSelection;
        }

        public static BundleArch GetArchSelection(this CommandResult commandResult)
        {
            var archSelection = (BundleArch)0;

            if (commandResult.OptionResult(X86Option.Name) != null)
            {
                archSelection |= BundleArch.X86;
            }

            if (commandResult.OptionResult(X64Option.Name) != null)
            {
                archSelection |= BundleArch.X64;
            }

            if (archSelection == 0)
            {
                archSelection = Enum.GetValues(typeof(BundleArch)).OfType<BundleArch>().Aggregate((BundleArch)0, (orSum, next) => orSum | next);
            }

            return archSelection;
        }

        public static VerbosityLevel GetVerbosityLevel(this CommandResult commandResult)
        {
            var optionResult = commandResult.OptionResult(UninstallVerbosityOption.Name);

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
