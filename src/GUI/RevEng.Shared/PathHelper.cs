﻿using System;
using System.IO;

namespace RevEng.Common
{
    public static class PathHelper
    {
        private const string HeaderConst = @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a C# generator.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
";

        public static string Header { get; set; } = HeaderConst;

        public static string GetAbsPath(string outputPath, string fullName)
        {
            if (outputPath is null)
            {
                throw new ArgumentNullException(nameof(outputPath));
            }

            // ' The output folder can have these patterns:
            // ' 1) "\\server\folder"
            // ' 2) "drive:\folder"
            // ' 3) "..\..\folder"
            // ' 4) "folder"
            if (outputPath.StartsWith(string.Empty + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                return outputPath;
            }
            else if (outputPath.Length >= 2 && outputPath[0] == Path.VolumeSeparatorChar)
            {
                return outputPath;
            }
            else if (outputPath.IndexOf("..\\", StringComparison.OrdinalIgnoreCase) != -1)
            {
                var projectFolder = Path.GetDirectoryName(fullName);
                while (outputPath.StartsWith("..\\", StringComparison.OrdinalIgnoreCase))
                {
                    outputPath = outputPath.Substring(3);
                    projectFolder = Path.GetDirectoryName(projectFolder);
                }

                return Path.Combine(projectFolder, outputPath);
            }
            else
            {
                var projectFolder = Path.GetDirectoryName(fullName);
                return Path.Combine(projectFolder, outputPath);
            }
        }

        public static string GetNamespaceFromOutputPath(string directoryPath, string projectDir, string rootNamespace)
        {
            if (directoryPath is null)
            {
                throw new ArgumentNullException(directoryPath);
            }

            if (projectDir is null)
            {
                throw new ArgumentNullException(projectDir);
            }

            var subNamespace = SubnamespaceFromOutputPath(projectDir, directoryPath);
            return string.IsNullOrEmpty(subNamespace)
                ? rootNamespace
                : rootNamespace + "." + subNamespace;
        }

        // if outputDir is a subfolder of projectDir, then use each subfolder as a subnamespace
        // --output-dir $(projectFolder)/A/B/C
        // => "namespace $(rootnamespace).A.B.C"
        private static string SubnamespaceFromOutputPath(string projectDir, string outputDir)
        {
            if (!outputDir.StartsWith(projectDir, StringComparison.Ordinal))
            {
                return null;
            }

            var subPath = outputDir.Substring(projectDir.Length);

            return !string.IsNullOrWhiteSpace(subPath)
                ? string.Join(
                    ".",
                    subPath.Split(
                        new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries))
                : null;
        }
    }
}
