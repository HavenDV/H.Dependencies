using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace H.Dependencies
{
    /// <summary>
    /// Searches full paths of dependencies from .deps.json files.
    /// </summary>
    public static class DepsJsonDependenciesSearcher
    {
        /// <summary>
        /// Searches full paths of dependencies from .deps.json files.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="nugetFolder"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static IList<string> Search(string json, string? nugetFolder = null)
        {
            json = json ?? throw new ArgumentNullException(nameof(json));
            nugetFolder ??= Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".nuget", "packages");

            var obj = JsonConvert.DeserializeObject<DepsJsonObject>(json);
            if (obj == null)
            {
                throw new ArgumentException("Invalid json: Deserialized object is null");
            }

            var packages = obj.Targets
                .Where(i => i.Value != null)
                .SelectMany(i => i.Value)
                .Where(i => i.Value?.Runtime != null)
                .ToList();

            var explicitDependencies = packages
                .SelectMany(i => 
                    i.Value?.Runtime
                        .Where(j => !j.Key.StartsWith("lib"))
                        .Select(j => (packageName: i.Key, fileName: j.Key)))
                .ToList();
            var nugetDependencies = packages
                .SelectMany(i =>
                    i.Value?.Runtime
                        .Where(j => j.Key.StartsWith("lib"))
                        .Select(j => (packageName: i.Key, fileName: j.Key)))
                .ToList();

            /*
            var runtimeDependencies = nugetDependencies
                .Select(i => Path.Combine(nugetFolder, i.packageName, "runtimes"))
                .Where(Directory.Exists)
                .SelectMany(path => Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
                .ToList();
            */
            var runtimeDependencies = nugetDependencies
                .Select(i => Path.Combine(nugetFolder, i.packageName, "runtimes"))
                .Where(Directory.Exists)
                .ToList();

            var dependencies = new List<string>();

            dependencies.AddRange(explicitDependencies
                .Select(i => i.fileName));
            dependencies.AddRange(nugetDependencies
                .Select(i => Path.Combine(nugetFolder, i.packageName, i.fileName)));
            dependencies.AddRange(runtimeDependencies);

            return dependencies;
        }
    }
}
