// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.DotNet.Tools.Uninstall.MacOs;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Commands;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;
using Microsoft.DotNet.Tools.Uninstall.Windows;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Configs
{
    internal static class CommandLineConfigs
    {
        public static Parser UninstallCommandParser;

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
        public static readonly string WindowsDesktopRuntimeOptionName = "windows-desktop-runtime";
        public static readonly string X64OptionName = "x64";
        public static readonly string X86OptionName = "x86";
        public static readonly string Arm64OptionName = "arm64";

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

        public static readonly Option ListArm64Option = new Option(
            $"--{Arm64OptionName}",
            LocalizableStrings.ListArm64OptionDescription);

        public static readonly Command VersionSubcommand = new Command("--version")
        {
            Description = LocalizableStrings.VersionOptionDescription
        };
        public static readonly Option YesOption = new Option(
            new[] { "--yes", "-y" },
            LocalizableStrings.YesOptionDescription);

        public static readonly Option ForceOption = new Option(
            "--force",
            RuntimeInfo.RunningOnWindows ? LocalizableStrings.ForceOptionDescriptionWindows
            : LocalizableStrings.ForceOptionDescriptionMac);

        public static readonly Option MacOSPreserveVSSdksOption = new Option(
            "--preserve-vs-for-mac-sdks", 
            LocalizableStrings.MacOSPreserveVSSdksOptionDescription);

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
            new Option($"--{HostingBundleOptionName}", LocalizableStrings.ListHostingBundleOptionDescription),
            new Option($"--{WindowsDesktopRuntimeOptionName}", LocalizableStrings.ListWindowsDesktopRuntimeOptionDescription)
        };

        public static readonly Option[] UninstallBundleTypeOptions = new Option[]
        {
            new Option($"--{SdkOptionName}", LocalizableStrings.UninstallSdkOptionDescription),
            new Option($"--{RuntimeOptionName}", LocalizableStrings.UninstallRuntimeOptionDescription),
            new Option($"--{AspNetRuntimeOptionName}", LocalizableStrings.UninstallAspNetRuntimeOptionDescription),
            new Option($"--{HostingBundleOptionName}", LocalizableStrings.UninstallHostingBundleOptionDescription),
            new Option($"--{WindowsDesktopRuntimeOptionName}", LocalizableStrings.UninstallWindowsDesktopRuntimeOptionDescription)
        };

        public static readonly Option[] ArchUninstallOptions = new Option[]
        {
            new Option($"--{X64OptionName}", LocalizableStrings.UninstallX64OptionDescription),
            new Option($"--{X86OptionName}", LocalizableStrings.UninstallX86OptionDescription),
            new Option($"--{Arm64OptionName}", LocalizableStrings.UninstallArm64OptionDescription)
        };

        public static readonly Option[] AdditionalUninstallOptions = new Option[]
        {
            VerbosityOption,
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

        public static readonly IEnumerable<Option> RemoveAuxOptions;
        public static readonly IEnumerable<Option> DryRunAuxOptions;
        public static readonly IEnumerable<Option> WhatIfAuxOptions;
        public static readonly IEnumerable<Option> ListAuxOptions;

        private static readonly Lazy<string> _assemblyVersion =
            new Lazy<string>(() =>
            {
                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                var assemblyVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                if (assemblyVersionAttribute == null)
                {
                    return assembly.GetName().Version.ToString();
                }
                else
                {
                    return assemblyVersionAttribute.InformationalVersion;
                }
            });

        static CommandLineConfigs()
        {
            DryRunCommand.AddAlias(WhatIfCommandName);

            UninstallRootCommand.AddCommand(ListCommand);
            UninstallRootCommand.AddCommand(DryRunCommand);
            UninstallRootCommand.AddCommand(RemoveCommand);
            UninstallRootCommand.AddCommand(VersionSubcommand);

            if (RuntimeInfo.RunningOnOSX)
            {
                ListCommand.AddOption(MacOSPreserveVSSdksOption);
                RemoveCommand.AddOption(MacOSPreserveVSSdksOption);
                DryRunCommand.AddOption(MacOSPreserveVSSdksOption);
            }

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
                    .Append(ListX86Option)
                    .Append(ListArm64Option);
            }
            AssignOptionsToCommand(ListCommand, ListAuxOptions);

            var bundleCollector = OperatingSystem.IsWindows() ? new RegistryQuery() as IBundleCollector : new FileSystemExplorer() as IBundleCollector;
            ListCommand.Handler = CommandHandler.Create(ExceptionHandler.HandleException((ParseResult parseResult) => ListCommandExec.Execute(bundleCollector)));
            DryRunCommand.Handler = CommandHandler.Create(ExceptionHandler.HandleException((ParseResult parseResult) => DryRunCommandExec.Execute(bundleCollector, parseResult)));
            RemoveCommand.Handler = CommandHandler.Create(ExceptionHandler.HandleException((ParseResult parseResult) => UninstallCommandExec.Execute(bundleCollector, parseResult)));
            VersionSubcommand.Handler = CommandHandler.Create(() =>
            {
                Console.WriteLine(_assemblyVersion.Value);
            });

            UninstallCommandParser = new CommandLineBuilder(UninstallRootCommand)
                .UseDefaults()
                .UseHelpBuilder(context => new UninstallHelpBuilder(context.Console))
                .Build();
        }

        public static Option GetUninstallMainOption(this CommandResult commandResult)
        {
            var specified = UninstallFilterBundlesOptions
                .Where(option => commandResult.FindResultFor(option) != null);

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

        public static BundleType GetTypeSelection(this ParseResult parseResult)
        {
            var supportedBundleTypes = SupportedBundleTypeConfigs.GetSupportedBundleTypes();

            var typeSelection = supportedBundleTypes
                .Where(type => parseResult.ValueForOption<bool>($"--{type.OptionName}"))
                .Select(type => type.Type)
                .Aggregate((BundleType)0, (orSum, next) => orSum | next);

            return typeSelection == 0 ?
                supportedBundleTypes.Select(type => type.Type).Aggregate((BundleType)0, (orSum, next) => orSum | next) :
                typeSelection;
        }

        public static BundleArch GetArchSelection(this ParseResult parseResult)
        {
            var archSelection = new[]
            {
                (OptionName: X64OptionName, Arch: BundleArch.X64),
                (OptionName: X86OptionName, Arch: BundleArch.X86),
                (OptionName: Arm64OptionName, Arch: BundleArch.Arm64)
            }
            .Where(tuple => parseResult.ValueForOption<bool>($"--{tuple.OptionName}"))
            .Select(tuple => tuple.Arch)
            .Aggregate((BundleArch)0, (orSum, next) => orSum | next);

            return archSelection == 0 ?
                archSelection = Enum.GetValues(typeof(BundleArch)).OfType<BundleArch>().Aggregate((BundleArch)0, (orSum, next) => orSum | next) :
                archSelection;
        }

        public static VerbosityLevel GetVerbosityLevel(this CommandResult commandResult)
        {
            var optionResult = commandResult.FindResultFor(VerbosityOption);

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
