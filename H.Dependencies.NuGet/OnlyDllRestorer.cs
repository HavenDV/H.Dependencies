using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace H.Dependencies
{
    public static class OnlyDllRestorer
    {
        #region Public methods

        private static List<string> FrameworksPriority { get; } = new List<string>
        {
            "netstandard2.1",
            "netstandard2.0",
            "netstandard1.6",
            "netstandard1.5",
            "netstandard1.4",
            "netstandard1.3",
            "netstandard1.2",
            "netstandard1.1",
            "netstandard1.0",
        };

        public static async Task RestoreRecursiveAsync(
            string folder,
            string packageName,
            string targetFramework,
            Version targetFrameworkVersion,
            string? version = default,
            int maxDepth = 99,
            IEnumerable<string>? ignoredNames = default,
            CancellationToken cancellationToken = default)
        {
            await Restorer.RestoreRecursiveAsync(
                folder, packageName, targetFramework, targetFrameworkVersion,
                version, maxDepth, ignoredNames, cancellationToken);

            foreach (var directory in Directory.EnumerateDirectories(folder))
            {
                foreach (var framework in FrameworksPriority)
                {
                    var libFolder = Path.Combine(directory, "lib", framework);
                    if (!Directory.Exists(libFolder))
                    {
                        continue;
                    }

                    foreach (var path in Directory.EnumerateFiles(libFolder))
                    {
                        if (path == null)
                        {
                            continue;
                        }

                        var name = Path.GetFileName(path);

                        File.Copy(path, Path.Combine(folder, name));
                    }
                    Directory.Delete(directory, true);
                    break;
                }
            }
        }

        #endregion
    }
}
