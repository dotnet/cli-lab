// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Microsoft.DotNet.Tools.Bootstrapper
{
    internal static class BootstrapperCommandParser
    {
        public static Parser BootstrapParser;

        public static RootCommand BootstrapperRootCommand = new RootCommand("dotnet bootstrapper");

        public static readonly Command VersionCommand = new("--version")
        {
            Handler = CommandHandler.Create(() =>
            {
                Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                Console.WriteLine(assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? assembly.GetName().Version.ToString());
            })
        };

        public static readonly Command HelpCommand = new("--help")
        {
            Handler = CommandHandler.Create(() =>
            {
                Console.WriteLine(LocalizableStrings.BootstrapperHelp);
            })
        };

        public static readonly Command ListCommand = new("list", LocalizableStrings.ListCommandDescription)
        {
            Handler = CommandHandler.Create((ParseResult parseResult) =>
            {
                string dotnetDir = FindLocalDotnet(parseResult.ValueForOption(DotnetPath));
                if (dotnetDir is null)
                {
                    Console.WriteLine("dotnet executable not found. Ensure you execute this command from a directory with it.");
                    return;
                }

                foreach (string s in FindLocalSDKs(dotnetDir))
                {
                    Console.WriteLine(s);
                }
            })
        };

        public static readonly Command UninstallCommand = new("uninstall", LocalizableStrings.UninstallCommandDescription)
        {
            Handler = CommandHandler.Create((ParseResult parseResult) =>
            {
                string dotnetDir = FindLocalDotnet(parseResult.ValueForOption(DotnetPath));
                if (dotnetDir is null)
                {
                    Console.WriteLine("dotnet executable not found. Ensure you execute this command from a directory with it.");
                    return;
                }

                string sdkToUninstall = parseResult.ValueForArgument(UninstallArgument);

                IEnumerable<string> localSdks = FindLocalSDKs(dotnetDir).Where(sdk => sdk.EndsWith("foo"));
                if (!localSdks.Any())
                {
                    Console.WriteLine("Failed to find SDK " + "foo");
                    return;
                }

                string sdkFolder = Path.Combine(Path.GetDirectoryName(dotnetDir), "sdk");
                foreach (string sdk in localSdks)
                {
                    Console.WriteLine("Deleting SDK " + sdk);
                    Directory.Delete(sdk, recursive: true);
                }
            })            
        };

        public static readonly Command InstallCommand = new("install", LocalizableStrings.InstallCommandDescription)
        {
            Handler = CommandHandler.Create((ParseResult parseResult) =>
            {
                string dotnetDir = FindLocalDotnet(parseResult.ValueForOption(DotnetPath));
                if (dotnetDir is null)
                {
                    // install dotnet.exe
                }
            })
        };

        public static readonly Argument<string> UninstallArgument = new()
        {
            Name = "uninstall",
            Arity = ArgumentArity.ExactlyOne
        };

        public static readonly Option<string> DotnetPath = new("--dotnetPath", getDefaultValue: () => null);

        internal enum hostfxr_resolve_sdk2_result_key_t
        {
            resolved_sdk_dir = 0,
            global_json_path = 1,
        };

        [DllImport("hostfxr", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int hostfxr_get_available_sdks(string exe_dir, hostfxr_get_available_sdks_result_fn result);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        internal delegate void hostfxr_get_available_sdks_result_fn(
                hostfxr_resolve_sdk2_result_key_t key,
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
                string[] value);

        private static string[] FindLocalSDKs(string dotnetDir)
        {
            string[] resolvedPaths = null;
            int returnCode = hostfxr_get_available_sdks(exe_dir: dotnetDir, result: (key, value) => resolvedPaths = value);
            if (returnCode == 0)
            {
                return resolvedPaths ?? [];
            }
            else
            {
                throw new InvalidOperationException("Failed to find SDKs");
            }
        }

        private static string FindLocalDotnet(string pathFromSwitch)
        {
            if (pathFromSwitch is not null)
            {
                string pathToDotnet = CheckFile(pathFromSwitch) ?? pathFromSwitch;
                if (!File.Exists(pathToDotnet))
                {
                    throw new ArgumentException($"Path {pathFromSwitch} does not lead to the dotnet executable.");
                }

                return pathToDotnet;
            }

            string currentDirectory = Directory.GetCurrentDirectory();

            string path = FindAndParseGlobalJson(currentDirectory);
            if (path is not null)
            {
                return path;
            }

            return CheckFile(currentDirectory) ??
                CheckFile(Path.GetDirectoryName(currentDirectory)) ??
                Directory.GetDirectories(currentDirectory).SelectMany(subdirectory =>
                {
                    if (CheckFile(subdirectory) is string subDotnet)
                    {
                        return [subDotnet];
                    }

                    return Directory.GetDirectories(subdirectory).Select(CheckFile);
                }).FirstOrDefault(path => path is not null);
        }

        private static string FindAndParseGlobalJson(string startingDirectory)
        {
            string currentDirectory = startingDirectory;
            string globalJsonPath = Path.Combine(currentDirectory, "global.json");
            while (!File.Exists(globalJsonPath))
            {
                startingDirectory = Path.GetDirectoryName(startingDirectory);
                if (startingDirectory is null)
                {
                    return null;
                }

                globalJsonPath = Path.Combine(currentDirectory, "global.json");
            }

            JsonDocument jsonDocument = JsonDocument.Parse(globalJsonPath);
            jsonDocument.RootElement.
        }

        private static string CheckFile(string directory)
        {
            string fileToCheck = Path.Combine(directory, "dotnet.exe");
            return File.Exists(fileToCheck) ? fileToCheck :  null;
        }

        static BootstrapperCommandParser()
        {
            BootstrapperRootCommand.AddCommand(VersionCommand);
            BootstrapperRootCommand.AddCommand(HelpCommand);
            BootstrapperRootCommand.AddCommand(ListCommand);
            BootstrapperRootCommand.AddCommand(UninstallCommand);
            BootstrapperRootCommand.AddCommand(InstallCommand);

            ListCommand.AddOption(DotnetPath);
            UninstallCommand.AddOption(DotnetPath);
            InstallCommand.AddOption(DotnetPath);

            UninstallCommand.AddArgument(UninstallArgument);

            BootstrapParser = new CommandLineBuilder(BootstrapperRootCommand)
                .UseDefaults()
                .UseHelpBuilder(context => new HelpBuilder(context.Console))
                .Build();
        }
    }
}
