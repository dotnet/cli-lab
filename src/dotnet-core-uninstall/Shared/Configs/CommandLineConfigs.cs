// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
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
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? LocalizableStrings.UninstallNoOptionDescriptionWindows 
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

        public static readonly Option<bool> UninstallAllOption = new(
            "--all",
            LocalizableStrings.UninstallAllOptionDescription) {
                Arity = ArgumentArity.Zero
            };

        public static readonly Option<bool> UninstallAllLowerPatchesOption = new(
            "--all-lower-patches",
            LocalizableStrings.UninstallAllLowerPatchesOptionDescription){
                Arity = ArgumentArity.Zero
            };

        public static readonly Option<bool> UninstallAllButLatestOption = new(
            "--all-but-latest",
            LocalizableStrings.UninstallAllButLatestOptionDescription){
                Arity = ArgumentArity.Zero
            };

        public static readonly Option<IEnumerable<string>> UninstallAllButOption = new(
            "--all-but",
            LocalizableStrings.UninstallAllButOptionDescription)
        {
            ArgumentHelpName = LocalizableStrings.UninstallAllButOptionArgumentName
        };

        public static readonly Option<string> UninstallAllBelowOption = new(
            "--all-below",
            LocalizableStrings.UninstallAllBelowOptionDescription)
        {
            ArgumentHelpName = LocalizableStrings.UninstallAllBelowOptionArgumentName
        };

        public static readonly Option<bool> UninstallAllPreviewsOption = new(
            "--all-previews",
            LocalizableStrings.UninstallAllPreviewsOptionDescription){
                Arity = ArgumentArity.Zero
            };

        public static readonly Option<bool> UninstallAllPreviewsButLatestOption = new(
            "--all-previews-but-latest",
            LocalizableStrings.UninstallAllPreviewsButLatestOptionDescription){
                Arity = ArgumentArity.Zero
            };

        public static readonly Option<string> UninstallMajorMinorOption = new(
            "--major-minor",
            LocalizableStrings.UninstallMajorMinorOptionDescription)
        {
            ArgumentHelpName = LocalizableStrings.UninstallMajorMinorOptionArgumentName
        };

        public static readonly Option<VerbosityLevel> VerbosityOption = new(
            new[] { "--verbosity", "-v" },
            () => VerbosityLevel.Normal,
            LocalizableStrings.VerbosityOptionDescription)
        {
            ArgumentHelpName = LocalizableStrings.VerbosityOptionArgumentName
        };

        public static readonly Option<bool> ListX64Option = new(
            $"--{X64OptionName}",
            LocalizableStrings.ListX64OptionDescription){
                Arity = ArgumentArity.Zero
            };

        public static readonly Option<bool> ListX86Option = new(
            $"--{X86OptionName}",
            LocalizableStrings.ListX86OptionDescription){
                Arity = ArgumentArity.Zero
            };

        public static readonly Option<bool> VersionOption = new("--version")
        {
            IsHidden = true
        };

        public static readonly Option<bool> YesOption = new(
            new[] { "--yes", "-y" },
            LocalizableStrings.YesOptionDescription)
            {
                Arity = ArgumentArity.Zero
            };

        public static readonly Option<bool> ForceOption = new(
            "--force",
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? LocalizableStrings.ForceOptionDescriptionWindows
            : LocalizableStrings.ForceOptionDescriptionMac){
                Arity = ArgumentArity.Zero
            };

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

        public static readonly Option<bool>[] ListBundleTypeOptions = new Option<bool>[]
        {
            new($"--{SdkOptionName}", LocalizableStrings.ListSdkOptionDescription){
                Arity = ArgumentArity.Zero
            },
            new($"--{RuntimeOptionName}", LocalizableStrings.ListRuntimeOptionDescription){
                Arity = ArgumentArity.Zero
            },
            new($"--{AspNetRuntimeOptionName}", LocalizableStrings.ListAspNetRuntimeOptionDescription){
                Arity = ArgumentArity.Zero
            },
            new($"--{HostingBundleOptionName}", LocalizableStrings.ListHostingBundleOptionDescription){
                Arity = ArgumentArity.Zero
            }
        };

        public static readonly Option<bool>[] UninstallBundleTypeOptions = new Option<bool>[]
        {
            new($"--{SdkOptionName}", LocalizableStrings.UninstallSdkOptionDescription){
                Arity = ArgumentArity.Zero
            },
            new($"--{RuntimeOptionName}", LocalizableStrings.UninstallRuntimeOptionDescription){
                Arity = ArgumentArity.Zero
            },
            new($"--{AspNetRuntimeOptionName}", LocalizableStrings.UninstallAspNetRuntimeOptionDescription){
                Arity = ArgumentArity.Zero
            },
            new($"--{HostingBundleOptionName}", LocalizableStrings.UninstallHostingBundleOptionDescription){
                Arity = ArgumentArity.Zero
            }
        };

        public static readonly Option<bool>[] ArchUninstallOptions = new Option<bool>[]
        {
            new($"--{X64OptionName}", LocalizableStrings.UninstallX64OptionDescription){
                Arity = ArgumentArity.Zero
            },
            new($"--{X86OptionName}", LocalizableStrings.UninstallX86OptionDescription){
                Arity = ArgumentArity.Zero
            }
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

        static CommandLineConfigs() 
        {
            DryRunCommand.AddAlias(WhatIfCommandName);

            UninstallRootCommand.AddCommand(ListCommand);
            UninstallRootCommand.AddCommand(DryRunCommand);
            UninstallRootCommand.AddCommand(RemoveCommand);

            var supportedBundleTypeNames = SupportedBundleTypeConfigs.GetSupportedBundleTypes().Select(type => type.OptionName);

            var bundleTypeOptions = UninstallBundleTypeOptions
                .Where(option => supportedBundleTypeNames.Contains(option.Name));
            
            foreach(var bundleTypeOption in bundleTypeOptions){
                RemoveCommand.Add(bundleTypeOption);
                DryRunCommand.Add(bundleTypeOption);
                ListCommand.Add(bundleTypeOption);
            }
            
            foreach(var option in AdditionalUninstallOptions) {
                RemoveCommand.Add(option);
                DryRunCommand.Add(option);
            }
            RemoveCommand.Add(YesOption);

            RemoveCommand.Add(VersionArgument);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                foreach (var option in ArchUninstallOptions) {
                    RemoveCommand.Add(option);
                    DryRunCommand.Add(option);
                }
            }
            
            foreach (var option in UninstallFilterBundlesOptions) {
                RemoveCommand.Add(option);
                DryRunCommand.Add(option);
            }

            DryRunCommand.Add(VersionArgument);


            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ListCommand.Add(ListX64Option);
                ListCommand.Add(ListX86Option);
            }

            var bundleCollector = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new RegistryQuery() as IBundleCollector : new FileSystemExplorer() as IBundleCollector;
            ListCommand.SetHandler(() => ListCommandExec.Execute(bundleCollector));
            DryRunCommand.SetHandler((context) => DryRunCommandExec.Execute(context.ParseResult, bundleCollector));
            RemoveCommand.SetHandler((context) => UninstallCommandExec.Execute(context.ParseResult, bundleCollector));

            UninstallCommandParser = new CommandLineBuilder(UninstallRootCommand)
                .UseDefaults()
                .UseHelpBuilder(context => new UninstallHelpBuilder(LocalizationResources.Instance))
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
                .Select(type => (type, ListBundleTypeOptions.FirstOrDefault(o => o.HasAlias($"--{type.OptionName}"))))
                .Where(tup => tup.Item2 != null && parseResult.GetValueForOption(tup.Item2))
                .Select(tup => tup.type.Type)
                .Aggregate((BundleType)0, (orSum, next) => orSum | next);

            return typeSelection == 0 ?
                supportedBundleTypes.Select(type => type.Type).Aggregate((BundleType)0, (orSum, next) => orSum | next) :
                typeSelection;
        }

        public static BundleArch GetArchSelection(this ParseResult parseResult)
        {
            var archSelection = new[]
            {
                (Option: ListX64Option, Arch: BundleArch.X64),
                (Option: ListX86Option, Arch: BundleArch.X86)
            }
            .Where(tuple => parseResult.GetValueForOption(tuple.Option))
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

        public static readonly Argument<IEnumerable<string>> VersionArgument = new (LocalizableStrings.UninstallNoOptionArgumentName, LocalizableStrings.UninstallNoOptionArgumentDescription);
    }
}
